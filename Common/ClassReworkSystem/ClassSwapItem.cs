using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.ClassReworkSystem
{
    public class ClassSwapItem : ModItem
    {
        private int _useIndex;
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 0.9f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Balls");
        }

        public override bool? UseItem(Player player)
        {

            _useIndex++;
            if(_useIndex >= 6)
            {
                _useIndex = 0;
            }
            PlayerClass c = (PlayerClass)_useIndex;
            ClassReworkPlayer classReworkPlayer =player.GetModPlayer<ClassReworkPlayer>();
            classReworkPlayer.playerClass = c;
            Main.NewText($"You are {classReworkPlayer.playerClass}");
            return true;
        }
    }
}
