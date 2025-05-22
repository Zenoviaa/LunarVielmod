using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Dusts
{
	public class BubbleDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.scale = 1f;
			dust.alpha = 0;
			dust.velocity *= 0.1f;
			dust.frame = new Rectangle(0, 0, 14, 14);
		}

		public override bool Update(Dust dust)
		{
			Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), 0.2f, 0.2f, 0.2f);
			dust.noGravity = true;
			dust.position += dust.velocity;
			dust.velocity *= 0.92f;
			dust.scale *= 0.98f;
			dust.alpha += 12;
			if (dust.scale < 0.5f)
			{
				dust.active = false;
			}
			return false;
		}
	}
}
