using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.WeaponsRC;

public class SparklestarArtifact : ModItem
{
    private int _dir;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.damage = 277;
        Item.width = 16;
        Item.height = 16;
        Item.channel = false;
        Item.autoReuse = true;
        Item.mana = 25;
        Item.useAnimation = Item.useTime = 24;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 2;
        Item.crit = 4;
        Item.shoot = ModContent.ProjectileType<SparkleStarSwing>();
        Item.shootSpeed = 15;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool AltFunctionUse(Player player)
    {
        return false;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if(_dir == 1)
        {
            _dir = -1;
        }
        else
        {
            _dir = 1;
        }

        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: _dir);
        return false;
        //return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(),
            material: ModContent.ItemType<AlcaricMush>());
    }
}

public class SparkleStarSwing : ModProjectile,
    IDrawToRenderTarget
{
    private Vector2 _initialVelocity;
    private ref float Timer => ref Projectile.ai[0];
    private ref float Dir => ref Projectile.ai[1];
    private Player Owner => Main.player[Projectile.owner];
    private float SwingTime => 60f;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_initialVelocity);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _initialVelocity = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.timeLeft = 60;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    private Vector2 CalculatePositionAtTimeStep(float time)
    {
        float swingDistance = 500;
        float swingTime = SwingTime;
        float ratio = time / swingTime;
        float upDistance = MathHelper.Lerp(0, 120, EasingFunction.QuadraticBump(ratio));

        float easeIn = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(ratio));
        float easeOut = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(ratio));
        Vector2 thrustOffset = Vector2.Lerp(Vector2.Zero, _initialVelocity.SafeNormalize(Vector2.Zero) * swingDistance, easeIn * easeOut);
        Vector2 up = _initialVelocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
        Vector2 upwardOffset = Vector2.Lerp(up, -up, EasingFunction.InOutSine(ratio)) * upDistance * Dir;
        Vector2 fullyOffset = upwardOffset + thrustOffset;
        Vector2 positionToMoveTo = Owner.Center + fullyOffset;
        return positionToMoveTo;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle useSound = new SoundStyle("Stellamod/Assets/Sounds/LaserChannel");

            useSound = useSound with { PitchVariance = 0.5f };
            useSound.MaxInstances = 3;
            useSound.Pitch = -0.5f;
            useSound.Volume = 0.15f;
            SoundEngine.PlaySound(useSound, Projectile.position);

            SoundStyle useSound2 = new SoundStyle("Stellamod/Assets/Sounds/StarringDeath") with { PitchVariance = 0.3f };
          //  useSound2.Volume = 0.2f;
            SoundEngine.PlaySound(useSound2, Projectile.position);
            _initialVelocity = Projectile.velocity;
        }

        Projectile.velocity = CalculatePositionAtTimeStep(Timer) - Projectile.Center;


        if(Timer % 11 == 0)
        {
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetItemSource_FromThis(), Projectile.Center, Vector2.Zero, 
                    ModContent.ProjectileType<SparklePowder>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        if (Main.rand.NextBool(4))
        {
            var sparkle = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(24, 24), Vector2.Zero, Scale: Main.rand.NextFloat(0.3f, 0.6f));
            sparkle.noTileCollide = true;
            sparkle.gravity = 0f;
            sparkle.dampening = 0.05f;
            sparkle.outerColor = Color.Goldenrod;
            sparkle.Scale *= 0.6f;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {

        SpritebatchDrawer sparkleStarArtifactDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sparkleStarArtifactDrawer.VerticalFrame(1, 3);
        for (int i = 0; i < 16; i++)
        {
           Vector2 oldPos = CalculatePositionAtTimeStep(Timer - i);
            sparkleStarArtifactDrawer.color = Color.Lerp(Color.Goldenrod, Color.Transparent, (float)i / 16f) * 0.1f;
            sparkleStarArtifactDrawer.color.A = 0;
            sparkleStarArtifactDrawer.worldPosition = oldPos;
            Main.spriteBatch.Draw(sparkleStarArtifactDrawer);
        }

        sparkleStarArtifactDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sparkleStarArtifactDrawer);

        sparkleStarArtifactDrawer.VerticalFrame(2, 3);
        sparkleStarArtifactDrawer.color = Color.Lerp(Color.Transparent, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 32));
        Main.spriteBatch.Draw(sparkleStarArtifactDrawer);

        sparkleStarArtifactDrawer.VerticalFrame(1, 3);
        sparkleStarArtifactDrawer.color = Color.Lerp(Color.Yellow, Color.Transparent, EasingFunction.OutExpo(Timer / 45f));
        Main.spriteBatch.Draw(sparkleStarArtifactDrawer);

        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    private void DrawSwingTrail(GraphicsDevice gDevice)
    {
        Vector2[] oldSwingPos = new Vector2[16];
        for(int i = 0; i < oldSwingPos.Length; i++)
        {
            oldSwingPos[i] = CalculatePositionAtTimeStep(Timer - i);
        }

        var shader2 = RichLaserShader.Instance;
        shader2.LaserColor = Color.White;
        shader2.LaserTexture = TrailRegistry.WaterTrail;
        shader2.InnerColor = Color.LightGoldenrodYellow * 0.5f;
        shader2.OuterColor = Color.DarkGoldenrod;
        TrailDrawer.Draw(Main.spriteBatch, oldSwingPos, ColorFunction, WidthFunction, shader2);

        var bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.LightGoldenrodYellow * 0.5f;
        bloom.OuterColor = Color.DarkGoldenrod;
        TrailDrawer.Draw(Main.spriteBatch, oldSwingPos, ColorFunction, WidthFunction2, bloom);
    }

    private Color ColorFunction(float completionRatio)
    {
        Color inColor = Color.White;
        Color trailColor = Color.Lerp(Color.LightGoldenrodYellow, Color.DarkGoldenrod, completionRatio);
        Color easeColor = Color.Lerp(inColor, trailColor, EasingFunction.InExpo(Timer / 60f)) ;
        easeColor = Color.Lerp(easeColor, Color.Black, completionRatio);
        return easeColor;
    }

    private float WidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(16, 2, completionRatio);
    }

    private float WidthFunction2(float completionRatio)
    {
        return WidthFunction(completionRatio) * 2f;
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawSwingTrail);
    }
}

public class SparklePowder : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.friendly = true;
        Projectile.timeLeft = 45;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Main.rand.NextBool(12))
        {
            for (int i = 0; i < 2; i++)
            {
                var sparkle = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(24, 24), Vector2.Zero, Scale: Main.rand.NextFloat(0.3f, 0.6f));
                sparkle.noTileCollide = true;
                sparkle.gravity = 0f;
                sparkle.dampening = 0.05f;
                sparkle.outerColor = Color.Goldenrod;
            }
        }

        if (Main.rand.NextBool(12))
        {
            for (int i = 0; i < 1; i++)
            {
                var smoke = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(24, 24), Vector2.Zero);
                smoke.fadeToColor = Color.Black;
                smoke.color = Color.Lerp(Color.Purple, Color.Black, 0.85f);
                smoke.Scale *= 0.4f;
                smoke.behindLayer = true;
            }
        }

        if (Timer == 15)
        {
            PixelPrimitiveCircleFactory.CreateInGoldBoom(Projectile.Center);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<FourSparkBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Main.rand.NextFloat(0.5f, 1f));
        }
    }
}
public class FourSparkBoom : ModProjectile,
    IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float Scale => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 6;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle explosioNSound = new SoundStyle("Stellamod/Assets/Sounds/Starexplosion") with { PitchVariance = 0.4f };
            SoundEngine.PlaySound(explosioNSound, Projectile.position);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            for (int i = 0; i < 6; i++)
            {
                var sparkle = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(16, 16), Main.rand.NextVector2CircularEdge(7, 7), Scale: Main.rand.NextFloat(0.3f, 0.6f));
                sparkle.noTileCollide = true;
                sparkle.gravity = 0f;
                sparkle.dampening = 0.05f;
                sparkle.outerColor = Color.Goldenrod;
                sparkle.Scale *= Scale;
                sparkle.fast = true;
            }

            for (int i = 0; i < 8; i++)
            {
                var smoke = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(24, 24), Vector2.Zero);
                smoke.fadeToColor = Color.Black;
                smoke.color = Color.Lerp(Color.Purple, Color.Black, 0.85f);
                smoke.Scale *= 1f * Scale;
                smoke.behindLayer = true;
            }
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGoldenrodYellow, Color.DarkGoldenrod, 3, baseSize: 0.24f);
            fx.Scale *= 0.5f * Scale;

            var fx2 = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGoldenrodYellow, Color.DarkGoldenrod, 8, baseSize: 0.24f);
            fx2.Scale *= Scale;
        }

        if (Main.rand.NextBool(5))
        {
            var sparkle = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(16, 16), Vector2.Zero, Scale: Main.rand.NextFloat(0.3f, 0.6f));
            sparkle.noTileCollide = true;
            sparkle.gravity = 0f;
            sparkle.dampening = 0.05f;
            sparkle.outerColor = Color.Goldenrod;
            sparkle.Scale *= 0.6f * Scale;
        }

        float frameSpeed = 4;
        Projectile.frameCounter++;
        if (Projectile.frameCounter >= frameSpeed)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.Kill();
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {

        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    private void DrawPixelatedBoom(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.scale = Vector2.One * Scale;
        sbDrawer.color = Color.Goldenrod * 0.5f;
        sbDrawer.color.A = 0;
        Main.spriteBatch.Draw(sbDrawer);

        sbDrawer.color = Color.White;
        sbDrawer.color.A = 0;
        sbDrawer.scale *= 0.7f;
        Main.spriteBatch.Draw(sbDrawer);
    }
    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedBoom);
    }
}