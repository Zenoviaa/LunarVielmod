using ReLogic.Content;
using Stellamod.Common.ArmorRework;
using Stellamod.Content.Armors.Velioza;
using Stellamod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Stellamod.Content.Vanity.VerliaHat;

public class VerliaHatDrawLayer : PlayerDrawLayer
{

    public override bool IsHeadLayer => true;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.head == ModContent.GetInstance<VerliaHat>().Item.headSlot || drawInfo.drawPlayer.head == ModContent.GetInstance<VerliaHatMoon>().Item.headSlot;

    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        var headSlot = drawInfo.drawPlayer.head;
        var type = ModContent.ItemType<VerliaHat>();

        var parameters = HatDrawParameters.Default;
        var data = CostumeUtilities.GetHatDrawData(ref drawInfo, TextureAssets.Item[ModContent.ItemType<VerliaHat>()], parameters);
        data.position.Y -= 2;
        drawInfo.DrawDataCache.Add(data);

        if (headSlot == ModContent.GetInstance<VerliaHatMoon>().Item.headSlot)
        {
            data = CostumeUtilities.GetHatDrawData(ref drawInfo, AssetReferences.Content.Vanity.VerliaHat.VerliaHat_Moon.Asset, parameters);
            drawInfo.DrawDataCache.Add(data);
        }
    }
}



[AutoloadEquip(EquipType.Head)]
public class VerliaHat : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.vanity = true;
    }

}
[AutoloadEquip(EquipType.Head)]
public class VerliaHatMoon : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.vanity = true;
    }
}