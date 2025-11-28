using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime
{
    public struct PunkerPrimeDraw
    {
        public Color outlineColor;
        public Vector2 scale;
        public Vector2 shakeOffset;
        public float afterImageStrength;
     
        public void SetDefaults()
        {
            outlineColor = Color.Transparent;
            afterImageStrength = 0f;
        }
    }
    public class PunkerPrime : ScarletBoss,
        IDrawOutlines
    {
        private Vector2 _teleportPosition;
        private enum AIState
        {
            Spawn,
            Despawn,
            Idle,
            Death,

            RePosition
        }

        private PunkerPrimeDraw _draw;
        private Vector2 _startCenter;
        private Vector2 _hoverCenter;
        private Color TargetOutlineColor;
        private ref float Timer => ref NPC.ai[0];
        
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_teleportPosition);
            writer.WriteVector2(_hoverCenter);
            writer.WriteVector2(_startCenter);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _teleportPosition = reader.ReadVector2();
            _hoverCenter = reader.ReadVector2();
            _startCenter = reader.ReadVector2();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 128;
            NPC.height = 128;
            NPC.damage = 100;
            NPC.defense = 14;
            NPC.lifeMax = 6000;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/PunkerPrime");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override void AI()
        {
            base.AI();
            _draw.outlineColor = Color.Lerp(_draw.outlineColor, TargetOutlineColor, 0.1f);
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget && State != AIState.Despawn)
                {
                    SwitchState(AIState.Despawn);
                }
            }

            if(_teleportPosition != Vector2.Zero)
            {
                NPC.position = _teleportPosition;
                _teleportPosition = Vector2.Zero;
            }

            switch (State)
            {
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Despawn:
                    AI_Despawn();
                    break;
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;
                case AIState.RePosition:
                    AI_RePosition();
                    break;
            }
        }

        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void TeleportTo(Vector2 teleportCenter)
        {
            if (MultiplayerHelper.IsHost)
            {
                NPC.Center = teleportCenter;
                _teleportPosition = NPC.position;
                NPC.netUpdate = true;
            }
        }

        private void AI_Spawn()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();

                //Start from above the player and come down
                Vector2 teleportCenter = MyTarget.Center + new Vector2(0, -500);
                TeleportTo(teleportCenter);

                SoundStyle mechTurnSound = AssetRegistry.Sounds.SteamPunking.MechTurn;
                mechTurnSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(mechTurnSound, NPC.position);
            }

            TargetOutlineColor = Color.Transparent;
            RetargetCameraModifier.ReTargetPosition = NPC.Center;

            float time = 120f;
            float completionRatio = Timer / time;
            float ease = EasingFunction.InOutSine(completionRatio);
            float yVelocity = MathHelper.Lerp(3f, 0f, ease);
            NPC.velocity.Y = yVelocity;
            NPC.velocity.X = 0;
            NPC.rotation = 0;
            if(Timer >= time)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Despawn()
        {
            TargetOutlineColor = Color.Transparent;
            //Just fly up and despawn, very simepl
            Timer++;
            float despawnTime = 90;
            NPC.velocity.Y -= 0.5f;
            NPC.velocity.X *= 0.5f;
            NPC.rotation = 0;
            if (Timer >= despawnTime)
            {
                NPC.active = false;
            }
        }

        private bool CanAttack()
        {
            //This is going to check all of the arms and check how many of them are moving
            return true;
        }

        private void AI_Idle()
        {
            _draw.afterImageStrength *= 0.5f;

            //Steampunker prime is just going to hover around and above you most of the time for the most part
            //If you get far from him he'll track you, but otherwise he's mostly stationary and doesn't move too much
            //Which should be easy to deal with?
            //The extension from melee should make it easy to hit the cores
            //Hopefully;
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
            }
            float idleTime = 120f;
            float distanceToTarget = Vector2.Distance(NPC.Center, MyTarget.Center);
            if (distanceToTarget > 800)
            {
                SwitchState(AIState.RePosition);
            }

            _draw.afterImageStrength = MathHelper.Lerp(_draw.afterImageStrength, 0f, 0.1f);

            TargetOutlineColor = Color.Transparent;
            Vector2 hoverVelocity = Vector2.Zero;
            hoverVelocity.Y = MathF.Sin(Timer * 0.25f) * 0.5f + 0.5f;
            hoverVelocity.X = FacingDirectionToTarget;
            NPC.velocity = Vector2.Lerp(NPC.velocity, hoverVelocity, 0.1f);
            NPC.rotation = NPC.velocity.X * 0.02f;
            if(Timer >= idleTime)
            {
                ChooseAttack();
            }
        }
        private void AI_RePosition()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                _startCenter = NPC.Center;
                _hoverCenter = MyTarget.Center + new Vector2(0, -300);

                SoundStyle mechMove = AssetRegistry.Sounds.SteamPunking.MechMove;
                mechMove.PitchVariance = 0.2f;
                SoundEngine.PlaySound(mechMove, NPC.position);
            }


            TargetOutlineColor = Color.Transparent;
            _draw.afterImageStrength = MathHelper.Lerp(_draw.afterImageStrength, 1f, 0.1f);
            float repositionTime = 60f;
            float completionRatio = Timer / repositionTime;
            float ease = EasingFunction.Anticipation2(completionRatio);
            Vector2 positionToMoveTo = Vector2.Lerp(_startCenter, _hoverCenter, ease);
            Vector2 velocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = velocity;
            NPC.rotation = NPC.velocity.X * 0.025f;
            if(Timer >= repositionTime)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void ChooseAttack()
        {
            if (!CanAttack())
                return;

        }

        private void AI_Death()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
            }

            float deathTime = 300f;
            if(Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }

            if(Timer % 15 == 0)
            {
                _draw.shakeOffset = Main.rand.NextVector2Circular(16, 16);
                NPC.rotation = _draw.shakeOffset.X * 0.05f;
            }

            if(Timer % 12 == 0)
            {
                Vector2 spawnPoint = NPC.Top;
                spawnPoint.X += Main.rand.NextFloat(-64f, 64f);
                var fireDust = Dust.NewDustPerfect(spawnPoint, DustID.FireworkFountain_Red, Scale: Main.rand.NextFloat(0.5f, 1f));
                fireDust.noGravity = false;
            }

            NPC.velocity = Vector2.Zero;
            _draw.afterImageStrength = MathHelper.Lerp(_draw.afterImageStrength, 0f, 0.1f);
            _draw.outlineColor = Color.Lerp(Color.Transparent, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 12f));
            if(Timer >= deathTime)
            {
                for (int i = 0; i < 16; i++)
                {
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<TSmokeDust>(),
                        (Vector2.One * Main.rand.Next(5, 15)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }
                for (float f = 0; f < 12; f++)
                {
                    Vector2 v = Main.rand.NextVector2Circular(128, 128);
                    FXUtil.GlowStretch(NPC.Center, v);
                }

                float numDust = 32;
                for(float n = 0; n < numDust; n++)
                {
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(32, 32);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), dustVelocity,
                        newColor: Color.Red,
                        Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
                SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/GlocketRouncher");
                SoundEngine.PlaySound(explosionSound, NPC.position);
                FXUtil.ShakeCamera(NPC.position, 1024, 8);
                ShakeModSystem.Shake = 16;
                var p = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Red, Color.Black);
                NPC.Kill();
            }
        }

        public override void OnKill()
        {
            base.OnKill();
            DownedBossTracker.ClearFlag(DownedBossFlag.PunkerPrime);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            if (NPC.life <= 0 && State != AIState.Death)
            {
                NPC.life = 1;
                SwitchState(AIState.Death);
            }

            if (NPC.life <= 0)
            {
                NPC.life = 1;
            }
        }
        private void SpawnSteamParticle()
        {
            Vector2 spawnPosition = NPC.Top;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Vector2.Zero;
            spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            var steamParticle = Particle.NewParticle<BlackSmokeParticle>(spawnPosition, spawnVelocity, Scale: spawnScale);
            steamParticle.innerColor = Color.DarkGray;
            steamParticle.outerColor = Color.Black;
            steamParticle.fadeToColor = Color.Black;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawBodyAfterImage(spriteBatch, screenPos);
            DrawBodySprite(spriteBatch, screenPos, drawColor);
            return false;
        }

        private void DrawBodyAfterImage(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            float length = NPCID.Sets.TrailCacheLength[Type];
            for (int i = 0; i < length; i++)
            {
                float f = i;
                float completionRatio = f / length;
                Vector2 oldPosition = NPC.oldPos[i];
                Vector2 oldCenter = oldPosition + NPC.Size / 2f;
                Color color = Color.White;
                color *= 0.4f;
                color *= _draw.afterImageStrength;
                color *= MathHelper.SmoothStep(1f, 0f, completionRatio);
                spriteBatch.Draw(texture, oldCenter, frame, color, NPC.rotation, drawOrigin, _draw.scale, SpriteEffects.None, 0);
            }
        }

        private void DrawBodySprite(SpriteBatch spriteBatch, Vector2 screenPos, Color color)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 drawCenter = NPC.Center - screenPos;
            Vector2 drawOrigin = frame.Size() / 2f;
            drawCenter += _draw.shakeOffset;
            spriteBatch.Draw(texture, drawCenter, frame, color, NPC.rotation, drawOrigin, _draw.scale, SpriteEffects.None, 0);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2f;
            if (_draw.outlineColor == Color.Transparent)
                return;
            Vector2 h = Vector2.UnitX * outlineOffset;
            Vector2 v = Vector2.UnitY * outlineOffset;
            DrawBodySprite(spriteBatch, screenPos + h, _draw.outlineColor);
            DrawBodySprite(spriteBatch, screenPos - h, _draw.outlineColor);
            DrawBodySprite(spriteBatch, screenPos + v, _draw.outlineColor);
            DrawBodySprite(spriteBatch, screenPos - v, _draw.outlineColor);
        }
    }
}
