using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Effects;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Particles
{
    public abstract class Particle : ModTexturedType
    {
        public int Type { get; internal set; }

        public Vector2 Center;
        public Vector2 Velocity;
        public float fadeIn;
        public float Scale;
        public float Rotation;
        public bool active;
        public bool shouldKilledOutScreen = true;
        public bool isBlack;
        public Color color;
        public Rectangle Frame;
        public ArmorShaderData shader;
        public Shader customShader;

        protected sealed override void Register()
        {

            ModTypeLookup<Particle>.Register(this);
            ParticleLoader.Particles ??= new List<Particle>();
            ParticleLoader.Particles.Add(this);
            Type = ParticleLoader.ReserveParticleID();
        }

        public virtual Particle NewInstance()
        {
            var inst = (Particle)Activator.CreateInstance(GetType(), true);
            inst.Type = Type;
            return inst;
        }

        public static T NewParticle<T>(Vector2 center, Vector2 velocity, Color newColor = default, float Scale = 1f) where T : Particle
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            if (ParticleSystem.Particles.Count > ParticleSystem.MaxParticleCount - 2)
                return null;

            T p = ParticleLoader.GetParticle(ParticleUtils.ParticleType<T>()).NewInstance() as T;
            p.active = true;
            p.color = newColor;
            p.Center = center;
            p.Velocity = velocity;
            p.Scale = Scale;
            p.OnSpawn();

            ParticleSystem.Particles.Add(p);
            return p;
        }

        public static T NewBlackParticle<T>(Vector2 center, Vector2 velocity, Color newColor = default, float Scale = 1f) where T : Particle
        {
            if (Main.netMode == NetmodeID.Server)
                return null;

            if (ParticleSystem.BlackParticles.Count > ParticleSystem.MaxParticleCount - 2)
                return null;

            T p = ParticleLoader.GetParticle(ParticleUtils.ParticleType<T>()).NewInstance() as T;
            p.active = true;
            p.color = newColor;
            p.Center = center;
            p.Velocity = velocity;
            p.Scale = Scale;
            p.OnSpawn();

            p.Scale *= Scale;
            ParticleSystem.BlackParticles.Add(p);
            return p;
        }

        public virtual void OnSpawn() { }

        public virtual void Update() { }

        public virtual bool ShouldUpdateCenter() => true;

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Rectangle frame = Frame;
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(GetTexture().Value, Center - Main.screenPosition, frame, color, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }

        public virtual void DrawInUI(SpriteBatch spriteBatch)
        {
            Rectangle frame = Frame;
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(GetTexture().Value, Center, frame, color, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }

        public Asset<Texture2D> GetTexture() => ParticleSystem.ParticleAssets[Type];
    }
}
