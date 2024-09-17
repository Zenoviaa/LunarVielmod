using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles.Chains
{
    internal class SupernovaChainCircle : ModProjectile
    {
        public const float Circle_Radius = 900;
        public const float Outer_Circle_Radius = 2700;
        public const float Beam_Width = 8;

        public Vector2[] CirclePos;
        public int FrameCounter;
        public int FrameTick;
        ref float NPCToFollow => ref Projectile.ai[0];
        internal PrimitiveTrail BeamDrawer;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.scale = 0.001f;
            Projectile.hide = true;
            //Points on the circle
            CirclePos = new Vector2[128];
        }

        public override void AI()
        {
            DrawHelper.DrawCircle(Projectile.Center, Circle_Radius, CirclePos);
            SnapToNPC();
            PushAway();
        }

        private void PushAway()
        {
            Rectangle myRect = Projectile.getRect();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active)
                    continue;
                Rectangle playerRect = player.getRect();
                float distanceToPlayer = Vector2.Distance(Projectile.Center, player.Center);
                if (Projectile.Colliding(myRect, playerRect) && Projectile.active)
                {
                    Vector2 direction = Projectile.DirectionTo(player.Center);
                    direction = -direction;
                    player.velocity += direction * 8;
                    player.wingTime = player.wingTimeMax;
                } 
                else if(distanceToPlayer > Circle_Radius + 64 && distanceToPlayer < Outer_Circle_Radius)
                {
                    Vector2 direction = Projectile.DirectionTo(player.Center);
                    Vector2 teleportPosition = Projectile.Center + direction * Circle_Radius;
                    player.Teleport(teleportPosition, 6);
                    NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, teleportPosition.X, teleportPosition.Y, 1);
                }
            }
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
                    Projectile.alpha += 10;
                    Projectile.scale += 0.01f;
                    if (Projectile.scale >= 1f)
                        Projectile.scale = 1f;
                    if (Projectile.alpha >= 255)
                        Projectile.alpha = 255;
                    Projectile.Center = npc.Center;
                    Projectile.timeLeft = 3600;
                }
                else
                {
                    NPCToFollow = -1;
                }
            }
            else
            {
                Projectile.alpha -= 10;
                Projectile.scale -= 0.01f;
                if (Projectile.alpha <= 0)
                {
                    Projectile.Kill();
                }
            }
        }

        public float WidthFunction(float completionRatio)
        {
            return Projectile.scale * Beam_Width;
        }


        public Color ColorFunction(float completionRatio)
        {
            return Color.OrangeRed;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = CirclePos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.Red);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);

            BeamDrawer.DrawPixelated(CirclePos, -Main.screenPosition, CirclePos.Length);
            Main.spriteBatch.ExitShaderRegion();
            
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            int frameCount = 15;
            int frameTime = 2;
            Rectangle animationFrame = chainTexture.AnimationFrame(
                ref FrameCounter, ref FrameTick, frameTime, frameCount, true);
            DrawHelper.DrawSupernovaChains(chainTexture, CirclePos, animationFrame, Projectile.alpha / 255f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
    }
}
