using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL;

public class IllurianBible : ModItem
{

    public override void SetDefaults()
    {
        Item.DefaultToArtifact();
        Item.width = 30;
        Item.height = 42;
        Item.DamageType = DamageClass.Magic;
        Item.damage = 91;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(gold: 1);
        Item.shootSpeed = 10;
        Item.autoReuse = true;
        Item.noMelee = true;
        Item.rare = ItemRarityID.Lime;
        Item.mana = 15;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 20;
        Item.useTime = 20;
        Item.UseSound = SoundID.Item20;
        Item.shoot = ModContent.ProjectileType<IllurianBibleProj>();
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-3f, -2f);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, -velocity, type, damage, knockback, player.whoAmI);
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<IllurineScale, BlankStaff>();
    }
}
public class IllurianBibleProj : ModProjectile,
    IDrawToRenderTarget
{
    private Vector2 OriginalVelocity;
    private Vector2 OriginalPosition;

    public override string Texture => TextureRegistry.EmptyTexture;
    private ref float Timer => ref Projectile.ai[0];
    private ref float ReturnTimer => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 720;
        Projectile.extraUpdates = 1;
        Projectile.penetrate = 2;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
    }

    private void AI_MoveToward(Vector2 targetCenter, float speed = 8)
    {
        //chase target
        Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
        float distanceToTarget = Vector2.Distance(Projectile.Center, targetCenter);
        if (distanceToTarget < speed)
        {
            speed = distanceToTarget;
        }

        Vector2 targetVelocity = directionToTarget * speed;

        if (Projectile.velocity.X < targetVelocity.X)
        {
            Projectile.velocity.X++;
            if (Projectile.velocity.X >= targetVelocity.X)
            {
                Projectile.velocity.X = targetVelocity.X;
            }
        }
        else if (Projectile.velocity.X > targetVelocity.X)
        {
            Projectile.velocity.X--;
            if (Projectile.velocity.X <= targetVelocity.X)
            {
                Projectile.velocity.X = targetVelocity.X;
            }
        }

        if (Projectile.velocity.Y < targetVelocity.Y)
        {
            Projectile.velocity.Y++;
            if (Projectile.velocity.Y >= targetVelocity.Y)
            {
                Projectile.velocity.Y = targetVelocity.Y;
            }
        }
        else if (Projectile.velocity.Y > targetVelocity.Y)
        {
            Projectile.velocity.Y--;
            if (Projectile.velocity.Y <= targetVelocity.Y)
            {
                Projectile.velocity.Y = targetVelocity.Y;
            }
        }
    }

    public override void AI()
    {
        Timer++;
        if (Timer == 1)
        {
            Player owner = Main.player[Projectile.owner];
            Vector2 offset = -Vector2.UnitY;
            offset *= 128;
            Vector2 targetCenter = owner.Center + offset;
            OriginalVelocity = Projectile.velocity;
            OriginalPosition = targetCenter;
            SoundStyle soundStyle = SoundRegistry.Niivi_StarSummon;
            soundStyle.PitchVariance = 0.5f;
            soundStyle.Volume = 0.48f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
        }

        if (Timer < 30 || ReturnTimer > 0)
        {
            ReturnTimer--;
            if (ReturnTimer == 0 && Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * OriginalVelocity.Length();
                Projectile.netUpdate = true;
            }

            if (ReturnTimer > 0)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Pi / 30);
            }
            else
            {
                AI_MoveToward(OriginalPosition, 8);

            }
        }

        if (Timer > 30 && Timer < 45)
        {
            Projectile.velocity *= 0.99f;
        }

        if (Timer == 45)
        {
            SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2");
            shootSound = shootSound with { PitchVariance = 0.7f };
            SoundEngine.PlaySound(shootSound, Projectile.position);

            if (Main.myPlayer == Projectile.owner)
            {

                Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * OriginalVelocity.Length();
                Projectile.netUpdate = true;
            }
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 6, baseSize: 0.10f);
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.SkyBlue, 12, 32);
            for (float f = 0; f < 2; f++)
            {
                var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(10, 10));
                dp.innerColor = Color.SkyBlue;
                dp.outerColor = Color.Blue;
                dp.gravity = 0;
                dp.dampening = 0.05f;
                dp.noTileCollide = true;
                dp.Scale *= 0.65f;
            }
        }

        if (Timer > 45)
        {
            if (Main.rand.NextBool(8))
            {
                var sp = SparkleParticle.Spawn(Projectile.Center, Vector2.Zero);
                sp.flickering = true;
                sp.outerColor = Color.SkyBlue;
                sp.innerColor = Color.White;
                sp.dampening = 0.05f;
                sp.noTileCollide = true;
                sp.gravity = 0;
                sp.Scale *= 0.4f;
                sp.fast = true;
            }
        }

        if (Timer > 45 && ReturnTimer <= 0)
        {
            float maxDetectDistance = 256;
            NPC closestNpc = NPCHelper.FindClosestNPC(Projectile.position, maxDetectDistance);
            if (closestNpc != null)
            {
                AI_MoveToward(closestNpc.Center, 8);
            }
        }

        if (Timer % 7 == 0)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(2, 2);
            Color[] colors = new Color[] { Color.LightCyan, Color.Cyan, Color.Blue, Color.White };
            Color color = colors[Main.rand.Next(0, colors.Length)];
            float scale = Main.rand.NextFloat(0.5f, 0.8f);
        }

        Projectile.rotation += 0.05f;
        Lighting.AddLight(Projectile.position, Color.White.ToVector3() * 0.78f);
    }

    public float WidthFunction(float completionRatio)
    {
        float baseWidth = Projectile.scale * Projectile.width;
        return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
    }

    public Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(Main.DiscoColor * 0.3f, Color.Transparent, completionRatio);
    }
    private Color GetColorFunction(float completionRatio)
    {
        Color inColor = Color.White;
        Color trailColor = Color.Lerp(Color.SpringGreen, Color.DarkBlue, completionRatio);


        Color rainbow = Color.Red;
        float degrees = completionRatio * 360f;
        degrees += Main.GlobalTimeWrappedHourly * 400;
        degrees %= 360;
        rainbow.ScrollHue(degrees);
        //DrawUtilities.IncreaseHueBy(ref rainbow, degrees, out float hue);
        trailColor = Color.Lerp(trailColor, rainbow, 0.5f);
        Color easeColor = Color.Lerp(inColor, trailColor, EasingFunction.InExpo(Timer / 60f));
        return easeColor;
    }

    private float GetWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(10, 2, completionRatio);
    }

    private float GetWidthFunction2(float completionRatio)
    {
        return WidthFunction(completionRatio) * 2f;
    }


    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2"), Projectile.position);
        ReturnTimer = 60;
        Projectile.velocity = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver2);
        FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 6, baseSize: 0.08f);
        PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.SkyBlue, 12, 24);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(
            Color.White.R,
            Color.White.G,
            Color.White.B, 0) * (1f - Projectile.alpha / 50f);
    }

    private void DrawTrail(GraphicsDevice gDevice)
    {
        var shader2 = RichLaserShader.Instance;
        shader2.LaserColor = Color.White;
        shader2.LaserTexture = TrailRegistry.StarTrail;
        shader2.InnerColor = Color.Turquoise * 0.5f;
        shader2.OuterColor = Color.DarkTurquoise;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetColorFunction, GetWidthFunction, shader2, Projectile.Size * 0.5f);

        var bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.LightBlue * 0.5f;
        bloom.OuterColor = Color.DarkBlue;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetColorFunction, GetWidthFunction2, bloom, Projectile.Size * 0.5f);
    }

    private void DrawStar(SpriteBatch sb, Vector2 sp)
    {
        float scale = MathHelper.Lerp(0.3f, 1f, EasingFunction.InOutSine(Timer / 45f));
        SpritebatchDrawer starDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, Projectile.Center);
        starDrawer.color = Color.Lerp(Color.Lerp(Color.White, Color.LightBlue, 0.5f), Color.LightBlue, ExtraMath.Osc(0f, 1f, speed: 12));
        starDrawer.color.A = 0;
        starDrawer.rotation = MathHelper.Lerp(MathHelper.Pi, 0, EasingFunction.InOutSine(Timer / 45));
        starDrawer.scale *= 0.125f * scale;
        sb.Draw(starDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawer.color = Color.Lerp(Color.Blue, Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 12)) * 0.25f;
        glowDrawer.color.A = 0;
        glowDrawer.rotation = MathHelper.Lerp(MathHelper.Pi, 0, EasingFunction.InOutSine(Timer / 45));
        glowDrawer.scale *= 0.35f * scale;
        sb.Draw(glowDrawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {

        return false;
    }

    public override void OnKill(int timeLeft)
    {
        SoundStyle soundStyle = SoundRegistry.Niivi_StarringDeath;
        soundStyle.PitchVariance = 0.1f;
        SoundEngine.PlaySound(soundStyle, Projectile.position);
        for (int i = 0; i < 48; i++)
        {
            Vector2 velocity = Main.rand.NextVector2CircularEdge(4, 4);
            float scale = Main.rand.NextFloat(0.3f, 0.5f);
        }
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawStar);
        PixelationManager.QueuePrimitivesDrawAction(DrawTrail);
    }
}