using Stellamod.Content.Special.DeadRomancesExcalibur;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.SwingSystem
{
    public class SwingPlayerV2 : ModPlayer
    {
        private int _comboWaitTimer;
        public int ComboCounter;
        public int comboWaitTime;
        public int ComboDirection = 1;
        public int Stamina;
        public int StaminaComboCounter;
        public int MaxStamina = 3;
        public bool InfiniteStamina;
        public bool unlockedFlask;
        public bool useStaminaThisFrame;
        public int MaxCombo;
        public int OldHeldItem;
        public bool isSwinging;
        public override void ResetEffects()
        {
            base.ResetEffects();
            isSwinging = false;
            comboWaitTime = 120;
            MaxStamina = 3;
            useStaminaThisFrame = false;
        }

        public override void UpdateDead()
        {
            base.UpdateDead();
            Stamina = MaxStamina;
        }
        public override void PreUpdate()
        {
            base.PreUpdate();
            if(MaxCombo > 0)
                ComboCounter %= MaxCombo;

        }

        public override void PostUpdate()
        {
            base.PostUpdate();
            if(OldHeldItem != Player.HeldItem.type)
            {
                ComboCounter = 0;
                OldHeldItem = Player.HeldItem.type;
            }

            if (Player.itemTime > 0)
            {
                _comboWaitTimer = 0;
                return;
            }

            _comboWaitTimer++;
            if (_comboWaitTimer >= comboWaitTime)
            {
                Player.GetModPlayer<DeadRomancePlayer>().attackSpeedStacks = 0;
                ResetCombo();
                //Main.NewText("Reset");
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
        public void IncreaseCombo(int maxCombo)
        {
            IncreaseCombo();
            ComboCounter %= maxCombo;
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
            ComboDirection = 1;
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
