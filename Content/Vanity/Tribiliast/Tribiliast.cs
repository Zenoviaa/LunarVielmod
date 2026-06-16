using ReLogic.Content;
using Stellamod.Content.Armors.Velioza;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.Tribiliast;


public class TribiliastHeadDrawLayer : PlayerDrawLayer
{
    private Asset<Texture2D> _hatTextureAsset;
    public override bool IsHeadLayer => true;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _hatTextureAsset = 
            ModContent.Request<Texture2D>(
                ModContent.GetInstance<TribiliastHead>().Texture);
    }
    public override void Unload()
    {
        base.Unload();
        _hatTextureAsset = null;
    }
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.head == 
            ModContent.GetInstance<TribiliastHead>().Item.headSlot;

    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        var position = drawInfo.Center - Main.screenPosition;
        position = new Vector2((int)position.X, (int)position.Y);
        position.Y -= 20;
        position.X -= drawInfo.drawPlayer.direction * 2;
        Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
        float yOsc = MathF.Sin(bodyFrame.Y) * 0.5f + 0.5f;
        position.Y += yOsc * 2;
        float rotation = 0;
        SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        var drawData = new DrawData(
            _hatTextureAsset.Value,
            position,
            null,
            drawInfo.colorArmorHead,
            rotation,
            _hatTextureAsset.Size() * 0.5f,
            1f,
           spriteEffects,
            0
        );
        drawData.shader = drawInfo.cHead;
        drawInfo.DrawDataCache.Add(drawData);
    }
}
public class TribiliastBodyDrawLayer : PlayerDrawLayer
{
    private Asset<Texture2D> _robeTextureAsset;
    public override bool IsHeadLayer => false;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _robeTextureAsset = ModContent.Request<Texture2D>(
            ModContent.GetInstance<TribiliastBody>().Texture);
    }
    public override void Unload()
    {
        base.Unload();
        _robeTextureAsset = null;
    }
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.body == ModContent.GetInstance<TribiliastBody>().Item.bodySlot;

    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Torso);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        var position = drawInfo.Center - Main.screenPosition;
        position = new Vector2((int)position.X, (int)position.Y);
        position.Y += 4;

        Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
        float yOsc = MathF.Sin(bodyFrame.Y) * 0.5f + 0.5f;
        position.Y += yOsc * 2;
        float rotation = yOsc * MathHelper.ToRadians(5);
        SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        var drawData = new DrawData(
            _robeTextureAsset.Value,
            position,
            null,
            drawInfo.colorArmorBody,
            rotation,
            _robeTextureAsset.Size() * 0.5f,
            1f,
           spriteEffects,
            0
        );


        drawData.shader = drawInfo.cBody;
        drawInfo.DrawDataCache.Add(drawData);
    }
}
public class TribiliastStaffDrawLayer : PlayerDrawLayer
{
    private Asset<Texture2D> _robeTextureAsset;
    public override bool IsHeadLayer => false;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _robeTextureAsset = ModContent.Request<Texture2D>(
            ModContent.GetInstance<TribiliastBody>().Texture + "_Staff");
    }
    public override void Unload()
    {
        base.Unload();
        _robeTextureAsset = null;
    }
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.body == ModContent.GetInstance<TribiliastBody>().Item.bodySlot;

    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Backpacks);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        var position = drawInfo.Center - Main.screenPosition;
        position = new Vector2((int)position.X, (int)position.Y);
        position.Y += -12;
        position.X -= drawInfo.drawPlayer.direction * 12;

        Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
        float yOsc = MathF.Sin(bodyFrame.Y) * 0.5f + 0.5f;
        position.Y += yOsc * 2;
        float rotation = yOsc * MathHelper.ToRadians(5);
        SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        var drawData = new DrawData(
            _robeTextureAsset.Value,
            position,
            null,
            drawInfo.colorArmorBody,
            rotation,
            _robeTextureAsset.Size() * 0.5f,
            1f,
           spriteEffects,
            0
        );


        drawData.shader = drawInfo.cBody;
        drawInfo.DrawDataCache.Add(drawData);
    }
}

[AutoloadEquip(EquipType.Legs)]
public class TribiliastLegs : ModItem
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
public class TribiliastHead : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
      //  ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = false;
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
public class TribiliastBody : ModItem
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
