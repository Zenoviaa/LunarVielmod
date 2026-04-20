using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Cinderspark.WeaponsCS;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class Mailloader : BaseGun
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 4;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 40;
        Item.height = 40;
        Item.useTime = 16;
        Item.useAnimation = 16;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.sellPrice(0, 0, 20, 0);
        Item.rare = ItemRarityID.Blue;
        Item.UseSound = SoundID.Item5;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.Bullet;
        Item.shootSpeed = 8f;
        Item.useAmmo = AmmoID.Bullet;
        Item.noMelee = true;


        muzzleOrigin = new Vector2(45, 10);
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(16, 0);
    }

    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 16;
    }

    public override bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.GunShot(player, source, position, velocity, type, damage, knockback);
    }
    public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        type = ModContent.ProjectileType<Letterbomb>();
        if (remainingAmmo == GetMaxAmmo(player) - 1)
        {
            type = ModContent.ProjectileType<Mailbomb>();
            damage *= 2;
        }
    
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankGun>(),
            material: ModContent.ItemType<Mushroom>());
    }
}

public class Letterbomb : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private bool Bounced
    {
        get => Projectile.ai[1] == 1;
        set => Projectile.ai[1] = value ? 1 : 0;
    }
    private int Frame
    {
        get => (int)Projectile.ai[2];
        set => Projectile.ai[2] = value;
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                Frame = Main.rand.Next(Main.projFrames[Type]);
                Projectile.netUpdate = true;
            }
        }

        if(Timer % 9 == 0)
        {
            Vector2 pos = Projectile.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
           var fx = FXUtil.GlowStretch(pos, -Projectile.velocity * 0.5f);
            fx.VectorScale *= 0.24f;
            fx.OuterGlowColor = Color.OrangeRed;
        }

        if(Timer % 15 == 0)
        {
            LetterParticle.SpawnInAlphaLayer(Projectile.Center, Vector2.Zero, 
                Scale: Main.rand.NextFloat(0.4f, 0.8f));
        }

        if (Bounced)
        {
            Projectile.velocity.Y += 0.5f;
        }
        Projectile.frame = Frame;
        Projectile.rotation += Projectile.velocity.Length() * 0.01f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer);
        return false;
    //    return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for(float f = 0; f < Main.rand.Next(4, 8); f++)
        {
            Vector2 fireVelocity = -Vector2.UnitY;
            fireVelocity *= Main.rand.NextFloat(2f, 5f);
            fireVelocity = fireVelocity.RotateRandom(MathHelper.ToRadians(35));
            LetterParticle.SpawnInAlphaLayer(Projectile.Center, fireVelocity, Scale: Main.rand.NextFloat(0.6f, 1.2f));
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (!Bounced)
        {
            Bounced = true;
            Projectile.velocity.X *= -Main.rand.NextFloat(-0.3f, -0.5f);
            Projectile.velocity.Y -= Main.rand.NextFloat(7, 10);
            Projectile.netUpdate = true;
        }
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }
}
public class Mailboom : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 120;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle hitSound = AssetRegistry.Sounds.Melee.Vinger2;
            hitSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(hitSound, Projectile.position);

            FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red, duration: 25, baseSize: 0.28f);

            var fx = FXUtil.GlowStretch(Projectile.Center, Main.rand.NextVector2Circular(1, 1));
            fx.VectorScale.X *= 4;
            fx.VectorScale.Y *= 6;
            fx.GlowColor = Color.Yellow;
            fx.OuterGlowColor = Color.Red;
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);

            for (float f = 0; f < 16; f++)
            {
               var dp = Particle<DustParticle>.Spawn(Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(10, 15), Scale: Main.rand.NextFloat(0.5f, 1f));
                dp.innerColor = Color.Yellow;
                dp.outerColor = Color.Red;
                dp.noTileCollide = true;
                dp.gravity = 0;
            }

            for(float f =0; f < 4; f++)
            {
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                var fs = FaintSmokeParticle.SpawnInAlphaLayer(pos, -Vector2.UnitY, Scale: Main.rand.NextFloat(0.25f, 0.5f));
                fs.noShrink = true;
                fs.Scale *= Main.rand.NextFloat(0.25f, 0.5f);
                fs.color = Color.Lerp(Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f)), Color.Black, 0.7f);
                fs.fadeToColor = Color.Lerp(Color.OrangeRed, Color.Black, 0.8f);
            }
            for (float f = 0; f < 4; f++)
            {
                var smoke = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                smoke.initialColor = Color.DarkGray;
            }


            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Red,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        float outRatio = Timer / 120f;
        float scale = 1.4f;
        RadialShearShader shearShader = RadialShearShader.Instance;
        shearShader.Time = outRatio * 1.4f;

        Asset<Texture2D> magicCircle = AssetManager.GlowMask.MuzzleFlash;
        SpritebatchDrawer waveDrawer = SpritebatchDrawer.FromTextureAsset(magicCircle, Projectile.Center);
        waveDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        waveDrawer.scale = Vector2.Lerp(Vector2.One * 0.8f, Vector2.One * 1.6f, EasingFunction.OutExpo(outRatio)) * scale;
        waveDrawer.color = Color.Red;
        waveDrawer.color *= MathHelper.SmoothStep(1f, 0f, outRatio);
        waveDrawer.color.A = 0;

        Main.spriteBatch.Restart(effect: shearShader.Effect);
        Main.spriteBatch.Draw(waveDrawer);

        waveDrawer.scale *= 0.95f;
        waveDrawer.color = Color.Gold;
        waveDrawer.color *= MathHelper.SmoothStep(1f, 0f, outRatio);
        waveDrawer.color.A = 0;
        Main.spriteBatch.Draw(waveDrawer);
        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkRed * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * scale;
        Main.spriteBatch.Draw(backGlowDrawwer);

        waveDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(outRatio));
        waveDrawer.color.A = 0;
        Main.spriteBatch.Draw(waveDrawer);
        Main.spriteBatch.RestartDefaults();
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }
}
public class Mailbomb : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 1;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 90;
        Projectile.penetrate = 1;
        Projectile.usesLocalNPCImmunity = true;
    }
    public override void AI()
    {
        base.AI();
 
        Timer++;
        if (Timer % 15 == 0)
        {
            LetterParticle.SpawnInAlphaLayer(Projectile.Center, Vector2.Zero,
                Scale: Main.rand.NextFloat(0.4f, 0.8f));
        }


        Projectile.rotation += Projectile.velocity.Length() * 0.015f;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        for(int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i];
            pos += Projectile.Size * 0.5f;

            SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            afDrawer.worldPosition = pos;
            afDrawer.color = Color.Lerp(Color.OrangeRed, Color.Transparent, (float)i / (float)Projectile.oldPos.Length) * 0.3f;
            Main.spriteBatch.Draw(afDrawer);
        }
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer);

        SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
        Main.spriteBatch.Restart(effect: whiteShader.Effect);
        sbDrawer.color = Color.Lerp(Color.Yellow * 0f, Color.Red * 0.5f, ExtraMath.Osc(0f, 1f, speed: 20));
        Main.spriteBatch.Draw(sbDrawer);
        Main.spriteBatch.RestartDefaults();

        SpritebatchDrawer flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
        flareDrawer.color = Color.Gold * MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 30f));
        flareDrawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One * 0.5f, EasingFunction.InOutSine(Timer / 30f));
        flareDrawer.color.A = 0;
        Main.spriteBatch.Draw(flareDrawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            float damage = Projectile.damage;
            float damageMult = MathHelper.Lerp(1f, 1.75f, EasingFunction.InOutSine(Timer / 30f));
            int finalDamage = (int)(damage * damageMult);
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, 
                ModContent.ProjectileType<Mailboom>(), finalDamage, Projectile.knockBack, Projectile.owner);
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        Projectile.Kill();
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }
}
