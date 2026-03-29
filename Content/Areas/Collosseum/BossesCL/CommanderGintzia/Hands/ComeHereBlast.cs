using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia.Hands
{
    public class BlowAwayPlayer : ModPlayer
    {
        public Vector2? blowVelocity;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (blowVelocity.HasValue)
            {
                Vector2 targetVelocity = blowVelocity.Value;
                //    Player.velocity = Vector2.Lerp(Player.velocity, targetVelocity, 0.5f);
                Player.velocity.X = MathHelper.Lerp(Player.velocity.X, targetVelocity.X, 0.5f);
                blowVelocity = null;
            }
        }
    }
    public class ComeHereBlast : ModProjectile
    {
        private Vector2[] _oldSwingPos;
        private ref float Timer => ref Projectile.ai[0];
        private bool Blow
        {
            get => Projectile.ai[1] == 1;
        }
        private Rectangle _blowRect;

        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1500;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _oldSwingPos = new Vector2[32];
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.height = 128;
            Projectile.width = 128;
            Projectile.hostile = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * Projectile.velocity;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle windStorm = new SoundStyle("Stellamod/Assets/Sounds/WindStorm");
                SoundEngine.PlaySound(windStorm);
            }

            for (int i = 0; i < _oldSwingPos.Length; i++)
            {
                float progress = i / (float)_oldSwingPos.Length;
                Vector2 pos = Vector2.Lerp(Projectile.Center,
                    Projectile.Center + Projectile.velocity, progress);
                _oldSwingPos[i] = pos;
            }

            Vector2 p1 = Projectile.Center - new Vector2(0, Projectile.width / 2);
            Vector2 p2 = Projectile.Center + new Vector2(Projectile.velocity.X, Projectile.width / 2);

            Vector2 topLeft = new Vector2();
            topLeft.X = MathF.Min(p1.X, p2.X);
            topLeft.Y = MathF.Min(p1.Y, p2.Y);

            Vector2 bottomRight = new Vector2();
            bottomRight.X = MathF.Max(p1.X, p2.X);
            bottomRight.Y = MathF.Max(p1.Y, p2.Y);
            float width = bottomRight.X - topLeft.X;
            float height = bottomRight.Y - topLeft.Y;
            _blowRect = new Rectangle((int)topLeft.X, (int)topLeft.Y, 
                (int)width, (int)height);

            foreach (var player in Main.ActivePlayers)
            {
                if (_blowRect.Intersects(player.getRect()))
                {

                    Vector2 blowVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    blowVelocity.Y = 0;
                    blowVelocity *= 7;
                             
                    BlowAwayPlayer blowAwayPlayer = player.GetModPlayer<BlowAwayPlayer>();
                    blowAwayPlayer.blowVelocity = blowVelocity;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, Color.LightGray, 0.5f);
            shader.NoiseColor = Color.LightGray;
            shader.OutlineColor = Color.Transparent;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;

            //This just applis the shader changes

            //Main Fill
            Rectangle r = _blowRect;
            r.X -= (int)Main.screenPosition.X;
            r.Y -= (int)Main.screenPosition.Y;
            Primitives2D.DrawRectangle(Main.spriteBatch, r, Color.Red);
            TrailDrawer.Draw(Main.spriteBatch,_oldSwingPos, Projectile.oldRot, StripColors, StripWidth, shader);
            return false;
        }
        private Color StripColors(float progressOnStrip)
        {
            //  return Color.Lerp(Color.LightGoldenrodYellow, Color.White, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            Color result = Color.Lerp(Color.LightGray, Color.White,
                Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            //     result.A /= 2;

            float alpha = MathHelper.Clamp(Timer / 60f, 0f,1f);
            result *= alpha;
            if(Projectile.timeLeft < 60)
            {
                float lerp = (Projectile.timeLeft) / 60f;
                result *= lerp; 
            }
            float fadeInOut = EasingFunction.QuadraticBump(progressOnStrip);
            result *= fadeInOut;
            return result;
        }

        private float StripWidth(float progressOnStrip)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1f;
            return MathHelper.SmoothStep(baseWidth, baseWidth, progressOnStrip);
        }
    }
}
