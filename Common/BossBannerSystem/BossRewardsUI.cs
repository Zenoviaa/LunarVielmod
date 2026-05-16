using ReLogic.Content;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.BossBannerSystem;

public class CommonClaimButton : UIPanel
{
    private bool _needsClaiming;
    private float _scale;
    private Action _closeFunction;
    public CommonClaimButton(Action closeFunction) : base()
    {
        _closeFunction = closeFunction;
        ClaimButtonTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "ClaimButton");
        ClaimButtonCheckTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "ClaimButtonCheck");
        ClaimButtonOutlineTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "ClaimButtonOutline");
    }

    public bool alreadyClaimed;
    public bool notClaimed;
    public bool canClaim;
    public bool notBeaten;
    public readonly Asset<Texture2D> ClaimButtonCheckTextureAsset;
    public readonly Asset<Texture2D> ClaimButtonTextureAsset;
    public readonly Asset<Texture2D> ClaimButtonOutlineTextureAsset;
    public Color drawColor;
    public Color outlineColor;
    public string hoverText;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 46;
        Height.Pixels = 20;
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);
        _closeFunction();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        hoverText = LangText.Common("Claim");
        drawColor = Color.White;
        _needsClaiming = true;
        Color faintColor = Color.White * 0.5f;
        if (alreadyClaimed)
        {
            _needsClaiming = false;
            hoverText = LangText.Common("AlreadyClaimed");
            drawColor = faintColor;
        }
        else if (notClaimed)
        {
            hoverText = LangText.Common("NotClaimed");
            drawColor = faintColor;
        }
        if (notBeaten)
        {
            _needsClaiming = false;
            hoverText = LangText.Common("NotBeaten");
            drawColor = faintColor;
        }

        BackgroundColor = Color.Lerp(Color.Blue, Color.Black, 1f) * 0f;
        BorderColor = Color.Lerp(Color.Purple, Color.Black, 0.8f) * 0f;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        this.QuickMouseInteraction();
        Vector2 origin = ClaimButtonTextureAsset.Value.Size() * 0.5f;
        Rectangle rect = GetDimensions().ToRectangle();
        Vector2 backgroundDrawPosition = GetDimensions().ToRectangle().TopLeft();
        backgroundDrawPosition += origin;

        if (IsMouseHovering)
        {
            _scale = MathHelper.Lerp(_scale, 1.2f, 0.3f);
            outlineColor = Color.Lerp(outlineColor, Color.Yellow, 0.2f);
        }
        else
        {
            _scale = MathHelper.Lerp(_scale, 1f, 0.3f);
            outlineColor = Color.Lerp(outlineColor, Color.Transparent, 0.2f);
        }

        if(_needsClaiming)
        {
            outlineColor = Main.DiscoColor;
        }
        spriteBatch.Draw(ClaimButtonOutlineTextureAsset.Value, backgroundDrawPosition, null, outlineColor, 0, origin, _scale, SpriteEffects.None, 0);
        spriteBatch.Draw(ClaimButtonTextureAsset.Value, backgroundDrawPosition, null, drawColor, 0, origin, _scale, SpriteEffects.None, 0 );
        if (alreadyClaimed)
        {
            spriteBatch.Draw(ClaimButtonCheckTextureAsset.Value, backgroundDrawPosition, null, Color.Green, 0, origin, _scale, SpriteEffects.None, 0);
        }

    }
}
/// <summary>
/// A group of rewards that you can claim
/// </summary>
public class BossRewardsGroupUI : UIPanel
{
    private CommonClaimButton _claimButton;
    private readonly BossPageUI _parent;
    public BossRewardsGroupUI(BossPageUI parent, BossPageRewardType rewardsToShow)
    {
        _claimButton = new CommonClaimButton(ClaimRewards);
        _parent = parent;
        RewardType = rewardsToShow;
        RewardsUI = new BossRewardsUI(parent, rewardsToShow);

    }

    public readonly BossRewardsUI RewardsUI;
    public readonly BossPageRewardType RewardType;
    public BossPage BossPage => _parent.BossPage;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width = _parent.Width;
        Height.Pixels = 64;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        Append(RewardsUI);
        Append(_claimButton);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        UpdateClaimButton();
    }
    private void UpdateClaimButton()
    {
    
        _claimButton.Left.Pixels = Width.Pixels - _claimButton.Width.Pixels - 40;
        _claimButton.Top.Pixels = -8;

        _claimButton.alreadyClaimed = false;
        _claimButton.notBeaten = true;
        _claimButton.canClaim = false;
        _claimButton.notClaimed = false;


        //idk why im confusing myself with this, this should be so simple.
        if (HasAlreadyClaimed())
        {
            _claimButton.alreadyClaimed = true;
        }

        if (CanClaimRewards())
        {
            _claimButton.notBeaten = false;
        }

        if (!_claimButton.notBeaten && !HasAlreadyClaimed())
        {
            _claimButton.canClaim = true;
        }


        if (_parent.Page != 2)
            _claimButton.Top.Pixels = 99999;
    }
    public bool HasAlreadyClaimed()
    {
        DownedBossRewardPlayer rewardPlayer = Main.LocalPlayer.GetModPlayer<DownedBossRewardPlayer>();
        int flag = (int)BossPage.flag;
        switch (RewardType)
        {
            default:
            case BossPageRewardType.Rewards:
                return rewardPlayer.claimedRegularRewards[flag];
            case BossPageRewardType.MasterModeRewards:
                return rewardPlayer.claimedMasterRewards[flag];
            case BossPageRewardType.NoHitRewards:
                return rewardPlayer.claimedNoHit[flag];
        }
    }
    public bool CanClaimRewards()
    {
        int flag = (int)BossPage.flag;
        switch (RewardType)
        {
            default:
            case BossPageRewardType.Rewards:
                return BossPage.CanClaimRewards();
            case BossPageRewardType.MasterModeRewards:
                return BossPage.CanClaimMasterRewards();
            case BossPageRewardType.NoHitRewards:
                return BossPage.CanClaimNoHitRewards();
        }
    }
    public void ClaimRewards()
    {
        if (!CanClaimRewards())
            return;
        if (HasAlreadyClaimed())
            return;

        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Harv1"));
        DownedBossRewardPlayer rewardPlayer = Main.LocalPlayer.GetModPlayer<DownedBossRewardPlayer>();
        int flag = (int)BossPage.flag;
        switch (RewardType)
        {
            case BossPageRewardType.Rewards:
                BossPage.Grant(BossPage.Rewards);
                rewardPlayer.claimedRegularRewards[flag] = true;
                break;
            case BossPageRewardType.MasterModeRewards:
                BossPage.Grant(BossPage.MasterModeRewards);
                rewardPlayer.claimedMasterRewards[flag] = true;
                break;
            case BossPageRewardType.NoHitRewards:
                BossPage.Grant(BossPage.NoHitRewards);
                rewardPlayer.claimedNoHit[flag] = true;
                break;
        }
    }
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {

        base.DrawSelf(spriteBatch);
    }
}
/// <summary>
/// Draws the rewards that a boss hass
/// </summary>
public class BossRewardsUI : UIPanel
{
    private BossPageRewardType _rewardsToShow;
    private readonly int _rewardContext;
    private readonly BossPageUI _parent;
    private BossPage _bossPage;
    private UIText _difficultyText;
    public BossRewardsUI(BossPageUI parent, BossPageRewardType rewardsToShow)
    {
        _parent = parent;
        _rewardContext = ItemSlot.Context.BankItem;
        _difficultyText = new UIText("Rewards");
        _difficultyText.Top.Pixels = -48;
        _difficultyText.Left.Pixels = -32;
        _difficultyText.SetText(GetRewardsText(rewardsToShow));
        _rewardsToShow = rewardsToShow;
        TreasureBackgroundTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "TreasureBackground");
    }

    public readonly Asset<Texture2D> TreasureBackgroundTextureAsset;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width = _parent.Width;
        Height.Pixels = 32;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        Append(_difficultyText);
    }

    public void SetBossPage(BossPage bossPage)
    {
        _bossPage = bossPage;
    }


    private string GetRewardsText(BossPageRewardType rewardType)
    {
        string text = LangText.BossBanners("RewardsNormal");
        switch (rewardType)
        {
            case BossPageRewardType.NoHitRewards:
                text = LangText.BossBanners("RewardsNoHit");
                break;
            case BossPageRewardType.MasterModeRewards:
                text = LangText.BossBanners("RewardsMaster");
                break;

        }
        return text;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        DrawRewards(spriteBatch);
    }


    private void DrawRewards(SpriteBatch spriteBatch)
    {
        _difficultyText.SetText(GetRewardsText(_rewardsToShow));
     //   Height.Pixels = 64;
        Vector2 backgroundDrawPosition = GetDimensions().ToRectangle().TopLeft() - new Vector2(24, 16);
        spriteBatch.Draw(TreasureBackgroundTextureAsset.Value, backgroundDrawPosition, Color.White);

        Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
        List<Item> rewards = _bossPage.GetRewards(_rewardsToShow);
        if (rewards.Count == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                var ivythorn = ModContent.GetInstance<Ivythorn>();
                rewards.Add(ivythorn.Item);
            }
        }
        for (int i = 0; i < rewards.Count; i++)
        {
            Item reward = rewards[i];
            float distanceBetween = 36;
            Vector2 drawPosition = topLeft + new Vector2(distanceBetween * i, 0);
            Color drawColor = Color.White;
            switch (_rewardsToShow)
            {
                case BossPageRewardType.Rewards:
                    if (!_bossPage.CanClaimRewards())
                    {
                        drawColor = Color.Black;
                    }
                    break;
                case BossPageRewardType.MasterModeRewards:
                    if (!_bossPage.CanClaimMasterRewards())
                    {
                        drawColor = Color.Black;
                    }
                    break;
                case BossPageRewardType.NoHitRewards:
                    if (!_bossPage.CanClaimNoHitRewards())
                    {
                        drawColor = Color.Black;
                    }
                    break;
            }

            ItemSlot.DrawItemIcon(reward, _rewardContext, spriteBatch, drawPosition, 1, 32, drawColor);
            Vector2 mousePos = Main.MouseScreen;


            Point topLeftRec = new Point((int)drawPosition.X, (int)drawPosition.Y);
            topLeftRec.X -= 16;
            topLeftRec.Y -= 16;
            Rectangle rec = new Rectangle(topLeftRec.X, topLeftRec.Y, 32, 32);

            //Primitives2D.DrawRectangle(Main.spriteBatch, rec, Color.Red);
            if (rec.Contains(mousePos.ToPoint()))
            {
                Main.HoverItem = new Item(reward.type);
                Main.hoverItemName = reward.Name;
            }
        }
    }
}
