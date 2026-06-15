using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity;
using Stellamod.Content.Areas.Underground.WeaponsUG;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.WeaponsTR;

public class SkeweredAxe : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 13;
        Item.shoot = ModContent.ProjectileType<SkeweredAxeSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<SkeweredAxeStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Hammer;
        staminaDamageMultiplier = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankSword>(),
            material: ModContent.ItemType<TerrorFragments>());
    }
}


public class SkeweredAxeBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private ref float ComboProgress => ref Projectile.ai[1];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.timeLeft = 30;
        Projectile.ignoreWater = true;
    }


    public override void AI()
    {
        base.AI();
        Timer++;

        if (Timer == 1)
        {
            SoundStyle sound = AssetRegistry.Sounds.SanguineSingularity.BloodyExplosion;
            sound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sound, Projectile.position);
            if (this.OwnedByLocalClient())
            {
                for(int i = 0; i < 1; i++)
                {
                    Vector2 vel = -Vector2.UnitY * 15;
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(45));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, 
                        ModContent.ProjectileType<FriendlyBloodyBurst>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            for (float f = 0; f < 4; f++)
            {
                var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                smoke.initialColor = Color.DarkGray;
            }
            for (float f = 0; f < 32; f++)
            {
                Vector2 vel = -Vector2.UnitY * 8 * Main.rand.NextFloat(0.2f, 1f);
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.RedTorch, vel, Scale: 1.5f);

            }
            //Big ass explosion
            float num = MathHelper.Lerp(4f, 10f, ComboProgress);
            for (float f = 0; f < num; f++)
            {
                Vector2 upwardVelocity = -Vector2.UnitY * 8;
                upwardVelocity = upwardVelocity.RotatedByRandom(MathHelper.ToRadians(45));
                DustParticle.Spawn(Projectile.Center, upwardVelocity);
            }

            for (float f = 0; f < num; f++)
            {
                Vector2 upwardVelocity = -Vector2.UnitY * 2;
                upwardVelocity = upwardVelocity.RotatedByRandom(MathHelper.ToRadians(45));
                SparkleParticle.Spawn(Projectile.Center, upwardVelocity, Scale: 0.5f);
            }

            Vector2 thrustUpVelocity = -Vector2.UnitY * 4;
            ThrustParticle thrustParticle = ThrustParticle.Spawn(Projectile.Center, thrustUpVelocity, Color.White,
                Scale: MathHelper.Lerp(0.5f, 1.5f, ComboProgress));
            thrustParticle.bloomColor = Color.Red;
            FXUtil.GlowCircleBoom(Projectile.Center, Color.Red, Color.DarkRed, Color.Black);
            var boom = FXUtil.GlowCircleBoom(Projectile.Center, Color.Red, Color.DarkRed, Color.Black);
            boom.Scale *= 2;
            boom.OuterGlowColor *= 0.5f;
            boom.InnerColor *= 0.5f;
            boom.GlowColor *= 0.5f;
        }
        Projectile.width = Projectile.height = (int)(128 * ComboProgress);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class SkeweredAxeStaminaSlash : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 69;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            Owner.AddBuff(ModContent.BuffType<SkeweredAxesFury>(), 360);
        }
    }
}
public class FriendlyBloodyBurst : ScarletProjectile,
       IDrawSanguineBlood
{
    private float _trailWidth;
    public override string Texture => TextureRegistry.EmptyTexture;

    private ref float Timer => ref Projectile.ai[0];
    private ref float Version => ref Projectile.ai[1];
    public override void SetDefaults()
    {
        base.SetDefaults();
        TrailCacheLength = 80;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.extraUpdates = 1;
    }

    private void AI_Gravity()
    {
        if (Projectile.velocity.Y < 25)
        {
            Projectile.velocity.Y += 0.25f;
        }


    }

    private void AI_Homing()
    {
        Projectile.extraUpdates = 2;
        var closest = PlayerHelper.FindClosestPlayer(Projectile.position, 2048);
        if (closest != null)
        {
            if (Timer < 100)
            {
                float degreesToRotate = MathHelper.Lerp(0.1f, 6f, EasingFunction.InOutSine(Timer / 100f));
                Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center, degreesToRotate);
                Projectile.velocity = homingVelocity;
            }

        }
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        switch (Version)
        {
            case 0:
                AI_Gravity();
                break;
            case 1:
                AI_Homing();
                break;
        }
        if (Timer >= 240f)
        {
            _trailWidth = MathHelper.Lerp(_trailWidth, 0f, 0.1f);
            Projectile.velocity *= 0.9f;
            if (Projectile.velocity.Length() <= 1f)
            {
                Projectile.Kill();
            }
        }
        else
        {
            _trailWidth = MathHelper.Lerp(_trailWidth, 1f, 0.1f);
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);

    }

    private Color ColorFunction(float completionRatio)
    {
        return Color.White;
    }

    private float WidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(24, 0, completionRatio) * _trailWidth * MathF.Sin(completionRatio * 4);
    }

    public void DrawToSanguineMask(SpriteBatch spriteBatch)
    {
        var flamingTrailShader = BasicLaserAlphaShader.Instance;
        flamingTrailShader.Tiling = Vector2.One * 1;
        flamingTrailShader.LaserTexture = TrailRegistry.LightningTrail2;
        flamingTrailShader.BlendState = BlendState.AlphaBlend;
        //This just applis the shader changes
        TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, flamingTrailShader);
        //     spriteBatch.Draw(ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value, Projectile.Center - Main.screenPosition, Color.White);
    }
}
public class SkeweredAxesFury : ModBuff
{

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (Main.rand.NextBool(8))
        {
            for (float f = 0; f < 4; f++)
            {
                Vector2 vel = -Vector2.UnitY * 8 * Main.rand.NextFloat(0.2f, 1f);
                Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Circular(32, 32), DustID.RedTorch, vel, Scale: 1.5f);

            }
        }

    }
}

public class SkeweredAxeSlash : BaseSwingProjectileV2
{
    private float _hitCount;
    private bool _hit;
    private bool _playSound;
    public override void DefineCombo()
    {
        base.DefineCombo();
        SwingV2Helper.AddHammerSwingStyle(this);
        useAfterImage = true;
        hitStopTime = 4 * EXTRA_UPDATE_COUNT;
    }

    private Color GetTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Black, completionRatio);
    }


    private float GetTrailWidth(float interpolant)
    {
        return EasingFunction.QuadraticBump(interpolant) * 16;
    }

    public override void AI()
    {
        base.AI();
        if (!_playSound && Interpolant >= 0.5f)
        {

            _playSound = true;
        }
        glowColor = Color.Lerp(Color.Transparent, Color.Goldenrod, EasingFunction.QuadraticBump(Interpolant));
        growScale = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuadraticBump(Interpolant));
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        void DrawPixelatedSwingTrail(GraphicsDevice gDevice)
        {
            FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
            flamingTrailShader.OuterColor = Color.DarkRed;
            flamingTrailShader.InnerColor = Color.Red;
            flamingTrailShader.Power = 0.3f;
            flamingTrailShader.Distortion = 6;
            flamingTrailShader.Tiling = Vector2.One * 0.5f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, points, Projectile.oldRot, GetTrailColor, GetTrailWidth, flamingTrailShader);
        }
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedSwingTrail);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Player player = Main.player[Projectile.owner];
        if (_hitCount < 3)
        {
            if (Owner.HasBuff<SkeweredAxesFury>())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<SkeweredAxeBoom>(), Projectile.damage, Projectile.knockBack, ai1: ComboProgress);
            }

            Bounce(8);
            ShakeScreenPosition.Shake = 2;
            FXUtil.ShakeCamera(target.Center, 1024, 16);
            FXUtil.PunchCamera(target.Center, Projectile.velocity, 0.5f, 2, 30);
        }
        _hitCount++;
        float pitch = MathHelper.Clamp(_hitCount * 0.05f, 0f, 1f);
        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);

        base.OnHitNPC(target, hit, damageDone);

    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        if (!_hit)
        {
            modifiers.Knockback *= 0.5f;
        }
        else
        {
            modifiers.Knockback *= 2;
        }
        if (Owner.HasBuff<SkeweredAxesFury>())
            modifiers.FinalDamage *= 2;
    }
}