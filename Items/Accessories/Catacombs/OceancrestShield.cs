using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Catacombs
{
    internal class OceanShieldPlayer : ModPlayer
    {
        private Projectile _waterShieldProj;
        private int _cooldown;
        public bool hasOceanShield;

        public override void ResetEffects()
        {
            hasOceanShield = false;
        }

        public override void UpdateEquips()
        {
            if (hasOceanShield)
            {
                if (_cooldown != 0)
                {
                    _cooldown--;
                }
                else if (_waterShieldProj == null || !_waterShieldProj.active)
                {
                    _waterShieldProj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                        ModContent.ProjectileType<WaterShield>(), 0, 0, Player.whoAmI);
                }
                else
                {
                    _waterShieldProj.timeLeft = 60;
                }
            }
            else if (_waterShieldProj != null && _waterShieldProj.active)
            {
                _waterShieldProj.Kill();
                _waterShieldProj = null;
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (hasOceanShield && modifiers.Dodgeable && _cooldown <= 0)
            {
                int cooldownInSeconds = 30;
                int cooldownInTicks = cooldownInSeconds * 60;
           
                _cooldown = cooldownInTicks;
                modifiers.FinalDamage *= 0f;
                SoundEngine.PlaySound(SoundID.NPCDeath58, Player.position);

                int count = 48;
                float degreesPer = 360 / (float)count;
                for (int k = 0; k < count; k++)
                {
                    float degrees = k * degreesPer;
                    Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                    Vector2 vel = direction * 4;
                    Dust.NewDust(Player.Center, 1, 1, DustID.Water, vel.X , vel.Y);
                }

                _waterShieldProj.Kill();
                _waterShieldProj = null;
            }
        }
    }

    internal class OceancrestShield : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 42;
            Item.accessory = true;
            Item.defense = 4;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OceanShieldPlayer>().hasOceanShield = true;
            player.AddBuff(BuffID.Gills, 2);
        }
    }
}
