using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace Stellamod.Common.GunSystem
{
    public class ReloadMeter : UIState
    {
        private Asset<AseSprite> _ammoSprite;
        public override void OnActivate()
        {
            base.OnActivate();
            _ammoSprite = ModContent.Request<AseSprite>(this.GetType().DirectoryHere() + "/Ammo");
        }

        public Color Color = Color.White;
        public float ImageScale = 1f;

        public Vector2 NormalizedOrigin = Vector2.Zero;
        private Vector2? _drag = null;
        private bool _isDragging;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            BaseGun gun = Main.LocalPlayer.HeldItem.ModItem as BaseGun;
            if (gun == null)
                return;
            if (!_ammoSprite.IsLoaded)
                return;

            Vector2 vector = _ammoSprite.Value.Size;
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
            Vector2 size = new Vector2(_ammoSprite.Value.Size.X, _ammoSprite.Value.Size.Y * gun.GetMaxAmmo(Main.LocalPlayer));
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
            int spacing = 12;
            drawPos += Main.screenPosition;
            for (int i = gun.GetMaxAmmo(Main.LocalPlayer) - 1; i >= 0; i--)
            {
                int k = i;
                int l = i / 24;
                Vector2 o = new Vector2(l * -spacing, k * 10 % 240);
                SpritebatchDrawer drawer = _ammoSprite.Value.GetSprite(frameIndex: 2, Main.screenPosition + o);
                drawer.color = Color.White;
                spriteBatch.Draw(drawer with { worldPosition = drawPos + o});

            }

            for (int i = gun.GetMaxAmmo(Main.LocalPlayer) - 1; i >= 0; i--)
            {
                int k = i;
                int l = i / 24;
                int frame = 0;
                if (i < gun.remainingAmmo)
                {
                    frame = 0;
                }
                else
                {
                    frame = 1;
                }


                Vector2 o = new Vector2(l * -spacing, k * 10 % 240);
                float f = (float)l / 4f;
                SpritebatchDrawer drawer = _ammoSprite.Value.GetSprite(frameIndex: frame, drawPos + o);
                drawer.color = Color.White;
                spriteBatch.Draw(drawer);
                k++;
                if (k >= 24)
                {
                    k = 0;
                    l++;
                }
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
