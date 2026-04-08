using ReLogic.Content;
using Stellamod.Common.ArmorRework;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.GovheilKing;

public class GovheilProtection : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        player.endurance += 0.75f;
        if (Main.rand.NextBool(8))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, player.width);
            pos.Y = Main.rand.Next(0, player.height);
            SparkleParticle sp = SparkleParticle.Spawn(pos + player.position, -Vector2.UnitY, Color.White);
            sp.outerColor = Color.Goldenrod;
            sp.Scale *= 0.66f;
        }
    }
}

public class GovheilKingPlayer : ModPlayer
{
    private Asset<Texture2D> _crownTextureAsset;
    public bool hasSetBonus;
    public override void Unload()
    {
        base.Unload();
        _crownTextureAsset = null;
    }
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasSetBonus = false;
    }

    private bool IsBeingTargeted()
    {
        foreach (var npc in Main.ActiveNPCs)
        {
            if (npc.target == Player.whoAmI)
                return true;
        }
        return false;
    }

    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (!hasSetBonus)
            return;
        if (!IsBeingTargeted())
            return;
        foreach(var player in Main.ActivePlayers)
        {
            if (player.whoAmI == Player.whoAmI)
                continue;
            player.AddBuff(ModContent.BuffType<GovheilProtection>(), 2);
        }
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        if (drawInfo.shadow != 0f)
            return;
        if (!hasSetBonus)
            return;


        _crownTextureAsset ??= ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "GovheilCrown");
        Vector2 drawCenter = drawInfo.drawPlayer.Center + new Vector2(0, -64) + Vector2.Lerp(Vector2.Zero, Vector2.UnitY * 4, ExtraMath.Osc(0f, 1f));
        Texture2D texture = _crownTextureAsset.Value;
        SpritebatchDrawer swordDrawer = SpritebatchDrawer.FromTextureAsset(texture, drawCenter);
        Main.spriteBatch.Draw(swordDrawer);
    }
}


[AutoloadEquip(EquipType.Head)]
public class GovheilHelmet : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        ArmorSetSystem.RegisterArmorSet<GovheilHelmet, GovheilChainplate, GovheilThighs>();
    }

    public override void SetDefaults()
    {
        Item.width = 18; // Width of the item
        Item.height = 18; // Height of the item
        Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
        Item.rare = ItemRarityID.LightRed; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.meleeAggressiveness += 100;
        stats.defenseBonus += 10;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<GovheilChainplate>() && legs.type == ModContent.ItemType<GovheilThighs>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<GovheilKingPlayer>().hasSetBonus = true;
        player.GetDamage(DamageClass.Melee) += 0.1f;
    }
}

[AutoloadEquip(EquipType.Body)]
public class GovheilChainplate : ModItem
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
        Item.rare = ItemRarityID.LightRed; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.accessorySlots++;
        stats.meleeDamage += 0.33f;
        stats.defenseBonus += 16;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class GovheilThighs : ModItem
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 18; // Width of the item
        Item.height = 18; // Height of the item
        Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
        Item.rare = ItemRarityID.LightRed; // The rarity of the item
    }

    public override void UpdateEquip(Player player)
    {
        var stats = player.GetStats();
        stats.meleeArmorPenetration += 5;
        stats.defenseBonus += 7;
        stats.accessorySlots += 2;
    }
}