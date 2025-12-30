using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.EliteCommander.Projectiles
{
    public class WindShockwave : ModProjectile
    {
        private Vector2[] _shockwavePos;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 16 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, Scale: 0.5f);
            }

            Projectile.velocity *= 1.01f;
        }

        private Color GetTrailColor(float progressOnTrail)
        {
            return Color.Lerp(Color.White, Color.Transparent, progressOnTrail);
        }
        private float GetTrailWidth(float progressOnTrail)
        {
            return MathHelper.SmoothStep(64, 0f, progressOnTrail);
        }
        private void DrawPixelatedShockwave(GraphicsDevice graphicsDevice)
        {        //Draw Trail
            _shockwavePos ??= new Vector2[Projectile.oldPos.Length];

            var shader = BasicLaserShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.DarkGray;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                List<Vector2> shockwavePos = new List<Vector2>();
                float totalP = (float)i / (float)Projectile.oldPos.Length;
                totalP = 1f - totalP;

                float numPoints = 4f;
                for (int s = 0; s < numPoints; s++)
                {
                    float p = (float)s / numPoints;
                    Vector2 pos = Vector2.Lerp(oldPos, oldPos - Vector2.UnitY * 80 * totalP *
                        VectorHelper.Osc(0.5f, 1f, speed: 12, offset: i * 4) * MathHelper.Clamp(Timer / 30f, 0f, 1f), p);
                    //
                    shockwavePos.Add(pos);
                }
                Vector2[] shockPos = shockwavePos.ToArray();
                Vector2 trailOffset = Projectile.Size / 2;
                TrailDrawer.Draw(Main.spriteBatch, shockPos, GetTrailColor, GetTrailWidth, shader, trailOffset);
            }


        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedShockwave);

            return false;
        }
    }
}
