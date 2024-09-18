

using Microsoft.Xna.Framework;
using Stellamod.Helpers;
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
                        Main.NewText(LangText.Misc("EDR.Occur"), Color.SpringGreen);
                        int npcID = NPC.NewNPC(player.GetSource_FromThis(), (int)player.position.X, (int)player.position.Y, ModContent.NPCType<IrradiatedNest>());
                        Main.npc[npcID].netUpdate2 = true;
                    }
                    else
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText(LangText.Misc("EDR.Occur"), Color.SpringGreen);
                            StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, ModContent.NPCType<IrradiatedNest>(), (int)player.position.X, (int)player.position.Y);

                        }


                    }
                }
                int TextToSpawn = Main.rand.Next(1, 8 + 1);

                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(152, 208, 113, 44), LangText.Misc("EDR." + TextToSpawn));
                // SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Button"));
            }
            else
            {
               // SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Button"));
                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(152, 208, 113, 44), LangText.Misc("EDR.9"));

                return false;
            }


            return true;
        }

    }
}