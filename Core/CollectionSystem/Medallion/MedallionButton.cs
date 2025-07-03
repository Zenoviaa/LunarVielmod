using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Content.Items.Fragments;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.CollectionSystem.Medallion
{
    internal class MedallionButton : UIPanel
    {
        private readonly BaseMedallionFragment _fragment;
        private Asset<Texture2D> FragmentTexture;
        private Color HighlightColor;
        public MedallionButton(BaseMedallionFragment fragment) : base()
        {

            _fragment = fragment;

            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        public float FlashProgress { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            float scale = 1f;
            FragmentTexture = ModContent.Request<Texture2D>(
                $"{CollectionBookUISystem.RootTexturePath}Medallion/Fragment{_fragment.Number}", AssetRequestMode.ImmediateLoad);


            Width.Set(FragmentTexture.Width() * scale, 0f);
            Height.Set(FragmentTexture.Height() * scale, 0f);
        }

        public bool IsBlackedOut()
        {
            FragmentPlayer fragmentPlayer = Main.LocalPlayer.GetModPlayer<FragmentPlayer>();
            return !_fragment.HasUnlocked(fragmentPlayer);
        }
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            FragmentPlayer fragmentPlayer = Main.LocalPlayer.GetModPlayer<FragmentPlayer>();
            if (_fragment.HasUnlocked(fragmentPlayer))
            {
                fragmentPlayer.HeldFragment = _fragment;
                FlashProgress = 1f;
                /*
                SoundStyle soundStyle = new SoundStyle("Urdveil/Assets/Sounds/CorsageRune1");
                SoundEngine.PlaySound(soundStyle);
             
                FXUtil.ShakeCamera(Main.LocalPlayer.position, 1024, 32);
                ShakeModSystem.Shake = 8;*/
            }
        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            FlashProgress *= 0.94f;
            if (IsMouseHovering)
            {
                CollectionBookUISystem uiSystem = ModContent.GetInstance<CollectionBookUISystem>();
                uiSystem.OpenDescriptionUI();

                DescriptionPageUI pageUI = uiSystem.fragmentDescriptionTabUIState.ui;
                pageUI.Description.SetText(_fragment.Effect.Value);

            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Color drawColor = Color.White;
            Rectangle rect = new Rectangle(point.X, point.Y, FragmentTexture.Width(), FragmentTexture.Height());
            Vector2 drawPos = new Vector2(rect.TopLeft().X, rect.TopLeft().Y);
            drawPos.X += VectorHelper.Osc(-2f, 2f, speed: 2, offset: _fragment.Number);
            drawPos.Y += VectorHelper.Osc(-2f, 2f, speed: 2, offset: _fragment.Number + 1);
            float rotation = 0;

            FragmentPlayer fragmentPlayer = Main.LocalPlayer.GetModPlayer<FragmentPlayer>();
            if (fragmentPlayer.HeldFragment != null)
            {
                if (fragmentPlayer.HeldFragment.Type == _fragment.Type)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);

                    for (float f = 0; f < 8f; f++)
                    {
                        Color circleGlowColor = Color.Goldenrod;
                        float progress = f / 8f;
                        float rot = progress * MathHelper.TwoPi;
                        rot += Main.GlobalTimeWrappedHourly;
                        Vector2 offsetDrawPos = drawPos + rot.ToRotationVector2() * VectorHelper.Osc(1f, 6f, speed: 4f);
                        spriteBatch.Draw(FragmentTexture.Value, offsetDrawPos, null, circleGlowColor, rotation, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
                        spriteBatch.Draw(FragmentTexture.Value, offsetDrawPos, null, circleGlowColor, rotation, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
                    }


                    spriteBatch.End();
                    spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
                }
            }


            spriteBatch.Draw(FragmentTexture.Value, drawPos, null, drawColor, rotation, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);

            Color glowColor = HighlightColor;
            spriteBatch.Draw(FragmentTexture.Value, drawPos, null, glowColor, rotation, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);

            Color flashColor = Color.White * FlashProgress;
            spriteBatch.Draw(FragmentTexture.Value, drawPos, null, flashColor, rotation, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(FragmentTexture.Value, drawPos, null, flashColor, rotation, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
            if (IsMouseHovering)
            {
                HighlightColor = Color.Lerp(HighlightColor, Color.White, 0.1f);
            }
            else
            {
                HighlightColor = Color.Lerp(HighlightColor, Color.Transparent, 0.1f);
            }

            if (IsBlackedOut())
            {
                spriteBatch.Draw(FragmentTexture.Value, drawPos, null, Color.Black, rotation, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
            }

            if (fragmentPlayer.HeldFragment != null)
            {
                /*
                if (fragmentPlayer.HeldFragment.Type == _fragment.Type)
                {
                    var shader = FirePixelShader.Instance;
                    shader.PrimaryColor = Color.Lerp(Color.White, new Color(255, 207, 79), 0.5f);
                    shader.NoiseColor = Color.Red;
                    shader.Distortion = 0.0075f;
                    shader.Speed = 10;
                    shader.Power = 0.01f;
                    shader.Apply();

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, default, default, default, default, Main.UIScaleMatrix);
                    shader.Data.Apply(null);

                    spriteBatch.Draw(FragmentTexture.Value, drawPos, null, flashColor, rotation, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

                    spriteBatch.End();
                    spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
                }*/
            }
        }
    }
}
