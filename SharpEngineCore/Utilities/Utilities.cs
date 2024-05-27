using System.Diagnostics;

using SharpEngineCore.Graphics;

namespace SharpEngineCore.Utilities;

internal sealed class Utilities
{
    public static FColor4 CubeFaceIndexToAngle(int index)
    {
        var result = new FColor4(0, 0, 0, 0);

        switch (index)
        {
            case 0:
                result = new(0, 0, 0, 0);
                break;
            case 1:
                result = new(0, 90, 0, 0);
                break;
            case 2:
                result = new(0, 180, 0, 0);
                break;
            case 3:
                result = new(0, 270, 0, 0);
                break;
            case 4:
                result = new(90, 0, 0, 0);
                break;
            case 5:
                result = new(-90, 0, 0, 0);
                break;
            default:
                Debug.Assert(false,
                    "Face index passed cube faces.");
                break;
        }

        return result;
    }
}