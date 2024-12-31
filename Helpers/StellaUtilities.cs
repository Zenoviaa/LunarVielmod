using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Terraria.ID;

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

        public static Rectangle AnimationFrame(this Texture2D texture,
			ref int frame, 
			ref int frameTick, 
			int frameTime, int startFrame, int frameCount, int totalFrameCount, bool frameTickIncrease = true)
        {
            if (frameTick >= frameTime)
            {
                frameTick = -1;
                frame = frame == frameCount - 1 ? 0 : frame + 1;
            }
            if (frameTickIncrease)
                frameTick++;

            int height = texture.Height / frameCount;
			int heightPerFrame = texture.Height / totalFrameCount;
			int startY = startFrame * heightPerFrame;
            return new Rectangle(0, startY + ((height / frameCount) * frame), texture.Width, heightPerFrame);
        }

        public static Rectangle GetFrame(this Texture2D texture, int frameNumber, int totalFrameCount)
        {    
            int heightPerFrame = texture.Height / totalFrameCount;
            int startY = frameNumber * heightPerFrame;
            return new Rectangle(0, startY, texture.Width, heightPerFrame);
        }

        public static Rectangle GetFrame(this Texture2D texture, int frameNumber, int horizontalFrameCount, int verticalFrameCount)
        {
            int widthPerFrame = texture.Width / horizontalFrameCount;
            int heightPerFrame = texture.Height / verticalFrameCount;

            Rectangle rectangle = new Rectangle(0, 0, widthPerFrame, heightPerFrame);
            rectangle.X = ((int)frameNumber % horizontalFrameCount) * rectangle.Width;
            rectangle.Y = (((int)frameNumber - ((int)frameNumber % horizontalFrameCount)) / horizontalFrameCount) * rectangle.Height;
            return rectangle;
        }
    }
}
