namespace SharpEngineCore.Graphics;

internal abstract class Resource
{
    protected Resource ()
    { }
}


internal abstract class ResourceView
{ 
    protected ResourceView()
    { }
}

internal sealed class TextureView : ResourceView
{
    public TextureView() :
        base()
    { }
}


internal interface IViewable
{

}
