using System.Diagnostics;
using System.Runtime.InteropServices;

using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

using SharpEngineCore.Exceptions;

namespace SharpEngineCore.Graphics;

public class Window
{
    [UnmanagedCallersOnly]
    public static LRESULT WndProcStub(HWND hWnd, uint msg, WPARAM wParam, LPARAM lPraram)
    {
        var window = (Window?)GCHandle.FromIntPtr(GetWindowLongPtrW(hWnd, GWLP.GWLP_USERDATA)).Target;
        LRESULT result = window?.WndProc(hWnd, msg, wParam, lPraram) ?? 0;

        return result;
    }

    [UnmanagedCallersOnly]
    private static LRESULT InitWndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (msg == WM.WM_NCCREATE)
        {
            unsafe
            {
                var pCreateInfo = (CREATESTRUCTW*) lParam;

                var windowHandle = GCHandle.FromIntPtr((IntPtr)pCreateInfo->lpCreateParams);
                var window = (Window?)windowHandle.Target;

                delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT> lpFnWndProc = &WndProcStub;

                SetWindowLongPtrW(hWND, GWLP.GWLP_USERDATA, GCHandle.ToIntPtr(windowHandle));
                SetWindowLongPtrW(hWND, GWLP.GWLP_WNDPROC, (IntPtr)lpFnWndProc);

                if(window != null)
                    return window.WndProc(hWND, msg, wParam, lParam);
            }
        }

        var result = DefWindowProcW(hWND, msg, wParam, lParam);
        return result;
    }

    private sealed class Class : IDisposable
    {
        private const string DEFAULT_NAME = "Window";

        public string Name { get; private set; }

        private static Class? _instance = null;
        private static object _instanceLock = new ();

        private bool _disposed = false;

        public static Class GetInstance()
        {
            lock(_instanceLock)
            {
                _instance ??= new (DEFAULT_NAME);

                return _instance;
            }
        }

        private Class(string name) 
        { 
            Name = name;

            Register();
        }

        private void Register()
        {
            NativeRegister();

            unsafe void NativeRegister()
            {
                fixed (char* pName = Name)
                {
                    var wc = new WNDCLASSEXW();
                    wc.cbSize = (uint)sizeof(WNDCLASSEXW);

                    wc.lpfnWndProc = &InitWndProc;
                    wc.lpszClassName = pName;
                    wc.hInstance = (HINSTANCE)Process.GetCurrentProcess().Handle;

                    var atom = RegisterClassExW(&wc);
                    if (atom == 0)
                    {
                        throw SharpException.GetLastWin32Exception(new SharpException("Window Register Error"));
                    }
                }
            }
        }

        private void UnRegister()
        {
            NativeUnRegister();

            unsafe void NativeUnRegister()
            {
                fixed (char* pName = Name)
                {
                    if (UnregisterClassW(pName, (HINSTANCE)Process.GetCurrentProcess().Handle) == 0u)
                    {
                        throw SharpException.GetLastWin32Exception(new SharpException("Class Unregister Error"));
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            UnRegister();

            _disposed = true;
        }

        ~Class()
        {
            Dispose();
        }
    }

    private const string DEFAULT_NAME = "Window";
    private readonly Size _defaultSize = new(800, 600);
    private readonly Point _defaultLocation = new(0, 0);

    private Class _class;
    private readonly GCHandle _pThis;

    public HWND HWnd { get; private set; }

    public (bool availability, MSG msg) PeekAndDispatchMessage()
    {
        return NativePeekAndDispatch();

        unsafe (bool availability, MSG msg) NativePeekAndDispatch()
        {
            (bool availability, MSG msg) result = (false, new MSG());

            result.availability = PeekMessageW(&result.msg, HWnd, 0u, 0u, PM.PM_REMOVE);

            if(result.msg.message == WM.WM_QUIT)
            {
                return result;
            }

            TranslateMessage(&result.msg);
            DispatchMessageW(&result.msg);

            return result;
        }
    }

    public void Destroy()
    {
        try
        {
            NativeDestroy();
        }
        catch(Exception e)
        {
            FreeHandle();
            throw new SharpException(e.Message, e);
        }

        FreeHandle();

        void NativeDestroy()
        {
            if (DestroyWindow(HWnd) == 0)
            {
                throw SharpException.GetLastWin32Exception(new SharpException("Window Destruction Error"));
            }
        }
    }

    public void Show()
    {
        _ = Toggle(true);
    }

    public void Hide()
    {
        _ = Toggle(false);
    }

    private bool Toggle(bool show = true)
    {
        return NativeToggle();

        unsafe bool NativeToggle()
        {
            var result = ShowWindow(HWnd, show ? SW.SW_SHOW : SW.SW_HIDE);
            return result;
        }
    }

    private void Create(string name, Point location, Size size)
    {
        try
        {
            NativeCreate();
        }
        catch(Exception e)
        {
            FreeHandle();
            throw new SharpException(e.Message, e);
        }

        unsafe void NativeCreate()
        {
            HWND hWnd = (HWND)0;

            fixed (char* pWindowName = name)
            {
                fixed (char* pClassName = _class.Name)
                {
                    RECT newSize = new RECT(0, 0, size.Width, size.Height);
                    AdjustWindowRectEx(&newSize, WS.WS_OVERLAPPEDWINDOW, FALSE, 0u);

                    hWnd = CreateWindowExW(0u,
                        pClassName, pWindowName, WS.WS_OVERLAPPEDWINDOW,
                        location.X, location.Y, newSize.right - newSize.left, newSize.bottom - newSize.top,
                        (HWND)null, (HMENU)null, (HINSTANCE)Process.GetCurrentProcess().Handle, (void*)GCHandle.ToIntPtr(_pThis));
                }
            }
            if (hWnd == (HWND)0)
            {
                // error here.
                throw SharpException.GetLastWin32Exception(new SharpException("Window Creation Error"));
            }

            HWnd = hWnd;

            // TODO: Handle other platforms code.
        }
    }

    protected virtual LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (msg == WM.WM_CLOSE)
        {
            PostQuitMessage(0);
            return 0;
        }

        return DefWindowProcW(hWND, msg, wParam, lParam);
    }

    public Window(string name, Point location, Size size)
    {
        _class = Class.GetInstance();
        _pThis = GCHandle.Alloc(this);

        Create(name, location, size);
    }

    public Window()
    {
        _class = Class.GetInstance();
        _pThis = GCHandle.Alloc(this);

        Create(DEFAULT_NAME, _defaultLocation, _defaultSize);
    }

    private void FreeHandle()
    {
        _pThis.Free();
    }
}