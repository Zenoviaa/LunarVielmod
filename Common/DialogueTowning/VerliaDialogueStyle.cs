using Stellamod.Core;
using Stellamod.Core.Rendering.RTs;
using Terraria;
using Stellamod.Common.Particles;

namespace Stellamod.Common.DialogueTowning;

public struct VerliaDialogueStyle : IBoxStyle
{
    public void Render(RenderTargetHandle output, 
        SpriteBatch spriteBatch, Quad<VertexPositionColorTexture> quad)
    {
        if (Main.GameUpdateCount % 2 == 0 && Main.hasFocus)
        {

            var color = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 0.5f)) * 0.25f;
            Particles.Particles.StarDonut.Spawn(new()
            {
                position = new Vector2(Main.rand.Next(0, 1920), Main.rand.Next(0, 1080)),
                color =  color,
                timeLeft = 120
            });
        }
        var topLeft = quad.vertices[0].Position.XY();
        var topRight = quad.vertices[1].Position.XY();
        var bottomRight = quad.vertices[3].Position.XY();
        var rect = ExtraMath.CreateRectangle(topLeft, bottomRight);

        rect.Width += 242;
        rect.Height += 48;
        rect.Y += 2;
        rect = rect.CenterPad(-4);
        using (RT.Clear(output, Color.Transparent))
        {
            var panelPass = AssetReferences.Effects.Dialogue.VerliaPanel.CreatePixelPass();
            panelPass.Parameters.time = -Main.GlobalTimeWrappedHourly;
            panelPass.Parameters.backSmokeColor = new Color(18, 21, 59).ToVector4();
            panelPass.Parameters.frontSmokeColor = new Color(69, 59, 117).ToVector4();
            panelPass.Parameters.starsSampler = new()
            {
                Sampler = SamplerState.PointWrap,
                Texture = AssetReferences.Assets.NoiseTextures.StarNoise2.Asset.Value
            };

            panelPass.Apply();
            using(spriteBatch.Ctx(SB.InWorldUnscaled with { effect = panelPass.Shader }))
            {
                var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.NoiseTextures.StarSmoke.Asset, Vector2.Zero);
                var rect2 = new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2);
                drawer.dstRect = rect2;
                drawer.color = Color.White;
                drawer.drawOrigin = Vector2.Zero;
                spriteBatch.Draw(drawer);
            }
            using (spriteBatch.Ctx(SB.InWorldUnscaled))
            {
                Particles.Particles.StarDonut.Draw(spriteBatch, Main.screenPosition);
            }

            using (spriteBatch.Ctx(SB.InWorldUnscaled with { blendState = BlendState.Additive }))
            {

                var sunOutlineColor = Color.White;
                DrawUtilities.DrawOutlinedRectangle(spriteBatch, rect, sunOutlineColor, 16);
            }
        }
    }

    public void RenderMini(RenderTargetHandle output, 
        SpriteBatch spriteBatch, Quad<VertexPositionColorTexture> quad)
    {

    }

    public void Update()
    {

    }
}
