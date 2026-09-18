using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Effects;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Trailing;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.NPCsWD
{
    public class HypnotizedSoulModPlayer : ModPlayer
    {
        public Vector2? targetSuckPosition;
        public Vector2? resetVelocity;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (targetSuckPosition.HasValue)
            {
                Vector2 suckPosition = targetSuckPosition.Value;
                Vector2 velocityToPosition = (suckPosition - Player.Center);
                Player.velocity = Vector2.Lerp(Player.velocity, velocityToPosition, 0.5f);
                targetSuckPosition = null;
            }
            if (resetVelocity.HasValue)
            {
                Player.velocity = resetVelocity.Value;
                resetVelocity = null;
            }
        }
    }
    public class HypnotizedSoulEnemy : ModNPC
    {
        private int _frame;
        private ITrailer _trailer;
        private ref float Timer => ref NPC.ai[0];
        private enum AIState
        {
            Idle,
            Wander,
            Chase,
            Suck
        }

        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        private ref float WanderAngle => ref NPC.ai[2];
        private ref float TargetWanderAngle => ref NPC.ai[3];

        private Vector2 _scale;
        private Vector2 _suckOffset;
        private Color OutlineColor;
        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                TargetWanderAngle = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_suckOffset);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _suckOffset = reader.ReadVector2();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 12;
            NPCID.Sets.TrailCacheLength[Type] = 64;
            NPCID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _scale = Vector2.One;
            NPC.lifeMax = 100;
            NPC.width = 32;
            NPC.height = 32;
            NPC.defense = 0;
            NPC.damage = 20;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.15f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            switch (State)
            {
                default:
                case AIState.Idle:
                    if (_frame >= 4f)
                    {
                        _frame = 0;
                    }
                    break;
                case AIState.Wander:
                    if (_frame >= 4f)
                    {
                        _frame = 0;
                    }
                    break;
                case AIState.Chase:
                    if (_frame < 4)
                    {
                        _frame = 4;
                    }

                    if (_frame >= 8)
                    {
                        _frame = 4;
                    }
                    break;
                case AIState.Suck:
                    if (_frame < 8)
                    {
                        _frame = 8;
                    }
                    if (_frame >= 12f)
                    {
                        _frame = 8;
                    }
                    break;

            }

            NPC.frame.Y = frameHeight * _frame;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return State == AIState.Suck;
        }

        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Wander:
                    AI_Wander();
                    break;
                case AIState.Chase:
                    AI_Chase();
                    break;
                case AIState.Suck:
                    AI_Suck();
                    break;
            }
        }
        private void AI_Idle()
        {
            Timer++;
            if (Timer >= 60)
            {
                SwitchState(AIState.Wander);
            }
            NPC.velocity *= 0.98f;
            OutlineColor = Color.Lerp(OutlineColor, Color.Transparent, 0.1f);
        }

        private void AI_Wander()
        {
            Timer++;
            if (MultiplayerHelper.IsHost && Timer % 200 == 0)
            {
                TargetWanderAngle = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
                NPC.netUpdate = true;
            }

            float rotAccel = 0.005f;
            if (WanderAngle < TargetWanderAngle)
            {
                WanderAngle += rotAccel;
            }
            else if (WanderAngle > TargetWanderAngle)
            {
                WanderAngle -= rotAccel;
            }

            Vector2 targetVelocity = WanderAngle.ToRotationVector2();
            Vector2 wanderVelocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
            NPC.velocity = wanderVelocity;
            NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
            NPC.rotation = NPC.velocity.X * 0.02f;
            NPC.spriteDirection = -NPC.direction;
            NPC.noTileCollide = true;
            NPC.TargetClosest();
            if (NPC.HasValidTarget && Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) < 180)
            {
                SwitchState(AIState.Chase);
            }
            OutlineColor = Color.Lerp(OutlineColor, Color.Transparent, 0.1f);
        }
        private void AI_Chase()
        {
            Timer++;
            OutlineColor = Color.Lerp(OutlineColor, Color.Yellow, 0.1f);
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowDust>(), Scale: Main.rand.NextFloat(0.2f, 0.4f), newColor: Color.LightCyan);
            }
            Player target = Main.player[NPC.target];
            Vector2 velocityToTarget = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            float distanceToTarget = Vector2.Distance(NPC.Center, target.Center);
            float maxSpeed = MathHelper.Clamp(distanceToTarget, 3, 6);
            if (Timer < 60)
            {
                maxSpeed *= 0.2f;
            }
            velocityToTarget *= maxSpeed;
            NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToTarget, 0.1f);
            NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
            NPC.spriteDirection = -NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.01f;
            NPC.noTileCollide = true;
            if (!NPC.HasValidTarget)
            {
                SwitchState(AIState.Idle);
            }
            else if (distanceToTarget <= 4)
            {
                _suckOffset = target.Center - NPC.Center;
                SwitchState(AIState.Suck);
            }
        }

        private void AI_Suck()
        {
            Timer++;
            OutlineColor = Color.Lerp(OutlineColor, Color.Red, 0.1f);
            NPC.noTileCollide = false;

            
            if (!NPC.HasValidTarget)
            {
                SwitchState(AIState.Idle);
            }
            else
            {
                Player target = Main.player[NPC.target];
                HypnotizedSoulModPlayer hypnotizedSoulModPlayer = target.GetModPlayer<HypnotizedSoulModPlayer>();
                hypnotizedSoulModPlayer.targetSuckPosition = NPC.Center + Vector2.UnitY * 32;

                if(Timer % 8 == 0)
                {
                    Vector2 spawnPoint = hypnotizedSoulModPlayer.targetSuckPosition.Value + Main.rand.NextVector2CircularEdge(128, 128);
                    Vector2 velocity = (hypnotizedSoulModPlayer.targetSuckPosition.Value - spawnPoint) * 0.1f;
                    var p = FXUtil.GlowStretch(spawnPoint, velocity);
    
                    p.VectorScale *= 0.5f;
                }


                if(Timer >= 200)
                {
                //    NPC.velocity *= 0.5f;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.UnitY * MathF.Sin(Timer * 0.1f), 0.1f);
                }
                else
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, -Vector2.UnitY * 1f + Vector2.UnitY * MathF.Sin(Timer * 0.1f), 0.1f);
                }

                    //  NPC.Center = target.Center + _suckOffset;
                    NPC.spriteDirection = -NPC.direction;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (State == AIState.Chase)
            {
                _trailer ??= TrailPresets.HypnotizedSoul;
                _trailer.DrawTrail(ref drawColor, NPC.oldPos);
            }

            if (State == AIState.Suck)
            {
                Player target = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    List<Vector2> suckPositions = new List<Vector2>();
                    float num = 16f;
                    for (float f = 0; f < num; f++)
                    {
                        suckPositions.Add(Vector2.Lerp(NPC.Center, target.Center, f / num));
                    }
                    Vector2[] suckPos = suckPositions.ToArray();
                    _trailer.DrawTrail(ref drawColor, suckPos);
                }

            }

            this.DrawOutline(OutlineColor, yOffset: 0, _scale);
            Vector2 drawOrigin = NPC.frame.Size() / 2;

            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawPos = NPC.position - screenPos + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture, drawPos, NPC.frame, Color.White.MultiplyRGB(drawColor), NPC.rotation, drawOrigin, _scale, spriteEffects, 0);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(spriteBatch, screenPos, drawColor);

            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color(255, 128, 125, 0), NPC.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 1.0f * Main.essScale);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HypnotizedSoul>(), minimumDropped: 2, maximumDropped: 4));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Player.GetModPlayer<MyPlayer>().ZoneWonder)
                return 0;
            return ScarletSpawnChance.Wondrous_Spawn_Rate;
        }

        public override void OnKill()
        {
            base.OnKill();
            float boomSize = Main.rand.NextFloat(0.025f, 0.08f);
            FXUtil.GlowCircleBoom(NPC.Center,
                innerColor: Color.White,
                glowColor: Color.LightBlue,
                outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);


            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleLongBoom(NPC.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.DarkBlue,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
    }
}
