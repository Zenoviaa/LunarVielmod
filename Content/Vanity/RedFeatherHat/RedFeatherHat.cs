using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.RedFeatherHat;

public class RedFeatherHatDrawLayer : PlayerDrawLayer
{
    private Asset<Texture2D> _hatTextureAsset;
    public override bool IsHeadLayer => true;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _hatTextureAsset = ModContent.Request<Texture2D>(ModContent.GetInstance<RedFeatherHat>().Texture);
    }
    public override void Unload()
    {
        base.Unload();
        _hatTextureAsset = null;
    }

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.head ==
            ModContent.GetInstance<RedFeatherHat>().Item.headSlot;

    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        var position = drawInfo.Center - Main.screenPosition;
        position = new Vector2((int)position.X, (int)position.Y);
        position.Y -= 12;
        position.X -= drawInfo.drawPlayer.direction * 8;
        Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
        float yOsc = MathF.Sin(bodyFrame.Y) * 0.5f + 0.5f;
        position.Y += (int)(yOsc * 2);
        float rotation = 0;
        SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        DrawData drawData = new DrawData(
            _hatTextureAsset.Value,
            position,
            null,
            drawInfo.colorArmorHead,
            rotation,
            new Vector2(_hatTextureAsset.Width() * 0.5f, _hatTextureAsset.Height()),
            1f,
           spriteEffects,
            0
        );

        drawData.shader = drawInfo.cHead;
        //drawData.shader = 4;
        drawInfo.DrawDataCache.Add(drawData);
    }
}


[AutoloadEquip(EquipType.Head)]
public class RedFeatherHat : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 18; // Width of the item
        Item.height = 18; // Height of the item
        Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
        Item.rare = ItemRarityID.LightPurple;// The rarity of the item
        Item.vanity = true;
      
    }
    public override void UpdateVanity(Player player)
    {
        base.UpdateVanity(player);
     //   player.GetModPlayer<RedFeatherPlayer>().hasHat = true;
    }
}

