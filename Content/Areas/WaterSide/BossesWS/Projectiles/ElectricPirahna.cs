using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.WaterSide.NPCsWS;
using Stellamod.Content.Dusts;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS.Projectiles;

public class ElectricPirahna : ModProjectile
{
    private Asset<Texture2D> _outlineTextureAsset;
    private enum AIState
    {
        Come_In,
        Slowdown,
        Anticipate,
        Go_Out
    }

    private float _trailingAlpha;
    private ref float Timer => ref Projectile.ai[0];
    private AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    private NPC Parent => Main.npc[(int)Projectile.ai[2]];
    public Vector2 LightningCircleCenter
    {
        get
        {
            if (Parent.ModNPC is LeviathanEel eel)
            {
                return eel.lightningCircleCenter;
            }

            return Projectile.Center;
        }
    }
    private float SlowdownTime => 30;
    private float AnticipationTime => 45;
    private bool _electrified;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 480;
    }

    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Come_In:
                AI_ComeIn();
                break;
            case AIState.Slowdown:
                AI_Slowdown();
                break;
            case AIState.Anticipate:
                AI_Anticipate();
                break;
            case AIState.Go_Out:
                AI_GoOut();
                break;
        }

        if (IsInsideCircle())
        {
            if (!_electrified)
            {
                string path = $"Stellamod/Assets/Sounds/Dreadmire__LightingRain{Main.rand.Next(3) + 1}";
                SoundStyle sound = new SoundStyle(path) with { PitchVariance = 0.3f };
                SoundEngine.PlaySound(sound, Projectile.position);
                FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.DarkBlue);
                for(float f = 0; f < 4f; f++)
                {
                    var z = ElectricZapParticle.Spawn(
                        Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                        Main.rand.NextVector2Circular(2, 2), Scale: Main.rand.NextFloat(0.3f, 0.6f));
                    z.Scale *= 0.5f;
                }
            }
            Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
            _electrified = true;
        }


    }

    private bool IsInsideCircle()
    {
        return Vector2.Distance(Projectile.Center, LightningCircleCenter) < LeviathanEel.DesperationCircleRadius;
    }

    private void AI_ComeIn()
    {
        Timer++;
        float distanceToCenter = Vector2.Distance(Projectile.Center, LightningCircleCenter);
        if (distanceToCenter < LeviathanEel.DesperationCircleRadius + 200)
        {
            SwitchState(AIState.Anticipate);
        }
        else
        {
            if (Main.rand.NextBool(30))
            {
                var bp = BubbleParticle.Spawn(Projectile.Center, Vector2.Zero, Scale: 0.1f);
            }

            Vector2 targetVelocity = (LightningCircleCenter - Projectile.Center).SafeNormalize(Vector2.Zero);
            targetVelocity *= MathHelper.Lerp(25, 6, EasingFunction.InOutSine(Timer / 100f));
            Projectile.velocity = Projectile.velocity.MoveTowards(targetVelocity, 2f);
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    private void AI_Slowdown()
    {
        Timer++;
        if (Projectile.velocity.Length() > 0.5f)
            Projectile.velocity *= 0.92f;

        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Timer >= SlowdownTime)
        {
            SwitchState(AIState.Slowdown);
        }
    }

    private void AI_Anticipate()
    {
        Timer++;
        Vector2 outVelocity = (LightningCircleCenter - Projectile.Center).SafeNormalize(Vector2.Zero);
        Vector2 velocity = Vector2.Lerp(outVelocity * 15, -outVelocity * 9, EasingFunction.QuadraticBump(Timer / AnticipationTime));
        Projectile.velocity = velocity;
        if (Timer >= AnticipationTime)
        {
            SwitchState(AIState.Go_Out);
        }
    }

    private void AI_GoOut()
    {
        Timer++;
        Projectile.hostile = true;

        if (_electrified)
        {
            if (Main.rand.NextBool(30))
            {
                var bp = BubbleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero, Scale: 0.1f);
            }

            if (Timer % 12 == 0)
            {
                var d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<SeafloorRockDust>());
                Main.dust[d].noGravity = true;
            }
            if (Timer % 2 == 0)
            {
                var z = DustParticle.Spawn(
                    Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                    Main.rand.NextVector2Circular(2, 2), Scale: Main.rand.NextFloat(0.3f, 0.6f));
                z.outerColor = Color.DarkBlue;
                z.Scale *= 0.5f;
            }

            if (Timer % 2 == 0)
            {
                var z = ElectricZapParticle.Spawn(
                    Projectile.Center + Main.rand.NextVector2Circular(32, 32),
                    Main.rand.NextVector2Circular(2, 2), Scale: Main.rand.NextFloat(0.3f, 0.6f));
                z.Scale *= 0.5f;
            }
            _trailingAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / 30f));
            Projectile.velocity *= 1.1f;
        }
  
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (_electrified && !IsInsideCircle())
        {
            Projectile.Kill();
        }
    }

    private void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }
    }
    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(1f, 0f, ratio) * 48;
    }

    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Transparent, ratio) * 0.66f * _trailingAlpha;
    }

    private void DrawPixelatedTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader laserShader = BasicLaserShader.Instance;
        laserShader.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.Lerp(Color.White, Color.LightBlue, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);
    }

    private void DrawFishy(SpriteBatch sb)
    {
        SpritebatchDrawer electricPirahnaDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(electricPirahnaDrawer);

        electricPirahnaDrawer.texture = _outlineTextureAsset.Value;
        electricPirahnaDrawer.color = Projectile.hostile ? Color.Red : Color.Yellow;
        Main.spriteBatch.Draw(electricPirahnaDrawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        _outlineTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Outline");
        if (_trailingAlpha > 0)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float ratio = i / (float)Projectile.oldPos.Length;
                SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
                afDrawer.color *= MathHelper.Lerp(1f, 0f, ratio) * 0.05f;
                afDrawer.color *= _trailingAlpha;
                afDrawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
                afDrawer.rotation = Projectile.oldRot[i];
                Main.spriteBatch.Draw(afDrawer);
            }
        }

        ModContent.GetInstance<HarmonicFishRenderer>().QueueDraw(DrawFishy);
        if (_electrified)
        {
            SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
            glowDrawer.color = Color.Lerp(Color.White * 0.5f, Color.White, ExtraMath.Osc(0f, 1f, speed: 16));
            glowDrawer.color.A = 0;
            glowDrawer.scale *= 0.5f;
            Main.spriteBatch.Draw(glowDrawer);
        }
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightBlue, Color.DarkBlue, duration: 30f, baseSize: 0.2f);
        for(float f =0; f < 6; f++)
        {
            Vector2 velocity = -Projectile.oldVelocity;
            velocity *= Main.rand.NextFloat(0.2f, 0.5f);
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(45));
            var dp = DustParticle.Spawn(Projectile.Center, velocity);
            dp.outerColor = Color.DarkBlue;
            dp.dampening = 0.05f;
            dp.gravity = 0;
            dp.noTileCollide = true;
        }
    }
}
