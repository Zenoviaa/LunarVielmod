using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Cinderspark
{
    internal class FlamingWitch : ModNPC
    {
        private Vector2 _dir;
        private ref float ai_Timer => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 15; // The amount of frames the NPC has
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 70;
            NPC.height = 54;
            NPC.aiStyle = -1;
            NPC.damage = 45;
            NPC.defense = 10;
            NPC.lifeMax = 158;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 1;
            NPC.lavaImmune = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Morrowsc1");
        }

        private bool CanMove()
        {
            return NPC.collideY || NPC.collideX || (NPC.velocity.Y == 0 && NPC.velocity.X == 0);
        }

        public override void FindFrame(int frameHeight)
        {
            int frame = (int)NPC.frameCounter;
            if (CanMove())
                NPC.frameCounter += 0.5f;
            else if(frame < 12 && !CanMove())
                NPC.frameCounter += 0.5f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            ai_Timer++;
            NPC.TargetClosest();
            NPC.spriteDirection = -NPC.direction;
            Player target = Main.player[NPC.target];

            if (ai_Timer > 120)
            {
                Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.InfernoFork);
                dust.velocity *= -1f;
                dust.scale *= .8f;
                dust.noGravity = true;

                Vector2 randVector = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                randVector.Normalize();
                
                Vector2 randVector2 = randVector * (Main.rand.Next(50, 100) * 0.04f);
                dust.velocity = randVector2;
                randVector2.Normalize();
                
                Vector2 vector2_3 = randVector2 * 34f;
                dust.position = NPC.Center - vector2_3;
            }

            if(ai_Timer == 240)
            {
                if (StellaMultiplayer.IsHost)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        Vector2 targetDirection = NPC.Center.DirectionTo(target.Center);
                        Vector2 velocity = targetDirection.RotatedByRandom(MathHelper.ToRadians(10)) * 15;
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
                ai_Timer = 0;
            }

            if (NPC.frameCounter == 7 && CanMove())
            {
                float ySpeed = 6;
                NPC.velocity.Y -= ySpeed;
                _dir = NPC.Center.DirectionTo(target.Center);

                if (NPC.collideX)
                {
                    _dir = -_dir;
                } 
            }

            if (NPC.frameCounter >= 7)
            {
                float xSpeed = 3.5f;  
                float xAcceleration = 1f;
                if(_dir.X < 0 && NPC.velocity.X > -xSpeed)
                {
                    NPC.velocity.X -= xAcceleration;
                } else if(_dir.X > 0 && NPC.velocity.X < xSpeed)
                {
                    NPC.velocity.X += xAcceleration;
                }
            }
            else
            {
                NPC.velocity.X = 0;
            }

            float targetRotation = NPC.velocity.X * 0.06f;
            NPC.rotation = MathHelper.WrapAngle(MathHelper.Lerp(NPC.rotation, targetRotation, 0.33f));
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 180);
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

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<CindersparkBiome>() && !spawnInfo.Player.ZoneUnderworldHeight)
            {
                return 0.6f;
            }

            //Else, the example bone merchant will not spawn if the above conditions are not met.
            return 0f;
        }

        public override void OnKill()
        {
            if (StellaMultiplayer.IsHost)
            {
                for (int i = 0; i < 1; i++)
                {
                    int radius = 8;
                    int x = (int)NPC.Center.X + Main.rand.Next(-radius, radius);
                    int y = (int)NPC.Center.Y + Main.rand.Next(-radius, radius);
                    NPC.NewNPC(NPC.GetSource_FromThis(), x, y, ModContent.NPCType<CharredSoul>());
                }
            }

            for (int i = 0; i < 16; i++)
            {
                float speedX = Main.rand.NextFloat(-1f, 1f);
                float speedY = Main.rand.NextFloat(-1f, 1f);
                float scale = Main.rand.NextFloat(0.66f, 1f);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork,
                    speedX, speedY, Scale: scale);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cinderscrap>(), chanceDenominator: 4, minimumDropped: 2, maximumDropped: 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoltenScrap>(), chanceDenominator: 2, minimumDropped: 1, maximumDropped: 3));
        }
    }
}
