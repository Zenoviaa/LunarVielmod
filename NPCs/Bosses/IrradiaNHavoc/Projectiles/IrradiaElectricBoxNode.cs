using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.IrradiaNHavoc.Projectiles
{
    internal class IrradiaElectricBoxNode : ModNPC
    {
        private float Timer
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        private float Connected
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.TrailingMode[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 40;
            NPC.lifeMax = 1500;
            NPC.damage = 30;
            NPC.defense = 20;
            NPC.timeLeft = 720;
            NPC.noGravity = true;
        }

        public override void AI()
        {
            //Oscillate movement
            Timer++;
            if(Timer < 60)
            {
                float ySpeed = Timer / 60;
                ySpeed = Easing.SpikeCirc(ySpeed);
                NPC.velocity = new Vector2(0, -ySpeed);
            } 
            else if (Timer < 120)
            {
                //Inverse
                float ySpeed = 1f - ((Timer - 60) / 60);
                ySpeed = Easing.SpikeCirc(ySpeed);
                NPC.velocity = new Vector2(0, ySpeed);
            }

            if(Timer == 120)
            {
                Timer = 0;
            }
        }
    }
}
