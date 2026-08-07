using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.StarrVeriplant;
using Stellamod.NPCs.Bosses.Zui;
using Stellamod.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Special
{
    internal class AnotherRock : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Blue;

        }

        public override bool? UseItem(Player player)
        {
            Vector2 teleportPosition = TeleportSystem.StoneGolemAltarWorld;
            if(teleportPosition == Vector2.Zero)
            {
                Point p = player.position.ToTileCoordinates();
                int i = p.X;
                int j = p.Y;
                if (!NPC.AnyNPCs(ModContent.NPCType<StarrVeriplant>()))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Main.NewText(LangText.Misc("FlowerSummon.1"), Color.Gold);
                        int npcID = NPC.NewNPC(new EntitySource_TileBreak(i + 10, j), i * 16, j * 16, ModContent.NPCType<StarrVeriplant>());
                        Main.npc[npcID].netUpdate2 = true;
                    }
                    else
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                            return false;

                        StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, ModContent.NPCType<StarrVeriplant>(), i * 16, (j * 16) - 5);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }

            player.Teleport(teleportPosition);
            NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, teleportPosition.X, teleportPosition.Y, 1);
            SoundEngine.PlaySound(SoundID.Item6, player.position);
            return true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.StoneBlock, 100);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }

}
