using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using ReLogic.Graphics;
using Stellamod.Buffs;
using Stellamod.Common.Shaders;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Core.Tooltips;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Common.ArmorRework
{
    public struct ArmorSet
    {
        public int helm;
        public int armor;
        public int legs;
    }

    public class ArmorReworkPlayerRenderer : IPlayerRenderer
    {
        private readonly List<DrawData> _drawData = new List<DrawData>();
        private readonly List<int> _dust = new List<int>();
        private readonly List<int> _gore = new List<int>();

        public static SamplerState MountedSamplerState
        {
            get
            {
                if (!Main.drawToScreen)
                    return SamplerState.AnisotropicClamp;

                return SamplerState.LinearClamp;
            }
        }

        public void DrawPlayers(Camera camera, IEnumerable<Player> players)
        {
            foreach (Player player in players)
            {
                DrawPlayerFull(camera, player);
            }
        }

        public void DrawPlayerHead(Camera camera, Player drawPlayer, Vector2 position, float alpha = 1f, float scale = 1f, Color borderColor = default(Color))
        {
            /*
            if (!drawPlayer.ShouldNotDraw) {
                _drawData.Clear();
                _dust.Clear();
                _gore.Clear();
                PlayerDrawHeadSet drawinfo = default(PlayerDrawHeadSet);
                drawinfo.BoringSetup(drawPlayer, _drawData, _dust, _gore, position.X, position.Y, alpha, scale);
                PlayerDrawHeadLayers.DrawPlayer_00_BackHelmet(ref drawinfo);
                PlayerDrawHeadLayers.DrawPlayer_01_FaceSkin(ref drawinfo);
                PlayerDrawHeadLayers.DrawPlayer_02_DrawArmorWithFullHair(ref drawinfo);
                PlayerDrawHeadLayers.DrawPlayer_03_HelmetHair(ref drawinfo);
                PlayerDrawHeadLayers.DrawPlayer_04_HatsWithFullHair(ref drawinfo);
                PlayerDrawHeadLayers.DrawPlayer_05_TallHats(ref drawinfo);
                PlayerDrawHeadLayers.DrawPlayer_06_NormalHats(ref drawinfo);
                PlayerDrawHeadLayers.DrawPlayer_07_JustHair(ref drawinfo);
                PlayerDrawHeadLayers.DrawPlayer_08_FaceAcc(ref drawinfo);
                CreateOutlines(alpha, scale, borderColor);
                PlayerDrawHeadLayers.DrawPlayer_RenderAllLayers(ref drawinfo);
            }
            */

            DrawPlayerInternal(camera, drawPlayer, position + Main.screenPosition, 0f, Vector2.Zero, alpha: alpha, scale: scale, headOnly: true);
        }

        public void DrawPlayer(Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow = 0f, float scale = 1.2f)
        {
            DrawPlayerInternal(camera, drawPlayer, position, rotation, rotationOrigin, shadow, scale: 1.3f);
        }

        // A split to add some not publicly exposed parameters.
        private void DrawPlayerInternal(Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow = 0f, float alpha = 1f, float scale = 1f, bool headOnly = false)
        {
            if (drawPlayer.ShouldNotDraw)
                return;

            PlayerDrawSet drawInfo = default(PlayerDrawSet);
            _drawData.Clear();
            _dust.Clear();
            _gore.Clear();


            if (headOnly)
            {
                drawInfo.HeadOnlySetup(drawPlayer, _drawData, _dust, _gore, position.X, position.Y, alpha, scale);
                goto SkipBoringSetup;
            }

            drawInfo.BoringSetup(drawPlayer, _drawData, _dust, _gore, position, shadow, rotation, rotationOrigin);
        SkipBoringSetup:

            /*
            DrawPlayer_UseNormalLayers(ref drawInfo);
            */

            PlayerLoader.ModifyDrawInfo(ref drawInfo);

            //For white, no lighting
            drawInfo.colorArmorBody = Color.White;
            drawInfo.colorArmorHead = Color.White;
            drawInfo.colorArmorLegs = Color.White;
            foreach (var layer in PlayerDrawLayerLoader.GetDrawLayers(drawInfo))
            {
                if (!headOnly || layer.IsHeadLayer)
                {
                    layer.DrawWithTransformationAndChildren(ref drawInfo);
                }
            }

            //TML: Copied from UseNormalLayers
            PlayerDrawLayers.DrawPlayer_MakeIntoFirstFractalAfterImage(ref drawInfo);

            PlayerDrawLayers.DrawPlayer_TransformDrawData(ref drawInfo);
            if (scale != 1f)
                PlayerDrawLayers.DrawPlayer_ScaleDrawData(ref drawInfo, scale);

            PlayerLoader.TransformDrawData(ref drawInfo);
   
            PlayerDrawLayers.DrawPlayer_RenderAllLayers(ref drawInfo);
        }
        private void DrawPlayerFull(Camera camera, Player drawPlayer)
        {
            SpriteBatch spriteBatch = camera.SpriteBatch;
            SamplerState samplerState = camera.Sampler;
            if (drawPlayer.mount.Active && drawPlayer.fullRotation != 0f)
                samplerState = MountedSamplerState;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, samplerState, DepthStencilState.None, camera.Rasterizer, null, camera.GameViewMatrix.TransformationMatrix);
            if (Main.gamePaused)
                drawPlayer.PlayerFrame();

            if (drawPlayer.ghost)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 vector = drawPlayer.shadowPos[i];
                    vector = drawPlayer.position - drawPlayer.velocity * (2 + i * 2);
                    DrawGhost(camera, drawPlayer, vector, 0.5f + 0.2f * (float)i);
                }

                DrawGhost(camera, drawPlayer, drawPlayer.position);
            }
            else
            {
                if (drawPlayer.inventory[drawPlayer.selectedItem].flame || drawPlayer.head == 137 || drawPlayer.wings == 22)
                {
                    drawPlayer.itemFlameCount--;
                    if (drawPlayer.itemFlameCount <= 0)
                    {
                        drawPlayer.itemFlameCount = 5;
                        for (int j = 0; j < 7; j++)
                        {
                            drawPlayer.itemFlamePos[j].X = (float)Main.rand.Next(-10, 11) * 0.15f;
                            drawPlayer.itemFlamePos[j].Y = (float)Main.rand.Next(-10, 1) * 0.35f;
                        }
                    }
                }

                PlayerLoader.DrawPlayer(drawPlayer, camera);

                if (drawPlayer.armorEffectDrawShadowEOCShield)
                {
                    int num = drawPlayer.eocDash / 4;
                    if (num > 3)
                        num = 3;

                    for (int k = 0; k < num; k++)
                    {
                        DrawPlayer(camera, drawPlayer, drawPlayer.shadowPos[k], drawPlayer.shadowRotation[k], drawPlayer.shadowOrigin[k], 0.5f + 0.2f * (float)k);
                    }
                }

                Vector2 position = default(Vector2);
                if (drawPlayer.invis)
                {
                    drawPlayer.armorEffectDrawOutlines = false;
                    drawPlayer.armorEffectDrawShadow = false;
                    drawPlayer.armorEffectDrawShadowSubtle = false;
                    position = drawPlayer.position;
                    if (drawPlayer.aggro <= -750)
                    {
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 1f);
                    }
                    else
                    {
                        drawPlayer.invis = false;
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin);
                        drawPlayer.invis = true;
                    }
                }

                if (drawPlayer.armorEffectDrawOutlines)
                {
                    _ = drawPlayer.position;
                    if (!Main.gamePaused)
                        drawPlayer.ghostFade += drawPlayer.ghostDir * 0.075f;

                    if ((double)drawPlayer.ghostFade < 0.1)
                    {
                        drawPlayer.ghostDir = 1f;
                        drawPlayer.ghostFade = 0.1f;
                    }
                    else if ((double)drawPlayer.ghostFade > 0.9)
                    {
                        drawPlayer.ghostDir = -1f;
                        drawPlayer.ghostFade = 0.9f;
                    }

                    float num2 = drawPlayer.ghostFade * 5f;
                    for (int l = 0; l < 4; l++)
                    {
                        float num3;
                        float num4;
                        switch (l)
                        {
                            default:
                                num3 = num2;
                                num4 = 0f;
                                break;
                            case 1:
                                num3 = 0f - num2;
                                num4 = 0f;
                                break;
                            case 2:
                                num3 = 0f;
                                num4 = num2;
                                break;
                            case 3:
                                num3 = 0f;
                                num4 = 0f - num2;
                                break;
                        }

                        position = new Vector2(drawPlayer.position.X + num3, drawPlayer.position.Y + drawPlayer.gfxOffY + num4);
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, drawPlayer.ghostFade);
                    }
                }

                if (drawPlayer.armorEffectDrawOutlinesForbidden)
                {
                    _ = drawPlayer.position;
                    if (!Main.gamePaused)
                        drawPlayer.ghostFade += drawPlayer.ghostDir * 0.025f;

                    if ((double)drawPlayer.ghostFade < 0.1)
                    {
                        drawPlayer.ghostDir = 1f;
                        drawPlayer.ghostFade = 0.1f;
                    }
                    else if ((double)drawPlayer.ghostFade > 0.9)
                    {
                        drawPlayer.ghostDir = -1f;
                        drawPlayer.ghostFade = 0.9f;
                    }

                    float num5 = drawPlayer.ghostFade * 5f;
                    for (int m = 0; m < 4; m++)
                    {
                        float num6;
                        float num7;
                        switch (m)
                        {
                            default:
                                num6 = num5;
                                num7 = 0f;
                                break;
                            case 1:
                                num6 = 0f - num5;
                                num7 = 0f;
                                break;
                            case 2:
                                num6 = 0f;
                                num7 = num5;
                                break;
                            case 3:
                                num6 = 0f;
                                num7 = 0f - num5;
                                break;
                        }

                        position = new Vector2(drawPlayer.position.X + num6, drawPlayer.position.Y + drawPlayer.gfxOffY + num7);
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, drawPlayer.ghostFade);
                    }
                }

                if (drawPlayer.armorEffectDrawShadowBasilisk)
                {
                    int num8 = (int)(drawPlayer.basiliskCharge * 3f);
                    for (int n = 0; n < num8; n++)
                    {
                        DrawPlayer(camera, drawPlayer, drawPlayer.shadowPos[n], drawPlayer.shadowRotation[n], drawPlayer.shadowOrigin[n], 0.5f + 0.2f * (float)n);
                    }
                }
                else if (drawPlayer.armorEffectDrawShadow)
                {
                    for (int num9 = 0; num9 < 3; num9++)
                    {
                        DrawPlayer(camera, drawPlayer, drawPlayer.shadowPos[num9], drawPlayer.shadowRotation[num9], drawPlayer.shadowOrigin[num9], 0.5f + 0.2f * (float)num9);
                    }
                }

                if (drawPlayer.armorEffectDrawShadowLokis)
                {
                    for (int num10 = 0; num10 < 3; num10++)
                    {
                        DrawPlayer(camera, drawPlayer, Vector2.Lerp(drawPlayer.shadowPos[num10], drawPlayer.position + new Vector2(0f, drawPlayer.gfxOffY), 0.5f), drawPlayer.shadowRotation[num10], drawPlayer.shadowOrigin[num10], MathHelper.Lerp(1f, 0.5f + 0.2f * (float)num10, 0.5f));
                    }
                }

                if (drawPlayer.armorEffectDrawShadowSubtle)
                {
                    for (int num11 = 0; num11 < 4; num11++)
                    {
                        position.X = drawPlayer.position.X + (float)Main.rand.Next(-20, 21) * 0.1f;
                        position.Y = drawPlayer.position.Y + (float)Main.rand.Next(-20, 21) * 0.1f + drawPlayer.gfxOffY;
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.9f);
                    }
                }

                if (drawPlayer.shadowDodge)
                {
                    drawPlayer.shadowDodgeCount += 1f;
                    if (drawPlayer.shadowDodgeCount > 30f)
                        drawPlayer.shadowDodgeCount = 30f;
                }
                else
                {
                    drawPlayer.shadowDodgeCount -= 1f;
                    if (drawPlayer.shadowDodgeCount < 0f)
                        drawPlayer.shadowDodgeCount = 0f;
                }

                if (drawPlayer.shadowDodgeCount > 0f)
                {
                    _ = drawPlayer.position;
                    position.X = drawPlayer.position.X + drawPlayer.shadowDodgeCount;
                    position.Y = drawPlayer.position.Y + drawPlayer.gfxOffY;
                    DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f);
                    position.X = drawPlayer.position.X - drawPlayer.shadowDodgeCount;
                    DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.5f + (float)Main.rand.Next(-10, 11) * 0.005f);
                }

                if (drawPlayer.brainOfConfusionDodgeAnimationCounter > 0)
                {
                    Vector2 vector2 = drawPlayer.position + new Vector2(0f, drawPlayer.gfxOffY);
                    float lerpValue = Utils.GetLerpValue(300f, 270f, drawPlayer.brainOfConfusionDodgeAnimationCounter);
                    float y = MathHelper.Lerp(2f, 120f, lerpValue);
                    if (lerpValue >= 0f && lerpValue <= 1f)
                    {
                        for (float num12 = 0f; num12 < (float)Math.PI * 2f; num12 += (float)Math.PI / 3f)
                        {
                            position = vector2 + new Vector2(0f, y).RotatedBy((float)Math.PI * 2f * lerpValue * 0.5f + num12);
                            DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, lerpValue);
                        }
                    }
                }

                position = drawPlayer.position;
                position.Y += drawPlayer.gfxOffY;
                if (drawPlayer.stoned)
                    DrawPlayerStoned(camera, drawPlayer, position);
                else if (!drawPlayer.invis)
                    DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin);
            }

            spriteBatch.End();
        }

        private void DrawPlayerStoned(Camera camera, Player drawPlayer, Vector2 position)
        {
            if (!drawPlayer.dead)
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                spriteEffects = ((drawPlayer.direction != 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                camera.SpriteBatch.Draw(TextureAssets.Extra[ExtrasID.PlayerStoned].Value, new Vector2((int)(position.X - camera.UnscaledPosition.X - (float)(drawPlayer.bodyFrame.Width / 2) + (float)(drawPlayer.width / 2)), (int)(position.Y - camera.UnscaledPosition.Y + (float)drawPlayer.height - (float)drawPlayer.bodyFrame.Height + 8f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2), null, Lighting.GetColor((int)((double)position.X + (double)drawPlayer.width * 0.5) / 16, (int)((double)position.Y + (double)drawPlayer.height * 0.5) / 16, Color.White), 0f, new Vector2(TextureAssets.Extra[ExtrasID.PlayerStoned].Width() / 2, TextureAssets.Extra[ExtrasID.PlayerStoned].Height() / 2), 1f, spriteEffects, 0f);
            }
        }

        private void DrawGhost(Camera camera, Player drawPlayer, Vector2 position, float shadow = 0f)
        {
            byte mouseTextColor = Main.mouseTextColor;
            SpriteEffects effects = ((drawPlayer.direction != 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            Color immuneAlpha = drawPlayer.GetImmuneAlpha(Lighting.GetColor((int)((double)drawPlayer.position.X + (double)drawPlayer.width * 0.5) / 16, (int)((double)drawPlayer.position.Y + (double)drawPlayer.height * 0.5) / 16, new Color((int)mouseTextColor / 2 + 100, (int)mouseTextColor / 2 + 100, (int)mouseTextColor / 2 + 100, (int)mouseTextColor / 2 + 100)), shadow);
            immuneAlpha.A = (byte)((float)(int)immuneAlpha.A * (1f - Math.Max(0.5f, shadow - 0.5f)));
            Rectangle value = new Rectangle(0, TextureAssets.Ghost.Height() / 4 * drawPlayer.ghostFrame, TextureAssets.Ghost.Width(), TextureAssets.Ghost.Height() / 4);
            Vector2 origin = new Vector2((float)value.Width * 0.5f, (float)value.Height * 0.5f);
            camera.SpriteBatch.Draw(TextureAssets.Ghost.Value, new Vector2((int)(position.X - camera.UnscaledPosition.X + (float)(value.Width / 2)), (int)(position.Y - camera.UnscaledPosition.Y + (float)(value.Height / 2))), value, immuneAlpha, 0f, origin, 1f, effects, 0f);
        }
    }

    /// <summary>
    /// Creates a cute little preview of the character for the armor UI
    /// </summary>
    public class ArmorPreviewUI : UIPanel
    {
        private Item _item;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 128;
            Height.Pixels = 128;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
        }
        public float alpha;
        public void SetArmorSet(Item item)
        {
            _item = item;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (_item == null)
                return;
            if (_item.IsAir)
                return;

            Vector2 position = GetDimensions().ToRectangle().TopLeft();
            ArmorSet set = ArmorSetSystem.FindArmorSet(_item);
            ArmorSetSystem.GetArmorSet(set, out Item helm, out Item armor, out Item leggings);

            Rectangle rectangle = ExpandableTooltip.GetBGRectangle((int)position.X, (int)position.Y, (int)Width.Pixels, (int)Height.Pixels);
            Utils.DrawInvBG(spriteBatch, rectangle, new Color(23, 25, 81, 255) * 0.925f * alpha);


            spriteBatch.End();
            spriteBatch.Begin(default, default, Main.graphics.GraphicsDevice.SamplerStates[0], default, Main.Rasterizer, SpriteWhiteShader.Instance.Effect, Main.UIScaleMatrix);


            Vector2 size = FontAssets.MouseText.Value.MeasureString(_item.Name);
            float xOffset = (rectangle.Width / 2f) - size.X / 2f;

            Vector2 armorIconPosition = position + new Vector2(-24, 0);// + new Vector2(-size.X / 2f, 0);
            //Step 3. Draw item icon of the current item
            Vector2 topRight = position;
            topRight.X += Width.Pixels * 1f; 

            for (float f = 0; f < 4f; f++)
            {
                Color outlineColor = Color.White;
                outlineColor *= (int)ExtraMath.Osc(0f, 2f, speed: 3);
                ItemSlot.DrawItemIcon(_item, 0, spriteBatch, armorIconPosition + (Vector2.UnitY * 2).RotatedBy(f / 4f * MathHelper.TwoPi), 1, 32, outlineColor * alpha);
            }
   
                
            spriteBatch.End();
            spriteBatch.Begin(default, default, Main.graphics.GraphicsDevice.SamplerStates[0], default, Main.Rasterizer, null, Main.UIScaleMatrix);



         
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, _item.Name,
               rectangle.TopLeft() + new Vector2(xOffset, 0), Color.White * alpha, 0, Vector2.Zero, Vector2.One);
            ItemSlot.DrawItemIcon(_item, 0, spriteBatch, armorIconPosition, 1, 32, Color.White * alpha);


            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, Main.Rasterizer, default, Main.UIScaleMatrix);

            Vector2 playerPosition = position + new Vector2(Width.Pixels, Height.Pixels) * 0.5f;
            playerPosition.Y -= Main.LocalPlayer.height / 2;
            playerPosition.Y += 20;
            ExpandableTooltip.DrawArmorPreview(playerPosition, helm, armor, leggings);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, Main.Rasterizer, default, Main.UIScaleMatrix);
        }
    }

    /// <summary>
    /// Creates a lore inspector for a piece of armor
    /// </summary>
    public class ArmorLoreUI : UIPanel
    {
        private UIText _loreText;
        public ArmorLoreUI() : base()
        {
            _loreText = new UIText("No Lore?");
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
   
            Width.Pixels = 384;
            Height.Pixels = 128;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
        
            _loreText.Width.Pixels = Width.Pixels;
            _loreText.Height.Pixels = Height.Pixels;
            _loreText.IsWrapped = true;
            Append(_loreText);
        }
        public float alpha;
        public int minHeight;
        public void SetText(string text)
        {
            _loreText.SetText(text);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            _loreText.TextColor = Color.White * alpha;
            minHeight = (int)_loreText.MinHeight.Pixels;
            Vector2 position = GetDimensions().ToRectangle().TopLeft();
            Rectangle rectangle = ExpandableTooltip.GetBGRectangle((int)position.X, (int)position.Y, (int)Width.Pixels, minHeight);
            Utils.DrawInvBG(spriteBatch, rectangle, new Color(23, 25, 81, 255) * 0.925f * alpha);
        }
    }


    /// <summary>
    /// Creates a state inspector for a piece of armor
    /// </summary>
    public class ArmorStatSummaryUI : UIPanel
    {
        private bool _setBonusActive;
        private UIText _summaryText;
        private List<TooltipLine> _lines;
        public ArmorStatSummaryUI() : base()
        {
            _summaryText = new UIText("Maidenless...");
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            _summaryText.Width.Pixels = Width.Pixels;
            _summaryText.Height.Pixels = Height.Pixels;
            Width.Pixels = 384;
            Height.Pixels = 128;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Append(_summaryText);
        }

        public float alpha;
        public void SetTooltips(List<TooltipLine> statLines)
        {
            _lines = statLines;
        }

        public void SetTooltips(List<TooltipLine> statLines, string setBonus)
        {
            _lines = statLines;
            _summaryText.Width.Pixels = Width.Pixels - 100;
            _summaryText.Height.Pixels = Height.Pixels;
            _summaryText.IsWrapped = true;


            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
  
            Rectangle rectangle = ExpandableTooltip.GetBGRectangle(statLines, (int)topLeft.X, (int)topLeft.Y, 1);
            _summaryText.Top.Pixels = rectangle.Height;
      
            _summaryText.SetText(setBonus);
        }
        public void SetTooltips(List<TooltipLine> statLines, string setBonus, bool setBonusActive)
        {
            _setBonusActive = setBonusActive;
            _lines = statLines;
            _summaryText.Width.Pixels = Width.Pixels - 100;
            _summaryText.Height.Pixels = Height.Pixels;
            _summaryText.IsWrapped = true;


            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();

            Rectangle rectangle = ExpandableTooltip.GetBGRectangle(statLines, (int)topLeft.X, (int)topLeft.Y, 1);
            _summaryText.Top.Pixels = rectangle.Height;

            _summaryText.SetText(setBonus);
   
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (_lines == null)
                return;

            _summaryText.TextColor = _setBonusActive ? Color.Green * alpha : Color.Lerp(Color.White, Color.Black, 0.75f) * alpha;
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            Vector2 position = new Vector2(0, 0);
            position += topLeft;


            int height = (int)_summaryText.MinHeight.Pixels;
            Rectangle tooltipRectangle = ExpandableTooltip.GetBGRectangle(_lines, (int)topLeft.X, (int)topLeft.Y, alpha);
            Rectangle rectangle = ExpandableTooltip.GetBGRectangle((int)position.X, (int)position.Y + (int)_summaryText.Top.Pixels, (int)_summaryText.Width.Pixels, height);

            Rectangle combinedRectangle = tooltipRectangle;
            combinedRectangle.Width =Math.Max(rectangle.Width, tooltipRectangle.Width);
            combinedRectangle.Height = (int)Parent.Height.Pixels;
            Utils.DrawInvBG(spriteBatch, combinedRectangle, new Color(23, 25, 81, 255) * 0.925f * alpha);

            ExpandableTooltip.DrawExpandableTooltip(spriteBatch, _lines, (int)topLeft.X, (int)topLeft.Y, alpha, false, Main.LocalPlayer.GetModPlayer<ArmorStatsPlayer>().RequestIconTexture);

        }
    }

    public class ArmorInspectorUI : UIPanel
    {
        public ArmorStatSummaryUI summaryUI;
        public ArmorLoreUI loreUI;
        public ArmorPreviewUI previewUI;
        public ArmorInspectorUI() : base()
        {
            summaryUI = new();
            loreUI = new();
            previewUI = new();
        }
        public float alpha;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 750;
            Height.Pixels = 300;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Append(summaryUI);
            Append(loreUI);
            Append(previewUI);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Height.Pixels = 400;
          
            Vector2 mouseScreen = Main.MouseScreen;
            mouseScreen.X += 64;

            Vector2 targetPoint = mouseScreen;
            targetPoint.X -= Width.Pixels;
            if (targetPoint.X < 64)
                targetPoint.X = 64;
            Left.Pixels = MathHelper.Lerp(targetPoint.X + 128, targetPoint.X, alpha);

            Top.Pixels = mouseScreen.Y + 8;
            summaryUI.Left.Set(0, 0);
            summaryUI.Top.Set(0, 0);
            previewUI.Left.Set(-300, 1);
            previewUI.Top.Set(0, 0);
            loreUI.Left.Set(-loreUI.Width.Pixels - 32, 1);
            loreUI.Top.Set(-loreUI.minHeight, 1);
       
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            Vector2 position = topLeft;
            Rectangle rectangle = ExpandableTooltip.GetBGRectangle((int)position.X, (int)position.Y, (int)Width.Pixels, (int)Height.Pixels);
            Utils.DrawInvBG(spriteBatch, rectangle, new Color(23, 25, 81, 255) * 0.925f * alpha);
        }
    }

    public class ArmorTooltipUIState : UIState
    {
        public ArmorInspectorUI inspectorUI;
        public ArmorTooltipUIState() : base()
        {
            inspectorUI = new();
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Append(inspectorUI);
        }
    }


    [Autoload(Side = ModSide.Client)]
    public class ArmorTooltipSystem : ModSystem
    {
        private float _timer;
        private float _alpha;
        private bool _active;
        private UserInterface _userInterface;
        private GameTime _lastUpdateUiGameTime;
        private ArmorTooltipUIState _uiState;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _userInterface = new UserInterface();
            _uiState = new();
            _uiState.Activate();
            On_Main.DrawInterface_33_MouseText += DisableMouseText;
        }

        private void DisableMouseText(On_Main.orig_DrawInterface_33_MouseText orig, Main self)
        {
            if (_timer > 0)
                return;
            orig(self);
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DrawInterface_33_MouseText -= DisableMouseText;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
            _timer += (float)(gameTime.ElapsedGameTime.TotalSeconds * (_active ? 1 : -1));
            if(_timer >= 1f)
            {
                _timer = 1f;
            }
            if (_timer <= 0f)
                _timer = 0f;
            _active = false;
            _alpha = EasingFunction.OutExpo(_timer / 1f);


            _uiState.inspectorUI.alpha = _alpha;
            _uiState.inspectorUI.previewUI.alpha = _alpha;
            _uiState.inspectorUI.loreUI.alpha = _alpha;
            _uiState.inspectorUI.summaryUI.alpha = _alpha;
            _userInterface?.Update(gameTime);
            _lastUpdateUiGameTime = gameTime;
        }

        public void InspectArmor(Item item, string lore, string setBonus, List<TooltipLine> stats)
        {
            _active = true;



            _uiState.inspectorUI.previewUI.SetArmorSet(item);
            _uiState.inspectorUI.loreUI.SetText(lore);

            ArmorSet set = ArmorSetSystem.FindArmorSet(item.type);
            ArmorSetSystem.GetArmorSet(set, out Item helm, out Item armor, out Item leggings);
            Player player = Main.LocalPlayer;
            bool isActive = player.armor[0].type == helm.type && player.armor[1].type == armor.type && player.armor[2].type == leggings.type; 
            _uiState.inspectorUI.summaryUI.SetTooltips(stats, setBonus, isActive);
            if (_userInterface.CurrentState == null)
                OpenUI();
        }

        public void OpenUI()
        {
            _userInterface.SetState(_uiState);
        }

        public void CloseUI()
        {
            _userInterface.SetState(null);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Item / NPC Head"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Scarlet Sun: Armor Rework",
                    delegate
                    {
                        if (_timer <= 0f)
                            return true;
                        if (_lastUpdateUiGameTime != null && _userInterface?.CurrentState != null)
                        {
                            _userInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
    public class ArmorSetSystem : ModSystem
    {
        private static List<ArmorSet> _armorSets;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            _armorSets = new List<ArmorSet>();
        }

        public override void Unload()
        {
            base.Unload();
            _armorSets = null;
        }

        public static ArmorSet FindArmorSet(int type)
        {
            return _armorSets.Find(x => x.helm == type || x.armor == type || x.legs == type);
        }

        public static ArmorSet FindArmorSet(Item item)
        {
            return FindArmorSet(item.type);
        }

        public static void GetArmorSet(ArmorSet armorSet, out Item helm, out Item armor, out Item leggings)
        {
            helm = new Item(armorSet.helm);
            armor = new Item(armorSet.armor);
            leggings = new Item(armorSet.legs);
        }
        public static void RegisterArmorSet<Helm, Armor, Legs>()
            where Helm : ModItem
            where Armor : ModItem
            where Legs : ModItem
        {
            RegisterArmorSet(ModContent.ItemType<Helm>(), ModContent.ItemType<Armor>(), ModContent.ItemType<Legs>());
        }

        public static void RegisterArmorSet(int helm, int armor, int legs)
        {
            ArmorSet set = new ArmorSet
            {
                helm = helm,
                armor = armor,
                legs = legs
            };
            _armorSets.Add(set);
        }
    }

    public class ArmorReworkExpandableTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            //            throw new NotImplementedException();
            //Here we want to get the stats for the entire armor set
            if (item.vanity)
                return;

            ArmorStatsPlayer armorStatsPlayer = Main.LocalPlayer.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.GetStatTooltipsLocalToItem(item, lines);
        }
    }
    public class ArmorReworkGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
        }
    }

    public class ExtraAccessorySlot1 : ModAccessorySlot
    {
        public override bool IsEnabled()
        {
            return Player.GetModPlayer<ArmorStatsPlayer>().accessorySlots >= 6;
        }
    }

    public class ExtraAccessorySlot2 : ModAccessorySlot
    {
        public override bool IsEnabled()
        {
            return Player.GetModPlayer<ArmorStatsPlayer>().accessorySlots >= 7;
        }
    }

    public class ExtraAccessorySlot3 : ModAccessorySlot
    {
        public override bool IsEnabled()
        {
            return Player.GetModPlayer<ArmorStatsPlayer>().accessorySlots >= 8;
        }
    }

    public class ExtraAccessorySlot4 : ModAccessorySlot
    {
        public override bool IsEnabled()
        {
            return Player.GetModPlayer<ArmorStatsPlayer>().accessorySlots >= 9;
        }
    }

    public class ArmorAccessoryRework : ModSystem
    {
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Player.IsItemSlotUnlockedAndUsable += LimitAccessorySlots;
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Player.IsItemSlotUnlockedAndUsable -= LimitAccessorySlots;
        }


        private bool LimitAccessorySlots(On_Player.orig_IsItemSlotUnlockedAndUsable orig, Player self, int slot)
        {
            int start = 3;
            int end = 9;
            if (slot >= start && slot <= end)
            {
                int accessoryNumber = slot - start;
                ArmorStatsPlayer armorStatsPlayer = self.GetModPlayer<ArmorStatsPlayer>();
                if (armorStatsPlayer.accessorySlots > accessoryNumber)
                    return true;
                else
                    return false;
            }

            return orig(self, slot);
        }
    }

    public class ExtraPierceGlobalProjectile : GlobalProjectile
    {

        //I believe net update is called after on spawn automatically
        //and penetrate will be synced
        //so this should work
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            base.OnSpawn(projectile, source);
            Player player = Main.player[projectile.owner];
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            if (projectile.penetrate != -1)
            {
                projectile.penetrate += armorStatsPlayer.rangedPiercing;
                projectile.maxPenetrate += armorStatsPlayer.rangedPiercing;
            }
        }
    }

    public static class ArmorStatsExtensions
    {
        public static void AddEndurance(this Player player, float value)
        {
            player.GetModPlayer<ArmorStatsPlayer>().generalEndurance += value;
        }

        public static void AddBossEndurance(this Player player, float value)
        {
            player.GetModPlayer<ArmorStatsPlayer>().bossEndurance += value;
        }

        public static ArmorStatsPlayer GetStats(this Player player) => player.GetModPlayer<ArmorStatsPlayer>();

    }

    public class ArmorStatsPlayer : ModPlayer
    {
        //Textures
        private Dictionary<string, Asset<Texture2D>> _iconAssets;
        private Player _localDummyPlayer;
        private Player _currentDummyPlayer;
        public float generalEndurance;
        public float bossEndurance;
        public float enemyEndurance;
        public int defenseBonus;
        public int healthBonus;

        public float criticalStrikeChance;
        public float criticalStrikeDamage;

        public int stamina;
        public int accessorySlots;
        public int insourceSlots;
        public int inventorySlots;
        public float insourceTimeFlatBonus;
        public float insourceTimeBonus;
        public float movementSpeedBonus;

        public float meleeAttackSpeed;
        public float meleeDamage;
        public int meleeArmorPenetration;
        public int meleeAggressiveness;

        public float rangedBowChargeTime;
        public float rangedDamage;
        public int rangedPiercing;
        public int rangedGunAmmoAmount;
        public int rangedStealthtiness;

        public float summonCastTime;
        public float summonDamage;
        public int minionSlots;
        public float mainSummonDamage;
        public float mainSummonHealth;
        public float minionSummonHealth;
        public int minionAggressiveness;

        public float artifactManaReduction;
        public float wandCastTime;
        public int totalMana;
        public float magicDamage;
        public int wandNormalEnchantmentSlots;
        public int wandTimerEnchantmentSlots;

        public bool isComparison;


        public override void Unload()
        {
            base.Unload();
            _iconAssets = null;
          
        }


        public Asset<Texture2D> RequestIconTexture(string name)
        {
            _iconAssets ??= new Dictionary<string, Asset<Texture2D>>();
            if (_iconAssets.ContainsKey(name))
                return _iconAssets[name];

            string path = this.GetType().DirectoryHere() + $"/{name}";
            bool exists = ModContent.RequestIfExists<Texture2D>(path, out Asset<Texture2D> asset);
            if (exists)
            {
                _iconAssets.Add(name, asset);
            }
            else
            {
                asset = ModContent.Request<Texture2D>(TextureRegistry.EmptyTexture);
            }

            return asset;
        }

        public override void ResetEffects()
        {
            base.ResetEffects();
            //Defensive Stats
            generalEndurance = 0;
            bossEndurance = 0;
            enemyEndurance = 0;
            defenseBonus = 0;
            healthBonus = 0;

            //Critical Strike Stats
            criticalStrikeChance = 0;
            criticalStrikeDamage = 0;

            //Resource Stats
            stamina = 0;
            accessorySlots = 0;
            insourceSlots = 0;
            insourceTimeBonus = 0;
            movementSpeedBonus = 0;
            inventorySlots = 0;

            //Melee Damage
            meleeAttackSpeed = 0;
            meleeDamage = 0;
            meleeArmorPenetration = 0;
            meleeAggressiveness = 0;

            //Ranged stats
            rangedBowChargeTime = 0;
            rangedDamage = 0;
            rangedPiercing = 0;
            rangedGunAmmoAmount = 0;
            rangedStealthtiness = 0;

            //Summoner Stats
            summonCastTime = 0;
            summonDamage = 0;
            minionSlots = 0;
            mainSummonDamage = 0;
            mainSummonHealth = 0;
            minionSummonHealth = 0;
            minionAggressiveness = 0;

            //Magic Stats
            artifactManaReduction = 0;
            wandCastTime = 0;
            totalMana = 0;
            magicDamage = 0;
            wandNormalEnchantmentSlots = 0;
            wandTimerEnchantmentSlots = 0;
        }

        private string GetComparison(string name, float comparedValue, bool invert = false, float localStatValue = 0f, bool isShowingComparison = false)
        {
            if (comparedValue == 0)
                return string.Empty;
            string percentString = MathF.Abs(localStatValue).ToString("P0");
            string increaseDecreaseKey = localStatValue < 0 ? "StatSubtraction" : "StatAddition";
            if (invert)
            {
                increaseDecreaseKey = localStatValue > 0 ? "StatSubtraction" : "StatAddition";
            }
            string comparisonText = LangText.Common(increaseDecreaseKey, LangText.Common($"Stat{name}"), percentString);
            if(isShowingComparison)
            {
                string symbol = MathF.Sign(comparedValue) >= 0f ? "+" : "-";
             
                comparisonText += $" ({symbol}{MathF.Abs(comparedValue)})";
            }
            return comparisonText;
        }

        private string GetComparison(string name, int comparedValue, int localStatValue = 0, bool isShowingComparison = false)
        {
            if (comparedValue == 0)
                return string.Empty;
            string percentString = MathF.Abs(localStatValue).ToString();
            string increaseDecreaseKey = localStatValue < 0 ? "StatSubtractionAlt" : "StatAdditionAlt";
            string comparisonText = LangText.Common(increaseDecreaseKey, LangText.Common($"Stat{name}"), percentString);
            if (isShowingComparison)
            {
                string symbol = MathF.Sign(comparedValue) >= 0f ? "+" : "-";

                comparisonText += $" ({symbol}{MathF.Abs(comparedValue)})";
            }
            return comparisonText;
        }

        private void ApplyArmor(Item item, Player player)
        {
            /*
            if (item.accessory)
            {
                ItemLoader.UpdateAccessory(item, player, false);
            }*/
            if (item.headSlot != -1 || item.bodySlot != -1 || item.legSlot != -1)
            {
                ItemLoader.UpdateEquip(item, player);
            }
        }


        public void GetStatTooltipsLocalToItem(Item item, List<TooltipLine> tooltips)
        {
            if (item.IsAir || item == null)
                return;

            _localDummyPlayer ??= new Player();
            _localDummyPlayer.ResetEffects();

            _currentDummyPlayer ??= new Player();
            _currentDummyPlayer.ResetEffects();

            Player player = Main.LocalPlayer;
            Item helmer = player.armor[0];
            Item armor = player.armor[1];
            Item legs = player.armor[2];

            bool isShowingComparison = false;
            //Apply all of our stat bonuses here
            if (!helmer.IsAir && item.headSlot != -1 && item.type != helmer.type)
            {
                isShowingComparison = true;
                ApplyArmor(helmer, _currentDummyPlayer);
            } else if (!armor.IsAir && item.bodySlot != -1 && item.type != armor.type)
            {
                isShowingComparison = true;
                ApplyArmor(armor, _currentDummyPlayer);
            } else if (!legs.IsAir && item.legSlot != -1 && item.type != legs.type)
            {
                isShowingComparison = true;
                ApplyArmor(legs, _currentDummyPlayer);
            }

            ApplyArmor(item, _localDummyPlayer);

            //Compare the differences here
            ArmorStatsPlayer currentStatsPlayer = _currentDummyPlayer.GetModPlayer<ArmorStatsPlayer>();
            ArmorStatsPlayer localItemStatsPlayer = _localDummyPlayer.GetModPlayer<ArmorStatsPlayer>();


            ArmorStatsPlayer comparisonPlayer = currentStatsPlayer.CompareArmorStatsPlayer(localItemStatsPlayer);
            comparisonPlayer.GetStatTooltips(localItemStatsPlayer, tooltips, isShowingComparison);
        }

        public void GetStatTooltips(ArmorStatsPlayer originalStatsPlayer, List<TooltipLine> tooltips, bool isShowingComparison = false)
        {
            void AddLineIfDifferent(string name, float comparisonValue, float currentValue, bool invert = false)
            {
                string comparison = GetComparison(name, comparisonValue, invert, currentValue, isShowingComparison);
                if (string.IsNullOrEmpty(comparison))
                    return;
                TooltipLine line = new TooltipLine(Stellamod.Instance, name, comparison);
                if (comparisonValue < 0)
                    line.OverrideColor = Color.IndianRed;
                if (isShowingComparison && comparisonValue > 0)
                    line.OverrideColor = Color.LightGreen;
                tooltips.Add(line);
            }
            void AddLineIfDifferentInt(string name, int comparisonValue, int currentValue)
            {
                string comparison = GetComparison(name, comparisonValue, currentValue, isShowingComparison);
                if (string.IsNullOrEmpty(comparison))
                    return;
                TooltipLine line = new TooltipLine(Stellamod.Instance, name, comparison);
                if (comparisonValue < 0)
                    line.OverrideColor = Color.IndianRed;
                if (isShowingComparison && comparisonValue > 0)
                    line.OverrideColor = Color.LightGreen;
                tooltips.Add(line);
            }

            //damage goes here
            AddLineIfDifferent("MeleeDamage", meleeDamage, originalStatsPlayer.meleeDamage);
            AddLineIfDifferent("RangedDamage", rangedDamage, originalStatsPlayer.rangedDamage);
            AddLineIfDifferent("MagicDamage", magicDamage, originalStatsPlayer.magicDamage);
            AddLineIfDifferent("MinionDamage", summonDamage, originalStatsPlayer.summonDamage);
            AddLineIfDifferentInt("MaxHealth", healthBonus, originalStatsPlayer.healthBonus);
            AddLineIfDifferent("CriticalStrikeChance", criticalStrikeChance, originalStatsPlayer.criticalStrikeChance);
            AddLineIfDifferent("CriticalStrikeDamage", criticalStrikeDamage, originalStatsPlayer.criticalStrikeDamage);
            AddLineIfDifferentInt("Stamina", stamina, originalStatsPlayer.stamina);
            AddLineIfDifferentInt("ArmorPenetration", meleeArmorPenetration, originalStatsPlayer.meleeArmorPenetration);
            AddLineIfDifferentInt("AccessorySlots", accessorySlots, originalStatsPlayer.accessorySlots);
            AddLineIfDifferentInt("InventorySlots", inventorySlots, originalStatsPlayer.inventorySlots);
            AddLineIfDifferent("MovementSpeed", movementSpeedBonus, originalStatsPlayer.movementSpeedBonus);
            AddLineIfDifferent("Endurance", generalEndurance, originalStatsPlayer.generalEndurance);
            AddLineIfDifferent("BossEndurance", bossEndurance, originalStatsPlayer.bossEndurance);
            AddLineIfDifferent("EnemyEndurance", enemyEndurance, originalStatsPlayer.enemyEndurance);
            AddLineIfDifferentInt("InsourceSlots", insourceSlots, originalStatsPlayer.insourceSlots);
            AddLineIfDifferent("MeleeAttackSpeed", meleeAttackSpeed, originalStatsPlayer.meleeAttackSpeed);
            AddLineIfDifferentInt("Defense", defenseBonus, originalStatsPlayer.defenseBonus);
            AddLineIfDifferent("InsourceTime", insourceTimeBonus, originalStatsPlayer.insourceTimeBonus);
            AddLineIfDifferent("Aggressiveness", meleeAggressiveness, originalStatsPlayer.meleeAggressiveness);
            AddLineIfDifferent("BowChargeTime", rangedBowChargeTime, originalStatsPlayer.rangedBowChargeTime, invert: true);
            AddLineIfDifferentInt("Piercing", rangedPiercing, originalStatsPlayer.rangedPiercing);
            AddLineIfDifferentInt("GunAmmoAmount", rangedGunAmmoAmount, originalStatsPlayer.rangedGunAmmoAmount);
            AddLineIfDifferentInt("Stealthiness", rangedStealthtiness, originalStatsPlayer.rangedStealthtiness);
            AddLineIfDifferent("SummonCastTime", summonCastTime, originalStatsPlayer.summonCastTime, invert: true);
            AddLineIfDifferent("MinionSlots", minionSlots, originalStatsPlayer.minionSlots);
            AddLineIfDifferent("MainMinionDamage", mainSummonDamage, originalStatsPlayer.mainSummonDamage);
            AddLineIfDifferent("MainMinionHealth", mainSummonHealth, originalStatsPlayer.mainSummonHealth);
            AddLineIfDifferent("MinionHealth", minionSummonHealth, originalStatsPlayer.minionSummonHealth);
            AddLineIfDifferentInt("MinionAggressiveness", minionAggressiveness, originalStatsPlayer.minionAggressiveness);
            AddLineIfDifferent("ArtifactManaReduction", artifactManaReduction, originalStatsPlayer.artifactManaReduction);
            AddLineIfDifferent("WandCastTime", wandCastTime, originalStatsPlayer.wandCastTime, invert: true);
            AddLineIfDifferentInt("MaxMana", totalMana, originalStatsPlayer.totalMana);
            AddLineIfDifferentInt("WandNormalEnchantmentSlots", wandNormalEnchantmentSlots, originalStatsPlayer.wandNormalEnchantmentSlots);
            AddLineIfDifferentInt("WandTimerEnchantmentSlots", wandTimerEnchantmentSlots, originalStatsPlayer.wandTimerEnchantmentSlots);
        }


        public ArmorStatsPlayer CompareArmorStatsPlayer(ArmorStatsPlayer otherPlayer)
        {
            ArmorStatsPlayer armorStatsPlayer = new ArmorStatsPlayer();
            armorStatsPlayer.generalEndurance = otherPlayer.generalEndurance - generalEndurance;
            armorStatsPlayer.bossEndurance = otherPlayer.bossEndurance - bossEndurance;
            armorStatsPlayer.enemyEndurance = otherPlayer.enemyEndurance - enemyEndurance;
            armorStatsPlayer.defenseBonus = otherPlayer.defenseBonus - defenseBonus;
            armorStatsPlayer.healthBonus = otherPlayer.healthBonus - healthBonus;

            armorStatsPlayer.criticalStrikeChance = otherPlayer.criticalStrikeChance - criticalStrikeChance;
            armorStatsPlayer.criticalStrikeDamage = otherPlayer.criticalStrikeDamage - criticalStrikeDamage;

            armorStatsPlayer.stamina = otherPlayer.stamina - stamina;
            armorStatsPlayer.accessorySlots = otherPlayer.accessorySlots - accessorySlots;
            armorStatsPlayer.insourceSlots = otherPlayer.insourceSlots - insourceSlots;
            armorStatsPlayer.inventorySlots = otherPlayer.inventorySlots - inventorySlots;
            armorStatsPlayer.insourceTimeBonus = otherPlayer.insourceTimeBonus - insourceTimeBonus;
            armorStatsPlayer.movementSpeedBonus = otherPlayer.movementSpeedBonus - movementSpeedBonus;

            armorStatsPlayer.meleeAttackSpeed = otherPlayer.meleeAttackSpeed - meleeAttackSpeed;
            armorStatsPlayer.meleeDamage = otherPlayer.meleeDamage - meleeDamage;
            armorStatsPlayer.meleeArmorPenetration = otherPlayer.meleeArmorPenetration - meleeArmorPenetration;
            armorStatsPlayer.meleeAggressiveness = otherPlayer.meleeAggressiveness - meleeAggressiveness;

            armorStatsPlayer.rangedBowChargeTime = otherPlayer.rangedBowChargeTime - rangedBowChargeTime;
            armorStatsPlayer.rangedDamage = otherPlayer.rangedDamage - rangedDamage;
            armorStatsPlayer.rangedPiercing = otherPlayer.rangedPiercing - rangedPiercing;
            armorStatsPlayer.rangedGunAmmoAmount = otherPlayer.rangedGunAmmoAmount - rangedGunAmmoAmount;
            armorStatsPlayer.rangedStealthtiness = otherPlayer.rangedStealthtiness - rangedStealthtiness;

            armorStatsPlayer.summonCastTime = otherPlayer.summonCastTime - summonCastTime;
            armorStatsPlayer.summonDamage = otherPlayer.summonDamage - summonDamage;
            armorStatsPlayer.minionSlots = otherPlayer.minionSlots - minionSlots;
            armorStatsPlayer.mainSummonDamage = otherPlayer.mainSummonDamage - mainSummonDamage;
            armorStatsPlayer.mainSummonHealth = otherPlayer.mainSummonHealth - mainSummonHealth;
            armorStatsPlayer.minionAggressiveness = otherPlayer.minionAggressiveness - minionAggressiveness;

            armorStatsPlayer.artifactManaReduction = otherPlayer.artifactManaReduction - artifactManaReduction;
            armorStatsPlayer.wandCastTime = otherPlayer.wandCastTime - wandCastTime;
            armorStatsPlayer.totalMana = otherPlayer.totalMana - totalMana;
            armorStatsPlayer.magicDamage = otherPlayer.magicDamage - magicDamage;
            armorStatsPlayer.wandNormalEnchantmentSlots = otherPlayer.wandNormalEnchantmentSlots - wandNormalEnchantmentSlots;
            armorStatsPlayer.wandTimerEnchantmentSlots = otherPlayer.wandTimerEnchantmentSlots - wandTimerEnchantmentSlots;

            armorStatsPlayer.isComparison = true;
            return armorStatsPlayer;
        }

        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
            base.ModifyWeaponCrit(item, ref crit);
            crit += criticalStrikeChance * 100;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.CritDamage += criticalStrikeDamage;
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            Player.statLifeMax2 += healthBonus;
            Player.statDefense += defenseBonus;
            Player.moveSpeed += movementSpeedBonus;
            Player.GetAttackSpeed(DamageClass.Melee) += meleeAttackSpeed;
            if (NPC.AnyDanger())
            {
                Player.endurance += bossEndurance;
            }
            else
            {
                Player.endurance += enemyEndurance;
            }
            Player.endurance += generalEndurance;


            Player.GetDamage(DamageClass.Melee) += meleeDamage;
            Player.GetArmorPenetration(DamageClass.Melee) += meleeArmorPenetration;
            Player.aggro += meleeAggressiveness;
            Player.GetDamage(DamageClass.Ranged) += rangedDamage;
            Player.GetDamage(DamageClass.Magic) += magicDamage;
            Player.statManaMax2 += totalMana;
            Player.GetModPlayer<DashPlayer>().MaxDashCount += stamina;
            Player.GetModPlayer<FlaskPlayer>().maxInsourceCount += insourceSlots;
            Player.maxMinions += minionSlots;
            Player.aggro += meleeAggressiveness;
            Player.aggro -= rangedStealthtiness;
        }
    }
}
