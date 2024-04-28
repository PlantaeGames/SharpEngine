using TerraFX.Interop.Windows;

using SharpEngineCore.Graphics;
using System.Text;

namespace SharpEngineCore.Components;

internal sealed class MainWindow : Window
{
    private Logger _logger;

    public void Update()
    {
        //_logger.LogMessage("Tick");
    }

    public MainWindow(string name, Point location, Size size) : base(name, location, size)
    {
        _logger = new ();

        _logger.LogMessage("Queried Adapters:-");
        var adapters = DXGIFactory.GetInstance().GetAdpters();
        foreach (var adapter in adapters)
        {
            var description = adapter.GetDescription().Description;
            var sb = new StringBuilder();
            foreach (var c in description)
            {
                sb.Append(c);
            }
            _logger.LogMessage(sb.ToString());
        }
    }

    protected override LRESULT WndProc(HWND hWND, uint msg, WPARAM wParam, LPARAM lParam)
    {
        return base.WndProc(hWND, msg, wParam, lParam);
    }
}
