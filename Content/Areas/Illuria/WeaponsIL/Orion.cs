using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Projectiles.Thrown;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL;

public class Orion : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToArtifact();
        Item.width = 44;
        Item.height = 58;
        Item.damage = 400;
        Item.DamageType = DamageClass.Magic;

        Item.useTime = 32;
        Item.useAnimation = 32;
        Item.useStyle = ItemUseStyleID.Swing;

        Item.knockBack = 6;
        Item.value = Item.sellPrice(0, 20, 0, 0);
        Item.noUseGraphic = true;

        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<OrionProj>();
        Item.shootSpeed = 15;
        Item.mana = 25;
    }

    public override void AddRecipes()
    {
        this.RegisterBrew<GhastlySpirit, BlankStaff>();
    }
}

public class OrionProj : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 27;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.hostile = false;
    }

    public override void AI()
    {
        Timer++;
        Projectile.velocity.Y += 0.1f;
        Projectile.rotation = Projectile.velocity.ToRotation();


        if (Timer % 15 == 0)
        {
            //Spawn Star
            if (this.OwnedByLocalClient())
            {
                Vector2 offset = Main.rand.NextVector2Circular(24, 24);
                Vector2 velocity = Main.rand.NextVector2Circular(2, 2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, velocity,
                    ModContent.ProjectileType<OrionStarProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        if (Main.rand.NextBool(4))
        {
            var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity.SafeNormalize(Vector2.Zero));
            sp.dampening = 0.05f;
            sp.noTileCollide = true;
            sp.outerColor = Color.Blue;
            sp.innerColor = Color.White;
            sp.Scale *= 0.6f;
            sp.fast = true;
            sp.gravity = 0;
        }

        if (Timer % 8 == 0)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                ModContent.DustType<Sparkle>(), newColor: Color.White);
        }
    }


    public override void OnKill(int timeLeft)
    {
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<SiriusBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

        }

        //Play Sound
        switch (Main.rand.Next(2))
        {
            case 0:
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb1"), Projectile.position);
                break;
            case 1:
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb2"), Projectile.position);
                break;
        }
    }

    public float WidthFunction(float completionRatio)
    {
        float baseWidth = Projectile.scale * Projectile.width;
        return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
    }

    public Color ColorFunction(float completionRatio)
    {
        return Color.Lerp(ColorFunctions.Niivin, Color.Black, completionRatio);
    }

    private void DrawAuraTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader basicLaserShader = ShaderContent.GetInstance<BasicLaserShader>();
        basicLaserShader.LaserTexture = AssetManager.LaserTextures.Aura;
        basicLaserShader.InnerColor = Color.White;
        basicLaserShader.OuterColor = Color.SkyBlue;
        TrailDrawer.Draw(Projectile.oldPos, ColorFunction, WidthFunction, basicLaserShader, Projectile.Size * 0.5f);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawAuraTrail);
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.Blue, Color.Purple * 0.4f, 0.3f);
        SpritebatchDrawer orionDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(orionDrawer);

        orionDrawer.VerticalFrame(1, Main.projFrames[Type]);
        orionDrawer.color = Color.Lerp(Color.White, Color.Transparent, EasingFunction.InOutSine(Timer / 38));
        Main.spriteBatch.Draw(orionDrawer);

        //Main.spriteBatch.Draw(orionDrawer);

        orionDrawer.VerticalFrame(2, Main.projFrames[Type]);
        orionDrawer.color = Color.Lerp(Color.White, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 12));
        Main.spriteBatch.Draw(orionDrawer);
        return false;
    }
}
public class OrionStarProj : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        Timer++;
        if (Timer % 4 == 0)
        {
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.GemSapphire, Scale: 0.6f);
        }

        if(Timer == 25)
        {
           for(float f = 0; f < 4; f++)
            {
                var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8));
                dp.dampening = 0.05f;
                dp.outerColor = Color.SkyBlue;
                dp.innerColor = Color.White;
                dp.gravity = 0;
                dp.noTileCollide = true;
               
            }
        }

        if (Timer >= 30)
        {
            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<OrionStarBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            }

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            for (float f = 0; f < 8; f++)
            {
                var dp = DustParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(20, 20));
                dp.dampening = 0.1f;
                dp.outerColor = Color.SkyBlue;
                dp.innerColor = Color.White;
                dp.gravity = 0;
                dp.noTileCollide = true;
                dp.Scale *= 0.6f;
                dp.fast = true;
                dp.superFast = true;
            }
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.SkyBlue, Color.DarkBlue, 25, 40);
            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.SkyBlue, Color.Blue, Color.DarkBlue, duration: 10, baseSize: 0.17f);
            fx.Scale *= 1.6f;
            Projectile.Kill();
            Timer = 0;
        }
        Projectile.velocity *= 0.9f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer starDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        float alpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 15f));
        starDrawer.color = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.Blue, 0.75f), ExtraMath.Osc(0f, 1f, speed: 16)) * alpha;
        starDrawer.rotation = MathHelper.Lerp(0.75f, 0f, EasingFunction.InOutSine(Timer / 30f));
        starDrawer.scale *= MathHelper.Lerp(2f, 0f, EasingFunction.InOutSine(Timer / 30f));
        Main.spriteBatch.Draw(starDrawer);

        starDrawer.color = Color.Lerp(Color.SkyBlue, Color.White, ExtraMath.Osc(0f, 1f, speed: 16)) * alpha;
        starDrawer.VerticalFrame(1, Main.projFrames[Type]);
        Main.spriteBatch.Draw(starDrawer);
        return false;

    }
}
