using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.EliteCommander.Projectiles
{
    public class EliteCommanderHand : ModProjectile,
        IDrawOutlines
    {
        private int _frame = -1;
        private ref float Timer => ref Projectile.ai[0];

        private int ParentNPC
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private bool Die;
        private ref float KillTimer => ref Projectile.ai[2];
        private float PunchTimer;
        private float AlphaTimer;
        private float Duration => 1800;
        private float Radius => 200;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.timeLeft = (int)Duration;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            if (_frame == -1)
            {
                _frame = Main.rand.Next(0, Main.projFrames[Type]);
            }
            Timer++;


            if (Timer == 1)
            {
                SoundStyle soundStyle;
                switch (Main.rand.Next(2))
                {
                    default:
                    case 0:
                        soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GladiatorMirage1");
                        break;
                    case 1:
                        soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GladiatorMirage2");
                        break;
                }
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }
            NPC parent = Main.npc[ParentNPC];
            Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 1024);
            if (player != null)
            {
                float distance = Vector2.Distance(parent.Center, player.Center);
                if (distance < Radius)
                {
                    PunchTimer++;
                    if (PunchTimer >= 45)
                    {
                        PunchTimer = 45;
                    }
                }
                else
                {
                    PunchTimer--;
                    if (PunchTimer <= 0)
                    {
                        PunchTimer = 0;
                    }
                }
            }


            if (!parent.active || Timer > Duration - 60)
            {
                Die = true;
            }
            else if (parent.active && !Die)
            {
                AlphaTimer = MathHelper.Lerp(AlphaTimer, 1f, 0.1f);
                float rot = (Timer / 120f) * MathHelper.TwoPi;
                float progress = Easing.OutCubic(PunchTimer / 45f);
                Vector2 offset = rot.ToRotationVector2() * (52 + progress * 150);
                Projectile.Center = parent.Center + offset;
                Projectile.rotation = rot;
            }

            if (Die)
            {
                Projectile.hostile = false;
                
                AlphaTimer = MathHelper.Lerp(1f, 0f, KillTimer / 60f);
                KillTimer++;
                if (KillTimer >= 60)
                {
                    Projectile.Kill();
                }
            }
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.GetFrame(_frame, 4);
            Vector2 drawOrigin = frame.Size() / 2;
            float drawRotation = Projectile.rotation;
            Color outlineColor = Color.Red * AlphaTimer; 

            float drawScale = 1f;
            float outlineOffset = 2;
            spriteBatch.Draw(texture, drawPos + Vector2.UnitX * outlineOffset, frame, outlineColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPos + Vector2.UnitX * -outlineOffset, frame, outlineColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPos + Vector2.UnitY * outlineOffset, frame, outlineColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPos + Vector2.UnitY * -outlineOffset, frame, outlineColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.GetFrame(_frame, 4);
            Vector2 drawOrigin = frame.Size() / 2;
            float drawRotation = Projectile.rotation;
            Color drawColor = Color.White.MultiplyRGB(lightColor) * AlphaTimer;
            float drawScale = 1f;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            return false;
        }

    }
}
