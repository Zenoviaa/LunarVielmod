using Microsoft.Xna.Framework;
using Stellamod.Content.NPCs.Bosses.Jiitas.Projectiles;
using Stellamod.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.NPCs.Bosses.Jiitas
{
    internal partial class Jiitas
    {
        public float SpinJumpHorizontalSpeed
        {
            get
            {
                if (Empowered)
                    return 35;
                return 25;
            }
        }
        public float NumSpinJumps
        {
            get
            {
                if (Empowered)
                    return 8;
                return 6;
            }
        }
        public int SpinJumpKnifeDamage
        {
            get
            {
                return 20;
            }
        }
        private void AI_SpinJump()
        {
            switch (ActionStep)
            {
                default:
                case 0:
                    AI_Sitdown();
                    break;
                case 1:
                    AI_SpinJumpStart();
                    break;
                case 2:
                    AI_SpinJumpLoop();
                    break;
                case 3:
                    AI_SpinJumpLand();
                    break;
                case 4:
                    AI_SpinJumpDragUp();
                    break;
            }
        }

        private void AI_SpinJumpStart()
        {
            Timer++;
            Warn();
            NPC.noGravity = false;
            NPC.velocity.X *= 0.9f;
            if (Timer == 1)
            {
                //JUMP
                int randIndex = Main.rand.Next(0, 3);
                switch (randIndex)
                {
                    case 0:
                        PlayAnimation(AnimationState.Jumpspin1);
                        break;
                    case 1:
                        PlayAnimation(AnimationState.Jumpspin2);
                        break;
                    case 2:
                        PlayAnimation(AnimationState.Jumpspin3);
                        break;
                }

                NPC.velocity.Y = -11;

                float jumpHorizontalSpeed = SpinJumpHorizontalSpeed;
                NPC.velocity.X = DirectionToTarget.X * jumpHorizontalSpeed;
                //Dust Particles
                for (int k = 0; k < 7; k++)
                {
                    Vector2 newVelocity = NPC.velocity.RotatedByRandom(MathHelper.ToRadians(7));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Dust.NewDust(NPC.Bottom, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
                }
                SoundEngine.PlaySound(SoundID.Item73, NPC.position);
                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_SpinJumpLoop()
        {
            Timer++;
            WarnContactDamage();
            NPC.rotation += DirectionToTarget.X * 0.01f;
            if (NPC.Bottom.Y < Target.Top.Y && Timer == 20)
            {
                //Throw Knives
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 throwVelocity = DirectionToTarget * 2;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, throwVelocity, ModContent.ProjectileType<JiitasKnife>(), SpinJumpKnifeDamage, 1, Main.myPlayer);
                }
            }

            if (NPC.Bottom.Y < Target.Top.Y && Timer == 30)
            {
                //Throw Knives
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 throwVelocity = DirectionToTarget * 2;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, throwVelocity, ModContent.ProjectileType<JiitasKnife>(), SpinJumpKnifeDamage, 1, Main.myPlayer);
                }
            }
            if (NPC.Bottom.Y < Target.Top.Y && Timer == 40)
            {
                //Throw Knives
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 throwVelocity = DirectionToTarget * 2;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, throwVelocity, ModContent.ProjectileType<JiitasKnife>(), SpinJumpKnifeDamage, 1, Main.myPlayer);
                }
            }
            if (Empowered)
            {
                if ( Timer == 50)
                {
                    //Throw Knives
                    if (MultiplayerHelper.IsHost)
                    {
                        Vector2 throwVelocity = DirectionToTarget * 2;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, throwVelocity, ModContent.ProjectileType<JiitasKnife>(), SpinJumpKnifeDamage, 1, Main.myPlayer);
                    }
                }
                if ( Timer == 60)
                {
                    //Throw Knives
                    if (MultiplayerHelper.IsHost)
                    {
                        Vector2 throwVelocity = DirectionToTarget * 2;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, throwVelocity, ModContent.ProjectileType<JiitasKnife>(), SpinJumpKnifeDamage, 1, Main.myPlayer);
                    }
                }
            }
            if (NPC.collideY && Timer >= 10)
            {
                Timer = 0;
                AttackCounter++;
                ActionStep++;
            }
        }

        private void AI_SpinJumpLand()
        {
            Timer++;
            NoWarn();
            PlayAnimation(AnimationState.Situp);
            NPC.velocity.X *= 0.9f;
            NPC.rotation *= 0.9f;
            if(Timer >= 30)
            {
                if (AttackCounter >= NumSpinJumps)
                {
                    Timer = 0;
                    ActionStep++;
                }
                else
                {
                    Timer = 0;
                    ActionStep = 1;
                }
            }
        }

        private void AI_SpinJumpDragUp()
        {
            PlayAnimation(AnimationState.Situp);
            ShouldDealContactDamage = false;

            Timer++;
            NPC.velocity.X *= 0.9f;
            NPC.velocity.Y -= 0.02f;
            NPC.noGravity = true;
            Empowered = false;
            if (Timer >= SitupTime)
            {
                SwitchState(ActionState.Idle);
            }
        }
    }
}
