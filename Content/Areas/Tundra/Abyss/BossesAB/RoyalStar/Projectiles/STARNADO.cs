using Stellamod.Common.Particles;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

public class STARNADO : ModProjectile,
    IDrawToRenderTarget
{
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private ref float Timer => ref Projectile.ai[1];
    private float OutEasing
    {
        get
        {
            return EasingFunction.InOutSine(Projectile.timeLeft / 60f);
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 80;
        Projectile.height = 96;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
        Projectile.light = 0.7f;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (!Parent.active)
        {
            if (Projectile.timeLeft > 60)
            {
                Projectile.timeLeft = 60;
            }
        }
        else
        {
            Projectile.Center = Parent.Center;

        }

        if (Main.rand.NextBool(8))
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(154, 154);
            var sp = SparkleParticle.Spawn(pos, Vector2.Zero);
            sp.gravity = 0;
            sp.innerColor = Color.LightGoldenrodYellow;
            sp.outerColor = Color.DarkOrange;
            sp.dampening = 0.05f;
            sp.Scale *= 0.34f;
            sp.fast = true;
        }

        if(Timer % 2 == 0)
        {
            Vector2 pos = Projectile.Center;
            pos.Y += Main.rand.NextFloat(-128, 128);
            Particles.GoldenLeaf.Spawn(GoldenLeaf.Data.Default with { rootPosition = pos, timeLeft = 150});
        }
        for (int i = 0; i < 2; i++)
        {

        }
    }

    private void DrawPixelatedTornado(SpriteBatch sb, Vector2 sp)
    {
        var pass = AssetReferences.Effects.Generic.MysteriousWind.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly * 0.09f;
        pass.Parameters.resolution = new Vector2(Main.screenWidth, Main.screenHeight);
        HlslSampler noiseSampler = new();
        noiseSampler.Sampler = SamplerState.LinearWrap;
        noiseSampler.Texture = AssetReferences.Assets.Noise.FlameVortexNoise.Asset.Value;
        pass.Parameters.noiseSampler = noiseSampler;

        pass.Apply();
        using (new SpritebatchContext(sb, sb.Parameters with
        {
            effect = pass.Shader,
            blendState = CustomBlendStates.Max
        }))
        {
            SpritebatchDrawer drawer2 = SpritebatchDrawer.FromProjectile(Projectile);
            drawer2.color = Color.Orange;

            drawer2.CenterOrigin();
            drawer2.scale *= 0.65f;
            drawer2.worldPosition -= new Vector2(0, 66);
            //drawer2.scale *= 2f;

            float ease2 = Timer / 144;
            ease2 = EasingFunction.InOutExpo(ease2);
            drawer2.rotation = Parent.rotation;
            drawer2.scale *= MathHelper.Lerp(1.5f, 1f, ease2);
            drawer2.scale *= MathHelper.Lerp(1.5f, 1f, OutEasing);
            drawer2.color = Color.Lerp(Color.Transparent, drawer2.color, EasingFunction.InSine(Timer / 50f));
            drawer2.color *= MathHelper.Lerp(0f, 1f, OutEasing);
            drawer2.color *= 0.85f;
            sb.Draw(drawer2);

            drawer2.spriteEffects = SpriteEffects.FlipHorizontally;
            drawer2.worldPosition.Y -= 64;
            drawer2.color *= 0.86f;
            drawer2.scale *= 1.2f;
            sb.Draw(drawer2);

            drawer2.spriteEffects = SpriteEffects.None;
            drawer2.worldPosition.Y -= 64;
            drawer2.color *= 0.86f;
            drawer2.scale *= 1.2f;
            sb.Draw(drawer2);

            drawer2.spriteEffects = SpriteEffects.FlipHorizontally;
            drawer2.color *= 0.56f;
            drawer2.scale *= 2f;
            sb.Draw(drawer2);
        }
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedTornado);
        //       throw new System.NotImplementedException();
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
