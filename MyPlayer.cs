using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod
{
	public class MyPlayer : ModPlayer
	{

		public bool Bossdeath = false;
		public bool Boots = false;
		public bool doArmorNEWAbilityThing;
		public int extraSlots;





		public override void UpdateDead()
		{
			ResetStats();
		}
		public void ResetStats()
		{
			Bossdeath = false;
			Boots = false;



		}



		public override void PostUpdate()
		{
			//player.extraAccessorySlots = extraAccSlots; dont actually use, it'll fuck things up

			if (Boots)
			{
				if (Player.controlJump)
				{
					const float jumpSpeed = 6.01f;
					if (Player.gravDir == 1)
					{
						Player.velocity.Y -= Player.gravDir * 1f;
						if (Player.velocity.Y <= -jumpSpeed) Player.velocity.Y = -jumpSpeed;
						Dust.NewDust(new Vector2(Player.position.X, Player.position.Y + Player.height), Player.width, 0, ModContent.DustType<Dusts.Sparkle>());
					}
					else
					{
						Player.velocity.Y += Player.gravDir * 0.5f;
						if (Player.velocity.Y >= jumpSpeed) Player.velocity.Y = jumpSpeed;
					}
				}
			}
		}

		public const int CAMO_DELAY = 100;

		internal static bool swingingCheck;
		internal static Item swingingItem;

		public int Shake = 0;
	}


}
