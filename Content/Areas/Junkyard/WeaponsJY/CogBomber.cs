using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Dusts;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Junkyard.WeaponsJY;

public class CogBomber : BaseGun
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.noUseGraphic = true;
        Item.damage = 72;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 62;
        Item.height = 34;
        Item.useTime = 40;
        Item.useAnimation = 37;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(0, 15, 0, 0);
        Item.rare = ItemRarityID.LightRed;
        Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/gun1") with { PitchVariance = 0.7f };
        Item.autoReuse = true;
        Item.shootSpeed = 12f;
        Item.shoot = ModContent.ProjectileType<CogBomb>();
        Item.useAmmo = AmmoID.Bullet;

        Item.noMelee = true;
    }


    public override Vector2? HoldoutOffset()
    {
        return new Vector2(16, 0);
    }

    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 6;
    }

    public override bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.GunShot(player, source, position, velocity, type, damage, knockback);
    }
    public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        muzzleOrigin = new Vector2(68, 14);
        type = ModContent.ProjectileType<CogBomb>();
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }

    public override void ShootEffects(Vector2 position, Vector2 velocity)
    {
        base.ShootEffects(position, velocity);
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankGun>(),
            material: ModContent.ItemType<MechanizedSoul>());
    }
}
public class CogBomb : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private float _rotation;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.aiStyle = 2;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 3600;
     
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        Projectile.Kill();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.Red, Color.Transparent, 0.3f);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void AI()
    {
        Timer++;
        if(Timer % 6 == 0)
        {
            var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(40, 40), -Projectile.velocity.SafeNormalize(Vector2.Zero));
            dp.dampening = 0.05f;
            dp.innerColor = Color.OrangeRed;
            dp.fast = true;
            dp.superFast = true;
        }
        Projectile.rotation += _rotation;
        _rotation += 0.01f;
    }


    public override void OnKill(int timeLeft)
    {

        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                ModContent.ProjectileType<CogBombBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

    }
}

public class CogBombBoom : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
        Projectile.light = 1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
            }
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }
        
            for(int i = 0; i < 14; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 vel = Main.rand.NextVector2Circular(10, 10);
                var smokeParitcle = SmokeParticle.SpawnInAlphaLayer(pos, vel);
                smokeParitcle.dampening = 0.09f;
                smokeParitcle.fadeToColor = Color.Black * 0.5f;
                smokeParitcle.initialColor = Color.DarkRed * 0.5f;
                smokeParitcle.Scale *= 2f;
            }

            for(int i = 0; i < 8; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                var dp = DustParticle.Spawn(pos, vel);
                dp.dampening = 0.05f;
                dp.innerColor = Color.OrangeRed;
                dp.fast = true;

            }

            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Yellow, Color.Red, duration: 12, baseSize: 0.24f);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with {  PitchVariance = 0.6f }, Projectile.position);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }

    public void DrawToRenderTargets()
    {
     
    }
}