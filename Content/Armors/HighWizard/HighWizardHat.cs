using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Stellamod.Content.Armors.HighWizard;

public class HighWizardLegsDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => false;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.legs == ModContent.GetInstance<HighWizardLegs>().Item.legSlot;

    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Leggings);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        var drawPosition = CostumeUtilities.GetArmorDrawPosition(ref drawInfo);
        Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
        float yOsc = MathF.Sin(bodyFrame.Y) * 0.5f + 0.5f;
        drawPosition.Y += yOsc * 2;
      //  drawPosition.Y += drawInfo.drawPlayer.gfxOffY;
        drawPosition.Y += 16;
        var texAsset = AssetReferences.Content.Armors.HighWizard.HighWizardLegsWear.Asset;
        float rotation = yOsc * MathHelper.ToRadians(5);
        SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        var drawData = new DrawData(
            texAsset.Value,
            drawPosition,
            null,
            drawInfo.colorArmorLegs,
            rotation,
            texAsset.Size() * 0.5f,
            1f,
           spriteEffects,
            0
        );

        drawData.shader = drawInfo.cLegs;
        drawInfo.DrawDataCache.Add(drawData);
    }
}
public class HighWizardRobeDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => false;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return false;
      //  return drawInfo.drawPlayer.body == ModContent.GetInstance<HighWizardRobe>().Item.bodySlot;

    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Torso);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        var drawPosition = CostumeUtilities.GetArmorDrawPosition(ref drawInfo);
        Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
        float yOsc = MathF.Sin(bodyFrame.Y) * 0.5f + 0.5f;
        drawPosition.Y += yOsc * 2;
        drawPosition.Y += drawInfo.drawPlayer.gfxOffY;
        drawPosition.Y += 3;
        var robeTextureAsset = AssetReferences.Content.Armors.HighWizard.HighWizardRobeWear.Asset;
        float rotation = yOsc * MathHelper.ToRadians(5);
        SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        var drawData = new DrawData(
            robeTextureAsset.Value,
            drawPosition,
            null,
            drawInfo.colorArmorBody,
            rotation,
            robeTextureAsset.Size() * 0.5f,
            1f,
           spriteEffects,
            0
        );

        drawData.shader = drawInfo.cBody;
        drawInfo.DrawDataCache.Add(drawData);
    }
}

public class HighWizardHatDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => true;
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.head == ModContent.GetInstance<HighWizardHat>().Item.headSlot;

    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
  
        Player drawPlayer = drawInfo.drawPlayer;
        var drawPosition = CostumeUtilities.GetHatDrawPosition(ref drawInfo);
        drawPosition.Y -= 18;
        var texAsset = AssetReferences.Content.Armors.HighWizard.HighWizardHatWear.Asset;
        Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
        float yOsc = MathF.Sin(bodyFrame.Y) * 0.5f + 0.5f;
        drawPosition.Y += yOsc * 2;


        float rotation = yOsc * MathHelper.ToRadians(5);
        SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        var drawData = new DrawData(
            texAsset.Value,
            drawPosition,
            null,
            drawInfo.colorArmorHead,
            rotation,
            texAsset.Size() * 0.5f,
            1f,
           spriteEffects,
            0
        );
        drawData.shader = drawInfo.cHead;
        drawInfo.DrawDataCache.Add(drawData);
    }
}

public class HighWizardPlayer : ModPlayer
{
    public bool hasSetBonus;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasSetBonus = false;
    }
    public override void UpdateEquips()
    {
        base.UpdateEquips();

    }
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (hasSetBonus)
        {
            Lighting.AddLight(Player.position, new Vector3(0.6f));
        }
    }

}


[AutoloadEquip(EquipType.Head)]
public class HighWizardHat : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorSetSystem.RegisterArmorSet<HighWizardHat, HighWizardRobe, HighWizardLegs>(ArmorGroup.Act_I);
    }

    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 30;
        Item.value = 10000;
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.artifactManaReduction += 0.15f;
        stats.accessorySlots++;
        stats.defenseBonus = 3;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<HighWizardRobe>() && legs.type == ModContent.ItemType<HighWizardLegs>();
    }


    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<HighWizardPlayer>().hasSetBonus = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class HighWizardRobe : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 18; // Width of the item
        Item.height = 18; // Height of the item
        Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
        Item.rare = ItemRarityID.Green; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.magicDamage += 0.08f;
        stats.accessorySlots++;
        stats.defenseBonus += 5;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class HighWizardLegs : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 22;
        Item.value = 10000;
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.totalMana += 50;
        stats.accessorySlots++;
        stats.defenseBonus += 4;
    }
}
