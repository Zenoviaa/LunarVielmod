using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.PunkerTown.TilesPT;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Grass
{
    /// <summary>
    /// Describes how a patch a grass should be drawn
    /// </summary>
    public abstract class GrassProfile : ModType
    {
        protected sealed override void Register()
        {
            ModTypeLookup<GrassProfile>.Register(this);
        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            maxHeight = 90;
            maxWidth = 4.4f;
            maxExtraBladesPerPatch = 4;
            minBladesPerPatch = 2;
            grassColor = new Color(80, 107, 26);
        }


        public float maxHeight;
        public float maxWidth;
        public int maxExtraBladesPerPatch;
        public int minBladesPerPatch;
        public Color grassColor;
        public virtual void Grow(int i, int j)
        {
            Vector2 worldPosition = new Point(i, j).ToWorldCoordinates();
            GrassRenderer grassRenderer = ModContent.GetInstance<GrassRenderer>();

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


        }
    }
}
