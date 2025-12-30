using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Collosseum.Event.Common;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Stellamod.Content.Areas.Collosseum.Event
{

    public class GintzeWindRider : BaseColosseumNPC,
        IDrawOutlines
    {
        private Color _outlineColor;
        private bool _contactDamage;
        private bool _warn;
        private Color TargetOutlineColor;
        private Vector2 _targetPos2;
        private Vector2 _targetPos;
        public bool Dir;
        private ref float Timer => ref NPC.ai[0];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_targetPos);
            writer.WriteVector2(_targetPos2);
            writer.Write(Dir);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _targetPos = reader.ReadVector2();
            _targetPos2 = reader.ReadVector2();
            Dir = reader.ReadBoolean();
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Storm Spirit");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.lifeMax = 120;
            NPC.defense = 10;
            NPC.value = 65f;
            NPC.knockBackResist = 0.55f;
            NPC.width = 30;
            NPC.height = 44;
            NPC.damage = 36;
            NPC.scale = 1.0f;
            NPC.lavaImmune = false;
            NPC.alpha = 0;
            NPC.dontTakeDamage = false;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
        }

        private int _frame = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5f;
            if (NPC.frameCounter >= 5)
            {
                _frame++;
                NPC.frameCounter = 0;
            }
            if (_frame >= 4)
            {
                _frame = 0;
            }
            NPC.frame.Y = frameHeight * _frame;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            GintzeHitEffect(hit);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
        }

        public override void Colosseum_AI()
        {
            base.Colosseum_AI();
            Player player = Main.player[NPC.target];
            if (!NPC.HasValidTarget)
                NPC.TargetClosest();

            NPC.spriteDirection = -NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.03f;
            _targetPos = player.Center;
            Timer++;
            if (NPC.ai[0] >= 200)
            {
                NPC.ai[0] = 0;
            }
            if (_contactDamage)
                TargetOutlineColor = Color.Red;
            else if (_warn)
                TargetOutlineColor = Color.Yellow;
            else
                TargetOutlineColor = Color.Transparent;
                _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            _targetPos2 = Vector2.Lerp(_targetPos2, _targetPos, 0.02f);


            _contactDamage = false;
            _warn = false;
            if (Timer == 151)
            {
                if (NPC.position.X >= player.position.X)
                {
                    Dir = true;
                }
                else
                {
                    Dir = false;
                }

                NPC.velocity.Y *= 0.94f;
            }

            if(Timer > 60 && Timer < 150)
            {
                _warn = true;
            }
            if(Timer > 150 && Timer < 200)
            {
                _contactDamage = true;
            }
            if (Timer >= 150)
            {
                if (Dir)
                {
                    NPC.velocity.X -= 0.9f;
                }
                else
                {
                    NPC.velocity.X += 0.9f;
                }

                NPC.velocity.Y *= 0.94f;
            }
            else
            {
                if (NPC.position.X >= player.position.X)
                {
                    Movement(_targetPos, 300, 0, 0.006f);
                }
                else
                {
                    Movement(_targetPos, -300, 0, 0.006f);
                }
            }
        }


        public void Movement(Vector2 Player2, float PosX, float PosY, float Speed)
        {
            Player player = Main.player[NPC.target];
            Vector2 target = Player2 + new Vector2(PosX, PosY);
            NPC.velocity = Vector2.Lerp(NPC.velocity, VectorHelper.MovemontVelocity(NPC.Center, Vector2.Lerp(NPC.Center, target, 0.5f), NPC.Center.Distance(target) * Speed), 0.1f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = TextureAssets.Npc[Type].Value;
            //Draw after images
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                float progressOnTrail = (float)i / (float)NPC.oldPos.Length;
                Vector2 oldPos = NPC.oldPos[i];
                Vector2 drawCenter = oldPos + NPC.Size / 2f - screenPos;
                Vector2 drawOrigin = NPC.frame.Size() / 2f;
                Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, progressOnTrail);
                afterImageColor *= 0.15f;
                spriteBatch.Draw(texture, drawCenter, NPC.frame, afterImageColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
            }
            DrawSprite(spriteBatch, screenPos, drawColor);
            return false;
        }

        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            Vector2 drawCenter = NPC.Center - screenPos;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawCenter, NPC.frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            DrawExtensions.DrawOutline(DrawSprite, spriteBatch, screenPos, _outlineColor);
        }
    }
}