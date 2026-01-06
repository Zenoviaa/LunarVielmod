using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Core.Grass
{
    /// <summary>
    /// Describes how a patch a grass should be drawn
    /// </summary>
    public abstract class GrassProfile : ModTexturedType
    {
        private UnifiedRandom _random;
        private List<ReedProfile> _reeds;
        public Asset<Texture2D> GrassTextureAsset;
        public int frameCount;
        public float maxHeight;
        public float maxWidth;
        public int maxExtraBladesPerPatch;
        public int minBladesPerPatch;
        public Color grassColor;
        protected sealed override void Register()
        {
            ModTypeLookup<GrassProfile>.Register(this);
        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            _reeds = new List<ReedProfile>();
            SetStaticDefaults();
        }


        public void RegisterReed<T>()where T : ReedProfile
        {
            _reeds.Add(ModContent.GetInstance<T>());
        }
        public List<ReedProfile> GetReedProfiles()
        {
            return _reeds;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            frameCount = 3;
            GrassTextureAsset = ModContent.Request<Texture2D>(Texture);
            maxHeight = 90;
            maxWidth = 4.4f;
            maxExtraBladesPerPatch = 4;
            minBladesPerPatch = 2;
            grassColor = new Color(80, 107, 26);
     
        }


        public Rectangle GetFrame(int frameIndex)
        {
            int frameHeight = GrassTextureAsset.Height() / frameCount;
            Rectangle frame = new Rectangle(0, frameIndex * frameHeight, GrassTextureAsset.Width(), frameHeight);
            return frame;
        }

        public virtual GrassProfile GetVariantProfile(int i, int j)
        {
            return this;
        }

        public virtual void Grow(int i, int j)
        {

            Vector2 worldPosition = new Point(i, j).ToWorldCoordinates();
            GrassRenderer grassRenderer = ModContent.GetInstance<GrassRenderer>();

            float patchNum = (maxExtraBladesPerPatch * ExtraMath.Osc(0f, 1f, 0, i * 0.3f)) + minBladesPerPatch;
            int num = (int)(patchNum * ExtraMath.Osc(0f, 2f, 0, i * 0.6f));

            _random ??= new UnifiedRandom();
            _random.SetSeed(i);
          

            for (int n = 0; n < num; n++)
            {
                Vector2 position = worldPosition;
                position.X += ExtraMath.Osc(-32f, 32f, 0, i + n);
                position.Y += ExtraMath.Osc(0f, 4f, 0, i + n);

                Point tilePoint = position.ToTileCoordinates();
                
                Tile tile = Main.tile[i, j];
               while(!WorldGen.SolidTile(i, j))
                {
                    j++;
                    position.Y += 16;
                }
                int frame = _random.Next(0, frameCount);
                grassRenderer.AddGrass(this, GrassTextureAsset, GetFrame(frame), Color.White, position, -Vector2.UnitY);
      
            }
            grassRenderer.AddGrassPatch(grassColor, worldPosition, -Vector2.UnitY, 100, 2, num);


            /*
            Color lightColor = Lighting.GetColor(i, j);
            Color finalColor = grassColor.MultiplyRGB(lightColor);
            float height = ExtraMath.Osc(0.5f, 1f, 0, i * 3);
            float width = ExtraMath.Osc(0.5f, 1f, 0, i * 3);
            float h = height * maxHeight;
            float w = width * maxWidth;
            worldPosition.Y += 8 * ExtraMath.Osc(0f, 1f, 0, i * 3);

            int num = (int)(maxExtraBladesPerPatch * ExtraMath.Osc(0f, 1f, 0, i * 0.3f)) + minBladesPerPatch;
          //  num *= (int)ExtraMath.Osc(0f, 2f, 0, i * 0.6f);
            grassRenderer.AddGrassPatch(finalColor, worldPosition, -Vector2.UnitY, h, w, num);
            */

        }
    }
}
