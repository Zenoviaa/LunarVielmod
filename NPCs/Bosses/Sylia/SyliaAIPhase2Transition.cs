using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Sylia.Projectiles;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia
{
    public partial class Sylia
    {
        private void AIPhase2Transition()
        {
            AttackCycle = 0;
            Timer++;
            Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(NPC.Center, 1f);
            if(Main.netMode != NetmodeID.Server)
            {
                FilterManager filterManager = Terraria.Graphics.Effects.Filters.Scene;
                filterManager[ShaderRegistry.Screen_Black].GetShader()
                 .UseProgress(1f - (Timer / 240));
                filterManager.Activate(ShaderRegistry.Screen_Black);
            }
  

            //Kill the barrier
            if (Timer == 1)
            {
   

                DrawMagicCircle = true;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SyliaTransition"));
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type == ModContent.ProjectileType<VoidBeamBarrier>())
                    {
                        Main.projectile[i].timeLeft = 2;
                    }
                }
            }

            Vector2 targetCenter = ArenaCenter + new Vector2(-ArenaRadius, 0);
            MoveTo(targetCenter, 7);
            if(Timer < 240)
            {
                ChargeVisuals(Timer, 240);
            }

            if(Timer == 240)
            {
                //Void Wall Spawn Here
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y + 128, ModContent.NPCType<VoidWall>());
                }
               
                SoundEngine.PlaySound(SoundID.NPCDeath62, NPC.position);
                ShakeModSystem.Shake = 0;
                Phase = ActionPhase.Phase_2;
                ResetAI();
            }
        }
    }
}
