using System;
using Terraria.ModLoader;

namespace Stellamod.Common.SwingSystem
{
    internal class SwingPlayer : ModPlayer
    {
        private int _comboWaitTimer;
        public int ComboCounter;
        public int ComboWaitTime;
        public int ComboDirection = 1;
        public int Stamina;
        public int StaminaComboCounter;
        public int MaxStamina = 3;
        public override void ResetEffects()
        {
            base.ResetEffects();
            MaxStamina = 3;
        }

        public override void UpdateDead()
        {
            base.UpdateDead();
            Stamina = MaxStamina;
        }

        public override void PostUpdate()
        {
            base.PostUpdate();
            _comboWaitTimer++;
            if (_comboWaitTimer >= ComboWaitTime)
            {
                ResetCombo();
            }
        }

        public bool CanUseStamina(int amountToUse)
        {
            return Stamina >= amountToUse;
        }

        public void ConsumeStamina(int amountToUse)
        {
            Stamina -= amountToUse;
        }

        public void IncreaseCombo()
        {
            _comboWaitTimer = 0;
            ComboCounter++;
            ComboDirection = -ComboDirection;
        }

        public void IncreaseStaminaCombo(int maxStaminaCombo)
        {
            _comboWaitTimer = 0;
            StaminaComboCounter++;
            if (StaminaComboCounter >= maxStaminaCombo)
            {
                StaminaComboCounter = 0;
            }
            ComboDirection = -ComboDirection;
        }

        internal void ResetCombo()
        {
            StaminaComboCounter = 0;
            ComboCounter = 0;
            _comboWaitTimer = 0;
        }
    }
}
