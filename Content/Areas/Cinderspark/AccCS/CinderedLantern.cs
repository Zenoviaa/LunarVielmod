using Stellamod.Core.Bases;
using Stellamod.Core.LunarLightingSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS
{
    public class CinderedLantern : BaseLanternItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.buffType = ModContent.BuffType<CinderediatingLantern>();
            Item.shoot = ModContent.ProjectileType<CinderedLanternProjectile>();
        }
    }

    public class CinderedLanternProjectile : BaseLanternProjectile<CinderediatingLantern>
    {
        private ConeLight _coneLight;
        public override void SetDefaults()
        {
            base.SetDefaults();
            FlashlightDegrees = 30;
            FlashlightLength = 800;
            FlashlightWidth = 860;
        }
        protected override ILight GetLight()
        {
            _coneLight ??= new ConeLight();
            return _coneLight;
        }
    }

    public class CinderediatingLantern : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<CinderedLanternProjectile>());
        }
    }
}
