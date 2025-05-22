using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Sylia.Projectiles;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia
{
    public partial class Sylia
    {
        public Vector2 QuickSlashV2Start;
        public Vector2 QuickSlashV2Velocity;
        private void AIPhase1()
        {
            //Determine the attack
            switch (State)
            {
                case ActionState.Idle:
                    IdleP1();
                    break;
                case ActionState.XScissor:
                    XScissorP1();
                    break;
                case ActionState.XScissor_Horizontal:
                    XScissorHorizontalP1();
                    break;
                case ActionState.QuickSlash_V2:
                    QuickSlashV2P1();
                    break;
                case ActionState.QuickSlash:
                    QuickSlashP1();
                    break;
                case ActionState.Scissor_Rain:
                    ScissorRainP1();
                    break;
            }
        }

        private void IdleHover()
        {
            float osc = VectorHelper.Osc(-1, 1);
            NPC.velocity = new Vector2(0, osc);
        }

        private void IdleP1()
        {
            Timer++;
            IdleHover();
            //Go to the next phase
            if (NPC.IsHealthLowerThanPercent(0.66f))
            {
                Phase = ActionPhase.Phase_2_Transition;
                ResetAI();
                return;
            }

            if(Timer >= 90 && StellaMultiplayer.IsHost)
            {
                //Determine attack
                switch (AttackCycle)
                {
                    default:
                    case 0:
                        SwitchState(ActionState.XScissor);
                        break;
                    case 1:
                        if (Main.rand.NextBool(2))
                        {
                            SwitchState(ActionState.XScissor_Horizontal);
                        }
                        else
                        {
                            SwitchState(ActionState.QuickSlash_V2);
                        }
        
                        break;
                    case 2:
                        if (Main.rand.NextBool(2))
                        {
                            SwitchState(ActionState.QuickSlash);
                        }
                        else
                        {
                            SwitchState(ActionState.XScissor);
                        }
                        break;
                    case 3:
                        if (Main.rand.NextBool(2))
                        {
                            SwitchState(ActionState.QuickSlash);
                        }
                        else
                        {
                            SwitchState(ActionState.Scissor_Rain);
                        }
                        break;
                    case 4:
                        if (Main.rand.NextBool(2))
                        {
                            SwitchState(ActionState.QuickSlash);
                        }
                        else
                        {
                            SwitchState(ActionState.QuickSlash_V2);
                        }
                        break;
                }

                AttackCycle++;
                int maxAttacks = 5;
                if(AttackCycle >= maxAttacks)
                {
                    AttackCycle = 0;
                }
            }
        }

        private void XScissorP1()
        {
            Timer++;
            IdleHover();
            float height = 200;

            //Determine where the x scissors go
            Vector2 topLeft = ArenaCenter + new Vector2(-ArenaRadius, -height);
            Vector2 topRight = ArenaCenter + new Vector2(ArenaRadius, -height);
            
            if(Timer % 16 == 0 && StellaMultiplayer.IsHost)
            {
                float rand = Main.rand.NextFloat(0.00f, 1.00f);
                Vector2 randPos = Vector2.Lerp(topLeft, topRight, rand);
                Vector2 velocity = Vector2.UnitX.RotatedBy(-MathHelper.PiOver4);
                randPos += new Vector2(0, Main.rand.NextFloat(-128, 0));
                Projectile.NewProjectile(NPC.GetSource_FromThis(), randPos, velocity,
                    ModContent.ProjectileType<XScissorRift>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);

                velocity = velocity.RotatedBy(-MathHelper.PiOver2);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), randPos, velocity,
                    ModContent.ProjectileType<XScissorRift>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);

                Projectile.NewProjectile(NPC.GetSource_FromThis(), randPos, Vector2.UnitY,
                    ModContent.ProjectileType<VoidBolt>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);
                TelegraphTimer++;
            }

            if(TelegraphTimer >= 5)
            {
                SwitchState(ActionState.Idle);
            }
        }

        private void XScissorHorizontalP1()
        {
            Timer++;
            IdleHover();
            Vector2 left = ArenaCenter + new Vector2(-ArenaRadius, 0);
            Vector2 right = ArenaCenter + new Vector2(ArenaRadius, 0);

            if (Timer % 16 == 0 && StellaMultiplayer.IsHost)
            {
                int randDir = Main.rand.NextBool(2) ? -1 : 1;
                float range = 128;
                Vector2 randPos;
                if(randDir == -1)
                {
                    randPos = left + new Vector2(0, Main.rand.NextFloat(-range, range));
                }
                else
                {
                    randPos = right + new Vector2(0, Main.rand.NextFloat(-range, range));
                }

                Vector2 velocity = Vector2.UnitX.RotatedBy(-MathHelper.PiOver4);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), randPos, velocity,
                    ModContent.ProjectileType<XScissorRift>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);

                Projectile.NewProjectile(NPC.GetSource_FromThis(), randPos, Vector2.UnitX * (-randDir),
                    ModContent.ProjectileType<VoidBolt>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);
                TelegraphTimer++;
            }

            if (TelegraphTimer >= 5)
            {
                SwitchState(ActionState.Idle);
            }
        }

        private void QuickSlashV2P1()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.velocity = Vector2.Zero;
            }

            if(Timer == 1 && StellaMultiplayer.IsHost)
            {
             
                //Get random start target
                float range = 256;
                if (Main.rand.NextBool(2))
                {
                    QuickSlashV2Start = ArenaCenter + new Vector2(-ArenaRadius, Main.rand.NextFloat(-range, range));    
                
                }
                else
                {
                    QuickSlashV2Start = ArenaCenter + new Vector2(ArenaRadius, Main.rand.NextFloat(-range, range));
                }

                QuickSlashV2Velocity = QuickSlashV2Start.DirectionTo(Target.Center) * 32;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), QuickSlashV2Start, QuickSlashV2Velocity * 1.25f,
                    ModContent.ProjectileType<VoidLine>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);
                NPC.netUpdate = true;
            }

            if(Timer == 59)
            {  
                //Teleport to it
                DrawMagicCircle = false;
                Teleport(QuickSlashV2Start.X, QuickSlashV2Start.Y);
            }

            if(Timer > 60 && Timer < 120)
            {
                //Move along it
                NPC.velocity = QuickSlashV2Velocity;
                if(Timer % 4 == 0 && StellaMultiplayer.IsHost)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(1, 1);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                       ModContent.ProjectileType<RipperSlashProjSmall>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                        ModContent.ProjectileType<XScissorRift>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);
                }
            }

            if (Timer == 119)
            {
                NPC.velocity = Vector2.Zero;
                Teleport(ArenaCenter.X, ArenaCenter.Y);
            }

            if(Timer == 120)
            {
                Timer = 0;
                TelegraphTimer++;   
            }

            if(TelegraphTimer >= 3)
            {
                DrawMagicCircle = true;
                NPC.velocity = Vector2.Zero;
                SwitchState(ActionState.Idle);
            }
        }

        private void QuickSlashP1()
        {
            Timer++;
            IdleHover();
            if(Timer % 20 == 0)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), Target.Center, velocity,
                        ModContent.ProjectileType<XScissorRift>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);
                }

                TelegraphTimer++;
            }


            if(TelegraphTimer >= 3)
            {
                SwitchState(ActionState.Idle);
            }
        }

        private void ScissorRainP1()
        {
            Timer++;
            IdleHover();
            if (Timer < 60)
            {
                ChargeVisuals(Timer, 60);
                return;
            }

            if (Timer % 10 == 0 && StellaMultiplayer.IsHost)
            {
                Vector2 velocity = NPC.Center.DirectionTo(Target.Center);
                velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 4);
                velocity *= 16;

                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlash");
                soundStyle.Pitch = -0.5f;
                soundStyle.PitchVariance = 0.15f;
                soundStyle.Volume = 0.3f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + velocity.SafeNormalize(Vector2.Zero) * 32, velocity,
                      ModContent.ProjectileType<VoidScissor>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);
               
                TelegraphTimer++;
            }

            if(Timer % 40 == 0 && StellaMultiplayer.IsHost)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), Target.Center, velocity,
                    ModContent.ProjectileType<XScissorRift>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);
            }

            if(TelegraphTimer >= 21)
            {
                SwitchState(ActionState.Idle);
            }
        }
    }
}
