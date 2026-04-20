using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.SirestiasShop;
using Stellamod.Common.UI;
using Stellamod.Core.Camera;
using Stellamod.Core.Utilities;
using Stellamod.Core.ZTileSystem;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Common.WaypointSystem;

public enum OrganWaypoint : byte
{
    WitchTown = 0,
    Marsh = 1,
    Desert = 2,
    Moonspiral = 3,
    ApocalypseTower = 4,
    BloodySanctum = 5,
    Dragonhome = 6,
    Hallowrooms = 7,
    Ishtar = 8,
    Platform = 9,
    RunicaWaterside = 10,
    WonderousDarkspace = 11,
    WorldsEnd = 12,
    MistyDungeon = 13
}

public class OrganWave : ModProjectile
{
    private float Time => 120;
    private ref float Timer => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = false;
        Projectile.timeLeft = (int)Time * 2;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            ModContent.GetInstance<OrganWaypointTracker>().darknessAnimation = 175;
        }
        CameraTargetSystem.AddTarget(Projectile.Center);

        //  ModContent.GetInstance<CameraTargetSystem>().TargetPositions.Add(Projectile.Center);
        if (Timer == 60)
        {
            PixelPrimitiveCircleFactory.CreateOrganBoom(Projectile.Center);
            if (Main.netMode != NetmodeID.Server)
                ModContent.GetInstance<ScreenShaderSystem>().TintScreen(Color.White, 0.2f, 60);
        }

        if (Timer > 60 && Timer % 4 == 0)
        {
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(256, 256), ModContent.DustType<MusicDust>(), -Vector2.UnitY, 0, Color.Orange, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            if (Main.rand.NextBool(2))
            {
                SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(256, 256), -Vector2.UnitY, Color.White, Scale: 0.5f);
                sp.noTileCollide = true;
                sp.gravity = 0;
                sp.outerColor = Color.White;
            }

        }
        for (int i = 0; i < Main.musicFade.Length; i++)
        {
            Main.musicFade[i] = 0;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Timer < 2)
            return false;
        float outRatio = Timer / Time;
        RadialShearShader shearShader = RadialShearShader.Instance;
        shearShader.Time = outRatio * 1.4f;

        float scale = MathHelper.Lerp(1.8f, 0f, EasingFunction.OutExpo(outRatio));
        Asset<Texture2D> magicCircle = AssetManager.GlowMask.SpiralVortex;
        SpritebatchDrawer waveDrawer = SpritebatchDrawer.FromTextureAsset(magicCircle, Projectile.Center);
        waveDrawer.rotation += Main.GlobalTimeWrappedHourly * 4;
        waveDrawer.scale = Vector2.Lerp(Vector2.One * 0.8f, Vector2.One * 1.6f, EasingFunction.OutExpo(outRatio)) * scale;
        waveDrawer.color = Color.Orange;
        waveDrawer.color *= MathHelper.SmoothStep(1f, 0f, outRatio);
        waveDrawer.color.A = 0;

        Main.spriteBatch.Restart(effect: shearShader.Effect);
        //   Main.spriteBatch.Draw(waveDrawer);

        SpritebatchDrawer backGlowDrawwer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        backGlowDrawwer.color = Color.DarkOrange * 0.5f;
        backGlowDrawwer.color.A = 0;
        backGlowDrawwer.scale = Vector2.One * scale;
        Main.spriteBatch.Draw(backGlowDrawwer);

        waveDrawer.color = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(outRatio));
        waveDrawer.color.A = 0;
        Main.spriteBatch.Draw(waveDrawer);
        Main.spriteBatch.RestartDefaults();

        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/MuzzleFlash"), Projectile.Center);
        drawer.scale = new Vector2(3, 10);
        float timer = Timer - 60f;

        drawer.color = Color.Orange * MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(timer / 180f));
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);

        float alpha = EasingFunction.QuadraticBump(timer / 180f);
        string text = $"Waypoint Unlocked!";
        Vector2 pos = Projectile.Center - Main.screenPosition;
        pos.Y -= 128;

        Vector2 size = FontAssets.DeathText.Value.MeasureString(text);
        float textScale = 1.5f;
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, text,
            pos, Color.White * alpha, 0f, size * 0.5f, new Vector2(textScale), -1, textScale);
        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class OrganWaypointTracker : ModSystem
{
    public bool[] locations;
    public float darknessAnimation;
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

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        if (Keyboard.GetState().IsKeyDown(Keys.O))
        {
            ResetWaypoints();
        }
        if (darknessAnimation > 0)
            darknessAnimation--;
    }
    public ref bool GetWaypoint(OrganWaypoint waypoint)
    {
        int index = (int)waypoint;
        return ref locations[index];
    }

    public void ActivateWaypoint(OrganWaypoint waypoint, Vector2 worldPosition)
    {
        int index = (int)waypoint;
        locations[index] = true;
        Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), worldPosition, Vector2.Zero,
            ModContent.ProjectileType<OrganWave>(), 1, 1, Main.LocalPlayer.whoAmI);
        ActivateWaypointEffect(worldPosition);
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            //Need to sync the activation across clients
            int clientToIgnore = Main.LocalPlayer.whoAmI;
            Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.WaypointActivate,
                (byte)waypoint,
                worldPosition.X,
                worldPosition.Y).Send(ignoreClient: clientToIgnore);
        }
    }

    public void HandleWaypointActivatePacket(BinaryReader reader)
    {
        OrganWaypoint waypoint = (OrganWaypoint)reader.ReadByte();
        Vector2 worldPosition = reader.ReadVector2();
        locations[(int)waypoint] = true;
        ActivateWaypointEffect(worldPosition);
    }

    private void ActivateWaypointEffect(Vector2 worldPosition)
    {
        SoundStyle activateSound = AssetRegistry.Sounds.Waypoint.WaypointActivate;
        SoundEngine.PlaySound(activateSound);

        //Bit of screenshake never hurt anyone
        ShakeModSystem.Shake = 4;
        FXUtil.ShakeCamera(worldPosition, 1024, 4);

    }

    public void ResetWaypoints()
    {
        for (int i = 0; i < locations.Length; i++)
        {
            locations[i] = false;
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        int length = locations.Length;
        writer.Write(length);
        for (int i = 0; i < length; i++)
        {
            writer.Write(locations[i]);
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        int length = reader.ReadInt32();
        for (int i = 0; i < length; i++)
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

public class WaypointButtonsUI : UIPanel
{
    public class WaypointButton
    {
        private Asset<Texture2D> LoadPhotoAsset(string fileName)
        {
            bool succeed = ModContent.RequestIfExists<Texture2D>(WaypointSystem.AssetPath(fileName), out Asset<Texture2D> photoAsset);
            if (!succeed)
            {
                photoAsset = ModContent.Request<Texture2D>(WaypointSystem.AssetPath("Placeholder"));
            }
            return photoAsset;
        }
        public WaypointButton(string textureName, OrganWaypoint WaypointType)
        {
            this.TextureAsset = LoadTextureAsset(textureName);
            this.PhotoTextureAsset = LoadPhotoAsset(textureName + "_Photo");
            this.WaypointType = WaypointType;
        }

        public readonly Asset<Texture2D> TextureAsset;
        public readonly Asset<Texture2D> PhotoTextureAsset;
        public readonly OrganWaypoint WaypointType;
        public Vector2 scale;
        //        Asset<Texture2D> TextureAsset = TextureAs
    }

    private Asset<Texture2D> _photoPanelTextureAsset;
    private WaypointButton[] _waypointButtons;
    private WaypointButton _previewButton;
    private bool _hovering;

    private float _easeInTimer;
    public float alpha;
    public static Asset<Texture2D> LoadTextureAsset(string fileName)
    {
        return ModContent.Request<Texture2D>(WaypointSystem.AssetPath(fileName));
    }

    public WaypointButtonsUI()
    {
        _photoPanelTextureAsset = LoadTextureAsset("PhotoFrame");
        _waypointButtons = new WaypointButton[14];
        _waypointButtons[0] = new WaypointButton("ApocalypseTower", OrganWaypoint.ApocalypseTower);
        _waypointButtons[1] = new WaypointButton("BloodySanctum", OrganWaypoint.BloodySanctum);
        _waypointButtons[2] = new WaypointButton("Dragonhome", OrganWaypoint.Dragonhome);
        _waypointButtons[3] = new WaypointButton("GintzeDesert", OrganWaypoint.Desert);
        _waypointButtons[4] = new WaypointButton("Hallowrooms", OrganWaypoint.Hallowrooms);
        _waypointButtons[5] = new WaypointButton("Ishtar", OrganWaypoint.Ishtar);
        _waypointButtons[6] = new WaypointButton("OvergrownMarsh", OrganWaypoint.Marsh);
        _waypointButtons[7] = new WaypointButton("Platform", OrganWaypoint.Platform);
        _waypointButtons[8] = new WaypointButton("RunicaWaterside", OrganWaypoint.RunicaWaterside);
        _waypointButtons[9] = new WaypointButton("Witchtown", OrganWaypoint.WitchTown);
        _waypointButtons[10] = new WaypointButton("WonderousDarkspace", OrganWaypoint.WonderousDarkspace);
        _waypointButtons[11] = new WaypointButton("WorldsEnd", OrganWaypoint.WorldsEnd);
        _waypointButtons[12] = new WaypointButton("MoonlightCathedral", OrganWaypoint.Moonspiral);
        _waypointButtons[13] = new WaypointButton("MistyDungeon", OrganWaypoint.MistyDungeon);
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        Width.Pixels = 394 * 2;
        Height.Pixels = 272 * 2;
    }

    private void PreviewPopup(SpriteBatch spriteBatch, WaypointButton waypointButton, Vector2 position)
    {

        position.Y += ExtraMath.Osc(0f, 4f, speed: 2);
        Vector2 origin = _photoPanelTextureAsset.Value.Size() * 0.5f;
        Vector2 scale = Vector2.One;

        Vector2 previewOrigin = waypointButton.PhotoTextureAsset.Value.Size() * 0.5f;
        Vector2 previewPosition = position;
        previewPosition.Y -= 13;

        Color drawColor = Color.White * alpha;
        spriteBatch.Draw(waypointButton.PhotoTextureAsset.Value, previewPosition, null, drawColor, 0, previewOrigin, scale, SpriteEffects.None, 0);
        spriteBatch.Draw(_photoPanelTextureAsset.Value, position, null, drawColor, 0, origin, scale, SpriteEffects.None, 0);


        string text = LangText.Common(waypointButton.WaypointType.ToString());
        Vector2 size = FontAssets.DeathText.Value.MeasureString(text);
        float textScale = 0.5f;
        Vector2 textPosition = position;
        textPosition.Y -= 100;
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, text,
            textPosition, drawColor, 0f, size * 0.5f, new Vector2(textScale), -1, textScale);
    }

    private void DrawButton_Inner(SpriteBatch spriteBatch, WaypointButton waypointButton, Vector2 position)
    {
        Vector2 origin = new Vector2();
        origin.X = waypointButton.TextureAsset.Value.Width * 0.5f;
        origin.Y = waypointButton.TextureAsset.Value.Height * 0.5f;

        Vector2 targetScale = Vector2.One * 2;
        Rectangle intersectRectangle = new Rectangle(
            (int)(position.X - origin.X * 2),
            (int)(position.Y - origin.Y * 2),
            waypointButton.TextureAsset.Value.Width * 2,
            waypointButton.TextureAsset.Value.Height * 2);

        Vector2 mouseScreen = Main.MouseScreen;
        bool isMouseHovering = !_hovering && intersectRectangle.Contains(mouseScreen.ToPoint());
        if (isMouseHovering)
        {
            _previewButton = waypointButton;
            _hovering = true;
            targetScale *= 1.1f;
        }

        waypointButton.scale = Vector2.Lerp(waypointButton.scale, targetScale, 0.2f);
        Vector2 scale = waypointButton.scale;
        spriteBatch.Draw(waypointButton.TextureAsset.Value, position, null, Color.White, 0, origin, scale, SpriteEffects.None, 0);
        //   Primitives2D.DrawRectangle(spriteBatch, intersectRectangle, Color.Red);

        if (isMouseHovering && !PlayerInput.IgnoreMouseInterface)
        {
            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                //    Main.NewText("Click");
                Main.mouseLeftRelease = false;
            }
            Main.LocalPlayer.mouseInterface = true;
        }


        if (isMouseHovering)
        {
            SpriteWhiteShader spriteWhiteShader = SpriteWhiteShader.Instance;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                effect: spriteWhiteShader.Effect, Main.UIScaleMatrix);

            Color highlightedColor = Color.Yellow;
            highlightedColor *= ExtraMath.Osc(0.5f, 1f, speed: 6);
            spriteBatch.Draw(waypointButton.TextureAsset.Value, position, null, highlightedColor, 0, origin, scale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                effect: null, Main.UIScaleMatrix);

        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (_hovering)
        {
            _easeInTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //_easeInTimer = MathHelper.Lerp(_easeInTimer, 1f, 0.2f);
        }
        else
        {
            _easeInTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        _easeInTimer = MathHelper.Clamp(_easeInTimer, 0f, 1.5f);
        alpha = EasingFunction.InOutSine(_easeInTimer);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        //So we need to draw ALL of the things in their correct spots 
        //Draw the platform
        Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
        _hovering = false;

        Rectangle rect = GetDimensions().ToRectangle();
        Vector2 platformWaypointPos = topLeft;
        platformWaypointPos.X += Width.Pixels * 0.5f;
        platformWaypointPos.Y += 64;
        //The platform
        DrawButton_Inner(spriteBatch, _waypointButtons[7], platformWaypointPos);

        //Runica Waterside
        Vector2 mistyDungeonPos = platformWaypointPos;
        mistyDungeonPos.X += 111;
        mistyDungeonPos.Y += 111;
        DrawButton_Inner(spriteBatch, _waypointButtons[13], mistyDungeonPos);

        Vector2 witchTownPos = platformWaypointPos;
        witchTownPos.X += 16;
        witchTownPos.Y += 222;
        DrawButton_Inner(spriteBatch, _waypointButtons[9], witchTownPos);

        Vector2 gintzeDesertPos = witchTownPos;
        gintzeDesertPos.X -= 152;
        gintzeDesertPos.Y -= 28;
        DrawButton_Inner(spriteBatch, _waypointButtons[3], gintzeDesertPos);

        Vector2 hallowRoomsPos = gintzeDesertPos;
        hallowRoomsPos.Y += 107;
        hallowRoomsPos.X += 6;
        DrawButton_Inner(spriteBatch, _waypointButtons[4], hallowRoomsPos);

        Vector2 marshPos = gintzeDesertPos;
        marshPos.X -= 100;
        marshPos.Y -= 68;
        DrawButton_Inner(spriteBatch, _waypointButtons[6], marshPos);

        Vector2 worldsEndPos = marshPos;
        worldsEndPos.X -= 50;
        worldsEndPos.Y -= 0;
        DrawButton_Inner(spriteBatch, _waypointButtons[11], worldsEndPos);

        Vector2 ishtarPos = worldsEndPos;
        ishtarPos.Y += 174;
        ishtarPos.X += 24;
        DrawButton_Inner(spriteBatch, _waypointButtons[5], ishtarPos);

        Vector2 wonderousDarkSpacePos = witchTownPos;
        wonderousDarkSpacePos.Y += 100;
        DrawButton_Inner(spriteBatch, _waypointButtons[10], wonderousDarkSpacePos);

        Vector2 bloodySanctumPos = wonderousDarkSpacePos;
        bloodySanctumPos.X += 144;
        bloodySanctumPos.Y += 40;
        DrawButton_Inner(spriteBatch, _waypointButtons[1], bloodySanctumPos);

        Vector2 dragonHomePos = bloodySanctumPos;
        dragonHomePos.X += 128;
        dragonHomePos.Y -= 2;
        DrawButton_Inner(spriteBatch, _waypointButtons[2], dragonHomePos);

        Vector2 moonlightPos = bloodySanctumPos;
        moonlightPos.Y -= 128;
        DrawButton_Inner(spriteBatch, _waypointButtons[12], moonlightPos);

        Vector2 runicaPos = dragonHomePos;
        runicaPos.Y -= 196;
        DrawButton_Inner(spriteBatch, _waypointButtons[8], runicaPos);

        Vector2 apocalypseTowerPos = topLeft;
        apocalypseTowerPos.Y += Height.Pixels - 128;
        apocalypseTowerPos.X += 184;
        DrawButton_Inner(spriteBatch, _waypointButtons[0], apocalypseTowerPos);

        _previewButton ??= _waypointButtons[0];
        Vector2 previewPos = GetDimensions().ToRectangle().TopLeft();
        previewPos.X += Width.Pixels + 196;
        previewPos.Y += Height.Pixels * 0.5f;
        previewPos.Y += MathHelper.Lerp(64, 0, alpha);
        PreviewPopup(spriteBatch, _previewButton, previewPos);

    }
}

public class WaypointUI : UIPanel
{

    private UIImage _background;
    private WaypointButtonsUI _buttonsUI;
    private UIText _titleText;
    public WaypointUI()
    {
        _titleText = new UIText("Lunar Veil");
        _buttonsUI = new WaypointButtonsUI();
        _background = new UIImage(ModContent.Request<Texture2D>(WaypointSystem.AssetPath("WaypointBackground")));
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        Width.Pixels = 394 * 2;
        Height.Pixels = 272 * 2;
        Append(_background);
        Append(_buttonsUI);
        _titleText.SetText(LangText.Common("WaypointTitle"), 1, true);
        _titleText.HAlign = 0.5f;
        Append(_titleText);
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
        this.QuickMouseInteraction();
    }
}

public class WaypointUIState : UIState
{
    public WaypointUI ui;
    public BackButton backButton;
    public WaypointUIState() : base()
    {

    }

    public override void OnInitialize()
    {
        ui = new WaypointUI();
        Append(ui);

        backButton = new BackButton(ModContent.GetInstance<WaypointSystem>().CloseUI);
        Append(backButton);
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
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null,
                            Main.UIScaleMatrix);

                        _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null,
                            Main.UIScaleMatrix);

                    }
                    return true;
                },
                InterfaceScaleType.UI));
        }
    }
}
