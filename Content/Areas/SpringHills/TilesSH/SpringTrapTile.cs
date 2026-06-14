using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Dusts;
using Stellamod.Trails;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Content.Areas.SpringHills.TilesSH
{
    public class SpringArrow : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = false;
            Projectile.hostile = true;

        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 16 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 1f)).RotatedByRandom(19.0), 0, Color.LightPink, Main.rand.NextFloat(0.2f, 0.4f)).noGravity = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightPink, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.LoveTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() / 2;
            float drawRotation = Projectile.rotation;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = Projectile.scale;
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0);
            return false;

        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (float f = 0; f < 2; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.LightPink, Main.rand.NextFloat(0.3f, 0.5f)).noGravity = true;
            }
            for (int i = 0; i < 3; i++)
            {
                //Old velocity is the velocity before this tick, so it won't be zero or whatever
                Vector2 velocity = -Projectile.oldVelocity.RotatedByRandom(MathHelper.ToRadians(30)).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5, 15);
            }
        }
    }

    // This item shows off using 1 class to load multiple items. This is an alternate to typical inheritance.
    // Read the comments in this example carefully, as there are many parts necessary to make this approach work.
    // The real strength of this approach is when you have many items that vary by small changes, like how these 2 trap items vary only by placeStyle.
    public class SpringTrap : ModItem
    {

        public override void SetDefaults()
        {
            // With all the setup above, placeStyle will be either 0 or 1 for the 2 ExampleTrap instances we've loaded.
            Item.DefaultToPlaceableTile(ModContent.TileType<SpringTrapTile>());

            Item.width = 12;
            Item.height = 12;
            Item.value = 10000;
            Item.mech = true; // lets you see wires while holding.
        }
    }
    public class SpringTrapTileEntity : ModTileEntity
    {
        public float Timer;

        public override void Update()
        {
            base.Update();

            Timer++;
            int i = Position.X;
            int j = Position.Y;
            Tile tile = Main.tile[i, j];


            int style = tile.TileFrameY / 18;
            Vector2 spawnPosition;
            // This logic here corresponds to the orientation of the sprites in the spritesheet, change it if your tile is different in design.
            int horizontalDirection = -1;
            // Each trap style within this Tile shoots different projectiles.
            // Wiring.CheckMech checks if the wiring cooldown has been reached. Put a longer number here for less frequent projectile spawns. 200 is the dart/flame cooldown. Spear is 90, spiky ball is 300
            if (Timer >= 150)
            {
                Timer = 0;
                for (int x = 0; x < 4; x++)
                {
                    int y = j + x;
                    spawnPosition = new Vector2(i * 16 + 8, y * 16 + 9 + 0); // The extra numbers here help center the projectile spawn position if you need to.
                    if (horizontalDirection > 0)
                        spawnPosition.X += 72;
                    // In a real mod you should be spawning projectiles that are both hostile and friendly to do damage to both players and NPC, as Terraria traps do.
                    // Make sure to change velocity, projectile, damage, and knockback.
                    Projectile.NewProjectile(Wiring.GetProjectileSource(i, j), spawnPosition, new Vector2(horizontalDirection, 0) * 6f,
                        ModContent.ProjectileType<SpringArrow>(), 40, 2f, Main.myPlayer);
                }

            }
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Sync the entire multitile's area.  Modify "width" and "height" to the size of your multitile in tiles
                int width = 4;
                int height = 4;
                NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

                // Sync the placement of the tile entity with other clients
                // The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
                NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
                return -1;
            }

            // ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
            int placedEntity = Place(i, j);

            return placedEntity;
        }


        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            //The MyTile class is shown later
            return tile.HasTile && tile.TileType == ModContent.TileType<SpringTrapTile>();
        }
    }

    public class SpringTrapTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.DontDrawTileSliced[Type] = true;
            TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;

            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.Width = 4;

            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            // MyTileEntity refers to the tile entity mentioned in the previous section
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<SpringTrapTileEntity>().Hook_AfterPlacement, -1, 0, true);

            // This is required so the hook is actually called.
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);

            // These 2 AddMapEntry and GetMapOption show off multiple Map Entries per Tile. Delete GetMapOption and all but 1 of these for your own ModTile if you don't actually need it.
            AddMapEntry(new Color(21, 179, 192), Language.GetText("MapObject.Trap")); // localized text for "Trap"
        }


        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            base.KillMultiTile(i, j, frameX, frameY);
            // ModTileEntity.Kill() handles checking if the tile entity exists and destroying it if it does exist in the world for you
            // The tile coordinate parameters already refer to the top-left corner of the multitile
            ModContent.GetInstance<SpringTrapTileEntity>().Kill(i, j);
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;


        // PlaceInWorld is needed to facilitate styles and alternates since this tile doesn't use a TileObjectData. Placing left and right based on player direction is usually done in the TileObjectData, but the specifics of that don't work for how we want this tile to work. 
        public override void PlaceInWorld(int i, int j, Item item)
        {
            int style = Main.LocalPlayer.HeldItem.placeStyle;
            Tile tile = Main.tile[i, j];
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
            }
        }
    }

    public class SpringTrapRight : ModItem
    {

        public override void SetDefaults()
        {
            // With all the setup above, placeStyle will be either 0 or 1 for the 2 ExampleTrap instances we've loaded.
            Item.DefaultToPlaceableTile(ModContent.TileType<SpringTrapTileRight>());

            Item.width = 12;
            Item.height = 12;
            Item.value = 10000;
            Item.mech = true; // lets you see wires while holding.
        }
    }
    public class SpringTrapTileEntityRight : ModTileEntity
    {
        public float Timer;

        public override void Update()
        {
            base.Update();

            Timer++;
            int i = Position.X;
            int j = Position.Y;
            Tile tile = Main.tile[i, j];


            int style = tile.TileFrameY / 18;
            Vector2 spawnPosition;
            // This logic here corresponds to the orientation of the sprites in the spritesheet, change it if your tile is different in design.
            int horizontalDirection = 1;
            // Each trap style within this Tile shoots different projectiles.
            // Wiring.CheckMech checks if the wiring cooldown has been reached. Put a longer number here for less frequent projectile spawns. 200 is the dart/flame cooldown. Spear is 90, spiky ball is 300
            if (Timer >= 150)
            {
                Timer = 0;
                for (int x = 0; x < 4; x++)
                {
                    int y = j + x;
                    spawnPosition = new Vector2(i * 16 + 8, y * 16 + 9 + 0); // The extra numbers here help center the projectile spawn position if you need to.
                    if (horizontalDirection > 0)
                        spawnPosition.X += 72;
                    // In a real mod you should be spawning projectiles that are both hostile and friendly to do damage to both players and NPC, as Terraria traps do.
                    // Make sure to change velocity, projectile, damage, and knockback.
                    Projectile.NewProjectile(Wiring.GetProjectileSource(i, j), spawnPosition, new Vector2(horizontalDirection, 0) * 6f,
                        ModContent.ProjectileType<SpringArrow>(), 40, 2f, Main.myPlayer);
                }

            }
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Sync the entire multitile's area.  Modify "width" and "height" to the size of your multitile in tiles
                int width = 4;
                int height = 4;
                NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

                // Sync the placement of the tile entity with other clients
                // The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
                NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
                return -1;
            }

            // ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
            int placedEntity = Place(i, j);

            return placedEntity;
        }


        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            //The MyTile class is shown later
            return tile.HasTile && tile.TileType == ModContent.TileType<SpringTrapTileRight>();
        }
    }

    public class SpringTrapTileRight : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.DontDrawTileSliced[Type] = true;
            TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;

            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.Width = 4;

            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            // MyTileEntity refers to the tile entity mentioned in the previous section
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<SpringTrapTileEntityRight>().Hook_AfterPlacement, -1, 0, true);

            // This is required so the hook is actually called.
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);

            // These 2 AddMapEntry and GetMapOption show off multiple Map Entries per Tile. Delete GetMapOption and all but 1 of these for your own ModTile if you don't actually need it.
            AddMapEntry(new Color(21, 179, 192), Language.GetText("MapObject.Trap")); // localized text for "Trap"
        }


        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            base.KillMultiTile(i, j, frameX, frameY);
            // ModTileEntity.Kill() handles checking if the tile entity exists and destroying it if it does exist in the world for you
            // The tile coordinate parameters already refer to the top-left corner of the multitile
            ModContent.GetInstance<SpringTrapTileEntityRight>().Kill(i, j);
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;


        // PlaceInWorld is needed to facilitate styles and alternates since this tile doesn't use a TileObjectData. Placing left and right based on player direction is usually done in the TileObjectData, but the specifics of that don't work for how we want this tile to work. 
        public override void PlaceInWorld(int i, int j, Item item)
        {
            int style = Main.LocalPlayer.HeldItem.placeStyle;
            Tile tile = Main.tile[i, j];
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
            }
        }
    }
}
