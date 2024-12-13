using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    public class TechnoBeam : ModProjectile,
        IPixelPrimitiveDrawer
    {
        private List<Vector2> _beamPoints;
        private ref float Timer => ref Projectile.ai[0];
        private ref float PulseTimer => ref Projectile.ai[1];
        private float BeamLength;
        private PrimitiveTrail BeamDrawer;
        private Player Owner => Main.player[Projectile.owner];
        private Vector2 EndPoint => Projectile.Center + Projectile.velocity * BeamLength;
        public override string Texture => "Stellamod/Items/Weapons/Ranged/TecnoBlaster";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // Signals to Terraria that this Projectile requires a unique identifier beyond its index in the Projectile array.
            // This prevents the issue with the vanilla Last Prism where the beams are invisible in multiplayer.
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;

            // Prevents jitter when stepping up and down blocks and half blocks
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _beamPoints = new List<Vector2>();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.timeLeft = int.MaxValue;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(BeamLength);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            BeamLength = reader.ReadSingle();
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width;
            Vector2 start = Projectile.Center;

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * (BeamLength);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public override void AI()
        {
            base.AI();
            Vector2 rrp = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            // Update the Prism's position in the world and relevant variables of the player holding it.
            UpdatePlayerVisuals(Owner, rrp);

            Timer++;
            if(Timer == 1)
            {
                Projectile.Center = Owner.Center;
            }
            PulseTimer--;
            if(PulseTimer <= 0)
            {
                PulseTimer = 0;
            }

            if(Timer % 24 == 0)
            {
                PulseTimer = 15;
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew5");
                    soundStyle.PitchVariance = 0.5f;
                    SoundEngine.PlaySound(soundStyle, Projectile.position);
                }
                else
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew5");
                    soundStyle.PitchVariance = 3.5f;
                    SoundEngine.PlaySound(soundStyle, Projectile.position);
                }
            }

            if (Main.myPlayer == Projectile.owner)
            {
                // Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, Main.MouseWorld);
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                bool stillInUse = Owner.channel && !Owner.noItems && !Owner.CCed;
                if (!stillInUse)
                {
                    Projectile.Kill();
                }


                float targetBeamLength = ProjectileHelper.PerformBeamHitscan(Projectile, 600);
                float mouseBeamLength = Vector2.Distance(Projectile.Center, Main.MouseWorld);
                targetBeamLength = MathF.Min(targetBeamLength, mouseBeamLength);
                float ep = Easing.InOutCubic(Timer / 30f);
                BeamLength = MathHelper.Lerp(0f, targetBeamLength, ep);
                Projectile.netUpdate = true;
            }

            //Hitscan the Beam
            _beamPoints.Clear();

            float num = 8;
            Vector2 endPoint = Projectile.Center + Projectile.velocity * BeamLength;
            for (float i = 0; i < num; i++)
            {
                float progress = i / num;
                Vector2 velocity = Projectile.velocity;
                //      velocity = velocity.RotatedBy(MathF.Sin(progress + Main.GlobalTimeWrappedHourly) * MathHelper.ToRadians(15));
                Vector2 point = Vector2.Lerp(Projectile.Center, Projectile.Center + velocity * BeamLength, progress);
                _beamPoints.Add(point);
            }
            _beamPoints.Add(endPoint);
            _beamPoints.Add(endPoint);
            _beamPoints.Add(endPoint);
            if (Timer % 16 == 0)
            {
                Dust.NewDustPerfect(EndPoint + Main.rand.NextVector2Circular(32, 32), DustID.BlueTorch);
                Dust.NewDustPerfect(EndPoint + Main.rand.NextVector2Circular(32, 32), DustID.PinkTorch);
            }
            ShakeModSystem.Shake = 0.05f;

        }
        private void UpdatePlayerVisuals(Player player, Vector2 playerHandPos)
        {
            // Place the Prism directly into the player's hand at all times.
            Projectile.Center = playerHandPos + Projectile.velocity * 16;
            // The beams emit from the tip of the Prism, not the side. As such, rotate the sprite by pi/2 (90 degrees).
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            // The Prism is a holdout Projectile, so change the player's variables to reflect that.
            // Constantly resetting player.itemTime and player.itemAnimation prevents the player from switching items or doing anything else.
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            // If you do not multiply by Projectile.direction, the player's hand will point the wrong direction while facing left.
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }


        public float WidthFunction(float completionRatio)
        {
            float osc = VectorHelper.Osc(0.5f, 1f, speed: 32);

            float width = 2f;
            float pulseWidth = PulseTimer / 15f;
            return (Projectile.width * Projectile.scale) * osc * width * 0.9f + pulseWidth * 15f;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.LightPink, Color.Blue, VectorHelper.Osc(0, 1, speed: 32));
            return color;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw the sparkle
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 centerPos = EndPoint - Main.screenPosition;
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyLongGlowParticle).Value;
            GlowCircleShader shader = GlowCircleShader.Instance;
            shader.Speed = 5;

            shader.BasePower = 2.5f;
            shader.Size = VectorHelper.Osc(0.09f, 0.15f, speed: 34);

            Color startInner = Color.Lerp(Color.LightPink, Color.LightCyan, VectorHelper.Osc(0, 1));
            Color startGlow = Color.Lerp(Color.DarkBlue, Color.DarkBlue, VectorHelper.Osc(0, 1));
            Color startOuterGlow = Color.Lerp(Color.Black, Color.Black, VectorHelper.Osc(0, 1));

            shader.InnerColor = startInner;
            shader.GlowColor = startGlow;
            shader.OuterGlowColor = startOuterGlow;
            Color endColor = startOuterGlow;

            float rotation = Main.GlobalTimeWrappedHourly * 4;
            shader.Pixelation = 0.005f;

            shader.Apply();

            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            Texture2D secondTexture = ModContent.Request<Texture2D>(TextureRegistry.EmptyGlowParticle).Value;
            spriteBatch.Draw(secondTexture, centerPos, null, Color.White, rotation, secondTexture.Size() / 2f, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(secondTexture, centerPos, null, Color.White, rotation, secondTexture.Size() / 2f, 1f, SpriteEffects.None, 0);
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, centerPos, null, Color.White, rotation + MathHelper.ToRadians(90), texture.Size() / 2f, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, centerPos, null, Color.White, rotation + MathHelper.ToRadians(180), texture.Size() / 2f, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, centerPos, null, Color.White, rotation + MathHelper.ToRadians(270), texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();

            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int spriteSheetOffset = frameHeight * Projectile.frame;
            Vector2 mainDrawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            // The Prism is always at full brightness, regardless of the surrounding light. This is equivalent to it being its own glowmask.
            // It is drawn in a non-white color to distinguish it from the vanilla Last Prism.
            Color drawColor = Color.White.MultiplyRGB(lightColor);

            spriteBatch.Restart(blendState: BlendState.Additive);
            for (float f = 0; f < 8; f++)
            {
                float rot = (f / 8f) * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 3;
                offset += Main.rand.NextVector2Circular(1.5f, 1.5f);
                Main.EntitySpriteDraw(texture, mainDrawPos + offset, null, drawColor * VectorHelper.Osc(0f, 1f, speed: 8), Projectile.velocity.ToRotation(), drawOrigin, Projectile.scale, effects, 0f);
            }

            spriteBatch.RestartDefaults();

            Main.EntitySpriteDraw(texture, mainDrawPos, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, Projectile.scale, effects, 0f);

            spriteBatch.Restart(blendState: BlendState.Additive);
            Main.EntitySpriteDraw(texture, mainDrawPos, null, drawColor * VectorHelper.Osc(0f, 1f, speed: 8), Projectile.velocity.ToRotation(), drawOrigin, Projectile.scale, effects, 0f);

            spriteBatch.RestartDefaults();

            return false;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            FXUtil.GlowCircleBoom(target.Center,
               innerColor: Color.White,
               glowColor: Color.LightCyan,
               outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.06f);

            FXUtil.GlowCircleBoom(target.Center,
               innerColor: Color.White,
               glowColor: Color.LightPink,
               outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.03f);

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger"), target.position);
            ShakeModSystem.Shake = 1;
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightPink, 0.5f).noGravity = true;
            }
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            TrailRegistry.LaserShader.UseColor(Color.Lerp(Color.LightPink, Color.LightCyan, VectorHelper.Osc(0, 1, speed: 32)));
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.CorkscrewTrail);

            for (int i = 1; i < _beamPoints.Count - 4; i++)
            {
                //APply Offests
                float progress = (float)i / (float)_beamPoints.Count;
                float velOffset = MathF.Sin(progress * 64 * Main.GlobalTimeWrappedHourly);
                Vector2 vel = Vector2.UnitY.RotatedBy(Projectile.velocity.ToRotation());
                Vector2 offset = Vector2.Lerp(-vel, vel, velOffset) * 16 * (PulseTimer / 15f);
                _beamPoints[i] += offset;
            }
            BeamDrawer.DrawPixelated(_beamPoints, -Main.screenPosition, 255);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}