using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Sylia.Projectiles;
using Stellamod.Particles;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia
{
    public partial class Sylia
    {
        private bool _tooFar;
        private float _tooFarCounter;
        private void AIPhase2()
        {
            switch (State)
            {
                case ActionState.Idle:
                    IdleP2();
                    break;
                case ActionState.XScissor:
                    XScissorP2();
                    break;
                case ActionState.XScissor_Horizontal:
                    XScissorThrowP2();
                    break;
                case ActionState.QuickSlash_V2:
                    QuickSlashV2P2();
                    break;
                case ActionState.QuickSlash:
                    QuickSlashP2();
                    break;
            }
        }

        private void P2MoveRight()
        {
            Vector2 targetCenter = Target.Center + new Vector2(0, -64);
            Vector2 homingVelocity = (targetCenter - NPC.Center).SafeNormalize(Vector2.Zero) * 3.5f;

            float tooFarDistance = 16 * 64;
            float distanceToTarget = Vector2.Distance(NPC.Center, targetCenter);
            if (!_tooFar)
            {
                if (distanceToTarget > tooFarDistance)
                {
                    _tooFarCounter++;
                    if (_tooFarCounter > 30)
                    {
                        _tooFar = true;
                    }
                }

                NPC.velocity = homingVelocity;
            }
            else
            {
                if (distanceToTarget < tooFarDistance)
                {
                    _tooFarCounter--;
                    if (_tooFarCounter <= 0)
                    {
                        _tooFar = false;
                    }
                }

                NPC.velocity = new Vector2(homingVelocity.X * 4f, homingVelocity.Y);
            }
        }
        private void P2MoveRightOfPlayer(float offset = 0)
        {
            NPC npc = NPC;
            Player target = Main.player[npc.target];
            Vector2 targetCenter = target.Center;

            float playerRightCenterOffsetX = 256 + offset;
            Vector2 playerRightCenterOffset = new Vector2(playerRightCenterOffsetX, 0);
            Vector2 playerRightCenter = targetCenter + playerRightCenterOffset;

            float distanceToTarget = Vector2.Distance(npc.Center, playerRightCenter);
            float slowdownDistance = 8;
            float distanceCorrector = 1;
            if (distanceToTarget < slowdownDistance)
            {
                distanceCorrector = distanceToTarget / slowdownDistance;
            }

            Vector2 playerRightVelocity = npc.Center.DirectionTo(playerRightCenter) * 16 * distanceCorrector;
            npc.velocity = Vector2.Lerp(npc.velocity, playerRightVelocity, 0.1f);
        }
        private void IdleP2()
        {
            P2MoveRight();
            Timer++;
            if(Timer >= 240 && StellaMultiplayer.IsHost)
            {
                switch (AttackCycle)
                {
                    case 0:
                        SwitchState(ActionState.XScissor);
                        break;
                    case 1:
                        SwitchState(ActionState.XScissor_Horizontal);
                        break;
                    case 2:
                        SwitchState(ActionState.QuickSlash_V2);
                        break;
                    case 3:
                        SwitchState(ActionState.QuickSlash);
                        break;
                }

                AttackCycle++;
                int maxAttacks = 4;
                if (AttackCycle >= maxAttacks)
                {
                    AttackCycle = 0;
                }
            }
        }

        private void XScissorP2()
        {
            P2MoveRight();
            Timer++;
            if(Timer % 30 == 0 && StellaMultiplayer.IsHost)
            {
                Vector2 velocity = Vector2.UnitX.RotatedBy(-MathHelper.PiOver4);
                Vector2 offset = Target.velocity.SafeNormalize(Vector2.Zero) * 768;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), Target.Center + offset, velocity,
                    ModContent.ProjectileType<XScissorRift>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);

                velocity = velocity.RotatedBy(-MathHelper.PiOver2);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), Target.Center + offset, velocity,
                    ModContent.ProjectileType<XScissorRift>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);

                TelegraphTimer++;
            }

            if(TelegraphTimer > 7)
            {
                SwitchState(ActionState.Idle);
                TelegraphTimer = 0;
            }
        }

        private void XScissorThrowP2()
        {
            P2MoveRight();
            Timer++;
            if(Timer == 30)
            {
                Teleport(Target.Center.X, Target.Center.Y - 48);
            }

            if (Timer < 90)
            {
                ChargeVisuals(Timer, 90);
                //Don't run the rest of the AI< we charging up 
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
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                      ModContent.ProjectileType<VoidScissor2>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);

                TelegraphTimer++;
            }


            if (TelegraphTimer > 7)
            {
                SwitchState(ActionState.Idle);
                TelegraphTimer = 0;
            }
        }

        private void QuickSlashV2P2()
        {
            Timer++;
            if (Timer < 180)
            {
                float xOffset = 768;
                float yOffset = -512;
                Vector2 verticalHoverTarget = Target.Center + new Vector2(xOffset, yOffset);
                NPC.velocity = VectorHelper.VelocitySlowdownTo(NPC.position, verticalHoverTarget, 30 * (Timer / 60));
            }
            else
            {
                Vector2 dashVelocity = new Vector2(1, 0) * 16;
                NPC.velocity = Vector2.Lerp(NPC.velocity, dashVelocity, 0.5f);
                if (Timer % 5 == 0)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(1, 1);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                            ModContent.ProjectileType<VoidHorizontalRift>(), 45, 1, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                            ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, Main.myPlayer);
                    }
                    TelegraphTimer++;
                }

                if(TelegraphTimer == 15)
                {
                    SwitchState(ActionState.Idle);
                }
            }
        }

        private void QuickSlashP2()
        {
            Timer++;
            if (Timer % 20 == 0)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), Target.Center, velocity,
                        ModContent.ProjectileType<XScissorRift>(), NPC.ScaleFromContactDamage(1f), 1, Main.myPlayer);
                }

                TelegraphTimer++;
            }


            if (TelegraphTimer >= 3)
            {
                SwitchState(ActionState.Idle);
            }
        }
    }
}
