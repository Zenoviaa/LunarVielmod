using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviThundercloudProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float LifeTime => 60;
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.LightGoldenrodYellow.R,
                Color.LightGoldenrodYellow.G,
                Color.LightGoldenrodYellow.B, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float alpha = Timer / LifeTime;
            Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Extra_47").Value;
            Color drawColor = (Color)GetAlpha(lightColor);
            drawColor *= alpha;

            Vector2 drawOrigin = texture.Size() / 2;

            float drawScale = Projectile.scale;
            float rotation = Projectile.rotation + MathHelper.PiOver2;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }

        public override void AI()
        {
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_LightingZap");
            SoundEngine.PlaySound(soundStyle, Projectile.position);

            for(int i = 0; i < 7; i++)
            {
                int type = ModContent.ProjectileType<NiiviLightningProj>();
                int damage = Projectile.damage;
                int knockback = 1;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(0, -1200), Projectile.velocity,
                    type, damage, knockback, Projectile.owner);

            }

        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
