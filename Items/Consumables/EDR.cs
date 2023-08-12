

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Stellamod.Items.Materials;
using Stellamod.NPCs.Bosses.Jack;
using Stellamod.NPCs.Bosses.INest;
using Stellamod.Utilis;

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
            Item.rare = 2;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = 4;
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
            if (player.ZoneAcid())
            {

                int TextToSpawn = Main.rand.Next(1, 8 + 1);
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<IrradiatedNest>());
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
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Button"));
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Button"));
                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(152, 208, 113, 44), "Usage outside of contamination detacted!");
            }


            return true;
        }

    }
}