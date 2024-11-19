using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.DrawEffects;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Gustbeak.Projectiles
{
    internal abstract class BaseWindProjectile : ModProjectile
    {
        private CommonWind _wind;

        protected CommonWind Wind
        {
            get
            {
                _wind ??= new CommonWind();
                return _wind;
            }
        }

        protected ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;

        public PrimDrawer Trail { get; set; }

        public override void AI()
        {
            base.AI();
            Timer++;
            Wind.AI(Projectile.Center);
        }
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.62f;
            return MathHelper.SmoothStep(32, baseWidth, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.White, Easing.SpikeOutCirc(completionRatio));
        }

        protected virtual void DrawWindTrail(ref Color lightColor)
        {
            Trail ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.Dashtrail);
            Trail.DrawPrims(Projectile.oldPos, -Main.screenPosition, totalTrailPoints: 155);
        }

        protected virtual void DrawWindSlashes(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Wind.Draw(spriteBatch, lightColor);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawWindTrail(ref lightColor);
            DrawWindSlashes(ref lightColor);
            return false;
        }
    }
}
