using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Common.UI;
using Stellamod.Common.WeaponUpgrade.UI;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Core.UI;


/// <summary>
/// Draws the spalsh art with Ereshkigal and Zui
/// </summary>
public class LoadingScreenArt : UIPanel
{

    public LoadingScreenArt()
    {

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

    }
}

public class FakeLoadingScreenUIState : UIState
{
    public LoadingScreenArt screenArt;
    public FakeLoadingScreenUIState()
    {

    }
    public override void OnInitialize()
    {
        screenArt = new LoadingScreenArt();
        Append(screenArt);
    }
}

public class SplashArtShader : CrystalShader<SplashArtShader> { }
[Autoload(Side = ModSide.Client)]
public class FakeLoadingScreen : ModSystem
{
    private int _invokeDelay;
    private On_WorldGen.orig_playWorld _playFunction;
    private float _alpha;
    private float _timer;
    private float _blackAlpha;
    private float _blackTimer;
    private int _tip;
    public enum Visibility
    {
        Invisible,
        Visible
    }

    private AnimationFramer _framer;
    private Asset<Texture2D> _runningWhiteZuiTextureAsset;
    private Asset<Texture2D> _loadingScreenSplashTextureAsset;

    public int MaxTips { get; set; }
    public float DelayTime => 480;
    public float FadeTime => 90;
    public Visibility visibility;

    public bool IsFullyInvisible()
    {
        return _timer <= 0 && visibility == Visibility.Invisible;
    }

    public override void Load()
    {
        base.Load();
        _loadingScreenSplashTextureAsset = ModContent.Request<Texture2D>($"{this.GetTypeDirectoryWithSlash()}LoadingScreenSplash");
        _runningWhiteZuiTextureAsset = ModContent.Request<Texture2D>($"{this.GetTypeDirectoryWithSlash()}RunningWhiteZui");
        On_Main.Draw_Inner += DrawLoadingScreen;
        On_Main.UpdateOldNPCShop += UpdateLoadingScreen;
        On_WorldGen.playWorld += OpenUI;
        On_WorldGen.FinishPlayWorld += CloseUI;
    }


    public override void Unload()
    {
        base.Unload();

        _loadingScreenSplashTextureAsset = null;
        _runningWhiteZuiTextureAsset = null;
        On_Main.PostDrawMenu -= DrawLoadingScreen;
        On_WorldGen.playWorld -= OpenUI;
        On_WorldGen.FinishPlayWorld -= CloseUI;
    }


    private void DrawLoadingScreen(On_Main.orig_Draw_Inner orig, Main self, GameTime gameTime)
    {
        orig(self, gameTime);

        //We gotta force the loading screen or there's a flicker
        DrawLoadingScreen_Inner();
    }


    private void UpdateLoadingScreen(On_Main.orig_UpdateOldNPCShop orig)
    {
        orig();
        //   float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _framer.maxFrame = 9;
        _framer.frameSpeed = 4;
        _framer.UpdateTick();

        if (_invokeDelay > 0)
        {
            _invokeDelay--;

        }
        if (_invokeDelay <= 0 && _playFunction != null)
        {
            _blackTimer++;
            if (_blackTimer >= 60f)
            {
                _playFunction();
                _playFunction = null;

            }

        }
        switch (visibility)
        {
            case Visibility.Invisible:
                _timer -= 1;
                if(_blackTimer > 0)
                {
                    _blackTimer -= 1;
                    if (_blackTimer <= 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GotInWorld"));
                    }
                }
         
                break;
            case Visibility.Visible:
                _timer += 1;
                break;
        }

        _blackTimer = MathHelper.Clamp(_blackTimer, 0f, 60f);
        _blackAlpha = EasingFunction.InOutSine(_blackTimer / 60f);
        _timer = MathHelper.Clamp(_timer, 0f, FadeTime * 2f);
        _alpha = EasingFunction.InOutSine(_timer / FadeTime);
    }


    private void DrawLoadingScreen_Inner()
    {
        if (_loadingScreenSplashTextureAsset == null)
            return;
        if (_alpha <= 0)
            return;
        SpriteBatch sb = Main.spriteBatch;

        //screen width and screen height aren't reliable for what we want to do here
        int width = sb.GraphicsDevice.Viewport.Bounds.Width;
        int height = sb.GraphicsDevice.Viewport.Bounds.Height;


        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, 
            null);


        Vector2 centerScreen = new Vector2(width, height) * 0.5f;
        //sb.Draw(_loadingScreenSplashTextureAsset.Value, centerScreen, null, Color.White * _alpha, 0, _loadingScreenSplashTextureAsset.Value.Size() * 0.5f, 0.9f, SpriteEffects.None, 0);
        
        Vector2 middleOffset = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
        SpritebatchDrawer splashArtDrawer = SpritebatchDrawer.FromTextureAsset(
            _loadingScreenSplashTextureAsset, Main.screenPosition + centerScreen);
        splashArtDrawer.color = Color.White * _alpha;
        splashArtDrawer.scale = Vector2.One * 0.925f * 0.39f;

        SpritebatchDrawer overlayDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile, Main.screenPosition + centerScreen);
        Rectangle screenRect = new Rectangle(0, 0, width, height);
        overlayDrawer.color = Color.Black * _alpha;
        overlayDrawer.dstRect = screenRect;
        overlayDrawer.drawOrigin = Vector2.Zero;
        sb.Draw(overlayDrawer);

        if(Main.gameMenu)
            sb.Draw(splashArtDrawer);




        if (Main.gameMenu)
        {
            float ratio = 1f - (_invokeDelay / DelayTime);
            float maxWidth = 768;
            int w = (int)MathHelper.Lerp(0f, maxWidth, ratio);

            float xOffset = width * 0.85f;
            xOffset -= maxWidth;
            Rectangle fillRect = new Rectangle((int)xOffset - 16, height - 64, w, 8);
            Rectangle fillRect2 = new Rectangle((int)xOffset - 16, height - 64, (int)maxWidth, 8);

            SpritebatchDrawer blackBarDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile, Main.screenPosition + centerScreen);
            blackBarDrawer.dstRect = fillRect2;
            blackBarDrawer.drawOrigin = Vector2.Zero;
            blackBarDrawer.color = Color.Black * 0.6f * _alpha;
            sb.Draw(blackBarDrawer);

            SpritebatchDrawer whiteBarDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile, Main.screenPosition + centerScreen);
            whiteBarDrawer.dstRect = fillRect;
            whiteBarDrawer.drawOrigin = Vector2.Zero;
            whiteBarDrawer.color *= _alpha;
            sb.Draw(whiteBarDrawer);


            SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile, Main.screenPosition + fillRect.TopRight() + new Vector2(0, 4));
            lineDrawer.scale = new Vector2(0.25f, 2f);
            lineDrawer.color *= _alpha;
            sb.Draw(lineDrawer);

            SpritebatchDrawer whiteZui = SpritebatchDrawer.FromTextureAsset(_runningWhiteZuiTextureAsset, Main.screenPosition + fillRect2.TopRight() + new Vector2(64, 0));
            whiteZui.sourceRect = _runningWhiteZuiTextureAsset.Value.GetFrame(_framer.frame, 9);
            whiteZui.BottomCenterOrigin();
            whiteZui.color *= _alpha;
            sb.Draw(whiteZui);


            string tipText = LangText.Tip(_tip);
            Vector2 tipOrigin = Vector2.Zero;
            Vector2 tipPosition = new Vector2(width, height) * new Vector2(0.1f, 1f) - new Vector2(0, 252);


            //Draw the backdrop for the help text
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer,
                ShaderContent.GetInstance<SpriteFadeShader>().Effect);

            SpritebatchDrawer backBoxDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile, Vector2.Zero);
            backBoxDrawer.drawOrigin = Vector2.Zero;
            Rectangle blackBoxDrawRect = new Rectangle((int)tipPosition.X - 252, (int)tipPosition.Y - 128, 1024, 384);
            backBoxDrawer.dstRect = blackBoxDrawRect;
            backBoxDrawer.color = Color.Black * _alpha;
            sb.Draw(backBoxDrawer);


            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer,
                null);


         //   sb.Draw(backBoxDrawer);

            string loadingText = LangText.Common("Loading");
            Vector2 loadingOrigin = FontAssets.DeathText.Value.MeasureString(loadingText) * new Vector2(1f, 0.5f);
            Vector2 textPosition = fillRect.TopLeft();
            textPosition.X += maxWidth;
            textPosition.Y -= 32;
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.DeathText.Value, loadingText, textPosition, Color.White * _alpha, 0, loadingOrigin, Vector2.One);

   
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, tipText, tipPosition, 
                Color.White * _alpha, 0, tipOrigin, Vector2.One * 1.4f, maxWidth: 512);

        }


        overlayDrawer.color = Color.Black * _blackAlpha;
        sb.Draw(overlayDrawer);

        sb.End();

        if (!Main.gameMenu)
            return;

        if (_blackTimer < 60)
            return;
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer,
            null);

        Vector2 origin = FontAssets.DeathText.Value.MeasureString(Main.statusText) * 0.5f;
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.DeathText.Value, Main.statusText, new Vector2(width, height) * new Vector2(0.5f, 0.95f), Color.White, 0, origin, Vector2.One);


        sb.End();
    }
    private void DrawLoadingScreen(On_Main.orig_PostDrawMenu orig, Point screenSizeCache, Point screenSizeCacheAfterScaling)
    {
        orig(screenSizeCache, screenSizeCacheAfterScaling);
        DrawLoadingScreen_Inner();
    }

    public override void OnModLoad()
    {
        base.OnModLoad();

    }

    private void CloseUI(On_WorldGen.orig_FinishPlayWorld orig)
    {
        visibility = Visibility.Invisible;
        orig();
    }

    private void OpenUI(On_WorldGen.orig_playWorld orig)
    {
        MaxTips = LangText.TipCount;
        visibility = Visibility.Visible;
        _tip = Main.rand.Next(MaxTips);
        _playFunction = orig;
        _invokeDelay = (int)DelayTime;
    }
}
