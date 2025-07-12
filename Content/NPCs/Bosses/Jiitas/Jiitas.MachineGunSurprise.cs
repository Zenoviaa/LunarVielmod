using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.NPCs.Bosses.Jiitas.Projectiles;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.NPCs.Bosses.Jiitas
{
    internal partial class Jiitas
    {
        public static float UndressStartupTime
        {
            get
            {
                return 60;
            }
        }
        public static float RedressTime
        {
            get
            {
                return 60;
            }
        }
        public static float UndressBlitzTime
        {
            get
            {
                return 300;
            }
        }
        public static float TimeBetweenShots
        {
            get
            {
                return 5;
            }
        }

        public static int JiitasShotDamage
        {
            get
            {
                return 20;
            }
        }
        private Vector2 SurpriseTrackingPosition;
        private Vector2 SurpriseTrackingVelocity;
        private void AI_MachineGunSurprise()
        {
            NPC.noGravity = false;
            switch (ActionStep)
            {
                case 0:
                    AI_Undress();
                    break;
                case 1:
                    AI_DressShot();
                    break;
                case 2:
                    AI_Redress();
                    break;
            }
        }

        private void AI_Undress()
        {
            Timer++;
            if(Timer == 1)
            {
                SoundStyle jiitasSit = AssetRegistry.Sounds.Jiitas.JiitasReload;
                jiitasSit.PitchVariance = 0.2f;
                SoundEngine.PlaySound(jiitasSit, NPC.position);
            }
            NPC.TargetClosest();
            Warn();
            if (Timer <= 4)
            {
                NPC.velocity -= DirectionToTarget * 2;
            }
            SurpriseTrackingPosition = NPC.Center;
            SurpriseTrackingVelocity = DirectionToTarget * 3.5f;
            PlayAnimation(AnimationState.Undress);
            NPC.noGravity = false;
            NPC.velocity.X *= 0.9f;
            NPC.rotation *= 0.9f;
            if (Timer >= UndressStartupTime)
            {
                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_DressShot()
        {
            Timer++;
            NPC.noGravity = true;
            NPC.rotation *= 0.9f;
            NPC.velocity.X *= 0.9f;
            PlayAnimation(AnimationState.Undress);

            if (Empowered)
            {
                if(Timer % (TimeBetweenShots * 8) == 0)
                {
                    if (MultiplayerHelper.IsHost)
                    {
                        Vector2 spawnPos = Target.Center;
                        spawnPos.Y -= 600;
                        spawnPos.X += Main.rand.NextFloat(-500, 500);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, -Vector2.UnitY, 
                            ModContent.ProjectileType<JiitasBombString>(), BombDropDamage, 1, Main.myPlayer);
                    }
                }
            }
            //Tracking the player
            Vector2 newVelocity = ProjectileHelper.SimpleHomingVelocity(SurpriseTrackingPosition, Target.Center, SurpriseTrackingVelocity, degreesToRotate: 3);
            SurpriseTrackingVelocity = newVelocity;
            SurpriseTrackingPosition += SurpriseTrackingVelocity;
            if (Timer % TimeBetweenShots == 0)
            {
                Vector2 shootPosition = SurpriseTrackingPosition;
                shootPosition.Y = Target.Bottom.Y;
                shootPosition += Main.rand.NextVector2Circular(8, 8);
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), shootPosition, Vector2.Zero, ModContent.ProjectileType<JiitasGunShot>(), JiitasShotDamage, 0, Main.myPlayer);
                }

                Vector2 muzzlePosition = NPC.Bottom + new Vector2(0, -24);
                muzzlePosition += Main.rand.NextVector2Circular(8, 8);
                muzzlePosition += DirectionToTarget * 24;
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(muzzlePosition,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Red,
                        baseSize: Main.rand.NextFloat(0.025f, 0.035f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
                NPC.velocity -= DirectionToTarget * 0.1f;
                AttackCounter++;
            }

            if (Timer >= UndressBlitzTime)
            {
                if(Empowered && AttackCounter == 0)
                {
                    Timer = 0;

                    AttackCounter++;
                }
                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_Redress()
        {
            Timer++;
            NoWarn();
            NPC.velocity.X *= 0.9f;
            NPC.rotation *= 0.9f;
            PlayAnimation(AnimationState.Redress);
            Empowered = false;
            if (Timer >= RedressTime)
            {
                SwitchState(ActionState.Idle);
            }
        }
    }
}
