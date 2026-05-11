using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Gores;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class DeadRomanceLightningStrike : ModProjectile
{
    private Vector2 _lightningHitPos;
    private bool _calculatedStrikePoints;
    public float BeamLength;
    public Vector2[] BeamPoints;
    public float[] BeamRot;
    private float _lightningPower;
    private float _lightningTime;
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 6;
        Projectile.height = 6;
        Projectile.penetrate = -1;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.timeLeft = 60;
        Projectile.tileCollide = false;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();

        BeamLength = 500;
        if (!_calculatedStrikePoints)
        {
            List<Vector2> beamPoints = new List<Vector2>();
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            float numPoints = 80;
            float randOffset = Main.rand.NextFloat(-1f, 1f);
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + direction * BeamLength;
            for (float i = 0; i <= numPoints; i++)
            {


                float interp = i / numPoints;
                Vector2 point = Vector2.Lerp(start, end, interp);
             //   point.X += EasingFunction.QuadraticBump(interp) * 64 * randOffset;
                //if(i % 4 == 0)
                //point.X += Main.rand.Next(-16, 16);
                beamPoints.Add(point);
            }

            BeamPoints = beamPoints.ToArray();
            BeamRot = new float[BeamPoints.Length];

            _calculatedStrikePoints = true;
        }
        Timer++;
        if (Timer == 1)
        {

            _lightningPower = 0.9f;
            _lightningTime = 0;


            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 16; i++)
            {
                Vector2 dustSpawnPoint = Projectile.Center + direction * BeamLength;
                Vector2 dustVelocity = Main.rand.NextVector2Circular(8, 8);
                Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, dustVelocity, Scale: 0.5f);
                d.noGravity = true;
            }


            _lightningHitPos = Projectile.position + Projectile.velocity.SafeNormalize(Vector2.Zero) * BeamLength; // new Vector2(0, BeamLength);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(_lightningHitPos, 1024, 32);

            var part = FXUtil.GlowCircleBoom(_lightningHitPos,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Blue, duration: 12, baseSize: 0.14f);
            part.Scale *= 2;
            for (float f = 0; f < 32; f++)
            {
                Dust.NewDustPerfect(_lightningHitPos, DustID.Torch,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }


            for (float i = 0; i < 15; i++)
            {
                float rot = rot = -Projectile.velocity.ToRotation();
                rot += Main.rand.NextFloat(-0.5f, 0.5f);

                Vector2 offset = rot.ToRotationVector2() * Main.rand.NextFloat(32, 64);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(2, 15);
                var particle = FXUtil.GlowCircleDetailedBoom1(_lightningHitPos + offset,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Blue,
                    baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                    duration: Main.rand.NextFloat(5, 25));
                particle.Velocity = velocity;
                particle.Scale *= 0.35f;
                particle.Rotation = rot;
            }


            Vector2 position = _lightningHitPos;
            Vector2 lvelocity = -Projectile.velocity * 8;
            for (float f = 0; f < 8; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 2f);
                var frag = LegacyParticle.NewParticle<GlowFragmentParticle>(position, pVelocity);
                FXUtil.GlowFragmentParticle(position, pVelocity,
                    innerColor: Color.White,
                    outerColor: Color.Yellow,
                    fadeToColor: Color.Purple,
                    distortOut: true);

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<TSmokeDust>(),
                                     lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 2);
                }
                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(),
                                     lvelocity.RotatedByRandom(MathHelper.PiOver4 / 2f) * 3 * Main.rand.NextFloat(0.4f, 1f), newColor: Color.White, Scale: 0.2f);
                }
            }
            for (float f = 0; f < 8; f++)
            {
                Vector2 pVelocity = lvelocity.RotatedByRandom(MathHelper.PiOver4 / 3f);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = LegacyParticle.NewParticle<SparkParticle>(position + Main.rand.NextVector2Circular(64, 64), pVelocity);
            }

            var sear = LegacyParticle.NewParticle<SearParticle>(_lightningHitPos, Vector2.Zero);

            for (int i = 0; i < BeamPoints.Length; i++)
            {
                if (Main.rand.NextBool(16))
                {
                    Vector2 pos = BeamPoints[i];
                    pos += Main.rand.NextVector2Circular(32, 32);
                    var zap = LegacyParticle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(2, 4));

                }
            }
        }

        if (Timer == 15)
        {
            _lightningPower = 5;
        }

        if (Timer == 15)
        {
            _lightningPower = 30;
        }

        if (Timer == 30)
        {
        }

        if (Timer > 35)
        {
            _lightningPower = MathHelper.Lerp(_lightningPower, 10, 0.1f);



        }

        if (Timer == 42)
        {
            _lightningPower = 1.5f;
        }
        if (Timer == 42)
        {
            var part = FXUtil.GlowCircleBoom(_lightningHitPos,
                              innerColor: Color.White,
                              glowColor: Color.Yellow,
                              outerGlowColor: Color.Blue, duration: 6, baseSize: 0.12f);
        }
        if (Timer == 52)
        {
            _lightningPower = 2.3f;
        }
        if (Timer == 52)
        {
            var part = FXUtil.GlowCircleBoom(_lightningHitPos,
                              innerColor: Color.White,
                              glowColor: Color.Yellow,
                              outerGlowColor: Color.Blue, duration: 6, baseSize: 0.07f);
        }


        if (Timer == 58)
        {
            SoundStyle zap = SoundID.DD2_LightningBugZap;
            zap.PitchVariance = 0.3f;
            SoundEngine.PlaySound(zap, Projectile.position);

            for (float f = 0; f < 2; f++)
            {
                Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = LegacyParticle.NewParticle<ZapParticle>(_lightningHitPos + Main.rand.NextVector2Circular(64, 64), pVelocity);
                spark.Scale *= 0.5f;
                spark.Rotation = Main.rand.NextFloat(0f, 3.14f);
            }
        }
        _lightningTime -= 0.1f;

    }

    public override bool? CanDamage()
    {
        return Timer > 30;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = 0f;
        float width = Projectile.width * 0.8f;
        Vector2 start = Projectile.Center;

        Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
        Vector2 end = start + direction * BeamLength;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (BeamPoints == null)
            return false;

        SpriteBatch spriteBatch = Main.spriteBatch;
        LightningShader lightningShader = LightningShader.Instance;
        lightningShader.Time = _lightningTime;
        lightningShader.Power = _lightningPower;
        TrailDrawer.Draw(spriteBatch, BeamPoints, BeamRot, LightningColorFunction, LightningWidthFunction, lightningShader);
        if (Timer >= 30)
            TrailDrawer.Draw(spriteBatch, BeamPoints, BeamRot, LightningColorFunction, LightningWidthFunction, lightningShader);

        return false;
    }

    private float LightningWidthFunction(float completionRatio)
    {
        return MathHelper.Lerp(180, 0, completionRatio);
    }

    private Color LightningColorFunction(float completionRatio)
    {
        Color lerpColor = Color.Lerp(Color.White, Color.Blue, (Timer - 30f) / 30f);
        return Color.Lerp(Color.Transparent, lerpColor, EasingFunction.QuadraticBump(completionRatio)); ;
    }

}
public class DeadRomanceAscendedCrashBlade : ModProjectile
{
    private float _scale;
    private float _lineRot;
    private float _lineRotLerp;
    private Vector2 _targetCenter;
    private Vector2 _offset;
    private float _afterImageAlpha;
    private float _alpha;
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
        if (_scale == 0f)
        {
            _scale = Projectile.scale = Main.rand.NextFloat(0.5f, 1f);
        }

        if (Target != -1)
        {
            NPC targetNPC = Main.npc[Target];
            if (targetNPC.active)
            {
                _targetCenter = targetNPC.Center;
            }
            else
            {
                Target = -1;
            }
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
            for (int i = 0; i < 2; i++)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                SirestiasSparkleParticle sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero);
                sp.gravity = 0;
                sp.noTileCollide = true;
                sp.Scale *= 0.1f * Projectile.scale;
                sp.fast = true;
                sp.outerColor = Color.Yellow;
            }
        }


     //   SmokeParticles();
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

        spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
        Vector2 spawnPos2 = Projectile.Center + Main.rand.NextVector2Circular(32, 32); ;
        Vector2 spawnVelocity = spawnPos2 - spawnPos;
        spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
        spawnVelocity *= 24;

        int denom = 2 * (Projectile.extraUpdates + 1);
        if (Main.rand.NextBool(denom))
        {
            Color color = new Color(41, 43, 66);
            var sp2 = SirestiasSmokeParticle2.SpawnInAlphaLayer(spawnPos + Main.rand.NextVector2Circular(32, 32), spawnVelocity * 0.02f);
            sp2.color = Color.Lerp(color, Color.White, 0.25f);
            sp2.gravity = 0;
            sp2.noTileCollide = true;
            sp2.Scale *= 0.66f * Projectile.scale;
            sp2.stretchScale2 = new Vector2(1f, 0.5f);
            sp2.offsetRot = 0;
            sp2.noRot = true;
        }
        if (State == AIState.Fall)
        {
            denom = 12 * (Projectile.extraUpdates + 1);
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
        _alpha = 1f;
        _afterImageAlpha = 1f;
        Timer++;
        if (Timer == 1)
        {
            SoundStyle sound = AssetRegistry.Sounds.Stars.Starsingle1;
            sound.PitchVariance = 0.5f;
            SoundEngine.PlaySound(sound, Projectile.position);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGoldenrodYellow, Color.DarkGoldenrod);

        }
        int denom = 16 * (Projectile.extraUpdates + 1);
        if (Timer % denom == 0)
        {
            var donut = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center,
                (-Projectile.velocity.SafeNormalize(Vector2.Zero) + -Projectile.velocity.SafeNormalize(Vector2.Zero)) * 4, Color.Red);
            donut.Scale *= Projectile.scale;
            donut.fadeToColor = Color.Goldenrod;
            Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, Scale: 0.5f);
        }

        Vector2 targetVelocity = Projectile.DirectionTo(_targetCenter);
        float interp = EasingFunction.InOutExpo(Timer / 60f);
        float speed = MathHelper.Lerp(0.2f, 24, interp);
        Projectile.extraUpdates = (int)MathHelper.Lerp(0, 4, interp);
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity * speed, 0.2f);
        Projectile.rotation = Projectile.velocity.ToRotation();
        _lineRot = Projectile.rotation;
        _lineRotLerp = interp;
    }

    private void AI_Stick()
    {
        _alpha *= 0.99f;
        _afterImageAlpha *= 0.9f;
        Timer++;
        if (Timer == 1)
        {
            if(Target != -1)
            {
                _offset = (Projectile.position - Main.npc[Target].position);
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
            for (float f = 0; f < 3f; f++)
            {
                float ratio = f / 3f;
                float radians = ratio * MathHelper.TwoPi;
                float range = MathHelper.ToRadians(45);
                radians += Main.rand.NextFloat(-range, range);
                if (this.OwnedByLocalClient())
                {
                    Vector2 vel = radians.ToRotationVector2();
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel,
                        ModContent.ProjectileType<DeadRomanceLightningStrike>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            var cp = CrackParticle.Spawn(Projectile.Center, Vector2.Zero);
            cp.fast = true;
            cp.color = Color.Goldenrod;
            cp.Scale *= Projectile.scale;

            SoundStyle hitSound = AssetRegistry.Sounds.Melee.ExcaliburSmallSwordrain;
            hitSound.PitchVariance = 0.4f;
            SoundEngine.PlaySound(hitSound, Projectile.position);

            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HeavenlyCrashBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
        if(Target != -1)
        {
            NPC target = Main.npc[Target];
            if (!target.active)
                Target = -1;
            else
            {
                Projectile.position = target.position + _offset;
          
            }
        }
        Projectile.extraUpdates = 0;
        Projectile.velocity = Vector2.Zero;
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

        if(State != AIState.Stick)
        {
            SwitchState(AIState.Stick);
            target.AddBuff(ModContent.BuffType<HeavenlyImpact>(), 60 * 15);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<HolyBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
 

    }

    private void DrawPixelatedBlade(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(
            AssetManager.GlowMask.RomanceGlowSword, Projectile.Center);
        drawer.scale *= Projectile.scale * 0.75f; ;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i];
            Vector2 worldPos = pos + Projectile.Size * 0.5f;
            drawer.worldPosition = worldPos;
            drawer.rotation = Projectile.oldRot[i];
            float ratio = (float)i / (float)Projectile.oldPos.Length;
            float ease = EasingFunction.InOutSine(ratio);
            Color bladeColor = Color.Lerp(Color.Goldenrod, Color.Black, ease);
            bladeColor.A = 0;
            drawer.color = bladeColor * _afterImageAlpha;
            spriteBatch.Draw(drawer);
        }

        drawer.color = Color.LightGoldenrodYellow;
        drawer.color *= _alpha;
        drawer.color.A = 0;

        /*
        SpritebatchDrawer bloomLineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.ShootingStarTrail, Projectile.Center);
        bloomLineDrawer.rotation = _lineRot;
        bloomLineDrawer.scale = new Vector2(16, 1);
        bloomLineDrawer.color = Color.Goldenrod;
        bloomLineDrawer.color *= MathHelper.Lerp(0f, 1f, _lineRotLerp);
        bloomLineDrawer.color.A = 0;
        bloomLineDrawer.RightCenterOrigin();
        bloomLineDrawer.worldPosition += _lineRot.ToRotationVector2() * 128 * MathHelper.Lerp(1f, 0f, _lineRotLerp);
        spriteBatch.Draw(bloomLineDrawer);
        */


        drawer.worldPosition = Projectile.Center;
      spriteBatch.Draw(drawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedBlade);
        return false;
        // return base.PreDraw(ref lightColor);
    }
}


public class HeavenlyCrashBoom : ModProjectile
{
    private Vector2[] _lightningPoints;
    private float _lightningPower;
    private float _lightningTime;
    private bool _drawLightning;
    private bool _calcLightningPoints;
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private ref float ForceLightning => ref Projectile.ai[1];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
    }
    private float LightningWidthFunction(float completionRatio)
    {
        return MathHelper.Lerp(180, 0, completionRatio);
    }

    private Color LightningColorFunction(float completionRatio)
    {
        Color lerpColor = Color.Lerp(Color.Gray, Color.Blue, Timer / 30f);
        lerpColor *= MathHelper.Lerp(1f, 0f, EasingFunction.InOutExpo(Timer / 60f));
        return Color.Lerp(Color.Transparent, lerpColor, EasingFunction.QuadraticBump(completionRatio)); ;
    }

    private void DrawLightning()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        LightningShader lightningShader = LightningShader.Instance;
        lightningShader.Time = _lightningTime;
        lightningShader.Power = _lightningPower;
        lightningShader.InnerColor = Color.Gray;
        lightningShader.OuterColor = Color.DarkGray;
        TrailDrawer.Draw(spriteBatch, _lightningPoints, LightningColorFunction, LightningWidthFunction, lightningShader);

        TrailDrawer.Draw(spriteBatch, _lightningPoints, LightningColorFunction, LightningWidthFunction, lightningShader);

    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool CanHitPlayer(Player target)
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            _lightningPower = 0.9f;
            _lightningTime = 0;
            var p = Particle<ThickSmokeParticle>.Spawn(Projectile.Bottom, Vector2.Zero, Color.DarkGray);
            var sear = LegacyParticle.NewParticle<SearParticle>(Projectile.Center, Vector2.Zero);
            sear.innerColor = Color.White;
            sear.outerColor = Color.Goldenrod;
            sear.fadeToColor = Color.Black;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            ShakeScreenPosition.Shake = 2;

            for (float f = 0; f < 4f; f++)
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

            SoundStyle smashSound = AssetRegistry.Sounds.Melee.ExcaliburSwordCrashFall;
            var p3 = FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.Gray,
               glowColor: Color.Goldenrod,
               outerGlowColor: Color.DarkGoldenrod, duration: 15, baseSize: .09f);
            p3.Scale *= 4;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            _drawLightning = true;
            if (ForceLightning > 0)
            {
                _drawLightning = true;
            }

            smashSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(smashSound, Projectile.position);


            var part = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
            part.fadeToColor = Color.Black;
            part.outerColor = Color.Gray;
            part.noStretch = true;
            part.shrink = true;

            var part2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
            part2.fadeToColor = Color.Black;
            part2.outerColor = Color.Gray;
            part2.noStretch = true;
            part2.color *= 0.5f;
            for (float f = 0; f < 5; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                vel.Y -= 10;
                var d = Dust.NewDustPerfect(Projectile.Center,
                    ModContent.DustType<GlowSparkleDust>(), newColor: Color.Gray, Scale: Main.rand.NextFloat(0f, 2f), Velocity: vel);

            }
            var soundStyle = AssetRegistry.Sounds.Stars.Starsingle5;
            soundStyle.PitchVariance = 0.3f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            float boomSize = Main.rand.NextFloat(0.06f, 0.08f);
            FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.Gray,
               glowColor: Color.Goldenrod,
               outerGlowColor: Color.DarkGoldenrod, duration: 15, baseSize: boomSize * 2);
        }


        if (Timer == 15)
        {
            _lightningPower = 5;
        }

        if (Timer == 15)
        {
            _lightningPower = 30;
        }
        if (_drawLightning)
        {

            if (Timer > 35)
            {
                _lightningPower = MathHelper.Lerp(_lightningPower, 10, 0.1f);



            }

            if (Timer == 42)
            {
                _lightningPower = 1.5f;
            }
            if (Timer == 42)
            {
                var part = FXUtil.GlowCircleBoom(Projectile.Center,
                                  innerColor: Color.Goldenrod,
                                  glowColor: Color.GhostWhite,
                                  outerGlowColor: Color.DarkGoldenrod, duration: 6, baseSize: 0.12f);
            }
            if (Timer == 52)
            {
                _lightningPower = 2.3f;
            }
            if (Timer == 52)
            {
                var part = FXUtil.GlowCircleBoom(Projectile.Center,
                                  innerColor: Color.Goldenrod,
                                  glowColor: Color.GhostWhite,
                                  outerGlowColor: Color.DarkGoldenrod, duration: 6, baseSize: 0.07f);
            }


            if (Timer == 58)
            {
                SoundStyle zap = SoundID.DD2_LightningBugZap;
                zap.PitchVariance = 0.3f;
                SoundEngine.PlaySound(zap, Projectile.position);

                for (float f = 0; f < 2; f++)
                {
                    Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = LegacyParticle.NewParticle<ZapParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                    spark.Scale *= 0.5f;
                    spark.Rotation = Main.rand.NextFloat(0f, 3.14f);
                    spark.outerColor = Color.Goldenrod;
                }
            }
        }

        _lightningPower = MathHelper.Lerp(_lightningPower, 10, 0.1f);
        _lightningTime -= 0.01f;
        if (!_calcLightningPoints)
        {
            List<Vector2> beamPoints = new List<Vector2>();
            Vector2 direction = -Projectile.velocity.SafeNormalize(Vector2.Zero);
            float numPoints = 80;
            float randOffset = Main.rand.NextFloat(-1f, 1f);
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + direction * Main.rand.NextFloat(600, 984);
            end.X += Main.rand.Next(-16, -16);
            for (float i = 0; i <= numPoints; i++)
            {
                float interp = i / numPoints;
                Vector2 point = Vector2.Lerp(start, end, interp);
                point.X += EasingFunction.QuadraticBump(interp) * 64 * randOffset;
                beamPoints.Add(point);
            }

            _lightningPoints = beamPoints.ToArray();
            _calcLightningPoints = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (_drawLightning && _lightningPoints != null)
        {
            DrawLightning();
        }
        return base.PreDraw(ref lightColor);
    }
}