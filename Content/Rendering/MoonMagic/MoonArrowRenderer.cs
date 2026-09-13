using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.Meshes;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Stellamod.Content.Rendering.MoonMagic;

public sealed class MoonArrowRenderer : BatchedRenderer<MoonArrowRenderer.Data>
{
    public record struct Data(Vector2[] oldPos, Vector2 position, Vector2 velocity)
    {

    }

    public override void Load()
    {
        base.Load();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += QueuePixelatedRendering;
    }

    public override void Unload()
    {
        base.Unload();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady -= QueuePixelatedRendering;
    }

    private void QueuePixelatedRendering()
    {
        if (_draws.Count <= 0)
            return;
        PixelationManager.QueueSpritebatchDrawAction(Render, DrawLayer.OverNPCs);
    }

    private void Render(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var pass = AssetReferences.Effects.Abyss.GravityCircle.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly * 1.2f;
        pass.Apply();
        using (new SpritebatchContext(spriteBatch, spriteBatch.Parameters with { effect = pass.Shader }))
        {
            foreach (var draw in _draws)
            {
                foreach (OldPosition oldPos in new OldPositionEnum(draw.oldPos))
                {
                    var asset = AssetReferences.Assets.NoiseTextures.CloudNoise3.Asset;
                    SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(asset, oldPos.position);
                    drawer.color = Color.Lerp(Color.LightCyan, Color.DarkViolet, oldPos.progress) * 0.9f;
                    drawer.color.A = (byte)((oldPos.index + 12) % 255);
                    drawer.scale *= 0.3f;
                    drawer.rotation += Main.GlobalTimeWrappedHourly + oldPos.index;
                    spriteBatch.Draw(drawer);
                }
            }
        }

        //omg guys look im using the funny texture
        var bestVfx = TextureAssets.Extra[ExtrasID.SharpTears];
        SpritebatchDrawer drawer3 = SpritebatchDrawer.FromTextureAsset(bestVfx, Vector2.Zero);
        drawer3.scale *= 1.3f;
        drawer3.color = Color.White;
        foreach (var draw in _draws)
        {
            drawer3.worldPosition = draw.position;
            drawer3.rotation = draw.velocity.ToRotation() + MathHelper.PiOver2;
            Main.spriteBatch.Draw(drawer3);
        }

        var p = AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset;
        var drawer2 = SpritebatchDrawer.FromTextureAsset(p, Vector2.Zero);
        drawer2.color = Color.White * 0.5f;
        drawer2.color.A = 0;
        drawer2.scale *= 0.15f;
        foreach (var draw in _draws)
        {
            drawer2.worldPosition = draw.position;
            Main.spriteBatch.Draw(drawer2);
        }
        _draws.Clear();
    }
}
