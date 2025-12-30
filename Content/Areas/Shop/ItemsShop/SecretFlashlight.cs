using Stellamod.Core.Bases;
using Stellamod.Core.LunarLightingSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.ItemsShop
{
    public class SecretFlashlight : BaseLanternItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.buffType = ModContent.BuffType<HoldingMyFlashlight>();
            Item.shoot = ModContent.ProjectileType<SecretFlashlightProjectile>();
        }
    }

    public class SecretFlashlightProjectile : BaseLanternProjectile<HoldingMyFlashlight>
    {
        private ConeLight _coneLight;
        public override void SetDefaults()
        {
            base.SetDefaults();
            FlashlightLength = 1200;
            FlashlightWidth = 1200;
        }
        protected override ILight GetLight()
        {

            _coneLight ??= new ConeLight();
            return _coneLight;
        }
    }

    public class HoldingMyFlashlight : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<SecretFlashlightProjectile>());
        }
    }
}
