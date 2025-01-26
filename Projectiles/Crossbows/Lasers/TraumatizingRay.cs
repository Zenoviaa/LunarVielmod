using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Crossbows.Lasers
{
    internal class TraumatizingRay : ModProjectile, IPixelPrimitiveDrawer
    {
        private bool _setRotation;
        private float _radians;
        private float _targetRadians;
        private float BeamLength;
        internal PrimitiveTrail BeamDrawer;
        public ref float Time => ref Projectile.ai[0];
        private ref float Dir => ref Projectile.ai[1];
        public NPC Owner => Main.npc[(int)Projectile.ai[1]];
        public const float LaserLength = 2400f;
        private Vector2 EndPoint => Projectile.Center + Projectile.velocity * BeamLength;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9;
            Projectile.timeLeft = 60;
            Projectile.alpha = 255;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Time++;
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                swordRotation = (Main.MouseWorld - player.Center).ToRotation();
                if (!player.channel)
                    Projectile.Kill();


                Projectile.velocity = swordRotation.ToRotationVector2();

                Projectile.netUpdate = true;
            }


            Projectile.spriteDirection = player.direction;

            if (!_setRotation)
            {
                _radians = MathHelper.ToRadians(32);
                if (Dir == 1)
                    _radians = -_radians;
                _targetRadians = -_radians;
                _setRotation = true;
            }

            float progress = Time / 60f;
            float easedProgress = Easing.InOutCubic(progress);
            float rads = MathHelper.Lerp(_radians, _targetRadians, easedProgress);
            if (Projectile.spriteDirection == 1)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(rads);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(rads);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            }

            Projectile.Center = playerCenter + Projectile.velocity * 1f;
            float targetBeamLength = ProjectileHelper.PerformBeamHitscan(Projectile, 800);
            BeamLength = targetBeamLength;
            // Fade in.
            Projectile.alpha = Utils.Clamp(Projectile.alpha - 25, 0, 255);

            Projectile.scale = MathF.Sin(Time / 100 * MathHelper.Pi) * 3f;
            if (Projectile.scale > 1f)
                Projectile.scale = 1f;

            if (Projectile.timeLeft < 50)
            {
                Projectile.scale = (float)Projectile.timeLeft / (float)50;
            }
            else
            {
                Projectile.scale = MathF.Sin(Time / 100 * MathHelper.Pi) * 3f;
                if (Projectile.scale > 1f)
                    Projectile.scale = 1f;
            }

            // And create bright light.
            Lighting.AddLight(Projectile.Center, Color.Goldenrod.ToVector3() * 1.5f);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
            FXUtil.GlowCircleBoom(target.Center,
               innerColor: Color.White,
               glowColor: Color.Red,
               outerGlowColor: Color.DarkRed, duration: 25, baseSize: 0.06f);

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger"), target.position);
            ShakeModSystem.Shake = 1;
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Red, 0.5f).noGravity = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * BeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public float WidthFunction(float completionRatio)
        {
            return Projectile.width * Projectile.scale * 0.5f;
        }

        public override bool ShouldUpdatePosition() => false;

        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.Red, Color.DarkRed, 0.65f);
            return color * Projectile.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 centerPos = EndPoint - Main.screenPosition;
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyLongGlowParticle).Value;
            GlowCircleShader shader = GlowCircleShader.Instance;
            shader.Speed = 5;

            shader.BasePower = 2.5f;
            shader.Size = VectorHelper.Osc(0.09f, 0.15f, speed: 34);

            Color startInner = Color.Lerp(Color.Red, Color.OrangeRed, VectorHelper.Osc(0, 1));
            Color startGlow = Color.Lerp(Color.DarkRed, Color.DarkRed, VectorHelper.Osc(0, 1));
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
            float scale = Projectile.scale;
            spriteBatch.Draw(secondTexture, centerPos, null, Color.White, rotation, secondTexture.Size() / 2f, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(secondTexture, centerPos, null, Color.White, rotation, secondTexture.Size() / 2f, scale, SpriteEffects.None, 0);
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, rotation, texture.Size() / 2f, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, centerPos, null, Color.White, rotation + MathHelper.ToRadians(90), texture.Size() / 2f, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, centerPos, null, Color.White, rotation + MathHelper.ToRadians(180), texture.Size() / 2f, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, centerPos, null, Color.White, rotation + MathHelper.ToRadians(270), texture.Size() / 2f, scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            Color middleColor = Color.Lerp(Color.White, Color.Red, 0.6f);
            Color middleColor2 = Color.Lerp(Color.DarkRed, Color.OrangeRed, 0.5f);
            Color finalColor = Color.Lerp(middleColor, middleColor2, Time / 600);

            TrailRegistry.LaserShader.UseColor(Color.LightYellow);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.TwistingTrail);

            List<Vector2> points = new();
            for (int i = 0; i <= 8; i++)
            {
                points.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity * BeamLength, i / 8f));
            }
            points.Add(Projectile.Center + Projectile.velocity * BeamLength);
            points.Add(Projectile.Center + Projectile.velocity * BeamLength);
            BeamDrawer.DrawPixelated(points, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }

        public override bool? CanDamage() => true;
    }
}
