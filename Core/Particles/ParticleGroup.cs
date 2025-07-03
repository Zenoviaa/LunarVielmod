using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Stellamod.Core.Particles
{
    public class ParticleGroup : IEnumerable<Particle>
    {
        private List<Particle> _particles;

        public ParticleGroup()
        {
            _particles = new List<Particle>();
        }

        public IEnumerator<Particle> GetEnumerator() => _particles.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _particles.GetEnumerator();

        public void Clear() => _particles.Clear();

        public Particle this[int i] => _particles[i];

        public T NewParticle<T>(Vector2 center, Vector2 velocity, Color newColor = default, float Scale = 1f) where T : Particle
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            T p = ParticleLoader.GetParticle(ParticleUtils.ParticleType<T>()).NewInstance() as T;

            //设置各种初始值
            p.active = true;
            p.color = newColor;
            p.Center = center;
            p.Velocity = velocity;
            p.Scale = Scale;
            p.OnSpawn();

            _particles.Add(p);

            return p;
        }

        public Particle NewParticle(Vector2 center, Vector2 velocity, int type, Color newColor = default, float Scale = 1f)
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            Particle p = ParticleLoader.GetParticle(type).NewInstance();

            //设置各种初始值
            p.active = true;
            p.color = newColor;
            p.Center = center;
            p.Velocity = velocity;
            p.Scale = Scale;
            p.OnSpawn();

            _particles.Add(p);

            return p;
        }

        public void Add(Particle particle)
        {
            _particles.Add(particle);
        }

        public bool Any() => _particles.Count > 0;

        public void UpdateParticles()
        {
            if (Main.netMode == NetmodeID.Server)//不在服务器上运行
                return;

            for (int i = 0; i < _particles.Count; i++)
            {
                var particle = _particles[i];

                if (particle == null)
                    continue;

                particle.Update();
                if (particle.ShouldUpdateCenter())
                    particle.Center += particle.Velocity;

                if (particle.Scale < 0.001f)
                    particle.active = false;

                if (particle.fadeIn > 1000)
                    particle.active = false;

                if (!particle.active)
                    _particles.Remove(particle);
            }

            _particles.RemoveAll(p => p is null || !p.active);
        }

        public void DrawParticles(SpriteBatch spriteBatch)
        {
            ArmorShaderData armorShaderData = null;
            for (int i = 0; i < _particles.Count; i++)
            {
                var particle = _particles[i];
                if (particle == null || !particle.active)
                    continue;

                if (!ParticleUtils.OnScreen(particle.Center - Main.screenPosition))
                    continue;

                if (particle.shader != armorShaderData)
                {
                    spriteBatch.End();
                    armorShaderData = particle.shader;
                    if (armorShaderData == null)
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, default, default, default, Main.GameViewMatrix.TransformationMatrix);
                    else
                    {
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                        particle.shader.Apply(null);
                    }
                }

                particle.Draw(spriteBatch);
            }

            if (armorShaderData != null)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, default, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public void DrawParticlesInUI(SpriteBatch spriteBatch)
        {
            ArmorShaderData armorShaderData = null;
            foreach (var particle in _particles)
            {
                if (!particle.active)
                    continue;

                if (!ParticleUtils.OnScreen(particle.Center))
                    continue;

                if (particle.shader != armorShaderData)
                {
                    spriteBatch.End();
                    armorShaderData = particle.shader;
                    if (armorShaderData == null)
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, default, default, default, Main.UIScaleMatrix);
                    else
                    {
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
                        particle.shader.Apply(null);
                    }
                }

                particle.DrawInUI(spriteBatch);
            }

            if (armorShaderData != null)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, default, default, default, Main.UIScaleMatrix);
            }
        }

        public void DrawParticlesPrimitive()
        {
            foreach (var particle in _particles)
                if (particle.active && particle is IDrawParticlePrimitive p)
                    p.DrawPrimitives();
        }
    }
}
