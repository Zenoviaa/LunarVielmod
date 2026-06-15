using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi.Projectiles
{
    public class NiiviSpawnExplosionProj : ModProjectile
    {
        //Texture
        public override string Texture => TextureRegistry.EmptyTexture;

        //AI
        private float LifeTime => 32f;
        private ref float Timer => ref Projectile.ai[0];

        private float Progress
        {
            get
            {
                float p = Timer / LifeTime;
                return MathHelper.Clamp(p, 0, 1);
            }
        }

        //Draw Code
        private int DrawMode;

        //Trailing
        private Asset<Texture2D> FrontTrailTexture => TrailRegistry.WaterTrail;
        private MiscShaderData FrontTrailShader => TrailRegistry.LaserShader;

        private Asset<Texture2D> BackTrailTexture => TrailRegistry.WhispyTrail;
        private MiscShaderData BackTrailShader => TrailRegistry.FireWhiteVertexShader;

        //Radius
        private float StartRadius => 4;
        private float EndRadius => 384;
        private float Width => 128;

        //Colors
        private Color FrontCircleStartDrawColor => Color.White;
        private Color FrontCircleEndDrawColor => Color.Transparent;
        private Color BackCircleStartDrawColor => Color.Lerp(Color.White, Color.LightCyan, 0.4f);
        private Color BackCircleEndDrawColor => Color.Lerp(Color.DarkCyan, Color.BlueViolet, 0.7f);
        private Vector2[] CirclePos;

        public override void SetDefaults()
        {
            Projectile.width = 384;
            Projectile.height = 384;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            //Points on the circle
            CirclePos = new Vector2[64];
        }

        public override void AI()
        {
            Timer++;
            AI_ExpandCircle();
        }

        private void AI_ExpandCircle()
        {
            float easedProgess = Easing.InOutCirc(Progress);
            float radius = MathHelper.Lerp(StartRadius, EndRadius, easedProgess);
            DrawCircle(radius);
        }


        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private void DrawCircle(float radius)
        {
            Vector2 startDirection = Vector2.UnitY;
            for (int i = 0; i < CirclePos.Length; i++)
            {
                float circleProgress = i / (float)CirclePos.Length;
                float radiansToRotateBy = circleProgress * (MathHelper.TwoPi + MathHelper.PiOver4 / 2);
                CirclePos[i] = Projectile.Center + startDirection.RotatedBy(radiansToRotateBy) * radius;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float width = Width;
            float startExplosionScale = 4f;
            float endExplosionScale = 0f;
            float easedProgess = Easing.OutCirc(Progress);
            float scale = MathHelper.Lerp(startExplosionScale, endExplosionScale, easedProgess);
            switch (DrawMode)
            {
                default:
                case 0:
                    return Projectile.scale * scale * width * Easing.SpikeInOutCirc(Progress);
                case 1:
                    return Projectile.scale * width * 2.2f * Easing.SpikeInOutCirc(Progress);

            }
        }

        public Color ColorFunction(float completionRatio)
        {
            switch (DrawMode)
            {
                default:
                case 0:
                    //Front Trail
                    return Color.Transparent;
                case 1:
                    //Back Trail
                    return Color.Transparent;
            }
        }
    }
}
