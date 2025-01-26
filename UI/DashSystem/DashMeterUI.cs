namespace Stellamod.UI.DashSystem
{
    /*
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
                    "LunarVeil: Dash Meter",
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
        private string AssetDirectory = $"Stellamod/UI/DashSystem/";
        private Texture2D
            _outline,
            _filled;

        public override void OnActivate()
        {
            base.OnActivate();
            _outline = ModContent.Request<Texture2D>($"{AssetDirectory}DashMeterOutline").Value;
            _filled = ModContent.Request<Texture2D>($"{AssetDirectory}DashMeterFill").Value;
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
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            Vector2 ratioPos = new Vector2(50, 55);
            Vector2 drawPos = ratioPos;
            drawPos.X = (int)(drawPos.X * 0.01f * Main.screenWidth);
            drawPos.Y = (int)(drawPos.Y * 0.01f * Main.screenHeight);

            DashPlayer dashPlayer = Main.LocalPlayer.GetModPlayer<DashPlayer>();
            Color drawColor = Color.White;
            if (dashPlayer.FillProgress < 0.1f)
            {
                Color = Color.Lerp(Color, Color.White, 0.3f);
            }
            else if (dashPlayer.FillProgress >= 1f)
            {
                Color = Color.Lerp(Color, Color.Transparent, 0.3f);
            }

            //Draw Outline
            texture2D = _outline;
            Vector2 drawScale = Vector2.One;

            //Fix the position
            drawPos.X -= _filled.Width / 2f;
            drawPos.Y -= 16;
            spriteBatch.Draw(texture2D, drawPos, null, Color, Rotation, vector * NormalizedOrigin, drawScale, SpriteEffects.None, 0f);

            //Draw Fill
            texture2D = _filled;

            float width = dashPlayer.FillProgress * _filled.Width;
            Rectangle source = new Rectangle(0, 0, (int)width, _filled.Height);
            spriteBatch.Draw(texture2D, drawPos, source, Color, Rotation, vector * NormalizedOrigin, drawScale, SpriteEffects.None, 0f);
        }
    }*/
}
