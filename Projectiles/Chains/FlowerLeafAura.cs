using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Armors.Flower;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Chains
{
    internal class FlowerLeafAura : ModProjectile
    {
        public Vector2[] ChainPos;
        public int FrameCounter;
        public int FrameTick;

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
            ChainPos = new Vector2[16];
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (owner == player)
                    continue;

                float distance = Vector2.Distance(owner.Center, player.Center);
                if (distance <= 64)
                {
                    player.AddBuff(ModContent.BuffType<FlowerPower>(), 60);
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            MakeOval();
            SnapToPlayer();
        }

        private void MakeOval()
        {
            //Calculate Points On Oval
            Vector2 chainCenter = Projectile.Center;
            float ovalXRadius = 64;
            float ovalYRadius = 64;

            float ovalAngle = MathHelper.TwoPi + MathHelper.PiOver4 / 2;
            DrawHelper.DrawChainOval(chainCenter, ovalXRadius, ovalYRadius, ovalAngle, 0,
                ref ChainPos);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            int frameCount = 8;
            int frameTime = 2;
            Rectangle animationFrame = chainTexture.AnimationFrame(
                ref FrameCounter, ref FrameTick, frameTime, frameCount, true);
            DrawHelper.DrawFlowerChains(chainTexture, ChainPos, animationFrame, Projectile.alpha / 255f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        private void SnapToPlayer()
        {
            //Snap to NPC to follow
            Player owner = Main.player[Projectile.owner];
            if (owner.GetModPlayer<FlowerPlayer>().hasQuiver)
            {
                //Fade In
                Projectile.alpha += 2;
                if (Projectile.alpha >= 255)
                    Projectile.alpha = 255;
                Projectile.Center = owner.Center;
                Projectile.timeLeft = 3600;
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
    }
}
