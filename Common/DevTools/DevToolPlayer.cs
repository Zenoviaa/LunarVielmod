using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Common.ItemBrowser;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.TriggersSystem;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static System.Formats.Asn1.AsnWriter;

namespace Stellamod.Common.DevTools;

public enum FreeCamera : byte
{
    Normal,
    Free,
    Freeze
}

public enum GodMode : byte
{
    Disabled,
    NoDeath,
    NoHit
}

public class NPCSpawner : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.DebugWand.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.rare = ItemRarityID.White;
        Item.useTime = 4;
        Item.useAnimation = 4;
        Item.useStyle = ItemUseStyleID.Swing;
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
                ModContent.GetInstance<DevToolsUISystem>().ToggleNPCMenuUI();
            }
            else
            {
                NPCUtilities.SpawnNPCFromClient(Main.MouseWorld, DevTools.spawnNPCType);
            }
        
        }

        return true;
    }
}

public class LightHack : GlobalWall
{
    public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, type, ref r, ref g, ref b);
        if (DevTools.LightIntensity <= 0)
            return;
        r = MathHelper.Clamp(r + DevTools.LightIntensity, 0f, 1f);
        g = MathHelper.Clamp(g + DevTools.LightIntensity, 0f, 1f);
        b = MathHelper.Clamp(b + DevTools.LightIntensity, 0f, 1f);

    }
}
public class DevToolHotbarButton : UIPanel
{
    public DevToolHotbarButton(
        Asset<Texture2D> buttonIconTexture,
        Func<(int frame, int frameCount)> getFrameFunc,
        Action toggleFunction,
        Func<Color> getDrawColor = null)
    {
        ButtonIconTexture = buttonIconTexture;
        GetFrameFunction = getFrameFunc;
        ToggleFunction = toggleFunction;
        GetDrawColorFunction = getDrawColor;
    }
    public readonly Asset<Texture2D> ButtonIconTexture;
    public readonly Func<(int frame, int frameCount)> GetFrameFunction;
    public readonly Action ToggleFunction;
    public readonly Func<Color> GetDrawColorFunction;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 32;
        Height.Pixels = 32;
        OnLeftClick += Toggle;
    }

    private void Toggle(UIMouseEvent evt, UIElement listeningElement)
    {
        ToggleFunction();
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        this.QuickMouseInteraction();
        var rect = GetDimensions().ToRectangle();
        var drawer = SpritebatchDrawer.FromTextureAsset(ButtonIconTexture, Vector2.Zero);
        drawer.color = Color.White;
        if (GetDrawColorFunction != null)
            drawer.color = GetDrawColorFunction();
        var framing = GetFrameFunction();
        if (framing.frame == 0)
            drawer.color = Color.Lerp(drawer.color, Color.Black, 0.5f);
        drawer.VerticalFrame(framing.frame, framing.frameCount);
        drawer.CenterOrigin();

        drawer.worldPosition = rect.Center() + Main.screenPosition;
        spriteBatch.Draw(drawer);
    }
}
public class DevToolTimeSliderUIPanel : UIPanel
{
    bool _setting;
    bool _needsToSet;
    double _timeToSetTo;
    bool _dayToSetTo;
    Texture2D _dayTexture;
    Texture2D _nightTexture;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 264;
        Height.Pixels = 16;
        OnLeftMouseDown += SettingTime;
        OnLeftMouseUp += FinishSettingTime;
        Main.QueueMainThreadAction(CreateGradientTexture);
    }

    private void CreateGradientTexture()
    {
        var halfWidth = Width.Pixels / 2f;
        _dayTexture = new Texture2D(Main.graphics.GraphicsDevice, (int)halfWidth, (int)Height.Pixels);
        Color[] pixels = new Color[_dayTexture.Width * _dayTexture.Height];
        for (int y = 0; y < _dayTexture.Width; y++)
        {
            float ratio = (float)y / (float)_dayTexture.Width;
            for (int x = 0; x < _dayTexture.Height; x++)
            {
                int pixelIndex = TextureUtilities.GetPixelIndex(_dayTexture.Width, y, x);
                pixels[pixelIndex] = LunarLightingRenderer.GetSunColor(ratio);
            }
        }
        _dayTexture.SetData(pixels);
        _nightTexture = new Texture2D(Main.graphics.GraphicsDevice, (int)halfWidth, (int)Height.Pixels);
        for (int y = 0; y < _nightTexture.Width; y++)
        {
            float ratio = (float)y / (float)_nightTexture.Width;
            for (int x = 0; x < _nightTexture.Height; x++)
            {
                int pixelIndex = TextureUtilities.GetPixelIndex(_nightTexture.Width, y, x);
                pixels[pixelIndex] = LunarLightingRenderer.GetSunColor(1f);
            }
        }
        _nightTexture.SetData(pixels);
    }

    private void SettingTime(UIMouseEvent evt, UIElement listeningElement)
    {
        _setting = true;
    }

    private void FinishSettingTime(UIMouseEvent evt, UIElement listeningElement)
    {
        _setting = false;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        this.QuickMouseInteraction();
        var rect = GetDimensions().ToRectangle();
        if (_setting)
        {
            var topLeft = rect.TopLeft();
            var xPos = Main.MouseScreen.X;
            var diff = Main.MouseScreen.X - topLeft.X;
            var midPoint = topLeft;
            var halfWidth = Width.Pixels * 0.5f;
            midPoint.X += halfWidth;

            var ratio = 0F;
            if (xPos < midPoint.X)
            {
                ratio = (Main.MouseScreen.X - topLeft.X) / halfWidth;
                ratio = MathHelper.Clamp(ratio, 0f, 1f);
                _timeToSetTo = ratio * Main.dayLength;
                _dayToSetTo = false;
            }
            if(xPos > midPoint.X)
            {
                ratio = (Main.MouseScreen.X - midPoint.X) / halfWidth;
                ratio = MathHelper.Clamp(ratio, 0f, 1f);
                _timeToSetTo = ratio * Main.nightLength;
                _dayToSetTo = true;

            }

            WorldDevTools.SetTimeNoSync(_timeToSetTo, _dayToSetTo);
            _needsToSet = true;
        }

        if (_needsToSet && !_setting)
        {
            WorldDevTools.SetTime(_timeToSetTo, _dayToSetTo);
            _needsToSet = false;
        }

        var t = (float)Main.time;
        var offset = 0F;
        if (Main.dayTime)
        {
            t /= (float)Main.dayLength;
        }
        if (!Main.dayTime)
        {
            t /= (float)Main.nightLength;
            offset = Width.Pixels * 0.5f;
        }

        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        var drawer = SpritebatchDrawer.FromTextureAsset(_dayTexture, rect.TopLeft() + Main.screenPosition);
        drawer.color = Color.White;
        drawer.drawOrigin = Vector2.Zero;

        spriteBatch.Draw(drawer);

        var drawer2 = SpritebatchDrawer.FromTextureAsset(_nightTexture, rect.TopLeft() + Main.screenPosition + new Vector2(_dayTexture.Width, 0));
        drawer2.color = Color.White;
        drawer2.drawOrigin = Vector2.Zero;

        spriteBatch.Draw(drawer2);


        var pointToPlaceAt = rect.TopLeft() + new Vector2(t * Width.Pixels * 0.5f + offset, 0);
        var iconDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.UI.FancyScrollbarInner.Asset, pointToPlaceAt + Main.screenPosition);
        iconDrawer.drawOrigin = Vector2.Zero;
        iconDrawer.color = Color.White;
        spriteBatch.Draw(iconDrawer);

    }
}
public class DevToolHotbarUIPanel : UIPanel
{
    public DevToolHotbarUIPanel()
    {
        Buttons = new();
    }
    public readonly List<DevToolHotbarButton> Buttons;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 264;
        Height.Pixels = 64;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.GodmodeIcon.Asset,
            () =>
            {
                return ((int)DevTools.godMode, 3);
            },
            () =>
            {
                var godmode = (int)DevTools.godMode;
                godmode++;
                if (godmode >= 3)
                {
                    godmode = 0;
                }
                DevTools.godMode = (GodMode)godmode;
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.WingIcon.Asset,
            () =>
            {
                var frame = DevTools.infiniteFlight ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                DevTools.infiniteFlight = !DevTools.infiniteFlight;
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.ReachIcon.Asset,
            () =>
            {
                var frame = DevTools.infiniteReach ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                DevTools.infiniteReach = !DevTools.infiniteReach;
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.ItemIcon.Asset,
            () =>
            {
                var itemBrowser = ModContent.GetInstance<ItemBrowserSystem>();
                var frame = itemBrowser.IsOpen() ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                var itemBrowser = ModContent.GetInstance<ItemBrowserSystem>();
                itemBrowser.ToggleUI();
            }));



        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.CameraIcon.Asset,
            () =>
            {
                return ((int)DevTools.freeCamera, 3);
            },
            () =>
            {
                var godmode = (int)DevTools.freeCamera;
                godmode++;
                if (godmode >= 3)
                {
                    godmode = 0;
                }
                DevTools.freeCamera = (FreeCamera)godmode;
            }));
        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.SpawnsIcon.Asset,
            () =>
            {
                var frame = WorldDevTools.DisableNPCSpawns ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                WorldDevTools.ToggleNPCSpawns();
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.FreezeTimeIcon.Asset,
            () =>
            {
                var frame = WorldDevTools.FreezeTime ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                WorldDevTools.ToggleFreezeTime();
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.TimeSliderIcon.Asset,
            () =>
            {
                var frame = DevTools.showTimeSlider ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                DevTools.showTimeSlider = !DevTools.showTimeSlider;
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.LightHackIcon.Asset,
            () =>
            {
                var frame = DevTools.LightIntensity > 0 ? 1 : 0;
                return (frame, 2);
            },
            () =>
            {
                DevTools.LightIntensity += 0.25f;
                if(DevTools.LightIntensity >= 1.1f)
                {
                    DevTools.LightIntensity = 0f;
                }
            },
            () =>
            {
                return Color.Lerp(Color.Black, Color.White, DevTools.LightIntensity + 0.2f);
            }));

        Buttons.Add(new(AssetReferences.Assets.Textures.UI.DevTools.TrashIcon.Asset,
            () =>
            {
                return (0, 1);
            },
            () =>
            {
                WorldDevTools.ClearItems();
            }));

        foreach (var btn in Buttons)
        {
            Append(btn);
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Left.Pixels = Main.screenWidth - Width.Pixels - 432;
        Top.Pixels = 8;
        var i = 0;
        foreach (var btn in Buttons)
        {
            btn.Left.Pixels = i * 40 + 16;
            i++;
        }
        Width.Pixels = 432;
        Height.Pixels = 64;
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {

        base.DrawSelf(spriteBatch);
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
    }
}

public class DevToolHotbarUIState : UIState
{
    public DevToolHotbarUIState()
    {
        Panel = new();
        TimePanel = new();
    }
    public readonly DevToolTimeSliderUIPanel TimePanel;
    public readonly DevToolHotbarUIPanel Panel;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Append(Panel);
        Append(TimePanel);
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        TimePanel.Left.Pixels = Main.screenWidth - TimePanel.Width.Pixels - 450;
        TimePanel.Top.Pixels = 66;
        if (!DevTools.showTimeSlider)
            TimePanel.Top.Pixels = 999999;
    }
}

[Autoload(Side = ModSide.Client)]
public class DevToolsUISystem : ModSystem
{
    bool _pressed;
    private UserInterface _userInterface;
    private UserInterface _npcUserInterface;
    private GameTime _lastUpdateUiGameTime;
    public DevToolHotbarUIState uiState;
    public NPCViewerUIState viewerUIState;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _userInterface = new UserInterface();
        _npcUserInterface = new();
        uiState = new();
        uiState.Activate();
        viewerUIState = new((NPC npc) => DevTools.spawnNPCType = npc.type);
    }

    public void ToggleNPCMenuUI()
    {
        if(_npcUserInterface.CurrentState == null)
            _npcUserInterface.SetState(viewerUIState);
        else
            _npcUserInterface.SetState(null);
    }
    public void CloseNPCMenuUI()
    {
        _npcUserInterface.SetState(null);
    }
    public void OpenUI()
    {
        //Create a new editing context
        //Set the state of the interface.
        _userInterface.SetState(uiState);
    }

    public void CloseUI()
    {
        _userInterface.SetState(null);
    }

    public void ToggleUI()
    {
        if (_userInterface.CurrentState == null)
            OpenUI();
        else
            CloseUI();
    }

    public override void UpdateUI(GameTime gameTime)
    {
    
        DevTools.UpdateFreeCameraPosition();
        if (InputHelper.KeyDown(Keys.F1))
        {
            _pressed = true;
        }
        else if (_pressed && InputHelper.KeyUp(Keys.F1))
        {
            ToggleUI();
            _pressed = false;
        }
        _lastUpdateUiGameTime = gameTime;
        if (_userInterface?.CurrentState != null)
        {
            _userInterface.Update(gameTime);
        }
        if (_npcUserInterface?.CurrentState != null)
        {
            _npcUserInterface.Update(gameTime);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex != -1)
        {
            layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "Scarlet Sun: Dev Tools Hotbar",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                    {
                        _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    if (_lastUpdateUiGameTime != null && _npcUserInterface?.CurrentState != null)
                    {
                        _npcUserInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    return true;
                },
                InterfaceScaleType.UI));
        }
    }

    public void PostDrawFullScreenMap(ref string mouseText)
    {

        if (Main.mouseRight && Main.mouseRightRelease)
        {
            PlayerInput.SetZoom_Unscaled();
            int mapWidth = Main.maxTilesX * 16;
            int mapHeight = Main.maxTilesY * 16;
            Vector2 cursorPosition = new Vector2(Main.mouseX, Main.mouseY);

            cursorPosition.X -= Main.screenWidth / 2f;
            cursorPosition.Y -= Main.screenHeight / 2f;

            Vector2 mapPosition = Main.mapFullscreenPos;
            Vector2 cursorWorldPosition = mapPosition;

            cursorPosition /= 16;
            cursorPosition *= 16 / Main.mapFullscreenScale;
            cursorWorldPosition += cursorPosition;
            cursorWorldPosition *= 16;

            Player player = Main.player[Main.myPlayer];
            cursorWorldPosition.Y -= player.height;
            cursorWorldPosition.X = MathHelper.Clamp(cursorWorldPosition.X, 0, mapWidth - player.width);
            cursorWorldPosition.Y = MathHelper.Clamp(cursorWorldPosition.Y, 0, mapHeight - player.height);
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                player.Teleport(cursorWorldPosition, 1, 0);
                player.position = cursorWorldPosition;
                player.velocity = Vector2.Zero;
                player.fallStart = (int)(player.position.Y / 16f);
            }
            else
            {
                player.Teleport(cursorWorldPosition, 6);
                NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, cursorWorldPosition.X, cursorWorldPosition.Y, 1);
            }
            PlayerInput.SetZoom_UI();
        }
    }
}
public class WorldDevTools : ModSystem
{
    public static bool DisableNPCSpawns
    {
        get => field;
        set
        {
            var old = field;
            field = value;
            if (!old && field)
            {
                ClearNPCs();
            }
        }
    }

    public static bool FreezeTime
    {
        get => field;
        set
        {
            field = value;
        }
    }

    public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate)
    {
        base.ModifyTimeRate(ref timeRate, ref tileUpdateRate, ref eventUpdateRate);
        if (FreezeTime)
        {
            timeRate = 0;
            tileUpdateRate = 0;
            eventUpdateRate = 0;
        }
    }
    public static void ClearItems()
    {
        foreach(var item in Main.ActiveItems)
        {
            item.active = false;
        }
    }
    public static void SetTimeNoSync(double time, bool isNight)
    {
        Main.time = time;
        Main.dayTime = !isNight;
    }
    public static void SetTime(double time, bool isNight)
    {
        Main.time = time;
        Main.dayTime = !isNight;
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            SendTimeSyncPacket();
        }
    }

    public static void HandleDisableNPCSpawnsPacket(BinaryReader reader)
    {
        var areSpawnsEnabled = reader.ReadBoolean();
        DisableNPCSpawns = areSpawnsEnabled;
        if (Main.netMode == NetmodeID.Server)
        {
            SendDisableNPCSpawnsPacket();
        }
    }


    public static void ToggleClearItems()
    {
        ClearItems();
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            SendClearItemsPacket();
        }
    }

    public static void ToggleNPCSpawns()
    {
        DisableNPCSpawns = !DisableNPCSpawns;
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            SendDisableNPCSpawnsPacket();
        }

    }

    public static void ToggleFreezeTime()
    {
        FreezeTime = !FreezeTime;
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            SendFreezeTimePacket();
        }
    }
    public static void HandleTimeSyncPacket(BinaryReader reader)
    {
        var time = reader.ReadDouble();
        var isNight = reader.ReadBoolean();
        Main.time = time;
        Main.dayTime = !isNight;
        if (Main.netMode == NetmodeID.Server)
        {
            SendTimeSyncPacket();
        }
    }

    public static void HandleClearItemsPacket(BinaryReader reader)
    {
        if (Main.netMode == NetmodeID.Server)
        {
            SendClearItemsPacket();
        }
    }

    public static void HandleFreezeTimePacket(BinaryReader reader)
    {
        var areTimeFroze = reader.ReadBoolean();
        FreezeTime = areTimeFroze;
        if (Main.netMode == NetmodeID.Server)
        {
            SendFreezeTimePacket();
        }
    }
    public static void SendClearItemsPacket()
    {
        var packet = Stellamod.Instance.GetPacket();
        packet.Write((byte)MessageType.ClearItems);
        packet.Send(-1);
    }
    public static void SendTimeSyncPacket()
    {
        var packet = Stellamod.Instance.GetPacket();
        packet.Write((byte)MessageType.SetTime);
        packet.Write(Main.time);
        packet.Write(Main.dayTime);
        packet.Send(-1);
    }

    public static void SendFreezeTimePacket()
    {
        var packet = Stellamod.Instance.GetPacket();
        packet.Write((byte)MessageType.SetFrozenTime);
        packet.Write(FreezeTime);
        packet.Send(-1);
    }

    public static void SendDisableNPCSpawnsPacket()
    {
        var packet = Stellamod.Instance.GetPacket();
        packet.Write((byte)MessageType.SetDisableNPCSpawns);
        packet.Write(DisableNPCSpawns);
        packet.Send(-1);
    }

    public static void ClearNPCs()
    {
        foreach (var npc in Main.ActiveNPCs)
        {
            npc.life = 0;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0);
        }
    }


}

public class DisableNPCSpawns : GlobalNPC
{
    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (WorldDevTools.DisableNPCSpawns)
        {
            spawnRate = 0;
            maxSpawns = 0;
        }
    }
}
public static class DevTools
{
    public static int spawnNPCType;
    public static float LightIntensity;
    public static GodMode godMode;
    public static bool infiniteFlight;
    public static bool infiniteReach;
    public static FreeCamera freeCamera;
    public static bool infiniteMana;
    public static bool showTimeSlider;
    public static Vector2 freeCameraPosition;
    public static void UpdateFreeCameraPosition()
    {
        if (DevTools.freeCamera == FreeCamera.Freeze)
        {

        }
        else if (DevTools.freeCamera == FreeCamera.Free && !Main.blockInput)
        {
            float speed = 30f;
            if (Main.keyState.IsKeyDown(Keys.LeftAlt))
                speed *= .3f;
            if (Main.keyState.IsKeyDown(Keys.LeftShift))
                speed *= 1.5f;
            if (DevToolPlayer.left)
                DevTools.freeCameraPosition.X -= speed;
            if (DevToolPlayer.down)
                DevTools.freeCameraPosition.Y += speed;
            if (DevToolPlayer.right)
                DevTools.freeCameraPosition.X += speed;
            if (DevToolPlayer.up)
                DevTools.freeCameraPosition.Y -= speed;
        }
        else
        {
    
            DevTools.freeCameraPosition = Main.screenPosition;
        }
    }
}
public class DevToolPlayer : ModPlayer
{
    public static bool left;
    public static bool right;
    public static bool up;
    public static bool down;
    public override void SetControls()
    {
        base.SetControls();
        if (DevTools.freeCamera == FreeCamera.Free)
        {
            left = Player.controlLeft;
            right = Player.controlRight;
            up = Player.controlUp;
            down = Player.controlDown;

            Player.controlDown = false;
            Player.controlUp = false;
            Player.controlLeft = false;
            Player.controlRight = false;

            Player.controlMount = false;
            Player.controlHook = false;
            Player.controlThrow = false;
            //	player.controlJump = false;
            Player.controlSmart = false;
            Player.controlTorch = false;
        }
    }

    public override void PreUpdateMovement()
    {
        base.PreUpdateMovement();
    }

    public override void ModifyScreenPosition()
    {
        base.ModifyScreenPosition();
        if (DevTools.freeCamera == FreeCamera.Normal)
            return;
        Main.screenPosition = DevTools.freeCameraPosition;
    }

    public override void ResetEffects()
    {

        if (Player.whoAmI == Main.myPlayer)
        {
            if (DevTools.infiniteReach)
            {
                if (Main.SmartCursorWanted || Main.SmartCursorIsUsed)
                {
                    Main.SmartCursorWanted_Mouse = false;
                    Main.SmartCursorWanted_GamePad = false;
                }
                else
                {
                    Player.tileRangeX = int.MaxValue / 32 - 20;
                    Player.tileRangeY = int.MaxValue / 32 - 20;
                }
            }
        }
    }
    public override void PreUpdate()
    {
        if (DevTools.godMode == GodMode.NoDeath)
        {
            Player.statLife = Player.statLifeMax2;
        }
        if (DevTools.infiniteFlight)
        {
            Player.wingTime = Player.wingTimeMax;
        }
        if (DevTools.infiniteMana)
        {
            Player.statMana = Player.statManaMax2;
        }
    }
    public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
    {
        if (DevTools.godMode == GodMode.NoHit)
            return true;
        return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
    }
    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
    {
        if (DevTools.godMode == GodMode.NoDeath)
        {
            return false;
        }
        return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
    }
}
