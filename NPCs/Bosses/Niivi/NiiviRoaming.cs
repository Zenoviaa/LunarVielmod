using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.NPCs.Bosses.Niivi.Projectiles;
using Stellamod.NPCs.Town;
using Stellamod.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.NPCs.Bosses.Niivi.Niivi;

namespace Stellamod.NPCs.Bosses.Niivi
{
    internal partial class NiiviRoaming : ModNPC
    {
        public enum ActionState
        {
            Roaming,
            Obliterate,
            Sleeping
        }

        public ActionState State
        {
            get
            {
                return (ActionState)NPC.ai[0];
            }
            set
            {
                NPC.ai[0] = (float)value;
            }
        }

        //AI
        ref float Timer => ref NPC.ai[1];
        ref float AttackTimer => ref NPC.ai[2];

        int SleepingTimer;
        int ScaleDamageCounter;
        int AggroDamageCounter;

        private Player Target => Main.player[NPC.target];
        private IEntitySource EntitySource => NPC.GetSource_FromThis();
        private float DirectionToTarget
        {
            get
            {
                if (Target.position.X < NPC.position.X)
                    return -1;
                return 1;
            }
        }

        public float RoamingDirection = -1f;
        public float RoamingSpeed = 2;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ScaleDamageCounter);
            writer.Write(AggroDamageCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ScaleDamageCounter = reader.ReadInt32();
            AggroDamageCounter = reader.ReadInt32();
        }

        public override void SetStaticDefaults()
        {
            //Don't want her to be hit by any debuffs
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.TrailCacheLength[Type] = Total_Segments;
            NPCID.Sets.TrailingMode[Type] = 2;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
            drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Niivi/NiiviPreview";
            drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
            drawModifiers.PortraitPositionYOverride = 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Niivi, The First Dragon")
            });
        }

        public override void SetDefaults()
        {
            //Stats
            NPC.lifeMax = 232000;
            NPC.defense = 90;
            NPC.damage = 240;
            NPC.width = (int)NiiviHeadSize.X;
            NPC.height = (int)NiiviHeadSize.Y;
            //It won't be considered a boss or take up slots until the fight actually starts
            //So the values are like this for now
            NPC.npcSlots = 0f;
            NPC.aiStyle = -1;

            //She'll tile collide and have gravity while on the ground, but not while airborne.
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;

            NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
        }


        private void ResetState(ActionState actionState)
        {
            State = actionState;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int lifeToGiveIllurineScale = NPC.lifeMax / 300;
            int lifeToGiveIllurineScaleInBoss = NPC.lifeMax / 100;
            if (StellaMultiplayer.IsHost)
            {
                AggroDamageCounter += hit.Damage;
                ScaleDamageCounter += hit.Damage;
                if (ScaleDamageCounter >= lifeToGiveIllurineScale)
                {
                    Vector2 velocity = -Vector2.UnitY;
                    velocity *= Main.rand.NextFloat(4, 8);
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, velocity,
                        ModContent.ProjectileType<NiiviScaleProj>(), 0, 1, Main.myPlayer);
                    ScaleDamageCounter = 0;
                }

                if (AggroDamageCounter >= lifeToGiveIllurineScale * 15)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Niivi>());
                    NPC.active = false;
                    AggroDamageCounter = 0;
                }
            }
        }
        public override void AI()
        {
            NPC.TargetClosest();
            switch (State)
            {
                case ActionState.Roaming:
                    AI_Roaming();
                    break;
                case ActionState.Sleeping:
                    AI_Sleeping();
                    break;
            }
        }

        private void AI_Roaming()
        {
            if (!Main.dayTime)
            {
                AI_Roaming_GoHome();
            }
            else
            {
                AIRoaming_FlyAroundTree();
            }
        }

        private void AI_Sleeping()
        {
            if (Main.dayTime)
            {
                SleepingTimer = 0;
                ResetState(ActionState.Roaming);
            }
            else
            {
                FlightDirection = 1;
                LookDirection = 1;
                StartSegmentDirection = -Vector2.UnitX;

                //Go sleep
                Vector2 sleepPos = AlcadSpawnSystem.NiiviSpawnWorld + new Vector2(0, 164);
                NPC.Center = Vector2.Lerp(NPC.Center, sleepPos, 0.01f);
                NPC.velocity *= 0.9f;
                TargetSegmentRotation = -MathHelper.PiOver4 / 80;
                TargetHeadRotation = 0;
                SleepingTimer++;
                if (SleepingTimer > 60 && SleepingTimer % 60 == 0)
                {
                    ParticleManager.NewParticle<ZeeParticle>(NPC.Center + new Vector2(64, -32), -Vector2.UnitY, Color.White, 1f);
                }
            }
            UpdateOrientation();
        }


        private void AI_MoveToward(Vector2 targetCenter, float speed = 8)
        {
            //chase target
            Vector2 directionToTarget = NPC.Center.DirectionTo(targetCenter);
            float distanceToTarget = Vector2.Distance(NPC.Center, targetCenter);
            if (distanceToTarget < speed)
            {
                speed = distanceToTarget;
            }

            Vector2 targetVelocity = directionToTarget * speed;

            if (NPC.velocity.X < targetVelocity.X)
            {
                NPC.velocity.X++;
                if (NPC.velocity.X >= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }
            else if (NPC.velocity.X > targetVelocity.X)
            {
                NPC.velocity.X--;
                if (NPC.velocity.X <= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }

            if (NPC.velocity.Y < targetVelocity.Y)
            {
                NPC.velocity.Y++;
                if (NPC.velocity.Y >= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
            else if (NPC.velocity.Y > targetVelocity.Y)
            {
                NPC.velocity.Y--;
                if (NPC.velocity.Y <= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
        }

        private void AIRoaming_FlyAroundTree()
        {
            float orbitDistance = 2000;
            Vector2 home = AlcadSpawnSystem.NiiviSpawnWorld + new Vector2(0, 1024);
            Vector2 direction = home.DirectionTo(NPC.Center);
            direction = direction.RotatedBy(MathHelper.TwoPi / 2000);
            Vector2 targetCenter = home + direction * orbitDistance;
            Vector2 directionToTargetCenter = NPC.Center.DirectionTo(targetCenter);
            AI_MoveToward(targetCenter, 2);
            OrientArching();
            if (directionToTargetCenter.X > 0)
            {
                FlightDirection = 1;
                LookDirection = 1;
                StartSegmentDirection = -Vector2.UnitX;

            }
            else
            {
                FlightDirection = -1;
                LookDirection = -1;
                StartSegmentDirection = Vector2.UnitX;
                TargetHeadRotation = -MathHelper.PiOver4 + MathHelper.Pi;
            }

            UpdateOrientation();
            LookAtTarget();
        }

        private void AI_Roaming_GoHome()
        {
            Vector2 home = AlcadSpawnSystem.NiiviSpawnWorld;
            Vector2 directionToHome = NPC.Center.DirectionTo(home);
            float distanceToHome = Vector2.Distance(NPC.Center, home);

            //Set orientation
            if (directionToHome.X > 0)
            {
                FlightDirection = 1;
                LookDirection = 1;
                StartSegmentDirection = -Vector2.UnitX;
                OrientStraight();
                TargetHeadRotation = NPC.velocity.ToRotation();

            }
            else
            {
                FlightDirection = -1;
                LookDirection = -1;
                StartSegmentDirection = Vector2.UnitX;
                OrientStraight();
                TargetHeadRotation = NPC.velocity.ToRotation();
            }


            float speed = MathHelper.Min(RoamingSpeed, distanceToHome);
            Vector2 targetVelocity = directionToHome * speed;
            targetVelocity += new Vector2(0, VectorHelper.Osc(-1, 1));
            NPC.velocity = targetVelocity;

            if (distanceToHome <= 1)
            {
                ResetState(ActionState.Sleeping);
            }

            UpdateOrientation();
            LookAtTarget();
        }
    }
}
