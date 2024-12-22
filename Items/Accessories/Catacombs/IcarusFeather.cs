using Stellamod.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Catacombs
{
    internal class IcarusFeather : ModItem
    {
        private int _counter;
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //Infinite Flight but only when you run out
            if (player.wingTime <= 2 && player.controlJump && !player.HasBuff<Zuid>())
            {
                player.AddBuff(BuffID.OnFire, 2);
                player.wingTime = 2;
                player.wingRunAccelerationMult /= 2;
                player.runAcceleration /= 2;
                player.jumpSpeedBoost /= 2;
                player.maxRunSpeed /= 2;


                _counter++;
                if (_counter % 2 == 0)
                {

                }
                if (_counter % 8 == 0)
                {
                    SoundEngine.PlaySound(SoundID.LiquidsWaterLava, player.position);
                }
            }
        }
    }
}
