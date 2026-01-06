
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Core.Particles
{
    /// <summary>
    /// Base class for a particle
    /// </summary>
    public abstract class BaseParticle : ModTexturedType
    {
        private Asset<Texture2D> _textureAsset;

      
        private Vector2 _center;
        public Vector2 Center
        {
            get
            {
                return _center;
            }
            set
            {
                _center = value;
            }
        }
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
        public Entity parent;

        protected sealed override void Register()
        {
            ModTypeLookup<BaseParticle>.Register(this);
        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
   
        }

        public Asset<Texture2D> GetTexture()
        {
            _textureAsset ??= ModContent.Request<Texture2D>(Texture);
            return _textureAsset;
        }
        public abstract void OnSpawn();
        public abstract void Update();
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
