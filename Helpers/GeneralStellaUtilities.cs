using Terraria;
using Terraria.DataStructures;

namespace Stellamod.Helpers
{
    public static class GeneralStellaUtilities
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
}
