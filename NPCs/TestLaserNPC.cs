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
