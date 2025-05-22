using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class RibbonStaffStreamerProj : ModProjectile,
        IPixelPrimitiveDrawer
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.3f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public float WidthFunction(float completionRatio)
        {
            return 2;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Red;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 1; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.GoreType<RibbonRed>());
            }
        }

        internal PrimitiveTrail BeamDrawer;
        internal Vector2[] TrailPos;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            BeamDrawer.SpecialShader = null;
            TrailRegistry.LaserShader.UseColor(Color.Red);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.TwistingTrail);

            if(TrailPos == null)
            {
                TrailPos = new Vector2[Projectile.oldPos.Length];
        
                for (int i = 0; i < TrailPos.Length; i++)
                {
                    Projectile.oldPos[i] = Projectile.position;
                }
            }

            for(int i = 0; i < TrailPos.Length; i++)
            {
                TrailPos[i] = Projectile.oldPos[i];
                TrailPos[i] += new Vector2(VectorHelper.Osc(0, 16, offset: i));
            }
            BeamDrawer.DrawPixelated(TrailPos, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
