using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
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

    /// <summary>
    /// Base class for the right side page of the collection book
    /// </summary>
    public abstract class RightPageUI : UIPanel
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

        public void SetBossPage(BossPage bossPage)
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
            Rectangle rectangle = UIHelper.MouseInterfaceInteraction(this);
            spriteBatch.Draw(texture.Value, rectangle.TopLeft(), null, Color.White, 0f, default, 1, SpriteEffects.None, 0f);
        }
    }


    public class StoneGolemPage : BossPage
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            banner = BossBannerType.LifeNPlants;
        }
    }

    /// <summary>
    /// Draws the star ranking of a boss
    /// </summary>
    public class BossStarsUI : UIPanel
    {
        private BossPage _bossPage;
        private readonly BossPageUI _parent;
        public BossStarsUI(BossPageUI parent)
        {
            _parent = parent;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width = _parent.Width;
            Height.Pixels = 32;
        }

        public void SetBossPage(BossPage bossPage)
        {
            _bossPage = bossPage;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            DrawStars(spriteBatch);
        }

        private void DrawStars(SpriteBatch spriteBatch)
        {
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            var texture = BossBanner.RequestStarTexture();
            Color darkColor = Color.Lerp(Color.White, Color.Black, 0.8f);
            for (int i = 0; i < 7; i++)
            {
                float distanceBetween = 16;
                Vector2 drawPosition = topLeft + new Vector2(distanceBetween * i, 0);
                Color drawColor = i < _bossPage.StarRanking ? Color.White : darkColor;
                spriteBatch.Draw(texture.Value, drawPosition, null, drawColor, 0f, default, 1, SpriteEffects.None, 0f);
            }
        }
    }

    /// <summary>
    /// Draws the rewards that a boss hass
    /// </summary>
    public class BossRewardsUI : UIPanel
    {
        private readonly int _rewardContext;
        private readonly BossPageUI _parent;
        private BossPage _bossPage;
        public BossRewardsUI(BossPageUI parent)
        {
            _parent = parent;
            _rewardContext = ItemSlot.Context.BankItem;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width = _parent.Width;
            Height.Pixels = 32;
        }

        public void SetBossPage(BossPage bossPage)
        {
            _bossPage = bossPage;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            DrawRewards(spriteBatch);
        }

        private void DrawRewards(SpriteBatch spriteBatch)
        {
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            List<Item> rewards = _bossPage.Rewards;
            for (int i = 0; i < rewards.Count; i++)
            {
                Item reward = rewards[i];
                float distanceBetween = 16;
                Vector2 drawPosition = topLeft + new Vector2(distanceBetween * i, 0);
                ItemSlot.DrawItemIcon(reward, _rewardContext, spriteBatch, drawPosition, 2, 32, Color.White);
            }
        }
    }

    /// <summary>
    /// Opens up the lore tab for the boss
    /// </summary>
    public class BossLoreButtonUI : UIPanel
    {
        private readonly BossPageUI _parent;
        public BossLoreButtonUI(BossPageUI parent)
        {
            _parent = parent;
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 32;
            Height.Pixels = 32;
            OnLeftClick += _parent.ToggleLoreWindow;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Asset<Texture2D> glassTexture = BossBanner.RequestScrollTexture();
            Rectangle rectangle = UIHelper.MouseInterfaceInteraction(this);
            Vector2 drawPosition = rectangle.TopLeft();
            drawPosition.Y += ExtraMath.Osc(0f, 2f);
            if (IsMouseHovering)
            {
                UIHelper.QuickOutline(spriteBatch, glassTexture, drawPosition, Color.Yellow);
            }

            spriteBatch.Draw(glassTexture.Value, drawPosition, null, Color.White, 0f, default, 1, SpriteEffects.None, 0f);
        }
    }

    /// <summary>
    /// Opens a window that shows where you can find the boss
    /// </summary>
    public class BossFindButtonUI : UIPanel
    {
        private readonly BossPageUI _parent;
        public BossFindButtonUI(BossPageUI parent)
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
            OnLeftClick += _parent.ToggleLocationWindow;
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
            Rectangle rectangle = UIHelper.MouseInterfaceInteraction(this);
            Vector2 drawPosition = rectangle.TopLeft();

            //Adding a little bit of hover would be cool
            drawPosition.Y += ExtraMath.Osc(0f, 2f, speed: 1);

            //We also need a hover outline probably
            //I think I have a white shader somewhere
            if (IsMouseHovering)
            {
                UIHelper.QuickOutline(spriteBatch, texture.Value, drawPosition, Color.Yellow);
            }

            spriteBatch.Draw(texture.Value, drawPosition, null, Color.White, 0f, default, 1, SpriteEffects.None, 0f);
        }
    }

    /// <summary>
    /// Opens a window that shows all information about the boss
    /// </summary>
    public class BossPageUI : RightPageUI
    {
        private UIText _displayNameText;
        private BossFindButtonUI _glassUI;
        private BossLoreButtonUI _bossLoreUI;
        private BossPhotoUI _bossPhotoUI;
        private BossRewardsUI _bossRewardsUI;
        private BossStarsUI _bossStarsUI;
        public BossPageUI()
        {
            _displayNameText = new UIText("Your Mom");
            _glassUI = new BossFindButtonUI(this);
            _bossLoreUI = new BossLoreButtonUI(this);
            _bossPhotoUI = new BossPhotoUI(this);
            _bossRewardsUI = new BossRewardsUI(this);
            _bossStarsUI = new BossStarsUI(this);
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
            Append(_bossRewardsUI);
        }

        public void SetBossPage(BossPage bossPage)
        {
            BossPage = bossPage;
            _bossStarsUI.SetBossPage(bossPage);
            _bossRewardsUI.SetBossPage(bossPage);
            _bossPhotoUI.SetBossPage(bossPage);
            _displayNameText.SetText(bossPage.DisplayName);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
        }


        public void ToggleLocationWindow(UIMouseEvent evt, UIElement listeningElement)
        {
            throw new NotImplementedException();
        }

        public void ToggleLoreWindow(UIMouseEvent evt, UIElement listeningElement)
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
        public BossBannerType banner;
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
