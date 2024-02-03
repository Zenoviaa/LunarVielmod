using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Crossbows.Lasers
{
    internal class TraumatizingRay : ModProjectile, IPixelPrimitiveDrawer
    {
        private bool _setRotation;
        private float _radians;
        private float _targetRadians;
        internal PrimitiveTrail BeamDrawer;
        public ref float Time => ref Projectile.ai[0];
        public NPC Owner => Main.npc[(int)Projectile.ai[1]];
        public const float LaserLength = 2400f;


        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9;
            Projectile.timeLeft = 100;
            Projectile.alpha = 255;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
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
            }

            Projectile.velocity = swordRotation.ToRotationVector2();
            Projectile.spriteDirection = player.direction;

            if (!_setRotation)
            {
                _radians = MathHelper.ToRadians(62);
                if (Main.rand.NextBool(2))
                    _radians = -_radians;
                _targetRadians = -_radians;
                _setRotation = true;
            }

            _radians = MathHelper.Lerp(_radians, _targetRadians, 0.03f);
            if (Projectile.spriteDirection == 1)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(_radians);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(_radians);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            }
   
            Projectile.Center = playerCenter + Projectile.velocity * 1f;

            // Fade in.
            Projectile.alpha = Utils.Clamp(Projectile.alpha - 25, 0, 255);

            Projectile.scale = MathF.Sin(Time / 100 * MathHelper.Pi) * 3f;
            if (Projectile.scale > 1f)
                Projectile.scale = 1f;

            if(Projectile.timeLeft < 50)
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
            Time++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * (LaserLength - 80f);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public float WidthFunction(float completionRatio)
        {
            return Projectile.width * Projectile.scale * 1.3f;
        }

        public override bool ShouldUpdatePosition() => false;

        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.Red, Color.DarkRed, 0.65f);
            return color * Projectile.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            Color middleColor = Color.Lerp(Color.White, Color.Red, 0.6f);
            Color middleColor2 = Color.Lerp(Color.DarkRed, Color.OrangeRed, 0.5f);
            Color finalColor = Color.Lerp(middleColor, middleColor2, Time / 600);

            TrailRegistry.LaserShader.UseColor(Color.LightYellow);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);

            List<float> originalRotations = new();
            List<Vector2> points = new();
            for (int i = 0; i <= 8; i++)
            {
                points.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, i / 8f));
                originalRotations.Add(MathHelper.PiOver2);
            }

            BeamDrawer.DrawPixelated(points, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }

        public override bool? CanDamage() => Time >= 12f;
    }
}
