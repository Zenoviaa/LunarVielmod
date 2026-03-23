using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Buffers;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower.WeaponsMT;


public class HarmonicBlasphemy : BaseGun
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
        Item.damage = 16;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 56;
        Item.height = 56;
        Item.useTime = 12;
        Item.useAnimation = 12;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = 100000;
        Item.rare = ItemRarityID.Orange;
        Item.UseSound = SoundID.Item11;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<HarmonicBlasphemyBomb>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
    }


    public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HarmonicBlasphemyBomb>(), damage, knockback, player.whoAmI, ai0: remainingAmmo);
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
        fireParams.maxAmmo = 24;
        fireParams.reloadWindow = 150;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankGun>(), 
            material: ModContent.ItemType<PearlescentScrap>());
    }
}

public class HarmonicBlasphemyBoom : ModProjectile
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            PixelPrimitiveCircleFactory.CreateMoonBoom(Projectile.Center);
            for(float f = 0; f < 10; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(16, 16), vel, Scale: 0.5f);
                sp.flickering = true;
                sp.noTileCollide = true;
                sp.gravity = 0;
                sp.outerColor = Color.Blue;
                sp.fast = true;
            }
            for (float f = 0; f < 10; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(10, 10);
                var sp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(16, 16), vel, Scale: 0.5f);
                sp.outerColor = Color.Blue;
            }

            string path = $"Stellamod/Assets/Sounds/Crysalizer1";
            SoundStyle sound = new SoundStyle(path);
            sound.PitchVariance = 0.5f;
            sound.Volume = 0.3f;
            SoundEngine.PlaySound(sound, Projectile.position);
            ShakeModSystem.Shake = 2;
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Blue, Color.DarkBlue);
            var boom = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Blue, Color.DarkBlue);
            boom.Scale *= 2;
            boom.OuterGlowColor *= 0.6f;
            boom.GlowColor *= 0.6f;
            boom.InnerColor *= 0.6f;
        }
    }
}

public class HarmonicBlasphemyBomb : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
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
        Projectile.localNPCHitCooldown = 30;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater=true;
        Projectile.friendly = true;
        Projectile.timeLeft = 30;
    }


    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer % 8 == 0)
        {
           SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Vector2.Zero, Scale: 0.5f);
            sp.outerColor = Color.Blue;
            sp.noTileCollide = true;
            sp.gravity = 0;
            sp.fast = true;
        }
        float inScale = EasingFunction.InOutSine(Timer / 30f);
        float outScale = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        Projectile.scale = MathHelper.Lerp(1f, 0, outScale) * 0.5f;
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
        for(int i = 0; i < Projectile.oldPos.Length; i+=2)
        {
            float interp = (float)(i + 1) / (float)Projectile.oldPos.Length;
            float ease = EasingFunction.InOutSine(interp);
            Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            afterImageDrawer.worldPosition = drawPosition;
            afterImageDrawer.color = Color.Lerp(Color.White, Color.Blue, ease) * 0.25f;
            afterImageDrawer.color.A = 0;
            afterImageDrawer.scale = Vector2.Lerp(Vector2.One, Vector2.Zero, ease) * 2;
            sb.Draw(afterImageDrawer);
        }

        Vector2 drawPos = Projectile.Center - screenPos;
        Texture2D glowMask = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 glowDrawOrigin = glowMask.Size() / 2f;
        Color glowColor = Color.Lerp(Color.LightBlue, Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 8));
        glowColor.A = 0;
        sb.Draw(glowMask, drawPos, null, glowColor, 0, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.9f, 1.2f, speed: 8) * 0.6f, SpriteEffects.None, 0);
        // spriteBatch.RestartDefaults();


        glowMask = AssetManager.GlowMask.SpiralVortex.Value;
        glowDrawOrigin = glowMask.Size() / 2f;
        glowColor = Color.DarkBlue;
        glowColor.A = 0;
        sb.Draw(glowMask, drawPos, null, glowColor, Main.GlobalTimeWrappedHourly * 8, glowDrawOrigin, Projectile.scale * ExtraMath.Osc(0.99f, 1.01f, speed: 8) * 1f, SpriteEffects.None, 0);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedFlash);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<HarmonicBlasphemyBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}