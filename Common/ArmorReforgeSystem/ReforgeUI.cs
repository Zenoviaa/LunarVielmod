using ReLogic.Content;
using Stellamod.Common.ArmorReforge;
using Stellamod.Common.UI;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Stellamod.Common.ArmorReforgeSystem
{
    public class ReforgeUI : UIPanel
    {
        private float _particleSpawnTimer;
        private readonly Asset<Texture2D> _backgroundTextureAsset;
        public ReforgeSlot armorReforgeSlot;
        public ReforgeSlot accessoryReforgeSlot;
        public ReforgeButton reforgeButton;
        public ReforgePearl pearl;

        public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2) ;
        public int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2) - 64;

        public ReforgeUI()
        {
            _backgroundTextureAsset = ModContent.Request<Texture2D>($"{ReforgeUISystem.RootTexturePath}ReforgeBackground", AssetRequestMode.AsyncLoad);
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            if (!armorReforgeSlot.Item.IsAir)
            {
                Main.LocalPlayer.QuickSpawnItemDirect(Main.LocalPlayer.GetSource_FromThis(), armorReforgeSlot.Item);
                armorReforgeSlot.Item.TurnToAir();
            }

            if (!accessoryReforgeSlot.Item.IsAir)
            {
                Main.LocalPlayer.QuickSpawnItemDirect(Main.LocalPlayer.GetSource_FromThis(), accessoryReforgeSlot.Item);
                accessoryReforgeSlot.Item.TurnToAir();
            }
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 170;
            Height.Pixels = 258;

            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            armorReforgeSlot = new ReforgeSlot();
            Append(armorReforgeSlot);

            accessoryReforgeSlot = new ReforgeSlot();
            accessoryReforgeSlot.slotType = 1;
            Append(accessoryReforgeSlot);

            reforgeButton = new ReforgeButton();
            Append(reforgeButton);

            pearl = new ReforgePearl();
            Append(pearl);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Width.Pixels = 170;
            Height.Pixels = 258;

            if (Main.hasFocus)
            {
                _particleSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_particleSpawnTimer > 0.5f)
                {
                    Rectangle spawnRect = GetDimensions().ToRectangle();
                    Vector2 pos = new Vector2();
                    pos.X = Main.rand.Next(spawnRect.Left, spawnRect.Right);
                    pos.Y = Main.rand.Next(spawnRect.Top, spawnRect.Bottom);
                    Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(1f, 2f);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(75));
                    SparkleParticle dp = SparkleParticle.SpawnInUI(pos, velocity, Color.White, Scale: 0.5f);
                    dp.innerColor = Color.Lerp(Color.White, Color.Turquoise, Main.rand.NextFloat(0f, 1f));
                    dp.outerColor = Color.DarkGreen;
                    dp.gravity = 0;
                    dp.Scale *= 0.4f;
                    dp.dampening = 0.08f;
                    _particleSpawnTimer -= 0.1f;
                }


            }


            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop + ExtraMath.Osc(0f, 2f);

            int height = 8;
            armorReforgeSlot.Left.Pixels = 16;
            armorReforgeSlot.Top.Pixels = height;

            accessoryReforgeSlot.Left.Pixels = 80;
            accessoryReforgeSlot.Top.Pixels = height;


            pearl.Left.Pixels = (Width.Pixels / 2) - (pearl.Width.Pixels / 2) - 11;
            pearl.Top.Pixels = 128;

            reforgeButton.Left.Pixels = -24;
            reforgeButton.Top.Pixels = -16;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Vector2 drawPos = GetDimensions().ToRectangle().TopLeft();
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null,
                Main.UIScaleMatrix);

            float height = 0;
            if (!armorReforgeSlot.Item.IsAir)
            {
               
                Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
                Vector2 position = topLeft;
                position.X += 196;
                position.Y += ExtraMath.Osc(0f, 4f, speed: 2);
                //position.Y += Height.Pixels + 32;

                int width = (int)Width.Pixels;
                width *= 2;


                List<TooltipLine> lines = new List<TooltipLine>();
                ArmorReforgeGlobalItem globalItem = armorReforgeSlot.Item.GetGlobalItem<ArmorReforgeGlobalItem>();
                globalItem.ModifyTooltipsWithoutName(armorReforgeSlot.Item, lines);
                UIHelpers.DrawTooltips(spriteBatch, lines, position, width, 1);
                height = UIHelpers.CalculateTooltipsHeight(lines);

                Vector2 centerPos = position;
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, globalItem.GetNameTooltip(armorReforgeSlot.Item).Text,
                     centerPos + new Vector2(10f, 12) * 1f, Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 1f);
            } 

            if (!accessoryReforgeSlot.Item.IsAir)
            {
                Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
                Vector2 position = topLeft;
                position.X += 196;
                position.Y += ExtraMath.Osc(0f, 4f, speed: 2);
                position.Y += (int)height + 64;

                int width = (int)Width.Pixels;
                width *= 2;

                List<TooltipLine> lines = new List<TooltipLine>();
                AccessoryReforgeGlobalItem globalItem = accessoryReforgeSlot.Item.GetGlobalItem<AccessoryReforgeGlobalItem>();
                globalItem.ModifyTooltipsWithoutName(accessoryReforgeSlot.Item, lines);
                UIHelpers.DrawTooltips(spriteBatch, lines, position, width, 1f);

                Vector2 centerPos = position;
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, globalItem.GetNameTooltip(accessoryReforgeSlot.Item).Text,
                     centerPos + new Vector2(10f, 12) * 1f, Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 1f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null,
                Main.UIScaleMatrix);


            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_backgroundTextureAsset, Main.screenPosition + drawPos);
            drawer.color = Color.White;
            drawer.drawOrigin = Vector2.Zero;
            Rectangle frame = _backgroundTextureAsset.Value.GetFrame(0, 2);
            drawer.sourceRect = frame;
            spriteBatch.Draw(drawer);


            drawer.sourceRect = _backgroundTextureAsset.Value.GetFrame(1, 2);
            drawer.color = Color.Lerp(Color.Black, Color.Turquoise, ModContent.GetInstance<ReforgeUISystem>().flashTimer );
            drawer.color.A = 0;
            spriteBatch.Draw(drawer);

            //       Primitives2D.DrawRectangle(spriteBatch, GetDimensions().ToRectangle(), Color.Red);
        }
    }
}
