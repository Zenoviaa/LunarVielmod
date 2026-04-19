using Stellamod.Common.UI;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.WaypointSystem;

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

        //Placeholder debug code, remove this later.
        if (Main.playerInventory && _userInterface.CurrentState == null)
        {
            OpenUI();
        }
        else if (!Main.playerInventory && _userInterface.CurrentState != null)
        {
            CloseUI();
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
            SoundStyle soundStyle = SoundID.MenuOpen;
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
