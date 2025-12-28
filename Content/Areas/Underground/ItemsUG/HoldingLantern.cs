using Stellamod.Core.Bases;
using Stellamod.Core.LunarLightingSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.ItemsUG
{
    public class HoldingLantern : BaseLanternItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.buffType = ModContent.BuffType<HoldingMyLantern>();
            Item.shoot = ModContent.ProjectileType<HoldingLanternProjectile>();
        }
    }

    public class HoldingLanternProjectile : BaseLanternProjectile<HoldingMyLantern>
    {
        private PointLight _light;
        protected override ILight GetLight()
        {
            _light ??= new PointLight();
            return _light;
        }
    }

    public class HoldingMyLantern : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<HoldingLanternProjectile>());
        }
    }
}
