using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Stellamod.Particles;
using ParticleLibrary;
using Stellamod.Helpers;

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
                    for (int i = 0; i < count; i++)
                    {
                        Vector2 position = player.RandomPositionWithinEntity();
                        Vector2 speed = new Vector2(0, Main.rand.NextFloat(-0.2f, -1f));
                        Color color = Color.LightBlue.MultiplyAlpha(0.1f);
                        Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<MoonTrailParticle>(), color, Main.rand.NextFloat(0.2f, 0.8f));
                        p.layer = Particle.Layer.BeforePlayersBehindNPCs;
                    }
                }
            }

            player.GetCritChance(DamageClass.Generic) += 8;
        }
    }
}