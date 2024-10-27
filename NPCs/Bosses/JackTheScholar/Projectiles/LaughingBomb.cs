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
    internal class LaughingBomb : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private float _scale;
        private Player _target;
        private Vector2 InitialVelocity;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                InitialVelocity = Projectile.velocity;
            }
        
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            if(Timer < 30)
            {
                _scale = MathHelper.Lerp(0f, 1f, Easing.InCubic(Timer / 30f));
                Projectile.velocity *= 0.94f;
            }

            if (Timer < 30 && _target == null || !_target.active)
            {
                _target = PlayerHelper.FindClosestPlayer(Projectile.Center, maxDetectDistance: 1024);
            }
            if(Timer > 30 && Timer < 60)
            {
                Projectile.velocity *= 1.03f;
            }

            if(_target != null)
            {
                float degreesRotate = 5;
                float length = Projectile.velocity.Length();
                float targetAngle = Projectile.Center.AngleTo(_target.Center - new Vector2(0, 128));
                Vector2 newVelocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesRotate)).ToRotationVector2() * length;
                Projectile.velocity = newVelocity;
                
            }
        
            if (Timer >= 60)
            {
                //Starting heating up
                _scale += 0.001f;
            }
            AI_Visuals();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.position, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
            }
            for (int i = 0; i < 24; i++)
            {
                Dust.NewDustPerfect(Projectile.position, DustID.Torch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 6f).noGravity = true;
            }

            float num = 8;
            for(int i = 0; i < num; i++)
            {
                float f = i;
                float progress = f / num;
                float rot = progress * MathHelper.TwoPi;
                Vector2 velocity = rot.ToRotationVector2() * 8;
                int laughingBlastType = ModContent.ProjectileType<LaughingBlast>();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, laughingBlastType, Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 512f, 32f);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, Projectile.position);
        }

        private void AI_Visuals()
        {
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
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = _scale;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            if(Timer > 60f)
            {
                float progress = (Timer - 60) / 120f;
                float range = 4 * progress;
                drawPos += Main.rand.NextVector2Circular(range, range);
            }
            spriteBatch.Draw(texture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D dimLightTexture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            float drawScale = MathHelper.Lerp(0.4f, 2f, (Timer - 60f) / 120f);
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 6; i++)
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
