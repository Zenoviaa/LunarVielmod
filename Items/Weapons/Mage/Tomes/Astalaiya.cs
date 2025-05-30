﻿using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage.Tomes
{
    internal class Astalaiya : BaseMagicTomeItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.shoot = ModContent.ProjectileType<AstalaiyaTome>();
            Item.shootSpeed = 10f;
        }
    }

    internal class AstalaiyaTome : BaseMagicTomeProjectile
    {
        private int Star;
        private float _dustTimer;
        public override string Texture => this.PathHere() + "/Astalaiya";
        public override void SetDefaults()
        {
            base.SetDefaults();
            //How often it shoots
            AttackRate = 12;

            //How fast it drains mana, better to change the mana use in the item instead of this tho
            ManaConsumptionRate = 4;

            //How far the tome is held from the player
            HoldDistance = 36;

            //The glow effect around it
            GlowDistanceOffset = 4;
            GlowRotationSpeed = 0.05f;
        }

        public override void AI()
        {
            base.AI();
            _dustTimer++;
            if (_dustTimer % 16 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.LightPink, Main.rand.NextFloat(1f, 1.5f));
            }
        }

        protected override void Shoot(Player player, IEntitySource source, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Shoot(player, source, position, velocity, damage, knockback);
            int type = ModContent.ProjectileType<AstalaiyaStar1>();
            Star += 1;
            if (Star >= 3)
            {
                Star = 0;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya3"), player.position);
                type = ModContent.ProjectileType<AstalaiyaStar1>();
            }
            if (Star == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya2"), player.position);
                type = ModContent.ProjectileType<AstalaiyaStar3>();
            }
            if (Star == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya1"), player.position);
                type = ModContent.ProjectileType<AstalaiyaStar2>();
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Projectile.owner);
            }
        }
    }

}
