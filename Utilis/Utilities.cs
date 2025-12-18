using Stellamod.Assets.Biomes;
using Stellamod.Content.Areas.SpringHills;
using Terraria;
using Terraria.DataStructures;

namespace Stellamod.Utilis
{
    public static class Utilities
    {
        public static int NewProjectileBetter(float spawnX, float spawnY, float velocityX, float velocityY, int type, int damage, float knockback, int owner = -1, float ai0 = 0f, float ai1 = 0f)
        {
            if (owner == -1)
                owner = Main.myPlayer;
            damage = (int)(damage * 0.5);
            if (Main.expertMode)
                damage = (int)(damage * 0.5);
            int index = Projectile.NewProjectile(new EntitySource_WorldEvent(), spawnX, spawnY, velocityX, velocityY, type, damage, knockback, owner, ai0, ai1);
            if (index >= 0 && index < Main.maxProjectiles)
            {
                Main.projectile[index].netUpdate = true;
            }

            return index;
        }
    }

    public static class ExtensionMethods
    {
        public static bool ZoneFable(this Player player) => player.InModBiome<FableBiome>();
        public static bool ZoneAbyss(this Player player) => player.InModBiome<AbyssBiome>();
        public static bool ZoneAcid(this Player player) => player.InModBiome<AcidBiome>();
        public static bool ZoneXixianVillage(this Player player) => player.InModBiome<XixVillageBiome>();
    }
}
