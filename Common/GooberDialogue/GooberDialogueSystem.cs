using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Rendering.RTs;
using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Stellamod.Common.GooberDialogue;

[Autoload(Side = ModSide.Client)]
public class GooberDialogueSystem : ModSystem
{
    private static int _speechBubbleIndex;
    private readonly static Quad<VertexPositionColorTexture> _squareQuad = new();

    private static SpeechBubble[] SpeechBubbles
    {
        get
        {
            if(field == null)
            {
                field = new SpeechBubble[16];
                for (int i = 0; i < field.Length; i++)
                    field[i] = new SpeechBubble();
            }
            return field;
        }
    }

    private Vector2 BubbleOffset => new Vector2(64);
    private Vector2 BubbleOffsetForTail => new Vector2(96, -64);
    private Vector2 BubbleOffsetForElements => new Vector2(32, -128);
    public static event Action OnClearSpeechBubbles;
    public override void Load()
    {
        base.Load();

        On_Main.DrawPlayers_AfterProjectiles += RenderToScreen;
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        _speechBubbleIndex = 0;
        foreach(var speechBubble in SpeechBubbles)
        {
            speechBubble.showArrow = false;
        }
        OnClearSpeechBubbles?.Invoke();
        foreach(var speechBubble in SpeechBubbles)
        {
            speechBubble.activeTimer--;
            if(speechBubble.activeTimer <= 0)
            {
                speechBubble.activeTimer = 0;
                speechBubble.inOutTimer--;
            }
            else
            {
                speechBubble.inOutTimer++;
            }
            speechBubble.inOutTimer = MathHelper.Clamp(speechBubble.inOutTimer, 0, SpeechBubble.EaseTime);
        }
    }

    public static SpeechBubble GetSpeechBubble()
    {
        //failsafe
        if (_speechBubbleIndex >= SpeechBubbles.Length)
            return SpeechBubbles[0];
        var bubble =  SpeechBubbles[_speechBubbleIndex++];
        bubble.activeTimer = 15;
        return bubble;
    }

    private bool ShouldRender()
    {
        if (Main.gameMenu)
            return false;
        foreach(var bubble in SpeechBubbles)
        {
            if (bubble.IsActive())
                return true;
        }
        return false;
    }

    private void RenderToScreen(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        orig(self);
        if (!ShouldRender())
            return;



        //I think what we're going to do here is just render each bubble individually
        //We're not going to make a ton of bubbles so doing them one at a time is probably fine?
        foreach(var bubble in SpeechBubbles)
        {
            if (!bubble.IsActive())
                continue;
            if (!bubble.IsValid())
                continue;

            RenderTargetHandle pixelTarget = RenderTargets.HalfScreenTarget;
            RenderTargetHandle boxRenderTarget = RenderTargets.ScreenTarget;
            RenderTargetHandle boxRenderTargetSwap = RenderTargets.ScreenTarget;

            RenderDialogueBoxToPixelTarget(bubble, pixelTarget, boxRenderTarget, boxRenderTargetSwap);
            var spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            spriteBatch.Draw(pixelTarget,
                bubble.speaker.bubblePosition - Main.screenPosition + BubbleOffsetForTail, 
                null, 
                Color.White,
                0, 
                BubbleOffset, 
                2 * bubble.Scale, SpriteEffects.None, 0);
            spriteBatch.End();

            SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.GooberPortraits.PortraitLine.Asset, Vector2.Zero);
            SpritebatchDrawer arrowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.GooberPortraits.DialogueArrow.Asset, Vector2.Zero);
            SpritebatchParams worldParams = SpritebatchParams.InWorldAndZoomed();
            spriteBatch.Begin(worldParams);
            var portraitDrawer = SpritebatchDrawer.FromTextureAsset(bubble.speaker.profile.portraitTextureAsset, bubble.speaker.bubblePosition + new Vector2(-3, -48));
            portraitDrawer.color = Color.White;
            portraitDrawer.scale = Vector2.One * bubble.Scale;
            portraitDrawer.worldPosition += BubbleOffsetForElements;
            spriteBatch.Draw(portraitDrawer);

            lineDrawer.worldPosition = bubble.speaker.bubblePosition + new Vector2(0, 2);
            lineDrawer.worldPosition += BubbleOffsetForElements;
            lineDrawer.color = Color.White;
            lineDrawer.scale *= bubble.Scale;
            spriteBatch.Draw(lineDrawer);

            if (bubble.showArrow)
            {
                arrowDrawer.scale *= bubble.Scale;
                arrowDrawer.worldPosition = bubble.speaker.bubblePosition + new Vector2(368, 85);
                arrowDrawer.color = Color.White * ExtraMath.Osc(0.8f, 1f, speed: 3);
                arrowDrawer.worldPosition += BubbleOffsetForElements;
                spriteBatch.Draw(arrowDrawer);
            }

            spriteBatch.End();
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.TransformationMatrix);
            if (!string.IsNullOrEmpty(bubble.speaker.text))
            {
                var chatText = bubble.speaker.text;
                StringBuilder sb = new StringBuilder();
                int index = Math.Min(bubble.speaker.textIndex, chatText.Length);
                sb.Append(chatText.Substring(0, index));
          
                if(index < bubble.speaker.text.Length)
                {
                    sb.Append("[c/FFFF00:");
                    sb.Append(bubble.speaker.text.Substring(index, bubble.speaker.text.Length - index));
                    sb.Append("]");
                }
    
                /*
                for(int i = 0; i < chatText.Length; i++)
                {
                    if (i < bubble.speaker.textIndex || chatText[i] == ' ')
                        sb.Append(chatText[i]);
                    else
                    {
                        //Add invisible white space
                        //This makes it so the text doesn't just jump when it's wrapping,
                        //cause it already knows the general area of where the words go ahead of time
                        sb.Append("\u00A0");
                        sb.Append("\u00A0");

                    }
                }*/
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch,
                    FontAssets.DeathText.Value,
                    sb.ToString(),
                    bubble.speaker.bubblePosition - Main.screenPosition + new Vector2(32, 0) + BubbleOffsetForElements,
                    Color.White * bubble.Scale,
                    0,
                    Vector2.Zero,
                    Vector2.One * 0.5f,
                    maxWidth: 342);
            }

            if (!string.IsNullOrEmpty(bubble.speaker.profile.name))
            {
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch,
                    FontAssets.DeathText.Value,
                    bubble.speaker.profile.name,
                    bubble.speaker.bubblePosition + new Vector2(42, -46) - Main.screenPosition + BubbleOffsetForElements,
                    Color.White,
                    MathHelper.ToRadians(-8),
                    Vector2.Zero,
                    Vector2.One * 0.75f * bubble.Scale,
                    maxWidth: 128);
            }
            spriteBatch.End();
        }


    }


    private void PrepareQuad(Vector2 anchorPoint, Vector2 size, Color startColor, Color endColor)
    {
        //Top Left
        _squareQuad.vertices[0] = new VertexPositionColorTexture(new Vector3(anchorPoint.X, anchorPoint.Y, 0), startColor, Vector2.Zero);

        //Top Right
        _squareQuad.vertices[1] = new VertexPositionColorTexture(new Vector3(anchorPoint.X + size.X, anchorPoint.Y - 48, 0), endColor, new Vector2(1, 0));

        //Bottom Left
        _squareQuad.vertices[2] = new VertexPositionColorTexture(new Vector3(anchorPoint.X + 16, anchorPoint.Y + size.Y - 48, 0), startColor, new Vector2(0, 1));

        //Bottom Right
        _squareQuad.vertices[3] = new VertexPositionColorTexture(new Vector3(anchorPoint.X + size.X + 8, anchorPoint.Y + size.Y - 8, 0), endColor, Vector2.One);
    }

    private void DrawOutline(RenderTargetHandle src, RenderTargetHandle dst, SpriteBatch spriteBatch, Effect effect, Color outlineColor)
    {
        using (new RenderTargetContext(dst))
        {
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
    }


    private void RenderDialogueBoxToPixelTarget(SpeechBubble speechBubble, RenderTargetHandle pixelTarget, RenderTargetHandle boxRenderTarget, RenderTargetHandle boxRenderTargetSwap)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        using (new RenderTargetContext(boxRenderTarget))
        {
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            HlslSampler noiseSpriteSampler = new();
            noiseSpriteSampler.Texture = AssetReferences.Assets.Noise.PerlinBlurred.Asset.Value;
            noiseSpriteSampler.Sampler = SamplerState.PointClamp;

            var pass = AssetReferences.Effects.Generic.Square.CreatePrimitivesPass();
            pass.Parameters.transformMatrix = TrailDrawer.ViewProjection;
            pass.Parameters.time = Main.GlobalTimeWrappedHourly;
            pass.Parameters.spriteSampler = noiseSpriteSampler;
            pass.Apply();
            pass.Shader.CurrentTechnique.Passes[0].Apply();
            var offset = BubbleOffset;
            PrepareQuad(
                offset,
                new Vector2(384, 128),
                speechBubble.speaker.profile.startGradientColor * 0.8f
                , speechBubble.speaker.profile.endGradientColor * 0.8f);
            _squareQuad.Draw();
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.TransformationMatrix);
            SpritebatchDrawer tailDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.GooberPortraits.Tail.Asset, Main.screenPosition + offset + new Vector2(0, 72));
            tailDrawer.color = speechBubble.speaker.profile.startGradientColor;
            spriteBatch.Draw(tailDrawer);
            spriteBatch.End();
        }

        HlslSampler spriteSampler = new();
        spriteSampler.Texture = boxRenderTarget;
        spriteSampler.Sampler = SamplerState.PointClamp;

        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight) * 2;
        var outlinerPass = AssetReferences.Effects.Generic.Outliner.CreatePixelPass();

        outlinerPass.Parameters.spriteSampler = spriteSampler;
        outlinerPass.Parameters.texelSize = texelSize;
        outlinerPass.Apply();
        DrawOutline(boxRenderTarget, boxRenderTargetSwap, spriteBatch, outlinerPass.Shader, Color.White);
        DrawOutline(boxRenderTargetSwap, boxRenderTarget, spriteBatch, outlinerPass.Shader, Color.White);
        DrawOutline(boxRenderTarget, boxRenderTargetSwap, spriteBatch, outlinerPass.Shader, speechBubble.speaker.profile.outlineColor);

        var noisePass = AssetReferences.Effects.Generic.Scroll.CreatePixelPass();
        noisePass.Parameters.time = Main.GlobalTimeWrappedHourly * 4;

        using (new RenderTargetContext(pixelTarget))
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone);
            spriteBatch.Draw(boxRenderTargetSwap, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }

}

