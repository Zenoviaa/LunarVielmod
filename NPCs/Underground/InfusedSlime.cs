using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Underground
{
    internal class InfusedSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 23;
        }

        public override void SetDefaults()
        {
            NPC.width = 86;
            NPC.height = 48;
            NPC.damage = 68;
            NPC.defense = 8;
            NPC.lifeMax = 100;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.value = 563f;
            NPC.knockBackResist = .45f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.4f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        private ref float AI_Timer => ref NPC.ai[0];
        private ref float AI_Walk_Direction => ref NPC.ai[1];

        private void AI_Movement(float walkDirection)
        {
            walkDirection = Math.Sign(walkDirection);
            float targetX = walkDirection * 1;
            float accel = 0.2f;
            if (targetX <= -1)
            {
                if (NPC.velocity.X > targetX)
                {
                    NPC.velocity.X -= accel;
                }
                else
                {
                    NPC.velocity.X += accel;
                }

            }
            else if (targetX >= 1)
            {
                if (NPC.velocity.X < targetX)
                {
                    NPC.velocity.X += accel;
                }
                else
                {
                    NPC.velocity.X -= accel;
                }
            }
            else
            {
                NPC.velocity.X *= 0.9f;
            }
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            NPC.TargetClosest();
            if (NPC.collideX)
            {
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
            }

            if (NPC.HasValidTarget && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height))
            {
                float walkDirection = NPC.DirectionTo(target.Center).X;
                AI_Movement(walkDirection);
                AI_Timer++;
                if (AI_Timer == 60)
                {
                    SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, NPC.position);
                }
                else if (AI_Timer > 60 && AI_Timer < 120)
                {
                    Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.Electric);
                    dust.velocity *= -1f;
                    dust.scale *= 0.5f;
                    dust.noGravity = true;
                    Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                    vector2_1.Normalize();
                    Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                    dust.velocity = vector2_2;
                    vector2_2.Normalize();
                    Vector2 vector2_3 = vector2_2 * 34f;
                    dust.position = NPC.Center - vector2_3;

                    //Telegraph
                    NPC.FaceTarget();

                }
                else if (AI_Timer >= 120)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 velocity = NPC.DirectionTo(target.Center) * 10;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                                ModContent.ProjectileType<InfusedSlimeBolt>(), 34, 1, Owner: Main.myPlayer);
                    }

                    SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, NPC.position);
                    AI_Timer = 0;
                }
            } 
            else
            {
                float walkDirection = AI_Walk_Direction;
                AI_Movement(walkDirection);
                if (Main.rand.NextBool(300))
                {
                    AI_Walk_Direction = Main.rand.Next(-1, 2);
                    NPC.netUpdate = true;
                }
            }

            Visuals();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnRates.GetMechanicalEnemySpawnChance(spawnInfo);
        }
        private void Visuals()
        {
            if (Main.rand.NextBool(16))
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                Color.White.ToVector3(),
                Color.WhiteSmoke.ToVector3(),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(NPC, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, Color.White, Color.WhiteSmoke, 0);
            Lighting.AddLight(screenPos, Color.White.ToVector3() * 1.0f * Main.essScale);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BrokenTech>(), 6, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Electrifying>(), 7, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ItemID.IronOre, 1, 1, 5));
        }
    }
}
