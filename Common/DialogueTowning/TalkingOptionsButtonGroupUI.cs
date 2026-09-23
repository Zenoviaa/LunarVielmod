using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Common.DialogueTowning
{
    public class TalkingOptionsButtonGroupUI : UIPanel
    {
        private int _index;
        private TalkingOptionButtonUI[] _buttons;
        public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2) - 54;
        public int RelativeTop => Main.screenHeight - 220;
        public Vector2 DrawPos => new Vector2(Left.Pixels, Top.Pixels);

        public const int Max_Dialogue_Options = 4;
        public TalkingOptionsButtonGroupUI()
        {
            _buttons = new TalkingOptionButtonUI[Max_Dialogue_Options];
            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttons[i] = new TalkingOptionButtonUI();
            }
        }
        public Vector2 Offset { get; set; }
        public float Alpha { get; set; }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 480;
            Height.Pixels = 200;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            for (int i = 0; i < _buttons.Length; i++)
            {
                var btn = _buttons[i];
                Append(_buttons[i]);
            }
        }

        public void ClearButtons()
        {
            _index = 0;

        }

        public void AddButton(ITalkingOption talkingOption)
        {
            _buttons[_index].SetTalkingOption(talkingOption);
            _index++;
        }


        public override void Update(GameTime gameTime)
        {
            BackgroundColor = Color.Transparent;

            base.Update(gameTime);
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;


            var dx = 0;
            var dy = 0;
            for (int y = 0; y < _buttons.Length; y++)
            {
                var f = (float)y / (float)_index;
                var btn = _buttons[y];

                btn.Top.Pixels = dy * 64 - 14 ;
                btn.Top.Pixels += ExtraMath.Osc(-2f, 2f, speed: 2, y);
                btn.Left.Pixels = dx * 380 - 8;
                btn.Alpha = y < _index ? 1 : 0;
                btn.Alpha *= Alpha;

                dx++;
                if(dx >= 2)
                {
                    dx = 0;
                    dy++;
                }
            }
        }
    }
}
