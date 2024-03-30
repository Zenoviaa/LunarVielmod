using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Particles;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons
{
    internal class ScrappyGunLaser : ModProjectile, IPixelPrimitiveDrawer
    {
        private Vector2[] _lightningArcPos = new Vector2[1];
        public const int Trail_Width = 12;

        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = int.MaxValue;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            float ai = Projectile.ai[0];
            int projectileId = (int)ai;
            Projectile proj = Main.projectile[projectileId];
            if (proj.ai[1] == 1f || proj.type != ModContent.ProjectileType<ScrappyGunProj>())
            {
                Projectile.Kill();
            }

            alphaCounter = VectorHelper.Osc(0.5f, 1.00f, 3);
            Projectile.Center = proj.Center + Projectile.velocity * 1f;// customization of the hitbox position
            //Dunno if this is needed but whatever
            Projectile.rotation = proj.rotation;

            _lightningArcPos = CalculateLightningArc();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Electrifying!!!! nEMIES!!!
            target.AddBuff(BuffID.Electrified, 120);
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.position);

            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1, 1);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, speed, Scale: 1.5f);
                d.noGravity = true;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Trail_Width;
            return MathHelper.SmoothStep(baseWidth / 2, baseWidth, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.Cyan;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        private Vector2[] CalculateLightningArc()
        {
            float teleportDistance = 192;
            Vector2 currentPosition = Projectile.position;
            List<Vector2> positions = new List<Vector2>();
            positions.Add(currentPosition);
            for (int i = 0; i < 24; i++)
            {
                Vector2 direction = Projectile.rotation.ToRotationVector2().SafeNormalize(Vector2.Zero);
                direction = direction.RotatedByRandom(MathHelper.ToRadians(2));
                float distance = Main.rand.NextFloat(20, 24);
                Vector2 newPosition = currentPosition + direction * distance;
                currentPosition = newPosition;
                positions.Add(currentPosition);


                float distanceFromTarget = 700;
                Vector2 targetCenter = currentPosition;
                bool foundTarget = false;
                // This code is required either way, used for finding a target
                for (int j = 0; j < Main.maxNPCs; j++)
                {
                    NPC npc = Main.npc[j];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, currentPosition);
                        bool closest = Vector2.Distance(currentPosition, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(currentPosition, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        if (((closest && inRange) || !foundTarget) && lineOfSight)
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }

                if (foundTarget && distanceFromTarget < teleportDistance)
                {
                    positions.Add(targetCenter);
                    positions.Add(targetCenter);
                    break;
                }
            }


            return positions.ToArray();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = _lightningArcPos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, Trail_Width, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        internal PrimitiveTrail BeamDrawer;

        public float alphaCounter = 1;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Vector2 drawPos = _lightningArcPos[_lightningArcPos.Length - 1];
            Vector2 drawOrigin = new Vector2(32, 32);
            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(texture2D4, drawPos - Main.screenPosition, null, new Color((int)(255f * alphaCounter), (int)(255f * alphaCounter), (int)(255f * alphaCounter), 0), Projectile.rotation,
                    drawOrigin, 1f, SpriteEffects.None, 0f);
            }

            return base.PreDraw(ref lightColor);
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.OrangeRed);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);

            BeamDrawer.DrawPixelated(_lightningArcPos, -Main.screenPosition, _lightningArcPos.Length);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
