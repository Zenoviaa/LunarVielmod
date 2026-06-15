using Stellamod.Common.ClassReworkSystem.AmmoRework.UI;
using Stellamod.Common.UI;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.WeaponTypes.CombatTools.UI
{
    #region UI
    public class CombatToolMeunUIState : UIState
    {
        public CombatToolBrowserWindow xixianFlaskUI;
        public CommonBackButton backButton;
        public CombatToolMeunUIState() : base()
        {

        }
        public float timer;
        public bool isOpen;
        private int RelativeLeft => Main.screenWidth / 2 - (int)Width.Pixels / 2;
        private int RelativeTop => Main.screenHeight / 2 - (int)Height.Pixels / 2 - 64;
        public override void OnInitialize()
        {
            xixianFlaskUI = new();
            Append(xixianFlaskUI);

            backButton = new CommonBackButton(Close); // );
            backButton.Left.Pixels = RelativeLeft;
            backButton.Top.Pixels = RelativeTop;
            Append(backButton);
        }
        public override void OnActivate()
        {
            base.OnActivate();
            isOpen = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            backButton.Left.Pixels = RelativeLeft + 128;
            backButton.Top.Pixels = RelativeTop - 92;
            if (FullyClosed())
            {
                ModContent.GetInstance<CombatToolUISysten>().CloseUI();
            }
            if (isOpen)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            float time = 0.5f;
            timer = MathHelper.Clamp(timer, 0, time);
            float t = EasingFunction.InOutSine(timer / time);
            xixianFlaskUI.InventoryMenu.View?.transitionInterpolant = t;
            xixianFlaskUI.InventoryMenu.textAlpha = t;
        }
        public bool FullyClosed()
        {
            return !isOpen && timer <= 0;
            //    throw new NotImplementedException();
        }
        private void Close()
        {
            isOpen = false;

        }
    }
    #endregion
}
