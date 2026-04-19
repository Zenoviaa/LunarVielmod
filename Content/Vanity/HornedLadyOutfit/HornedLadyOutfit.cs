using ReLogic.Content;
using Stellamod.Content.Armors.Velioza;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.HornedLadyOutfit;


public class HornedLadyOutfitHeadDrawLayer : PlayerDrawLayer
{
    private Asset<Texture2D> _hatTextureAsset;
    public override bool IsHeadLayer => true;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _hatTextureAsset = 
            ModContent.Request<Texture2D>(
                ModContent.GetInstance<HornedLadyDemonOutfitHead>().Texture);
    }
    public override void Unload()
    {
        base.Unload();
        _hatTextureAsset = null;
    }
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.head == 
            ModContent.GetInstance<HornedLadyDemonOutfitHead>().Item.headSlot;

    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ProjectileOverArm);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        var position = drawInfo.Center - Main.screenPosition;
        position = new Vector2((int)position.X, (int)position.Y);
        position.Y -= 12;
        position.X -= drawInfo.drawPlayer.direction * 2;
        Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
        float yOsc = MathF.Sin(bodyFrame.Y) * 0.5f + 0.5f;
        position.Y += yOsc * 2;
        float rotation = 0;
        SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        drawInfo.DrawDataCache.Add(new DrawData(
            _hatTextureAsset.Value,
            position,
            null,
            drawInfo.colorArmorHead,
            rotation,
            _hatTextureAsset.Size() * 0.5f,
            1f,
           spriteEffects,
            0
        ));
    }
}


[AutoloadEquip(EquipType.Head)]
public class HornedLadyOutfitHead : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = false;
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class HornedLadyOutfitBody : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.vanity = true;
    }
}



[AutoloadEquip(EquipType.Legs)]
public class HornedLadyOutfitLegs : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Head)]
public class HornedLadyDemonOutfitHead : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = false;
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class HornedLadyDemonOutfitBody : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.vanity = true;
    }
}
