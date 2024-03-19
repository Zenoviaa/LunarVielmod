using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage.Stein
{
	public class SteinPlayer : ModPlayer
	{
		int Dd = 0;
		public bool HasHitDance;

		// Always reset the accessory field to its default value here.


		// Vanilla applies immunity time before this method and after PreHurt and Hurt
		// Therefore, we should apply our immunity time increment here
	
		public override void PostUpdate()
		{
			if (HasHitDance)
			{
				Dd++;

				if (Dd < 60)
				{
					float rot = Player.velocity.ToRotation();
					float spread = 0.8f;

					Vector2 offset = new Vector2(1.5f, -0.1f * Player.direction).RotatedBy(rot);



					for (int k = 0; k < 3; k++)
					{
						Vector2 direction = offset.RotatedByRandom(spread);

						Dust.NewDustPerfect(Player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, Color.IndianRed * 0.5f, Main.rand.NextFloat(0.5f, 1));
					}


					Player.immune = true;
					Player.fullRotation += 0.2f;

					Player.fullRotationOrigin = new Vector2(Player.width / 2, Player.height / 2);


					Player.immuneTime = 6;


				}
				if (Dd > 60)
				{
					HasHitDance = false;
					Player.fullRotation = 0;
					Dd = 0;
				}

			}
			

		}
	}
}