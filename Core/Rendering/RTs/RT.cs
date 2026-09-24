namespace Stellamod.Core.Rendering.RTs;

public static class RT
{
    public static RTContext Context(RenderTargetPool pool)
    {
        return new RTContext(pool.Request());
    }
    public static RTContext Context(RenderTargetHandle handle)
    {
        return new RTContext(handle);
    }

    public static RTUsage Clear(RenderTargetHandle handle, Color? clearColor = null)
    {
        return new RTUsage(handle, clearColor);
    }
}
