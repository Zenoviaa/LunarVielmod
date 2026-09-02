using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Rendering;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Common.GooberDialogue;

public static class GooberDialoguePresets
{
    public static GooberDialogueParameters Zui => new()
    {
        startGradientColor = new Color(240, 122, 35),
        endGradientColor = new Color(202, 68, 43),
        outlineColor = new Color(202, 68, 43),
        portraitTextureAsset = AssetReferences.Content.GooberPortraits.ZuiMiniPortrait.Asset,
        bubblePosition = Vector2.Zero,
        name = "You ain't put no text",
        text = string.Empty
    };
}

public interface IUpdateable
{
    void Update();
    bool IsActive { get; }
}
public class UpdateableSystem : ModSystem
{
    private static readonly List<IUpdateable> _inactiveUpdateables = new();
    public static readonly List<IUpdateable> Updateables = new();
    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        if (Updateables.Count <= 0)
            return;

        _inactiveUpdateables.Clear();
        foreach(var a in Updateables)
        {
            a.Update();
            if (!a.IsActive)
            {
                _inactiveUpdateables.Add(a);
            }
        }
        foreach (var item in _inactiveUpdateables)
            Updateables.Remove(item);
        _inactiveUpdateables.Clear();
    }
}
public class GooberDialogueSpeaker : 
    IUpdateable
{
    private int _textIndex;
    private float _timer;
    public GooberDialogueSpeaker(SpeechBubbleWrapper speechBubbleWrapper)
    {
        SpeechBubble = speechBubbleWrapper;
        timeBetweenTexts = 3;
        _timer = 0;
        talkingSound = AssetReferences.Assets.Sounds.AssassinsKnifeHit.Asset;
        isActive = true;
    }
    public bool isActive;
    public float timeBetweenTexts;
    public SoundStyle talkingSound;
    public readonly SpeechBubbleWrapper SpeechBubble;
    public bool IsActive => isActive; 
    public bool IsFinishedTyping()
    {
        return _textIndex > SpeechBubble.Bubble.parameters.text.Length;
    }

    public void Reset()
    {
        _timer = 0;
        _textIndex = 0;
    }

    public void Update()
    {
      //  isActive = false;
        if (!IsFinishedTyping())
        {
            _timer++;
            if (_timer >= timeBetweenTexts)
            {
                SpeechBubble.Bubble.parameters.textIndex = _textIndex;
                _textIndex++;
                _timer = 0;
                if (_textIndex % 3 == 0)
                    SoundEngine.PlaySound(talkingSound);
            }
        }
        else
        {
            isActive = false;
    
        }

    }
}


/// <summary>
/// Parameters for a speech bubble that's going to be drawn in the world
/// </summary>
public struct GooberDialogueParameters
{
    public Asset<Texture2D> portraitTextureAsset;
    public Color startGradientColor;
    public Color endGradientColor;
    public Color outlineColor;
    public Vector2 bubblePosition;
    public int textIndex;
    public string text;
    public string name;
}

public class SpeechBubble
{
    public GooberDialogueParameters parameters;
    public float activeTimer;
    public float inOutTimer;
    public float EaseInOut => EasingFunction.OutCirc(inOutTimer / EaseTime);
    public static float EaseTime => 45;
}


/// <summary>
/// Wrapper for a speech bubble class so we can automatically update the active timer when accessing it
/// </summary>
public class SpeechBubbleWrapper
{
    private readonly SpeechBubble _bubble;
    public SpeechBubbleWrapper(SpeechBubble bubble)
    {
        _bubble = bubble;
        _bubble.activeTimer = 10;
    }

    public SpeechBubble Bubble
    {
        get
        {
            _bubble.activeTimer = 10;
            return _bubble;
        }
    }
}
[Autoload(Side = ModSide.Client)]
public class GooberDialogueSystem : ModSystem
{
    private readonly static Quad<VertexPositionColorTexture> _squareQuad = new();
    private readonly static List<SpeechBubble> _speechBubbles = new();
    private RenderTargetProvider _pixelTarget = new RenderTargetProvider(RenderTargetParameters.DownsizedFunc(2));
    private RenderTargetProvider _boxRenderTarget = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    private RenderTargetProvider _boxRenderTargetSwap = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    public override void Load()
    {
        base.Load();


        On_Main.CheckMonoliths += RenderDialogueBox;
        On_Main.DrawPlayers_AfterProjectiles += RenderToScreen;
    }
    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();

     
        if (_speechBubbles.Count <= 0)
            return;
        foreach (var bubble in _speechBubbles)
        {
            bubble.activeTimer--;
            if (bubble.activeTimer <= 0)
            {
                bubble.inOutTimer--;
            }
            else
            {
                bubble.inOutTimer++;
            }
            bubble.inOutTimer = MathHelper.Clamp(bubble.inOutTimer, 0, SpeechBubble.EaseTime);
        }
        _speechBubbles.RemoveAll(x => x.inOutTimer <= 0);
    }

    private bool ShouldRender()
    {
        if (Main.gameMenu)
            return false;
        if (_speechBubbles.Count <= 0)
            return false;
        return true;
    }

    private void RenderToScreen(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        orig(self);
        if (!ShouldRender())
            return;
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
        spriteBatch.Draw(_pixelTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
        spriteBatch.End();

        SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.GooberPortraits.PortraitLine.Asset, Vector2.Zero);
        SpritebatchDrawer arrowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.GooberPortraits.DialogueArrow.Asset, Vector2.Zero);
        SpritebatchParams worldParams = SpritebatchParams.InWorldAndZoomed();
        spriteBatch.Begin(worldParams);
        foreach (var bubble in _speechBubbles)
        {
            var portraitDrawer = SpritebatchDrawer.FromTextureAsset(bubble.parameters.portraitTextureAsset, bubble.parameters.bubblePosition + new Vector2(-3, -48));
            portraitDrawer.color = Color.White;
            spriteBatch.Draw(portraitDrawer);

            lineDrawer.worldPosition = bubble.parameters.bubblePosition + new Vector2(0, 2);
            lineDrawer.color = Color.White;
            spriteBatch.Draw(lineDrawer);

            arrowDrawer.worldPosition = bubble.parameters.bubblePosition + new Vector2(368, 85);
            arrowDrawer.color = Color.White;
            spriteBatch.Draw(arrowDrawer);
        }
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        foreach (var bubble in _speechBubbles)
        {
            if (!string.IsNullOrEmpty(bubble.parameters.text))
            {
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch,
                    FontAssets.DeathText.Value,
                    bubble.parameters.text,
                    bubble.parameters.bubblePosition - Main.screenPosition + new Vector2(32, 0),
                    Color.White,
                    0,
                    Vector2.Zero,
                    Vector2.One * 0.5f,
                    maxWidth: 342);
            }
            if (!string.IsNullOrEmpty(bubble.parameters.name))
            {
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch,
                    FontAssets.DeathText.Value,
                    bubble.parameters.name,
                    bubble.parameters.bubblePosition + new Vector2(42, -46) - Main.screenPosition,
                    Color.White,
                    MathHelper.ToRadians(-8),
                    Vector2.Zero,
                    Vector2.One * 0.75f,
                    maxWidth: 128);
            }
        }
        spriteBatch.End();
    }

    private void RenderDialogueBox(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (!ShouldRender())
            return;
        foreach (var bubble in _speechBubbles)
        {
            RenderDialogueBoxToPixelTarget(bubble);
        }
    }

    private void PrepareQuad(Vector2 anchorPoint, Vector2 size, Color startColor, Color endColor)
    {
        //Top Left
        _squareQuad.vertices[0] = new VertexPositionColorTexture(new Vector3(anchorPoint.X, anchorPoint.Y, 0), startColor, Vector2.Zero);

        //Top Right
        _squareQuad.vertices[1] = new VertexPositionColorTexture(new Vector3(anchorPoint.X + size.X, anchorPoint.Y - 48 , 0), endColor, new Vector2(1, 0));

        //Bottom Left
        _squareQuad.vertices[2] = new VertexPositionColorTexture(new Vector3(anchorPoint.X + 16, anchorPoint.Y + size.Y - 48, 0), startColor, new Vector2(0, 1));

        //Bottom Right
        _squareQuad.vertices[3] = new VertexPositionColorTexture(new Vector3(anchorPoint.X + size.X + 8, anchorPoint.Y + size.Y - 8, 0), endColor, Vector2.One);
    }

    private void DrawOutline(RenderTarget2D src, RenderTarget2D dst, SpriteBatch spriteBatch, Effect effect, Color outlineColor)
    {
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        graphicsDevice.SetRenderTarget(dst);
        graphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            effect);
        spriteBatch.Draw(src, Vector2.Zero, outlineColor);
        spriteBatch.End();
    }

    private void RenderDialogueBoxToPixelTarget(SpeechBubble speechBubble)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        graphicsDevice.SetRenderTarget(_boxRenderTarget);
        graphicsDevice.Clear(Color.Transparent);
        graphicsDevice.RasterizerState = RasterizerState.CullNone;



        HlslSampler noiseSpriteSampler = new();
        noiseSpriteSampler.Texture = AssetReferences.Assets.Noise.PerlinBlurred.Asset.Value;
        noiseSpriteSampler.Sampler = SamplerState.PointClamp;

        var pass = AssetReferences.Effects.Generic.Square.CreatePrimitivesPass();
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.spriteSampler = noiseSpriteSampler;
        pass.Apply();
        pass.Shader.CurrentTechnique.Passes[0].Apply();
        PrepareQuad(
            speechBubble.parameters.bubblePosition,
            new Vector2(384, 128),
            speechBubble.parameters.startGradientColor * 0.8f
            , speechBubble.parameters.endGradientColor * 0.8f);
        _squareQuad.Draw();
        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            null,
            Main.GameViewMatrix.TransformationMatrix);
        SpritebatchDrawer tailDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.GooberPortraits.Tail.Asset, speechBubble.parameters.bubblePosition + new Vector2(0, 72));
        tailDrawer.color = speechBubble.parameters.startGradientColor;
        spriteBatch.Draw(tailDrawer);
        spriteBatch.End();

        HlslSampler spriteSampler = new();
        spriteSampler.Texture = _boxRenderTarget;
        spriteSampler.Sampler = SamplerState.PointClamp;

        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight) * 2;
        var outlinerPass = AssetReferences.Effects.Generic.Outliner.CreatePixelPass();

        outlinerPass.Parameters.spriteSampler = spriteSampler;
        outlinerPass.Parameters.texelSize = texelSize;
        outlinerPass.Apply();
        DrawOutline(_boxRenderTarget, _boxRenderTargetSwap, spriteBatch, outlinerPass.Shader, Color.White);
        DrawOutline(_boxRenderTargetSwap, _boxRenderTarget, spriteBatch, outlinerPass.Shader, Color.White);
        DrawOutline(_boxRenderTarget, _boxRenderTargetSwap, spriteBatch, outlinerPass.Shader, speechBubble.parameters.outlineColor);

        var noisePass = AssetReferences.Effects.Generic.Scroll.CreatePixelPass();
        noisePass.Parameters.time = Main.GlobalTimeWrappedHourly * 4;

        graphicsDevice.SetRenderTarget(_pixelTarget);
        graphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone);
        spriteBatch.Draw(_boxRenderTargetSwap, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
        spriteBatch.End();

        graphicsDevice.SetRenderTarget(null);
    }

    /// <summary>
    /// Creates a new speech bubble
    /// </summary>
    /// <returns></returns>
    public static SpeechBubbleWrapper CreateBubble()
    {
        SpeechBubble bubble = new SpeechBubble();
        _speechBubbles.Add(bubble);
        return new SpeechBubbleWrapper(bubble);
    }
}
