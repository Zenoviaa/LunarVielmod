using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
	public class VialedUp : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Charm Buff!");
			// Description.SetDefault("10+ Defense and Golden trail oooo :0");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}
		private int _miracleSoulCooldown;
		public int miracleLevel;
		public int miracleTimeLeft;
		public bool hasMiracleSet;

	



		public override void Update(Player player, ref int buffIndex)
		{
			player.maxFallSpeed *= 3;
			player.noFallDmg = true;
			player.jumpBoost = true;
			player.moveSpeed += 0.4f;
			player.maxRunSpeed += 0.4f;
			
			for (int m = 0; m < 5; m++)
			{
				Vector2 position = new Vector2(player.position.X + Main.rand.Next(player.width), player.position.Y + player.height - Main.rand.Next(30));
				Particle p = ParticleManager.NewParticle(position, new Vector2(0, -2f), ParticleManager.NewInstance<morrowstar>(),
					default(Color), Main.rand.NextFloat(0.2f, 1));
				p.layer = Particle.Layer.BeforePlayersBehindNPCs;
			}
		}


	}
}