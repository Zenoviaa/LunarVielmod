using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.Modules;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;

[Autoload(Side = ModSide.Client)]
public class NiiviIceArenaRenderer : ModSystem
{
    float DistanceBetween => 64;
    float Radius => 1024;
    public static float effectTimer;
    void DrawPixelatedCrystals(SpriteBatch sb, Vector2 sp)
    {
        var rand = new UnifiedRandom();
        rand.SetSeed(1337);
        float[] randValues = new float[64];
        for (var i = 0; i < randValues.Length; i++)
        {
            randValues[i] = rand.NextFloat(-0.25f, 0.25f);
        }

        float[] randScales = new float[64];
        for (var i = 0; i < randValues.Length; i++)
        {
            randScales[i] = rand.NextFloat(0.85f, 1f);
        }

        float[] randOffsets = new float[64];
        for (var i = 0; i < randOffsets.Length; i++)
        {
            randOffsets[i] = rand.NextFloat(-64, 64);
        }

        var y = ModContent.GetInstance<DomainExpansionManager>().hoverPlatformY;
        sb.EndOut(out var oldParameters);
        using var rt = RT.Context(RenderTargets.ScreenTarget);
        using (RT.Clear(rt, Color.Transparent))
        {

            var pass = AssetReferences.Effects.Niivi.IceArena.CreatePixelPass();
            pass.Parameters.time = Main.GlobalTimeWrappedHourly;
            pass.Parameters.spriteSize = AssetReferences.Assets.Textures.Illuria.NPCs.NiiviIceSpike.Asset.Size();
            pass.Parameters.noiseTexelSize = AssetReferences.Assets.NoiseTextures.CrystalTrail.Asset.Value.GetTexelSize();
            pass.Apply();
            var gDevice = sb.GraphicsDevice;
            gDevice.Register(1, AssetReferences.Assets.NoiseTextures.CrystalTrail.Asset.Value, SamplerState.LinearWrap);
            var startParamets = SpritebatchParams.InWorldAndZoomed() with
            {
                matrix = Matrix.Identity,
                blendState = CustomBlendStates.Max,
                effect = pass.Shader
            };
            sb.Begin(startParamets);
            Vector2 originPoint = SavedGenerationParameters.IlluriaTreeLocation.ToWorldCoordinates();
            originPoint.X += 1926;
            Vector2 left = originPoint + new Vector2(-Radius, 0);// originPoint.X - Radius;
            Vector2 right = originPoint + new Vector2(Radius, 0);

            var y2 = y + 200;
            var j = 0;
            for (var x = left.X; x < right.X; x += DistanceBetween)
            {
                var pos = new Vector2(x, y2);
             //   pos.X += PreGeneratedNoise.SampleRand((int)(pos.X * 0.1f), (int)(pos.Y * 0.1f)) * 128;
                pos.Y += MathF.Sin(x * 9) * 16;
                pos.X += randOffsets[j];
                pos.Y -= PreGeneratedNoise.SampleRand((int)(pos.X * 0.1f), (int)(pos.Y * 0.1f)) * 16;
                var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.Illuria.NPCs.NiiviIceSpike.Asset, pos);
                drawer.color = Color.White * ExtraMath.Osc(0.8f, 1f, speed: 2, offset: x * 0.25f + y);
                drawer.rotation = randValues[j];// ExtraMath.Osc(-0.5f, 0.5F, 0, PreGeneratedNoise.SampleRand((int)(pos.X * 0.1f), (int)(pos.Y * 0.1f)));
                drawer.scale = Vector2.One * randScales[j];
                drawer.spriteEffects = SpriteEffects.FlipVertically;
                sb.Draw(drawer);
                j++;
                j %= randValues.Length;
            }
            sb.End();
        }

        var startParamets2 = SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.Identity, blendState = BlendState.Additive };
        sb.Begin(startParamets2);
        sb.Draw(rt, Vector2.Zero, Color.White);

        sb.End();
        sb.Begin(oldParameters);

        Vector2 drawPosition = new Vector2(Main.LocalPlayer.Center.X, y);

        var bloomLine = AssetReferences.Assets.NoiseTextures.BloomLine.Asset;
        //drawPosition -= Main.screenPosition;
        drawPosition.Y += 48;
        SpritebatchDrawer pdrawer = SpritebatchDrawer.FromTextureAsset(bloomLine, drawPosition);
        pdrawer.rotation += MathHelper.PiOver2;
        pdrawer.color = Color.White * ExtraMath.Osc(0.8f, 1f, speed: 12) * 0.2F;
        pdrawer.color.A = 0;
        pdrawer.scale.Y *= 8;
        sb.Draw(pdrawer);


    }
    public override void PostDrawTiles()
    {
        base.PostDrawTiles();
        if (effectTimer <= 0)
            return;
        effectTimer--;
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedCrystals);
    }
}
