using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class FrameStaffConnectorProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        Vector2[] ConnectorPos;
        int FrameTick;
        int FrameCounter;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = int.MaxValue;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            AI_Channel();
            AI_FillPoints();
        }

        private void AI_Channel()
        {

            //Channeling
            Player player = Main.player[Projectile.owner];

            Timer++;
            if (Timer % 58 == 0)
            {
                SoundEngine.PlaySound(SoundRegistry.LaserChannel, player.position);
            }

            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();


            if (Main.myPlayer == Projectile.owner)
            {
                if (!player.channel)
                    Projectile.Kill();
            }
        }

        private void AI_FillPoints()
        {
            //Get the points to connect
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (!Main.projectile[i].active)
                    continue;

                if (Main.projectile[i].owner != Projectile.owner)
                    continue;

                if (Main.projectile[i].type == ModContent.ProjectileType<FrameStaffNodeProj>())
                {
                    points.Add(Main.projectile[i].Center);
                }
            }

            ConnectorPos = points.ToArray();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float progress = 1f - ((float)ConnectorPos.Length / 10f);
            float multiplier = progress * 2f;
            modifiers.FinalDamage *= multiplier;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * 8;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.Yellow;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = ConnectorPos;
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
            if (ConnectorPos.Length == 0)
                return false;

            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture).Value;
            int frameCount = 8;
            int frameTime = 2;
            Rectangle animationFrame = chainTexture.AnimationFrame(
                ref FrameCounter, ref FrameTick, frameTime, frameCount, true);
            DrawHelper.DrawSupernovaChains(chainTexture, ConnectorPos, animationFrame, (float)Projectile.alpha / 255f);
            return false;
        }
    }
}
