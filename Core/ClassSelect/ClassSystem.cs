using ReLogic.Content;
using Stellamod.Common.ClassReworkSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;
using Terraria.UI;

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
        Width.Pixels = 160;
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
    public ClassCategoryButton(Asset<Texture2D> slotTexture, Asset<Texture2D> highlightTexture) : base()
    {
        _texture = slotTexture;
        _highlightTexture = highlightTexture;
        Width.Set(_texture.Width(), 0f);
        Height.Set(_texture.Height(), 0f);
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
            spriteBatch.Draw(_highlightTexture.Value, dimensions.Position() + new Vector2(4), Color.White);
        }
    }
}


public class ClassPanel : UIPanel
{
    public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2) ;
    public int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2) ;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 512;
        Height.Pixels = 384;
        

      
        Asset<Texture2D> categoryTexture = ModContent.Request<Texture2D>($"{this.GetTypeDirectoryWithSlash()}CategoryPanel", AssetRequestMode.ImmediateLoad);
        Asset<Texture2D> categoryTextureHighlight = ModContent.Request<Texture2D>($"{this.GetTypeDirectoryWithSlash()}CategoryPanelHighlight", AssetRequestMode.ImmediateLoad);
        for (int i = 0; i < 5; i++)
        {
            Asset<Texture2D> btnTexture = ModContent.Request<Texture2D>($"{this.GetTypeDirectoryWithSlash()}Classes_{i}");
            ClassCategoryButton btn = new ClassCategoryButton(categoryTexture, categoryTextureHighlight);
            btn.Top.Pixels = i * 52;
            btn.Left.Pixels = 8;
            btn.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => 
            {
                ModContent.GetInstance<ClassSystem>().selectedClass = i;
            };
          
            Append(btn);

            UIImage iconImage = new UIImage(btnTexture);
            iconImage.IgnoresMouseInteraction = true;
            iconImage.Top.Pixels = btn.Top.Pixels + 8;
            iconImage.Left.Pixels = btn.Left.Pixels + 8;
            Append(iconImage);
        }
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

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
        confirmButton.Left.Pixels = classPanel.Left.Pixels + classPanel.Width.Pixels;
        confirmButton.Top.Pixels = classPanel.Top.Pixels + classPanel.Height.Pixels;
        confirmButton.Left.Pixels -= confirmButton.Width.Pixels;

        backButton.Left.Pixels = classPanel.Left.Pixels;
        backButton.Top.Pixels = classPanel.Top.Pixels + classPanel.Height.Pixels;
    }
    
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

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
    public override void Load()
    {
        base.Load();
        On_Main.DrawMenu += DrawClassSelectMenu;
        
        //       Main
        On_UICharacterSelect.NewCharacterClick += OpenSelectCharacterClass;
    }

    private void OpenSelectCharacterClass(On_UICharacterSelect.orig_NewCharacterClick orig, UICharacterSelect self, UIMouseEvent evt, UIElement listeningElement)
    {
        _originalMethod = new OrigCharacterMethod(orig, self, evt, listeningElement);
        Main.menuMode = 888;
        Main.MenuUI.SetState(classUIState);
        OpenUI();
        // _openNewCharacterFunction = orig;
    }

    public void ContinueToCharacterCreation()
    {
        _originalMethod.orig(_originalMethod.self, _originalMethod.evt, _originalMethod.listeningElement);
        _originalMethod = null;

    }

    public bool ShouldShowClassSelectMenu()
    {
        return _originalMethod != null;
    //    return Main.PendingPlayer != null && Main.PendingPlayer.GetModPlayer<ClassReworkPlayer>().playerClass == PlayerClass.God;
    }
    
    private void DrawClassSelectMenu(On_Main.orig_DrawMenu orig, Main self, GameTime gameTime)
    {
        orig(self, gameTime);

      


        //throw new NotImplementedException();
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
}
