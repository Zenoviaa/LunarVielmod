using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;
public class GrassySlingshot : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.channel = true;
        Item.autoReuse = false;
        Item.damage = 18;
        Item.knockBack = 1;
        Item.DamageType = DamageClass.Ranged;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useTime = Item.useAnimation = 20;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.shoot = ModContent.ProjectileType<GrassySlingshotHeld>();
        Item.ArmorPenetration = 10;
    }

    public override bool CanShoot(Player player)
    {
        return base.CanShoot(player) && player.ownedProjectileCounts[Item.shoot] == 0;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankGun>();
    }
}

public class SlingshotBall : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(12);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 18;
        Projectile.friendly = true;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 120;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if(Timer >= 3)
        {
            Projectile.Kill();
        }
        else
        {
            Timer++;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
        }
        return false; // base.OnTileCollide(oldVelocity);
    }
    public override void AI()
    {
        base.AI();
        if (Main.rand.NextBool(6))
        {
            var pos = Projectile.position;
            pos.X += Main.rand.Next(0, Projectile.width);
            pos.Y += Main.rand.Next(0, Projectile.height);
            var vel = Main.rand.NextVector2Circular(2, 2);
            var d = Dust.NewDustPerfect(pos, DustID.Dirt, vel, Scale: Main.rand.NextFloat(0.6f, 0.8f));
            d.noGravity = true;
        }
        Projectile.velocity.Y += 0.2f;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawAdditiveFadingTrail(Projectile, Color.White, Color.Transparent, 0.4f);
        Main.spriteBatch.Draw(Projectile.Drawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (var i = 0; i < 16; i++)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(24, 24);
            var vel = Main.rand.NextVector2Circular(1.5f, 1.5f);
            var d = Dust.NewDustPerfect(pos, DustID.Dirt, vel, Scale: Main.rand.NextFloat(0.4f, 1.5f));
            d.noGravity = true;
        }
    }
}
public class GrassySlingshotHeld : ModProjectile
{
    Player Owner => Main.player[Projectile.owner];
    ref float Timer => ref Projectile.ai[0];
    ref float KillMe => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 24;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            var sling = AssetReferences.Assets.Sounds.Bow.CrossbowPull.Asset with { PitchVariance = 0.5f, Volume = 0.2f };
            SoundEngine.PlaySound(sling, Projectile.position);
        }
        if (Main.myPlayer == Projectile.owner)
        {
            Projectile.velocity = (Main.MouseWorld - Owner.MountedCenter);
            Projectile.netUpdate = true;
            if (!Owner.channel)
            {
                var shoot = AssetReferences.Assets.Sounds.Jack_Land.Asset with { PitchVariance = 0.3f };
                SoundEngine.PlaySound(shoot, Projectile.position);
                var charge = EasingFunction.InOutSine(Timer / 60f);
                var firer = ProjFirer.From<SlingshotBall>(Projectile);
                firer.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 25 * charge;
                firer.damage = (int)(Projectile.damage * MathHelper.Lerp(0.5f, 1f, charge));
                firer.knockback = 1;
                firer.New();

                KillMe = 1;

            }

        }
        if (KillMe != 0)
            Projectile.Kill();
        Projectile.Center = Owner.MountedCenter + Projectile.velocity.SafeNormalize(Vector2.Zero) * 16;
        Projectile.rotation = Projectile.velocity.ToRotation();
        Owner.heldProj = Projectile.whoAmI;
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        var rot = (Projectile.Center - Owner.Center).ToRotation() - MathHelper.PiOver2;
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot);
        Owner.ChangeDir((Projectile.velocity.X < 0) ? -1 : 1);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        if (Projectile.velocity.X < 0)
            drawer.spriteEffects = SpriteEffects.FlipVertically;
        drawer.worldPosition.Y += Owner.gfxOffY;
        Main.spriteBatch.Draw(drawer);

        var drawr2 = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.MagicCastSparkle.Asset, Projectile.Center);
        drawr2.worldPosition += Vector2.Lerp(Projectile.velocity.SafeNormalize(Vector2.Zero) * 32, Vector2.Zero, EasingFunction.InSine(Timer / 60f));
        drawr2.color = Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuickOutSlowIn(Timer / 60f)) * 0.4f;
        drawr2.color.A = 0;
        drawr2.scale = Vector2.Lerp(new Vector2(1.3f), Vector2.Zero, EasingFunction.InOutSine(Timer / 60f)) * 0.5f;
        drawr2.rotation = Main.GlobalTimeWrappedHourly * 3;
        Main.spriteBatch.Draw(drawr2);
        return false;
    }
}