using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles.Slashers.GrailAxe
{
    public class GrailShot : ModProjectile
    {
        private bool Moved;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 35;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = 250;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3) && !target.boss)
            {
                target.AddBuff(BuffID.Confused, 180);
            }
        }

        private float alphaCounter = 0;
        public override void AI()
        {
            Projectile.velocity *= 0.96f;
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.alpha = 255;
                Moved = true;
            }
            if (Projectile.ai[1] >= 20)
            {
                Projectile.tileCollide = true;
            }


            if (Projectile.ai[1] == 160)
            {
                var EntitySource = Projectile.GetSource_Death();
                Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<KaBoomMagic2>(), Projectile.damage, 1, Projectile.owner, 0, 0);
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Binding_Abyss_Rune"));
                Projectile.Kill();
            }
            if (Projectile.timeLeft <= 50)
            {
                Projectile.alpha += 4;
                alphaCounter -= 0.08f;
            }
            else
            {
                if (alphaCounter <= 1)
                {
                    alphaCounter += 0.08f;
                }
            }

            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation += 0.08f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

       
        public override bool PreDraw(ref Color lightColor)
        {
         
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.LightCyan.ToVector3() * 1.75f * Main.essScale);
        }
    }
}