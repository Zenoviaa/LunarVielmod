using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.CollectionSystem
{
    internal class Book : UIElement
    {
        private readonly int _context;
        private readonly float _scale;

        private int timer = 0;
        private int _frame;
        private float _closeTimer;
        private float _frameCounter;
        private bool _open;

        private enum State
        {
            Open,
            Opened,
            Close,
            Closed
        }

        private State _state;

        internal Book(float scale = 1f)
        {
            _scale = scale;
            _state = State.Closed;
            _closeTimer = 1f;
            Offset = Vector2.UnitY * -768;
            var asset = ModContent.Request<Texture2D>(
                $"{CollectionBookUISystem.RootTexturePath}Book", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            var rect = asset.Value.GetFrame(0, 10);
            Width.Set(rect.Size().X * scale * 1.75f, 0f);
            Height.Set(rect.Size().Y * scale * 1.75f, 0f);
        }

        public Vector2 Offset;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            CollectionBookUISystem collectionBookUISystem = ModContent.GetInstance<CollectionBookUISystem>();
            if (_open)
            {

                _closeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(_closeTimer <= 0)
                {
                    _closeTimer = 0;
                }
            }
            else
            {
                _closeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(_closeTimer >= 1f)
                {
                    _closeTimer = 1f;
                }
            }

            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            float progress = _closeTimer / 1f;
            float easedProgress = Easing.InOutCubic(progress);
            Offset = Vector2.Lerp(Vector2.Zero, Vector2.UnitY * -900, easedProgress);
            switch (_state)
            {
                case State.Open:
                    _frameCounter += (float)gameTime.ElapsedGameTime.TotalSeconds * 10;
                    if (_frameCounter >= 1f)
                    {
                        _frame++;
                        _frameCounter = 0;
                    }

                    if (_frame >= 10)
                    {
                        _frame = 9;
                        _state = State.Opened;

                        //Open to collection page
                        collectionBookUISystem.OpenQuestsTabUI();
                    }
                    _open = true;
                    break;
                case State.Opened:
                    _frameCounter = 0;
                    _frame = 9;
                    _open = true;
                    break;
                case State.Close:
                    _frameCounter += (float)gameTime.ElapsedGameTime.TotalSeconds * 10;
                    if (_frameCounter >= 1f)
                    {
                        _frame--;
                        _frameCounter = 0;
                    }
                    if (_frame < 0)
                    {
                        _state = State.Closed;
                        _frame = 0;
                    }
                    _open = true;
                    break;
                case State.Closed:
                    _frameCounter = 0;
                    _frame = 0;
                
                    if(_closeTimer >= 1f)
                    {
             
                        collectionBookUISystem.ReallyCloseBookUI();
                    }
                    _open = false;
                    break;
            }
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();

            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();

            Texture2D texture = ModContent.Request<Texture2D>(
                $"{CollectionBookUISystem.RootTexturePath}Book").Value;
          
            int offset = (int)(texture.Size().Y / 2);
            Vector2 drawOrigin = texture.GetFrame(_frame, totalFrameCount: 10).Size() / 2f;
            Vector2 centerPos = pos + drawOrigin;
       
            centerPos += Offset;
            spriteBatch.Draw(texture, centerPos, texture.GetFrame(_frame, totalFrameCount: 10), Color.White, 0f, drawOrigin, _scale * 1.75f, SpriteEffects.None, 0f);
            Main.inventoryScale = oldScale;
        }

        public void Toggle()
        {
            if(_state == State.Open || _state == State.Opened)
            {
                _state = State.Close;
            } else if (_state == State.Close || _state == State.Closed)
            {
                _state = State.Open;
            }
        }
        public void Open()
        {
            if (_state != State.Open && _state != State.Opened)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/BookOpen");
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle);
                _state = State.Open;
            }
        }

        public void Close()
        {
            if (_state != State.Close && _state != State.Closed)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/BookClose");
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle);
                _state = State.Close;
            }
        }
    }
}
