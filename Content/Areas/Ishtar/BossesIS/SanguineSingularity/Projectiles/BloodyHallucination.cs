using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity.Projectiles
{
    public class BloodyHallucination : ModNPC
    {
        private float _alpha;
        private float _outAlpha;
        private ref float Timer => ref NPC.ai[0];
        private Player Owner
        {
            get => Main.player[(int)NPC.ai[1]];
        }

        private Player Target => Main.player[NPC.target];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 5;
            NPCID.Sets.TrailCacheLength[Type] = 15;
            NPCID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 24;
            NPC.height = 24;
            NPC.damage = 30;
            NPC.defense = 1;
            NPC.lifeMax = 1000;
            
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamageFromHostiles = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && target.whoAmI == Target.whoAmI;
        }
        private int _frame;
        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);

            //Animation Speed
            NPC.frameCounter += 0.15f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            if (_frame >= Main.npcFrameCount[Type])
                _frame = 0;
            NPC.frame.Y = frameHeight * _frame;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {


            }
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                NPC.netUpdate = true;

            }
            if (Timer % 12 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Lava, Scale: Main.rand.NextFloat(0.2f, 0.5f));
            }
            
            Player target = Target;

            float lifeTime = 180f;

            float endTime = 10;
            float inAlpha = EasingFunction.InOutSine(Timer / 30f);
            if(Timer >= lifeTime - endTime)
            {
                _outAlpha *= 0.92f;
            } else
            {
                _outAlpha = 1f;
            }
                _alpha = inAlpha * _outAlpha;
            Vector2 targetVelocity = 3.5f * (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
            if(Timer >= 180f)
            {
                NPC.active = false;
            }
        }
        private void DrawAfterImage(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            float numAfterImages = NPC.oldPos.Length;
            for(int i = 0; i < numAfterImages; i++)
            {
                float a = i;
                float completionRatio = a / numAfterImages;
                Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, MathHelper.SmoothStep(0f, 1f, completionRatio));
                afterImageColor *= 0.15f;
                afterImageColor *= _alpha;

                Vector2 drawCenter = NPC.oldPos[i] + NPC.Size / 2f - screenPos;
                SpriteEffects flip = NPC.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(texture, drawCenter, frame, afterImageColor, NPC.rotation, drawOrigin, NPC.scale, flip, 0);
            }
        }
        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            Vector2 drawCenter = NPC.Center - screenPos;
            Color finalColor = Color.White.MultiplyRGB(lightColor);
            finalColor *= _alpha;
            finalColor *= 0.5f;
            SpriteEffects flip = NPC.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawCenter, frame, finalColor, NPC.rotation, drawOrigin, NPC.scale, flip, 0);
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {            //This will make it so only the person who owns the projectile can see it
            if (Main.LocalPlayer.whoAmI != Target.whoAmI)
                return false;
            DrawAfterImage(Main.spriteBatch, Main.screenPosition, drawColor);
            DrawSprite(Main.spriteBatch, Main.screenPosition, drawColor);
            return false;
        }
    }
}
