using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Bases;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;

namespace Stellamod.Visual.Explosions
{
    internal class BasicStaminaExplosion : BaseExplosionProjectile
    {
        int trailMode;
        int rStart = 4;
        int rEnd = 128;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.timeLeft = 24;
            rStart = Main.rand.Next(4, 8);
            rEnd = Main.rand.Next(96, 128);
        }

        protected override float BeamWidthFunction(float p)
        {
            //How wide the trail is going to be
            float trailWidth = MathHelper.Lerp(32, 16, p);
            float fadeWidth = MathHelper.Lerp(0, trailWidth, Easing.SpikeOutCirc(p)) * Main.rand.NextFloat(0.75f, 1.0f);
            return fadeWidth;
        }

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
            return MathHelper.Lerp(rEnd, rStart, Easing.OutCirc(p));
        }

        protected override BaseShader ReadyShader()
        {
            var shader = SimpleTrailShader.Instance;

            //Main trailing texture
            shader.TrailingTexture = TrailRegistry.GlowTrail;

            //Blends with the main texture
            shader.SecondaryTrailingTexture = TrailRegistry.GlowTrail;

            //Used for blending the trail colors
            //Set it to any noise texture
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail;
            shader.PrimaryColor = Color.White;
            shader.SecondaryColor = Color.DarkGreen;
            shader.Speed = 20;

            //Alpha Blend/Additive
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.FillShape = true;
            return shader;
        }
    }
}
