using Stellamod.Assets;
using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.AccMT;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Cards;

public class IvynCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 6;

    }
    public override int GetPowderSlotCount()
    {
        return 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<Ivythorn>());
    }
}
public class FableCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 10;

    }
    public override int GetPowderSlotCount()
    {
        return 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<AlcadizScrap>());
    }
}
public class GintzeCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 14;
    }

    public override int GetPowderSlotCount()
    {
        return 3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<GintzlMetal>());
    }
}
public class RingedCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 9;
    }

    public override int GetPowderSlotCount()
    {
        return 3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<MinersGold>());
    }
}

public class BloodyCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 11;
    }

    public override int GetPowderSlotCount()
    {
        return 3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<TerrorFragments>());
    }
}

public class WinterCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 11;
    }

    public override int GetPowderSlotCount()
    {
        return 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<WinterbornShard>());
    }
}

public class CinderedCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 16;
    }

    public override int GetPowderSlotCount()
    {
        return 3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<Cinderscrap>());
    }
}

public class ConvulgingCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 18;
    }

    public override int GetPowderSlotCount()
    {
        return 3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<ConvulgingMater>());
    }
}



public class LarvaedCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 14;
    }

    public override int GetPowderSlotCount()
    {
        return 3;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankCard>(), material: ModContent.ItemType<HypnotizedSoul>());
    }
}

public class MooneskCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 15;
        Item.shoot = ModContent.ProjectileType<MooneskCardProj>();
    }
    public override int GetPowderSlotCount()
    {
        return 4;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankCard>(), material: ModContent.ItemType<PearlescentScrap>());
    }
}

public class MooneskCardPlayer : ModPlayer
{
    public int hitCount;
}
public class MooneskCardProj : IgniterCardProjectile
{
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        MooneskCardPlayer cardPlayer = Owner.GetModPlayer<MooneskCardPlayer>();
        cardPlayer.hitCount++;
        if (cardPlayer.hitCount >= 10)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + new Vector2(0, -96), Vector2.Zero,
                ModContent.ProjectileType<MoonramMoon>(), damageDone * 10, Projectile.knockBack, Projectile.owner);
            cardPlayer.hitCount = 0;
        }
    }
    public override void DrawToRenderTargets()
    {
        base.DrawToRenderTargets();
        void DrawMoonyTrail(GraphicsDevice gDevice)
        {
            float GetSpiralDashTrailWidth(float completionRatio)
            {
                return MathHelper.SmoothStep(128, 96, completionRatio) * EasingFunction.QuadraticBump(completionRatio) * 0.35f;
            }
            float GetSpiralDashTrailWidth2(float completionRatio)
            {
                return GetSpiralDashTrailWidth(completionRatio) * 1.3f;
            }
            Color GetSpiralDashTrailColor(float completionRatio)
            {
                return Color.Lerp(Color.White, Color.Transparent, completionRatio);
            }


            BasicLaserShader bloomShader = ShaderContent.GetInstance<BasicLaserShader>();
            bloomShader.LaserTexture = AssetManager.LaserTextures.CometTrail;
            bloomShader.InnerColor = Color.SkyBlue;
            bloomShader.OuterColor = Color.DarkBlue;
            TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth2, bloomShader, Projectile.Size * 0.5f);

            BasicLaserShader basicLaserShader = ShaderContent.GetInstance<BasicLaserShader>();
            basicLaserShader.LaserTexture = AssetManager.LaserTextures.Aura;
            basicLaserShader.InnerColor = Color.SkyBlue;
            basicLaserShader.OuterColor = Color.DarkBlue;
            TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth2, basicLaserShader, Projectile.Size * 0.5f);


            basicLaserShader.InnerColor = Color.White;
            basicLaserShader.OuterColor = Color.DarkGray;
            TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth, basicLaserShader, Projectile.Size * 0.5f);
        }
        PixelationManager.QueuePrimitivesDrawAction(DrawMoonyTrail);
    }
}

public class EreshkigalsCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 200;
    }
    public override int GetPowderSlotCount()
    {
        return 6;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<EreshkinCandle>());
    }
}
public class RadiantCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 210;
    }
    public override int GetPowderSlotCount()
    {
        return 6;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<RadiantNectar>());
    }
}

public class FenixCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 225;
    }
    public override int GetPowderSlotCount()
    {
        return 6;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<AlcaricMush>());
    }
}

public class JunkyCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 45;
    }
    public override int GetPowderSlotCount()
    {
        return 4;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<MechanizedSoul>());
    }
}

public class GhetsisCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 30;
        Item.shoot = ModContent.ProjectileType<GhetsisCardProj>();
    }
    public override int GetPowderSlotCount()
    {
        return 4;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<MarshScrap>());
    }
}

public class GhetsisCardProj : IgniterCardProjectile
{
    private int _hitCount;
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    protected override void OnExplode()
    {
      //  base.OnExplode();
        _hitCount++;
        Projectile.velocity.X *= -1;
        Projectile.velocity.Y -= 7;
        if(_hitCount >= 3)
        {
            Projectile.Kill();
        }
    }
}

public class YaoiYuriCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 50;
    }
    public override int GetPowderSlotCount()
    {
        return 4;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<KaleidoscopicInk>());
    }
}

public class SiegfriedsCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 60;
    }

    public override int GetPowderSlotCount()
    {
        return 5;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<IllurineScale>());
    }
}

public class MiracleCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 75;
    }

    public override int GetPowderSlotCount()
    {
        return 5;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankCard>(),
            material: ModContent.ItemType<MiracleThread>());
    }
}