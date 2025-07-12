using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Helpers;
using Stellamod.Core.Helpers.Math;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.NPCs.Bosses.Jiitas
{
    internal class DancingPuppet : ModNPC
    {
        private int _frame;
        private ref float Timer => ref NPC.ai[0];
        private float DrawAlpha = 0;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            Main.npcFrameCount[Type] = 14;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 32;
            NPC.height = 64;
            NPC.damage = 32;
            NPC.defense = 0;
            NPC.lifeMax = 1100;
            NPC.HitSound = SoundID.NPCHit16;

            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.boss = false;


            NPC.aiStyle = -1;
        }
        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);

            //Animation Speed
            NPC.frameCounter += 0.2f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }
            if(_frame >= 14)
            {
                _frame = 0;
            }
            NPC.frame.Y = frameHeight * _frame;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> stringTextureAsset = ModContent.Request<Texture2D>(Texture + "_String");
            Vector2[] stringDrawPoints = new Vector2[]
            {
                NPC.Center + new Vector2(32, -10),
                NPC.Center + new Vector2(-32, -10),
                NPC.Center + new Vector2(-16, 8),
                NPC.Center + new Vector2(16, 8),
                NPC.Center + new Vector2(0, -8)
            };

            //Draw Strings
            //I LOVE STRINGS
            for (int i = 0; i < stringDrawPoints.Length; i++)
            {
                Vector2 drawPoint = stringDrawPoints[i];
                drawPoint -= screenPos;
                drawPoint.Y -= ExtraMath.Osc(0f, 1f, speed: 1, offset: (float)i * 4);
                Vector2 drawOrigin = new Vector2(0, stringTextureAsset.Height());
                float drawRotation = MathHelper.Lerp(-0.05f, 0.05f, ExtraMath.Osc(0f, 1f, speed: 1, offset: (float)i * 4));
                float drawAlpha = ExtraMath.Osc(0f, 1f, speed: 1, offset: (float)i * 4);
                spriteBatch.Draw(stringTextureAsset.Value, drawPoint, null, drawColor * drawAlpha * DrawAlpha, drawRotation, drawOrigin, 1, SpriteEffects.None, 0);
            }

            DrawCharacter(spriteBatch, screenPos, drawColor);
            return false;
        }

        private void DrawCharacter(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 mainDrawOrigin = frame.Size() / 2f;
            SpriteEffects effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPosition = NPC.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPosition, frame, drawColor * DrawAlpha, NPC.rotation, mainDrawOrigin, NPC.scale, effects, 0);

        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer <= 10)
            {
                DrawAlpha = MathHelper.Lerp(0, 1, Timer / 10);
            }
            else if (Timer >= 120)
            {
                DrawAlpha = MathHelper.Lerp(1, 0, (Timer-120) / 10);
            }

            NPC.velocity.Y = MathHelper.Lerp(2, -2, EasingFunction.QuadraticBump(Timer/120));
            if(Timer >= 130)
            {
                NPC.active = false;
            }
        }
    }
}
