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
using static Stellamod.Core.LocalizationReferences.Mods.Stellamod.Projectiles;

namespace Stellamod.Common.GooberDialogue;

[Autoload(Side = ModSide.Client)]
public class GooberDialogueSystem : ModSystem
{
    private static int _speechBubbleIndex;
    private Vector2 _speechBubbleSize;
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
        On_Main.DrawInfernoRings += RenderToScreenOverWater;
    }

    private void RenderToScreenOverWater(On_Main.orig_DrawInfernoRings orig, Main self)
    {
        orig(self);
        if (!ShouldRender())
            return;

        var spriteBatch = Main.spriteBatch;
        spriteBatch.EndOut(out var oldParameters);

        //I think what we're going to do here is just render each bubble individually
        //We're not going to make a ton of bubbles so doing them one at a time is probably fine?
        foreach (var bubble in SpeechBubbles)
        {
            if (!bubble.IsActive())
                continue;
            if (!bubble.IsValid())
                continue;
            Vector2 pos = _squareQuad.vertices[0].Position.XY() + bubble.speaker.bubblePosition + new Vector2(-64, -112);
            pos += new Vector2(16, -4);
            var portraitDrawer = SpritebatchDrawer.FromTextureAsset(bubble.speaker.profile.portraitTextureAsset, pos);
            portraitDrawer.color = Color.White;
            portraitDrawer.scale = Vector2.One * bubble.Scale;
            portraitDrawer.rotation = MathHelper.ToRadians(-7);
            portraitDrawer.worldPosition += BubbleOffsetForElements;


            RenderTargetHandle pixelTarget = RenderTargets.HalfScreenTarget;
            RenderTargetHandle boxRenderTarget = RenderTargets.ScreenTarget;
            RenderTargetHandle boxRenderTargetSwap = RenderTargets.ScreenTarget;


            RenderDialogueBoxToPixelTarget(bubble, pixelTarget, boxRenderTarget, boxRenderTargetSwap);

            //Prepare a mask for the portrait to mask onto
            using(new RenderTargetContext(boxRenderTarget))
            {
                using(new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.Identity }))
                {
                    spriteBatch.Draw(pixelTarget,
                        bubble.speaker.bubblePosition - Main.screenPosition + BubbleOffsetForTail,
                        null,
                        Color.White,
                        0,
                        BubbleOffset,
                        2 * bubble.Scale, SpriteEffects.None, 0);
                }
            }

            //Prpeare the portrait draw
            using (new RenderTargetContext(boxRenderTargetSwap))
            {
                using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed()))
                {

                    var nameTagDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.GooberPortraits.NameTag.Asset, Vector2.Zero);
                    var gradientPass = AssetReferences.Effects.CrystalShaders.SimpleGradient.CreatePixelPass();
                    gradientPass.Parameters.startGradientColor = bubble.speaker.profile.startGradientColor.ToVector4();
                    gradientPass.Parameters.endGradientColor = bubble.speaker.profile.endGradientColor.ToVector4();
                    gradientPass.Apply();
                    var nameSize = ChatManager.GetStringSize(FontAssets.DeathText.Value,
                         bubble.speaker.profile.name, Vector2.One * 0.5f, -1);
                    using (new SpritebatchContext(spriteBatch, spriteBatch.Parameters with { effect = gradientPass.Shader }))
                    {
                        nameTagDrawer.worldPosition = pos;
                        nameTagDrawer.worldPosition += new Vector2(64, -100);
                        nameTagDrawer.rotation = MathHelper.ToRadians(-8);
                        nameTagDrawer.color = Color.White * bubble.Scale;
                        nameTagDrawer.LeftCenterOrigin();

                        float xSize = nameSize.X / nameTagDrawer.texture.Width;
                        xSize += 0.14f;
                        nameTagDrawer.scale *= new Vector2(xSize, 1f);
                        nameTagDrawer.scale *= bubble.Scale;
                        spriteBatch.Draw(nameTagDrawer);
                    }

                    spriteBatch.Draw(portraitDrawer);
                }
            }


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);



            var afDrawer = portraitDrawer;
            afDrawer.worldPosition += new Vector2(8, 0);
            var whiteShader = ShaderContent.GetInstance<SpriteWhiteShader>();
            using (new SpritebatchContext(spriteBatch, spriteBatch.Parameters with { effect = whiteShader }))
            {
                afDrawer.color = bubble.speaker.profile.startGradientColor;
                spriteBatch.Draw(afDrawer);
            }


            spriteBatch.Draw(pixelTarget,
                bubble.speaker.bubblePosition - Main.screenPosition + BubbleOffsetForTail,
                null,
                Color.White,
                0,
                BubbleOffset,
                2 * bubble.Scale, SpriteEffects.None, 0);
            spriteBatch.End();



            var invertedMask = AssetReferences.Effects.Generic.InvertedMask.CreatePixelPass();
            invertedMask.Parameters.maskSampler = new()
            {
                Texture = boxRenderTarget,
                Sampler = SamplerState.PointClamp
            };
            invertedMask.Apply();
            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity, effect = invertedMask.Shader }))
            {
                spriteBatch.Draw(boxRenderTargetSwap, Vector2.Zero, Color.White);
            }

            SpritebatchDrawer arrowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.GooberPortraits.DialogueArrow.Asset, Vector2.Zero);
            SpritebatchParams worldParams = SpritebatchParams.InWorldAndZoomed();
            spriteBatch.Begin(worldParams);



           // spriteBatch.Draw(portraitDrawer);

            if (bubble.showArrow)
            {
                arrowDrawer.scale *= bubble.Scale;
                arrowDrawer.worldPosition = _squareQuad.vertices[3].Position.XY() + bubble.speaker.bubblePosition + new Vector2(-64, -222);

                arrowDrawer.color = Color.White * ExtraMath.Osc(0.8f, 1f, speed: 3);
  
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
                for (int i = 0; i < chatText.Length; i++)
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
                }
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch,
                    FontAssets.DeathText.Value,
                    sb.ToString(),
                    bubble.speaker.bubblePosition - Main.screenPosition + new Vector2(32, 9) + BubbleOffsetForElements,
                    Color.White * bubble.Scale,
                    0,
                    Vector2.Zero,
                    Vector2.One * 0.5f,
                    maxWidth: 342);
            }

            if (!string.IsNullOrEmpty(bubble.speaker.profile.name))
            {

                var tr = _squareQuad.vertices[1].Position.XY();
                var tl = _squareQuad.vertices[0].Position.XY();
                var nameRot = (tr - tl).ToRotation();
                var dist = Vector2.Distance(tl, tr);
                var interp = 1f - ExtraMath.Saturate(dist / 256);
                var upOffset = interp * 20;
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch,
                    FontAssets.DeathText.Value,
                    bubble.speaker.profile.name,
                    pos - Main.screenPosition + BubbleOffsetForElements + new Vector2(36, 7) + new Vector2(0, -upOffset),
                    Color.White,
                   nameRot,
                    Vector2.Zero,
                    Vector2.One * 0.75f * bubble.Scale,
                    maxWidth: 128);
            }
            spriteBatch.End();
        }
        spriteBatch.Begin(oldParameters);
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



    }


    private void PrepareQuad(Vector2 anchorPoint, Vector2 size, Color startColor, Color endColor)
    {

        float yRange = 4;
        Vector3 topLeftOffset = new Vector3();
        topLeftOffset.X = ExtraMath.Osc(-16f, 16f, speed: 1);
        topLeftOffset.Y = ExtraMath.Osc(-yRange, yRange, speed: 1);

        Vector3 topRightOffset = new Vector3();
        topRightOffset.X = ExtraMath.Osc(-16f, 16f, speed: 1);
        topRightOffset.Y = ExtraMath.Osc(-yRange, yRange, speed: 1, offset: 3.14f);

        Vector3 bottomLeftOffset = new Vector3();
        bottomLeftOffset.X = ExtraMath.Osc(-16f, 16f, speed: 1, offset: 3.14f);
        bottomLeftOffset.Y = ExtraMath.Osc(-yRange, yRange, speed: 1);

        Vector3 bottomRightOffset = new Vector3();
        bottomRightOffset.X = ExtraMath.Osc(-16f, 16f, speed: 1, offset: 3.14f);
        bottomRightOffset.Y = ExtraMath.Osc(-yRange, yRange, speed: 1, offset: 3.14f);

        //Top Left
        _squareQuad.vertices[0] = new VertexPositionColorTexture(
            new Vector3(anchorPoint.X, anchorPoint.Y, 0) + topLeftOffset, startColor, Vector2.Zero);

        //Top Right
        _squareQuad.vertices[1] = new VertexPositionColorTexture(
            new Vector3(anchorPoint.X + size.X, anchorPoint.Y - 48, 0) + topRightOffset, endColor, new Vector2(1, 0));

        //Bottom Left
        _squareQuad.vertices[2] = new VertexPositionColorTexture(
            new Vector3(anchorPoint.X + 16, anchorPoint.Y + size.Y - 48, 0) + bottomLeftOffset, startColor, new Vector2(0, 1));

        //Bottom Right
        _squareQuad.vertices[3] = new VertexPositionColorTexture(
            new Vector3(anchorPoint.X + size.X + 8, anchorPoint.Y + size.Y - 8, 0) + bottomRightOffset, endColor, Vector2.One);
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

    private Vector2 DefaultSize => new Vector2(384, 128);

    private void RenderDialogueBoxToPixelTarget(SpeechBubble speechBubble,
        RenderTargetHandle pixelTarget, RenderTargetHandle boxRenderTarget, RenderTargetHandle boxRenderTargetSwap)
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
            var size = DefaultSize;

            Vector2 textSize = ChatManager.GetStringSize(FontAssets.DeathText.Value,
                speechBubble.speaker.text, Vector2.One * 0.5f, 342);
            textSize.X = Math.Min(textSize.X, 342) + 64;
            textSize.Y += 48;

            int length = Math.Min(speechBubble.speaker.textIndex, speechBubble.speaker.text.Length);
            Vector2 currentTextSize = ChatManager.GetStringSize(FontAssets.DeathText.Value,
                speechBubble.speaker.text.Substring(0, length), Vector2.One * 0.5f, 342);
            currentTextSize.X = Math.Min(currentTextSize.X, 342) + 80;
            currentTextSize.Y += 64;
            _speechBubbleSize = Vector2.Lerp(_speechBubbleSize, currentTextSize, 0.15f);
            PrepareQuad(
                offset,
                _speechBubbleSize,
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
            Vector3 bottomLeftVertex = _squareQuad.vertices[2].Position;
            Vector2 bottomLeft = new Vector2(bottomLeftVertex.X, bottomLeftVertex.Y);
            SpritebatchDrawer tailDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.GooberPortraits.Tail.Asset,
                Main.screenPosition + offset + new Vector2(0, 64 + ExtraMath.Osc(-8, 8, speed: 1)));
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

