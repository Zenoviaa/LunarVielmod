using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellamod.Core.Helpers.Math;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace Stellamod.Core.DashSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class DashMeter : ModSystem
    {
        private UserInterface _interface;
        private DashMeterUI _dashMeterUI;
        private GameTime _lastUpdateUiGameTime;
        public override void Load()
        {
            base.Load();
            if (!Main.dedServ)
            {
                _interface = new UserInterface();
                _dashMeterUI = new DashMeterUI();
                _dashMeterUI.Activate();
                _interface.SetState(_dashMeterUI);
            }
        }

        public override void Unload()
        {
            base.Unload();
            //_staminaMeterUI?.SomeKindOfUnload(); // If you hold data that needs to be unloaded, call it in OO-fashion
            _dashMeterUI = null;
        }


        public override void UpdateUI(GameTime gameTime)
        {
            _dashMeterUI.Activate();
            _lastUpdateUiGameTime = gameTime;
            if (_interface?.CurrentState != null)
            {
                _interface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Urdveil: Dash Meter",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _interface?.CurrentState != null)
                        {
                            _interface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }

    internal class DashMeterUI : UIState
    {
        private float _prevDashCount;
        private float _flashStrength;
        private int _staminaFlashIndex;
        private string AssetDirectory = $"Stellamod/Core/DashSystem/";
        private Texture2D
            _empty,
            _edge,
            _filled,
            _white;

        public override void OnActivate()
        {
            base.OnActivate();
            _empty = ModContent.Request<Texture2D>($"{AssetDirectory}DashMeterEmpty").Value;
            _edge = ModContent.Request<Texture2D>($"{AssetDirectory}DashMeterEdge").Value;
            _filled = ModContent.Request<Texture2D>($"{AssetDirectory}DashMeterFilled").Value;
            _white = ModContent.Request<Texture2D>($"{AssetDirectory}DashMeterFilledWhite").Value;
        }

        public Color Color = Color.White;
        public bool ScaleToFit = false;
        public float ImageScale = 1f;
        public float Rotation;
        public bool AllowResizingDimensions = true;
        public Vector2 NormalizedOrigin = Vector2.Zero;
        public Color FlashColor = Color.White;
        private static Vector2? _drag = null;
        private static bool _isDragging;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Texture2D texture2D = _filled;
            Vector2 vector = texture2D.Size();
            var config = ModContent.GetInstance<StellamodClientConfig>();
            Vector2 ratioPos = new Vector2(config.DashMeterX, config.DashMeterY);
            if (ratioPos.X < 0f || ratioPos.X > 100f)
            {
                ratioPos.X = 50;
            }

            if (ratioPos.Y < 0f || ratioPos.Y > 100f)
            {
                ratioPos.Y = 3;
            }

            Vector2 drawPos = ratioPos;
            drawPos.X = (int)(drawPos.X * 0.01f * Main.screenWidth);
            drawPos.Y = (int)(drawPos.Y * 0.01f * Main.screenHeight);

            DashPlayer dashPlayer = Main.LocalPlayer.GetModPlayer<DashPlayer>();
            if (_prevDashCount != dashPlayer.DashCount)
            {
                FlashColor = Color.White;
                _prevDashCount = dashPlayer.DashCount;
            }

            int filledAmount = dashPlayer.DashCount;
            int maxFillAmount = dashPlayer.MaxDashCount;

            Rectangle mouseRect = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Vector2 size = new Vector2(_edge.Size().X * (maxFillAmount + 3), _edge.Size().Y);
            Rectangle barRect = Utils.CenteredRectangle(drawPos + size / 2, size * Main.UIScale);

            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Main.MouseScreen;
            Vector2 newScreenRatioPosition = ratioPos;
            if (ms.LeftButton == ButtonState.Pressed && !_isDragging && barRect.Intersects(mouseRect))
            {
                _isDragging = true;
            }

            //Handle dragging
            if (_isDragging)
            {

                if (!_drag.HasValue)
                    _drag = mousePos - drawPos;

                Vector2 newCorner = mousePos - _drag.GetValueOrDefault(Vector2.Zero);

                // Convert the new corner position into a screen ratio position.
                newScreenRatioPosition.X = 100f * newCorner.X / Main.screenWidth;
                newScreenRatioPosition.Y = 100f * newCorner.Y / Main.screenHeight;

                // Compute the change in position. If it is large enough, actually move the meter
                Vector2 delta = newScreenRatioPosition - ratioPos;
                if (Math.Abs(delta.X) >= 0.05f || Math.Abs(delta.Y) >= 0.05f)
                {
                    config.DashMeterX = newScreenRatioPosition.X;
                    config.DashMeterY = newScreenRatioPosition.Y;
                }

                if (ms.LeftButton == ButtonState.Released)
                {
                    _isDragging = false;
                    _drag = null;
                    MethodInfo saveMethodInfo = typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);
                    if (saveMethodInfo is not null)
                        saveMethodInfo.Invoke(null, new object[] { config });
                }
            }

            if (dashPlayer.ShouldFlicker)
            {
                _staminaFlashIndex = filledAmount;
                _flashStrength = MathHelper.Lerp(_flashStrength, 1f, 0.05f);
            }
            else
            {
                _flashStrength = MathHelper.Lerp(_flashStrength, 0f, 0.05f);
            }

            for (int i = -1; i < maxFillAmount + 1; i++)
            {
                if (i == -1)
                {
                    texture2D = _edge;
                    spriteBatch.Draw(texture2D, drawPos, null, Color, Rotation, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);
                }
                else if (i == maxFillAmount)
                {
                    texture2D = _edge;
                    spriteBatch.Draw(texture2D, drawPos, null, Color, Rotation, vector * NormalizedOrigin, ImageScale, SpriteEffects.FlipHorizontally, 0f);
                }
                else
                {
                    Vector2 o = new Vector2(0, 4);
                    if (i < filledAmount)
                    {
                        texture2D = _filled;
                        spriteBatch.Draw(texture2D, drawPos - o, null, Color, Rotation, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);

                        if (FlashColor.A > 1)
                        {
                            FlashColor *= 0.98f;
                            texture2D = _white;
                            spriteBatch.Draw(texture2D, drawPos - o, null, FlashColor, Rotation, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);
                        }
                    }
                    else
                    {
                        texture2D = _empty;
                        spriteBatch.Draw(texture2D, drawPos - o, null, Color, Rotation, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);



                        if (i == _staminaFlashIndex)
                        {
                            texture2D = _white;
                            float progress = ExtraMath.Osc(0f, 1f, speed: 12);
                            progress *= _flashStrength;
                            Color drawColor = Color.Lerp(Color.Transparent, Color.White, progress);
                            spriteBatch.Draw(texture2D, drawPos - o, null, drawColor, Rotation, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);
                        }
                    }
                }

                drawPos += new Vector2(texture2D.Size().X, 0);
            }
        }
    }
}
