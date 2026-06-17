using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Gun;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Junkyard.WeaponsJY;

public class GearGutter : BaseGun
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 254;
        Item.DamageType = DamageClass.Ranged;

        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.sellPrice(0, 0, 20, 0);
        Item.rare = ItemRarityID.Blue;
        Item.UseSound = SoundID.Item5;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.Bullet;
    
        Item.useAmmo = AmmoID.Bullet;



        Item.noUseGraphic = true;
        Item.width = 84;
        Item.height = 36;
        Item.useTime = 72;
        Item.useAnimation = 72;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(0, 15, 0, 0);
        Item.rare = ItemRarityID.LightRed;
     //   Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew7") with { PitchVariance = 0.7f, Volume = 0.9f };
        Item.autoReuse = true;
        Item.shootSpeed = 50f;
        Item.shoot = ModContent.ProjectileType<GearSniper>();
        Item.noMelee = true;
        muzzleOrigin = new Vector2(68, 14);
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
        type = ModContent.ProjectileType<GearSniper>();
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }

    public override void ShootEffects(Vector2 position, Vector2 velocity)
    {
        base.ShootEffects(position, velocity);
   
        SoundStyle snipeSound = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew7") with { PitchVariance = 0.7f, Volume = 0.25f };
        SoundEngine.PlaySound(snipeSound, position);
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankGun>(),
            material: ModContent.ItemType<MechanizedSoul>());
    }
}


public class GearSniper : ModProjectile,
    IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.light = 0.78f;
    }


    public override void AI()
    {
        Timer++;
        if(Timer < 16 && Main.rand.NextBool(2))
        {
            var smokeParticle = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.5f));
            smokeParticle.fadeToColor = Color.Black * 0.3f;
            smokeParticle.color = Color.RosyBrown;
            smokeParticle.Scale *= 0.3f;
        }
        Projectile.rotation = Projectile.velocity.ToRotation();
        if(Timer % 14 == 0)
        {
            var p =LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero));
            p.Scale *= 0.45f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Bleeding, 300);
        target.AddBuff(BuffID.Poisoned, 300);
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
            ModContent.ProjectileType<NailKaboom>(), 0, 0, Projectile.owner);
        SoundStyle sound;
        switch (Main.rand.Next(0, 2))
        {
            default:
            case 0:
                sound = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit");
                break;
            case 1:
                sound = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit2");
                break;
        }
        sound = sound with { PitchVariance = 0.8f };
        SoundEngine.PlaySound(sound, target.position);

        FXUtil.ShakeCamera(target.Center, 256, 8);
        FXUtil.GlowCircleBoom(target.Center, Color.White, Color.Red, Color.DarkRed, duration: 8, baseSize: 0.16f);
        PixelPrimitiveCircleFactory.CreateGenericBoom(target.Center, Color.White, Color.Red, 15, 128);
    }

    public override void OnKill(int timeLeft)
    {
        if (!this.OwnedByLocalClient())
            return;
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
            ModContent.ProjectileType<NailKaboom>(), 0, 0, Projectile.owner);
    }
    
    private void DrawPixelatedTrail(GraphicsDevice gDevice)
    {
        float GetTrailWidth(float progress)
        {
            return MathHelper.SmoothStep(16, 0, progress);
        }

        float GetTrailWidth2(float progress)
        {
            return GetTrailWidth(progress) * 1.6f;
        }

        Color GetTrailColor(float progress)
        {
            Color inColor = Color.White;
            Color trailColor = Color.Lerp(Color.OrangeRed, Color.DarkRed, progress);
            Color easeColor = Color.Lerp(inColor, trailColor, EasingFunction.InExpo(Timer / 60f));
            return easeColor * 2;
        }

        var shader2 = RichLaserShader.Instance;
        shader2.LaserColor = Color.White;
        shader2.LaserTexture = TrailRegistry.StarTrail;
        shader2.InnerColor = Color.Yellow ;
        shader2.OuterColor = Color.DarkRed;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader2, Projectile.Size * 0.5f);

        var bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.Yellow ;
        bloom.OuterColor = Color.Red;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth2, bloom, Projectile.Size * 0.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.Red, Color.Transparent, alpha: 0.3f);
        SpritebatchDrawer mainSprite = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(mainSprite);


        mainSprite.VerticalFrame(1, Main.projFrames[Type]);
        mainSprite.color = Color.Lerp(Color.Red, Color.Transparent, EasingFunction.InOutSine(Timer / 30f));
        Main.spriteBatch.Draw(mainSprite);

        mainSprite.VerticalFrame(2, Main.projFrames[Type]);
        mainSprite.color = Color.Lerp(Color.Yellow, Color.Yellow * 0.5f, ExtraMath.Osc(0f, 1f, speed: 16));
        Main.spriteBatch.Draw(mainSprite);
        return false;
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail);
    }
}
