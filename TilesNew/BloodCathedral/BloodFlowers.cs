using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.TilesNew.BloodCathedral
{
    //Wall Version
    public class BloodFlower1Item : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<BloodFlower1>();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Tiles/ExampleDecorativeWallItem").Value;
            Vector2 drawOrigin = Vector2.Zero;
            Vector2 drawPosition = position + drawOrigin;
            spriteBatch.Draw(iconTexture, drawPosition, null, drawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0);
        }

    }

    internal class BloodFlower1 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;
           
            //If you need other static defaults it go here
        }
    }


    public class BloodFlower2Item : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<BloodFlower2>();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Tiles/ExampleDecorativeWallItem").Value;
            Vector2 drawOrigin = Vector2.Zero;
            Vector2 drawPosition = position + drawOrigin;
            spriteBatch.Draw(iconTexture, drawPosition, null, drawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0);
        }

    }

    internal class BloodFlower2 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;
            
            //If you need other static defaults it go here
        }
    }

    public class BloodFlower3Item : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<BloodFlower3>();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Tiles/ExampleDecorativeWallItem").Value;
            Vector2 drawOrigin = Vector2.Zero;
            Vector2 drawPosition = position + drawOrigin;
            spriteBatch.Draw(iconTexture, drawPosition, null, drawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0);
        }

    }

    internal class BloodFlower3 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;
           
            //If you need other static defaults it go here
        }
    }

    public class BloodFlower4Item : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<BloodFlower4>();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Tiles/ExampleDecorativeWallItem").Value;
            Vector2 drawOrigin = Vector2.Zero;
            Vector2 drawPosition = position + drawOrigin;
            spriteBatch.Draw(iconTexture, drawPosition, null, drawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0);
        }

    }

    internal class BloodFlower4 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;
           
            //If you need other static defaults it go here
        }
    }

    public class BloodFlower5Item : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<BloodFlower5>();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Tiles/ExampleDecorativeWallItem").Value;
            Vector2 drawOrigin = Vector2.Zero;
            Vector2 drawPosition = position + drawOrigin;
            spriteBatch.Draw(iconTexture, drawPosition, null, drawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0);
        }

    }

    internal class BloodFlower5 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;
        
            //If you need other static defaults it go here
        }
    }
}
