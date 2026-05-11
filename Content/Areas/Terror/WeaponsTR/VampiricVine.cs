using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.WeaponsTR;

public class VampiricVine : BaseCrossbowItem
{
    public override MagicCircle GetMagicCircle()
    {
        return new MagicCircle
        {
            color = Color.Red,
            textureAsset = AssetManager.GlowMask.MagicCircleVampiricVine
        };
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 20;
        Item.knockBack = 6;
        Item.rare = ItemRarityID.LightRed;
        staminaCost = 3;
    }

    public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        base.ShootBow(player, source, shootParams);
        float numberProjectiles = 3;
        float rotation = MathHelper.ToRadians(10);

        Vector2 velocity = shootParams.velocity * shootParams.speed * 24;
        velocity *= 3;
        velocity *= shootParams.chargeStrength;

        Vector2 position = shootParams.position;
        float bowDamage = shootParams.damage * shootParams.chargeStrength;
        for (int i = 0; i < numberProjectiles; i++)
        {
            Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
            Projectile crossShot = Projectile.NewProjectileDirect(source, position, perturbedSpeed,
                shootParams.projToShoot, (int)bowDamage, Item.knockBack, player.whoAmI);
            crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
        }
    }

    public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        base.StaminaShootBow(player, source, shootParams);

        Vector2 velocity = shootParams.velocity * shootParams.speed;
        velocity *= 3;
        velocity *= shootParams.chargeStrength;

        float bowDamage = shootParams.damage * shootParams.chargeStrength;
        float numberProjectiles = 3;
        float rotation = MathHelper.ToRadians(10);
        void Shoot()
        {
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                perturbedSpeed = perturbedSpeed.RotatedByRandom(MathHelper.ToRadians(5));
                Projectile crossShot = Projectile.NewProjectileDirect(source, shootParams.position, perturbedSpeed,
                    ModContent.ProjectileType<VampiricArrow>(), (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);
                crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
            }
        }
        FunctionRepeatHelper.Repeat(Shoot, repeats: 2, rate: 7);
 
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankBow>(),
            material: ModContent.ItemType<TerrorFragments>());
    }
}

public class VampiricArrow : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.timeLeft = 300;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
    }

    public override void AI()
    {
        Timer++;
        Projectile.velocity.Y += 0.05f;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }

    private void HealEffect()
    {
        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Suckler"), Projectile.position);
        ShakeScreenPosition.Shake = 2;
        float Speed = Main.rand.Next(4, 7);
        float offsetRandom = Main.rand.Next(0, 50);

        float spread = 45f * 0.0174f;
        double startAngle = Math.Atan2(1, 0) - spread / 2;
        double deltaAngle = spread / 8f;
        double offsetAngle;

        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
            ModContent.ProjectileType<KaBoomKaev>(), (int)(Projectile.damage * 2), 0f, Projectile.owner, 0f, 0f);

            for (int i = 0; i < 2; i++)
            {
                Owner.Heal(1);
                offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y,
                    (float)(Math.Sin(offsetAngle) * Speed), (float)(Math.Cos(offsetAngle) * Speed), ProjectileID.VampireHeal, 16, 0, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y,
                    (float)(-Math.Sin(offsetAngle) * Speed), (float)(-Math.Cos(offsetAngle) * Speed), ProjectileID.VampireHeal, 16, 0, Projectile.owner);
            }
        }

        for (float f = 0; f < 8; f++)
        {
            Vector2 vel = Main.rand.NextVector2Circular(8, 8);
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, vel, Scale: 0.5f);
            sp.outerColor = Color.Red;
        }
    }

    public float WidthFunction(float completionRatio)
    {
        float osc = VectorHelper.Osc(0.75f, 1f);
        float w = MathHelper.SmoothStep(0f, 1f, (float)Projectile.timeLeft / 30f);
        return (Projectile.width * Projectile.scale) * osc * 2 * w * MathHelper.SmoothStep(1f, 0f, completionRatio);
    }

    public Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.Red, Color.White, ExtraMath.Osc(0f, 1f, speed: 32)) * MathHelper.SmoothStep(1f, 0f, completionRatio);
    }

    public void DrawPixelated(GraphicsDevice graphicsDevice)
    {
        //Put in the points
        //This is just a straight beam that collides with tiles
        var shader = RichLaserShader.Instance;
        shader.LaserColor = Color.IndianRed;
        shader.InnerColor = Color.Red;
        shader.OuterColor = Color.DarkRed;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
        Main.spriteBatch.Draw(SpritebatchDrawer.FromProjectile(Projectile));
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        HealEffect();
    }
}