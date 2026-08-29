using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ReLogic.Content;
using Stellamod.Common.ClassReworkSystem;
using Stellamod.Common.SummonerSystem.UI;
using Stellamod.Common.UI;
using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using Stellamod.Core.Tooltips;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Core.ClassSelect;


public class ConfirmButton : UIPanel
{
    private float _scale;
    private Action _closeFunction;
    private UIText _backText;
    private string _text;
    public ConfirmButton(Action closeFunction, string text) : base()
    {
        _text = text;
        _closeFunction = closeFunction;
        _backText = new UIText("Back", large: true);
    }

    public bool asXButton;


    public override void OnInitialize()
    {
        base.OnInitialize();
        Height.Pixels = 54;
        _backText.Width.Pixels = Width.Pixels;
        _backText.Height.Pixels = Height.Pixels;
        _backText.HAlign = 0.5f;
        _backText.SetText(LangText.Common(_text));
        BackgroundColor = Color.Black * 0.5f;
        BorderColor = Color.White;
        Append(_backText);
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);
        _closeFunction();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);


    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        this.QuickMouseInteraction();

        if (IsMouseHovering)
        {
            _scale = MathHelper.Lerp(_scale, 1.25f, 0.3f);
        }
        else
        {
            _scale = MathHelper.Lerp(_scale, 1f, 0.3f);
        }
        _backText.SetText(LangText.Common(_text), _scale, true);
    }
}
public class ClassCategoryButton : UIPanel
{
    private readonly Asset<Texture2D> _texture;
    private readonly Asset<Texture2D> _highlightTexture;
    private readonly int _slot;
    public ClassCategoryButton(Asset<Texture2D> slotTexture, Asset<Texture2D> highlightTexture, int slot) : base()
    {
        _slot = slot;
        _texture = slotTexture;
        _highlightTexture = highlightTexture;
        OnLeftClick += SelectClass;
        Width.Set(_texture.Width(), 0f);
        Height.Set(_texture.Height(), 0f);
    }

    private ClassSystem ClassSystem => ModContent.GetInstance<ClassSystem>();
    private void SelectClass(UIMouseEvent evt, UIElement listeningElement)
    {
        ClassSystem.selectedClass = _slot;
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public float visibilityActive = 1f;
    public float visibilityInactive = 0.4f;
    public override void OnInitialize()
    {
        base.OnInitialize();

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
        CalculatedStyle dimensions = GetDimensions();
        spriteBatch.Draw(_texture.Value, dimensions.Position(), Color.White * (base.IsMouseHovering ? visibilityActive : visibilityInactive));

        if (_highlightTexture != null && base.IsMouseHovering)
        {
            Color glowColor = Color.White * 0.5f;
            spriteBatch.Draw(_highlightTexture.Value, dimensions.Position(), glowColor);
        }

        if (ClassSystem.selectedClass == _slot)
        {
            Color glowColor = Color.White;
            spriteBatch.Draw(_highlightTexture.Value, dimensions.Position(), glowColor);
        }
    }
}

public class ClassVideo : UIPanel
{
    public int lastClass;
    public bool initialized;
    public VideoPlayer videoPlayer;
    private ClassSystem ClassSystem => ModContent.GetInstance<ClassSystem>();
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 400;
        Height.Pixels = 256;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        if (videoPlayer is null || videoPlayer.IsDisposed)
            return;

        videoPlayer?.Stop();
        videoPlayer?.Dispose();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        BackgroundColor = Color.Black;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if(lastClass != ClassSystem.selectedClass)
        {
            lastClass = ClassSystem.selectedClass;
            initialized = false;
        }
        if (!initialized || ClassSystem.refresh)
        {
            ClassSystem.refresh = false;
            string path = ModContent.GetInstance<ClassSystem>().GetPreviewPath();
            var video = ModContent.Request<Video>(path, AssetRequestMode.ImmediateLoad).Value;
            
            videoPlayer?.Dispose();

            videoPlayer = new VideoPlayer();
            videoPlayer.IsLooped = true;

            videoPlayer.Play(video);
            initialized = true;
            return;
        }
        Width.Pixels = 480;

        if (videoPlayer is null || videoPlayer.IsDisposed)
            return;

        Texture2D videoTexture = videoPlayer.GetTexture();
        int width = (int)(Width.Pixels - 64);
        int height = (int)(Height.Pixels - 64);
        Rectangle sourceRect = new Rectangle(videoTexture.Width / 2 - width / 2, videoTexture.Height / 2 - height / 2, width, height);
        spriteBatch.Draw(videoTexture, GetDimensions().ToRectangle().Center(), sourceRect, Color.White, 0, sourceRect.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
    }
}

public class ClassPanel : UIPanel
{
    public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2);
    public int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2);

    public ClassVideo classVideo;
    public ClassTooltip classTooltip;
    public UIPanel classTitleBackground;
    public ClassTitle classTitle;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 512;
        Height.Pixels = 272;



        Asset<Texture2D> categoryTexture = ModContent.Request<Texture2D>($"{this.GetTypeDirectoryWithSlash()}CategoryPanel", AssetRequestMode.ImmediateLoad);
        Asset<Texture2D> categoryTextureHighlight = ModContent.Request<Texture2D>($"{this.GetTypeDirectoryWithSlash()}CategoryPanelBorder", AssetRequestMode.ImmediateLoad);
        for (int i = 0; i < 5; i++)
        {
            Asset<Texture2D> btnTexture = ModContent.Request<Texture2D>($"{this.GetTypeDirectoryWithSlash()}Classes_{i}");
            ClassCategoryButton btn = new ClassCategoryButton(categoryTexture, categoryTextureHighlight, i);
            btn.Top.Pixels = i * 52;
            btn.Left.Pixels = 8;
            Append(btn);

            UIImage iconImage = new UIImage(btnTexture);
            iconImage.IgnoresMouseInteraction = true;
            iconImage.Top.Pixels = btn.Top.Pixels + 8;
            iconImage.Left.Pixels = btn.Left.Pixels + 8;
            Append(iconImage);
        }
        classVideo = new ClassVideo();
        classVideo.Left.Pixels = 80;
        Append(classVideo);

        classTooltip = new ClassTooltip();
        classTooltip.Left.Pixels = 80;
        Append(classTooltip);

        classTitleBackground = new UIPanel();
        classTitleBackground.BackgroundColor = Color.Black;
        classTitleBackground.BorderColor = Color.White;
        Append(classTitleBackground);

        classTitle = new ClassTitle();
        Append(classTitle);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        Height.Pixels = 272;
        classTooltip.Left.Pixels = 80;
        classTooltip.Top.Pixels = 252;
        Width.Pixels = 600;
        Height.Pixels = 464;

        classTitleBackground.Width.Pixels = 384;
        classTitleBackground.Height.Pixels = 56;
        classTitleBackground.Left.Pixels = (Width.Pixels / 2 - classTitleBackground.Width.Pixels / 2);
        classTitleBackground.Top.Pixels = -108+16;
        Recalculate();
        BackgroundColor = Color.Black * 0.75f;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);


    }
}

public class ClassTitle : UIElement
{
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

      
        Vector2 titlePosition = Parent.GetDimensions().ToRectangle().TopLeft();
        titlePosition.X += Parent.GetDimensions().Width * 0.5f;
        titlePosition.Y -= 64-16;
        string text = Language.GetTextValue($"Mods.Stellamod.LoadingScreen.ClassSelect");
        Vector2 origin = FontAssets.DeathText.Value.MeasureString(text) * new Vector2(0.5f);
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, text, titlePosition, Color.White, 0, origin, Vector2.One);

    }
}
public class ClassTooltip : UIElement
{
    public override void OnInitialize()
    {
        base.OnInitialize();
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        (string, string, string) classText = ModContent.GetInstance<ClassSystem>().GetClassText();
        List<TooltipLine> lines = new List<TooltipLine>();
        TooltipLine nameTooltipLine = new TooltipLine(ModContent.GetInstance<Stellamod>(), classText.Item1, classText.Item1);
        nameTooltipLine.OverrideColor = Color.Yellow;
        lines.Add(nameTooltipLine);

        TooltipLine flavorTooltipLine = new TooltipLine(ModContent.GetInstance<Stellamod>(), classText.Item2, classText.Item2);
        flavorTooltipLine.OverrideColor = Color.White;
        lines.Add(flavorTooltipLine);

        Vector2 position = GetDimensions().ToRectangle().TopLeft();
        position.X += 18;
        position.Y += 16;


        int width = 480;
        UIHelpers.DrawTooltipsNoBG(spriteBatch, lines, position, width, 1);


        position = Parent.GetDimensions().ToRectangle().TopRight() + new Vector2(32, 0);
        position.X -= 20;
        position.Y += 8;
        List<TooltipLine> statLines = new List<TooltipLine>();
        TooltipLine statLine = new TooltipLine(ModContent.GetInstance<Stellamod>(), classText.Item3, classText.Item3);
        statLine.OverrideColor = Color.White;
        statLines.Add(statLine);
        UIHelpers.DrawTooltips(spriteBatch, statLines, position, 250, 1, yOffset: 0);

        Item[] startingItems = ModContent.GetInstance<ClassSystem>().GetClassStartingItems();
        Vector2 startingPosition = position;
        startingPosition.Y += UIHelpers.CalculateTooltipsHeight(statLines, 250);
        startingPosition.Y += 75;


        Rectangle backgroundRect = ExpandableTooltip.GetBGRectangle((int)startingPosition.X, (int)startingPosition.Y - 32, 250, (int)(56));
        Utils.DrawInvBG(spriteBatch, backgroundRect, new Color(23, 25, 81, 255) * 0.625f);

        startingPosition.X += 32;


        float between = 50;
        for (int i = 0; i < startingItems.Length; i++)
        {
            Vector2 drawPosition = startingPosition + new Vector2(i * between, 0);
            SpritebatchDrawer backDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BasicGlow"), Main.screenPosition + drawPosition);
            backDrawer.color = Color.Black * 0.95f;
            spriteBatch.Draw(backDrawer);
        }
        for (int i = 0; i < startingItems.Length; i++)
        {
            Item item = startingItems[i];
            Vector2 drawPosition = startingPosition + new Vector2(i * between, 0);
            Color glowColor = Color.White * 0.05f;
            glowColor.A = 0;
            for(float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
            {
                ItemSlot.DrawItemIcon(item, 0, spriteBatch, drawPosition + (f+Main.GlobalTimeWrappedHourly).ToRotationVector2() * 8, 1.25f, 42, glowColor);
            }
   
        }
        for (int i = 0; i < startingItems.Length; i++)
        {
            Item item = startingItems[i];
            Vector2 drawPosition = startingPosition + new Vector2(i * between, 0);
            ItemSlot.DrawItemIcon(item, 0, spriteBatch, drawPosition, 1.25f, 42, Color.White);
        }

        for (int i = 0; i < startingItems.Length; i++)
        {
            Item item = startingItems[i];
            Vector2 drawPosition = startingPosition + new Vector2(i * between, 0);
            int x = (int)drawPosition.X;
            int y = (int)drawPosition.Y;
            x -= 25;
            y -= 25;
            Rectangle rect = new Rectangle(x, y, 50, 50);
            if (rect.Contains(Main.MouseScreen.ToPoint()))
            {
                List<TooltipLine> tooltipLines = new List<TooltipLine>();
                TooltipLine helpLine = new TooltipLine(Stellamod.Instance, "SlotHelp", item.Name);
                helpLine.OverrideColor = Color.Goldenrod;
                tooltipLines.Add(helpLine);
                UIHelpers.DrawTooltips(spriteBatch, tooltipLines, Main.MouseScreen + new Vector2(16), 250, 1, yOffset: 0, extraHeight: 0);
            }
        }


    }
}
public class ClassUIState : UIState
{
    public ClassPanel classPanel;
    public ConfirmButton confirmButton;
    public ConfirmButton backButton;
    public override void OnInitialize()
    {
        base.OnInitialize();
        classPanel = new ClassPanel();
        Append(classPanel);

        confirmButton = new ConfirmButton(
            ModContent.GetInstance<ClassSystem>().ContinueToCharacterCreation, "Create");
        Append(confirmButton);

        backButton = new ConfirmButton(
            Main.OpenCharacterSelectUI, "Back"
            );
        Append(backButton);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        confirmButton.Width.Pixels = backButton.Width.Pixels = 252;
        confirmButton.Left.Pixels = classPanel.Left.Pixels + classPanel.Width.Pixels;
        confirmButton.Top.Pixels = classPanel.Top.Pixels + classPanel.Height.Pixels + 12;
        confirmButton.Left.Pixels -= confirmButton.Width.Pixels;

        backButton.Left.Pixels = classPanel.Left.Pixels;
        backButton.Top.Pixels = confirmButton.Top.Pixels;

    }
}

[Autoload(Side = ModSide.Client)]
public class ClassSystem : ModSystem
{

    private record class OrigCharacterMethod(On_UICharacterSelect.orig_NewCharacterClick orig, UICharacterSelect self, UIMouseEvent evt, UIElement listeningElement);
    private OrigCharacterMethod _originalMethod;
    private UserInterface _userInterface;
    public int selectedClass;
    public ClassUIState classUIState;
    public bool refresh;
    public override void Load()
    {
        base.Load();
        On_Main.DrawMenu += DrawClassSelectMenu;
        On_UICharacterListItem.DrawSelf += DrawClassIcon;
        On_UICharacterSelect.NewCharacterClick += OpenSelectCharacterClass;
    }

    private void DrawClassIcon(On_UICharacterListItem.orig_DrawSelf orig, UICharacterListItem self, SpriteBatch spriteBatch)
    {
        orig(self, spriteBatch);
        Player player = self.Data.Player;//(Player)typeof(UICharacter).GetField("_player", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(self);
        ClassReworkPlayer classReworkPalyer = player.GetModPlayer<ClassReworkPlayer>();
        if (classReworkPalyer == null)
            return;

        int index = (int)classReworkPalyer.playerClass;
        if (index >= 5)
            return;
        Asset<Texture2D> btnTexture = ModContent.Request<Texture2D>($"{this.GetTypeDirectoryWithSlash()}Classes_{index}");
        spriteBatch.Draw(btnTexture.Value, self.GetDimensions().ToRectangle().TopLeft() + new Vector2(540, 0), Color.White);
    }

    private void OpenSelectCharacterClass(On_UICharacterSelect.orig_NewCharacterClick orig, UICharacterSelect self, UIMouseEvent evt, UIElement listeningElement)
    {
        _originalMethod = new OrigCharacterMethod(orig, self, evt, listeningElement);
        Main.menuMode = 888;
        Main.MenuUI.SetState(classUIState);
        ModContent.GetInstance<ClassSystem>().refresh = true;
        OpenUI();

        // _openNewCharacterFunction = orig;
    }

    public void ContinueToCharacterCreation()
    {
        _originalMethod.orig(_originalMethod.self, _originalMethod.evt, _originalMethod.listeningElement);
        _originalMethod = null;

    }

    private void DrawClassSelectMenu(On_Main.orig_DrawMenu orig, Main self, GameTime gameTime)
    {

        if (Main.MenuUI.CurrentState == classUIState && Main.PendingPlayer != null)
        {
            Main.PendingPlayer.GetModPlayer<ClassReworkPlayer>().playerClass = (PlayerClass)ModContent.GetInstance<ClassSystem>().selectedClass;
            Main.MenuUI.Recalculate();
        }
    
        orig(self, gameTime);



        //throw new NotImplementedException();
    }
    public (string, string, string) GetClassText()
    {
        string classKey = "Melee";
        switch (selectedClass)
        {
            default:
            case 0:
                classKey = "Melee";
                break;
            case 1:
                classKey = "Ranger";
                break;
            case 2:
                classKey = "Mage";
                break;
            case 3:
                classKey = "Summoner";
                break;
            case 4:
                classKey = "Omni";
                break;
        }

        (string, string, string) classText;
        classText.Item1 = LangText.LoadingScreen(classKey, "DisplayName");
        classText.Item2 = LangText.LoadingScreen(classKey, "Tooltip");
        classText.Item3 = LangText.LoadingScreen(classKey, "Stats");
        return classText;
    }

    public string GetPreviewPath()
    {
        string classKey = "Melee";
        switch (selectedClass)
        {
            default:
            case 0:
                classKey = "Melee";
                break;
            case 1:
                classKey = "Ranger";
                break;
            case 2:
                classKey = "Magic";
                break;
            case 3:
                classKey = "Summon";
                break;
            case 4:
                classKey = "Omni";
                break;
        }
        return $"Stellamod/Assets/Videos/{classKey}Preview";
    }
    public override void OnModLoad()
    {
        base.OnModLoad();

        _userInterface = new UserInterface();
        classUIState = new();
    }
    public void OpenUI()
    {
        _userInterface.SetState(classUIState);
    }

    public void CloseUI()
    {
        _userInterface.SetState(null);
    }

    public Item[] GetClassStartingItems()
    {
        List<Item> items = new List<Item>();
        switch (selectedClass)
        {
            case 0:
                items.Add(new Item(ModContent.ItemType<WarriorsGrace>()));
                break;
            case 1:
                items.Add(new Item(ModContent.ItemType<IronBow>()));
                break;
            case 2:
                items.Add(new Item(ModContent.ItemType<GildedStaff>()));
                break;
            case 3:
                items.Add(new Item(ModContent.ItemType<SummoningBell>()));
                items.Add(new Item(ModContent.ItemType<DogBone>()));
                break;
            case 4:
                items.Add(new Item(ModContent.ItemType<WarriorsGrace>()));
                items.Add(new Item(ModContent.ItemType<IronBow>()));
                items.Add(new Item(ModContent.ItemType<GildedStaff>()));
                items.Add(new Item(ModContent.ItemType<SummoningBell>()));
                break;
        }
        return items.ToArray();
        //throw new NotImplementedException();
    }
    public Item[] GetClassStartingItems(int selectedClass)
    {
        List<Item> items = new List<Item>();
        switch (selectedClass)
        {
            case 0:
                items.Add(new Item(ModContent.ItemType<WarriorsGrace>()));
                break;
            case 1:
                items.Add(new Item(ModContent.ItemType<IronBow>()));
                break;
            case 2:
                items.Add(new Item(ModContent.ItemType<GildedStaff>()));
                break;
            case 3:
                items.Add(new Item(ModContent.ItemType<SummoningBell>()));
                items.Add(new Item(ModContent.ItemType<DogBone>()));
                break;
            case 4:
                items.Add(new Item(ModContent.ItemType<WarriorsGrace>()));
                items.Add(new Item(ModContent.ItemType<IronBow>()));
                items.Add(new Item(ModContent.ItemType<GildedStaff>()));
                items.Add(new Item(ModContent.ItemType<SummoningBell>()));
                break;
        }
        return items.ToArray();
        //throw new NotImplementedException();
    }
}
