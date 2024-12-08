using Microsoft.Xna.Framework;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Projectiles.GunHolster;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.ScorpionMountSystem
{
    internal class ScorpionPlayer : ModPlayer
    {
        public Vector2 gunMountPosition;
        public Vector2[] holsterPositions;
        public bool[] onRight;
        public MiniGun[] miniGuns;

        public override void ResetEffects()
        {
            base.ResetEffects();
            holsterPositions ??= new Vector2[4];
            onRight ??= new bool[4];
            miniGuns ??= new MiniGun[4];
        }

        public override void PostUpdate()
        {
            base.PostUpdate();
            if (Player.mount.Active && Player.mount._mountSpecificData is ScorpionSpecificData scorpionSpecificData)
            {
                if (Main.myPlayer == Player.whoAmI && Player.ownedProjectileCounts[scorpionSpecificData.scorpionItem.gunType] == 0)
                {
                    BaseScorpionItem scorpionItem = scorpionSpecificData.scorpionItem;
                    int finalDamage = (int)Player.GetTotalDamage(scorpionItem.Item.DamageType).ApplyTo(scorpionItem.Item.damage);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                        scorpionItem.gunType, finalDamage, scorpionItem.Item.knockBack);
                }
            }

            for(int i = 0; i < holsterPositions.Length; i++)
            {
                float progress = (float)i / (float)holsterPositions.Length;
                Vector2 holsterPosition = Player.Center + new Vector2(0, 12);
                holsterPosition += Vector2.Lerp(-Vector2.UnitX * 64, Vector2.UnitX * 64, progress);
                if(Player.direction == -1)
                {
                    holsterPosition.X += 32;
                }
                holsterPositions[i] = holsterPosition;
            }
            if(Player.HeldItem.ModItem is BaseScorpionItem myScorpionItem)
            {
                int index = 0;
                foreach(var minigunItem in myScorpionItem.leftHandedGuns)
                {
                    miniGuns[index] = null;
                    if (minigunItem.ModItem is MiniGun miniGun)
                    {
                        HolsterGun(miniGun, index);
                        miniGuns[index] = miniGun;
                    }

                    onRight[index] = false;
                    index++;
                }

                foreach (var minigunItem in myScorpionItem.rightHandedGuns)
                {
                    miniGuns[index] = null;
                    if (minigunItem.ModItem is MiniGun miniGun)
                    {
                        HolsterGun(miniGun, index);
                        miniGuns[index] = miniGun;
                    }

                    onRight[index] = true;
                    index++;
                }
            }
        }

        private void HolsterGun(MiniGun miniGun, int index)
        {
            int newDamage = (int)Player.GetTotalDamage(Player.HeldItem.DamageType).ApplyTo(miniGun.Item.damage);
            int gunHolsterType = ModContent.ProjectileType<ScorpionHolsterProjectile>();
            if (Player.ownedProjectileCounts[gunHolsterType] == 0)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    gunHolsterType, newDamage / 2, miniGun.Item.knockBack, Player.whoAmI, ai2: index);
            }
        }
    }
}
