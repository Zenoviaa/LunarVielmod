using Microsoft.Xna.Framework;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Dusts;
using Stellamod.Items;
using Stellamod.Projectiles.Magic;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;

public class BlackEye : ModItem
{
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Wooden Crossbow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        /* Tooltip.SetDefault("Use a small crossbow and shoot three bolts!"
            + "\n'Triple Threat!'"); */
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.damage = 666;
        Item.DamageType = DamageClass.Magic;
        Item.width = 32;
        Item.mana = 50;
        Item.height = 25;
        Item.useTime = 80;
        Item.useAnimation = 80;
        Item.useStyle = ItemUseStyleID.Shoot;

        Item.knockBack = 2;
        Item.rare = ItemRarityID.Purple;
        Item.autoReuse = false;
        Item.shootSpeed = 30f;
        Item.shoot = ModContent.ProjectileType<BlackEyeProj>();
        Item.scale = 0.8f;
        Item.noMelee = true; // The projectile will do the damage and not the item
        Item.value = Item.buyPrice(gold: 95);
        Item.noUseGraphic = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, position);
        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, position);
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<FallenEyes, BlankStaff>();
    }
}

public class BlackEyeLaserProj : ModProjectile
{
    public ref float Time => ref Projectile.ai[0];

    public override string Texture => TextureRegistry.EmptyTexture;

    private const float MaxBeamLength = 2400f;

    public float BeamLength;

    private Vector2 ImpactPoint => Projectile.Center + Projectile.velocity * BeamLength;
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 48;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 90;
        Projectile.alpha = 255;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 8;
    }

    public override void AI()
    {
        float targetBeamLength = ProjectileHelper.PerformBeamHitscan(Projectile, 2400);
        BeamLength = targetBeamLength;
        if (Time == 1)
        {
            SoundStyle sound = new SoundStyle("Stellamod/Assets/Sounds/RekLaser2") with { PitchVariance = 0.6f };
            SoundEngine.PlaySound(sound, Projectile.position);
        }

        if (Main.myPlayer == Projectile.owner)
        {
            Vector2 p = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero);
            Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, Main.MouseWorld, 1);
            Projectile.netUpdate = true;
        }
        if (Time % 2 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            Vector2 vel = Projectile.velocity * 8;
            float scale = Main.rand.NextFloat(2.5f, 3.75f);
            var dp = DustParticle.Spawn(pos, vel * Main.rand.NextFloat(1f, 5f));
            dp.gravity = 0;
            dp.dampening = 0.05f;
            dp.noTileCollide = true;
            dp.innerColor = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0f, 1f));
            dp.outerColor = Color.Red;
        }



        // And create bright light.
        Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 1.5f);
        Time++;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = 0f;
        float width = Projectile.width * 0.8f;
        Vector2 start = Projectile.Center;
        Vector2 end = start + Projectile.velocity * (MaxBeamLength + 40f);
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
    }


    public override bool ShouldUpdatePosition() => false;


    private void DrawLaser(GraphicsDevice gDevice)
    {
        FixedRichLaserShader richlaserShader = ShaderContent.GetInstance<FixedRichLaserShader>();
        richlaserShader.LaserColor = Color.Gold;
        richlaserShader.InnerColor = Color.Red;
        richlaserShader.OuterColor = Color.Yellow;
        richlaserShader.LaserTexture = TrailRegistry.Beamlight;
        richlaserShader.BloomTexture = AssetManager.LaserTextures.TexturedLaser2;
        richlaserShader.Time = Main.GlobalTimeWrappedHourly * 77;
        Vector2[] points = DrawUtilities.InterpolateBetweenPoints(Projectile.Center, Projectile.Center + Projectile.velocity * BeamLength, 80);
        TrailDrawer.Draw(points, GetTrailColor, GetTrailWidth, richlaserShader);

    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMuzzleFlash);
        PixelationManager.QueuePrimitivesDrawAction(DrawLaser,DrawLayer.OverNPCsWithOutline);
        PixelationManager.QueueSpritebatchDrawAction(DrawImpactPoints);
        return false;
    }
    public void DrawPixelatedMuzzleFlash(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Asset<Texture2D> muzzleFlashTexture = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/MuzzleFlash");
        Vector2 drawOrigin = muzzleFlashTexture.Size() / 2f;
        Vector2 drawCenter = Projectile.Center - screenPos;
        Color drawColor = Color.Yellow;
        drawColor.A = 0;
        float s = EasingFunction.InOutSine((float)Projectile.timeLeft / 120f) * MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Time / 30f)) * 1.65f;
        float width = (float)Projectile.timeLeft / 30f;
        float outWidth = EasingFunction.InOutSine(width);
        float scale = outWidth * s * 1.5f;
        Vector2 flashScale = Vector2.One;
        flashScale.X *= 1.5f;
        flashScale.Y *= 1.2f;
        flashScale *= scale;
        spriteBatch.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale, SpriteEffects.None, 0);

        drawColor = Color.White;
        drawColor.A = 0;
        spriteBatch.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale * 0.6f, SpriteEffects.None, 0);

        Asset<Texture2D> impactTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/ZuiEffect");
        drawOrigin = impactTexture.Size() / 2f;

        Vector2 impactPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * BeamLength;
        drawCenter = impactPoint - screenPos;
        drawColor = Color.Red;
        drawColor.A = 0;
        spriteBatch.Draw(impactTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, scale * 1.2f, SpriteEffects.None, 0);

        drawColor = Color.White;
        drawColor.A = 0;
        spriteBatch.Draw(impactTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, scale * 0.8f, SpriteEffects.None, 0);
    }
    private void DrawImpactPoints(SpriteBatch sb, Vector2 sp)
    {
        float s = EasingFunction.InOutSine((float)Projectile.timeLeft / 120f) * MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Time / 30f)) * 1.65f;
        SpritebatchDrawer flareDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, ImpactPoint);
        flareDrawer.color = Color.Red;
        flareDrawer.color.A = 0;
        flareDrawer.scale *= 0.65f * ExtraMath.Osc(0.7f, 1f, speed: 12) * s;
        flareDrawer.rotation = Main.GlobalTimeWrappedHourly * 3;
        sb.Draw(flareDrawer);

        flareDrawer.color = Color.Goldenrod;
        flareDrawer.color.A = 0;
        sb.Draw(flareDrawer);

        SpritebatchDrawer fireDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, ImpactPoint);
        fireDrawer.color = Color.Red;
        fireDrawer.color.A = 0;
        fireDrawer.scale *= s;
        sb.Draw(fireDrawer);

        fireDrawer.color = Color.Yellow;
        fireDrawer.color.A = 0;
        fireDrawer.scale *= 0.44f;
        sb.Draw(fireDrawer);

        fireDrawer.color = Color.White;
        fireDrawer.color.A = 0;
        fireDrawer.scale *= 0.44f;
        sb.Draw(fireDrawer);

    }

    public Color GetTrailColor(float completionRatio)
    {
        float osc = ExtraMath.Osc(0f, 1f, speed: 12, Projectile.identity);
        return Color.Lerp(Color.Yellow, Color.Lerp(Color.OrangeRed, Color.Red, osc), osc);
    }

    private float GetTrailWidth(float completionRatio)
    {
        return 128 * EasingFunction.InOutSine((float)Projectile.timeLeft / 120f) * MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Time / 30f));
    }

}
public class BlackEyeProj : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 32;
    }

    public override void SetDefaults()
    {
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        Timer++;
        if (Timer == 1)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(0.25f, 0.25f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speed * 17, Scale: 1f);
                d.noGravity = true;

                Vector2 speeda = Main.rand.NextVector2CircularEdge(0.25f, 0.25f);
                var da = Dust.NewDustPerfect(Projectile.Center, DustID.OrangeTorch, speeda * 11, Scale: 1f);
                da.noGravity = false;

                Vector2 speedab = Main.rand.NextVector2CircularEdge(0.25f, 0.25f);
                var dab = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, speeda * 30, Scale: 1f);
                dab.noGravity = false;
            }

            FXUtil.GlowCircleBoom(Projectile.Center,
                 innerColor: Color.White,
                 glowColor: Color.Yellow,
                 outerGlowColor: Color.Red, duration: 25, baseSize: 0.24f);
            Projectile.velocity = Vector2.Zero;
            Projectile.velocity += -Vector2.UnitY * 4;
        }
        else
        {
            Projectile.velocity *= 0.8f;
        }

        if (Timer == 30)
        {
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero),
                    ModContent.ProjectileType<BlackEyeLaserProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        if (Timer >= 120)
        {
            Projectile.Kill();
        }
        DrawHelper.AnimateTopToBottom(Projectile, 5);
        Lighting.AddLight(Projectile.Center, Color.LightGoldenrodYellow.ToVector3() * 1.5f);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        FXUtil.GlowCircleBoom(Projectile.Center,
            innerColor: Color.White,
            glowColor: Color.Yellow,
            outerGlowColor: Color.Red, duration: 25, baseSize: 0.24f);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(BuffID.OnFire3, 120);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer eyeDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        eyeDrawer.color.A = 0;
        Main.spriteBatch.Draw(eyeDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Red * 0.4f * ExtraMath.Osc(0.5f, 1f, speed: 10);
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.6f;
        Main.spriteBatch.Draw(glowDrawer);
        return false;
    }

    
}