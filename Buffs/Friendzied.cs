using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
	public class Friendzied : ModBuff
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

		private const int Particle_Count = 2;
		private const int Miracle_Soul_Cooldown = 60;
		

		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetAttackSpeed(DamageClass.Generic) += 0.50f;
			player.statDefense += 10;
			Dust.NewDustPerfect(new Vector2(player.position.X + Main.rand.Next(player.width), player.position.Y + player.height - Main.rand.Next(7)), DustID.SilverCoin, Vector2.Zero);
			for (int m = 0; m < 20; m++)
			{
				Vector2 position = new Vector2(player.position.X + Main.rand.Next(player.width), player.position.Y + player.height - Main.rand.Next(7));
				Particle p = ParticleManager.NewParticle(position, new Vector2(0, -2f), ParticleManager.NewInstance<VoidParticle>(),
					default(Color), Main.rand.NextFloat(0.2f, 1));
				p.layer = Particle.Layer.BeforePlayersBehindNPCs;
			}
		}

		
	}
}