using Stellamod.Common.ArmorRework;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Town;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Content.Bar.Drinks;


public class Cab : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToDrink<CabBuff>();
    }
}

public class CabBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (player.dead)
            player.ClearBuff(Type);
        player.GetModPlayer<ArmorStatsPlayer>().defenseBonus += 10;
    }
}

public class Noir : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToDrink<NoirBuff>();
    }
}
public class NoirBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (player.dead)
            player.ClearBuff(Type);
        player.endurance += 0.1f;
    }
}

public class Zin : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToDrink<ZinBuff>();
    }
}

public class ZinBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (player.dead)
            player.ClearBuff(Type);
        float maxLife2 = player.statLifeMax2;
        maxLife2 *= 1.1f;
        player.statLifeMax2 = (int)maxLife2;
    }
}

public class Rose : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToDrink<RoseBuff>();
    }
}

public class RoseBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (player.dead)
            player.ClearBuff(Type);
        player.GetDamage(DamageClass.Generic) += 0.1f;
    }
}

public class Chard : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToDrink<ChardBuff>();
    }
}

public class ChardBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
   
    }

    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (player.dead)
            player.ClearBuff(Type);
        player.GetModPlayer<ArmorStatsPlayer>().minionSlots += 2;
    }
}

public class Ries : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToDrink<RiesBuff>();
    }
}

public class RiesBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (player.dead)
            player.ClearBuff(Type);
        player.GetModPlayer<ArmorStatsPlayer>().rangedGunAmmoAmount += 5;
    }
}

public class Port : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToDrink<PortBuff>();
    }
}

public class PortBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (player.dead)
            player.ClearBuff(Type);
        player.GetModPlayer<ArmorStatsPlayer>().wandCastTime += 0.2f;
    }
}

public class VinhoVerde : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToDrink<VinhoVerdeBuff>();
    }
}

public class VinhoVerdeBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (player.dead)
            player.ClearBuff(Type);
        player.GetModPlayer<ArmorStatsPlayer>().meleeAttackSpeed += 0.2f;
    }
}

public class Blanc : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToDrink<BlancBuff>();
    }
}

public class BlancBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (player.dead)
            player.ClearBuff(Type);
        player.GetModPlayer<ArmorStatsPlayer>().rangedBowChargeTime += 0.2f;
    }
}


public class LastDrink : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToDrink<LastDrinkBuff>();
    }
    public override bool? UseItem(Player player)
    {
        player.GetModPlayer<LastDrinkPlayer>().Reroll();
        return base.UseItem(player);
    }
}

public class LastDrinkBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
       
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        if (player.dead)
            player.ClearBuff(Type);
        float maxLife2 = player.statLifeMax2;
        maxLife2 *= 0.7f;
        player.statLifeMax2 = (int)maxLife2;
    }

}

public class LastDrinkPlayer : ModPlayer
{
    private int[] _activeBuffs;
    public void Reroll()
    {
        int[] pool = new int[]
        {
            BuffID.Ironskin,
            BuffID.Swiftness,
            BuffID.Regeneration,
            BuffID.Lifeforce,
            BuffID.Endurance,
            BuffID.Mining,
            BuffID.Fishing,
            BuffID.AmmoReservation,
            BuffID.Archery,
            BuffID.Summoning,
            BuffID.Battle,
            BuffID.Dangersense,
            BuffID.NightOwl,
            BuffID.Shine
        };

        List<int> selectedBuffs = new List<int>(3);
        while(selectedBuffs.Count < 3)
        {
            int rand = Main.rand.Next(pool.Length);
            int buffId = pool[rand];
            if (buffId == -1)
                continue;
            selectedBuffs.Add(buffId);
            pool[rand] = -1;
        }
        _activeBuffs = selectedBuffs.ToArray();
    }
    public override void PostUpdateBuffs()
    {
        base.PostUpdateBuffs();
        if (!Player.HasBuff<LastDrinkBuff>())
            return;
        if (_activeBuffs == null)
            return;
        for(int i = 0; i < _activeBuffs.Length; i++)
        {
            int buff = _activeBuffs[i];
            Player.AddBuff(buff, 2);
        }

    }
}


public enum FoodType : byte
{
    None,
    RedBerries,
    MysteriousGrapes,
    DeepseaBerry,
    EdibleCrystal,
    RottenChunk,
    LavishedJelly,
    Shrimp
}

public class PermamentFoodGlobalItem : GlobalItem
{
    public bool isPermanentFood => permanentFoodType != FoodType.None;
    public bool isDrink;
    public FoodType permanentFoodType;
    public override bool InstancePerEntity => true;
    public override bool? UseItem(Item item, Player player)
    {
        if (isPermanentFood)
        {
            player.GetModPlayer<PermanentFoodsPlayer>().foods[(int)permanentFoodType] = true;
        }
        return true;
    }
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        base.ModifyTooltips(item, tooltips);
        if (isPermanentFood)
        {
            TooltipLine line = new TooltipLine(Mod, "PermanentFoodTooltip", LangText.Common("PermanentFoodHelp"));
            line.OverrideColor = Color.Lerp(Color.DarkRed, Color.White, 0.2f);
            tooltips.Add(line);
        }

        if (isDrink)
        {
            TooltipLine line = new TooltipLine(Mod, "DrinkTooltip", LangText.Common("DrinkHelp"));
            line.OverrideColor = Color.Lerp(new Color(80, 187, 124), Color.Black, 0.15f);
            tooltips.Add(line);
        }

        if(isDrink || isPermanentFood)
        {
            TooltipLine buffTimeTooltip = tooltips.Find(x => x.Name == "BuffTime");
            buffTimeTooltip.Hide();
        }
    }
}
public class RedBerries : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPermanentFood(FoodType.RedBerries);
    }
}
public class MysteriousGrapes : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPermanentFood(FoodType.MysteriousGrapes);
    }
}
public class DeepseaBerry : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPermanentFood(FoodType.DeepseaBerry);
    }
}
public class EdibleCrystal : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPermanentFood(FoodType.EdibleCrystal);
    }
}
public class RottenChunk : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPermanentFood(FoodType.RottenChunk);
    }
}
public class LavishedJelly : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPermanentFood(FoodType.LavishedJelly);
    }
}
public class Shrimp : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPermanentFood(FoodType.Shrimp);
    }
}
public class PermanentFoodsPlayer : ModPlayer
{
    public bool[] foods = new bool[Enum.GetNames<FoodType>().Length];
    public bool IsFoodActive(FoodType foodType)
    {
        int type = (int)foodType;
        return foods[type];
    }
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();

        /*
        for(int i = 0; i < foods.Length; i++)
        {
            foods[i] = false;
        }*/
        /*
        StellaWorld stellaWorld = ModContent.GetInstance<StellaWorld>();
        Point spawnPoint = stellaWorld.FableHillStartLocation + new Point(350, -140); ;
        Vector2 position = spawnPoint.ToWorldCoordinates();
        Player.position = position;*/

        if (IsFoodActive(FoodType.RedBerries))
        {
            Player.statLifeMax2 += 10;
        }

        if (IsFoodActive(FoodType.MysteriousGrapes))
        {
            Player.GetDamage(DamageClass.Generic) += 0.01f;
        }

        if (IsFoodActive(FoodType.DeepseaBerry))
        {
            Player.GetModPlayer<ArmorStatsPlayer>().inventorySlots += 1;
        }
        if (IsFoodActive(FoodType.EdibleCrystal))
        {
            Player.statDefense += 2;
        }

        if (IsFoodActive(FoodType.RottenChunk))
        {
            Player.moveSpeed += 0.05f;
        }

        if (IsFoodActive(FoodType.LavishedJelly))
        {
            Player.statManaMax2 += 10;
        }

        if (IsFoodActive(FoodType.Shrimp))
        {
            Player.GetModPlayer<ArmorStatsPlayer>().stamina += 1;
        }
    }
    public override void SaveData(TagCompound tag)
    {
        base.SaveData(tag);
        tag["foods"] = foods;
    }
    public override void LoadData(TagCompound tag)
    {
        base.LoadData(tag);
        foods = tag.Get<bool[]>("foods");
    }
}



public class DrinkShopSystem : ModSystem
{
    private bool _refreshOnDay;
    public static List<Item> items;
    public override void OnModLoad()
    {
        base.OnModLoad();
        items = new List<Item>();
    }
    public override void OnModUnload()
    {
        base.OnModUnload();
        items.Clear();
        items = null;
    }
    public static List<Item> CreateNewShop()
    {
        // create a list of item ids
    
        int[] pool = new int[]
        {
            ModContent.ItemType<Cab>(),
            ModContent.ItemType<Noir>(),
            ModContent.ItemType<Rose>(),
            ModContent.ItemType<Chard>(),
            ModContent.ItemType<Ries>(),
            ModContent.ItemType<Port>(),
            ModContent.ItemType<VinhoVerde>(),
            ModContent.ItemType<Blanc>(),
            ModContent.ItemType<LastDrink>(),
            ModContent.ItemType<RedBerries>(),
            ModContent.ItemType<MysteriousGrapes>(),
            ModContent.ItemType<DeepseaBerry>(),
            ModContent.ItemType<EdibleCrystal>(),
            ModContent.ItemType<RottenChunk>(),
            ModContent.ItemType<LavishedJelly>(),
            ModContent.ItemType<Shrimp>(),
            ModContent.ItemType<CrystalStar>()
        };

        var itemIds = new List<int>();
        while (itemIds.Count < 3)
        {
            int rand = Main.rand.Next(pool.Length);
            int id = pool[rand];
            if (id == -1)
                continue;
            itemIds.Add(id);
            pool[rand] = -1;
        }

        var items = new List<Item>();
        foreach (int itemId in itemIds)
        {
            Item item = new Item();
            item.SetDefaults(itemId);
            items.Add(item);
        }
        return items;
    }

    public override void PreUpdateWorld()
    {
        if (!Main.dayTime)
        {
            _refreshOnDay = true;
        }

        if((Main.dayTime && _refreshOnDay) || items.Count == 0)
        {
            _refreshOnDay = false;
            items = CreateNewShop();
        }


        // ExampleTravelingMerchant.UpdateTravelingMerchant();
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["shopItems"] = items;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        items.Clear();
        items.AddRange(tag.Get<List<Item>>("shopItems"));
    }

    public override void ClearWorld()
    {
        items.Clear();
    }

    public override void NetSend(BinaryWriter writer)
    {
        // Note that NetSend is called whenever WorldData packet is sent.
        // We use this so that shop items can easily be synced to joining players
        // We recommend modders avoid sending WorldData too often, or filling it with too much data, lest too much bandwidth be consumed sending redundant data repeatedly
        // Consider sending a custom packet instead of WorldData if you have a significant amount of data to synchronise

        writer.Write(items.Count);
        foreach (Item item in items)
        {
            ItemIO.Send(item, writer, writeStack: true);
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        items.Clear();
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            items.Add(ItemIO.Receive(reader, readStack: true));
        }
    }
}