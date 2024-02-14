using Stellamod.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Stellamod.NPCs
{
    internal class TestLaserNPC : ModNPC
    {
        private int _timer = 600;
        private float _circleDegrees;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 60;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 115;
            NPC.height = 85;
            NPC.lifeMax = 1000;
            NPC.damage = 50;
        }

        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            NPC.TargetClosest();
            Player target = Main.player[NPC.target];
            Vector2 velocity = NPC.Center.DirectionTo(target.Center) * 10;

            //Circling Code
            float circleSpeed = 2;

            //How far from the player the NPC will be
            float circleDistance = 128;

            _circleDegrees += circleSpeed;
            float circleRadians = MathHelper.ToRadians(_circleDegrees);
            Vector2 offsetFromPlayer = new Vector2(circleDistance, 0).RotatedBy(circleRadians);
            Vector2 circlePosition = target.Center + offsetFromPlayer;

            //This is just how quickly the NPC will move to the circle position
            //This number should be higher than the circle speed
            float movementSpeed = 15;
            NPC.velocity = VectorHelper.VelocitySlowdownTo(NPC.Center, circlePosition, movementSpeed);

            _timer--;
            if(_timer <= 0)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                        ModContent.ProjectileType<TestRay>(), 10, 10, Main.myPlayer, ai0: NPC.whoAmI);
                }
                _timer = 600;
            }
        }
    }
}
