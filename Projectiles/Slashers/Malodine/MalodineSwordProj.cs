using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod;
using Stellamod.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.Malodine
{
    public class MalodineSwordProj : ModProjectile
    {
        public static bool swung = false;
        public int SwingTime = 35;
        public float holdOffset = 30f;
        public int combowombo;
        private bool _initialized;
        private int timer;
        private bool ParticleSpawned;

        public override void SetDefaults()
        {
            Projectile.damage = 10;
            Projectile.timeLeft = SwingTime;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 74;
            Projectile.width = 74;
            Projectile.friendly = true;
            Projectile.scale = 1f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

      
    }
}