using Stellamod.Dusts;
using Stellamod.Projectiles.IgniterExplosions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;

namespace Stellamod.Items.Armors.Astrasilk
{
    internal class AstrasilkGigaStarProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 35;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.98f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightPink, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.CausticTrail);
            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            //Cool explosion or something
            switch (Main.rand.Next(2))
            {
                case 0:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb1"), Projectile.position);
                    break;
                case 1:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb2"), Projectile.position);
                    break;
            }

            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), 
                    (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSkyBlue, 1f).noGravity = true;
            }

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<SiriusBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
