using System.Windows;
using System.Windows.Media;

namespace SharpEngineEditor.Extensions;

internal static class UIElementExtensions
{
    public static UIElement FindUid(this DependencyObject parent, string uid)
    {
        var count = VisualTreeHelper.GetChildrenCount(parent);
        if(count == 0)
            return null;

        for(var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, 0) as UIElement;

            if (child == null)
                continue;

            if(child.Uid == uid)
                return child;

            child = child.FindUid(uid);
            if(child != null)
                return child;
        }

        return null;
    }
}
