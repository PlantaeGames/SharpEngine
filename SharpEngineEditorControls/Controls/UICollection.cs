using HandyControl.Controls;
using System.CodeDom;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SharpEngineEditorControls.Controls;

public sealed class UICollection : SimpleStackPanel
{
    public int Count => Children.Count;

    private static UICollection _empty = new();

    public static UICollection operator + (UICollection left, UIElement right)
    {
        left.Children.Add(right);

        return left;
    }

    public static UICollection operator - (UICollection left, UIElement right)
    {
        left.Children.Remove(right);

        return left;
    }

    public static UICollection operator + (UICollection left, UICollection right)
    {
        left.Children.Add(right);

        return left;
    }

    public static UICollection operator - (UICollection left, UICollection right)
    {
        left.Children.Remove(right);

        return left;
    }

    public static UICollection Empty() => _empty;
}
