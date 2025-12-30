using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{

    /// <summary>
    /// Implements basic functionality for a safunai weapon type
    /// </summary>
    public class SafunaiGlobalItem : GlobalItem
    {
        private int _combo;
        public bool isSafunai;
        public override bool InstancePerEntity => true;
        public float UseTimeMultiplier(Player player) => player.GetAttackSpeed(DamageClass.Melee);
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (isSafunai)
            {
                _combo++;
                if (_combo == 1)
                {
                    SoundEngine.PlaySound(AssetRegistry.Sounds.Melee.Safunais1, position);

                }
                if (_combo == 2)
                {
                    SoundEngine.PlaySound(AssetRegistry.Sounds.Melee.Safunais2, position);

                }
                if (_combo == 3)
                {
                    SoundEngine.PlaySound(AssetRegistry.Sounds.Melee.Safunais1, position);

                }
                if (_combo == 4)
                {
                    SoundEngine.PlaySound(AssetRegistry.Sounds.Melee.Safunais2, position);

                }
                if (_combo == 5)
                {
                    _combo = 0;
                    SoundEngine.PlaySound(AssetRegistry.Sounds.Melee.Safunais3, position);
                }

                float distanceMult = Main.rand.NextFloat(0.8f, 1.2f);
                float curvatureMult = 0.7f;
                bool slam = _combo % 5 == 4;

                Vector2 direction = velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));
                Projectile proj = Projectile.NewProjectileDirect(source, position, direction, type, damage, knockback, player.whoAmI);

                if (proj.ModProjectile is BaseSafunaiProjectile modProj)
                {
                    modProj.SwingTime = (int)(item.useTime * UseTimeMultiplier(player) * (slam ? 1.75f : 1)) * 22;
                    modProj.SwingDistance = player.Distance(Main.MouseWorld) * distanceMult;
                    modProj.Curvature = 0.33f * curvatureMult;
                    modProj.Flip = _combo % 2 == 1;
                    modProj.Slam = slam;
                    modProj.PreSlam = _combo % 5 == 3;
                    modProj.Projectile.netUpdate = true;
                }

                return false;
            }
            else
            {
                return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
            }
        }
    }
}
