using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Gun;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.WeaponsUG;


public class RingShotgun : BaseGun
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }


    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-4, 0);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 9;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 56;
        Item.height = 56;
        Item.useTime = 36;
        Item.useAnimation = 36;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = 100000;
        Item.rare = ItemRarityID.Orange;
        Item.UseSound = SoundID.Item11;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<RingShotgunRing>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
    }


    public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        for(int i = 0; i < 5; i++)
        {
            Vector2 p = velocity;
            p = p.RotatedByRandom(MathHelper.ToRadians(35));
            p *= Main.rand.NextFloat(0.66f, 1f);
            Projectile.NewProjectile(source, position, p, ModContent.ProjectileType<RingShotgunRing>(), damage, knockback, player.whoAmI, ai0: remainingAmmo, ai2:  30 + i * 6);
        }
        return false;
    }
    public override void ShootEffects(Vector2 position, Vector2 velocity)
    {
        int rand = Main.rand.Next(0, 3);
        SoundStyle shootSound;
        int Sound = Main.rand.Next(1, 3);
        if (Sound == 1)
        {
            shootSound = new SoundStyle("Stellamod/Assets/Sounds/HarmonicBlasphemy2");
        }
        else
        {
            shootSound = new SoundStyle("Stellamod/Assets/Sounds/HarmonicBlasphemy2");
        }

        shootSound.PitchVariance = 0.3f;
        shootSound.Volume = 0.5f;
        SoundEngine.PlaySound(shootSound, position);
        FXUtil.GlowCircleBoom(position, Color.White, Color.LightBlue, Color.DarkBlue, baseSize: 0.03f, duration: 15);

        for (float f = 0; f < 2; f++)
        {
            float rot = f / 8f;
            rot += Main.rand.NextFloat(-0.5f, 0.5f);
            var p = LegacyParticle.NewParticle<ImpactParticle>(position, velocity.RotatedByRandom(0.7f));
            p.fast = true;
            p.color = Color.DarkBlue;
        }

        for (float f = 0; f < 3; f++)
        {
            var dp = Particle<DustParticle>.Spawn(position, velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(0.25f, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 2f));
            dp.gravity = 0.02f;
            dp.outerColor = Color.Blue;
            dp.dampening = 0.1f;
        }
    }

    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 8;
        fireParams.reloadWindow = 150;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankGun>(),
            material: ModContent.ItemType<MinersGold>());
    }
}

public class ClockworkBoomer : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.friendly = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
      
            FXUtil.ShakeCamera(Projectile.Center, 1024, 2);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            for (float f = 0; f < 32; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            SoundStyle morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/MorrowExp");
            morrowExp.PitchVariance = 0.3f;
            SoundEngine.PlaySound(morrowExp, Projectile.position);

            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Turquoise,
                outerGlowColor: Color.Black, duration: 25, baseSize: Main.rand.NextFloat(0.12f, 0.24f));

            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Aquamarine,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }

        if (Timer == 10)
        {
            PixelPrimitiveCircleFactory.CreateMoonBoom(Projectile.Center);
            for(float f = 0; f < 7; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                var sp = SparkleParticle.Spawn(Projectile.Center, vel, Scale: 0.5f);
                sp.outerColor = Color.Turquoise;
                sp.fast = true;
                sp.noTileCollide = true;
                sp.gravity = 0;
            }

            float damage = Projectile.damage;
            damage *= 0.5f;
            
            /*
            if (Main.myPlayer == Projectile.owner)
            {
                var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<KaBoomMagic2>(), (int)damage, Projectile.knockBack, Projectile.owner);
            }*/


            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundStyle s = new SoundStyle("Stellamod/Assets/Sounds/ClockworkCity1");
                s.Volume = 0.5f;

                SoundEngine.PlaySound(s, Projectile.position);
            }
            else
            {
                SoundStyle s = new SoundStyle("Stellamod/Assets/Sounds/ClockworkCity2");
                s.Volume = 0.5f;
                SoundEngine.PlaySound(s, Projectile.position);
            }
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            Projectile.Kill();
        }
    }

}

public class RingShotgunRing : ModProjectile
{
    private ref float CountDown => ref Projectile.ai[2];
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.localNPCHitCooldown = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.friendly = true;
        Projectile.timeLeft = 45;
    }


    public override void AI()
    {
        base.AI();
        CountDown--;
        if (CountDown <= 0)
            Projectile.Kill();
        Timer++;
        if (Timer % 8 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero, Scale: 0.5f);
            sp.outerColor = Color.Gold;
            sp.noTileCollide = true;
            sp.gravity = 0;
            sp.fast = true;
        }
        float inScale = EasingFunction.InOutSine(Timer / 30f);
        float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        Projectile.scale = MathHelper.Lerp(1f, 0.5f, outScale);
        if(Timer > 18)
            Projectile.velocity *= 0.95f;
    }

    private void DrawPixelatedFlash(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer glintDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.ShootingStarGlint, Projectile.Center);
        glintDrawer.color = Color.White;
        glintDrawer.color *= ExtraMath.Osc(0f, 1f, speed: 16);
        glintDrawer.color.A = 0;
        glintDrawer.scale *= 2;
        sb.Draw(glintDrawer);

        SpritebatchDrawer afterImageDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.ShootingStarGlint, Projectile.Center);
        for (int i = 0; i < Projectile.oldPos.Length; i += 2)
        {
            float interp = (float)(i + 1) / (float)Projectile.oldPos.Length;
            float ease = EasingFunction.InOutSine(interp);
            Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            afterImageDrawer.worldPosition = drawPosition;
            afterImageDrawer.color = Color.Lerp(Color.White, Color.Goldenrod, ease) * 0.25f;
            afterImageDrawer.color.A = 0;
            afterImageDrawer.scale = Vector2.Lerp(Vector2.One, Vector2.Zero, ease) * 2;
            sb.Draw(afterImageDrawer);
        }

    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedFlash);
        SpritebatchDrawer ringDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(ringDrawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<ClockworkBoomer>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}