using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.EliteCommander.Projectiles
{
    internal class WindShockwave : ModProjectile
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

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.62f;
            return MathHelper.SmoothStep(baseWidth * 2, baseWidth, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.White;
            float easedCompletion = Easing.InCubic(completionRatio);
            return Color.Lerp(startColor, Color.Transparent, easedCompletion);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override bool PreDraw(ref Color lightColor)
        {
            //Draw Trail
            _shockwavePos ??= new Vector2[Projectile.oldPos.Length];
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.RestartDefaults();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                List<Vector2> shockwavePos = new List<Vector2>();
                float totalP = (float)i / (float)Projectile.oldPos.Length;
                totalP = 1f - totalP;
                for (int s = 0; s < 8; s++)
                {
                    float p = (float)s / 8f;
                    Vector2 pos = Vector2.Lerp(oldPos, oldPos - Vector2.UnitY * 80 * totalP *
                        VectorHelper.Osc(0.5f, 1f, speed: 6, offset: i * 4) * MathHelper.Clamp(Timer / 30f, 0f, 1f), p);
                    //
                    shockwavePos.Add(pos);
                }
                Vector2[] shockPos = shockwavePos.ToArray();
                Vector2 trailOffset = -Main.screenPosition + Projectile.Size / 2;
                TrailDrawer.DrawPrims(shockPos, trailOffset, 155);
                shockwavePos.Clear();

                for (int s = 0; s < 8; s++)
                {
                    float p = (float)s / 8f;
                    shockwavePos.Add(Vector2.Lerp(oldPos, oldPos + Vector2.UnitY * 40 * totalP *
                        VectorHelper.Osc(0.5f, 1f, speed: 6, offset: i * 4) * MathHelper.Clamp(Timer / 30f, 0f, 1f), p));
                }
                shockPos = shockwavePos.ToArray();
                TrailDrawer.DrawPrims(shockPos, trailOffset, 155);
            }




            return false;
        }
    }
}
