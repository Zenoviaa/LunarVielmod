using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Core.Helpers.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.UI;

namespace Stellamod.Core.Map.UI
{
    internal class MapFog : UIElement
    {
        private float _timer;
        private Dictionary<Rectangle, Func<bool>> _areaVisibilities;
        private List<Rectangle> _particleSpawnBounds;
        private List<MapFogParticle> _particles;
        public MapFog() : base()
        {
            _particleSpawnBounds = new List<Rectangle>();
            _areaVisibilities = new Dictionary<Rectangle, Func<bool>>();
            _particles = new List<MapFogParticle>();
        }

        public float timeBetweenParticles;
        private void SetParticleSpawnBounds()
        {

            _particleSpawnBounds.Clear();
            foreach (var kvp in _areaVisibilities)
            {
                if (kvp.Value())
                {
                    _particleSpawnBounds.Add(kvp.Key);
                }
            }
        }


        public void AddHiddenArea(Rectangle rectangle, Func<bool> isVisible)
        {
            _areaVisibilities.TryAdd(rectangle, isVisible);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            timeBetweenParticles = 0.05f;
            foreach (var particle in _particles)
            {
                particle.Update(gameTime);
            }
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer >= timeBetweenParticles)
            {
                //Spawn Them
                SetParticleSpawnBounds();
                foreach (Rectangle rectangle in _particleSpawnBounds)
                {
                    MapFogParticle particle = new MapFogParticle();
                    particle.position = new Vector2(Main.rand.Next(0, rectangle.Width), Main.rand.Next(0, rectangle.Height));
                    particle.position += rectangle.Location.ToVector2();
                    particle.position += Main.rand.NextVector2Circular(8, 8);
                    particle.velocity = Main.rand.NextVector2Circular(1, 1);
                    particle.rotation = Main.rand.NextFloat(3);
                    particle.scale = Vector2.One * Main.rand.NextFloat(0.35f, 0.7f);
                    particle.duration = Main.rand.NextFloat(3f, 4f);
                    particle.startColor = Color.White * 0.2f;
                    _particles.Add(particle);
                }
                _timer = 0;
            }
            _particles = _particles.Where(x => x.timer < x.duration).ToList();
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            foreach (var particle in _particles)
            {
                particle.Draw(spriteBatch);
            }
        }
    }

    internal class MapFogParticle
    {
        public static Asset<Texture2D> texture;
        public float timer;
        public Vector2 position;
        public Vector2 scale;
        public Vector2 velocity;
        public Color startColor;
        public Color color;
        public float duration;
        public float rotation;

        public MapFogParticle()
        {
            //Default Values
            texture ??= AssetRegistry.Textures.Noise.Clouds3;
            position = Vector2.Zero;
            scale = Vector2.One;
            velocity = Vector2.Zero;
            color = Color.Transparent;
            startColor = Color.White;
            duration = 30;
            rotation = 0;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer += dt;
            float progress = timer / duration;
            float easedColorProgress = EasingFunction.QuadraticBump(progress);
            Color newColor = Color.Lerp(Color.Transparent, startColor, easedColorProgress);
            // newColor.A = 0;
            color = newColor;
            position += dt * velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color myCOlor = color;
            myCOlor.A = 0;
            spriteBatch.Draw(texture.Value, position, null, myCOlor, rotation, texture.Size() / 2, scale, SpriteEffects.None, 0);
        }
    }
}
