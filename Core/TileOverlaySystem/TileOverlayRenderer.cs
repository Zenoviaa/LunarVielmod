using Microsoft.Xna.Framework.Input;
using Stellamod.Common.ConsoleMenu;
using Stellamod.Core.ZTileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.IO;
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

        for (int i = 0; i < TileOverlayRenderer.TileOverlays.Length; i++)
        {
            var tileOverlay = TileOverlayRenderer.TileOverlays[i];
            TileOverlayDropdownButton btn = new(tileOverlay.Type);
            _btns.Add(btn);
        }

        float index = 0;
        foreach (var btn in _btns)
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
public class TileOverlaySerializer : TagSerializer<TileOverlaySaveData, TagCompound>
{
    public override TileOverlaySaveData Deserialize(TagCompound tag)
    {
        TileOverlaySaveData deserializedData = new TileOverlaySaveData();
        deserializedData.x = tag.Get<ushort>("x");
        deserializedData.y = tag.Get<ushort>("y");
        deserializedData.overlayType = TileOverlayUtility.NameToType(tag.Get<string>("overlayType"));
        deserializedData.overlayFrame = tag.Get<byte>("overlayFrame");
        return deserializedData;
    }

    public override TagCompound Serialize(TileOverlaySaveData value)
    {
        return new TagCompound
        {
            ["x"] = value.x,
            ["y"] = value.y,
            ["overlayType"] = TileOverlayUtility.TypeToName(value.overlayType),
            ["overlayFrame"] = value.overlayFrame,
        };
    }
}
public struct TileOverlaySaveData
{
    public ushort x;
    public ushort y;
    public byte overlayType;
    public byte overlayFrame;
}

public struct TileOverlayData
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
        if (compareType == 0)
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
        Point point = new Point(x, y);
        TileOverlayData overlayData = new();
        overlayData.overlayType = (byte)Type;
        overlayData.overlayFrame = placer.frame;
        TileOverlayUtility.PlacedTileOverlays[point] = overlayData;
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
        Item.rare = ItemRarityID.Expert;
        Item.useTime = 2;
        Item.useAnimation = 2;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.autoReuse = false;
    }

    public override bool? UseItem(Player player)
    {
        if (Main.myPlayer == player.whoAmI)
        {
            Point tilePoint = Main.MouseWorld.ToTileCoordinates();
            TileOverlayUtility.KillTileOverlay(tilePoint.X, tilePoint.Y);
            if(Main.netMode != NetmodeID.SinglePlayer)
                TileOverlayUtility.SendTileOverlayData(-1, -1, tilePoint.X, tilePoint.Y, 1, 1);
        }

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
        if (Main.myPlayer == player.whoAmI)
        {
            if (player.altFunctionUse == 2)
            {
                TileOverlaySelector.OverlayCenter = Main.MouseScreen;
                TileOverlaySelector selector = ModContent.GetInstance<TileOverlaySelector>();
                selector.ToggleUI();
            }

            Point tilePoint = Main.MouseWorld.ToTileCoordinates();
            byte type = TileOverlayUtility.TileOverlayType<GoldenLeafTileOverlayData>();
            TileOverlayUtility.PlaceTileOverlay(tilePoint.X, tilePoint.Y, type, new TileOverlayPlacer { frame = (byte)Main.rand.Next(20) });
            if (Main.netMode != NetmodeID.SinglePlayer)
                TileOverlayUtility.SendTileOverlayData(-1, -1, tilePoint.X, tilePoint.Y, 1, 1);
        }

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

    public override void ClearWorld()
    {
        base.ClearWorld();
        TileOverlayUtility.PlacedTileOverlays.Clear();
    }

    //Referenced from SLR's system
    public override void SaveWorldData(TagCompound tag)
    {
        base.SaveWorldData(tag);
        int i = 0;
        List<TileOverlaySaveData> overlays = new();
        foreach (var kvp in TileOverlayUtility.PlacedTileOverlays)
        {
            overlays.Add(new TileOverlaySaveData {
                x = (ushort)kvp.Key.X, 
                y = (ushort)kvp.Key.Y, 
                overlayType = kvp.Value.overlayType, 
                overlayFrame = kvp.Value.overlayFrame });
        }

        tag["tData"] = overlays;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        base.LoadWorldData(tag);
        TileOverlayUtility.PlacedTileOverlays.Clear();
        List<TileOverlaySaveData> overlaySaveData = tag.Get<List<TileOverlaySaveData>>("tData");
        foreach (var data in overlaySaveData)
        {
            TileOverlayUtility.PlacedTileOverlays.Add(new Point(data.x, data.y), new TileOverlayData { overlayType = data.overlayType, overlayFrame = data.overlayFrame });
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
                Point p = new Point(x, y);
                if (TileOverlayUtility.PlacedTileOverlays.ContainsKey(p))
                {
                    var overlayData = TileOverlayUtility.PlacedTileOverlays[p];
                    if (overlayData.overlayType != 0)
                    {
                        TileOverlayRenderer.DrawData.Add(new TileOverlayDrawData(x, y, overlayData));
                    }
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

public class TileOverlayPlayer : ModPlayer
{
    public override void OnEnterWorld()
    {
        base.OnEnterWorld();
        if (Main.netMode == NetmodeID.SinglePlayer)
            return;
        if (Main.netMode == NetmodeID.Server)
            return;
        Main.NewText("Request Tile Overlay Data");
        TileOverlayUtility.RequestTileOverlayData();
    }
}

public class TileOverlayUtility : ModSystem
{
    public static readonly Dictionary<Point, TileOverlayData> PlacedTileOverlays = new Dictionary<Point, TileOverlayData>();
    public static void RequestTileOverlayData()
    {
        ModPacket packet = Stellamod.Instance.GetPacket(capacity: 16);
        packet.Write((byte)MessageType.RequestTileOverlayData);
        packet.Send();
    }

    public static void ReceiveTileOverlaySync(BinaryReader reader, int whoAmI)
    {
        List<Point> points = new List<Point>();
        int oX = reader.ReadInt32();
        int oY = reader.ReadInt32();
        int oW = reader.ReadInt32();
        int oH = reader.ReadInt32();
        Rectangle rect = new Rectangle(oX, oY, oW, oH);
        for(int i = rect.Left; i < rect.Right; i++)
        {
            for(int j = rect.Top; j < rect.Bottom; j++)
            {
                Point p = new Point(i, j);
                if (PlacedTileOverlays.ContainsKey(p))
                    PlacedTileOverlays.Remove(p);
            }
        }

        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            int x = reader.ReadUInt16();
            int y = reader.ReadUInt16();
            byte overlapType = reader.ReadByte();
            byte overlayFrame = reader.ReadByte();
            PlacedTileOverlays[new Point(x,y)] = new TileOverlayData { overlayType = overlapType, overlayFrame = overlayFrame };
            points.Add(new(x, y));
        }

        if (Main.netMode == NetmodeID.Server)
        {
            // Forward the changes to the other clients
            SendTileOverlayData(-1, -1, rect, points);
        }
    }
    public static void SendTileOverlayData(int requester, int ignore, Rectangle overlayArea, List<Point> points)
    {
        var packet = Stellamod.Instance.GetPacket();
        packet.Write((byte)MessageType.TileOverlaySync);
        packet.Write(overlayArea.X);
        packet.Write(overlayArea.Y);
        packet.Write(overlayArea.Width);
        packet.Write(overlayArea.Height);
        packet.Write(points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            Point p = points[i];
            var data = PlacedTileOverlays[p];
            packet.Write((ushort)p.X);
            packet.Write((ushort)p.Y);
            packet.Write(data.overlayType);
            packet.Write(data.overlayFrame);
        }
        packet.Send(toClient: requester);
    }

    public static void SendTileOverlayData(int requester, int ignore, int x, int y, int width, int height)
    {
        Rectangle rect = new Rectangle(x, y, width, height);
        List<Point> points = new List<Point>();
        for (int i = rect.Left; i < rect.Right; i++)
        {
            for (int j = rect.Top; j < rect.Bottom; j++)
            {
                Point overlayPoint = new Point(i, j);
                if (PlacedTileOverlays.ContainsKey(overlayPoint))
                    points.Add(overlayPoint);
            }
        }
        SendTileOverlayData(requester, ignore, rect, points);
    }

    public static void HandleRequestPacket(BinaryReader reader, int whoAmI)
    {
        //This should loop over the world, appending points and when it gets to big it cuts it off and goes next
        List<Point> points = new List<Point>();
        for (int x = 0; x < Main.maxTilesX; x++)
        {
            for (int y = 0; y < Main.maxTilesY; y++)
            {
                Point tilePoint = new Point(x, y);
                if (PlacedTileOverlays.ContainsKey(tilePoint))
                {
                    points.Add(tilePoint);
                    if (points.Count >= 2000)
                    {
                        SendTileOverlayData(whoAmI, -1, Rectangle.Empty, points);
                        points.Clear();
                    }
                }
            }
        }
        if(points.Count > 0)
        {
            SendTileOverlayData(whoAmI, -1, Rectangle.Empty, points);
        }
    }

    public static string TypeToName(byte type)
    {
        var tile = TileOverlayRenderer.TileOverlays[type];
        return tile.Name;
    }
    public static byte NameToType(string name) => TileOverlayRenderer.TileOverlayTypeLookup[name];
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
        Point tilePoint = new Point(x, y);
        if (!PlacedTileOverlays.ContainsKey(tilePoint))
            return;
        PlacedTileOverlays.Remove(tilePoint);
    }
}