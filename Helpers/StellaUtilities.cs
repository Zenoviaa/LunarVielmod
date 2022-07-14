﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Helpers
{
	public static class StellaUtilities
	{
		// Drawing
		public static Rectangle AnimationFrame(this Texture2D texture, ref int frame, ref int frameTick, int frameTime, int frameCount, Rectangle area, bool frameTickIncrease = true)
		{
			if (frameTick >= frameTime)
			{
				frameTick = -1;
				frame = frame == frameCount - 1 ? 0 : frame + 1;
			}
			if (frameTickIncrease)
				frameTick++;

			int height = texture.Height;
			if (area != Rectangle.Empty)
				height = area.Height;

			return new Rectangle(0, area.Y + ((height / frameCount) * frame), texture.Width, height / frameCount);
		}
	}
}