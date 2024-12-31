using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    internal class HookaramaProj : ModProjectile
    {
        Vector2 StartCenter;
        Vector2 ReturnCenter;
        ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Timer++;
            if(Timer == 44)
            {
                StartCenter = Projectile.Center;
                ReturnCenter = owner.Center;

                //Sound
                if (Main.rand.NextBool(2))
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg");
                    soundStyle.PitchVariance = 0.15f;
                    soundStyle.Pitch = 0.75f;
                    SoundEngine.PlaySound(soundStyle, Projectile.position);
                }
                else
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg2");
                    soundStyle.PitchVariance = 0.15f;
                    soundStyle.Pitch = 0.75f;
                    SoundEngine.PlaySound(soundStyle, Projectile.position);
                }

                //Dust Burst
                for (int i = 0; i < 8; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, speed * 4, Scale: 2f);
                    d.noGravity = true;
                }
            }

            if(Timer >= 45)
            {         
                float returnTime = 45;
                float progress = (Timer - 45) / returnTime;
                float easedProgress = Easing.InOutExpo(progress);
                Projectile.Center = Vector2.Lerp(StartCenter, ReturnCenter, easedProgress);
                Projectile.rotation += easedProgress * 0.06f;
                if(easedProgress >= 1f)
                {
                    Projectile.Kill();
                }
            }

            Projectile.velocity *= 0.92f;
            Projectile.rotation += Projectile.velocity.Length() * 0.03f;
        }
        //Trails
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkGoldenrod, Color.Transparent, completionRatio) * 0.5f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if(Timer >= 45)
            {
                DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
                DrawHelper.DrawAdditiveAfterImage(Projectile, Color.DarkGoldenrod * 0.5f, Color.Transparent, ref lightColor);
            }
          
            return base.PreDraw(ref lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit2");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, target.position);
            }
            else
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, target.position);
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, speed * 4, Scale: 2f);
                d.noGravity = true;
            }
        }
    }
}
