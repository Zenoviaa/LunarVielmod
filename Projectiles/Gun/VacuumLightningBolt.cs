using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class VacuumLightningBolt : ModProjectile
    {
        private Vector2[] _lightningArcPos = new Vector2[1];
        public const int Trail_Width = 24;
        private ref float Timer => ref Projectile.ai[0];

        private Vector2 TargetPosition;
        private Player Owner => Main.player[Projectile.owner];
        public CommonLightning Lightning { get; set; } = new CommonLightning();
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 48;
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

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(TargetPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            TargetPosition = reader.ReadVector2();
        }

        public override void AI()
        {
            if (TargetPosition == Vector2.Zero)
                TargetPosition = Owner.Center;
            Vector2 playerCenter = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Timer++;

            if (Main.myPlayer == Projectile.owner)
            {
                Owner.ChangeDir(Projectile.direction);
                TargetPosition = Vector2.Lerp(TargetPosition, Main.MouseWorld, 0.1f);
                Projectile.velocity = (TargetPosition - Owner.Center).SafeNormalize(Vector2.Zero);
                Projectile.netUpdate = true;
                if (!Owner.channel)
                    Projectile.Kill();
            }

            Projectile.Center = playerCenter + Projectile.velocity * 1f;// customization of the hitbox position

            //Dunno if this is needed but whatever
            Projectile.rotation = Projectile.velocity.ToRotation();
            _lightningArcPos = CalculateLightningArc();
            for (int i = 1; i < _lightningArcPos.Length - 1; i++)
            {
                float p = (float)i / (float)_lightningArcPos.Length - 1;
                ref Vector2 pos = ref _lightningArcPos[i];
                ref Vector2 nextPos = ref _lightningArcPos[i + 1];
                Vector2 vec = (nextPos - pos);
                vec = vec.RotatedBy(MathHelper.ToRadians(90));
                vec *= p;

                pos += vec * MathF.Sin(Main.GlobalTimeWrappedHourly * -12 + p * 24);
                pos += vec * MathF.Sin((Main.GlobalTimeWrappedHourly + 4) * -12 + p * 12);

            }

            for (int i = 0; i < Lightning.Trails.Length; i++)
            {
                float progress = (float)i / (float)Lightning.Trails.Length;
                var trail = Lightning.Trails[i];
                trail.LightningRandomOffsetRange = 4;
                trail.LightningRandomExpand = 24;
                trail.PrimaryColor = Color.Lerp(Color.White, Color.Cyan, progress);
                trail.NoiseColor = Color.Lerp(Color.White, Color.Cyan, progress);
                Lightning.WidthTrailFunction = WidthFunction;
            }
            if (Timer % 3 == 0)
            {
                // Lightning.SyncOffsets = true;
                Lightning.RandomPositions(_lightningArcPos);
                for (int i = 0; i < _lightningArcPos.Length - 3; i++)
                {
                    Vector2 pos = _lightningArcPos[i];
                    if (Main.rand.NextBool(8))
                    {
                        Dust.NewDustPerfect(pos, ModContent.DustType<GlyphDust>(), Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 1f), 0, Color.Cyan, Main.rand.NextFloat(1f, 2f)).noGravity = true;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Electrifying!!!! nEMIES!!!
            target.AddBuff(BuffID.Electrified, 120);
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.position);

            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4, 4);
                var d = Dust.NewDustPerfect(target.Center, DustID.Electric, speed, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                d.noGravity = true;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(24, 16, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.Cyan;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        private Vector2[] CalculateLightningArc()
        {
            float teleportDistance = 96;
            Vector2 currentPosition = Projectile.position;
            List<Vector2> positions = new List<Vector2>();
            positions.Add(currentPosition);
            for (int i = 0; i < 48; i++)
            {
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                float distance = 40;
                Vector2 newPosition = currentPosition + direction * distance;
                currentPosition = newPosition;
                positions.Add(currentPosition);



                Vector2 targetCenter = currentPosition;
                bool foundTarget = false;
                NPC nearest = ProjectileHelper.FindNearestEnemy(currentPosition, teleportDistance);
                if (nearest != null)
                {
                    targetCenter = nearest.Center;
                    positions.Add(targetCenter);
                    positions.Add(targetCenter);
                    break;
                }

                if (!foundTarget)
                {
                    float distanceToMouse = Vector2.Distance(currentPosition, TargetPosition);
                    if (distanceToMouse < teleportDistance)
                    {
                        positions.Add(TargetPosition);
                        positions.Add(TargetPosition);
                        break;
                    }
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

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Lightning.Draw(spriteBatch, _lightningArcPos, Projectile.oldRot);

            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
            Vector2 centerPos = _lightningArcPos[_lightningArcPos.Length - 1] - Main.screenPosition;
            centerPos += Main.rand.NextVector2Circular(8, 8);
            GlowCircleShader shader = GlowCircleShader.Instance;

            //How quickly it lerps between the colors
            shader.Speed = 10f;

            //This effects the distribution of colors
            shader.BasePower = 2.5f;

            //Radius of the circle
            shader.Size = VectorHelper.Osc(0.09f, 0.14f, speed: 6);


            //Colors
            Color startInner = Color.White;
            Color startGlow = Color.Lerp(Color.Cyan, Color.Cyan, VectorHelper.Osc(0f, 1f, speed: 3f));
            Color startOuterGlow = Color.Lerp(Color.Black, Color.Black, VectorHelper.Osc(0f, 1f, speed: 3f));

            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;

            //Idk i just included this to see how it would look
            //Don't go above 0.5;
            shader.Pixelation = 0.005f;

            //This affects the outer fade
            shader.OuterPower = 13.5f;
            shader.Apply();

            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
            return false;
        }
    }
}
