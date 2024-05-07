using Microsoft.Xna.Framework;
using Stellamod.NPCs.Catacombs.Water.WaterCogwork;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Catacombs
{
    internal class AquaticEmblemPlayer : ModPlayer
    {
        public bool hasAquaticEmblem;
        public override void ResetEffects()
        {
            hasAquaticEmblem = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hasAquaticEmblem && Main.rand.NextBool(4))
            {
                float bubbleCount = Main.rand.Next(3, 5);
                for (int i = 0; i < bubbleCount; i++)
                {
                    Vector2 offset = Main.rand.NextVector2CircularEdge(256, 256);
                    Vector2 position = target.Center + offset;
                    Vector2 bubbleVel = target.DirectionFrom(position) * 2;

                    //Bubbles deal a percent of the base damage
                    float newDamage = (float)hit.SourceDamage;
                    Projectile p = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), position, bubbleVel,
                       ProjectileID.Bubble, (int)newDamage, 1, Player.whoAmI);

                    p.timeLeft = 300;
                    p.tileCollide = false;

                    float count = 16;
                    float degreesPer = 360 / count;
                    for (int k = 0; k < count; k++)
                    {
                        float degrees = k * degreesPer;
                        Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                        Vector2 vel = direction * 4;
                        Dust.NewDust(position, 1, 1, DustID.Water, vel.X, vel.Y);
                    }
                }

                SoundEngine.PlaySound(SoundID.Item85, target.position);
            }
        }
    }

    internal class AquaticEmblem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AquaticEmblemPlayer>().hasAquaticEmblem = true;
        }
    }
}
