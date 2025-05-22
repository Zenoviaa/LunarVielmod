using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.NPCs.Bosses.StarrVeriplant;
using Stellamod.Projectiles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Cinderspark
{
    // These three class showcase usage of the WormHead, WormBody and WormTail classes from Worm.cs
    internal class CinderCrawlerHead : WormHead
    {
        private int _attackCounter;
        private int _movementTimer;
        private float _xDir;
        private float _yDir;
        public override int BodyType => ModContent.NPCType<CinderCrawlerBody>();

        public override int TailType => ModContent.NPCType<CinderCrawlerTail>();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            //If your worm is not connected, messed with this NPC.width number
            //It is based on that, lower will make them closer, higher will make them farther
            NPC.width = 22;
            NPC.height = 70;
            NPC.damage = 70;
            NPC.defense = 10;
            NPC.lifeMax = 500;
            NPC.value = 5000f;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 1;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.lavaImmune = true;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void Init()
        {
            // Set the segment variance
            // If you want the segment length to be constant, set these two properties to the same value
            MinSegmentLength = 24;
            MaxSegmentLength = 36;

            CommonWormInit(this);
        }

        // This method is invoked from ExampleWormHead, ExampleWormBody and ExampleWormTail
        internal static void CommonWormInit(Worm worm)
        {
            // These two properties handle the movement of the worm
            worm.MoveSpeed = 13f;
            worm.Acceleration = 0.1f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }


        public override void AI()
        {
            NPC.TargetClosest();
            if (NPC.HasValidTarget)
            {
                _attackCounter++;
                _movementTimer++;
                Player target = Main.player[NPC.target];

                //The below code is for cardinal only flying movement
                //Get the distancessss
                float xDist = Math.Abs(target.Center.X - NPC.Center.X);
                float yDist = Math.Abs(target.Center.Y - NPC.Center.Y);

                //Switch Direction
                if(_movementTimer > 60)
                {
                    if (xDist > yDist)
                    {
                        if (target.Center.X < NPC.Center.X)
                        {
                            _xDir = -1f;
                            _yDir = 0f;
                        }
                        else if (target.Center.X > NPC.Center.X)
                        {
                            _xDir = 1f;
                            _yDir = 0f;
                        }
                    }
                    else if (yDist > xDist)
                    {
                        if (target.Center.Y < NPC.Center.Y)
                        {
                            _xDir = 0f;
                            _yDir = -1f;
                        }
                        else if (target.Center.Y > NPC.Center.Y)
                        {
                            _xDir = 0f;
                            _yDir = 1f;
                        }
                    }
                    _movementTimer = 0;
                }

                Vector2 targetDirection = new Vector2(_xDir, _yDir);
                Vector2 targetVelocity = new Vector2(_xDir, _yDir) * 1.2f;
                NPC.velocity = targetVelocity;
                if(_attackCounter > 120)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        for (int i = 0; i < Main.rand.Next(4, 8); i++)
                        {
                            Vector2 velocity = targetDirection.RotatedByRandom(MathHelper.ToRadians(10)) * 30;
                            velocity *= new Vector2(
                                Main.rand.NextFloat(0.5f, 1f),
                                Main.rand.NextFloat(0.5f, 1f));

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                                ModContent.ProjectileType<CinderFireball2>(), (int)(NPC.damage * 0.1f), 1, Main.myPlayer);

                            //Dust Particles
                            for (int k = 0; k < 4; k++)
                            {
                                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
                                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                                Dust.NewDust(NPC.Center, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
                            }
                        }
                    }

                    SoundEngine.PlaySound(SoundID.Item73, NPC.position);
                    _attackCounter = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<CindersparkBiome>() && !spawnInfo.Player.ZoneUnderworldHeight)
            {
                return 0.15f;
            }

            //Else, the example bone merchant will not spawn if the above conditions are not met.
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cinderscrap>(), chanceDenominator: 4, minimumDropped: 2, maximumDropped: 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoltenScrap>(), chanceDenominator: 2, minimumDropped: 1, maximumDropped: 3));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                Color.OrangeRed.ToVector3(),
                Color.Red.ToVector3(),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(NPC, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, Color.OrangeRed, Color.White, 0);
            Lighting.AddLight(screenPos, Color.OrangeRed.ToVector3() * 1.0f * Main.essScale);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }

    internal class CinderCrawlerBody : WormBody
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerBody);
            //If your worm is not connected, messed with this NPC.width number
            //It is based on that, lower will make them closer, higher will make them farther
            NPC.width = 22;
            NPC.height = 70;
            NPC.aiStyle = -1;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void Init()
        {
            CinderCrawlerHead.CommonWormInit(this);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                Color.OrangeRed.ToVector3(),
                Color.Red.ToVector3(),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(NPC, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, Color.OrangeRed, Color.White, 0);
            Lighting.AddLight(screenPos, Color.OrangeRed.ToVector3() * 1.0f * Main.essScale);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }

    internal class CinderCrawlerTail : WormTail
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }


        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerTail);
            //If your worm is not connected, messed with this NPC.width number
            //It is based on that, lower will make them closer, higher will make them farther
            NPC.width = 22;
            NPC.height = 70;
            NPC.aiStyle = -1;
        }

        public override void Init()
        {
            CinderCrawlerHead.CommonWormInit(this);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                Color.OrangeRed.ToVector3(),
                Color.Red.ToVector3(),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(NPC, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, Color.OrangeRed, Color.White, 0);
            Lighting.AddLight(screenPos, Color.OrangeRed.ToVector3() * 1.0f * Main.essScale);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}