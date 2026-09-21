using Stellamod.Content.Biomes;
using Stellamod.Core.DungeonFogSystem;
using Stellamod.WorldG;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss;

public class StarHouseFog : DungeonFogType
{
    private float _alpha;
    private bool ShouldRenderFog()
    {
        if (!Main.LocalPlayer.ZoneAbyss)
        {
            return false;
        }
        return true;
    }
    protected override void PrepareFogRectangle(List<DungeonFog> fogRectangles)
    {
        _alpha = MathHelper.Lerp(_alpha, ShouldRenderFog() ? 1 : 0, 0.03f);
        if (_alpha < 0.02f)
            return;

        Rectangle rectangle = SavedGenerationParameters.StarrHouseRectangle;
        rectangle.Width *= 16;
        rectangle.Height *= 16;
        rectangle.Location = rectangle.Location.ToWorldCoordinates().ToPoint();

        Rectangle closeRect = rectangle.CenterPad(rectangle.Width, rectangle.Height);
        if (!closeRect.Contains(Main.LocalPlayer.Center.ToPoint()))
            return;


        //rectangle = rectangle.CenterPad(-128);
        fogRectangles.Add(new DungeonFog(rectangle, _alpha));
    }
}
