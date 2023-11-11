using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Verlia.Projectiles
{
    public class Sword2F : ModProjectile
	{
		public int timer = 0;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Nothingness");

		}
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.hostile = true;
		}
		public override void AI()
		{
			Projectile.velocity *= 0.98f;
			var entitySource = Projectile.GetSource_FromAI();
			timer++;
			
			if (timer == 5)
			{



				
					int index = NPC.NewNPC(entitySource, (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<Sword2>());
					NPC minionNPC = Main.npc[index];

					// Now that the minion is spawned, we need to prepare it with data that is necessary for it to work
					// This is not required usually if you simply spawn NPCs, but because the minion is tied to the body, we need to pass this information to it



					// Finally, syncing, only sync on server and if the NPC actually exists (Main.maxNPCs is the index of a dummy NPC, there is no point syncing it)
					if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
					{
						NetMessage.SendData(MessageID.SyncNPC, number: index);
					}


				
			}
			if (timer == 70)
			{
				Projectile.Kill();

			}

		}
		
		
	}
}