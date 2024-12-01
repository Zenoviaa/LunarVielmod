using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.ScorpionMountSystem
{
    internal class ScorpionPlayer : ModPlayer
    {
        public Vector2 gunMountPosition;
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
        }
    }
}
