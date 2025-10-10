using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.TilesNew.Colosseum
{
    public class GintzeSpectatorItem1 : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<GintzeSpectator1>();
        }
    }

    public class GintzeSpectatorItem2 : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<GintzeSpectator2>();
        }
    }

    public class GintzeSpectatorItem3 : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<GintzeSpectator3>();
        }
    }

    public class GintzeSpectatorItem4 : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<GintzeSpectator4>();
        }
    }

    public class GintzeSpectatorItem5 : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<GintzeSpectator5>();
        }
    }

    public class GintzeSpectatorItem6 : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<GintzeSpectator6>();
        }
    }

    public class GintzeSpectatorItem7 : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<GintzeSpectator7>();
        }
    }

    public abstract class BaseGintzeSpectator : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
        }

        public override SpriteEffects GetSpriteEffects(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Vector2 worldPos = new Vector2(i*16, j*16);
            if (player.Center.X < worldPos.X)
            {
                return SpriteEffects.FlipHorizontally;
            }
            return SpriteEffects.None;
        }
    }

    public class GintzeSpectator1 : BaseGintzeSpectator
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //Number of frames in the animation
            FrameCount = 6;

            //How fast the animation is
            FrameSpeed = 10;

            //If this is set to true, wall tiles will offset their animation so they're not all synced
            DesyncAnimations = true;
        }
    }

    public class GintzeSpectator2 : BaseGintzeSpectator
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //Number of frames in the animation
            FrameCount = 4;

            //How fast the animation is
            FrameSpeed = 10;

            //If this is set to true, wall tiles will offset their animation so they're not all synced
            DesyncAnimations = true;
        }
    }

    public class GintzeSpectator3 : BaseGintzeSpectator
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //Number of frames in the animation
            FrameCount = 4;

            //How fast the animation is
            FrameSpeed = 10;

            //If this is set to true, wall tiles will offset their animation so they're not all synced
            DesyncAnimations = true;
        }
    }

    public class GintzeSpectator4 : BaseGintzeSpectator
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //Number of frames in the animation
            FrameCount = 10;

            //How fast the animation is
            FrameSpeed = 13;

            //If this is set to true, wall tiles will offset their animation so they're not all synced
            DesyncAnimations = true;
        }
    }

    public class GintzeSpectator5 : BaseGintzeSpectator
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //Number of frames in the animation
            FrameCount = 4;

            //How fast the animation is
            FrameSpeed = 10;

            //If this is set to true, wall tiles will offset their animation so they're not all synced
            DesyncAnimations = true;
        }
    }

    public class GintzeSpectator6 : BaseGintzeSpectator
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //Number of frames in the animation
            FrameCount = 7;

            //How fast the animation is
            FrameSpeed = 10;

            //If this is set to true, wall tiles will offset their animation so they're not all synced
            DesyncAnimations = true;
        }
    }
    public class GintzeSpectator7 : BaseGintzeSpectator
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //Number of frames in the animation
            FrameCount = 8;

            //How fast the animation is
            FrameSpeed = 10;

            //If this is set to true, wall tiles will offset their animation so they're not all synced
            DesyncAnimations = true;
        }
    }
}
