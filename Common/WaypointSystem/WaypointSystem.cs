using Stellamod.Assets;
using Stellamod.Common.UI;
using Stellamod.Core.ZTileSystem;
using Stellamod.UI;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Stellamod.Common.WaypointSystem;

public enum OrganWaypoint : byte
{
    WitchTown = 0,
    Marsh = 1,
    Desert = 2,
    Moonspiral = 3
}

public class OrganWaypointTracker : ModSystem
{
    public bool[] locations;
    public override void Load()
    {
        base.Load();
        locations = new bool[20];
    }
    public override void Unload()
    {
        base.Unload();
        locations = null;
    }

    public ref bool GetWaypoint(OrganWaypoint waypoint)
    {
        int index = (int)waypoint;
        return ref locations[index];    
    }
    
    public void ActivateWaypoint(OrganWaypoint waypoint)
    {
        int index = (int)waypoint;
        locations[index] = true;
        SoundStyle activateSound = AssetRegistry.Sounds.Waypoint.WaypointActivate;
        SoundEngine.PlaySound(activateSound);
    }

    public void ResetWaypoints()
    {
        for(int i = 0; i < locations.Length; i++)
        {
            locations[i] = false;
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        int length = locations.Length;
        writer.Write(length);
        for(int i = 0; i < length; i++)
        {
            writer.Write(locations[i]);
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        int length = reader.ReadInt32();    
        for(int i = 0; i < length; i++)
        {
            locations[i] = reader.ReadBoolean();
        }
    }
    public override void SaveWorldData(TagCompound tag)
    {
        base.SaveWorldData(tag);
        tag["locations"] = locations;
    }
    public override void LoadWorldData(TagCompound tag)
    {
        base.LoadWorldData(tag);
        bool[] savedLocations = tag.Get<bool[]>("locations");
        if (savedLocations != null)
        {
            locations = savedLocations;
        }
    }
}
public abstract class OrganZTile : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        interactable = true;
    }
    public virtual bool IsActivated()
    {
        return true;
    }

    public override void RightClick()
    {
        base.RightClick();
        //   Main.NewText("yay");
        WaypointSystem wayPointSystem = ModContent.GetInstance<WaypointSystem>();
        wayPointSystem.ToggleUI();
    }
    public override (int, int) GetBounds()
    {
        return base.GetBounds();
    }
}

public class MoonSpiralTowerOrgan : OrganZTile
{
    public override (int, int) GetBounds()
    {
        return (146, 162);
    }
}

public class MarshOrgan : OrganZTile
{
    public override (int, int) GetBounds()
    {
        return (146, 162);
    }
}

public class WitchTownOrgan : OrganZTile
{
    public override (int, int) GetBounds()
    {
        return (146, 162);
    }
}

public class DesertOrgan : OrganZTile
{
    public override (int, int) GetBounds()
    {
        return (146, 162);
    }
}


public class WaypointUI : UIPanel
{
    private UIImage _background;
    public WaypointUI()
    {
        _background = new UIImage(ModContent.Request<Texture2D>(WaypointSystem.AssetPath("WaypointBackground")));
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        Width.Pixels = 394;
        Height.Pixels = 272;
        Append(_background);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Vector2 pxOffset = UIHelpers.ScreenOffset(
            new Vector2(Width.Pixels, Height.Pixels),
            normalizedOrigin: new Vector2(0.5f),
            offset: new Vector2(0, -64));
        Left.Pixels = pxOffset.X;
        Top.Pixels = pxOffset.Y;
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
    }
}

public class WaypointUIState : UIState
{
    public WaypointUI ui;
    public WaypointUIState() : base()
    {

    }

    public override void OnInitialize()
    {
        ui = new WaypointUI();
        Append(ui);
    }
}

[Autoload(Side = ModSide.Client)]
public class WaypointSystem : BaseUISystem
{
    private GameTime _lastUpdateUiGameTime;
    private UserInterface _userInterface;
    public WaypointUIState uiState;
    public override int uiSlot => Slot_MajorUI;

    /// <summary>
    /// Gets an asset path local to the waypoint system's assets
    /// </summary>
    /// <param name="localPath"></param>
    /// <returns></returns>
    public static string AssetPath(string localPath)
    {
        string rootPath = $"Stellamod/Common/WaypointSystem/UI/";
        string combinedPath = rootPath + localPath;
        return combinedPath;
    }

    public override void OnModLoad()
    {
        base.OnModLoad();
        _userInterface = new UserInterface();
        uiState = new();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        _lastUpdateUiGameTime = gameTime;
        if (_userInterface.CurrentState != null)
        {
            _userInterface.Update(gameTime);
        }
    }

    public override void CloseThis()
    {
        base.CloseThis();
        CloseUI();
    }

    public void ToggleUI()
    {
        if (_userInterface.CurrentState != null)
        {
            SoundStyle soundStyle = SoundID.MenuClose;
            SoundEngine.PlaySound(soundStyle);
            CloseUI();
        }
        else
        {
            SoundStyle soundStyle = AssetRegistry.Sounds.Waypoint.OpenWaypointSection;
            SoundEngine.PlaySound(soundStyle);
            OpenUI();
        }
    }

    public void OpenUI()
    {
        //Set State
        TakeSlot();
        _userInterface.SetState(uiState);
    }

    public void CloseUI()
    {
        ClearSlot();
        _userInterface.SetState(null);
    }

    public override void PreSaveAndQuit()
    {
        //Calls Deactivate and drops the item
        if (_userInterface.CurrentState != null)
        {
            CloseUI();
            _userInterface.SetState(null);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "Stellamod: Waypoint UI",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                    {
                        _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    return true;
                },
                InterfaceScaleType.UI));
        }
    }
}
