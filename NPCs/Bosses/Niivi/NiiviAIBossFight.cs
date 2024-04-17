using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Niivi.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi
{
    internal partial class Niivi
    {
        Player Target => Main.player[NPC.target];
        IEntitySource EntitySource => NPC.GetSource_FromThis();
        float DirectionToTarget
        {
            get
            {
                if (Target.position.X < NPC.position.X)
                    return -1;
                return 1;
            }
        }

        int AttackSide;
        bool Slowdown;
        Vector2 AttackPos;
        private void AIBossFight()
        {
            switch (BossState)
            {
                case BossActionState.Idle:
                    AI_Idle();
                    break;
                case BossActionState.Swoop_Out:
                    AI_SwoopOut();
                    break;
                case BossActionState.PrepareAttack:
                    AI_PrepareAttack();
                    break;
                case BossActionState.Frost_Breath:
                    AI_FrostBreath();
                    break;
                case BossActionState.Laser_Blast:
                    AI_LaserBlast();
                    break;
                case BossActionState.Star_Wrath:
                    AI_StarWrath();
                    break;
                case BossActionState.Charge:
                    AI_Charge();
                    break;
                case BossActionState.Thunderstorm:
                    AI_Thunderstorm();
                    break;
                case BossActionState.Baby_Dragons:
                    AI_BabyDragons();
                    break;
            }
            UpdateOrientation();
        }

        private void AI_Idle()
        {
            NPC.TargetClosest();
            Timer++;
            if(Timer >= 180)
            {
                ResetState(BossActionState.PrepareAttack);
                NextAttack = BossActionState.Frost_Breath;
            }

            UpdateOrientation();
            NPC.velocity *= 0.98f;
        }

        private void AI_FrostBreath()
        {
            Timer++;
            //Rotate Head
            TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

            //BREATHE
            if (Timer % 15 == 0 && StellaMultiplayer.IsHost)
            {
                float speed = 16;
                Vector2 velocity = NPC.Center.DirectionTo(Target.Center) * speed;
                int type = ModContent.ProjectileType<NiiviFrostBreathProj>();
                int damage = NPC.ScaleFromContactDamage(0.5f);
                float knockback = 1;
                Projectile.NewProjectile(EntitySource, NPC.Center, velocity, type,
                    damage, knockback, Main.myPlayer);
            }

            if (Timer >= 120)
            {
                ResetState(BossActionState.Swoop_Out);
            }
        }

        private void AI_SwoopOut()
        {
            Timer++;
            if(Timer == 1)
            {
                OrientationSpeed = 0.03f;
   
                OrientStraight();
                FlipToDirection();
          
                float swoopOutDistance = 768;
                AttackPos = NPC.Center + (-AttackSide * Vector2.UnitX * swoopOutDistance);
                NPC.velocity = NPC.Center.DirectionTo(AttackPos);
                TargetHeadRotation = NPC.velocity.ToRotation();
            }

            NPC.velocity *= 1.02f;
            NPC.velocity.Y -= 0.05f;
            if (Timer >= 120)
            {
                ResetState(BossActionState.Idle);
            }
        }

        private void AI_PrepareAttack()
        {
            Timer++;
            if (Timer == 1)
            {
                Slowdown = false;

                //Initialize Attack
                NPC.TargetClosest();
                LookDirection = DirectionToTarget;
                OrientArching();
                FlipToDirection();

                if (NPC.position.X > Target.position.X)
                {
                    AttackSide = 1;
                }
                else
                {
                    AttackSide = -1;
                }

                //Values
                float offsetDistance = 384;
                float hoverDistance = 180;

                //Get the direction
                Vector2 targetCenter = Target.Center + (AttackSide * Vector2.UnitX * offsetDistance) + new Vector2(0, -hoverDistance);
                AttackPos = targetCenter;
            }

            //Rotate Head
            TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

            if (AttackTimer == 0)
            {
                float flySpeed = 2;
                float distance = Vector2.Distance(NPC.Center, AttackPos);
                if (distance <= flySpeed)
                {
                    Slowdown = true;
                }
                else
                {
                    //Velocity
                    Vector2 targetVelocity = NPC.Center.DirectionTo(AttackPos) * flySpeed;
                    if(distance >= 384)
                    {
                        targetVelocity *= distance / 16;
                    }
                    NPC.velocity = targetVelocity;
                }
            }

            if (Slowdown)
            {
                AttackTimer++;
                NPC.velocity *= 0.98f;
                if(AttackTimer >= 60)
                {
                    ResetState(NextAttack);
                }
            }
        }

        private void AI_LaserBlast()
        {

        }

        private void AI_StarWrath()
        {

        }

        private void AI_Charge()
        {

        }

        private void AI_Thunderstorm()
        {

        }

        private void AI_BabyDragons()
        {

        }
    }
}
