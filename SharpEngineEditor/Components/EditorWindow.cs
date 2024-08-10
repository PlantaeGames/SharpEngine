using SharpEngineCore.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraFX.Interop.Windows;

namespace SharpEngineEditor.Components;

internal sealed class EditorWindow : Window
{

    public void Start()
    { }
    public void Update()
    {}
    public void Stop()
    { }

    public EditorWindow(string name, Point location, Size size, HWND parent) :
        base(name, location, size, parent)
    {
    }
}
