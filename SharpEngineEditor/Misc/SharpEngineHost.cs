using System.Runtime.InteropServices;
using System.Windows.Interop;
using TerraFX.Interop.Windows;

using System.Diagnostics;
using System.Reflection;
using SharpEngineCore.Graphics;
using System.Windows.Media.Media3D;

namespace SharpEngineEditor.Misc
{
    public sealed class SharpEngineHost : HwndHost
    {
        private const float WIDTH = 1920f;
        private const float HEIGHT = 1080f;

        public event Action<SharpEngineHost> OnEngineLoaded;
        public event Action<SharpEngineHost> OnEngineUnloaded;

        private SharpEngineCore.Components.MainWindow _engineWindow;
        private SharpEngineCore.Graphics.SecondaryWindow _engineSecondaryWindow;
        private Thread _engineThread;

        public Assembly EngineCoreAssembly { get; private set; }

#nullable enable
        public GameAssembly? GameAssembly { get; private set; }
#nullable disable

        private static object _engineThreadLock = new();
        private static object _engineThreadExitLock = new();
        private static bool _quitEngineThread;

#nullable enable
        public CameraObject? AssignSecondaryWindow(SecondaryWindow secondaryWindow)
        {
            lock (_engineThreadLock)
            {
                _engineSecondaryWindow = secondaryWindow;

                if (_engineSecondaryWindow == null)
                    return null;

                var viewport = new Viewport(new()
                {
                    MaxDepth = 1f,
                    Height = HEIGHT,
                    Width = WIDTH
                });

                var cameraData = new CameraConstantData()
                {
                    Position = new(-3, 0, 0, 0),
                    Rotation = new(20, 0, 0, 0),
                    Scale = new(20, 20, 1000, 1),
                    Projection = CameraInfo.Perspective,
                    Attributes = new(viewport.AspectRatio, 90, 0.03f, 1000f)
                };

                var camera = _engineWindow.InitializeSecondaryWindow(_engineSecondaryWindow, new()
                {
                    cameraTransform = cameraData,
                    viewport = viewport
                });

                return camera;
            }
        }
#nullable disable

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            EngineCoreAssembly = Assembly.GetAssembly(typeof(SharpEngineCore.Components.MainWindow));
        }

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

        private void EngineThread()
        {
            Debug.Assert(_engineThread != null);

            SharpEngineCore.Components.MainWindow window = null;
            lock (_engineThreadLock)
            {
                window = _engineWindow;
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

                    if (_engineSecondaryWindow != null)
                    {
                        while (true)
                        {
                            var result = _engineSecondaryWindow.PeekAndDispatchMessage();

                            if (result.availability == false)
                                break;

                            if (result.msg.message == WM.WM_QUIT)
                            {
                                stop = true;
                                break;
                            }
                        }
                    }


                    lock (_engineThreadExitLock)
                    {
                        if (_quitEngineThread)
                            break;
                    }

                    // other code here.
                    window.Update();
                    _engineSecondaryWindow?.Update();
                    //          //
                }
            }
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            GameAssembly = new GameAssembly("SharpEngineGame.dll");

            unsafe
            {
                _engineWindow = new SharpEngineCore.Components.MainWindow(GameAssembly,
                    "SharpEngine", new(0, 0), new((int)HEIGHT, (int)WIDTH), new HWND((void*)hwndParent.Handle));
            }
            _engineThread = new Thread(new ThreadStart(EngineThread));
            _engineThread.Start();

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