using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class PlatformZLayerComparer : IComparer<BlackSeaPlatform>
    {
        public int Compare(BlackSeaPlatform x, BlackSeaPlatform y)
        {
            return x.rotatedPosition.Y.CompareTo(y.rotatedPosition.Y);
        }
    }
    public class BlackSeaPlatform
    {
        private readonly Asset<Texture2D> _platformTextureAsset;
        public BlackSeaPlatform(Asset<Texture2D> platformTextureAsset)
        {
            _platformTextureAsset = platformTextureAsset;
            scale = 1f;
            randScale = Main.rand.NextFloat(-0.4f, 0);
        }

        public Vector3 initialPosition;
        public Vector3 rotatedPosition;
        public float rotation;
        public float scale;
        public float randScale;
        public Color color;
        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D textureToDraw = _platformTextureAsset.Value;
            Vector2 drawOrigin = new Vector2(textureToDraw.Width / 2f, textureToDraw.Height / 2f);
            Vector2 drawPosition = new Vector2(rotatedPosition.X, rotatedPosition.Y);
            drawPosition += Main.Camera.Center;
            drawPosition -= screenPos;

            float drawScale = scale + randScale;
            spriteBatch.Draw(textureToDraw, drawPosition, null, color, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }
    }

    public class BlackSeaPlatformManager
    {
        private float _timer;
        private float _oscTimer;
        private UnifiedRandom _random;
        private readonly BlackSeaPlatform[] _platforms;
        private readonly BlackSeaPlatform[] _platformsByZLayer;
        private PlatformZLayerComparer _zLayerComparer;
        public BlackSeaPlatformManager()
        {
            _platforms = new BlackSeaPlatform[Platform_Count];

            Asset<Texture2D>[] assets = GetAssets();
            //Initialize all of our platforms
            for (int i = 0; i < _platforms.Length; i++)
            {
                Asset<Texture2D> asset = assets[Main.rand.Next(0, assets.Length)];
                _platforms[i] = new BlackSeaPlatform(asset);
            }

            xOvalRadius = 196;
            yOvalRadius = 64;
        }
        public BlackSeaPlatformManager(int platformCount)
        {
            _platforms = new BlackSeaPlatform[platformCount];
            _platformsByZLayer = new BlackSeaPlatform[platformCount];
            _zLayerComparer = new PlatformZLayerComparer();
            Asset<Texture2D>[] assets = GetAssets();
            //Initialize all of our platforms
            for (int i = 0; i < _platforms.Length; i++)
            {
                Asset<Texture2D> asset = assets[Main.rand.Next(0, assets.Length)];
                _platforms[i] = new BlackSeaPlatform(asset);
                _platformsByZLayer[i] = _platforms[i];
            }

            xOvalRadius = 196;
            yOvalRadius = 64;
        }

        public float xOvalRadius;
        public float yOvalRadius;

        public const int Platform_Count = 100;
        public const string Platform_FileName = "SingularPlatform_";
        private Asset<Texture2D>[] GetAssets()
        {
            int numUniqueAssets = 7;
            Asset<Texture2D>[] platformTextureAssets = new Asset<Texture2D>[numUniqueAssets];
            //ANaing scheme for the textures is just SingularPlatform_[num]
            for (int i = 0; i < numUniqueAssets; i++)
            {
                string rootTexturePath = this.GetType().DirectoryHere() + $"/{Platform_FileName}";
                string platformTexturePath = $"{rootTexturePath}{i}";
                Asset<Texture2D> platformTextureAsset = ModContent.Request<Texture2D>(platformTexturePath);
                platformTextureAssets[i] = platformTextureAsset;
            }
            return platformTextureAssets;

        }

        public void Update()
        {
            _timer++;
            //_timer = Main.Camera.Center.X / 16f;
            if (NPC.AnyNPCs(ModContent.NPCType<E>()))
            {
                DomainExpansionManager fallSystem = ModContent.GetInstance<DomainExpansionManager>();
                fallSystem.noWings = true;
                fallSystem.inSpace = true;
                fallSystem.hoveringPlatform = true;
                fallSystem.hoverPlatformY = 16000;
            }

            const float revolutionTime = 400;

            //Calculate the radians offset
            float radiansToRotate = _timer / revolutionTime * MathHelper.TwoPi;


            yOvalRadius = 400;
            xOvalRadius = 600;
            _random ??= new UnifiedRandom();
            _random.SetSeed(1337);

            for (int i = 0; i < _platforms.Length; i++)
            {
                BlackSeaPlatform platform = _platforms[i];
                //Calculate the initial position of the platform
                //Every platform would have the same initial position

                if (i > _platforms.Length / 2)
                {
                    float off = _random.NextFloat(0f, 10f);
                    float xRadius = 200f + ExtraMath.Osc(-1000, 1000, 0, off);
                    float yRadius = ExtraMath.Osc(-150f, 0f, 1, off) + ExtraMath.Osc(-1000, 1000, 0f, offset: off);
                    Vector3 initialPosition = new Vector3(xRadius, yRadius, yRadius);
                    platform.initialPosition = initialPosition;
                }
                else
                {
                    Vector3 initialPosition = new Vector3(xOvalRadius, -yOvalRadius / 2f, yOvalRadius);
                    platform.initialPosition = initialPosition;
                }


                //Calculate the new rotation of this point
                //We need to offset the radians
                float completionRatio = (float)i / (float)_platforms.Length;
                float maxRadiansOffset = MathHelper.TwoPi;
                float offset = completionRatio * maxRadiansOffset;
                float rotationRadians = offset + radiansToRotate;

                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(new Vector3(0.1F, 1, 0.5f + ExtraMath.Osc(-0.5f, 0.5f, speed: 0.3f, i)), rotationRadians);

                //Calculate the rotated position of the platform
                platform.rotatedPosition = Vector3.Transform(platform.initialPosition, rotationMatrix);

                //Calculate the scale of this platform
                float zPosition = platform.rotatedPosition.Z + xOvalRadius;
                float zScale = 1f - (zPosition / (xOvalRadius * 2f));
                platform.scale = MathHelper.Lerp(0.2f, 1f, zScale);

                float range = MathHelper.ToRadians(5);
                platform.rotation = (new Vector2(platform.rotatedPosition.X, platform.rotatedPosition.Y) - Vector2.Zero).ToRotation();
                platform.color = Color.Lerp(Color.Black, Color.White, EasingFunction.OutExpo(zScale));
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            if (_platformsByZLayer == null)
                return;

            Array.Sort(_platformsByZLayer, _zLayerComparer);
            for (int i = 0; i < _platformsByZLayer.Length; i++)
            {
                BlackSeaPlatform platform = _platformsByZLayer[i];
                platform.Draw(spriteBatch, screenPos);
            }
        }
    }
}
