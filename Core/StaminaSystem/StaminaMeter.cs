using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellamod.Core.SwingSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace Stellamod.Core.StaminaSystem
{
    [Autoload(Side = ModSide.Client)]
    internal class StaminaMeter : ModSystem
    {
        private UserInterface _interface;
        private StaminaMeterUI _staminaMeterUI;
        private GameTime _lastUpdateUiGameTime;
        public override void Load()
        {
            base.Load();
            if (!Main.dedServ)
            {
                _interface = new UserInterface();
                _staminaMeterUI = new StaminaMeterUI();
                _staminaMeterUI.Activate();
                _interface.SetState(_staminaMeterUI);
            }
        }

        public override void Unload()
        {
            base.Unload();
            //_staminaMeterUI?.SomeKindOfUnload(); // If you hold data that needs to be unloaded, call it in OO-fashion
            _staminaMeterUI = null;
        }


        public override void UpdateUI(GameTime gameTime)
        {
            _staminaMeterUI.Activate();
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
                    "Urdveil: Stamina Meter",
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

    internal class StaminaMeterUI : UIState
    {
        private string AssetDirectory = $"Stellamod/Core/StaminaSystem/";
        private Texture2D
            _staminaMeterEmpty,
            _staminaMeterEmptyEdge,
            _staminaMeterFilled,
            _staminaMeterFilledEdge;

        public override void OnActivate()
        {
            base.OnActivate();
            _staminaMeterEmpty = ModContent.Request<Texture2D>($"{AssetDirectory}StaminaMeterEmpty").Value;
            _staminaMeterEmptyEdge = ModContent.Request<Texture2D>($"{AssetDirectory}StaminaMeterEmptyEdge").Value;
            _staminaMeterFilled = ModContent.Request<Texture2D>($"{AssetDirectory}StaminaMeterFilled").Value;
            _staminaMeterFilledEdge = ModContent.Request<Texture2D>($"{AssetDirectory}StaminaMeterFilledEdge").Value;
        }

        public Color Color = Color.White;
        public bool ScaleToFit = false;
        public float ImageScale = 1f;
        public float Rotation;
        public bool AllowResizingDimensions = true;
        public Vector2 NormalizedOrigin = Vector2.Zero;
        private static Vector2? _drag = null;
        private static bool _isDragging;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Texture2D texture2D = _staminaMeterFilledEdge;
            Vector2 vector = texture2D.Size();
            var config = ModContent.GetInstance<StellamodClientConfig>();
            Vector2 ratioPos = new Vector2(config.StaminaMeterX, config.StaminaMeterY);
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

            SwingPlayer comboPlayer = Main.LocalPlayer.GetModPlayer<SwingPlayer>();
            int filledAmount = comboPlayer.Stamina;
            int maxFillAmount = comboPlayer.MaxStamina;
            if (comboPlayer.InfiniteStamina)
                filledAmount = maxFillAmount;



            Rectangle mouseRect = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Vector2 size = new Vector2(_staminaMeterFilledEdge.Size().X * (maxFillAmount + 1), _staminaMeterFilledEdge.Size().Y);
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
                    config.StaminaMeterX = newScreenRatioPosition.X;
                    config.StaminaMeterY = newScreenRatioPosition.Y;
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

            for (int i = 0; i < maxFillAmount; i++)
            {
                if (i == 0 || i == maxFillAmount - 1)
                {
                    SpriteEffects spriteEffects = i == maxFillAmount - 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    if (i < filledAmount)
                    {
                        texture2D = _staminaMeterFilledEdge;

                        spriteBatch.Draw(texture2D, drawPos, null, Color, Rotation, vector * NormalizedOrigin, ImageScale, spriteEffects, 0f);
                    }
                    else
                    {
                        texture2D = _staminaMeterEmptyEdge;
                        spriteBatch.Draw(texture2D, drawPos, null, Color, Rotation, vector * NormalizedOrigin, ImageScale, spriteEffects, 0f);
                    }
                }
                else
                {
                    if (i < filledAmount)
                    {
                        texture2D = _staminaMeterFilled;
                        spriteBatch.Draw(texture2D, drawPos, null, Color, Rotation, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        texture2D = _staminaMeterEmpty;
                        spriteBatch.Draw(texture2D, drawPos, null, Color, Rotation, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);
                    }
                }

                drawPos += new Vector2(texture2D.Size().X, 0);
            }


        }
    }
}
