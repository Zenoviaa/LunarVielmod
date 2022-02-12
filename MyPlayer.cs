using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace Stellamod
{
	public class MyPlayer : ModPlayer
	{
		public const int CAMO_DELAY = 100;

		internal static bool swingingCheck;
		internal static Item swingingItem;

		public int Shake = 0;
	}
}
