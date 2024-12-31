using Terraria.ModLoader;

namespace Stellamod.UI.Systems
{
    // This is a class for functionality related to ExampleProjectileModifications.
    public class BLProjectileModificationGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public int timesHitByModifiedProjectiles;
	}
}