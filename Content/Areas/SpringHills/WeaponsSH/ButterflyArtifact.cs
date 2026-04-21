using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Collosseum.WeaponsCL;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;


public class ButterflyArtifact : ModItem
{
    private int _dir;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.damage = 7;
        Item.width = 16;
        Item.height = 16;
        Item.mana = 20;
        Item.useAnimation = Item.useTime = 32;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item43 with { PitchVariance = 0.4f, Volume = 0.1f };
        Item.knockBack = 2;
        Item.shoot = ModContent.ProjectileType<MagicalButterfly>();
        Item.shootSpeed = 12;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
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
        SoundStyle castSound;
        switch (Main.rand.Next(2))
        {
            default:
            case 0:
                castSound = new SoundStyle("Stellamod/Assets/Sounds/Magic/HolyCast1");
                break;
            case 1:
                castSound = new SoundStyle("Stellamod/Assets/Sounds/Magic/HolyCast2");
                break;
        }
        castSound.PitchVariance = 0.4f;
        castSound.Volume = 0.3f;
        castSound.MaxInstances = 1;
        SoundEngine.PlaySound(castSound, player.position);

        for (float f = 0; f < 3; f++)
        {
            Vector2 bposition = position + Main.rand.NextVector2Circular(64, 64);
            Projectile.NewProjectile(source, bposition, velocity * Main.rand.NextFloat(0.6f, 1f), type, damage, knockback, player.whoAmI);
        }

        var p = Projectile.NewProjectileDirect(source, player.Center, velocity, 
            ModContent.ProjectileType<StaffWaveHold>(), damage, knockback, player.whoAmI,
            ai2: _dir);
        (p.ModProjectile as StaffWaveHold).MagicCircleStyle = 1;
        return false;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(),
            material: ModContent.ItemType<Ivythorn>());
    }
}

public class MagicalButterfly : ModProjectile
{
    private Vector2 _initialVelocity;
    private Asset<Texture2D> _outlineTextureAsset;
    private Asset<Texture2D> _whiteTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Main.projFrames[Type] = 6;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 180;
        Projectile.friendly = true;
        Projectile.light = 0.6f;
    }

    public override void Unload()
    {
        base.Unload();
        _outlineTextureAsset = null;
        _whiteTextureAsset = null;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            _initialVelocity = Projectile.velocity;

        }
        if (Timer % 12 == 0)
        {
            var sp = SparkleParticle.Spawn(Projectile.Center, Vector2.Zero, Scale: 0.5f);
            sp.innerColor = Color.LightPink;
            sp.outerColor = Color.DarkViolet;
            sp.flickering = true;
            sp.fast = true;
            sp.gravity = 0;
        }

        if (Main.rand.NextBool(16))
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, Scale: 0.5f);
        }

        if (Timer >= 30)
        {
            Projectile.tileCollide = true;
        }

        Projectile.velocity = _initialVelocity.RotatedBy(MathF.Sin(Timer * 0.125f) * 0.1f);
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Projectile.timeLeft < 30)
        {
            Projectile.velocity.Y -= 0.05f;
        }
        DrawHelper.AnimateTopToBottom(Projectile, 4);
    }

    private void DrawPixelatedTrail(GraphicsDevice gDevice)
    {
        var shader = BasicLaserShader.Instance;
        shader.InnerColor = Color.White;
        shader.OuterColor = Color.White;
        TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, shader, Projectile.Size * 0.5f);
    }

    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.White, Color.Transparent, ratio);
    }
    private float GetTrailWidth(float ratio)
    {
        return 2 * MathF.Sin(ratio * 16) * MathHelper.Lerp(1f, 0f, ratio);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        _whiteTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_White");

        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedTrail);

        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            afDrawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            afDrawer.rotation = Projectile.oldRot[i];

            float ratio = (float)i / (float)Projectile.oldPos.Length;
            afDrawer.scale = Vector2.Lerp(Vector2.One, Vector2.One * 0.5f, ratio);
            afDrawer.color = Color.Lerp(Color.LightPink, Color.DarkViolet, ratio) * 0.15f;
            afDrawer.color.A = 0;
            Main.spriteBatch.Draw(afDrawer);
        }

        SpritebatchDrawer butterflyDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(butterflyDrawer);

        SpritebatchDrawer whiteDrawer = butterflyDrawer;
        whiteDrawer.texture = _whiteTextureAsset.Value;
        whiteDrawer.color = Color.Lerp(Color.White, Color.Transparent, EasingFunction.InExpo(Timer / 30f));
        Main.spriteBatch.Draw(whiteDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);
        glowDrawer.color = Color.Lerp(Color.LightPink, Color.DarkViolet, ExtraMath.Osc(0f, 1f, speed: 6, Projectile.whoAmI)) * 0.2f;
        glowDrawer.scale *= 0.25f;
        glowDrawer.color.A = 0;
        glowDrawer.rotation = Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(glowDrawer);

        SpritebatchDrawer outlineDrawer = butterflyDrawer;
        outlineDrawer.texture = _outlineTextureAsset.Value;
        outlineDrawer.color = Color.Lerp(Color.White * 0.66f, Color.White * 0.2f, ExtraMath.Osc(0f, 1f, speed: 6, offset: Projectile.whoAmI));
        Main.spriteBatch.Draw(outlineDrawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        var fx = FXUtil.GlowCircleBoom(Projectile.Center,
            innerColor: Color.White,
            glowColor: Color.LightPink,
            outerGlowColor: Color.DarkViolet, duration: 25, baseSize: 0.18f);
        fx.Scale *= 0.4f;
        for (float f = 0; f < 3; f++)
        {
            var dp = Particle<DustParticle>.Spawn(Projectile.Center, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3, 5), Scale: Main.rand.NextFloat(0.5f, 1f));
            dp.innerColor = Color.LightPink;
            dp.outerColor = Color.DarkViolet;
            dp.noTileCollide = true;
            dp.gravity = 0;
            dp.dampening = 0.1f;
        }

        SoundStyle softSummon = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2");
        softSummon.PitchVariance = 0.3f;
        softSummon.Volume = 0.25f;
        SoundEngine.PlaySound(softSummon, Projectile.position);
    }
}