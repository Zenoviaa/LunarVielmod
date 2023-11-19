using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Items.Materials;
using Stellamod.Particles;
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

            //Shadow Visual
            if (Main.rand.NextBool(4))
            {
                float radius = 16;
                int count = Main.rand.Next(6);
                for (int i = 0; i < count; i++)
                {
                    Vector2 position = player.Center + new Vector2(radius, 0).RotatedBy(((i * MathHelper.PiOver2 / count)) * 4);
                    Vector2 speed = new Vector2(0, Main.rand.NextFloat(-0.2f, -1f));
                    Color color = default(Color).MultiplyAlpha(0.1f);
                    ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<Ink2>(), color, Main.rand.NextFloat(0.2f, 0.8f));
                }
            }

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
            recipe.AddIngredient(ItemID.StarCloak, 1);
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 30);
            recipe.AddIngredient(ItemID.Ichor, 15);
            recipe.AddIngredient(ItemID.SoulofNight, 7);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
