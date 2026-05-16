using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Areas.MoonspiralTower.WeaponsMT;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class FlyingLeavesTome : AbstractMagicTome
{
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Void's Grasp");
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 6;
        Item.width = 50;
        Item.height = 50;
        Item.shoot = ModContent.ProjectileType<FlyingLeaf>();
        Item.shootSpeed = 15f;
        Item.mana = 4;
        Item.useTime = Item.useAnimation = 7;

    }

    public override Color GetTomeHintColor()
    {
        return Color.DarkGreen;
    }

    public override Asset<Texture2D> GetMagicCircleTexture()
    {
        return AssetManager.GlowMask.MagicCircle2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<Ivythorn>());
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        velocity = velocity.RotatedByRandom(MathHelper.ToRadians(12));
        velocity *= Main.rand.NextFloat(0.3f, 0.6f);
        position += Main.rand.NextVector2Circular(32, 32);
    }
}

public class FlyingLeaf : ModProjectile
{
    private Asset<Texture2D> _outlineTextureAsset;
    private Asset<Texture2D> _whiteTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    private ref float RandScale => ref Projectile.ai[1];
    private int Variant
    {
        get => (int)Projectile.ai[2];
        set => Projectile.ai[2] = value;
    }
    public override void Unload()
    {
        base.Unload();
        _outlineTextureAsset = null;
        _whiteTextureAsset = null;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 5;
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = 120;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1 && this.OwnedByLocalClient())
        {
            Variant = Main.rand.Next(6);
            RandScale = Main.rand.NextFloat(0.5f, 1f);
            Projectile.netUpdate = true;
        }
        if(Timer == 1)
        {
            ShockOvalSpawnParams spawnParams = new ShockOvalSpawnParams
            {
                innerColor = GetVariantColor(),
                outerColor = Color.Black
            };
            ShockOvalParticle sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.4f, spawnParams);
            sp.color *= 0.85f;
            sp.Scale *= 0.6f;

            sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.2f, spawnParams);
            sp.color *= 0.85f;
            sp.Scale *= 0.3f;
        }

        if (Projectile.velocity.Length() < 15)
            Projectile.velocity *= 1.01f;
        if(Timer % 8 == 0)
        {
            int dust = DustID.Grass;
            if (Variant == 1)
                dust = DustID.Dirt;
            if (Variant == 2)
                dust = DustID.Dirt;
            var d = Dust.NewDustPerfect(Projectile.Center, dust, Vector2.Zero);
            d.noGravity = true;
        }

        if (Timer > 30)
            Projectile.tileCollide = true;
        Projectile.scale = RandScale;
        Projectile.rotation = Projectile.velocity.ToRotation();
        DrawHelper.AnimateTopToBottom(Projectile, 4);
    }

    private Color GetVariantColor()
    {
        switch (Variant)
        {
            default:
            case 0:
                return Color.DarkGreen;
            case 1:
                return Color.Brown;
            case 2:
                return Color.DarkRed;
        }
    }
    private Asset<Texture2D> GetVariant()
    {
        switch (Variant)
        {
            default:
            case 0:
                return TextureAssets.Projectile[Type];
            case 1:
                return ModContent.Request<Texture2D>(Texture + "_0");
            case 2:
                return ModContent.Request<Texture2D>(Texture + "_1");
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        _whiteTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_White");

        SpritebatchDrawer butterflyDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        butterflyDrawer.texture = GetVariant().Value;
        Main.spriteBatch.Draw(butterflyDrawer);


        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            afDrawer.worldPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            afDrawer.rotation = Projectile.oldRot[i];

            float ratio = (float)i / (float)Projectile.oldPos.Length;
            afDrawer.scale = Vector2.Lerp(Vector2.One, Vector2.One * 0.5f, ratio);
            afDrawer.color = Color.Lerp(GetVariantColor(), Color.Black, ratio) * 0.5f;
            afDrawer.color.A = 0;
            Main.spriteBatch.Draw(afDrawer);
        }

        SpritebatchDrawer whiteDrawer = butterflyDrawer;
        whiteDrawer.texture = _whiteTextureAsset.Value;
        whiteDrawer.color = Color.Lerp(Color.White, Color.Transparent, EasingFunction.InExpo(Timer / 30f));
        Main.spriteBatch.Draw(whiteDrawer);

        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);
        glowDrawer.color = Color.Lerp(GetVariantColor(), Color.Black, ExtraMath.Osc(0f, 1f, speed: 6, Projectile.whoAmI)) * 0.2f;
        glowDrawer.scale *= 0.25f;
        glowDrawer.color.A = 0;
        glowDrawer.rotation = Main.GlobalTimeWrappedHourly;
        Main.spriteBatch.Draw(glowDrawer);

        SpritebatchDrawer outlineDrawer = butterflyDrawer;
        outlineDrawer.texture = _outlineTextureAsset.Value;

        Color outlinerColor = Color.Lerp(GetVariantColor(), Color.White, 0.5f);
        outlineDrawer.color = Color.Lerp(outlinerColor * 0.66f, outlinerColor * 0.2f, ExtraMath.Osc(0f, 1f, speed: 6, offset: Projectile.whoAmI));
        Main.spriteBatch.Draw(outlineDrawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (float f = 0; f < 3; f++)
        {
            var dp = Particle<DustParticle>.Spawn(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(65)) * Main.rand.NextFloat(3, 5), Scale: Main.rand.NextFloat(0.5f, 1f));
            dp.innerColor = Color.Lerp(GetVariantColor(), Color.White, 0.5f);
            dp.outerColor = GetVariantColor();
            dp.noTileCollide = true;
            dp.gravity = 0;
            dp.dampening = 0.1f;
            dp.Scale *= 0.6f;
        }

    }
}