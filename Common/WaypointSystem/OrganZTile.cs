using Stellamod.Core.ZTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.WaypointSystem;

public abstract class OrganZTile : ZTile
{
    protected OrganWaypointTracker WaypointTracker => ModContent.GetInstance<OrganWaypointTracker>();
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        interactable = true;
    }

    public virtual OrganWaypoint GetWaypoint()
    {
        return OrganWaypoint.Desert;
    }

    public virtual bool IsActivated()
    {
        return WaypointTracker.GetWaypoint(GetWaypoint());
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        if (!IsActivated())
        {
            drawParams.tileData.value += 175;
        }
        drawParams.tileData.value += (byte)WaypointTracker.darknessAnimation;

        base.Draw(spriteBatch, screenPos, drawParams);
    }

    public override void RightClick(Point tilePoint)
    {
        base.RightClick(tilePoint);
        OrganWaypoint waypoint = GetWaypoint();
        if (!WaypointTracker.GetWaypoint(waypoint))
        {
            Vector2 worldCoordinates = tilePoint.ToWorldCoordinates();
            worldCoordinates.Y -= 64;
            WaypointTracker.ActivateWaypoint(waypoint, worldCoordinates);
            return;
        }

        WaypointSystem wayPointSystem = ModContent.GetInstance<WaypointSystem>();
        wayPointSystem.ToggleUI();
    }
    public override (int, int) GetBounds()
    {
        return base.GetBounds();
    }
}
public class MistyDungeonOrgan : OrganZTile
{
    public override OrganWaypoint GetWaypoint()
    {
        return OrganWaypoint.Moonspiral;
    }

    public override (int, int) GetBounds()
    {
        return (224, 168);
    }
}


public class MoonSpiralTowerOrgan : OrganZTile
{
    public override OrganWaypoint GetWaypoint()
    {
        return OrganWaypoint.Moonspiral;
    }

    public override (int, int) GetBounds()
    {
        return (178, 162);
    }
}

public class MarshOrgan : OrganZTile
{
    public override OrganWaypoint GetWaypoint()
    {
        return OrganWaypoint.Marsh;
    }

    public override (int, int) GetBounds()
    {
        return (168, 162);
    }
}

public class WitchTownOrgan : OrganZTile
{
    public override OrganWaypoint GetWaypoint()
    {
        return OrganWaypoint.WitchTown;
    }

    public override (int, int) GetBounds()
    {
        return (146, 162);
    }
}

public class DesertOrgan : OrganZTile
{
    public override OrganWaypoint GetWaypoint()
    {
        return OrganWaypoint.Desert;
    }

    public override (int, int) GetBounds()
    {
        return (146, 162);
    }
}
