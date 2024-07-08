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
        public event Action<SharpEngineHost> OnEngineLoaded;
        public event Action<SharpEngineHost> OnEngineUnloaded;

        private SharpEngineCore.Components.MainWindow _engineWindow;
        private Thread _engineThread;
#nullable enable

        private static object _engineThreadLock = new();
        private static object _engineThreadExitLock = new();
        private static bool _quitEngineThread;

        public void ENGINE_ACTION_CALL(Action action)
        {
            Debug.Assert(action != null);

            lock(_engineThreadLock)
            {
                action.Invoke();
            }
        }

        public T ENGINE_FUNC_CALL<T>(Func<T> function)
        {
            Debug.Assert(function != null);

            lock (_engineThreadLock)
            {
                return function.Invoke();
            }
        }

        private void EngineThread(object? _engineWindow)
        {
            Debug.Assert(_engineThread != null);

#nullable disable
            SharpEngineCore.Components.MainWindow window = null;
            lock (_engineThreadLock)
            {
                window = (SharpEngineCore.Components.MainWindow)_engineWindow;
            }

            while (true)
            {
                lock (_engineThreadLock)
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

                    lock (_engineThreadExitLock)
                    {
                        if (_quitEngineThread)
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
            var game = new Game("SharpEngineGame.dll");

            unsafe
            {
                _engineWindow = new SharpEngineCore.Components.MainWindow(game,
                    "SharpEngine", new(0, 0), new(1920, 1080), new HWND((void*)hwndParent.Handle));
            }
            _engineThread = new Thread(new ParameterizedThreadStart(EngineThread));
            _engineThread.Start(_engineWindow);

            OnEngineLoaded?.Invoke(this);

            lock (_engineThreadLock)
            {
                return new(null, _engineWindow.HWnd);
            }
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            lock(_engineThreadExitLock)
            {
                _quitEngineThread = true;
            }

            _engineThread.Join();
            _engineWindow.Destroy();

            OnEngineUnloaded?.Invoke(this);
        }
    }
}
