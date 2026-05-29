using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Core.PlayerLevelingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.UI.CollectionSystem;

public class LevelTabStatsPanel : UIPanel
{
    private Asset<Texture2D> _helpBackgroundTextureAsset;
    private Asset<Texture2D> _statIconTextureAsset;
    private Asset<Texture2D> _statPointTextureAsset;

    public const int width = 480;
    public const int height = 155;

    public int RelativeLeft => Main.screenWidth / 2 - width / 2 + 288;
    public int RelativeTop => Main.screenHeight / 2 - height / 2 - 196;
    public LevelTabStatsPanel() : base()
    {
        _statIconTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "StatIcon");
        _statPointTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "StatPoint");
        _helpBackgroundTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "HelpBackground");
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 48 * 6f;
        Height.Pixels = 48 * 9;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
    }


    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
    }

    public void DrawPanel(SpriteBatch spriteBatch, Texture2D texture, Rectangle targetRect, Color color)
    {
        int _cornerSize = 48;
        int _barSize = 0;
        Point point = new Point((int)targetRect.X, (int)targetRect.Y);
        Point point2 = new Point(point.X + (int)targetRect.Width - _cornerSize, point.Y + (int)targetRect.Height - _cornerSize);
        int width = point2.X - point.X - _cornerSize;
        int height = point2.Y - point.Y - _cornerSize;
        spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, _cornerSize, _cornerSize), new Rectangle(0, 0, _cornerSize, _cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, _cornerSize, _cornerSize), new Rectangle(_cornerSize + _barSize, 0, _cornerSize, _cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, _cornerSize, _cornerSize), new Rectangle(0, _cornerSize + _barSize, _cornerSize, _cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, _cornerSize, _cornerSize), new Rectangle(_cornerSize + _barSize, _cornerSize + _barSize, _cornerSize, _cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(point.X + _cornerSize, point.Y, width, _cornerSize), new Rectangle(_cornerSize, 0, _barSize, _cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(point.X + _cornerSize, point2.Y, width, _cornerSize), new Rectangle(_cornerSize, _cornerSize + _barSize, _barSize, _cornerSize), color);
        spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + _cornerSize, _cornerSize, height), new Rectangle(0, _cornerSize, _cornerSize, _barSize), color);
        spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + _cornerSize, _cornerSize, height), new Rectangle(_cornerSize + _barSize, _cornerSize, _cornerSize, _barSize), color);
        spriteBatch.Draw(texture, new Rectangle(point.X + _cornerSize, point.Y + _cornerSize, width, height), new Rectangle(_cornerSize, _cornerSize, _barSize, _barSize), color);
    }

    private void DrawStatAmounts(SpriteBatch spriteBatch)
    {
        float yOffset = 220;
        float xOffset = 20;
        float offsetDist = 28;
        for (int i = 0; i < LevelingPlayer.stats.Length; i++)
        {
            float appliedPoints = LevelingPlayer.stats[i];
            float proposedPoints = Leveler.proposedStatChanges[i];
            float totalPoints = appliedPoints + proposedPoints;

            Vector2 iconPosition = GetDimensions().ToRectangle().TopLeft();
            iconPosition.Y += offsetDist * i;
            iconPosition.Y += yOffset;
            iconPosition.X += -26;
            iconPosition.Y += 12;

            string e = $"{totalPoints}";

            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(e);
            Vector2 origin = textSize * new Vector2(0f, 0.5f);
            Color color = Color.White;
            if(proposedPoints > 0)
            {
                color = Color.LightGreen;
            }
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, e, iconPosition,
    color * ExtraMath.Osc(0.48f, 1f, speed: 3), 0, origin, Vector2.One * 1f);


        }
    }
    private void DrawStatSummary(SpriteBatch spriteBatch, Vector2 drawOffset, bool showCurrent)
    {

        string statText = Leveler.GetHelpString(Leveler.selectedMedal, false);
        Vector2 rpPosition = GetDimensions().ToRectangle().TopLeft();
        rpPosition.X += 18;
        rpPosition.Y += 8;
        rpPosition.Y += ExtraMath.Osc(0, 4f, speed: 1);
        rpPosition += drawOffset;
        Vector2 textSize = FontAssets.MouseText.Value.MeasureString(statText);

        Color drawColor = Color.White;
        if (Leveler.proposedStatChanges[Leveler.selectedMedal] > 0)
        {
            drawColor = Color.LightGreen;
        }
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, statText, rpPosition,
            drawColor * ExtraMath.Osc(0.48f, 1f, speed: 3), 0, Vector2.Zero, Vector2.One * 1f, 300);


    }
    private LevelingPlayer LevelingPlayer => Main.LocalPlayer.GetModPlayer<LevelingPlayer>();
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Rectangle targetDrawRect = GetDimensions().ToRectangle();
        targetDrawRect.Width = 332;
        targetDrawRect.Height = 210;
        DrawPanel(spriteBatch, _helpBackgroundTextureAsset.Value, targetDrawRect, Color.White);


        float wPadding = (_statPointTextureAsset.Width() * 1f);
        float yOffset = 220;
        float xOffset = 20;
        float offsetDist = 28;
        float scale = 1f;
        for(int i = 0; i < LevelingPlayer.stats.Length; i++)
        {
            float appliedPoints = LevelingPlayer.stats[i];
            float proposedPoints = Leveler.proposedStatChanges[i];
            float totalPoints = appliedPoints + proposedPoints;

            Vector2 iconPosition = GetDimensions().ToRectangle().TopLeft();
            iconPosition.Y += offsetDist * i;
            iconPosition.Y += yOffset;
            iconPosition.X += 8;
            iconPosition.Y += 9;


         
            SpritebatchDrawer statIconDrawer = SpritebatchDrawer.FromTextureAsset(_statIconTextureAsset, Main.screenPosition + iconPosition);
            statIconDrawer.VerticalFrame(i, 7);
            statIconDrawer.CenterOrigin();
            statIconDrawer.scale *= 1.2f;
            statIconDrawer.color = Color.White;
            spriteBatch.Draw(statIconDrawer);

            for (int j = 0; j < DownedBossTracker.MaxPossiblePoints; j++)
            {
                Vector2 uiPosition = GetDimensions().ToRectangle().TopLeft();
                uiPosition.X += j * wPadding;
                uiPosition.Y += yOffset;
                uiPosition.Y += offsetDist * i;
                uiPosition.X += xOffset;

                Vector2 drawPosition = Main.screenPosition + uiPosition;
                drawPosition.X = MathF.Floor(drawPosition.X);
                drawPosition.Y = MathF.Floor(drawPosition.Y);
                SpritebatchDrawer bipDrawer = SpritebatchDrawer.FromTextureAsset(_statPointTextureAsset, drawPosition);
       
                bipDrawer.VerticalFrame(2, 3);
                bipDrawer.color = Color.White;
                bipDrawer.drawOrigin = Vector2.Zero;
                bipDrawer.scale *= scale;
                spriteBatch.Draw(bipDrawer);

               
            }

            for (int j = 0; j < DownedBossTracker.MaxPossiblePoints; j++)
            {
                Vector2 uiPosition = GetDimensions().ToRectangle().TopLeft();
                uiPosition.X += j * wPadding;
                uiPosition.Y += yOffset;
                uiPosition.Y += offsetDist * i;
                uiPosition.X += xOffset;
                Vector2 drawPosition = Main.screenPosition + uiPosition;
                drawPosition.X = MathF.Floor(drawPosition.X);
                drawPosition.Y = MathF.Floor(drawPosition.Y);
                SpritebatchDrawer bipDrawer = SpritebatchDrawer.FromTextureAsset(_statPointTextureAsset, drawPosition);
                bipDrawer.scale *= scale;
                if (appliedPoints > 0)
                {
                    bipDrawer.VerticalFrame(0, 3);
                    bipDrawer.color = Color.Lerp(Color.Goldenrod, Color.White, ExtraMath.Osc(0f, 1f, speed: 3, offset: j));
                    appliedPoints--;
                } else if (proposedPoints > 0)
                {
                    bipDrawer.VerticalFrame(0, 3);
                    bipDrawer.color = Color.Lerp(Color.LightGreen, Color.Green, ExtraMath.Osc(0f, 1f, speed: 8, offset: j)) * 0.5f;
                    proposedPoints--;
                } else
                {
                    bipDrawer.VerticalFrame(1, 3);
                    bipDrawer.color = Color.White;
                }

                bipDrawer.drawOrigin = Vector2.Zero;
                spriteBatch.Draw(bipDrawer);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, default, Main.UIScaleMatrix);

            DrawStatAmounts(spriteBatch);
            if (Leveler.selectedMedal != -1)
            {
              


                DrawStatSummary(spriteBatch, Vector2.Zero, false);


            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, default, Main.UIScaleMatrix);
        }
    }
}

public static class Leveler
{
    public static int selectedMedal;
    public static float[] proposedStatChanges = new float[7];


    public static float TotalProposedPoints
    {
        get
        {
            float p = 0;
            for(int i = 0; i < proposedStatChanges.Length; i++)
            {
                p += proposedStatChanges[i];
            }
            return p;
        }
    }
    
    public static string GetTitleString(int statID) => LangText.Common(GetLocalizationKey(statID)); 

    public static string GetHelpString(int statID, bool showCurrent)
    {
        float[] baseStats = new float[3];
        float[] originalStats = new float[3];
        float[] newStats = new float[3];
        bool[] asNumber = new bool[3];
        string[] p = new string[3]; 
        for(int i = 0; i < p.Length; i++)
        {
            p[i] = "P0";
        }
        switch (statID)
        {
            default:
            case 0:
                baseStats[0] = 0.01f;
                break;
            case 1:
                baseStats[0] = 0.01f;
                break;
            case 2:
                baseStats[0] = 0.01f;
                break;
            case 3:
                baseStats[0] = 0.01f;
                baseStats[1] = 0.01f;
                break;
            case 4:
                baseStats[0] = 0.05f;
                baseStats[1] = 0.005f;
                p[1] = "P1";
                break;
            case 5:
                baseStats[0] = 0.05f;
                baseStats[1] = 0.015f;
                break;
            case 6:
                baseStats[0] = 1;
                asNumber[0] = true;
                baseStats[1] = 0.05f;
                baseStats[2] = 0.02f;
                break;

        }

        for (int i = 0; i < baseStats.Length; i++)
        {
            originalStats[i] = baseStats[i] * Main.LocalPlayer.GetModPlayer<LevelingPlayer>().stats[statID];
            newStats[i] = baseStats[i] * (Main.LocalPlayer.GetModPlayer<LevelingPlayer>().stats[statID] + proposedStatChanges[statID]);
        }


        //Figure out what to show
        string[] statStrings = new string[3];
        for(int i = 0; i < statStrings.Length; i++)
        {
            if (proposedStatChanges[statID] <= 0)
            {
                if (asNumber[i])
                {
                    statStrings[i] = "+" + originalStats[i].ToString();
                }
                else
                {
                    statStrings[i] = "+" + originalStats[i].ToString(p[i]);
                }
            }

            if (proposedStatChanges[statID] > 0)
            {
                if (asNumber[i])
                {
                    statStrings[i] = "+" + originalStats[i].ToString() + " -> " + "+" + newStats[i].ToString();
                }
                else
                {
                    statStrings[i] = "+" + originalStats[i].ToString(p[i]) + " -> " + "+" + newStats[i].ToString(p[i]);
                }
              
            }
        }

        return LangText.Common(GetLocalizationKey(statID) + ".Stat", statStrings);
    }
    public static string GetLocalizationKey(int statID)
    {
        switch (statID)
        {
            default:
            case 0:
                return "Strength";
            case 1:
                return "Endurance";
            case 2:
                return "Agility";
            case 3:
                return "Dexterity";
            case 4:
                return "Focus";
            case 5:
                return "Resourcefulness";
            case 6:
                return "Veil";
        }
    }

    public static float SpeccablePoints
    {
        get
        {
            Player player = Main.LocalPlayer;
            LevelingPlayer levelingPlayer = player.GetModPlayer<LevelingPlayer>();
            return levelingPlayer.RemainingPoints - TotalProposedPoints;
        }
    }
    public static void ClearProposition()
    {
        for (int i = 0; i < proposedStatChanges.Length; i++)
            proposedStatChanges[i] = 0;
    }

    public static bool CanApplyPoints()
    {
        Player player = Main.LocalPlayer;
        LevelingPlayer levelingPlayer = player.GetModPlayer<LevelingPlayer>();
        if (!levelingPlayer.CanApplyPoints(TotalProposedPoints))
            return false;
        return true;
    }

    public static bool CanConfirmProposition()
    {
        return TotalProposedPoints > 0;
    }
    public static void ConfirmProposition()
    {
        Player player = Main.LocalPlayer;
        LevelingPlayer levelingPlayer = player.GetModPlayer<LevelingPlayer>();
        for(int i  = 0; i < proposedStatChanges.Length; i++)
        {
            levelingPlayer.stats[i] += proposedStatChanges[i];
            proposedStatChanges[i] = 0;
        }
    }

    public static void AddPoint(int medal)
    {
        if (!CanApplyPoints())
            return;
        proposedStatChanges[medal]++;
    }

    public static void RemovePoint(int medal)
    {
        proposedStatChanges[medal]--;
        if (proposedStatChanges[medal] <= 0)
            proposedStatChanges[medal] = 0;
    }
}

public class LevelingConfirmButton : UIPanel
{
    private Asset<Texture2D> _btnTextureAsset;
    private readonly Action _clickFunc;
    public LevelingConfirmButton(Action clickFunc)
    {
        _clickFunc = clickFunc;
        _btnTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "ConfirmButton");
    }
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 50;
        Height.Pixels = 24;
        OnLeftClick += Confirm;
    }

    private void Confirm(UIMouseEvent evt, UIElement listeningElement)
    {
        _clickFunc();
     //   throw new NotImplementedException();
    }

    public string textKey;
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        Width.Pixels = 100;
        base.DrawSelf(spriteBatch);
        this.QuickMouseInteraction();
        Vector2 uiPosition = GetDimensions().ToRectangle().Center();
        Vector2 drawPosition = Main.screenPosition + uiPosition;
        SpritebatchDrawer medalDrawer = SpritebatchDrawer.FromTextureAsset(_btnTextureAsset, drawPosition);
      
   //     bool isMouseHovering = GetDimensions().ToRectangle().Contains(Main.MouseScreen.ToPoint());
        int frame = IsMouseHovering ? 1 : 0;
        medalDrawer.VerticalFrame(frame, 2);
        medalDrawer.CenterOrigin();
        medalDrawer.color = Color.White;
        medalDrawer.scale *= 1.2f;
        spriteBatch.Draw(medalDrawer);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, default, Main.UIScaleMatrix);
        string statText = LangText.Common(textKey);
        Vector2 rpPosition = GetDimensions().ToRectangle().Center();
        rpPosition.Y += 4;
        Vector2 textSize = FontAssets.MouseText.Value.MeasureString(statText);
        Color drawColor = Color.White;
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, statText, rpPosition,
            drawColor * ExtraMath.Osc(0.48f, 1f, speed: 3), 0, textSize * 0.5f, Vector2.One * 1f, 300);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, default, Main.UIScaleMatrix);
    }
}

public class LevelingTabLeftPanel : UIPanel
{
    private struct Medal
    {
        public Vector2 drawOffset;
        public int statIndex;
    }

    private float _whiteFlash;
    private float[] _scales;
    private LevelingConfirmButton _confirmPointsBtn;
    private LevelingConfirmButton _cancelPointsBtn;
    private Asset<Texture2D> _shardTextureAsset;
    private Asset<Texture2D> _titleBackgroundTextureAsset;
    private Asset<Texture2D> _medalBackgroundTextureAsset;
    private Asset<Texture2D> _medalTextureAsset;
    private Asset<Texture2D> _countBackgroundTextureAsset;
    private Medal[] _medals;
    private bool _isMouseHovering;

    public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels - 12);
    public int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2 + 40 + 24);

    private LevelingPlayer LevelingPlayer => Main.LocalPlayer.GetModPlayer<LevelingPlayer>();
    public LevelingTabLeftPanel()
    {
        _confirmPointsBtn = new LevelingConfirmButton(ConfirmPoints);
        _confirmPointsBtn.textKey = "Ok";
        _cancelPointsBtn = new LevelingConfirmButton(CancelPoints);
        _cancelPointsBtn.textKey = "Cancel";
        _shardTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "MedalShard");
        _titleBackgroundTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "TitleBackground");
        _medalBackgroundTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "MedalBackground");
        _medalTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "Medal");
        _countBackgroundTextureAsset = ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "QuestBackground");
        _medals = new Medal[7];
        _scales = new float[7];
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 384;
        Height.Pixels = 480;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        OnLeftClick += AddPoint;
        OnRightClick += RemovePoint;
        _cancelPointsBtn.Top.Set(0f, 1f);
        Append(_cancelPointsBtn);

        _confirmPointsBtn.Top.Set(0f, 1f);
        Append(_confirmPointsBtn);
    }

    private void AddPoint(UIMouseEvent evt, UIElement listeningElement)
    {
        if (!_isMouseHovering)
            return;

        Leveler.AddPoint(Leveler.selectedMedal);
        SoundStyle hitSound = new SoundStyle("Stellamod/Assets/Sounds/HardRockHit") with { PitchVariance = 0.4f, Pitch = -0.8f };
        SoundEngine.PlaySound(hitSound);
        _scales[Leveler.selectedMedal] = 0.8f;
    }
    private void RemovePoint(UIMouseEvent evt, UIElement listeningElement)
    {
        if (!_isMouseHovering)
            return;

        Leveler.RemovePoint(Leveler.selectedMedal);
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public void ConfirmPoints()
    {
        if (!Leveler.CanConfirmProposition())
            return;
        Leveler.ConfirmProposition();
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Harv1"));
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Death_1"));
        for(float f = 0; f < 32; f++)
        {
            Rectangle rect = GetDimensions().ToRectangle();
            Vector2 pos = rect.TopLeft();
            pos.X += Main.rand.Next(0, rect.Width);
            pos.Y += Main.rand.Next(0, rect.Height);
            Vector2 vel = (pos - rect.Center()) * 0.2f;
            var sp = DustParticle.SpawnInUI(pos, vel);
            sp.dampening = 0.2f;
            sp.gravity = 0;
            sp.outerColor = Color.Gold;
            sp.noTileCollide = true;
            sp.Scale *= Main.rand.NextFloat(0.8f, 1f);
        }
        for (int i = 0; i < _scales.Length; i++)
        {
            _scales[i] = 0.4f;
        }
        _whiteFlash = 1f;
        ShakeScreenPosition.Shake = 8;
    }

    public void CancelPoints()
    {
        Leveler.ClearProposition();
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    public override void OnActivate()
    {
        base.OnActivate();

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        _confirmPointsBtn.Left.Set(0f, 0.67f);
        _confirmPointsBtn.Top.Set(0f, 0.81f);

        _cancelPointsBtn.Left.Set(0f, 0.67f);
        _cancelPointsBtn.Top.Set(0f, 0.91f);
        if (Main.rand.NextBool(24) && Main.hasFocus)
        {
            Rectangle rect = GetDimensions().ToRectangle();
            Vector2 pos = rect.TopLeft();
            pos.X += Main.rand.Next(0, rect.Width);
            pos.Y += Main.rand.Next(0, rect.Height);
            var sp = SparkleParticle.SpawnInUI(pos, -Vector2.UnitY);
            sp.flickering = true;
            sp.gravity = 0;
            sp.outerColor = Color.Gold;
            sp.noTileCollide = true;
            sp.Scale *= 0.5f;
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        this.QuickMouseInteraction();

        if (Keyboard.GetState().IsKeyDown(Keys.G))
        {
            //Main.NewText("g");
            DownedBossTracker.AllBosses();
        
        }

        for(int i = 0; i < _scales.Length; i++)
        {
            _scales[i] = MathHelper.Lerp(_scales[i], 1f, 0.1f);
        }
        _whiteFlash = MathHelper.Lerp(_whiteFlash, 0f, 0.1f);
        //   Main.NewText(DownedBossTracker.DownedBossCount);
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(_medalBackgroundTextureAsset, Main.screenPosition + GetDimensions().ToRectangle().Center());
        sbDrawer.scale *= 1.2f;
        sbDrawer.color = Color.White;
        spriteBatch.Draw(sbDrawer);

        SpritebatchDrawer titleBackgroundDrawer = SpritebatchDrawer.FromTextureAsset(_titleBackgroundTextureAsset, Main.screenPosition + GetDimensions().ToRectangle().Center() + new Vector2(0, -170));
        titleBackgroundDrawer.color = Color.White;
        titleBackgroundDrawer.scale *= 1.2f;
        spriteBatch.Draw(titleBackgroundDrawer);

        SpritebatchDrawer bgDrawer = SpritebatchDrawer.FromTextureAsset(_countBackgroundTextureAsset, Main.screenPosition + GetDimensions().ToRectangle().Center() + new Vector2(-120, 152));
        bgDrawer.color = Color.White;
        bgDrawer.LeftCenterOrigin();
        bgDrawer.drawOrigin.X += 24;
        //  shardDrawer.scale *= 1.2f;
        spriteBatch.Draw(bgDrawer);


        Vector2 shardUiPosition = GetDimensions().ToRectangle().Center() + new Vector2(-120, 152);
        Vector2 shardPos = Main.screenPosition + shardUiPosition;
        SpritebatchDrawer shardDrawer = SpritebatchDrawer.FromTextureAsset(_shardTextureAsset, shardPos);
        shardDrawer.color = Color.White;
        spriteBatch.Draw(shardDrawer);



        int length = _medals.Length;
        _medals[0].drawOffset = new Vector2(-52, -52);
        _medals[1].drawOffset = new Vector2(-66, 19);
        _medals[2].drawOffset = new Vector2(-32, 72);
        _medals[3].drawOffset = new Vector2(40, 72);
        _medals[4].drawOffset = new Vector2(66, 16);
        _medals[5].drawOffset = new Vector2(60, -56);
        _medals[6].drawOffset = new Vector2(-8, -8);

        for(int i = 0; i < _medals.Length; i++)
        {
            _medals[i].statIndex = i;
        }
        Leveler.selectedMedal = -1;
        bool blockRaycasts = false;
        _isMouseHovering = false;
        for (int i = 0; i < length; i++)
        {
            ref Medal medal = ref _medals[i];
            Vector2 uiPosition = GetDimensions().ToRectangle().Center() + medal.drawOffset;
            Vector2 drawPosition = Main.screenPosition + uiPosition;
            drawPosition.Y += ExtraMath.Osc(-2f, 2f, speed: 1, offset: i);
            drawPosition.Y += ExtraMath.Osc(-2f, 2f, speed: 0.3f);
            drawPosition.X += ExtraMath.Osc(-2f, 2f, speed: 1, offset: i + 3);
            drawPosition.X = MathF.Round(drawPosition.X);
            drawPosition.Y = MathF.Round(drawPosition.Y);

            bool isHovering = false;
            int size = 64;
            Rectangle clickRectangle = new Rectangle(
                (int)uiPosition.X - size / 2, 
                (int)uiPosition.Y - size/ 2, size, size);
            if (clickRectangle.Contains(Main.MouseScreen.ToPoint()) && !_isMouseHovering)
            {
                Leveler.selectedMedal = i;
                blockRaycasts = true;
                _isMouseHovering = true;
                isHovering = true;
             //   Main.LocalPlayer.mouseInterface = true;
            }

            SpritebatchDrawer medalDrawer = SpritebatchDrawer.FromTextureAsset(_medalTextureAsset, drawPosition);
            medalDrawer.VerticalFrame(i, 14);
            medalDrawer.CenterOrigin();
            medalDrawer.color = Color.White;
            medalDrawer.scale *= 1.2f * _scales[i];
            spriteBatch.Draw(medalDrawer);
            if (isHovering)
            {
                medalDrawer.VerticalFrame(i + 7, 14);
                medalDrawer.color *= ExtraMath.Osc(0.25f, 0.66f, speed: 4);
                medalDrawer.color.A = 0;
                spriteBatch.Draw(medalDrawer);
            }

            if (Leveler.proposedStatChanges[i] > 0)
            {
                medalDrawer.VerticalFrame(i + 7, 14);
                medalDrawer.color = Color.Green;
                medalDrawer.color *= ExtraMath.Osc(0.25f, 0.66f, speed: 4);
                medalDrawer.color.A = 0;
                spriteBatch.Draw(medalDrawer);
            }

            if (_whiteFlash > 0)
            {
                medalDrawer.VerticalFrame(i + 7, 14);
                medalDrawer.color = Color.White;
                medalDrawer.color *= _whiteFlash;
                medalDrawer.color.A = 0;
                spriteBatch.Draw(medalDrawer);
            }
        }
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, default, Main.UIScaleMatrix);

        //Draw remaining points text
        string remainingPoints = $"{Leveler.SpeccablePoints}";
        Vector2 rpPosition = shardUiPosition;
        rpPosition.X += 18;
        rpPosition.Y += ExtraMath.Osc(-2f, 2f, speed: 3);
        Vector2 t = FontAssets.DeathText.Value.MeasureString(remainingPoints) * new Vector2(0f, 0.5f);
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, remainingPoints, rpPosition,
            Color.White * ExtraMath.Osc(0.48f, 1f, speed: 3), 0, t, Vector2.One * 0.8f);

        if (_isMouseHovering)
        {
            Vector2 post =  GetDimensions().ToRectangle().Center();
            post.Y -= 164;
            string titleString = Leveler.GetTitleString(Leveler.selectedMedal);
            Vector2 textSize = FontAssets.DeathText.Value.MeasureString(titleString);
            Vector2 textOrigin = textSize * 0.5f;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, titleString, post, 
                Color.White, 0, textOrigin, Vector2.One * 0.8f);
        }
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, default, Main.UIScaleMatrix);
        if (blockRaycasts)
        {
            Main.LocalPlayer.mouseInterface = true;
        }
    }
}
