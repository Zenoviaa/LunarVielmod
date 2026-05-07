using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Collosseum.WeaponsCL;
using Stellamod.Content.Areas.WaterSide.KingJellyfishBoss;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Abyss.WeaponsAB;

public class ShadegraveStaff : ModItem
{
    private int _dir;
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Sun Blast Staff");
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.DefaultToArtifact();
        Item.staff[Item.type] = true;
        Item.damage = 40;
        Item.width = 50;
        Item.height = 50;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 4;
        Item.value = Item.sellPrice(0, 1, 1, 29);
        Item.rare = ItemRarityID.Blue;
        Item.shootSpeed = 35;
        Item.autoReuse = true;
        Item.UseSound = SoundID.DD2_BookStaffCast;

        Item.DamageType = DamageClass.Magic;
        Item.shoot = ModContent.ProjectileType<SGBolt>();
        Item.shootSpeed = 15f;
        Item.mana = 20;
        Item.useAnimation = 50;
        Item.useTime = 50;
        Item.consumeAmmoOnLastShotOnly = true;
        Item.noUseGraphic = true;

    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-5f, 0f);
    }


    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (_dir == 0)
        {
            _dir = 1;
        }
        else
        {
            _dir *= -1;
        }
        var p = Projectile.NewProjectileDirect(source, player.Center, velocity,
            ModContent.ProjectileType<StaffWaveHold>(), damage, knockback, player.whoAmI,
            ai2: _dir);
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<ConvulgingMater, BlankStaff>();
    }
}


public class SGBolt : ModProjectile
{
    private Asset<Texture2D> _gradientTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Shadow Hand");
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
    }
    public override void SetDefaults()
    {
        Projectile.penetrate = 3;
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.timeLeft = 700;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        Timer++;
        if (Timer % 3 == 0)
        {
            Dust.NewDustPerfect(Projectile.Bottom, ModContent.DustType<GlyphDust>(),
                (Vector2.One * Main.rand.NextFloat(0.2f, 1f)).RotatedByRandom(19.0), 0, Color.Purple, 2f).noGravity = true;
        }

        if (Timer == 1)
        {


            var dp = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 0.5f);
            dp.Scale *= 0.2f;
            dp.outerColor = Color.Purple;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;

            for (int j = 0; j < 10; j++)
            {
                Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                vector2 += -Utils.RotatedBy(Vector2.UnitY, (j * 3.141591734f / 6f), default(Vector2)) * new Vector2(8f, 16f);
                vector2 = Utils.RotatedBy(vector2, (Projectile.rotation - 1.57079637f), default(Vector2));
                int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Shadowflame, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[num8].scale = 1.3f;
                Main.dust[num8].noGravity = true;
                Main.dust[num8].position = Projectile.Center + vector2;
                Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                Main.dust[num8].noLight = true;
                Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
            }
        }
        if(Timer % 6 == 0)
        {
            var dp = DustParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.1f);
            dp.Scale *= 0.5f;
            dp.gravity = 0;
            dp.outerColor = Color.Purple;
        }
        if(Timer % 12 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            Vector2 vel = -Projectile.velocity * 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Purple;
            fx.VectorScale *= 0.5f;
        }
        if (Timer >= 20)
        {
            Projectile.tileCollide = true;
        }

        Projectile.spriteDirection = Projectile.direction;
        Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
            ModContent.ProjectileType<Skullboom>(), Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 20; i++)
        {
            int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, -2f, 0, default(Color), .8f);
            Main.dust[num1].noGravity = true;
            Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
            if (Main.dust[num1].position != Projectile.Center)
                Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
            int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, -2f, 0, default(Color), .8f);
            Main.dust[num].noGravity = true;
            Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
            Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
            if (Main.dust[num].position != Projectile.Center)
                Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
        }
        for (float f = 0; f < 30; f++)
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Purple, Main.rand.NextFloat(1f, 3f)).noGravity = true;
        }
        SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.position);
        for (float i = 0; i < 4; i++)
        {
            float progress = i / 4f;
            float rot = progress * MathHelper.ToRadians(360);
            Vector2 offset = rot.ToRotationVector2() * 24;
            var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Purple,
                outerGlowColor: Color.Black,
                baseSize: 0.3f);
            particle.Rotation = rot + MathHelper.ToRadians(45);
        }

        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<Skullboom>(), Projectile.damage, 0f, Projectile.owner, 0f, 0f);
        }
    }


    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Purple, ratio);
    }

    private float GetTrailWidth(float ratio)
    {

        float w = 16;
        return MathHelper.SmoothStep(w, w * 0f, ratio);
        float outEasing = EasingFunction.InExpo(Projectile.timeLeft / 30f);
        float outEasing2 = MathHelper.SmoothStep(0.5f, 1f, Timer / 15f);
        return MathHelper.SmoothStep(w * 0.85f, w, EasingFunction.QuadraticBump(ratio)) * outEasing * outEasing2;
    }

    private Color GetTrailColor2(float ratio)
    {
        Color bloomColor = Color.Lerp(Color.White, Color.Purple, 0.8f);
        Color bloomColor2 = Color.Lerp(Color.Transparent, bloomColor, EasingFunction.QuadraticBump(ratio));
        return Color.Lerp(Color.DarkViolet, Color.White, EasingFunction.QuadraticBump(ratio));
    }

    private float GetTrailWidth2(float ratio)
    {
        return GetTrailWidth(ratio) * 1.6f;
    }

    public override void PostDraw(Color lightColor)
    {
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Purple;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.1f;
        Main.spriteBatch.Draw(glowDrawer);

        glowDrawer.color = Color.White;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.75f;
        Main.spriteBatch.Draw(glowDrawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        _gradientTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Gradient");
        Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3() * 1.75f * Main.essScale);
        ZapLightningShader lightingShader = ZapLightningShader.Instance;
        lightingShader.Amplitude = 0.4f;

        float time = Main.GlobalTimeWrappedHourly * 32;
        float levels = 4;
        time = MathF.Floor(time * levels) / levels;
        lightingShader.Time = time;
        Asset<Texture2D> laserTexture = TrailRegistry.GlowTrail;
        lightingShader.LaserTexture = laserTexture;
        lightingShader.Noise = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BlurryPerlinNoise").Value;
        lightingShader.Gradient = _gradientTextureAsset.Value;
        lightingShader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        lightingShader.Levels = 64;
        lightingShader.Tiling = new Vector2(2f);
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, lightingShader, Projectile.Size * 0.5f);

        BloomTrailShader bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.Purple;
        bloom.OuterColor = Color.DarkViolet;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor2, GetTrailWidth2, bloom, Projectile.Size * 0.5f);


        return false;
    }

}
