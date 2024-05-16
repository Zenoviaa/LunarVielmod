using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    internal class NiiviFrostCircleProj: ModProjectile,
        IPixelPrimitiveDrawer
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private float CircleRadius => 768;
        private float BeamWidth => 64;
        private float Alpha;
        Vector2[] CirclePos = new Vector2[32];
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 720;
        }

        public override void AI()
        {
            if(Projectile.timeLeft > 25)
            {
                Alpha += 0.01f;
                if (Alpha >= 0.25f)
                {
                    Alpha = 0.25f;
                }
            }
            else
            {
                Alpha -= 0.01f;
            }


            foreach (var player in Main.ActivePlayers)
            {
                float distance = Vector2.Distance(Projectile.Center, player.Center);
                if(distance > CircleRadius)
                {
                    player.AddBuff(ModContent.BuffType<FlamesOfIlluria>(), 2);
                }
            }
        }

        public float WidthFunction(float completionRatio)
        {
            return Projectile.scale * BeamWidth * Alpha;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.LightCyan * Alpha;
        }

        internal PrimitiveTrail BeamDrawer;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, 
                TrailRegistry.LaserShader);
            // Some visuals here
            BeamDrawer.SpecialShader = TrailRegistry.FireWhiteVertexShader;
            BeamDrawer.SpecialShader.UseColor(Color.LightSkyBlue);
            BeamDrawer.SpecialShader.SetShaderTexture(TrailRegistry.BeamTrail);

            DrawHelper.DrawCircle(Projectile.Center, CircleRadius * Alpha * 4, CirclePos);
            BeamDrawer.DrawPixelated(CirclePos, -Main.screenPosition, CirclePos.Length);

            DrawHelper.DrawCircle(Projectile.Center, CircleRadius * Alpha * 4, CirclePos, MathHelper.PiOver2);
            BeamDrawer.DrawPixelated(CirclePos, -Main.screenPosition, CirclePos.Length);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
