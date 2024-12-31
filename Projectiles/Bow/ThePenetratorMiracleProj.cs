using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Bow
{
    internal class ThePenetratorMiracleProj : ModProjectile
    {
        public override string Texture => "Stellamod/Items/Weapons/Ranged/PenetratorMiracle";
        Vector2 HoldOffset;
        ref float Timer => ref Projectile.ai[0];
        ref float Rotation => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 82;
            Projectile.timeLeft = 3600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Timer++;
            if(Timer == 1)
            {
                HoldOffset = Main.rand.NextVector2CircularEdge(48, 48);
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/CrossbowPull");
                soundStyle.PitchVariance = 0.15f;
                soundStyle.Pitch = -0.75f;
                SoundEngine.PlaySound(soundStyle);
            }

            float progress = Timer / 44;
            float easedProgress = Easing.OutCubic(progress);
            Vector2 direction = player.Center.DirectionTo(player.Center + HoldOffset);
            Vector2 startHoldOffset = HoldOffset + direction * 128;
            Vector2 holdOffset = Vector2.Lerp(startHoldOffset, HoldOffset, easedProgress);
            Projectile.Center = player.Center + holdOffset;

            float rotationOffset = MathHelper.Lerp(MathHelper.TwoPi, 0, easedProgress);
            if(Timer < 44 && Main.myPlayer == Projectile.owner)
            {
                Rotation = Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation();
                Rotation += rotationOffset;
                Projectile.netUpdate = true;
            }

            Projectile.rotation = Rotation;
            if (Timer == 44)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon");
                soundStyle.PitchVariance = 0.15f;
                soundStyle.Pitch = -0.75f;
                SoundEngine.PlaySound(soundStyle);
            }

            if(Timer == 50)
            {
                Vector2 velocity = Projectile.rotation.ToRotationVector2() * 15;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<ThePenetratorMiracleArrowProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2"); 
                soundStyle.PitchVariance = 0.15f;
                soundStyle.Volume = 0.5f;
                soundStyle.Pitch = 0.75f;
                SoundEngine.PlaySound(soundStyle);
                Projectile.Kill();
            } 
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, ColorFunctions.MiracleVoid, 1f).noGravity = true;
            }
        }

        public override void PostDraw(Color lightColor)
        {
            float progress = Timer / 44;
            float scale = MathHelper.Lerp(1.5f, 1f, progress);
            Color color = Color.Black * progress;
        

            float width = 54;
            float height = 82;
            Vector2 drawOrigin = new Vector2(width, height) / 2;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D whiteTexture = (Texture2D)ModContent.Request<Texture2D>(Texture + "_White");
            Main.spriteBatch.Draw(whiteTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale * scale, SpriteEffects.None, 0f);
        }
    }
}
