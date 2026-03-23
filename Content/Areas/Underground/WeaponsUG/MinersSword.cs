using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.WeaponsUG;


public class MinersSword : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 16;
        Item.DamageType = DamageClass.Melee;
        Item.shootSpeed = 10;
        Item.shoot = ModContent.ProjectileType<MinersSwordSlash>();
        Item.autoReuse = true;
        staminaProjectileShoot = ModContent.ProjectileType<MinersSwordStaminaSlash>();
        meleeWeaponType = MeleeWeaponType.Sword;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankSword>(),
            material: ModContent.ItemType<MinersGold>());
    }
}
public class MinersSwordSlash : BaseSwingProjectileV2
{
    public bool Hit;
    public override void SetDefaults2()
    {
        base.SetDefaults2();
    }

    public override void DefineCombo()
    {
        base.DefineCombo();
        ComboBuilder comboBuilder = new ComboBuilder();
        comboBuilder.AddSwordSlash1(duration: 17)
            .AddSwordSlash2(duration: 17)
            .AddSpinningSwordSlash(duration: 30, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 720, hitCount: 2)
            .AddSpinningSwordSlash(duration: 30, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 720, hitCount: 2)
            .AddSwordSlash1(duration: 17)
            .AddSwordSlash2(duration: 17)
            .AddSpinningSwordSlash(duration: 30, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 720, hitCount: 2)
            .AddSpinningSwordSlash(duration: 30, xSwingRadius: 64, ySwingRadius: 64, swingDegrees: 720, hitCount: 2)
            .AddSwordSlash3(duration: 38, swingDegress: 720, hitCount: 3);
        comboBuilder.AddToProjectile(this);
        useAfterImage = true;
    }

    private float DefaultWidthFunction(float completionRatio)
    {
        return MathHelper.Lerp(0, 64, completionRatio) * EasingFunction.QuadraticBump(completionRatio);
    }

    private Color DefaultColorFunction(float p)
    {
        Color trailColor = Color.Lerp(Color.White, Color.LightBlue, p) * EasingFunction.QuadraticBump(p);
        return trailColor;
    }

    public override void RenderSwingTrail(ref Color lightColor, Vector2[] points)
    {
        base.RenderSwingTrail(ref lightColor, points);
        var shader = BlackFireShader.Instance;
        shader.InnerColor = Color.White * 0.15f;
        shader.OuterColor = Color.DarkGray * 0.15f;
        shader.BackColor = Color.Black * 0.15f;
        TrailDrawer.Draw(Main.spriteBatch, points, Projectile.oldRot, DefaultColorFunction, DefaultWidthFunction, shader);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!Hit && ComboIndex == 5)
        {
            FXUtil.ShakeCamera(target.Center, 1024, 4);
            FXUtil.GlowCircleBoom(target.Center,
                innerColor: Color.White,
                glowColor: Color.Black,
                outerGlowColor: Color.Black, duration: 12, baseSize: 0.24f);

            Hit = true;
        }

        for (float i = 0; i < 2; i++)
        {
            float progress = i / 4f;
            float rot = progress * MathHelper.ToRadians(360);
            rot += Main.rand.NextFloat(-0.5f, 0.5f);
            Vector2 offset = rot.ToRotationVector2() * 24;
            var particle = FXUtil.GlowCircleLongBoom(target.Center,
                innerColor: Color.White,
                glowColor: Color.LightGray,
                outerGlowColor: Color.Black,
                baseSize: Main.rand.NextFloat(0.05f, 0.1f),
                duration: Main.rand.NextFloat(5, 10));
            particle.Rotation = rot + MathHelper.ToRadians(45);
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        SoundStyle spearHit = AssetRegistry.Sounds.Melee.SpearHit1;
        spearHit.PitchVariance = 0.5f;
        SoundEngine.PlaySound(spearHit, Projectile.position);
        if (ComboIndex == 5)
        {
            modifiers.FinalDamage *= 2;
        }
    }
}


public class ThrowRock : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float HitstopTimer => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
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
        if (Timer == 1)
        {
            SoundStyle sounds = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg");
            sounds.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sounds, Projectile.position);
        }
        if (Timer % 8 == 0)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Stone);
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
        Projectile.rotation += Projectile.velocity.Length() * 0.05f;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer hammerDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Color c = hammerDrawer.color;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
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
        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        HitstopTimer = 10;
        Vector2 forwardVelocity = (target.Center - Projectile.Center);
        ThrustParticle thrustParticle = ThrustParticle.Spawn(Projectile.Center, forwardVelocity, Color.White);
        thrustParticle.innerColor = Color.White;
        thrustParticle.bloomColor = Color.DarkGray;
        ShakeModSystem.Shake = 2;

        SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, Projectile.position);

        for (float f = 0; f < 7; f++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Stone, Main.rand.NextVector2Circular(16, 16), Scale: 2);

        }
        for (float f = 0; f < 32; f++)
        {
            Vector2 vel = -Vector2.UnitY.RotatedBy(f / 32f * MathHelper.TwoPi) * 2;
            Dust.NewDustPerfect(Projectile.Center, DustID.Stone, vel, Scale: 1.5f);

        }
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class MinersSwordStaminaSlash : ModProjectile
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
                    ModContent.ProjectileType<ThrowRock>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}
