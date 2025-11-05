using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using Stellamod.Systems.MiscellaneousMath;
using Stellamod.UI.DashSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace Stellamod.Core.GunSystem
{
    public class ReloadMeter : UIState
    {
        private Texture2D
            _empty,
            _filled;

        public override void OnActivate()
        {
            base.OnActivate();
            _empty = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/AmmoEmpty").Value;
            _filled = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/AmmoFilled").Value;
        }

        public Color Color = Color.White;
        public float ImageScale = 1f;

        public Vector2 NormalizedOrigin = Vector2.Zero;
        private static Vector2? _drag = null;
        private static bool _isDragging;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            BaseGun gun = Main.LocalPlayer.HeldItem.ModItem as BaseGun;
            if (gun == null)
                return;


            Texture2D texture2D = _filled;
            Vector2 vector = texture2D.Size();
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            Vector2 ratioPos = new Vector2(config.AmmoBarX, config.AmmoBarY);
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

            Rectangle mouseRect = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
            Vector2 size = new Vector2(_filled.Width, _filled.Height * gun.maxAmmo);
            Rectangle barRect = Utils.CenteredRectangle(drawPos + size / 2, size * Main.UIScale);
            barRect.Location -= new Point(0, (int)(size.Y / 2));
            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Main.MouseScreen;
            Vector2 newScreenRatioPosition = ratioPos;
            if (barRect.Intersects(mouseRect) && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                _isDragging = true;
            }

            if (ms.LeftButton == ButtonState.Pressed && !_isDragging && barRect.Intersects(mouseRect) && !PlayerInput.IgnoreMouseInterface)
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
                newScreenRatioPosition.X = (100f * newCorner.X) / Main.screenWidth;
                newScreenRatioPosition.Y = (100f * newCorner.Y) / Main.screenHeight;

                // Compute the change in position. If it is large enough, actually move the meter
                Vector2 delta = newScreenRatioPosition - ratioPos;
                if (Math.Abs(delta.X) >= 0.05f || Math.Abs(delta.Y) >= 0.05f)
                {
                    config.AmmoBarX = newScreenRatioPosition.X;
                    config.AmmoBarY = newScreenRatioPosition.Y;
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



            //Draw Outline
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, default, default, default, SpriteWhiteShader.Instance.Effect, Main.UIScaleMatrix);

            for (int i = 0; i < gun.GetMaxAmmo(Main.LocalPlayer); i++)
            {
                if (i < gun.remainingAmmo)
                {
                    texture2D = _filled;

                }
                else
                {
                    texture2D = _empty;

                }


                Vector2 o = new Vector2(0, i * 10);
                spriteBatch.Draw(texture2D, drawPos - o + Vector2.UnitX * 2, null, Color.White, 0, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture2D, drawPos - o + Vector2.UnitX * -2, null, Color.White, 0, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture2D, drawPos - o + Vector2.UnitY * 2, null, Color.White, 0, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture2D, drawPos - o + Vector2.UnitY * -2, null, Color.White, 0, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
            for (int i = 0; i < gun.GetMaxAmmo(Main.LocalPlayer); i++)
            {
                if (i < gun.remainingAmmo)
                {
                    texture2D = _filled;
                
                }
                else
                {
                    texture2D = _empty;
              
                }


                Vector2 o = new Vector2(0, i * 10);
                spriteBatch.Draw(texture2D, drawPos - o, null, Color.White, 0, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);
            }
        }
    }
    [Autoload(Side = ModSide.Client)]
    public class ReloadSystem : ModSystem
    {
        private UserInterface _interface;
        private ReloadMeter _reloadMeter;
        private GameTime _lastUpdateUiGameTime;
        public override void Load()
        {
            base.Load();
            if (!Main.dedServ)
            {
                _interface = new UserInterface();
                _reloadMeter = new ReloadMeter();
                _reloadMeter.Activate();
                _interface.SetState(_reloadMeter);
            }
        }

        public override void Unload()
        {
            base.Unload();
            _reloadMeter = null;
        }


        public override void UpdateUI(GameTime gameTime)
        {
            _reloadMeter.Activate();
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
                    "LunarVeil: Reload System",
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
}
