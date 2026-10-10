using Stellamod.Content.Biomes;
using Stellamod.Core.DungeonFogSystem;
using Stellamod.WorldG;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss;

public class StarHouseFog : DungeonFogType
{
    private float _alpha;
    Rectangle rectangle
    {
        get
        {

            Rectangle rectangle = SavedGenerationParameters.StarrHouseRectangle;
            rectangle.Width *= 16;
            rectangle.Height *= 16;
            rectangle.Location = rectangle.Location.ToWorldCoordinates().ToPoint();

            return rectangle;
        }
    }
    private bool ShouldRenderFog()
    {
        if (!Main.LocalPlayer.ZoneAbyss)
        {
            return false;
        }

        if (rectangle.Contains(Main.LocalPlayer.Center.ToPoint()))
            return false;


        return true;
    }
    protected override void PrepareFogRectangle(List<DungeonFog> fogRectangles)
    {
        _alpha = MathHelper.Lerp(_alpha, ShouldRenderFog() ? 1 : 0, 0.03f);
        if (_alpha < 0.02f)
            return;

        //rectangle = rectangle.CenterPad(-128);
        fogRectangles.Add(new DungeonFog(rectangle, _alpha));
    }
}
