using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Catacombs
{
    internal class SpikedEmblemPlayer : ModPlayer
    {
        public bool hasSpikedEmblem;
        public override void ResetEffects()
        {
            hasSpikedEmblem = false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (hasSpikedEmblem && !target.boss)
            {
                if (!target.HasBuff<Chained>())
                {
                    //Only add the chain thingy if the buff isn't already there
                    //This should prevent multiple chains
                    target.AddBuff(ModContent.BuffType<Chained>(), 300);
                    ChainProj chainProj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                        ModContent.ProjectileType<ChainProj>(), 1, 1, Player.whoAmI).ModProjectile as ChainProj;
                    chainProj.Target = target;
                }
                else
                {
                    target.AddBuff(ModContent.BuffType<Chained>(), 300);
                }
            }
        }
    }

    internal class SpikedEmblem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpikedEmblemPlayer>().hasSpikedEmblem = true;
            player.GetDamage(DamageClass.Generic) += 0.08f;
        }
    }
}
