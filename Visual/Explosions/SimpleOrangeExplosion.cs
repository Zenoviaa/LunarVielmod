using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Visual.Explosions
{
    internal class SimpleOrangeExplosion : BaseExplosionProjectile
    {
        int trailMode;
        int rStart = 4;
        int rEnd = 100;

        public float Lifetime = 32;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.timeLeft = 32;
            Lifetime = Projectile.timeLeft;

            rStart = Main.rand.Next(4, 4);
            rEnd = Main.rand.Next(100, 100);

        }
        float timer = 0;
        public override void AI()
        {
            base.AI();
            timer++;
            Projectile.friendly = true;
            if (Projectile.timeLeft == Projectile.timeLeft - 1)
            {

                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }
            }

            AI_DustCircle();

        }

        private float Progress
        {
            get
            {
                float p = timer / Lifetime;
                return MathHelper.Clamp(p, 0, 1);
            }
        }

        private int DrawMode;
        private bool SpawnDustCircle;




        protected override float BeamWidthFunction(float p)
        {
            //How wide the trail is going to be
            float trailWidth = MathHelper.Lerp(32, 16, p);
            float fadeWidth = MathHelper.Lerp(0, trailWidth, Easing.OutCirc(p)) * Main.rand.NextFloat(0.75f, 1.0f);
            return fadeWidth;
        }

        private Color FrontCircleStartDrawColor => Color.White;
        private Color FrontCircleEndDrawColor => Color.Orange;
        private Color BackCircleStartDrawColor => Color.Lerp(Color.White, Color.OrangeRed, 0.4f);
        private Color BackCircleEndDrawColor => Color.Lerp(Color.Orange, Color.DarkGoldenrod, 0.7f);

        protected override Color ColorFunction(float p)
        {
            //Main color of the beam
            Color c;
            c = Color.Lerp(Color.White, Color.Transparent, p);
            return c;
        }

        protected override float RadiusFunction(float p)
        {
            //How large the circle is going to be
            return MathHelper.Lerp(rEnd, rStart, Easing.InOutCirc(p));
        }

        protected override BaseShader ReadyShader()
        {
            float easedProgess = Easing.OutCubic(Progress);

            var shader = SimpleTrailShader.Instance;

            //Main trailing texture
            shader.TrailingTexture = TrailRegistry.GlowTrail;

            //Blends with the main texture
            shader.SecondaryTrailingTexture = TrailRegistry.WhispyTrail;

            //Used for blending the trail colors
            //Set it to any noise texture
            shader.TertiaryTrailingTexture = TrailRegistry.WaterTrail;
            shader.PrimaryColor = Color.Lerp(FrontCircleStartDrawColor, FrontCircleEndDrawColor, Easing.OutCirc(Progress));
            shader.SecondaryColor = Color.Lerp(BackCircleStartDrawColor, BackCircleEndDrawColor, easedProgess);
            shader.Speed = 20;

            //Alpha Blend/Additive
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.FillShape = true;
            return shader;
        }

        private void AI_DustCircle()
        {
            if (!SpawnDustCircle && timer <= 16)
            {
                for (int i = 0; i < 48; i++)
                {
                    Vector2 rand = Main.rand.NextVector2CircularEdge(rEnd, rEnd);
                    Vector2 pos = Projectile.Center + rand;
                    Dust d = Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), Vector2.Zero,
                        newColor: BackCircleStartDrawColor,
                        Scale: Main.rand.NextFloat(0.3f, 0.6f));
                    d.noGravity = true;
                }
                SpawnDustCircle = true;
            }
        }
    }
}
