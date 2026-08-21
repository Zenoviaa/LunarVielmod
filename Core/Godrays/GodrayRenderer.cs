
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Threading;
using Stellamod.Common.Shaders;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static Stellamod.Content.Areas.Illuria.BossesIL.EStyr.BlackStarParticleManager;

namespace Stellamod.Core.Godrays
{
    /// <summary>
    /// Contains a position and two colors and a texture coordinate
    /// </summary>
    public struct GodrayVertex : IVertexType
    {
        private Vector3 _position;
        private Vector4 _color;
        private Vector4 _color2;
        private Vector2 _texCoord;
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
            new VertexElement(28, VertexElementFormat.Vector4, VertexElementUsage.Color, 1),
            new VertexElement(44, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );
        public GodrayVertex(Vector3 position, Color color, Color color2, Vector2 textureCoordinate)
        {
            _position = position;
            _color = color.ToVector4();
            _color2 = color2.ToVector4();
            _texCoord = textureCoordinate;
        }
        public GodrayVertex(Vector2 position, Color color, Color color2, Vector2 textureCoordinate) : this(new Vector3(position.X, position.Y, 0), color, color2, textureCoordinate)
        { 
        }
        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public Vector4 Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public Vector4 Color2
        {
            get { return _color2; }
            set { _color2 = value; }
        }

        public Vector2 TextureCoordinate
        {
            get { return _texCoord; }
            set { _texCoord = value; }
        }
    }

    public struct GodrayParticle
    {
        public Color innerColor;
        public Color outerColor;
        public Vector2 position;
        public float time;
        public bool active;
    }

    /// <summary>
    /// Handles rendering godray particles
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class GodrayRenderer : ModSystem
    {
        private int _lastIndex;
        private int _godrayIndex;
        private int _primitiveCount;
        private int[] _indexBuffer;
        private GodrayVertex[] _vertexBuffer;
        private GodrayVertex[] _drawVertexBuffer;
        private GodrayParticle[] _particles;
        public const int Max_Particle_Count = 50;
        public float godrayTime => 240f;
        public float intensity => 0.2f;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _vertexBuffer = new GodrayVertex[Max_Particle_Count * 4];
            _drawVertexBuffer = new GodrayVertex[_vertexBuffer.Length];
            _indexBuffer = new int[Max_Particle_Count * 6];
            int connectIndex = 0;
            for (int i = 0; i < _indexBuffer.Length; i += 6)
            {
                _indexBuffer[i] = connectIndex + 0;
                _indexBuffer[i + 1] = connectIndex + 2;
                _indexBuffer[i + 2] = connectIndex + 3;
                _indexBuffer[i + 3] = connectIndex + 0;
                _indexBuffer[i + 4] = connectIndex + 1;
                _indexBuffer[i + 5] = connectIndex + 3;
                connectIndex += 4;
            }

            _particles = new GodrayParticle[Max_Particle_Count];
        }

        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            LunarVeilClientConfig config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.Godrays)
                return;
            UpdateParticles();
        }
        public override void PostDrawTiles()
        {
            base.PostDrawTiles();
            if (_godrayIndex <= 0)
                return;
            LunarVeilClientConfig config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.Godrays)
                return;
            PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedGodrays, DrawLayer.OverPlayers);
        }

        private int FindFreeGodrayParticle()
        {
           for(int i = 0; i <  Max_Particle_Count; i++)
            {
                _lastIndex++;
                _lastIndex = _lastIndex % Max_Particle_Count;
                if (!_particles[_lastIndex].active)
                    return _lastIndex;
            }
            return -1;
        }
        public void AddGodrayParticle(Vector2 position, Color innerColor, Color outerColor)
        {
            int nextIdnex = FindFreeGodrayParticle();
            if (nextIdnex == -1)
                return;
    
            ref GodrayParticle particle = ref _particles[nextIdnex];
            particle.position = position;
            particle.innerColor = innerColor;
            particle.outerColor = outerColor;
            particle.time = 0;
            particle.active = true;
        }
        public void AddGodrayParticle(Vector2 position)
        {
            int nextIdnex = FindFreeGodrayParticle();
            if (nextIdnex == -1)
                return;

            ref GodrayParticle particle = ref _particles[nextIdnex];
            particle.position = position;
            particle.innerColor = Color.Lerp(SunLightManager.SunColor, Color.White, 0.5f);
            particle.outerColor = SunLightManager.SunColor;
            particle.time = 0;
            particle.active = true;
        }
        private void RenderPixelatedGodrays(GraphicsDevice graphicsDevice)
        {
            if (_godrayIndex <= 0)
                return;

            GlowingGodrayShader godrayShader = GlowingGodrayShader.Instance;
            godrayShader.ApplyPasses();
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DrawUserIndexedPrimitives<GodrayVertex>(PrimitiveType.TriangleList, _drawVertexBuffer, 0, _godrayIndex, 
                _indexBuffer, 0, _godrayIndex / 2);
        }

        private void UpdateParticles()
        {
            _godrayIndex = 0;
            _primitiveCount = 0;

            Vector2 shadowDirection = SunLightManager.ShadowDirection;
            float rotation = shadowDirection.ToRotation();
            for (int i = 0; i < Max_Particle_Count; i++)
            {
                ref GodrayParticle particle = ref _particles[i];
                if (!particle.active)
                    continue;
                particle.time++;
                if(particle.time >= godrayTime)
                {
                    particle.active = false;
                }


                float width = ExtraMath.Osc(0, 1f, 0, i);
                float lengthModifier = ExtraMath.Osc(0.5f, 1f, 0, i);
                CalculateVertices(i, particle.position, 2000 * lengthModifier, 90 * width + 16, rotation);
                PushVertices(i);
                _primitiveCount += 2;
              
            }
        }
        private void PushVertices(int index)
        {
            int startIndex = index * 4;

            _drawVertexBuffer[_godrayIndex++] = _vertexBuffer[startIndex];
            _drawVertexBuffer[_godrayIndex++] = _vertexBuffer[startIndex+1];
            _drawVertexBuffer[_godrayIndex++] = _vertexBuffer[startIndex+2];
            _drawVertexBuffer[_godrayIndex++] = _vertexBuffer[startIndex+3];
        }


        private void CalculateVertices(int index, Vector2 center, float length, float width, float rotation = 0)
        {
            float halfLength = length * 0.5f;
            float halfWidth = width * 0.5f;
            Vector2 topLeftOffset = new Vector2(-halfLength, -halfWidth);
            Vector2 bottomLeftOffset = new Vector2(-halfLength, halfWidth);
            Vector2 topRightOffset = topLeftOffset + new Vector2(halfLength, -halfWidth);
            Vector2 bottomRightOffset = bottomLeftOffset + new Vector2(halfLength, halfWidth);

            topLeftOffset = topLeftOffset.RotatedBy(rotation);
            bottomLeftOffset = bottomLeftOffset.RotatedBy(rotation);
            topRightOffset = topRightOffset.RotatedBy(rotation);
            bottomRightOffset = bottomRightOffset.RotatedBy(rotation);

            Vector2 topLeft = center + topLeftOffset;
            Vector2 bottomLeft = center + bottomLeftOffset;
            Vector2 topRight = center + topRightOffset;
            Vector2 bottomRight = center + bottomRightOffset;


            int startIndex = index * 4;
            GodrayParticle godrayParticle = _particles[index];
            float bump = EasingFunction.QuadraticBump(godrayParticle.time / godrayTime);
            Color innerColor = Color.Lerp(Color.Black, godrayParticle.innerColor, bump);
            Color outerColor = Color.Lerp(Color.Black, godrayParticle.outerColor, bump);
            innerColor *= intensity;
            outerColor *= intensity;
            innerColor *= LightingHelper.DayLightEase;
            outerColor *= LightingHelper.DayLightEase;

            _vertexBuffer[startIndex] = new GodrayVertex(topLeft, innerColor, outerColor, new Vector2(0, 0));
            _vertexBuffer[startIndex + 1] = new GodrayVertex(topRight, innerColor, outerColor, new Vector2(1, 0));
            _vertexBuffer[startIndex + 2] = new GodrayVertex(bottomLeft, innerColor, outerColor, new Vector2(0, 1));
            _vertexBuffer[startIndex + 3] = new GodrayVertex(bottomRight, innerColor, outerColor, new Vector2(1, 1));
        }
    }
}
