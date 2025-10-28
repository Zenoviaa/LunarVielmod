using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia
{
    public class WindStormParticle
    {
        public float timer;
        public Vector2 position;
        public Vector2 velocity;
        public Vector2[] oldPosition;
        public int trailCacheLength;
        public void Update(Vector2 vortexOriginPoint)
        {
            timer++;
            if(oldPosition == null || oldPosition.Length != trailCacheLength)
                oldPosition = new Vector2[trailCacheLength];
            for (int i = trailCacheLength - 1; i > 0; i--)
            {
                oldPosition[i] = oldPosition[i - 1];
            }

            oldPosition[0] = position;
            position += velocity;

            float distanceToTarget = Vector2.Distance(position, vortexOriginPoint);
            if(distanceToTarget > 1000)
            {
                position = vortexOriginPoint;
            }
            if(distanceToTarget > 8)
            {
                Vector2 targetPoint = new Vector2(position.X, vortexOriginPoint.Y);
                velocity = ProjectileHelper.SimpleHomingVelocity(position, vortexOriginPoint, velocity, degreesToRotate: Main.rand.NextFloat(12, 24));
            }
        }
    }

    public class WindStorm
    {
        private float[] _oldRot;
        private WindStormParticle[] _particles;
        public WindStorm(int numParticles)
        {
            this.numParticles = numParticles;
            _particles = new WindStormParticle[numParticles];
            for(int i = 0; i < _particles.Length; i++)
            {
                _particles[i] = new WindStormParticle();
                _particles[i].timer = Main.rand.NextFloat(0, 90);
                _particles[i].velocity = Main.rand.NextVector2CircularEdge(20, 20);
                _particles[i].trailCacheLength = 16;
            }
            _oldRot = new float[16];
        }
        public int numParticles;       
        public  void Update(Vector2 vortexOriginPoint)
        {
            for(int i = 0; i < _particles.Length; i++)
            {
                WindStormParticle particle = _particles[i];
                particle.Update(vortexOriginPoint);
            }
        }

        public void Draw()
        {
            for (int i = 0; i < _particles.Length; i++)
            {
                WindStormParticle particle = _particles[i];
                DrawWindTrail(particle.oldPosition, _oldRot);
            }
        }

        protected virtual void DrawWindTrail(Vector2[] oldPos, float[] oldRot)
        {
            var shader = MagicRadianceShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.CloudsSmall;
            shader.OutlineTexture = TrailRegistry.DottedTrailOutline;
            shader.PrimaryColor = Color.Lerp(Color.White, Color.LightGray, 0.5f);
            shader.NoiseColor = Color.LightGray;
            shader.OutlineColor = Color.Transparent;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 5.2f;
            shader.Distortion = 0.15f;
            shader.Power = 0.25f;

            //This just applis the shader changes

            //Main Fill
            TrailDrawer.Draw(Main.spriteBatch, oldPos, oldRot, StripColors, StripWidth, shader);
        }

        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.LightGray, Color.White,
                Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            return result * 0.5f;
        }

        private float StripWidth(float progressOnStrip)
        {
            return 8;
        }
    }
}
