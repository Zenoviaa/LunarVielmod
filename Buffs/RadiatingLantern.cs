using Stellamod.Projectiles.Lanterns;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    internal class RadiatingLantern : ModBuff
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
