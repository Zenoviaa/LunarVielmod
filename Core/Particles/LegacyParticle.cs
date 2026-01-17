
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Particles
{
    /// <summary>
    /// To be deprecated, we need to update all particles to the new system
    /// </summary>
    public abstract class LegacyParticle : ModTexturedType
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
        public BaseShader customShader;
        public override void Unload()
        {
            base.Unload();
            customShader = null;
            shader = null;
        }

        protected sealed override void Register()
        {
            ModTypeLookup<LegacyParticle>.Register(this);
            ParticleLoader.Particles ??= new List<LegacyParticle>();
            ParticleLoader.Particles.Add(this);
            Type = ParticleLoader.ReserveParticleID();
        }

        public virtual LegacyParticle NewInstance()
        {
            var inst = (LegacyParticle)Activator.CreateInstance(GetType(), true);
            inst.Type = Type;
            return inst;
        }

        public static T NewParticle<T>(Vector2 center, Vector2 velocity, Color newColor = default, float Scale = 1f) where T : LegacyParticle
        {
            T p = ParticleLoader.GetParticle(ParticleUtils.ParticleType<T>()).NewInstance() as T;
            if (Main.netMode != NetmodeID.Server)
            {
                p.active = true;
                p.color = newColor;
                p.Center = center;
                p.Velocity = velocity;
                p.Scale = Scale;
                p.OnSpawn();

                ParticleSystem.AddParticle(p);
            }
  
            return p;
        }

        public static T NewBlackParticle<T>(Vector2 center, Vector2 velocity, Color newColor = default, float Scale = 1f) where T : LegacyParticle
        {
            T p = ParticleLoader.GetParticle(ParticleUtils.ParticleType<T>()).NewInstance() as T;
            if (Main.netMode != NetmodeID.Server)
            {
                p.active = true;
                p.color = newColor;
                p.Center = center;
                p.Velocity = velocity;
                p.Scale = Scale;
                p.OnSpawn();
                ParticleSystem.AddBlackParticle(p);
            }
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

        public Asset<Texture2D> GetTexture() => ParticleSystem.ParticleAssets[Type];

        public string GetShaderPath()
        {
            if (customShader != null)
                return customShader.EffectPath;
            return string.Empty;
        }
    }
}
