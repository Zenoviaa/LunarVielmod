using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.JackTheScholar.Projectiles
{
    internal class WillOWisp : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];

        private float _scale;
        private Vector2 InitialVelocity;
        private Vector2 TargetVelocity;
        private Player _target;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.light = 0.78f;
            Projectile.timeLeft = 180;
        }


        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                InitialVelocity = Projectile.velocity;
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
            }
            if(Timer < 30 && _target == null || !_target.active)
            {
                _target = PlayerHelper.FindClosestPlayer(Projectile.Center, maxDetectDistance: 1024);
            }
            if(Timer < 30)
            {
                _scale = MathHelper.Lerp(0f, 1f, Easing.InCubic(Timer / 30f));
                Projectile.velocity *= 0.5f;
            }

            if(Timer == 30)
            {
                //Ping Sound
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_FirePing");
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            if(Timer == 90)
            {
                if(_target != null && _target.active)
                {
                    TargetVelocity = Projectile.velocity = (_target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * InitialVelocity.Length();
                }       
            }

            if(Timer > 90)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, TargetVelocity, 0.02f);
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.2f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightGoldenrodYellow, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.SimpleTrail);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = _scale;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size()/2f, drawScale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 24; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FlameBurst, 0f, -2f, 0, default(Color), 1.5f);
                Dust dust = Main.dust[num];
                dust.noGravity = true;
                dust.position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                dust.position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                dust.velocity = Projectile.DirectionTo(dust.position) * 6f;
            }

            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D dimLightTexture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            float drawScale = 0.4f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for(int i = 0; i < 3; i++)
            {
                Color glowColor = new Color(85, 45, 15);
                glowColor.A = 0;
                spriteBatch.Draw(dimLightTexture, Projectile.Center - Main.screenPosition, null, glowColor, 
                    Projectile.rotation, dimLightTexture.Size() / 2f, drawScale, SpriteEffects.None, 0f);
            }
         
            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 1.75f * Main.essScale);
        }
    }
}
