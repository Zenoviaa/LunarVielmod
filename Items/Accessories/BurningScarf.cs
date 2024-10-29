using Microsoft.Xna.Framework;
using Stellamod.Items.Accessories.Players;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class BurningScarf : BaseDashItem
    {
        private float Timer;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 23;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.width = 38;
            Item.height = 58;
        }

        public override void BeginDash(Player player)
        {
            base.BeginDash(player);
            Timer = 0;
            SoundEngine.PlaySound(SoundID.Item73, player.position);
            for (int i = 0; i < 16; i++)
            {
                float progress = (float)i / 16f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 3;
                Dust.NewDustPerfect(player.Center, DustID.Torch, vel, Scale: 2);
            }
        }

        public override void UpdateDash(Player player)
        {
            base.UpdateDash(player);
            Timer++;
            if (Timer % 6 == 0)
            {
                for (int i = 0; i < 12; i++)
                {
                    float progress = (float)i / 12f;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 1.3f;
                    Dust d = Dust.NewDustPerfect(player.Center, DustID.Torch, vel, Scale: 2);
                    d.noGravity = true;
                }

                //Make Flame
                int damage = Item.damage;
                float knockback = Item.knockBack;
                Vector2 velocity = -player.velocity.SafeNormalize(Vector2.Zero) * 8;
                velocity -= new Vector2(0, 0.013f);
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, velocity,
                    ModContent.ProjectileType<BurningScarfFlame>(), damage, knockback, player.whoAmI);
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);

            //Just gonna make your dash go a lil' farther
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.DashVelocity += 3;
        }
    }
}
