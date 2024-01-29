using Microsoft.Xna.Framework;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Catacombs
{
    internal class FireEmblemPlayer : ModPlayer
    {
        public bool hasFireEmblem;
        public override void ResetEffects()
        {
            hasFireEmblem = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hasFireEmblem)
            {
                switch (Main.rand.Next(0, 4))
                {
                    case 0:
                        target.AddBuff(BuffID.OnFire3, 120);
                        break;
                    case 1:
                        target.AddBuff(BuffID.ShadowFlame, 120);
                        break;
                    case 2:
                        target.AddBuff(BuffID.CursedInferno, 120);
                        break;
                    case 3:
                        target.AddBuff(BuffID.Daybreak, 60);
                        break;
                }

                //1/3 chance for crits to EXPLODE
                if (hit.Crit && Main.rand.NextBool(4))
                {
                    ShakeModSystem.Shake = 10;
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom"));
                    Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero,
                        ModContent.ProjectileType<FireBoom>(), damageDone / 2, hit.Knockback, Player.whoAmI);
                }
            }
        }
    }

    internal class FireEmblem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FireEmblemPlayer>().hasFireEmblem = true;
        }
    }
}
