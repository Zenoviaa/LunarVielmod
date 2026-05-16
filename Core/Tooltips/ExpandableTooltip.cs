using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Common.ArmorRework;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Stellamod.UI.CollectionSystem;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using TerrariaHooks;

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


    [Autoload(Side = ModSide.Client)]
    public class ExpandableLineSystem : ModSystem
    {
        private List<TooltipLine> _expandableLines;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _expandableLines = new List<TooltipLine>();
        }
        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
            Player player = Main.LocalPlayer;
            _expandableLines.Clear();
            Item item = Main.HoverItem;
            if (item == null)
                return;
            if (item.IsAir)
                return;
            if (item.ModItem == null)
                return;

            ExpandableTooltipRenderer renderer = ModContent.GetInstance<ExpandableTooltipRenderer>();
            for (int i = 0; i < renderer.ExpandableTooltips.Length; i++)
            {
                IExpandableTooltip expandableTooltip = renderer.ExpandableTooltips[i];
                expandableTooltip.ModifyExpandableTooltips(item, _expandableLines);
            }
            if (item.headSlot == -1 && item.bodySlot == -1 && item.legSlot == -1)
                return;
            if (_expandableLines.Count <= 0)
                return;


            Keys keys = Keys.LeftShift;
            bool isExpanded = Main.keyState.IsKeyDown(keys);
            if (isExpanded)
            {
                ArmorSet set = ArmorSetSystem.FindArmorSet(item.type);
                ArmorSetSystem.GetArmorSet(set, out Item helm, out Item armor, out Item leggings);
                if (helm.ModItem == null)
                    return;

                string lore = LangText.Armor(helm.ModItem, "Lore");
                ArmorTooltipSystem tooltipsSystem = ModContent.GetInstance<ArmorTooltipSystem>();
                tooltipsSystem.InspectArmor(item, lore, LangText.Armor("SetBonus", LangText.Armor(helm.ModItem, "SetBonus")), _expandableLines);
            }
        }

        public List<TooltipLine> GetExpandableLines()
        {
            return _expandableLines;
        }
    }
    /// <summary>
    /// Sets an expandable tooltip
    /// </summary>
    public class ExpandableTooltipGlobalItem : GlobalItem
    {
       
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
            _yOffset += 64;

        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            //This is called when it is in the hover item
            //So we can just do the code here lol

            ExpandableTooltipRenderer renderer = ModContent.GetInstance<ExpandableTooltipRenderer>();
            ExpandableLineSystem lineSystem = ModContent.GetInstance<ExpandableLineSystem>();
            var lines = lineSystem.GetExpandableLines();
            if (lines.Count > 0)
            {
                Keys keys = Keys.LeftShift;
                bool isExpanded = Main.keyState.IsKeyDown(keys);
                TooltipLine helpLine = new TooltipLine(Mod, "ExpandHelp", LangText.Common("ExpandableTooltipHelp", "Left Shift"));
                helpLine.OverrideColor = Color.Lerp(Color.White, Color.Black, 0.7f);
                tooltips.Add(helpLine);
                if (isExpanded)
                {

                    if (item.headSlot != -1 || item.bodySlot != -1 || item.legSlot != -1)
                    {

                        //renderer.SetArmorTooltipsToDraw(item, _expandableLines, _xOffset, _yOffset);

                    }
                    else
                    {
                        renderer.SetTooltipsToDraw(lines, _xOffset, _yOffset);
                    }
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
        private bool _drawGlass;
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

        public bool armorDraw;

        public IExpandableTooltip[] ExpandableTooltips { get; private set; }
        public float EaseTime => 0.9f;
     
        public void SetTooltipsToDraw(List<TooltipLine> lines, int startingXOffset, int startingYOffset, bool drawGlass = true)
        {
            _lines = lines;
            _startingXOffset = startingXOffset;
            _startingYOffset = startingYOffset;
            _holdingTooltip = true;
            _drawGlass = drawGlass;
        }
        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timer += deltaTime * (_holdingTooltip ? 1 : -1);
            _timer = MathHelper.Clamp(_timer, 0f, EaseTime);
            _holdingTooltip = false;

        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            base.ModifyInterfaceLayers(layers);
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex + 1, new LegacyGameInterfaceLayer(
                    "Scarlet Sun: Expandable Tooltip",
                    delegate
                    {

                        // ExpandableTooltip.DrawArmorPreview(new Vector2(Main.screenWidth, Main.screenHeight) * 0.45f, ModContent.GetInstance<JacklerHat>().Item, ModContent.GetInstance<JacklerCoat>().Item, ModContent.GetInstance<JacklerPants>().Item);
                        if (_lastUpdateUiGameTime != null && _lines != null)
                        {
                            int targetX = Main.mouseX + _startingXOffset;
                            int targetY = Main.mouseY + _startingYOffset;

                            float ratio = _timer / EaseTime;
                            float ease = EasingFunction.OutExpo(ratio);
                            int x = (int)MathHelper.Lerp(targetX - 128, targetX, ease);
                            int y = targetY;
                             ExpandableTooltip.DrawExpandableTooltip(Main.spriteBatch, _lines, x, y, ease, _drawGlass);
                        
                            if (_timer <= 0)
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
    [Autoload(Side = ModSide.Client)]
    public class ExpandableTooltip : ModSystem
    {
        private static ArmorReworkPlayerRenderer _playerRenderer;
        private static Player _player;
        private static Asset<Texture2D> _inspectTextureAsset;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _inspectTextureAsset = ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/InspectButton");
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            _inspectTextureAsset = null;
            _player = null;
        }
        public static long CountCurrency(Item[] inv)
        {
            long num = 0L;
            for (int i = 0; i < inv.Length; i++)
            {
                if (inv[i].type == ItemID.CopperCoin)
                    num += inv[i].stack;
                else if (inv[i].type == ItemID.SilverCoin)
                    num += inv[i].stack * 100;
                else if (inv[i].type == ItemID.GoldCoin)
                    num += inv[i].stack * 100 * 100;
                else if (inv[i].type == ItemID.PlatinumCoin)
                    num += inv[i].stack * 100 * 100 * 100;

            }
            return num;
        }
        private static void DrawLocalPlayer(Player player, Vector2 position)
        {
            //Substract the screen position because we want to draw in bounds of the UI, and this is auto-generated
            Vector2 drawPosition = position + Main.screenPosition;

            Vector2 left = drawPosition;
            float rotation = player.fullRotation;
            _playerRenderer ??= new();
            IPlayerRenderer playerRenderer = _playerRenderer;
            var camera = new Terraria.Graphics.Camera();
         
      
            playerRenderer.DrawPlayer(Main.Camera, player, left, rotation, player.fullRotationOrigin);
        }


  
        public static void DrawArmorPreview(Vector2 position, Item helmet, Item armor, Item leggings)
        {
            _player ??= new Player();

            _player.armor[0] = helmet;
            _player.armor[1] = armor;
            _player.armor[2] = leggings;

            _player.head = helmet.headSlot;
            _player.body = armor.bodySlot;
            if(!leggings.IsAir)
                _player.legs = leggings.legSlot;

            DrawLocalPlayer(_player, position);
        }


        public static void DrawArmorTooltip(Item item, SpriteBatch spriteBatch, List<TooltipLine> lines, int X, int Y, float alpha)
        {
            if (item.IsAir)
                return;

            ArmorSet set = ArmorSetSystem.FindArmorSet(item.type);
            //First draw a big box

            Vector2 bgDimensions = new Vector2(374, 180);
            int num17 = 14;
            int num18 = 9;

            int num13 = 4;
            int num14 = Main.screenWidth;
            int num15 = Main.screenHeight;
            if ((float)X + bgDimensions.X + (float)num13 > (float)num14)
                X = (int)((float)num14 - bgDimensions.X - (float)num13);

            if ((float)Y + bgDimensions.Y + (float)num13 > (float)num15)
                Y = (int)((float)num15 - bgDimensions.Y - (float)num13);

            int width = (int)bgDimensions.X + num17 * 2;
            int height = (int)bgDimensions.Y + num18 + num18 / 2;
            Vector2 topLeftBackground = new Vector2(X - num17, Y - num18);


            Rectangle bgDrawRect = CalculateBGDrawRect(lines, Main.mouseX, Y, alpha);
            bgDrawRect.X -= bgDrawRect.Width + 4;

            if (alpha > 0)
            {
                Utils.DrawInvBG(spriteBatch, new Rectangle((int)bgDrawRect.X, (int)bgDrawRect.Y, (int)(width * alpha), (int)(height * alpha)), new Color(23, 25, 81, 255) * 0.925f * alpha);
            }

            //Step. 2 Draw the item stat tooltips
            ArmorStatsPlayer armorStatsPlayer = Main.LocalPlayer.GetModPlayer<ArmorStatsPlayer>();

            ArmorSetSystem.GetArmorSet(set, out Item helm, out Item armor, out Item leggings);


            /*
            //Lore!!!
            TooltipLine loreLine = new TooltipLine(Stellamod.Instance, "Lore", LangText.Armor(helm.ModItem, "Lore"));
          
            lines.Add(loreLine);*/

            DrawExpandableTooltip(spriteBatch, lines, bgDrawRect.X + 8, bgDrawRect.Y + 4, alpha, false, armorStatsPlayer.RequestIconTexture);

            spriteBatch.End();
            spriteBatch.Begin(default, default, Main.graphics.GraphicsDevice.SamplerStates[0], default, Main.Rasterizer, null, Main.UIScaleMatrix);

            Vector2 playerDrawPosition = bgDrawRect.TopLeft() + new Vector2(width * 0.8f, height * 0.2f);
            _player ??= new Player();
            _player.opacityForAnimation = alpha;
            DrawArmorPreview(playerDrawPosition, helm, armor, leggings);
            spriteBatch.End();
            spriteBatch.Begin(default, default, Main.graphics.GraphicsDevice.SamplerStates[0], default, Main.Rasterizer, SpriteWhiteShader.Instance.Effect, Main.UIScaleMatrix);

            //Step 3. Draw item icon of the current item
            Vector2 topRight = bgDrawRect.TopLeft();
            topRight.X += width * 0.9f; ;
            topRight.Y += height * 0.1f;

            /*
            Main.inventoryScale = 1;
          
            Item air = new Item();
            air.SetDefaults(0);

            ItemSlot.Draw(spriteBatch, ref air, ItemSlot.Context.EquipMiscDye, new Vector2(540, 600), Color.White);
            ItemSlot.Draw(spriteBatch, ref air, ItemSlot.Context.EquipAccessoryVanity, new Vector2(570, 600), Color.White);
            ItemSlot.Draw(spriteBatch, ref air, ItemSlot.Context.EquipAccessory, new Vector2(600, 600), Color.White);
            */
            for (float f = 0; f < 4f; f++)
            {
                Color outlineColor = Color.White * alpha;
                outlineColor *= (int)ExtraMath.Osc(0f, 2f, speed: 3);
                ItemSlot.DrawItemIcon(item, 0, spriteBatch, topRight + (Vector2.UnitY * 2).RotatedBy(f / 4f * MathHelper.TwoPi), 1, 32, outlineColor);
            }


            spriteBatch.End();
            spriteBatch.Begin(default, default, Main.graphics.GraphicsDevice.SamplerStates[0], default, Main.Rasterizer, null, Main.UIScaleMatrix);

            ItemSlot.DrawItemIcon(item, 0, spriteBatch, topRight, 1, 32, Color.White * alpha);
        }
        public static Rectangle GetBGRectangle(int X, int Y, int Width, int Height)
        {
            Vector2 zero = Vector2.Zero;

            zero.X = Width;
            zero.Y = Height;
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

            int num17 = 14;
            int num18 = 9;
            int width = (int)zero.X + num17 * 2;
            int height = (int)zero.Y + num18 + num18 / 2;
            return new Rectangle(X - num17, Y - num18, width, height);
        }
        public static Rectangle GetBGRectangle(List<TooltipLine> lines, int X, int Y, float alpha)
        {
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

            int num17 = 14;
            int num18 = 9;
            int width = (int)zero.X + num17 * 2;
            int height = (int)zero.Y + num18 + num18 / 2;
            return new Rectangle(X - num17, Y - num18, width, height);
        }

        private static Rectangle CalculateBGDrawRect(List<TooltipLine> lines, int X, int Y, float alpha)
        {
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

            int num17 = 14;
            int num18 = 9;
            int width = (int)zero.X + num17 * 2;
            int height = (int)zero.Y + num18 + num18 / 2;
            return new Rectangle(X - num17, Y - num18, width, height);
        }

        //modified from vanilla code so we can draw our own tooltip wherever we want
        public delegate Asset<Texture2D> LookupFunction(string path);
        public static void DrawExpandableTooltip(SpriteBatch spriteBatch, List<TooltipLine> lines, int X, int Y, float alpha, bool drawGlass = true, LookupFunction iconTextureLookup = null)
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
            if (alpha > 0)
            {
                int width = (int)zero.X + num17 * 2;
                int height = (int)zero.Y + num18 + num18 / 2;
                Utils.DrawInvBG(spriteBatch, new Rectangle(X - num17, Y - num18, (int)(width * alpha), (int)(height * alpha)), new Color(23, 25, 81, 255) * 0.925f * alpha);
            }

            for (int k = 0; k < lines.Count; k++)
            {
                Color black = new Color(num4, num4, num4, num4);
                Color realLineColor = black * alpha;
                var line = lines[k];
                DrawableTooltipLine drawableLine = drawableLines[k];
                if (drawableLine.OverrideColor.HasValue)
                {
                    realLineColor = drawableLine.OverrideColor.Value * num4;
                }

                //Draw the icon if it has one
                if (iconTextureLookup != null)
                {
                    string name = line.Name;
                    Asset<Texture2D> iconTextureAsset = iconTextureLookup(name);
                    Vector2 drawOrigin = iconTextureAsset.Size() / 2f;
                    Vector2 drawPosition = new Vector2(X, drawableLine.Y + yOffset);
                    drawPosition.X -= 16;
                    drawPosition.Y += 8;
                    spriteBatch.Draw(iconTextureAsset.Value, drawPosition, null, Color.White * alpha, 0, drawOrigin, 1, SpriteEffects.None, 0);
                }

                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, drawableLine.Font, drawableLine.Text,
                    new Vector2(X, drawableLine.Y + yOffset), realLineColor * alpha, drawableLine.Rotation, drawableLine.Origin, drawableLine.BaseScale * alpha,
                    drawableLine.MaxWidth, drawableLine.Spread);
                yOffset += (int)(FontAssets.MouseText.Value.MeasureString(drawableLine.Text).Y);
            }

            if (drawGlass)
            {

                Vector2 drawOrigin = _inspectTextureAsset.Size() / 2f;
                Vector2 drawCenter = new Vector2(X, Y);
                drawCenter.Y += ExtraMath.Osc(0f, 2f);
                drawCenter.X -= 16;
                drawCenter.Y -= 16;
                spriteBatch.Draw(_inspectTextureAsset.Value, drawCenter, null, Color.White * alpha, 0, drawOrigin, alpha, SpriteEffects.None, 0);
            }
        }
    }
}
