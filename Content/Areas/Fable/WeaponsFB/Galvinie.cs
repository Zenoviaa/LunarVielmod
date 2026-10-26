using Stellamod.Common;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Fable.WeaponsFB;


public class GalvinieNote : ModProjectile
{
    float Time => 120;
    float Scale => EasingFunction.InOutSine(Timer / 30f) * EasingFunction.InOutSine(Projectile.timeLeft / 30f);
    ref float Timer => ref Projectile.ai[0];
    ref float Frame => ref Projectile.ai[1];
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.GalvinieNote.KEY;

    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        Frame = Main.rand.Next(2);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(12);
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 16;
        Projectile.friendly = true;
        Projectile.LocalPiercingImmunityTime = 30;
        Projectile.light = 0.4f;
        Projectile.tileCollide = false;
        Projectile.timeLeft = (int)Time;

    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Main.rand.NextBool(8))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(1, 1);
            Dust.NewDustPerfect(pos, ModContent.DustType<GlyphDust>(), vel, Scale: 0.5f, newColor: Color.DarkOrange);
        }

        Projectile.frame = (int)Frame;
        Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, 0f, 0.03f);
        Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.X, -0.04f, 0.03f);
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.X * 0.05f, 0.03f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.DarkOrange, Color.Transparent, 0.3f);
        var drawer = Projectile.Drawer;
        drawer.scale = new Vector2(Scale);
        Main.spriteBatch.Draw(drawer);
        return false;
    }
}

public class Galvinie : BaseCrossbowItem
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.Galvinie.KEY;
    private int _fireSound;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 14;
        Item.knockBack = 4;
        Item.useTime = Item.useAnimation = 45;
        Item.rare = ItemRarityID.Green;
    }

    public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        Vector2 fireVelocity = shootParams.velocity * shootParams.speed;
        fireVelocity *= 3;
        fireVelocity *= shootParams.chargeStrength;

        if (shootParams.projToShoot == ProjectileID.WoodenArrowFriendly)
        {
            shootParams.projToShoot = ModContent.ProjectileType<GalvinieNote>();

            for (var i = 0; i < 4; i++)
            {
                float bowDamage = shootParams.damage * shootParams.chargeStrength;
                Projectile crossShot = Projectile.NewProjectileDirect(source, shootParams.position, fireVelocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.5f, 1f),
                    shootParams.projToShoot, (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);
            }

            _fireSound++;
            if (_fireSound >= 3)
            {
                _fireSound = 0;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MorrowSong"), player.position);

            }
            if (_fireSound == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MorrowSong2"), player.position);
            }
            if (_fireSound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MorrowSong3"), player.position);
            }
        }
        else
        {
            for (var i = 0; i < 4; i++)
            {
                float bowDamage = shootParams.damage * shootParams.chargeStrength;
                Projectile crossShot = Projectile.NewProjectileDirect(source, shootParams.position, fireVelocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.5f, 1f),
                    shootParams.projToShoot, (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);
                crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
            }
        }
    }

    public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        base.StaminaShootBow(player, source, shootParams);
        shootParams.projToShoot = ModContent.ProjectileType<GalvinieNote>();
        Vector2 fireVelocity = shootParams.velocity * shootParams.speed;
        fireVelocity *= 3;
        fireVelocity *= shootParams.chargeStrength;
        for (var i = 0; i < 15; i++)
        {
            float bowDamage = shootParams.damage * shootParams.chargeStrength;
            Projectile crossShot = Projectile.NewProjectileDirect(source, shootParams.position, fireVelocity.RotatedByRandom(1.5F) * Main.rand.NextFloat(0.5f, 1f),
                shootParams.projToShoot, (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);
        }

        _fireSound++;
        if (_fireSound >= 3)
        {
            _fireSound = 0;
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MorrowSong"), player.position);

        }
        if (_fireSound == 2)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MorrowSong2"), player.position);
        }
        if (_fireSound == 1)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MorrowSong3"), player.position);
        }
    }
}




