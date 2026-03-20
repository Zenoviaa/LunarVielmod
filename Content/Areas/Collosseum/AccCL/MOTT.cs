using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.AccCL;

public class MOTTPlayer : ModPlayer
{
    public bool hasMOTT;
    public float mottTimer;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasMOTT = false;
    }
    public override void UpdateEquips()
    {
        base.UpdateEquips();
        if (Main.myPlayer != Player.whoAmI)
        {
            return;
        }
        if (!hasMOTT)
            return;

        mottTimer++;
        if (mottTimer % 180 == 0)
        {
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Bottom, Player.velocity * 0.2f, ProjectileID.SpikyBall, 10, 1, Player.whoAmI);
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Bottom, -Player.velocity * 0.2f, ProjectileID.SpikyBall, 10, 1, Player.whoAmI);
        }
    }
}

public class MOTT : ModItem
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 28;
        Item.value = Item.sellPrice(silver: 12);
        Item.rare = ItemRarityID.Green;
        Item.accessory = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<MOTTPlayer>().hasMOTT = true;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<GintzlMetal, BlankAccessory>();
    }
}