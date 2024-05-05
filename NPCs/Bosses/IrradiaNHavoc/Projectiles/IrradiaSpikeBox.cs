using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.IrradiaNHavoc.Projectiles
{
    internal class IrradiaSpikeBox : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.lifeMax = 500;
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 120;
            NPC.noGravity = true;
        }

        public override void AI()
        {
            NPC.velocity *= 0.98f;
            if (NPC.collideX)
            {
                NPC.velocity.X = -NPC.velocity.X;
            }
            if (NPC.collideY)
            {
                NPC.velocity.Y = -NPC.velocity.Y;
            }

            NPC.TargetClosest();
            Player target = Main.player[NPC.target];
            if (NPC.HasValidTarget)
            {
                Vector2 directionToTarget = NPC.Center.DirectionTo(target.Center);
                NPC.velocity += directionToTarget * 0.01f;
            }
            Visuals();
        }

        private void Visuals()
        {
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.78f);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
    }
}
