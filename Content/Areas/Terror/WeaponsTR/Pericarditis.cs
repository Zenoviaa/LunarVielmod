using Stellamod.Assets;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Magic;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.WeaponsTR;

public class Pericarditis : ModItem
{
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Sun Blast Staff");
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.DefaultToArtifact();
        Item.staff[Item.type] = true;
        Item.damage = 25;
        Item.width = 50;
        Item.height = 50;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 4;
        Item.value = Item.sellPrice(0, 1, 1, 29);
        Item.rare = ItemRarityID.Green;
        Item.shootSpeed = 35;
        Item.autoReuse = true;

        Item.DamageType = DamageClass.Magic;
        Item.shoot = ModContent.ProjectileType<PericarditisProj>();
        Item.shootSpeed = 12;
        Item.mana = 35;
        Item.useAnimation = 44;
        Item.useTime = 44;
        Item.consumeAmmoOnLastShotOnly = true;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-5f, 0f);
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(),
            material: ModContent.ItemType<TerrorFragments>());
    }
}
public class PericarditisProj : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Pericarditis");
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.penetrate = 1;
        Projectile.friendly = true;
        Projectile.timeLeft = 180;
    }

    public override void AI()
    {
        Timer++;
        if (Timer == 1)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Pericarditis"), Projectile.position);
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Projectile.velocity *= 1.02f;
        if (Timer % 5 == 0)
        {
            int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch, 0f, 0f, 150, Color.White, 1f);
            Main.dust[dustnumber].velocity *= 0.3f;
        }
        if (Timer % 10 == 0)
        {
            SparkleParticle dp = SparkleParticle.Spawn(Projectile.Center, Vector2.Zero, Color.Red, Scale: 1);
            dp.gravity = 0;
            dp.Scale *= 0.5f;
        }
    }

    private void DrawPixelatedAura(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, Projectile.Center);
        drawer.color = Color.Red;
        drawer.blackIsTransparency = true;
        drawer.rotation = Main.GlobalTimeWrappedHourly * 9;
        drawer.scale *= 0.25f;
        sb.Draw(drawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Vector2 drawOrigin = texture.Size() / 2f;
        float drawRotation = Projectile.rotation;

        SpriteBatch spriteBatch = Main.spriteBatch;
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 trailDrawPos = (Projectile.oldPos[k] - Main.screenPosition) + Projectile.Size / 2 + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            color.A = 0;
            spriteBatch.Draw(texture, trailDrawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }

        for (float f = 0f; f < 1f; f += 0.1f)
        {
            float rot = f * MathHelper.TwoPi;
            Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(0.5f, 1f) * 3;
            spriteBatch.Draw(texture, drawPos + offset, null, Color.White * 0.2f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }

        spriteBatch.Draw(texture, drawPos, null, Color.White * 0.2f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);

        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedAura);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Lighting.AddLight(Projectile.Center, Color.DarkRed.ToVector3() * 1.75f * Main.essScale);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        int Sound = Main.rand.Next(1, 3);
        if (Sound == 1)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn1"), Projectile.position);
        }
        else
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn2"), Projectile.position);
        }

        for (int i = 0; i < 8; i++)
        {
            float progress = (float)i / 8f;
            float rot = progress * MathHelper.TwoPi;
            Vector2 vel = rot.ToRotationVector2() * 4;
            Dust.NewDustPerfect(Projectile.Center, DustID.RedTorch, vel, Scale: 1f);
        }

        var p3 = FXUtil.GlowCircleBoom(Projectile.Center,
            innerColor: Color.White,
            glowColor: Color.Red,
            outerGlowColor: Color.DarkRed, duration: 15, baseSize: .09f);
        p3.Scale *= 3f;
        for (float n = 0; n < 6f; n++)
        {
            var spawnParams = new DustParticleSpawnParams();
            spawnParams.innerColor = Color.White;
            spawnParams.outerColor = Color.DarkRed;
            spawnParams.scaleRange = new Vector2(0.1f, 1f);
            DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.15f, 0.5f), spawnParams);
        }

        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Top, -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(45)) * 15,
                                      ModContent.ProjectileType<BloodWaterProj>(), Projectile.damage / 2, 1f, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<PericarditisBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
