using Stellamod.Core.Rendering.RTs;

namespace Stellamod.Common.DialogueTowning;

/// <summary>
/// Used to draw over the dialogue box, any custom styles should be implemented here
/// </summary>
public interface IBoxStyle
{
    void Update();
    void RenderMini(RenderTargetHandle output, SpriteBatch spriteBatch, Quad<VertexPositionColorTexture> quad);
    void Render(RenderTargetHandle output, SpriteBatch spriteBatch, Quad<VertexPositionColorTexture> quad);
}
