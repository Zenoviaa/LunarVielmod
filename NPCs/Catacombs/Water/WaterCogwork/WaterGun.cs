using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Water.WaterCogwork
{
    internal class WaterGun : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.lifeMax = 100;
            NPC.width = 58;
            NPC.height = 34;
            NPC.damage = 1;
            NPC.defense = 10;
            NPC.lifeMax = 20000;
            NPC.value = 1f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
        }

        private void LookAtTarget()
        {
            NPC npc = NPC;

            //Look at target
            Player player = Main.player[NPC.target];

            // First, calculate a Vector pointing towards what you want to look at
            Vector2 vectorFromNpcToPlayer = player.Center - npc.Center;

            // Second, use the ToRotation method to turn that Vector2 into a float representing a rotation in radians.
            float desiredRotation = vectorFromNpcToPlayer.ToRotation();

            // A second approach is to use that rotation to turn the npc while obeying a max rotational speed. Experiment until you get a good value.
            npc.rotation = npc.rotation.AngleTowards(desiredRotation, 0.1f);
        }

        private ref float ai_Counter => ref NPC.ai[0];
        private ref float attack_Count => ref NPC.ai[1];

        public override void AI()
        {
            NPC npc = NPC;
            npc.TargetClosest();
            if (!npc.HasValidTarget)
                return;

            Player player = Main.player[NPC.target];
            LookAtTarget();

            ai_Counter++;
            if(ai_Counter > 60)
            {      
                Vector2 velocity = NPC.Center.DirectionTo(player.Center) * 15;
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STARSHOOT"));
                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity, 
                    ModContent.ProjectileType<WaterBolt>(), 47, 0f);
                
                ai_Counter = 0;
                attack_Count++;
            }

            if(attack_Count > 3)
            {
                NPC.Kill();
            }
        }
    }
}
