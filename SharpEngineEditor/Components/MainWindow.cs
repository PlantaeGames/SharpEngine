using TerraFX.Interop.Windows;
using ImGuiNET;
using System.Diagnostics;
using Gui = ImGuiNET.ImGui;
using SharpEngineEditor.ImGui.Backend;
using SharpEngineEditor.ImGui;

namespace SharpEngineEditor.Components;

internal sealed class MainWindow : GuiWindow
{
    public MainWindow(
        string name, Point location, Size size, HWND parent) : 
        base(name, location, size, parent)
    {}

    public void Start()
    {
        SetContext();
    }
    public void Update()
    {
        Render();
    }
    public void Stop()
    { }
}
