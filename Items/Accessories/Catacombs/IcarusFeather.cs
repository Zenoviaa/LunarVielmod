using Stellamod.Buffs;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Catacombs
{
    public class IcarusFeather : ModItem
    {
        private int _counter;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //Infinite Flight but only when you run out
            if (player.wingTime <= 2 && player.controlJump && !player.HasBuff<Zuid>())
            {
                player.AddBuff(BuffID.OnFire, 2);
                player.lifeRegen -= 32;
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

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<RadiantNectar, BlankAccessory>();
        }

    }
}
