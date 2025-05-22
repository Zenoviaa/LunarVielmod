using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.IrradiaNHavoc.Projectiles
{
    internal class IrradiaElectricBoxNode : ModNPC
    {
        private float Timer
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        private float Connected
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        private float LifeTimer
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[Type] = 32;
            NPCID.Sets.TrailingMode[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 72;
            NPC.height = 68;
            NPC.lifeMax = 1000;
            NPC.damage = 30;
            NPC.defense = 20;
            NPC.timeLeft = 720;
            NPC.noGravity = true;
        }

        public override void AI()
        {
            //Oscillate movement
            Timer++;
            if(Timer < 60)
            {
                float ySpeed = Timer / 60;
                ySpeed = Easing.SpikeInOutCirc(ySpeed);
                NPC.velocity = new Vector2(0, -ySpeed);
            } 
            else if (Timer < 120)
            {
                //Inverse
                float ySpeed = 1f - ((Timer - 60) / 60);
                ySpeed = Easing.SpikeInOutCirc(ySpeed);
                NPC.velocity = new Vector2(0, ySpeed);
            }

            if(Timer == 120)
            {
                Timer = 0;
            }

            LifeTimer++;
            if(LifeTimer>= 480)
            {
                NPC.Kill();
            }

            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.78f * Main.essScale);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            int projFrames = Main.npcFrameCount[NPC.type];
            int frameHeight = texture.Height / projFrames;
            int startY = frameHeight * NPC.frame.Y;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            //drawOrigin.X = projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - Main.screenPosition + drawOrigin;// + new Vector2(0f, projectile.gfxOffY);
                Color color = NPC.GetAlpha(Color.Lerp(Color.Red, Color.Transparent, 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, NPC.oldRot[k], drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
