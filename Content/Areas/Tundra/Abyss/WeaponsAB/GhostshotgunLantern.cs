using Stellamod.Common.GunSystem;
using Stellamod.Common.Particles;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.WeaponsAB;
public class GhostShot : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float TargetX => ref Projectile.ai[1];
    private ref float TargetY => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 5;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.light = 0.7f;
        Projectile.friendly = true;
        Projectile.extraUpdates = 1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            Projectile.frame = Main.rand.Next(3);
        }

        if (Timer < 60)
        {
            Projectile.velocity *= 0.93f;
        }

        if(Timer > 60)
        {
            if (this.OwnedByLocalClient())
            {
                Vector2 pos = Main.MouseWorld;
                TargetX = pos.X;
                TargetY = pos.Y;

                Projectile.netUpdate = true;
            }
            Vector2 target = new Vector2(TargetX, TargetY);
            Vector2 velTo = target - Projectile.Center;
            velTo = velTo.SafeNormalize(Vector2.Zero) * MathHelper.SmoothStep(0, 25, (Timer - 60) / 120f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, velTo, 0.1F);
            if(Vector2.DistanceSquared(Projectile.Center, target) < 32 * 32)
            {
                Projectile.Kill();
            }
        }

        if (Timer >= 180)
            Projectile.Kill();
        if (Main.rand.NextBool(30))
        {
            var smoke = FaintSmokeParticle.SpawnInAlphaLayer(
                Projectile.Center + Main.rand.NextVector2Circular(32, 32), 
                -Projectile.velocity.SafeNormalize(Vector2.Zero));
            smoke.color = Color.DarkGray;
            smoke.Scale *= 0.2f;
        }

        float targetRotation = Projectile.velocity.ToRotation();
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, targetRotation, 0.35f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);

        SpritebatchDrawer outlineDrawer = drawer;
        outlineDrawer.VerticalFrame(4, 5);
        foreach (OldPosition oldPos in Projectile.IterateOldPosBackwards())
        {
            outlineDrawer.worldPosition = oldPos.position + Projectile.Size * 0.5f;
            outlineDrawer.color = Color.LightBlue;
            outlineDrawer.rotation = Projectile.oldRot[oldPos.index];
            outlineDrawer.scale = Vector2.Lerp(Vector2.One, Vector2.One * 0.1f, EasingFunction.OutSine(oldPos.progress));
            Main.spriteBatch.Draw(outlineDrawer);
        }

        SpritebatchDrawer trailDrawer = drawer;
        trailDrawer.color = trailDrawer.color.MultiplyRGB(new Color(0.75f, 0.75f, 0.75f));
        trailDrawer.VerticalFrame(3, 5);
        foreach (OldPosition oldPos in Projectile.IterateOldPosBackwards())
        {
            trailDrawer.worldPosition = oldPos.position + Projectile.Size * 0.5f;
          //  trailDrawer.color = Color.Lerp(Color.White, Color.Transparent, oldPos.progress);
            trailDrawer.rotation = Projectile.oldRot[oldPos.index];
            trailDrawer.scale = Vector2.Lerp(Vector2.One, Vector2.One * 0.1f, EasingFunction.OutSine(oldPos.progress));
            Main.spriteBatch.Draw(trailDrawer);
        }

        drawer.scale *= 0.8f;
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for(float f = 0; f < 3; f++)
        {
            var smoke = FaintSmokeParticle.SpawnInAlphaLayer(
            Projectile.Center + Main.rand.NextVector2Circular(32, 32),
            Main.rand.NextVector2Circular(8, 8));
            smoke.color = Color.DarkGray;
            smoke.expand = true;
            smoke.Scale *= 0.1f;
        }
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.DarkGray, Color.Black, 25, 64);
        var shadeHand = AssetReferences.Assets.Sounds.ShadeHand.Asset with { PitchVariance = 1f, Volume = 0.4f };
        SoundEngine.PlaySound(shadeHand, Projectile.position);
    }
}
public class GhostshotgunLantern : BaseGun
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToGun();
        Item.damage = 14;
        Item.UseSound = SoundID.Item38;
        muzzleOrigin = new Vector2(72, 14);
    }
    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 7;
        fireParams.reloadWindow = 150;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(27, 0);
    }

    public override void ShootEffects(Vector2 position, Vector2 velocity)
    {

        SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew7");
        shootSound.PitchVariance = 0.3f;
        shootSound.Volume = 0.05f;
        SoundEngine.PlaySound(shootSound, position);
        BasicMuzzleFlash(position, velocity, Color.SkyBlue, Color.DarkBlue);
        for (float f = 0; f < 9; f++)
        {
            Vector2 vel = velocity;
            vel = vel.RotatedByRandom(1.5f);
            vel *= Main.rand.NextFloat(0.6f, 1.2f);
  
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = position,
                velocity = vel,
                innerColor = Color.SkyBlue.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.6f, 1.6f))
            });
        }
    }
    public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (type == ProjectileID.Bullet)
        {
            type = ModContent.ProjectileType<GhostShot>();
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = velocity;
                vel = vel.RotatedByRandom(1.5f);
                vel *= Main.rand.NextFloat(0.6f, 1.2f);
                vel *= 3;
                Projectile.NewProjectile(source, position, vel, type, damage, knockback, player.whoAmI);
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = velocity;
                vel = vel.RotatedByRandom(0.5f);
                vel *= Main.rand.NextFloat(0.6f, 1.2f);
                Projectile.NewProjectile(source, position, vel, type, damage, knockback, player.whoAmI);
            }
        }
  
        return false;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<ConvulgingMater, BlankGun>();
    }
}
