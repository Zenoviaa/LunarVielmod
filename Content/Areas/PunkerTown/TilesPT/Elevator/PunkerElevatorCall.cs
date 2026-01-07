using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.WorldG;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT.Elevator
{

    public class PunkerElevatorCall : ModItem
    {
        public override void SetDefaults()
        {
            // With all the setup above, placeStyle will be either 0 or 1 for the 2 ExampleTrap instances we've loaded.
            Item.DefaultToPlaceableTile(ModContent.TileType<PunkerElevatorCallTile>());
            Item.width = 12;
            Item.height = 12;
            Item.value = 10000;
        }
    }
    public class PunkerElevatorCallTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            // MyTileEntity refers to the tile entity mentioned in the previous section
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<PunkerElevatorCallTileEntity>().Hook_AfterPlacement, -1, 0, true);

            // This is required so the hook is actually called.
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            // These 2 AddMapEntry and GetMapOption show off multiple Map Entries per Tile. Delete GetMapOption and all but 1 of these for your own ModTile if you don't actually need it.
            AddMapEntry(new Color(21, 179, 192), Language.GetText("MapObject.Trap")); // localized text for "Trap"
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {

            return false;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            base.PostDraw(i, j, spriteBatch);
            Texture2D texture = TextureAssets.Tile[Type].Value;
            Vector2 worldCoordinates = (new Vector2(i, j - 1) + VeilGen.TileAdj) * 16;
           // worldCoordinates.X;
            Vector2 drawPosition = worldCoordinates - Main.screenPosition;
            spriteBatch.Draw(texture, drawPosition, null, Lighting.GetColor(i, j).MultiplyRGB(Color.DarkGray), 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
            ModContent.GetInstance<PunkerElevatorCallTileEntity>().Kill(i, j);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            base.KillMultiTile(i, j, frameX, frameY);
            // ModTileEntity.Kill() handles checking if the tile entity exists and destroying it if it does exist in the world for you
            // The tile coordinate parameters already refer to the top-left corner of the multitile
            ModContent.GetInstance<PunkerElevatorCallTileEntity>().Kill(i, j);
        }

        /// <summary>
        /// Returns the attached elevator for a tile position, if any, returns null if there are none
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static NPC GetElevator(int i, int j)
        {
            int xRange = 80;
            Vector2 worldPosition = new Point(i, j).ToWorldCoordinates();
            float minX = worldPosition.X;
            float maxX = minX + xRange;
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.type != ModContent.NPCType<PunkerElevator>())
                    continue;

                float x = npc.Center.X;
                if (x >= minX && x <= maxX)
                {
                    //Found our elevator
                    //So we can just return out the rest of the code
                    return npc;
                }
            }
            return null;
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

    public class PunkerElevatorCallTileEntity : ModTileEntity
    {
        public override void Update()
        {
            base.Update();
            //We don't need to check for an elevator every frame
            //That's completely unnecessary
            if (Main.GameUpdateCount % 60 != 0)
                return;

            int xRange = 80;
            int halfXRange = xRange / 2;
            Point point = Position.ToPoint();
            point.X++;
            point.Y++;
            Vector2 worldPosition = point.ToWorldCoordinates();
            float minX = worldPosition.X;
            float maxX = minX + xRange;
            if (AlreadyHasAttachedElevator(minX, maxX))
                return;

            //If we make it down here we need to spawn an elevator
            Vector2 spawnCenter = worldPosition;
            spawnCenter.X += xRange / 2;
            spawnCenter.Y = worldPosition.Y;

            Vector2 upCenter = spawnCenter - Vector2.UnitY * 8;
            Vector2 downCenter = spawnCenter + Vector2.UnitY * 8;

            bool shouldSpawnUp = Collision.CanHitLine(spawnCenter, 1, 1, upCenter, 1, 1);
            bool shouldSpawnDown = Collision.CanHitLine(spawnCenter, 1, 1, downCenter, 1, 1);
            if (shouldSpawnUp)
            {
                int spawnX = (int)upCenter.X;
                int spawnY = (int)upCenter.Y;
                NPC.NewNPC(new EntitySource_TileBreak(Position.X, Position.Y), spawnX, spawnY, ModContent.NPCType<PunkerElevator>());
            }
            else if (shouldSpawnDown)
            {
                int spawnX = (int)downCenter.X;
                int spawnY = (int)downCenter.Y;
                NPC.NewNPC(new EntitySource_TileBreak(Position.X, Position.Y), spawnX, spawnY, ModContent.NPCType<PunkerElevator>());
            }
        }

        private bool AlreadyHasAttachedElevator(float minX, float maxX)
        {
            //Look above and below the tile entity, checking for every npc for the punker elevator
            //If one doesn't exist, we spawn one here
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.type != ModContent.NPCType<PunkerElevator>())
                    continue;

                float x = npc.Center.X;
                if (x >= minX && x <= maxX)
                {
                    //Found our elevator
                    //So we can just return out the rest of the code
                    return true;
                }
            }
            return false;
        }
        private void SummonPunkerElevator()
        {

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
            return tile.HasTile && tile.TileType == ModContent.TileType<PunkerElevatorCallTile>();
        }
    }
}
