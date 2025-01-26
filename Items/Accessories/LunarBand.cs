using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class LunarBand : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 2500;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!Main.dayTime)
            {
                player.GetDamage(DamageClass.Generic) *= 1.12f;

                //Lighting Effect
                float osc = VectorHelper.Osc(0.5f, 1f);
                float lightingStrength = 1 * osc;
                Lighting.AddLight(player.position, lightingStrength * 1f, lightingStrength * 1f, lightingStrength);
                if (!hideVisual)
                {
                    int count = Main.rand.Next(3);
                }
            }

            player.GetCritChance(DamageClass.Generic) += 8;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<ConvulgingMater>());
        }
    }
}