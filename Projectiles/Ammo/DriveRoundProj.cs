using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Ammo
{
    internal class DriveRoundProj : ModProjectile,
        IPixelPrimitiveDrawer
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 4; // The width of projectile hitbox
            Projectile.height = 4; // The height of projectile hitbox
            Projectile.aiStyle = 1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 3; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.penetrate = -1;
            AIType = ProjectileID.Bullet; // Act exactly like default Bullet
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            if(completionRatio < 0.33f)
            {
                return Color.Lerp(Color.White, Color.Red, completionRatio / 0.33f);
            }
            else
            {
                return Color.Black;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
            ShakeModSystem.Shake = 2;
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom") { PitchVariance = 0.15f }, Projectile.position);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<FireBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            ShakeModSystem.Shake = 2;
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom") { PitchVariance = 0.15f }, Projectile.position);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<FireBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        internal PrimitiveTrail BeamDrawer;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.Black);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);

            BeamDrawer.DrawPixelated(Projectile.oldPos, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
