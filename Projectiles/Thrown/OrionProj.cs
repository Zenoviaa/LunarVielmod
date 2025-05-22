using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Dusts;
using Terraria.Audio;
namespace Stellamod.Projectiles.Thrown
{
    internal class OrionProj : ModProjectile
    {
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.1f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            Timer++;
            if(Timer % 6 == 0)
            {
                int xRand = Main.rand.Next(0, Projectile.width);
                int yRand = Main.rand.Next(0, Projectile.height);
                Vector2 offset = new Vector2(xRand, yRand);
                Vector2 velocity = Main.rand.NextVector2Circular(2, 2);

                //Spawn Star
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position + offset, velocity,
                   ModContent.ProjectileType<OrionStarProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            if(Timer % 2 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<Sparkle>(), newColor: Color.White);
            }
        }


        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<SiriusBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            //Play Sound
            switch (Main.rand.Next(2))
            {
                case 0:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb1"), Projectile.position);
                    break;
                case 1:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb2"), Projectile.position);
                    break;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(ColorFunctions.Niivin, Color.Black, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.CausticTrail);
            DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.Niivin, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
    }
}
