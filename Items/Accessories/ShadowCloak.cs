using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class ShadowCloak : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 36;
            Item.value = 2500;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);

            //Increased armor pen
            player.statDefense += 8;
            player.endurance += 0.04f;
            player.aggro -= 500;

            // Starting search distance
            // This code is required either way, used for finding a target
            float distanceFromTarget = 128f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                float between = Vector2.Distance(npc.Center, player.Center);
                bool inRange = between < distanceFromTarget;
                if (npc.chaseable && inRange)
                {
                    npc.AddBuff(BuffID.Confused, 2);
                    npc.AddBuff(BuffID.CursedInferno, 2);
                    npc.AddBuff(BuffID.Ichor, 2);
                    npc.AddBuff(BuffID.ShadowFlame, 2);
                }
            }
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.FleshKnuckles, 1);
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 30);
            recipe.AddIngredient(ItemID.Ichor, 15);
            recipe.AddIngredient(ItemID.SoulofNight, 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
