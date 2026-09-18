using Stellamod.Assets;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;

public class StarCall : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToArtifact();
        Item.damage = 250; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
        Item.mana = 150;
        Item.useTime = 18;
        Item.useAnimation = 18;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 4;
        Item.shoot = ModContent.ProjectileType<Starbomb>();
        Item.shootSpeed = 8f; // the speed of the projectile (measured in pixels per frame)
        Item.noUseGraphic = true;
        Item.noMelee = true;
    }
    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
    {
        Lighting.AddLight(Item.position, 0.46f, .07f, .52f);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<MiracleThread>());
    }
}


public class Starbomb : ModProjectile
{

    private ref float Timer => ref Projectile.ai[0];
    private float ScaleProgress => Easing.InExpo(Timer / 60f);
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
    }

    public override void AI()
    {
        base.AI();

        if (Main.rand.NextBool(8))
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Yellow, Main.rand.NextFloat(1f, 3f)).noGravity = true;
        }
        if (Main.rand.NextBool(8))
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Purple, Main.rand.NextFloat(1f, 3f)).noGravity = true;
        }
        Projectile.velocity *= 0.98f;
        Projectile.rotation += Projectile.velocity.Length() * 0.05f + 0.05f;
        if (Projectile.velocity.Length() <= 0.25f)
        {
            Timer++;
            if (Timer >= 60)
            {
                Projectile.Kill();
            }
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }


    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer afterDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            float ratio = i / (float)Projectile.oldPos.Length;
            afterDrawer.worldPosition = pos;
            afterDrawer.color = Color.Lerp(Color.Yellow, Color.Black, ratio) * 0.5f;
            afterDrawer.color.A = 0;
            Main.spriteBatch.Draw(afterDrawer);
        }

        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        base.PostDraw(lightColor);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        float heat = EasingFunction.InOutSine(Timer / 60f);
        glowDrawer.scale *= 0.5f;
        glowDrawer.scale *= MathHelper.Lerp(0.75f, 1f, EasingFunction.InOutSine(Timer / 60f));
        glowDrawer.color = Color.Lerp(Color.Black, Color.Yellow, heat);
        glowDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowDrawer);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<StarBoomer>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}

public class StarBoomer : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 120;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        float progress = Timer / 120f;
        int divisor = (int)MathHelper.Lerp(20, 10, progress);
        if (Timer % divisor == 0 || Timer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(24, 24), Vector2.Zero,
                    ModContent.ProjectileType<StarBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}

public class StarBoom : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 15;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {

            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            for (float f = 0; f < 16; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Yellow, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            SoundStyle morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/MorrowExp");
            morrowExp.PitchVariance = 0.3f;
            SoundEngine.PlaySound(morrowExp, Projectile.position);

            switch (Main.rand.Next(3))
            {
                case 0:
                    morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower1");
                    break;
                case 1:
                    morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower1");
                    break;
                case 2:
                    morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower3");
                    break;
            }

            morrowExp.PitchVariance = 0.3f;
            SoundEngine.PlaySound(morrowExp, Projectile.position);

            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Blue, duration: 25, baseSize: 0.24f);

            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Blue,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                float oofed = Main.rand.NextFloat(-1f, 1f);
                for (float f = 0; f < 4; f++)
                {
                    float rot = (f / 4f) * MathHelper.TwoPi;
                    rot += oofed;
                    Vector2 velocity = rot.ToRotationVector2() * 8;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ProjectileID.SuperStar, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }
    }
}