using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    public class ComboSystemDrawing : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            IComboSystem iweapon = drawInfo.drawPlayer.HeldItem.ModItem as IComboSystem;

            if (drawInfo.drawPlayer.GetModPlayer<ComboSystem>().Style != 1) // this means that the player has a combo going... so show it
                return true;

            return iweapon != null;
        }

        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.BeetleBuff); // need a higher layer dont really know

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            // system
            ComboSystem modPlr = drawInfo.drawPlayer.GetModPlayer<ComboSystem>();

            // draw combo counter

            // item
            Item heldItem = drawInfo.drawPlayer.HeldItem;
            IComboSystem comboItem = heldItem.ModItem as IComboSystem;

            if (comboItem != null)
            {
                // draw styles
                int curStyle = modPlr.CurrentStyle;
                int nextStyle = curStyle + 1;
                int prevStyle = curStyle - 1;
                if (nextStyle >= comboItem.ComboProjectilesIcons.Length)
                    nextStyle = 0;
                if (prevStyle < 0)
                    prevStyle = comboItem.ComboProjectilesIcons.Length - 1;

                Texture2D cStyleIcon = (Texture2D)ModContent.Request<Texture2D>(comboItem.ComboProjectilesIcons[curStyle]);
                Texture2D nStyleIcon = (Texture2D)ModContent.Request<Texture2D>(comboItem.ComboProjectilesIcons[nextStyle]);
                Texture2D pStyleIcon = (Texture2D)ModContent.Request<Texture2D>(comboItem.ComboProjectilesIcons[prevStyle]);

                Rectangle rect = new Rectangle(0, 0, 32, 32);

                drawInfo.DrawDataCache.Add(new DrawData(cStyleIcon, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 + 50f), rect, Color.White, 0f, new Vector2(16, 16), 1f, SpriteEffects.None, 0));
                drawInfo.DrawDataCache.Add(new DrawData(nStyleIcon, new Vector2(Main.screenWidth / 2 + 30f, Main.screenHeight / 2 + 50f), rect, Color.White, 0f, new Vector2(16f, 16f), 0.7f, SpriteEffects.None, 0));
                drawInfo.DrawDataCache.Add(new DrawData(pStyleIcon, new Vector2(Main.screenWidth / 2 - 30f, Main.screenHeight / 2 + 50f), rect, Color.White, 0f, new Vector2(16f, 16f), 0.7f, SpriteEffects.None, 0));

                // draw style charges
                if (modPlr.currentProjectile != -1 && drawInfo.drawPlayer.statLife > 0)
                {
                    IComboProjectile cp = Main.projectile[modPlr.currentProjectile].ModProjectile as IComboProjectile;

                    int maxCharges = cp.MaxCharges;
                    int curCharges = cp.CurCharges;
                    if (curCharges == -1)
                    {
                        curCharges = modPlr.inactiveCharge;
                        int loopTime = (ModContent.GetModProjectile(comboItem.ComboProjectiles[modPlr.CurrentStyle]) as IComboProjectile).projectileChargeLoopTime;
                        curCharges = Math.Abs((curCharges - (curCharges % loopTime)) / loopTime);
                    }

                    Texture2D ChargeFull = (Texture2D)ModContent.Request<Texture2D>(comboItem.FullCharge);
                    Texture2D ChargeEmpty = (Texture2D)ModContent.Request<Texture2D>(comboItem.EmptyCharge);

                    Rectangle chargeRect = new Rectangle(0, 0, 16, 16);

                    int moduleWhen = 4;

                    Vector2 drawOrigin = new Vector2(Main.screenWidth / 2 + 20, Main.screenHeight / 2 - 56f);

                    for (int i = 0; i < maxCharges; i++)
                    {
                        Texture2D usedCharge = i >= curCharges ? ChargeEmpty : ChargeFull;
                        drawInfo.DrawDataCache.Add(new DrawData(usedCharge, drawOrigin, chargeRect, Color.White, 0f, new Vector2(8, 8), 1f, SpriteEffects.None, 0));

                        if (i % moduleWhen == 0 && i != 0)
                        {
                            drawOrigin.X -= 18 * moduleWhen;
                            drawOrigin.Y += 18;
                        }
                        else
                        {
                            drawOrigin.X += 18;
                        }
                    }
                }

                // draw style meter
                Texture2D Pixel = (Texture2D)ModContent.Request<Texture2D>("DivergencyMod/Placeholder/WhitePixel");

                int maxWidth = 100;
                int width = (int)MathF.Floor(maxWidth * (modPlr.Style / ComboSystem.MaxStyle));

                Vector2 drawOriginPos = new Vector2(Main.screenWidth / 2 - maxWidth, Main.screenHeight / 2 - 60f);
                Vector2 barOrigin = new Vector2(0, 0);

                Rectangle back = new Rectangle(0, 0, maxWidth, 12);
                Rectangle fill = new Rectangle(0, 0, width, 12);

                Vector3 resColor = comboItem.ColorStart + ((comboItem.ColorEnd - comboItem.ColorStart) / 100f * width);

                drawInfo.DrawDataCache.Add(new DrawData(Pixel, drawOriginPos, back, Color.Gray, 0f, barOrigin, 1f, SpriteEffects.None, 0));
                drawInfo.DrawDataCache.Add(new DrawData(Pixel, drawOriginPos, fill, new Color(resColor), 0f, barOrigin, 1f, SpriteEffects.None, 0));

                // draw style meter reset timer
                int widthReset = (int)MathF.Floor(maxWidth * (modPlr.StyleResetTimer / ((float)ComboSystem.StyleResetTimerMax)));

                drawOriginPos.Y -= 4;

                fill = new Rectangle(0, 0, widthReset, 4);

                drawInfo.DrawDataCache.Add(new DrawData(Pixel, drawOriginPos, fill, Color.White, 0f, barOrigin, 1f, SpriteEffects.None, 0));

            }
        }
    }
}