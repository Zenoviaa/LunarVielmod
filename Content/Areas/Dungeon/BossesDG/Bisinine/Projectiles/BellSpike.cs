using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Effects.Generic;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine.Projectiles;


public record struct BellSpikeDraw(Action<SpriteBatch> DrawBloomLine, Action<SpriteBatch> DrawTentacleBack, Action<SpriteBatch> DrawTentacleFront, Action<SpriteBatch> DrawGlow);

[Autoload(Side = ModSide.Client)]
public class BellSpikeRenderer : ModSystem
{
    public static readonly List<BellSpikeDraw> Draws = new List<BellSpikeDraw>();
    public override void Load()
    {
        base.Load();
        On_Main.DrawDust += DrawBellSpikes;
    }
    public override void Unload()
    {
        base.Unload();
        On_Main.DrawDust -= DrawBellSpikes;
    }
    public override void PreUpdateProjectiles()
    {
        base.PreUpdateProjectiles();
        Draws.Clear();
    }

    private void DrawBellSpikes(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (Draws.Count <= 0)
            return;

        //YIPEEEE BATCHING!!!
        //We're not drawing the tentacles individually anymore or creating massive arrays so this shouldn't lag.
        //I really need a better solution for automatically batching projectile shaders/textures
        //So we don't have to make a mod system everytime
        //A cool API might be to have decorated attributes?

        SpriteBatch spriteBatch = Main.spriteBatch;
        var worldBeginner = SpritebatchParams.InWorldAndZoomed();
        spriteBatch.Begin(worldBeginner);
        for (int i = 0; i < Draws.Count; i++)
        {
            Draws[i].DrawBloomLine(spriteBatch);
        }
        spriteBatch.End();

        var shader = BishinineTentacleShader.Instance;
        shader.Time = -Main.GlobalTimeWrappedHourly * 4;
        shader.Frequency = 8;
        shader.Amplitude = 0.1f;
        shader.BloomColor = Color.Blue;
        var beginner = SpritebatchParams.InWorldAndZoomed() with { effect = shader.Effect };

        using (SpritebatchStarter.Begin(spriteBatch, beginner))
        {
            for (int i = 0; i < Draws.Count; i++)
            {
                Draws[i].DrawTentacleBack(spriteBatch);
            }
            for (int i = 0; i < Draws.Count; i++)
            {
                Draws[i].DrawTentacleFront(spriteBatch);
            }
        }
        spriteBatch.Begin(worldBeginner);
        for (int i = 0; i < Draws.Count; i++)
        {
            Draws[i].DrawGlow(spriteBatch);
        }
        spriteBatch.End();
    }


}
public class BellSpike : ModProjectile
{
    private float _scalar;
    private float _pillarFlameScale;
    private float _randOffset;
    private float _bloomLine;
    private ref float Timer => ref Projectile.ai[0];
    private enum AIState
    {
        Telegraph,
        Stab
    }

    private AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 80;
        Projectile.hostile = true;
        Projectile.timeLeft = 180;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center - Vector2.UnitY * 252, lineWidth: 32, ref collisionPoint))
            return true;
        return false;
    }

    public override bool CanHitPlayer(Player target)
    {
        return base.CanHitPlayer(target) && Timer >= 40 && Timer < 150;
    }

    public override void AI()
    {
        base.AI();
        if (Main.netMode != NetmodeID.Server)
        {
            //Client only
            BellSpikeRenderer.Draws.Add(new BellSpikeDraw(DrawBloomLine, DrawBackTentacle, DrawFrontTentacle, DrawGlow));
        }

        float t = Timer + _randOffset;
        float scalar = 1;
        scalar *= _pillarFlameScale;
        scalar *= MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(t / 90));
        scalar *= MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(t / 180f));
        _scalar = scalar;
        _pillarFlameScale = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / 180f));
        Timer++;
        if (Timer == 1)
        {
            _randOffset = Main.rand.NextFloat(-15, 0);
            var p = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.UnitY, Color.Gray, Scale: 0.5f);

            for (float f = 0; f < 16; f++)
            {
                Vector2 velocity = -Vector2.UnitY;
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                velocity *= Main.rand.NextFloat(15, 35);     
                if (Main.rand.NextBool(8))
                {
                    FXUtil.GlowStretch(Projectile.Center, velocity);
                }
            }

        }
        if (Timer % 15 == 0)
        {
            Vector2 velocity = -Vector2.UnitY;
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
            velocity *= Main.rand.NextFloat(5, 15);

            var sparkle = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(24, 24), velocity, Scale: Main.rand.NextFloat(0.3f, 0.6f));
            sparkle.noTileCollide = true;
            sparkle.gravity = 0f;
            sparkle.dampening = 0.05f;
            sparkle.outerColor = Color.White;
            sparkle.Scale *= 0.6f;
        }

        _bloomLine = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(Timer / 30f));
    }

    private void DrawBloomLine(SpriteBatch spriteBatch)
    {
        Vector2 drawPosition = Projectile.Center - Main.screenPosition;
        Vector2 drawScale = Vector2.One;

        Texture2D bloomLineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
        Vector2 bloomLineOrigin = new Vector2(bloomLineTexture.Width / 2, bloomLineTexture.Height);
        Color glowDrawColor = Color.Lerp(Color.Gray, Color.LightBlue, ExtraMath.Osc(0f, 1f, speed: 32));
        glowDrawColor *= _bloomLine;
        glowDrawColor.A = 0;
        spriteBatch.Draw(bloomLineTexture, drawPosition, null, glowDrawColor, 0, bloomLineOrigin, drawScale * EasingFunction.InOutSine(Timer / 30f), SpriteEffects.None, 0);
    }


   private void DrawBackTentacle(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(Assets.AssetManager.LaserTextures.Aura, Projectile.Center);
        drawer.rotation = -Vector2.UnitY.ToRotation();
        drawer.LeftCenterOrigin();
        drawer.scale.Y *= 0.45f * _scalar;
        drawer.scale.X *= 2 * _scalar;
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);

        drawer.color = Color.White * 0.25f;
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);

    }
    private void DrawFrontTentacle(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(Assets.AssetManager.LaserTextures.TexturedLaser2, Projectile.Center);
        drawer.rotation = -Vector2.UnitY.ToRotation();
        drawer.LeftCenterOrigin();
        drawer.scale.Y *= 0.2f * _scalar;
        drawer.scale.X *= 1 * _scalar;
        drawer.color *= 0.4f;
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);
    }

    private void DrawGlow(SpriteBatch spriteBatch)
    {
        Texture2D voxTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 voxDrawOrigin = voxTexture.Size() / 2f;
        Color voxGlowColor = Color.Lerp(Color.Pink, Color.Lerp(Color.Pink, Color.Blue, 0.5f), ExtraMath.Osc(0f, 1f, speed: 8));
        voxGlowColor.A = 0;
        Vector2 voxDrawScale = new Vector2(4, 1) * 0.1f;
        spriteBatch.Draw(voxTexture, Projectile.Center - Main.screenPosition + new Vector2(0, 6), null, voxGlowColor
            * _scalar, 0, voxDrawOrigin, voxDrawScale * _scalar, SpriteEffects.None, 0);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
