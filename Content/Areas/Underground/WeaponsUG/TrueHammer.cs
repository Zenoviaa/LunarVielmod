using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.WeaponsUG;

public class TrueHammer : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 13;
        Item.shoot = ModContent.ProjectileType<TrueHammerSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<TrueHammerStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Hammer;
        staminaDamageMultiplier = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankSword>(),
            material: ModContent.ItemType<MinersGold>());
    }
}


public class TrueHammerBoom : ModProjectile
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
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            for (float f = 0; f < 4; f++)
            {
                var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                smoke.initialColor = Color.DarkGray;
            }
            for (float f = 0; f < 32; f++)
            {
                Vector2 vel = -Vector2.UnitY * 8 * Main.rand.NextFloat(0.2f, 1f);
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.Torch, vel, Scale: 1.5f);

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
            FXUtil.GlowCircleBoom(Projectile.Center, Color.Yellow, Color.Orange, Color.Red);
            var boom = FXUtil.GlowCircleBoom(Projectile.Center, Color.Yellow, Color.Orange, Color.Red);
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

public class ThrowTrueHammer : ModProjectile
{
    private bool _hasBounced;
    private ref float Timer => ref Projectile.ai[0];
    private ref float HitstopTimer => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.timeLeft = 360;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
    }
    public override bool ShouldUpdatePosition()
    {
        return base.ShouldUpdatePosition() && HitstopTimer <= 0;
    }
    public override void AI()
    {
        base.AI();
        HitstopTimer--;
        if (HitstopTimer >= 1)
            return;

        Timer++;
        if(Timer == 1)
        {
            SoundStyle sounds = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg");
            sounds.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sounds, Projectile.position);
        }
        if(Timer % 8 == 0)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Torch);
        }


        if (Timer % 16 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, Vector2.Zero, Scale: 0.5f);
            sp.gravity = 0;
            sp.fast = true;
            sp.easeInFade = true;
        }
        Projectile.velocity.Y += 0.3f;
  
        Projectile.rotation += 0.05f;
        Projectile.rotation += Projectile.velocity.Length() * 0.025f;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer hammerDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Color c = hammerDrawer.color;
        for(int i = 0; i < Projectile.oldPos.Length; i++)
        {
            hammerDrawer.worldPosition = Projectile.oldPos[i];
            hammerDrawer.color = Color.Lerp(Color.White, Color.Transparent, (float)i / (float)Projectile.oldPos.Length) * 0.2f;
            hammerDrawer.rotation = Projectile.oldRot[i];
            Main.spriteBatch.Draw(hammerDrawer);
        }

        hammerDrawer.color = c;
        hammerDrawer.rotation = Projectile.rotation;
        hammerDrawer.worldPosition = Projectile.Center;
        Main.spriteBatch.Draw(hammerDrawer);

        hammerDrawer.VerticalFrame(1, 2);
        hammerDrawer.color = Color.LightGreen * ExtraMath.Osc(0.5f, 1f, speed: 16);
        Main.spriteBatch.Draw(hammerDrawer);
        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (_hasBounced)
            return;

        _hasBounced = true;
        HitstopTimer = 10;
        Projectile.velocity.X *= -0.5f;
        Projectile.velocity.Y -= 20;
        Projectile.netUpdate = true;

        Vector2 forwardVelocity = (target.Center - Projectile.Center);
        ThrustParticle thrustParticle = ThrustParticle.Spawn(Projectile.Center, forwardVelocity, Color.White);
        thrustParticle.innerColor = Color.Yellow;
        thrustParticle.bloomColor = Color.OrangeRed;
        ShakeScreenPosition.Shake = 2;

        SoundStyle fireExplosion = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireExplosion1");
        fireExplosion.PitchVariance = 0.3f;
        SoundEngine.PlaySound(fireExplosion, Projectile.position);

        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);

        for (float f = 0; f < 7; f++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Circular(16, 16), Scale: 2);

        }
        for (float f = 0; f < 32; f++)
        {
            Vector2 vel = -Vector2.UnitY.RotatedBy(f / 32f * MathHelper.TwoPi) * 2;
            Dust.NewDustPerfect(Projectile.Center, DustID.Torch, vel, Scale: 1.5f);

        }
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class TrueHammerStaminaSlash : ModProjectile
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
        if (Timer % 10 == 0)
        {
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center,
                    Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15)) + -Vector2.UnitY * 5,
                    ModContent.ProjectileType<ThrowTrueHammer>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}

public class TrueHammerSlash : BaseSwingProjectileV2
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
        return Color.Lerp(Color.White, Color.Orange, completionRatio);
    }


    private float GetTrailWidth(float interpolant)
    {
        return MathHelper.Lerp(0, 20, EasingFunction.InOutSine(interpolant));
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
            var shader = FixedRichLaserShader.Instance;
            shader.LaserColor = Color.Orange;
            shader.InnerColor = Color.DarkOrange;
            shader.OuterColor = Color.DarkGoldenrod;
           
            shader.LaserTexture = AssetManager.LaserTextures.Aura;
            shader.BloomTexture = AssetManager.LaserTextures.Lightning2;
            TrailDrawer.Draw(Main.spriteBatch, swingTrailCache, GetTrailColor, GetTrailWidth, shader);
        }
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedSwingTrail);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Player player = Main.player[Projectile.owner];
        if (_hitCount < 2)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<TrueHammerBoom>(), Projectile.damage, Projectile.knockBack, ai1: ComboProgress);
            Bounce(8);
            FXUtil.ShakeCamera(target.Center, 1024, 4);
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

        modifiers.FinalDamage += MathHelper.Lerp(0, 1f, ComboProgress);
    }
}