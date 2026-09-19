using Microsoft.Xna.Framework.Input;
using Stellamod.Common.ConsoleMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;

namespace Stellamod.Core.TileOverlaySystem;


public struct TileOverlayPlacer
{
    public byte frame;
}

public class TileOverlayDropdownButton : UIPanel
{
    public TileOverlayDropdownButton(int type)
    {
        Type = type;
    }
    public readonly int Type;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = Height.Pixels = 48;
        OnLeftClick += SelectTileOverlay;
    }

    private void SelectTileOverlay(UIMouseEvent evt, UIElement listeningElement)
    {
        TileOverlaySelector.TileOverlayToPlaceType = (byte)Type;
        var sound = SoundID.MenuTick;
        SoundEngine.PlaySound(sound);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
    }
}

public class TileOverlayDropdownMenuButton : UIPanel
{
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = Height.Pixels = 48;
        OnLeftClick += OpenTileOverlayDropdownMenu;
    }

    private void OpenTileOverlayDropdownMenu(UIMouseEvent evt, UIElement listeningElement)
    {
        TileOverlaySelector.ShowTileOverlayDropdown = !TileOverlaySelector.ShowTileOverlayDropdown;
        var sound = SoundID.MenuTick;
        SoundEngine.PlaySound(sound);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
    }
}

public class TileOverlayUIPanel : UIPanel
{
    private readonly TileOverlayDropdownMenuButton _dropdownBtn;
    private readonly List<TileOverlayDropdownButton> _btns;
    public int RelativeLeft => (int)TileOverlaySelector.OverlayCenter.X;
    public int RelativeTop => (int)TileOverlaySelector.OverlayCenter.Y;
    public TileOverlayUIPanel()
    {
        _dropdownBtn = new();
        _btns = new();
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = Height.Pixels = 252;
        Append(_dropdownBtn);
        
        for(int i = 0; i < TileOverlayRenderer.TileOverlays.Length; i++)
        {
            var tileOverlay = TileOverlayRenderer.TileOverlays[i];
            TileOverlayDropdownButton btn = new(tileOverlay.Type);
            _btns.Add(btn);
        }

        float index = 0;
        foreach(var btn in _btns)
        {
            btn.Top.Pixels = index * 24;
            Append(btn);
            index++;
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
    }
}

public class TileOverlayUIState : UIState
{
    public TileOverlayUIPanel overlayUI;
    public TileOverlayUIState() : base()
    {

    }

    public override void OnInitialize()
    {
        overlayUI = new TileOverlayUIPanel();
        Append(overlayUI);
    }
}

[Autoload(Side = ModSide.Client)]
public class TileOverlaySelector : ModSystem
{
    private GameTime _lastUpdateUiGameTime;
    private UserInterface _userInterface;
    private TileOverlayUIState _uiState;
    public static Vector2 OverlayCenter;
    public static byte TileOverlayToPlaceType;
    public static bool ShowTileOverlayDropdown;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _userInterface = new UserInterface();
        _uiState = new();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        _lastUpdateUiGameTime = gameTime;
        if (_userInterface?.CurrentState != null)
        {
            _userInterface.Update(gameTime);
        }
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
        _userInterface.SetState(_uiState);
    }

    public void CloseUI()
    {
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
                "Stellamod: Tile Overlay UI",
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
public struct TileOverlayData : ITileData
{
    public byte overlayType;
    public byte overlayFrame;
}

public readonly record struct TileOverlayDrawData(int X, int Y, TileOverlayData Data)
{
    public readonly Vector2 WorldPosition = new Point(X, Y).ToWorldCoordinates();
}

//All tile overlays have the same draw layer
//We want to sort by their type because they may have differing draw parameters
//To be better batched it should be sorted

/// <summary>
/// Sorts the tile overlays by their type
/// </summary>
public class TileOverlayDataDataComparerByType : IComparer<TileOverlayDrawData>
{
    public int Compare(TileOverlayDrawData x, TileOverlayDrawData y)
    {
        int compareType = x.Data.overlayType.CompareTo(y.Data.overlayType);
        if(compareType == 0)
        {
            return y.Y.CompareTo(x.Y);
        }
        return compareType;
    }
}

public abstract class TileOverlayType : ModType
{
    public int Type { get; set; }
    public byte FrameCount { get; protected set; }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public sealed override void SetupContent()
    {
        base.SetupContent();
        SetStaticDefaults();
    }

    protected override void Register()
    {
        ModTypeLookup<TileOverlayType>.Register(this);
    }

    public virtual void PlaceTile(in int x, in int y, in TileOverlayPlacer placer)
    {
        ref TileOverlayData overlayData = ref Framing.GetTileSafely(x, y).Get<TileOverlayData>();
        overlayData.overlayType = (byte)Type;
        overlayData.overlayFrame = placer.frame;
    }

    public void PlaceStyleBlock2x2(in int x, in int y, in TileOverlayPlacer placer)
    {

    }
    
    public abstract void BeginSpritebatch(SpriteBatch spriteBatch);
    public abstract void DrawTileOverlay(in TileOverlayDrawData drawData, SpriteBatch spriteBatch);
}


public class GrafittiSponge : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 62;
        Item.height = 32;
        Item.scale = 0.9f;
        Item.rare = ItemRarityID.Expert;
        Item.useTime = 2;
        Item.useAnimation = 2;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.autoReuse = false;
    }

    public override bool? UseItem(Player player)
    {
        Point tilePoint = Main.MouseWorld.ToTileCoordinates();
        TileOverlayUtility.KillTileOverlay(tilePoint.X, tilePoint.Y);
        return true;
    }
}

public class GrafittiCan : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 62;
        Item.height = 32;
        Item.scale = 0.9f;
        Item.rare = ItemRarityID.Expert;
        Item.useTime = 2;
        Item.useAnimation = 2;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.autoReuse = false;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }
    public override bool? UseItem(Player player)
    {
        if(Main.myPlayer == player.whoAmI && player.altFunctionUse == 2)
        {
            TileOverlaySelector.OverlayCenter = Main.MouseScreen;
            TileOverlaySelector selector = ModContent.GetInstance<TileOverlaySelector>();
            selector.ToggleUI();
        }

        Point tilePoint = Main.MouseWorld.ToTileCoordinates();
        byte type = TileOverlayUtility.TileOverlayType<GoldenLeafTileOverlayData>();
        TileOverlayUtility.PlaceTileOverlay(tilePoint.X, tilePoint.Y, type, new TileOverlayPlacer { frame = (byte)Main.rand.Next(20) });
        return true;
    }
}

//Deciding to use structs here as we only need to define how the overlay draws
public class GoldenLeafTileOverlayData : TileOverlayType
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        FrameCount = 20;
    }

    public override void DrawTileOverlay(in TileOverlayDrawData drawData, SpriteBatch spriteBatch)
    {
        var goldenLeafAsset = AssetReferences.Content.Areas.Tundra.Abyss.TilesAB.GoldenLeafOverlay.Asset;
        Rectangle frame = goldenLeafAsset.Value.GetFrame(drawData.Data.overlayFrame, 20, 1);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(goldenLeafAsset, drawData.WorldPosition + new Vector2(8, -16));
        drawer.worldPosition += new Vector2(0, 24);
        drawer.sourceRect = frame;
        drawer.CenterOrigin();
        spriteBatch.Draw(drawer);

    }

    public override void BeginSpritebatch(SpriteBatch spriteBatch)
    {
        SpritebatchParams spritebatchParams = SpritebatchParams.InWorldAndZoomed();
        spriteBatch.Begin(spritebatchParams);
    }
}

public class TileOverlayGlobalTile : GlobalTile
{
    public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        base.DrawEffects(i, j, type, spriteBatch, ref drawData);

    }
   
}
public class TileOverlayRenderer : ModSystem
{
    //This should be ooptimized I think, I believe this sorts when it's enumerated on, not when the elements are added.
    //Profile different data structures later
    public static readonly IComparer<TileOverlayDrawData> Comparer = new TileOverlayDataDataComparerByType();
    public static TileOverlayType[] TileOverlays;
    public static readonly Dictionary<string, byte> TileOverlayTypeLookup = new();
    public static readonly List<TileOverlayDrawData> DrawData = new();
    public override void Load()
    {
        base.Load();
        On_Main.RenderTiles += ResetDustPoints;
        On_Main.DrawDust += DrawTileOverlays;
    }
    public override void PostSetupContent()
    {
        base.PostSetupContent();
        List<TileOverlayType> tileOverlays = new();
        tileOverlays.Add(null);
        Type[] types = AssemblyManager.GetLoadableTypes(Mod.Code);

        foreach (Type type in types.Where(t => t.IsClass && t.IsSubclassOf(typeof(TileOverlayType)) && !t.IsAbstract))
        {
            TileOverlayType overlayType = Activator.CreateInstance(type) as TileOverlayType;
            overlayType.Type = tileOverlays.Count;
            tileOverlays.Add(overlayType);
        }

        TileOverlays = tileOverlays.ToArray();
        for (byte i = 1; i < TileOverlays.Length; i++)
        {
            ref var tileOverlay = ref TileOverlays[i];
            string name = tileOverlay.GetType().Name;
            TileOverlayTypeLookup.Add(name, i);
            Mod.Logger.Info($"Register Tile Overlay {name} with id: {i}");
        }
    }

    private void ResetDustPoints(On_Main.orig_RenderTiles orig, Main self)
    {

        orig(self);
        DrawData.Clear();
        Rectangle drawArea = TileUtilities.GetScreenDrawArea();
        for (int x = drawArea.Left; x < drawArea.Right; x++)
        {
            for (int y = drawArea.Top; y < drawArea.Bottom; y++)
            {
                Tile tile = Main.tile[x, y];
                ref TileOverlayData overlayData = ref tile.Get<TileOverlayData>();
                if (overlayData.overlayType != 0)
                {
                    TileOverlayRenderer.DrawData.Add(new TileOverlayDrawData(x, y, overlayData));
                }
            }
        }
        //  Main.NewText(drawArea);

        if (DrawData.Count > 0)
        {
            DrawData.Sort(Comparer);
        }

    }

    private void DrawTileOverlays(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (DrawData.Count <= 0)
            return;

        DrawTileOverlays(Main.spriteBatch);
    }


    private void DrawTileOverlays(SpriteBatch spriteBatch)
    {
        byte lastType = 0;
        foreach (TileOverlayDrawData drawData in DrawData)
        {
            byte overlayType = drawData.Data.overlayType;
            var data = TileOverlays[overlayType];
            if (overlayType != lastType)
            {
                if (spriteBatch.beginCalled)
                    spriteBatch.End();
                data.BeginSpritebatch(spriteBatch);
                lastType = overlayType;
            }

            data.DrawTileOverlay(drawData, spriteBatch);
        }

        if (spriteBatch.beginCalled)
        {
            spriteBatch.End();
        }
    }

    private void DrawPixelatedTileOverlays(SpriteBatch sb, Vector2 screenPos)
    {

    }


}

public static class TileOverlayUtility
{
    public static byte TileOverlayType<T>() where T : TileOverlayType
    {
        string name = typeof(T).Name;

        return TileOverlayRenderer.TileOverlayTypeLookup[name];
    }
    public static void PlaceTileOverlay(int x, int y, byte overlayType, TileOverlayPlacer placer)
    {
        var tileToPlace = TileOverlayRenderer.TileOverlays[overlayType];
        tileToPlace.PlaceTile(x, y, placer);

    }

    public static void KillTileOverlay(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        ref TileOverlayData overlayData = ref tile.Get<TileOverlayData>();
        overlayData.overlayType = 0;
    }
}