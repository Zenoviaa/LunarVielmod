using Stellamod.Buffs;
using Stellamod.Core.Bases;
using Stellamod.Core.LunarLightingSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Tools
{
    public class RadiantLantern : BaseLanternItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.buffType = ModContent.BuffType<RadiatingLantern>();
            Item.shoot = ModContent.ProjectileType<RadiantLanternProjectile>();
        }
    }

    public class RadiantLanternProjectile : BaseLanternProjectile<RadiatingLantern>
    {
        private ConeLight _coneLight;
        protected override ILight GetLight()
        {
            _coneLight ??= new ConeLight();
            return _coneLight;
        }
    }

    public class RadiatingLantern : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<RadiantLanternProjectile>());
        }
    }
}
