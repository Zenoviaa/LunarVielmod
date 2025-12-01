using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.Collosseum.BossesCL.EliteCommander.Projectiles;
using Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria
{
    public struct TowerOfIlluraDraw
    {
        public Color outlineColor;
        public Vector2 towerDrawCenter;
        public float afterImageAlpha;
        public void SetDefaults()
        {
            outlineColor = Color.Transparent;
            afterImageAlpha = 1f;
        }
    }
    public class CrumblingTowerOfIlluria : ScarletBoss,
        IDrawOutlines
    {
        private bool _inPhase2;
        private bool _showNamePlate;
        private bool _setTowerPosition;
        private bool _summonedHearts;
        private TowerOfIlluraDraw _draw;
        private enum AIState
        {
            Spawn,
            Idle,
            Despawn,
            Death,

            LaserBolt,
            PhaseTransition,

            BouncingIdle,
            WhiteWhips,
            DiscoHead,

            SpawnIdle
        }


        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        private ref float BounceTimer => ref NPC.ai[2];
        private ref float AttackCounter => ref NPC.ai[3];
        private PatternManager<AIState> _p2PatternBackingField;
        private PatternManager<AIState> P2PatternManager
        {
            get
            {
                if(_p2PatternBackingField == null)
                {
                    _p2PatternBackingField = new PatternManager<AIState>(
                        new Tuple<AIState, float>(AIState.WhiteWhips, 1.0f),
                        new Tuple<AIState, float>(AIState.DiscoHead, 1.0f));
                }
                return _p2PatternBackingField;
            }
        }

        private bool AllHeartsDead => !NPC.AnyNPCs(ModContent.NPCType<TowerHeart>());
        private Color TargetOutlineColor;
        private int IllurianSnipeDamage => 28;
        private int ShockwaveDamage => 20;
        private int WhiteWhipDamage => 15;

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_setTowerPosition);
            writer.Write(_inPhase2);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _setTowerPosition = reader.ReadBoolean();
            _inPhase2 = reader.ReadBoolean();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _draw.SetDefaults();
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 100;
            NPC.defense = 33;
            NPC.lifeMax = 18000;
            NPC.scale = 1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SanguineSingularity");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitByProjectile(projectile, ref modifiers);
            if (_inPhase2)
                return;

            //Here we want to set the damage of the projectile to NOTHING if it's not the IllurianSoul
            modifiers.FinalDamage *= 0;
        }

        public override void AI()
        {
            base.AI();
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget && State != AIState.Despawn)
                {
                    SwitchState(AIState.Despawn);
                }
            }
            _draw.outlineColor = Color.Lerp(_draw.outlineColor, TargetOutlineColor, 0.1f);
            SummonHearts();
            //Check for all hearts dying to do the phase transition
            if (MultiplayerHelper.IsHost && !_inPhase2 && AllHeartsDead)
            {
                SwitchState(AIState.PhaseTransition);
                _inPhase2 = true;
            }

            ManageTowerPosition();
            switch (State)
            {
                case AIState.SpawnIdle:
                    AI_SpawnIdle();
                    break;
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Despawn:
                    AI_Despawn();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;
                case AIState.LaserBolt:
                    AI_LaserBolt();
                    break;
                case AIState.PhaseTransition:
                    AI_PhaseTransition();
                    break;
                case AIState.BouncingIdle:
                    AI_BouncingIdle();
                    break;
                case AIState.WhiteWhips:
                    AI_WhiteWhips();
                    break;
                case AIState.DiscoHead:
                    AI_DiscoHead();
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

        #region Tower of Illuria
        private void SummonHearts()
        {
            if (!MultiplayerHelper.IsHost)
                return;

            if (_summonedHearts)
                return;

            int numHearts = 8;
            int heartType = ModContent.NPCType<TowerHeart>();
            for(int n = 0; n < numHearts; n++)
            {
                int xRadius = Main.rand.Next(64, 256);
                int yRadius = Main.rand.Next(64, 256);
                int x = (int)NPC.Center.X;
                int y = (int)NPC.Center.Y;
                NPC.NewNPC(SourceFromThis, x, y, heartType, ai2: xRadius, ai3: yRadius);
            }

            _summonedHearts = true;

        }
        private void ManageTowerPosition()
        {
            if (!MultiplayerHelper.IsHost)
                return;

            if (!_setTowerPosition)
            {
                Vector2 ground = FindGround();
                _draw.towerDrawCenter = ground;
                NPC.Center = ground - new Vector2(0, 250);
                NPC.netUpdate = true;
                _setTowerPosition = true;
            }
        }

        private Vector2 FindGround()
        {
            Vector2 groundPoint = CollisionHelper.RayCast(NPC.Top, Vector2.UnitY, 2000, 3);
            return groundPoint;
        }

        #endregion
        private void AI_PhaseTransition()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                NPC.velocity.Y = -5;
            }

            TargetOutlineColor = Color.Yellow;
            RetargetCameraModifier.ReTargetPosition = NPC.Center;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.velocity.X = 0;
            NPC.rotation = 0;
            if (NPC.collideY)
            {
                NPC.velocity.Y = -2;
                Particle.NewParticle<GlowDonutParticle>(NPC.Bottom, -Vector2.UnitY, Color.White, Scale: 0.2f);
            }
            if(Timer >= 100)
            {
                SwitchState(AIState.BouncingIdle);
            }
        }

        private void CreateShockwaveParticles()
        {
            SoundStyle boom = SoundID.DD2_ExplosiveTrapExplode;
            boom.PitchVariance = 0.3f;
            SoundEngine.PlaySound(boom, NPC.position);
            for (int i = 0; i < 16; i++)
            {
                float radius = 150;
                Vector2 offset = Vector2.UnitX * Main.rand.Next(-1, 1);
                offset *= Main.rand.NextFloat(1f, radius);
                offset += new Vector2(radius / 2, 0);

                Vector2 velocity = Vector2.UnitX * Main.rand.Next(-1, 1);
                velocity *= Main.rand.NextFloat(1f, 2f);
                Dust.NewDustPerfect(NPC.Bottom + offset, ModContent.DustType<Dusts.TSmokeDust>(), velocity, 0, Color.Black * 0.5f,
                    Main.rand.NextFloat(0.3f, 0.7f));
            }

            FXUtil.GlowCircleBoom(NPC.Bottom,
               innerColor: Color.White,
               glowColor: Color.Black,
               outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(240);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Bottom,
                    innerColor: Color.White,
                    glowColor: Color.Black,
                    outerGlowColor: Color.Black,
                    baseSize: 0.24f);
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }

            for (int i = 0; i < 7; i++)
            {
                Vector2 velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(15f, 35f);
                var particle = FXUtil.GlowStretch(NPC.Bottom, velocity);
                particle.InnerColor = Color.White;
                particle.GlowColor = Color.LightCyan;
                particle.OuterGlowColor = Color.Black;
                particle.Duration = Main.rand.NextFloat(25, 50);
                particle.BaseSize = Main.rand.NextFloat(0.045f, 0.09f);
                particle.VectorScale *= 0.5f;
            }
        }
        private void AI_BouncingIdle()
        {
            BounceTimer++;
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
            }

            TargetOutlineColor = Color.Transparent;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.velocity.X = MathF.Sin(BounceTimer * 0.05f) * 0.5f;
            NPC.rotation += NPC.velocity.X * 0.03f;
            if (NPC.collideY)
            {
                NPC.velocity.Y = -5;
                Particle.NewParticle<GlowDonutParticle>(NPC.Bottom, -Vector2.UnitY, Color.White, Scale: 0.2f);
                CreateShockwaveParticles();
                if (MultiplayerHelper.IsHost)
                {
                    //This is the part where you spawn the cool ahh shockwaves
                    //But we have to make cool ahh shockwaves :(
                    int knockback = 1;
                    Vector2 velocity = Vector2.UnitX;
                    velocity *= 4;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, velocity,
                        ModContent.ProjectileType<WindShockwave>(), ShockwaveDamage, knockback, Main.myPlayer);
                    velocity = -velocity;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, velocity,
                        ModContent.ProjectileType<WindShockwave>(), ShockwaveDamage, knockback, Main.myPlayer);
                }
            }

            if(Timer % 10 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 
                    ModContent.DustType<Sparkle>());
            }

            if(Timer >= 240 && NPC.velocity.Y < -2)
            {
                ChoosePhase2Attack();
            }
        }

        private void AI_WhiteWhips()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                float numDust = 8;
                for(float d = 0; d < numDust; d++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    Vector2 spawnPos = NPC.Center;
                    Dust.NewDustPerfect(spawnPos, ModContent.DustType<GlowDust>(), velocity,
                        newColor: Color.White, 
                        Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
            }

            NPC.noGravity = true;
            NPC.velocity.X = 0;
            NPC.velocity.Y = 0;
            TargetOutlineColor = Color.Yellow;

    
            float totalNumWhips = 36;
            float loops = 4;
            if(Timer % 16 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    float radians = (AttackCounter / totalNumWhips) * MathHelper.TwoPi * loops;
                    Vector2 velocity = radians.ToRotationVector2();
                    velocity *= 7;
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity, 
                        ModContent.ProjectileType<WhiteWhip>(), WhiteWhipDamage, 1, Main.myPlayer);
                    AttackCounter++;
                    if(AttackCounter >= totalNumWhips)
                    {
                        SwitchState(AIState.BouncingIdle);
                    }
                }
            }
        }


        private void AI_DiscoHead()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                float numDust = 8;
                for (float d = 0; d < numDust; d++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    Vector2 spawnPos = NPC.Center;
                    Dust.NewDustPerfect(spawnPos, ModContent.DustType<GlowDust>(), velocity,
                        newColor: Color.White,
                        Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
            }

            NPC.noGravity = true;
            NPC.velocity.X = 0;
            NPC.velocity.Y = 0;
            TargetOutlineColor = Color.Yellow;
            float totalNumLights = 36;
            if (Timer % 16 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    float radians = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
                    Vector2 velocity = radians.ToRotationVector2();
                    velocity *= 256;
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                        ModContent.ProjectileType<DiscoLight>(), WhiteWhipDamage, 1, Main.myPlayer);
                    AttackCounter++;
                    if (AttackCounter >= totalNumLights)
                    {
                        SwitchState(AIState.BouncingIdle);
                    }
                }
            }
        }

        private void AI_LaserBolt()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            TargetOutlineColor = Color.Yellow;
            NPC.velocity.Y *= 0.9f;
            NPC.velocity.X = 0;
            if (Timer == 60 && MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, NPC.Top, Vector2.UnitX * 8,
                    ModContent.ProjectileType<IllurianSnipe>(), IllurianSnipeDamage, 1, Main.myPlayer);
            }

            if (Timer >= 120)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_SpawnIdle()
        {

        }

        private void AI_Spawn()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            ShakeModSystem.Shake = 4;
            if (Timer % 7 == 0)
            {
                Particle.NewParticle<ShockParticle>(NPC.Center, Vector2.Zero, Color.White);
            }

            NPC.noGravity = false;
            NPC.noTileCollide = false;
            RetargetCameraModifier.ReTargetPosition = NPC.Center;
            if (Timer >= 100)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Idle()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }
            if (!_showNamePlate)
            {
                ShowNamePlate();
                _showNamePlate = true;
            }

            _draw.afterImageAlpha = MathHelper.Lerp(_draw.afterImageAlpha, 1f, 0.1f);
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.velocity.Y = MathF.Sin(Timer * 0.05f) * 0.5f + 0.5f;
            NPC.velocity.X = 0;
            NPC.rotation = 0;
            TargetOutlineColor = Color.Transparent;
            if (Timer >= 100)
            {
                if (!_inPhase2)
                {
                    ChoosePhase1Attack();
                }
            }
        }

        private void ChoosePhase1Attack()
        {
            SwitchState(AIState.LaserBolt);
        }

        private void ChoosePhase2Attack()
        {
            if (!MultiplayerHelper.IsHost)
                return;
            SwitchState(P2PatternManager.NextPattern());
        }

        private void AI_Despawn()
        {
            Timer++;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.velocity.X *= 0.5f;
            NPC.velocity.Y += 0.5f;
            if (Timer >= 100)
            {
                NPC.active = false;
            }
        }

        private void AI_Death()
        {
            Timer++;
            if (Timer >= 200)
            {
                NPC.Kill();
            }
        }

        public override void OnKill()
        {
            base.OnKill();
            DownedBossTracker.ClearFlag(DownedBossFlag.CrumblingTowerOfIlluria);
        }


        #region Draw Code
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawAfterImages(spriteBatch, screenPos, drawColor);
            DrawBase(spriteBatch, screenPos, drawColor);
            DrawSprite(spriteBatch, NPC.Center - screenPos, drawColor);
            DrawGlow(spriteBatch, screenPos, drawColor);
            return false;
        }

        private void DrawAfterImages(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int trailLength = NPC.oldPos.Length;
            for (int i = 0; i < trailLength; i++)
            {
                float f = i;
                float numAfterImages = trailLength;
                float completionRatio = f / numAfterImages;
                Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
                afterImageColor *= 0.2f;
                afterImageColor *= _draw.afterImageAlpha;
                Vector2 drawPosition = NPC.oldPos[i] + NPC.Size / 2f;
                DrawSprite(spriteBatch, drawPosition - screenPos, afterImageColor);
            }
        }

        private void DrawBase(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D towerTexture = ModContent.Request<Texture2D>(Texture + "_Tower").Value;
            Rectangle? frame = null;
            Vector2 drawOrigin = towerTexture.Size() / 2f;
            Vector2 drawCenter = _draw.towerDrawCenter - screenPos;
            spriteBatch.Draw(towerTexture, drawCenter, frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 drawPosition, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            spriteBatch.Draw(texture, drawPosition, frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
        }

        private void DrawGlow(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float numAfterImages = 8;
            for (float n = 0; n < numAfterImages; n++)
            {
                float completionRatio = n / numAfterImages;
                float rot = MathHelper.TwoPi * completionRatio;
                Vector2 offset = rot.ToRotationVector2();
                offset *= ExtraMath.Osc(16, 24, speed: 2);
                Color glowColor = Color.White;
                glowColor.A = 0;
                glowColor *= 0.2f;
                glowColor *= ExtraMath.Osc(0.2f, 0.5f, speed: 1);
                DrawSprite(spriteBatch, NPC.Center - screenPos + offset, glowColor);
            }
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitX * outlineOffset;
            Vector2 h = Vector2.UnitY * outlineOffset;
            DrawSprite(spriteBatch, NPC.Center - screenPos + v, _draw.outlineColor);
            DrawSprite(spriteBatch, NPC.Center - screenPos - v, _draw.outlineColor);
            DrawSprite(spriteBatch, NPC.Center - screenPos + h, _draw.outlineColor);
            DrawSprite(spriteBatch, NPC.Center - screenPos - h, _draw.outlineColor);
        }
        #endregion
    }
}
