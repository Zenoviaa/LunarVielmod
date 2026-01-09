using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.SwingSystem
{
    public class SwingPlayerV2 : ModPlayer
    {
        private int _comboWaitTimer;
        public int ComboCounter;
        public int ComboWaitTime;
        public int ComboDirection = 1;
        public int Stamina;
        public int StaminaComboCounter;
        public int MaxStamina = 3;
        public bool InfiniteStamina;
        public bool unlockedFlask;
        public bool useStaminaThisFrame;
        public override void ResetEffects()
        {
            base.ResetEffects();
            MaxStamina = 3;
            useStaminaThisFrame = false;
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
        public void ResetProgress()
        {
            unlockedFlask = false;
        }


        public bool CanUseStamina(int amountToUse)
        {
            if (InfiniteStamina)
                return true;
            return Stamina >= amountToUse;
        }

        public void ConsumeStamina(int amountToUse)
        {
            if (InfiniteStamina)
                return;
            useStaminaThisFrame = true;
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

        public void ResetCombo()
        {
            StaminaComboCounter = 0;
            ComboCounter = 0;
            _comboWaitTimer = 0;
        }
        public bool HasUnlockedFlask()
        {
            return unlockedFlask;
        }

        public void UnlockFlask()
        {
            if (unlockedFlask)
                return;

            unlockedFlask = true;
        }
        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["unlockedFlask"] = unlockedFlask;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            unlockedFlask = tag.GetBool("unlockedFlask");
        }
    }
}
