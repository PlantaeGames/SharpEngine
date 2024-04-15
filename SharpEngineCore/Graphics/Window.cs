using System.Configuration;

namespace SharpEngineCore.Graphics;

internal class Window : Form
{
    public Window(string name, Point position, Size size)
    {
        base.Text = name;
        base.Name = name;

        base.Location = position;

        base.ClientSize = size;
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
    }
}
