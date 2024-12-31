using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Dusts
{
    public class RuneDust : ModDust
	{
        public override void OnSpawn(Dust dust)
        {

            Texture2D texture = ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            dust.noGravity = true;
            dust.frame = new Rectangle(0, texture.Height / 5 * Main.rand.Next(5), texture.Width, texture.Height / 5);
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity *= 0.95f;
            Lighting.AddLight(dust.position, Color.DarkRed.ToVector3() * 1f * Main.essScale);

            dust.scale *= 0.99f;
			if (dust.scale < 0.2f) {
				dust.active = false;
			}
			return false;
		}
	}
}
