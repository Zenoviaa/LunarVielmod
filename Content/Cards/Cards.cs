using Stellamod.Common.IgnitersNPowders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
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