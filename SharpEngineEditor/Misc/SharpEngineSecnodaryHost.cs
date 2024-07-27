using System.Runtime.InteropServices;
using System.Windows.Interop;
using TerraFX.Interop.Windows;
using SharpEngineCore.Graphics;

namespace SharpEngineEditor.Misc
{
    public sealed class SharpEngineSecnodaryHost : HwndHost
    {
#nullable enable
        public SecondaryWindow? SecondaryWindow { get; private set; }
#nullable disable

        public event Action<SharpEngineSecnodaryHost, SecondaryWindow> OnBuildCore;
        public event Action<SharpEngineSecnodaryHost, SecondaryWindow> OnDestoryCore;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            unsafe
            {
                SecondaryWindow = new SecondaryWindow("SharpEngineSecondaryWindow", new(0, 0), new(1920, 1080), new HWND((void*)hwndParent.Handle));
            }

            OnBuildCore?.Invoke(this, SecondaryWindow);

            return new(null, SecondaryWindow.HWnd);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            SecondaryWindow.Destroy();
            OnDestoryCore(this, SecondaryWindow);
        }
    }
}