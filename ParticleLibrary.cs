using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod
{
	/// <summary>
	/// </summary>
	public class ParticleLibrary : Mod
	{
		public static Texture2D emptyPixel;
		public override void Load()
		{
			ParticleManager.Load();
			emptyPixel = (Texture2D)ModContent.Request<Texture2D>("Stellamod/EmptyPixel");
		}
		public override void Unload()
		{
			ParticleManager.Unload();
			ParticleLibraryConfig.Instance = null;
		}
		
	}
}