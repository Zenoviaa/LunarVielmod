using ReLogic.Content;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Sanctorous;

public class SanctorousRobeDrawLayer : PlayerDrawLayer
{
    private Asset<Texture2D> _robeTextureAsset;
    public override bool IsHeadLayer => false;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _robeTextureAsset = ModContent.Request<Texture2D>(ModContent.GetInstance<SanctorousHead>().Texture + "_Body");
    }
    public override void Unload()
    {
        base.Unload();
        _robeTextureAsset = null;
    }
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.body == ModContent.GetInstance<SanctorousBody>().Item.bodySlot;

    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ProjectileOverArm);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        var position = drawInfo.Center - Main.screenPosition;
        position = new Vector2((int)position.X, (int)position.Y);
        position.Y += 0;

        Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
        float yOsc = MathF.Sin(bodyFrame.Y) * 0.5f + 0.5f;
        position.Y += yOsc * 2;
        float rotation = yOsc * MathHelper.ToRadians(5);
        SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        drawInfo.DrawDataCache.Add(new DrawData(
            _robeTextureAsset.Value,
            position,
            null,
            drawInfo.colorArmorBody,
            rotation,
            _robeTextureAsset.Size() * 0.5f,
            1f,
           spriteEffects,
            0
        ));
    }
}

public class SanctorousHelmetDrawLayer : PlayerDrawLayer
{
    private Asset<Texture2D> _hatTextureAsset;
    public override bool IsHeadLayer => true;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _hatTextureAsset = ModContent.Request<Texture2D>(ModContent.GetInstance<SanctorousHead>().Texture + "_Helm");
    }
    public override void Unload()
    {
        base.Unload();
        _hatTextureAsset = null;
    }
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.head == ModContent.GetInstance<SanctorousHead>().Item.headSlot;

    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {

        var position = drawInfo.Center - Main.screenPosition;
        position = new Vector2((int)position.X, (int)position.Y);
        position.Y -= 18;

        Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
        float yOsc = MathF.Sin(bodyFrame.Y) * 0.5f + 0.5f;
        position.Y += yOsc * 2;
        float rotation = yOsc * MathHelper.ToRadians(5);
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
public class Sanctorous : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (Main.rand.NextBool(3))
        {
            SmokeParticle sp = Particle<SmokeParticle>.Spawn(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), -Vector2.UnitY, Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
            sp.initialColor = Color.Lerp(Color.OrangeRed, Color.RosyBrown, Main.rand.NextFloat(0f, 1f)) * 0.4f;
            sp.expand = true;
        }
        if (Main.rand.NextBool(3))
        {
            LegacyParticle.NewParticle<EmberParticle>(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), -Vector2.UnitY.RotatedByRandom(1.5f), Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
        }
    }
}

public class SanctorousPlayer : ModPlayer
{
    private Asset<Texture2D> _featherTextureAsset;
    public bool hasSetBonus;
    public float alphaTimer;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasSetBonus = false;
    }

    public override void Load()
    {
        base.Load();
        FlaskPlayer.OnProc += ApplyImmunity;
    }

    public override void Unload()
    {
        base.Unload();
        FlaskPlayer.OnProc -= ApplyImmunity;
        _featherTextureAsset = null;
    }

    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (Player.HasBuff<Sanctorous>())
        {
            alphaTimer += 0.05f;
        }
        else
        {
            alphaTimer -= 0.05f;
        }
        alphaTimer = MathHelper.Clamp(alphaTimer, 0, 1f);
    }
    private void ApplyImmunity(Player player)
    {
        SanctorousPlayer elegant = player.GetModPlayer<SanctorousPlayer>();
        if (!elegant.hasSetBonus)
            return;
        player.SetImmuneTimeForAllTypes(120);
        player.AddBuff(ModContent.BuffType<Sanctorous>(), 120);
        SoundStyle drinkSound = new SoundStyle("Stellamod/Assets/Sounds/HolyCast1");
        SoundEngine.PlaySound(drinkSound, player.position);
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        if (drawInfo.shadow != 0f)
            return;

        int maxNumBlades = 6;
        SpriteBatch sb = Main.spriteBatch;
        if (alphaTimer <= 0)
        {
            return;
        }
        _featherTextureAsset ??= ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "SanctorousBone");
        for (int i = 0; i < maxNumBlades; i++)
        {
            float ratio = i / (float)maxNumBlades;
            float radians = ratio * MathHelper.TwoPi;
            radians += Main.GlobalTimeWrappedHourly * 0.5f;
            Vector2 drawCenter = radians.ToRotationVector2() * 48 + drawInfo.drawPlayer.Center;

            Texture2D texture = _featherTextureAsset.Value;
            SpritebatchDrawer swordDrawer = SpritebatchDrawer.FromTextureAsset(texture, drawCenter);
            //  float rads = MathHelper.ToRadians(3);

            swordDrawer.rotation = (drawCenter - drawInfo.drawPlayer.Center).ToRotation() + MathHelper.PiOver2;
            swordDrawer.color = Color.White * alphaTimer;
            sb.Draw(swordDrawer);
        }
    }
}

[AutoloadEquip(EquipType.Head)]
public class SanctorousHead : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorSetSystem.RegisterArmorSet<SanctorousHead, SanctorousBody, SanctorousLegs>();
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
        stats.insourceTimeFlatBonus = 5;
        stats.defenseBonus += 15;
        stats.accessorySlots += 2;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<SanctorousBody>() && legs.type == ModContent.ItemType<SanctorousLegs>();
    }


    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<SanctorousPlayer>().hasSetBonus = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class SanctorousBody : ModItem
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
        stats.defenseBonus += 20;
        stats.stamina += 3;
        stats.accessorySlots += 2;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class SanctorousLegs : ModItem
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
        stats.insourceSlots += 5;
        stats.inventorySlots += 20;
        stats.defenseBonus += 16;
        stats.accessorySlots += 2;
    }
}
