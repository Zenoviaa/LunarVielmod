﻿

using Microsoft.Xna.Framework;
using Stellamod.NPCs.Bosses.INest;
using Stellamod.Utilis;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    public class EDR : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electronic Death Remote (EDR)");
            // Tooltip.SetDefault("'that big red button probably will do something you’ll regret... \n Your conscience advises you to press it and see what happens!'");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Orange;
        }

      /*  public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<VirulentPlating>(), 30);
            recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 20);
            recipe.AddIngredient(ModContent.ItemType<DreadFoil>(), 15);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }

*/
        public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
        {
            if (player.ZoneAcid() || player.GetModPlayer<MyPlayer>().ZoneLab)
            {
                             
                if (NPC.AnyNPCs(ModContent.NPCType<IrradiatedNest>()))
                {
                        return false;
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<IrradiatedNest>()))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Main.NewText("A disruption has occured!", Color.SpringGreen);
                        int npcID = NPC.NewNPC(player.GetSource_FromThis(), (int)player.position.X, (int)player.position.Y, ModContent.NPCType<IrradiatedNest>());
                        Main.npc[npcID].netUpdate2 = true;
                    }
                    else
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText("A disruption has occured!", Color.SpringGreen);
                            StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, ModContent.NPCType<IrradiatedNest>(), (int)player.position.X, (int)player.position.Y);

                        }


                    }
                }
                 int TextToSpawn = Main.rand.Next(1, 8 + 1);
                if (TextToSpawn == 1)
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(152, 208, 113, 44), "Initiate death protocol");
                }
                if (TextToSpawn == 2)
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(152, 208, 113, 44), "The end is nigh…");
                }
                if (TextToSpawn == 3)
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(152, 208, 113, 44), "Tactical nuke incoming…");
                }
                if (TextToSpawn == 4)
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(152, 208, 113, 44), "We’re here to contact you about your deaths extended warranty");
                }
                if (TextToSpawn == 5)
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(152, 208, 113, 44), "Brace for impact…");
                }
                if (TextToSpawn == 6)
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(152, 208, 113, 44), "Not Contaminated life form detacted!");
                }
                if (TextToSpawn == 7)
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(152, 208, 113, 44), "Prepare for voltile Termenation!");
                }
                if (TextToSpawn == 8)
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(152, 208, 113, 44), "You’re going to have a bad time…");
                }
               // SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Button"));
            }
            else
            {
               // SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Button"));
                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(152, 208, 113, 44), "Usage outside of contamination detacted!");

                return false;
            }


            return true;
        }

    }
}