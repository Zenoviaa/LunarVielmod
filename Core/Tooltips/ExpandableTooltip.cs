using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellamod.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Core.Tooltips
{
    /// <summary>
    /// Implement this interface to add an expandable tooltip to an item!
    /// </summary>
    public interface IExpandableTooltip
    {
        void ModifyExpandableTooltips(Item item, List<TooltipLine> lines);
    }

    public abstract class AbstractExpandingTooltip : GlobalItem, IExpandableTooltip
    {
        public abstract void ModifyExpandableTooltips(Item item, List<TooltipLine> lines);
    }

    /// <summary>
    /// Sets an expandable tooltip
    /// </summary>
    public class ExpandableTooltipGlobalItem : GlobalItem
    {
        private static List<TooltipLine> _expandableLines;
        private static int _yOffset;
        private static int _xOffset;
        public override void PostDrawTooltip(Item item, ReadOnlyCollection<DrawableTooltipLine> lines)
        {
            base.PostDrawTooltip(item, lines);
            _yOffset = 0;
            _xOffset = 30;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Visible)
                {
                    _yOffset += (int)(FontAssets.MouseText.Value.MeasureString(line.Text).Y);
                }
            }
            _yOffset += 48;
   
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            //This is called when it is in the hover item
            //So we can just do the code here lol

            _expandableLines ??= new List<TooltipLine>();
            _expandableLines.Clear();
            ExpandableTooltipRenderer renderer = ModContent.GetInstance<ExpandableTooltipRenderer>();
            for (int i = 0; i < renderer.ExpandableTooltips.Length; i++)
            {
                IExpandableTooltip expandableTooltip = renderer.ExpandableTooltips[i];
                expandableTooltip.ModifyExpandableTooltips(item, _expandableLines);
            }
        
            if (_expandableLines.Count > 0)
            {
                Keys keys = Keys.LeftShift;
                bool isExpanded = Main.keyState.IsKeyDown(keys);
                TooltipLine helpLine = new TooltipLine(Mod, "ExpandHelp", LangText.Common("ExpandableTooltipHelp", "Left Shift"));
                helpLine.OverrideColor = Color.Lerp(Color.White, Color.Black, 0.7f);
                tooltips.Add(helpLine);
                if (isExpanded)
                {
                    renderer.SetTooltipsToDraw(_expandableLines, _xOffset, _yOffset);
                }
           
            }

      
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class ExpandableTooltipRenderer : ModSystem
    {
        private List<TooltipLine> _lines;
        private int _startingXOffset;
        private int _startingYOffset;
        private GameTime _lastUpdateUiGameTime;
        private float _timer;
        private bool _holdingTooltip;
        public override void OnModLoad()
        {
            base.OnModLoad();
            List<IExpandableTooltip> modifiers = new List<IExpandableTooltip>();
            foreach (var item in ModContent.GetContent<AbstractExpandingTooltip>())
            {
                modifiers.Add(item);
            }
            ExpandableTooltips = modifiers.ToArray();
        }

        public IExpandableTooltip[] ExpandableTooltips { get; private set; }

        public void SetTooltipsToDraw(List<TooltipLine> lines, int startingXOffset, int startingYOffset)
        {
            _lines = lines;
            _startingXOffset = startingXOffset;
            _startingYOffset = startingYOffset;
            _holdingTooltip = true;
        }
        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timer += deltaTime * (_holdingTooltip ? 1 : -1);
            _timer = MathHelper.Clamp(_timer, 0f, 1f);
            _holdingTooltip = false;

        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            base.ModifyInterfaceLayers(layers);
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Scarlet Sun: Expandable Tooltip",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && _lines != null)
                        {
                            int targetX = Main.mouseX + _startingXOffset;
                            int targetY = Main.mouseY + _startingYOffset;

                            float ratio = _timer / 1f;
                            float ease = EasingFunction.OutExpo(ratio);
                            int x = (int)MathHelper.Lerp(targetX - 128, targetX, ease);
                            int y = targetY;
                            ExpandableTooltip.DrawExpandableTooltip(Main.spriteBatch, _lines, x, y, ease);
                            if(_timer <= 0)
                                _lines = null;
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }


    /// <summary>
    /// Handles drawing another tooltip window that isn't linked to a specific item
    /// </summary>
    public static class ExpandableTooltip
    {

        //modified from vanilla code so we can draw our own tooltip wherever we want
        public static void DrawExpandableTooltip(SpriteBatch spriteBatch, List<TooltipLine> lines, int X, int Y, float alpha)
        {
            Color color = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor);
            Vector2 zero = Vector2.Zero;
            List<DrawableTooltipLine> drawableLines = lines.Select((TooltipLine x, int i) => new DrawableTooltipLine(x, i, X, Y, Color.White * alpha)).ToList();

            for (int j = 0; j < lines.Count; j++)
            {
                Vector2 stringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, lines[j].Text, Vector2.One);
                if (stringSize.X > zero.X)
                    zero.X = stringSize.X;

                zero.Y += stringSize.Y;
            }

            int toolTipDistance = 6;
            X += toolTipDistance;
            Y += toolTipDistance;
            int num13 = 4;
            float num3 = (float)(int)Main.mouseTextColor / 255f;
            float num4 = num3;

            int num14 = Main.screenWidth;
            int num15 = Main.screenHeight;
            if ((float)X + zero.X + (float)num13 > (float)num14)
                X = (int)((float)num14 - zero.X - (float)num13);

            if ((float)Y + zero.Y + (float)num13 > (float)num15)
                Y = (int)((float)num15 - zero.Y - (float)num13);

            int yOffset = 0;
            int num17 = 14;
            int num18 = 9;
            Utils.DrawInvBG(spriteBatch, new Rectangle(X - num17, Y - num18, (int)zero.X + num17 * 2, (int)zero.Y + num18 + num18 / 2), new Color(23, 25, 81, 255) * 0.925f * alpha);
            for (int k = 0; k < lines.Count; k++)
            {
                Color black = new Color(num4, num4, num4, num4);
                Color realLineColor = black;
                if (drawableLines[k].OverrideColor.HasValue)
                {
                    realLineColor = drawableLines[k].OverrideColor.Value * num4;
                }

                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, drawableLines[k].Font, drawableLines[k].Text, new Vector2(drawableLines[k].X, drawableLines[k].Y + yOffset), realLineColor * alpha, drawableLines[k].Rotation, drawableLines[k].Origin, drawableLines[k].BaseScale, drawableLines[k].MaxWidth, drawableLines[k].Spread);
                yOffset += (int)(FontAssets.MouseText.Value.MeasureString(drawableLines[k].Text).Y);
            }
        }
    }
}
