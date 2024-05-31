using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon.Orbs
{
    internal class OrbPlayer : ModPlayer
    {
        public int ItemType = -1;
        public int ItemType2 = -1;
        public int ProjType2Override = -1;
        public ModItem OrbItem => ModContent.GetModItem(ItemType);
        public ModItem OrbItem2 => ModContent.GetModItem(ItemType);

        public override void ResetEffects()
        {
            ItemType = -1;
            ItemType2 = -1;
            ProjType2Override = -1;
        }

        private void HolsterOrb(Player player, int projectileType, int baseDamage, float knockBack)
        {
            //   player.damage
            int newDamage = (int)player.GetTotalDamage(DamageClass.Summon).ApplyTo(baseDamage);
            if (player.ownedProjectileCounts[projectileType] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    projectileType, newDamage, knockBack, player.whoAmI);
            }
        }

        public void EquipOrbSlot1(int itemType)
        {
            if (Player.HeldItem.type != itemType)
                return;

            if (ItemType != -1)
                return;

            ItemType = itemType;
        }

        public void EquipOrbSlot2(int itemType, int projType2Override)
        {
            if (Player.HeldItem.type != itemType)
                return;

            if (ItemType2 != -1)
                return;

            ItemType2 = itemType;
            ProjType2Override = projType2Override;
        }

        public override void PostUpdate()
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                int buffType = ModContent.BuffType<OrbMaster>();
                if (!Player.HasBuff(buffType))
                {
                    if ((Player.HeldItem.type == ItemType || Main.mouseItem.type == ItemType))
                    {
                        Player.AddBuff(ModContent.BuffType<OrbMaster>(), 2, false);
                    }
                }
                else
                {
                    if (ItemType != -1)
                    {
                        HolsterOrb(Player, OrbItem.Item.shoot, OrbItem.Item.damage, OrbItem.Item.knockBack);
                    }
                    if (ItemType2 != -1)
                    {
                        HolsterOrb(Player, ProjType2Override, OrbItem.Item.damage, OrbItem.Item.knockBack);
                    }

                    if ((Player.HeldItem.type != ItemType) && Main.mouseItem.type != ItemType)
                    {
                        Player.ClearBuff(buffType);
                        NetMessage.SendData(MessageID.PlayerBuffs);
                    }
                }
            }
        }
    }
}
