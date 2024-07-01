using System.Runtime.InteropServices;
using System.Windows.Interop;
using TerraFX.Interop.Windows;

using SharpEngineCore.Graphics;
using System.Diagnostics;

namespace SharpEngineEditor.Misc
{
    public sealed class SharpEngineHost : HwndHost
    {
#nullable disable
        public SharpEngineCore.Components.MainWindow Window { get; private set; }
        private Thread _renderThread;
#nullable enable

        private static object _renderThreadWindowLock = new();
        private static object _renderThreadExitLock = new();
        private static bool _exitRenderThread;

        private void RenderThread(object? mainWindow)
        {
#nullable disable
            SharpEngineCore.Components.MainWindow window = null;
            lock (_renderThreadWindowLock)
            {
                window = (SharpEngineCore.Components.MainWindow)mainWindow;
            }

            while (true)
            {
                lock (_renderThreadWindowLock)
                {
                    bool stop = false;
                    // message loop
                    while (true)
                    {
                        var result = window.PeekAndDispatchMessage();

                        if (result.availability == false)
                            break;

                        if (result.msg.message == WM.WM_QUIT)
                        {
                            stop = true;
                            break;
                        }
                    }

                    if (stop)
                        break;

                    lock (_renderThreadExitLock)
                    {
                        if (_exitRenderThread)
                            break;
                    }

                    // other code here.
                    window.Update();
                    //          //
                }
            }
#nullable enable
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            unsafe
            {
                Window = new SharpEngineCore.Components.MainWindow("SharpEngine", new(0, 0), new(1920, 1080), new HWND((void*)hwndParent.Handle));
            }
            _renderThread = new Thread(new ParameterizedThreadStart(RenderThread));
            _renderThread.Start(Window);

            lock (_renderThreadWindowLock)
            {
                return new(null, Window.HWnd);
            }
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            lock(_renderThreadExitLock)
            {
                _exitRenderThread = true;
            }

            _renderThread.Join();
            Window.Destroy();
        }
    }
}
