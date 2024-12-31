using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Chains
{
    internal class SupernovaChainFront : ModProjectile
    {
        public Vector2[] ChainPos;
        public int FrameCounter;
        public int FrameTick;
        ref float NPCToFollow => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
            ChainPos = new Vector2[8];
        }

        public override void AI()
        {
            //Fade In
            Projectile.rotation = Projectile.velocity.ToRotation();

            MakeOval();
            SnapToNPC();
        }

        private void MakeOval()
        {
            //Calculate Points On Oval
            Vector2 chainCenter = Projectile.Center;
            float ovalXRadius = 128;
            float ovalYRadius = 48;
            float ovalAngle = MathHelper.Pi;
            float ovalRotation = MathHelper.PiOver4 / 2;
            float ovalRotationOsc = VectorHelper.Osc(0f, 1f, 1f) * (MathHelper.PiOver4 / 4);
            DrawHelper.DrawChainOval(chainCenter, ovalXRadius, ovalYRadius, ovalAngle, 
                ovalRotation + ovalRotationOsc + Projectile.rotation,
                ref ChainPos);
        }


        private void SnapToNPC()
        {
            //Snap to NPC to follow
            int npcIndex = (int)NPCToFollow;
            if (npcIndex != -1)
            {
                NPC npc = Main.npc[npcIndex];
                if (npc.active && npc.HasBuff<SupernovaChained>())
                {
                    //Fade In
                    Projectile.alpha += 2;
                    if (Projectile.alpha >= 255)
                        Projectile.alpha = 255;
                    Projectile.Center = npc.Center + new Vector2(0, -28);
                    Projectile.timeLeft = 3600;
                }
                else
                {
                    NPCToFollow = -1;
                }
            }
            else
            {
                Projectile.alpha -= 2;
                if (Projectile.alpha <= 0)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            int frameCount = 15;
            int frameTime = 2;
            Rectangle animationFrame = chainTexture.AnimationFrame(
                ref FrameCounter, ref FrameTick, frameTime, frameCount, true);
            DrawHelper.DrawSupernovaChains(chainTexture, ChainPos, animationFrame, (float)Projectile.alpha / 255f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
