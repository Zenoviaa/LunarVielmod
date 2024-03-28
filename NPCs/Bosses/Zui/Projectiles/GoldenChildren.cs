using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Zui.Projectiles
{
    internal class GoldenChildren : ModProjectile, IPixelPrimitiveDrawer
    {
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 22;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            Projectile.tileCollide = false;
        }

        private ref float AI_Timer => ref Projectile.ai[0];
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldPos[i] = Projectile.position;
            }
        }

        public override void AI()
        {
            AI_Timer++;
            //This runs every other frame
            if (AI_Timer % 2 == 0)
            {
                //Randomly teleport to make the jagged effect
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                direction = direction.RotatedByRandom(MathHelper.ToRadians(70));
                float distance = Main.rand.NextFloat(30, 160);
                Projectile.Center = Projectile.Center + direction * distance;
                Projectile.netUpdate = true;
            }

            //Dunno if this is needed but whatever
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Electrifying!!!! nEMIES!!!
            target.AddBuff(BuffID.Electrified, 120);
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, target.position);

            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1, 1);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, speed, Scale: 1.5f);
                d.noGravity = true;
            }
        }
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * 8;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.DarkGoldenrod;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = Projectile.oldPos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        internal PrimitiveTrail BeamDrawer;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            Color middleColor = Color.Lerp(Color.LightGoldenrodYellow, Color.YellowGreen, 0.6f);
            Color middleColor2 = Color.Lerp(Color.Gold, Color.Goldenrod, 0.5f);
            Color finalColor = Color.Lerp(middleColor, middleColor2, AI_Timer / 600);

            TrailRegistry.LaserShader.UseColor(Color.LightCyan);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.SpikyTrail2);

            BeamDrawer.DrawPixelated(Projectile.oldPos, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
