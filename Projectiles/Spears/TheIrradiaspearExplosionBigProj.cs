using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Projectiles.Spears
{
    internal class TheIrradiaspearExplosionBigProj : ModProjectile,
        IPixelPrimitiveDrawer
    {
        //Texture
        public override string Texture => TextureRegistry.EmptyTexture;

        //AI
        private float LifeTime => 24f;
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
        private PrimitiveTrail BeamDrawer;
        private int DrawMode;
        private bool SpawnDustCircle;

        //Trailing
        private Asset<Texture2D> FrontTrailTexture => TrailRegistry.FadedStreak;
        private MiscShaderData FrontTrailShader => TrailRegistry.LaserShader;

        private Asset<Texture2D> BackTrailTexture => TrailRegistry.SimpleTrail;
        private MiscShaderData BackTrailShader => TrailRegistry.LaserShader;

        //Radius
        private float StartRadius => 4;
        private float EndRadius => 252;
        private float Width => 64;

        //Colors
        private Color FrontCircleStartDrawColor => Color.White;
        private Color FrontCircleEndDrawColor => Color.White;
        private Color BackCircleStartDrawColor => Color.Lerp(Color.White, ColorFunctions.AcidFlame, 0.4f);
        private Color BackCircleEndDrawColor => Color.Lerp(ColorFunctions.AcidFlame, Color.DarkGreen, 0.7f);
        private Vector2[] CirclePos;

        public override void SetDefaults()
        {
            Projectile.width = 252;
            Projectile.height = 252;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            //Points on the circle
            CirclePos = new Vector2[96];
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/IrradiatedNest_Missile_Land");
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            AI_ExpandCircle();
            AI_DustCircle();
        }

        private void AI_ExpandCircle()
        {
            float easedProgess = Easing.InOutCirc(Progress);
            float radius = MathHelper.Lerp(StartRadius, EndRadius, easedProgess);
            DrawCircle(radius);
        }

        private void AI_DustCircle()
        {
            if (!SpawnDustCircle && Timer >= 15)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 rand = Main.rand.NextVector2CircularEdge(EndRadius, EndRadius);
                    Vector2 pos = Projectile.Center + rand;
                    Dust d = Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), Vector2.Zero,
                        newColor: BackCircleStartDrawColor,
                        Scale: Main.rand.NextFloat(0.3f, 0.6f));
                    d.noGravity = true;
                }
                SpawnDustCircle = true;
            }
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
                float radiansToRotateBy = circleProgress * (MathHelper.TwoPi + MathHelper.PiOver4);
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

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            float easedProgess = Easing.OutCubic(Progress);

            //Back Trail   
            DrawMode = 1;
            BeamDrawer.SpecialShader = BackTrailShader;
            BeamDrawer.SpecialShader.UseColor(
                Color.Lerp(BackCircleStartDrawColor, BackCircleEndDrawColor, easedProgess));
            BeamDrawer.SpecialShader.SetShaderTexture(BackTrailTexture);
            BeamDrawer.DrawPixelated(CirclePos, -Main.screenPosition, CirclePos.Length);

            //Front Trail
            DrawMode = 0;
            BeamDrawer.SpecialShader = FrontTrailShader;
            BeamDrawer.SpecialShader.UseColor(Color.Lerp(FrontCircleStartDrawColor, FrontCircleEndDrawColor,
                Easing.OutCirc(Progress)));
            BeamDrawer.SpecialShader.SetShaderTexture(FrontTrailTexture);
            BeamDrawer.DrawPixelated(CirclePos, -Main.screenPosition, CirclePos.Length);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
