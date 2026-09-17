using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Illuria.BossesIL.EStyr;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

public class STARNADO : ModProjectile,
    IDrawToRenderTarget
{
    private LittleStarParticleManager _tornadoStreakParticlesBackingField;
    private LittleStarParticleManager TornadoStreakParticles
    {
        get
        {
            _tornadoStreakParticlesBackingField ??= new LittleStarParticleManager(300, 8, GetTrailWidth, GetTrailColor);
            return _tornadoStreakParticlesBackingField;
        }
    }
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
        Projectile.timeLeft = 300;
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

        float inTornado = Timer / 30f;
        float outTornado = (float)Projectile.timeLeft / 30f;

        inTornado = EasingFunction.InOutSine(inTornado);
        outTornado = EasingFunction.InOutSine(outTornado);
        float alpha = inTornado * outTornado;

        TornadoStreakParticles.xOvalRadius = 5;
        TornadoStreakParticles.yOvalRadius = MathHelper.Lerp(50, 300, EasingFunction.InOutSine(Timer / 150f));
        TornadoStreakParticles.minX = MathHelper.Lerp(0f, 50f, EasingFunction.InOutSine(Timer / 150f));
        TornadoStreakParticles.spinTime = 25;
        TornadoStreakParticles.rotationAxis = new Vector3(0, 1, 0.2f);
        TornadoStreakParticles.alpha = 0.08f * alpha;
        TornadoStreakParticles.topOnly = true;
        TornadoStreakParticles.osc = false;
        TornadoStreakParticles.Update(Projectile.Center);

        if (Timer % 1 == 0 && Timer < 50)
        {
            Particles.GoldenLeafTornado.Spawn(GoldenLeaf.Data.Default with {
                root = Projectile, 
                rootOffset = new Vector2(0, Main.rand.NextFloat(-128, 128)),
                timeLeft = 150,
                xRange = 64, 
                yRange = 27 });
        }
        Particles.GoldenLeafTornado.noDecrement = true;
       
    }
    private Color GetTrailColor(float completionRatio)
    {
        return Color.White;
        Color trailColor = Color.Lerp(Color.Gold, Color.DarkOrange, EasingFunction.QuadraticBump(completionRatio));
        float alpha = EasingFunction.QuadraticBump(completionRatio);
        trailColor *= alpha;
        return trailColor;
    }
    private float GetTrailWidth(float completionRatio)
    {
        return MathHelper.Lerp(0.2f, 2, EasingFunction.QuadraticBump(completionRatio));
    }

    public void DrawPixelated(GraphicsDevice graphicsDevice)
    {
        TornadoStreakParticles.Draw();
    }

    private void DrawPixelatedTornado(SpriteBatch sb, Vector2 sp)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelated, DrawLayer.OverNPCsAdditive);
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


        }
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedTornado, DrawLayer.BehindNPCsWithOutline);
        //       throw new System.NotImplementedException();
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
