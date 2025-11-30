using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria
{
    public struct TowerOfIlluraDraw
    {
        public Color outlineColor;
        public void SetDefaults()
        {
            outlineColor = Color.Transparent;
        }
    }
    public class CrumblingTowerOfIlluria : ScarletBoss,
        IDrawOutlines
    {
        private bool _showNamePlate;
        private TowerOfIlluraDraw _draw;
        private enum AIState
        {
            Spawn,
            Idle,
            Despawn,
            Death,

            LaserBolt,

        }
        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private Color TargetOutlineColor;
        private int IllurianSnipeDamage => 28;
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
            NPC.height = 128;
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

        private void AI_LaserBolt()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, NPC.Top, Vector2.UnitX * 8,
                    ModContent.ProjectileType<IllurianSnipe>(), IllurianSnipeDamage, 1, Main.myPlayer);
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
        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawCenter = NPC.Center - screenPos;
            spriteBatch.Draw(texture, drawCenter, frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitX * outlineOffset;
            Vector2 h = Vector2.UnitY * outlineOffset;
            DrawSprite(spriteBatch, screenPos + v, _draw.outlineColor);
            DrawSprite(spriteBatch, screenPos - v, _draw.outlineColor);
            DrawSprite(spriteBatch, screenPos + h, _draw.outlineColor);
            DrawSprite(spriteBatch, screenPos - h, _draw.outlineColor);
        }
        #endregion
    }
}
