using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
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
        private TowerOfIlluraDraw _draw;
        private enum AIState
        {
            Spawn,
            Idle,
            Despawn,
            Death,

            LaserBolt,
            PhaseTransition,
        }


        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private bool AllHeartsDead => !NPC.AnyNPCs(ModContent.NPCType<TowerHeart>());
        private Color TargetOutlineColor;
        private int IllurianSnipeDamage => 28;

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
            //Check for all hearts dying to do the phase transition
            if (!_inPhase2 && AllHeartsDead)
            {
                SwitchState(AIState.PhaseTransition);
                _inPhase2 = true;
            }

            ManageTowerPosition();
            switch (State)
            {
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
        private void ManageTowerPosition()
        {
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
