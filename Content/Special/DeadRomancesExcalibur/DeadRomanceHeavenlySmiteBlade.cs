using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class DeadRomanceHeavenlySmiteBlade : ModProjectile
{
    private float _scale;
    private Vector2 _targetCenter;
    private int Target
    {
        get => (int)Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }
    private ref float Timer => ref Projectile.ai[1];
    private enum AIState
    {
        Fall,
        Stick
    }
    private AIState State
    {
        get => (AIState)Projectile.ai[2];
        set => Projectile.ai[2] = (float)value;
    }
    private Player Owner => Main.player[Projectile.owner];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_targetCenter);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _targetCenter = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 1;
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.ignoreWater = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = 1800;
        Projectile.extraUpdates = 2;
        Projectile.light = 2f;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        Projectile.tileCollide = Projectile.Bottom.Y > Owner.Top.Y;
        if (_scale == 0f)
        {
            _scale = Projectile.scale = Main.rand.NextFloat(0.5f, 1f);
        }

        int denom = 16 * (Projectile.extraUpdates + 1);
        if (Timer % denom == 0)
        {
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            SirestiasSparkleParticle sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero);
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.Scale *= 0.25f * Projectile.scale;
            sp.fast = true;
            sp.outerColor = Color.Yellow;
        }
        denom = 8 * (Projectile.extraUpdates + 1);
        if (Timer % denom == 0)
        {
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            SirestiasSparkleParticle sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero);
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.Scale *= 0.1f * Projectile.scale;
            sp.fast = true;
            sp.outerColor = Color.Yellow;
        }


        switch (State)
        {
            case AIState.Fall:
                AI_Fall();
                break;
            case AIState.Stick:
                AI_Stick();
                break;
        }
    }

    private void SmokeParticles()
    {
        Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
        SirestiasSmokeParticle sp = SirestiasSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
        sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Blue, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f));
        sp.gravity = 0;
        sp.noTileCollide = true;
        sp.Scale *= 0.4f * Projectile.scale;
        sp.offsetRot = Main.rand.NextFloat(0f, MathHelper.TwoPi);
        sp.behindLayer = true;
        spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
        Vector2 spawnPos2 = Projectile.Center + Main.rand.NextVector2Circular(32, 32); ;
        Vector2 spawnVelocity = spawnPos2 - spawnPos;
        spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
        spawnVelocity *= 24;

        int denom = 2 * (Projectile.extraUpdates + 1);
        if (Main.rand.NextBool(denom))
        {
            /*
            Color color = new Color(41, 43, 66);
            var sp2 = SirestiasSmokeParticle2.SpawnInAlphaLayer(spawnPos + Main.rand.NextVector2Circular(32, 32), spawnVelocity * 0.02f);
            sp2.color = Color.Lerp(color, Color.White, 0.25f);
            sp2.gravity = 0;
            sp2.noTileCollide = true;
            sp2.Scale *= 0.66f * Projectile.scale;
            sp2.stretchScale2 = new Vector2(1f, 0.5f);
            sp2.offsetRot = 0;
            sp2.noRot = true;
            sp2.behindLayer = true;*/
        }
        if (State == AIState.Fall)
        {
            denom = 18 * (Projectile.extraUpdates + 1);
            if (Main.rand.NextBool(denom))
            {
                DustParticle dp = DustParticle.Spawn(spawnPos, spawnVelocity);
                dp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Red, 0.1f), Color.Black, Main.rand.NextFloat(0f, 1f));
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.fast = true;
                dp.superFast = true;
                dp.Scale *= Projectile.scale;
            }
        }
    }

    private void AI_Fall()
    {
        Timer++;
        if (Timer == 1)
        {
            SoundStyle sound = AssetRegistry.Sounds.Stars.Starsingle1;
            sound.PitchVariance = 0.5f;
            SoundEngine.PlaySound(sound, Projectile.position);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGoldenrodYellow, Color.DarkGoldenrod);
        }

        //  SmokeParticles();
        float speed = 15;
        Projectile.extraUpdates = 4;
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity.SafeNormalize(Vector2.Zero) * speed, 0.2f);
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    private void AI_Stick()
    {
        Timer++;
        if (Timer == 1)
        {

            var boom = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Goldenrod, Color.DarkGoldenrod);
            boom.Scale *= 0.3f;

            var sear = LegacyParticle.NewParticle<SearParticle>(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 38, Vector2.Zero);
            sear.innerColor = Color.Gray;
            sear.outerColor = Color.Goldenrod;
            sear.fadeToColor = Color.Black;
            sear.Scale *= 0.5f;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            ShakeScreenPosition.Shake = 2;

            for (float f = 0; f < 2f; f++)
            {
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(80, 80);
                var zap = LegacyParticle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(10) * Main.rand.NextFloat(2, 15));
                zap.innerColor = Color.Gray;
                zap.outerColor = Color.Goldenrod;
                zap.fadeToColor = Color.Black;
                zap.Scale *= Main.rand.NextFloat(0f, 0.5f);
                zap.Rotation = Main.rand.NextFloat(0f, 3f);
            }

            for (float f = 0f; f < 3; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                vel.Y -= 8;
                Vector2 pos = Projectile.Center;
                var ds = DustParticle.Spawn(pos, vel);
                ds.noTileCollide = true;
                ds.outerColor = Color.Yellow;
                ds.Scale *= 0.5f;
            }
            for (float f = 0; f < 1f; f++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 velocity = (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * 32;
                var fx = FXUtil.GlowStretch(pos, velocity);
                fx.OuterGlowColor = Color.Goldenrod;
                fx.VectorScale *= 0.5f;
            }

            float numDust = 4;
            for (float f = 0; f < numDust; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, vel, Scale: 0.5f);
                sp.outerColor = Color.Goldenrod;
                sp.noTileCollide = true;
                sp.gravity *= 0.1f;
                sp.fast = true;
            }
            var cp = CrackParticle.Spawn(Projectile.Center, Vector2.Zero);
            cp.fast = true;
            cp.color = Color.Goldenrod;
            cp.Scale *= Projectile.scale;

            SoundStyle hitSound = AssetRegistry.Sounds.Melee.ExcaliburSmallSwordrain;
            hitSound.PitchVariance = 0.4f;
            SoundEngine.PlaySound(hitSound, Projectile.position);
        }
        Projectile.extraUpdates = 0;
        Projectile.velocity *= 0.25f;
        Projectile.scale *= 0.99f;
        if (Timer >= 180)
            Projectile.Kill();
    }

    private void SwitchState(AIState state)
    {
        State = state;
        Timer = 0;
        Projectile.netUpdate = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        SwitchState(AIState.Stick);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);

    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        SwitchState(AIState.Stick);
        target.AddBuff(ModContent.BuffType<HeavenlyImpact>(), 60 * 15);
    }

    private void DrawPixelatedBlade(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.scale *= new Vector2(1f, 0.4f);
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i];
            Vector2 worldPos = pos + Projectile.Size * 0.5f;
            drawer.worldPosition = worldPos;
            drawer.rotation = Projectile.oldRot[i];
            float ratio = i / (float)Projectile.oldPos.Length;
            float ease = EasingFunction.InOutSine(ratio);
            Color bladeColor = Color.Lerp(Color.Goldenrod, Color.Black, ease);
            bladeColor.A = 0;
            drawer.color = bladeColor;
            spriteBatch.Draw(drawer);
        }

        drawer.color = Color.LightGoldenrodYellow;
        drawer.color.A = 0;
        if (State == AIState.Stick)
            drawer.scale *= new Vector2(0.8f, 1f);
        else
        {
            drawer.scale *= new Vector2(1.2F, 1f);
        }
        /*
        SpritebatchDrawer bloomLineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.ShootingStarTrail, Projectile.Center);
        bloomLineDrawer.rotation = _lineRot;
        bloomLineDrawer.scale = new Vector2(16, 1);
        bloomLineDrawer.color = Color.Goldenrod;
        bloomLineDrawer.color *= MathHelper.Lerp(0f, 1f, _lineRotLerp);
        bloomLineDrawer.color.A = 0;
        bloomLineDrawer.RightCenterOrigin();
        bloomLineDrawer.worldPosition += _lineRot.ToRotationVector2() * 128 * MathHelper.Lerp(1f, 0f, _lineRotLerp);
        spriteBatch.Draw(bloomLineDrawer);*/
        spriteBatch.Draw(drawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedBlade);
        return false;
        // return base.PreDraw(ref lightColor);
    }
}
