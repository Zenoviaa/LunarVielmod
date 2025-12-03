using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.BossBannerSystem
{
    public enum BossBannerType : byte
    {
        WitchAcademy = 0,
        LifeNPlants = 1,
        ForgottenWarriors = 2,
        FountainOfMagic = 3,
        IllurianTroupe = 4,
        ColosseumOfProtection = 5,
        MechanizedRevivals = 6
    }

    public class BossPageUIState : UIState
    {
        public BossPageUI ui;
        public BossPageUIState() : base()
        {

        }
        public override void OnInitialize()
        {
            ui = new BossPageUI();
            Append(ui);
        }
    }

    public class RightPageUI : UIPanel
    {
        public int RelativeLeft => 0;
        public int RelativeTop => 0;
        public int GetPageWidth()
        {
            return 200;
        }

        public int GetPageHeight()
        {
            return 250;
        }
    }

    public class BossPhotoUI : UIPanel
    {
        private readonly BossPageUI _parent;
        public BossPhotoUI(BossPageUI parent)
        {
            _parent = parent;
        }
        public Asset<Texture2D> BossPhotoTextureAsset;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 376;
            Height.Pixels = 186;
        }

        public void SetPhotoTexture(BossPage bossPage)
        {
            BossPhotoTextureAsset = bossPage.RequestBossPhoto();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Asset<Texture2D> texture = BossPhotoTextureAsset;
            Rectangle rectangle = GetDimensions().ToRectangle();
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            spriteBatch.Draw(texture.Value, rectangle.TopLeft(), null, Color.White, 0f, default, 1, SpriteEffects.None, 0f);
        }
    }

    public class BossLoreUI : UIPanel
    {
        private readonly BossPageUI _parent;
        public BossLoreUI(BossPageUI parent)
        {
            _parent = parent;
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 32;
            Height.Pixels = 32;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Asset<Texture2D> glassTexture = BossBanner.RequestScrollTexture();
            Rectangle rectangle = GetDimensions().ToRectangle();
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            spriteBatch.Draw(glassTexture.Value, rectangle.TopLeft(), null, Color.White, 0f, default, 1, SpriteEffects.None, 0f);
        }
    }

    /// <summary>
    /// Opens a window that shows where you can find the boss
    /// </summary>
    public class BossGlassUI : UIPanel
    {
        private readonly BossPageUI _parent;
        public BossGlassUI(BossPageUI parent)
        {
            //I love dependency injection
            _parent = parent;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            //Width/Height doesn't need to be accurate thankfully
            Width.Pixels = 32;
            Height.Pixels = 32;
            OnLeftClick += _parent.ShowLocationWindow;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Now setting the position cause the page will set that
            //We just need the width I think?
            //Also click function
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            //Draw the glass texture
            Asset<Texture2D> texture = BossBanner.RequestGlassTexture();
            Rectangle rectangle = GetDimensions().ToRectangle();
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            Vector2 drawPosition = rectangle.TopLeft();

            //Adding a little bit of hover would be cool
            drawPosition.Y += ExtraMath.Osc(0f, 2f, speed: 1);

            //We also need a hover outline probably
            //I think I have a white shader somewhere
            if (IsMouseHovering)
            {
                var whiteShader = SpriteWhiteShader.Instance;
                float outlineOffset = 2;
                Vector2 h = Vector2.UnitX * outlineOffset;
                Vector2 v = Vector2.UnitY * outlineOffset;
                spriteBatch.Restart(effect: whiteShader.Effect);

                Color outlineColor = Color.Yellow;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, whiteShader.Effect, Main.UIScaleMatrix);

                spriteBatch.Draw(texture.Value, drawPosition + h, null, outlineColor, 0f, default, 1, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture.Value, drawPosition - h, null, outlineColor, 0f, default, 1, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture.Value, drawPosition + v, null, outlineColor, 0f, default, 1, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture.Value, drawPosition - v, null, outlineColor, 0f, default, 1, SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
            }

            spriteBatch.Draw(texture.Value, drawPosition, null, Color.White, 0f, default, 1, SpriteEffects.None, 0f);
        }
    }

    public class BossPageUI : RightPageUI
    {
        private UIText _displayNameText;
        private BossGlassUI _glassUI;
        private BossLoreUI _bossLoreUI;
        private BossPhotoUI _bossPhotoUI;
        public BossPageUI()
        {
            _displayNameText = new UIText("Your Mom");
            _glassUI = new BossGlassUI(this);
            _bossLoreUI = new BossLoreUI(this);
            _bossPhotoUI = new BossPhotoUI(this);
        }

        public BossPage BossPage { get; private set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = GetPageWidth();
            Height.Pixels = GetPageHeight();
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _displayNameText.Height.Pixels = Height.Pixels;
            _displayNameText.Width.Pixels = Width.Pixels;
            _displayNameText.IsWrapped = true;
            _displayNameText.ShadowColor = Color.Black;

            Append(_displayNameText);
            Append(_bossLoreUI);
            Append(_glassUI);
            Append(_bossPhotoUI);
        }

        public void UpdateUI(BossPage bossPage)
        {
            BossPage = bossPage;
            _displayNameText.SetText(bossPage.DisplayName);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
        }

        public void ShowLocationWindow(UIMouseEvent evt, UIElement listeningElement)
        {
            throw new NotImplementedException();
        }
    }

    public class BossPage : ModType,
        ILocalizedModType
    {
        public string LocalizationCategory => "BossPages";
        public string DisplayName
        {
            get
            {
                return LangText.BossPages(this, "DisplayName");
            }
        }

        public string Lore
        {
            get
            {
                return LangText.BossPages(this, "Lore");
            }
        }

        public string WhereToFind
        {
            get
            {
                return LangText.BossPages(this, "WhereToFind");
            }
        }

        public List<Item> Rewards;
        public List<Item> NoHitRewards;
        public int StarRanking;
        protected sealed override void Register()
        {
            ModTypeLookup<BossPage>.Register(this);
        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            Rewards = new List<Item>();
            NoHitRewards = new List<Item>();
            SetStaticDefaults();
            this.GetLocalization(nameof(DisplayName), () => "Who???");
            this.GetLocalization(nameof(WhereToFind), () => "In Your Mom");
            this.GetLocalization(nameof(Lore), () => "Birthed by your mom");
        }

        public void AddReward(Item item)
        {
            Rewards.Add(item);
        }

        public void AddNoHitReward(Item item)
        {
            NoHitRewards.Add(item);
        }

        public Asset<Texture2D> RequestBossPhoto()
        {
            Type type = this.GetType();
            return ModContent.Request<Texture2D>(type.DirectoryHere() + "/" + type.Name);
        }
    }

    public class BossBanner : ModType,
        ILocalizedModType
    {
        public string LocalizationCategory => "BossBanners";
        public string DisplayName
        {
            get
            {
                return LangText.BossBanners(this, "DisplayName");
            }
        }

        public string Description
        {
            get
            {
                return LangText.BossBanners(this, "Description");
            }
        }


        public BossPage[] Pages;
        public Asset<Texture2D> BannerTextureAsset;
        protected sealed override void Register()
        {
            ModTypeLookup<BossBanner>.Register(this);
        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
            this.GetLocalization(nameof(DisplayName), () => "");
            this.GetLocalization(nameof(Description), () => "");
            BannerTextureAsset = RequestBannerTexture();
        }
        public static Asset<Texture2D> RequestTexture(string fileName)
        {
            Type type = typeof(BossBanner);
            return ModContent.Request<Texture2D>(type.DirectoryHere() + "/" + fileName);
        }

        public static Asset<Texture2D> RequestBannerTexture()
        {
            Type type = typeof(BossBanner);
            return RequestTexture(type.Name);
        }

        public static Asset<Texture2D> RequestGlassTexture()
        {
            return RequestTexture("Glass");
        }

        public static Asset<Texture2D> RequestScrollTexture()
        {
            return RequestTexture("Scroll");
        }

        public static Asset<Texture2D> RequestStarTexture()
        {
            return RequestTexture("Star");
        }

        public Rectangle GetBannerFrame(BossBannerType type)
        {
            int frameIndex = (int)type;
            const int Frame_Height = 74;
            Rectangle frame = new Rectangle(0, frameIndex * Frame_Height, BannerTextureAsset.Value.Width, Frame_Height);
            return frame;
        }
    }
}
