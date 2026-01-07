using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT.Elevator
{
    public class PunkerElevatorLeverItem : ModItem
    {
        public override void SetDefaults()
        {
            // With all the setup above, placeStyle will be either 0 or 1 for the 2 ExampleTrap instances we've loaded.
            Item.DefaultToPlaceableTile(ModContent.TileType<PunkerElevatorLeverTile>());
            Item.width = 12;
            Item.height = 12;
            Item.value = 10000;
        }
    }
    public class PunkerElevatorLever : ModNPC,
        IDrawOutlines
    {
        private float _armRotation;
        private enum AIState
        {
            Idle,
            Bobble
        }
        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 48;
            NPC.height = 48;
            NPC.lifeMax = 200;
            NPC.damage = 1;
            NPC.defense = 1000;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.dontCountMe = true;
            NPC.knockBackResist = 0f;
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
        public override bool CheckActive()
        {
            return base.CheckActive();
        }
        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Bobble:
                    AI_Bobble();
                    break;
            }
        }

        private void AI_Idle()
        {

        }
        private void AI_Bobble()
        {
            Timer++;
            if(Timer == 1)
            {
                SendOutElevatorSignal();
            }

            float ratio = Timer / 100f;
            float ease = MathHelper.Lerp(1f, 0f, ratio);
            float osc = MathF.Sin(Timer * 0.5f) * 0.5f + 0.5f;
            float range = MathHelper.Lerp(-25, 25, osc);
            float radians = MathHelper.ToRadians(range) * ease;
            _armRotation = radians;
            if(Timer >= 100f)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void SendOutElevatorSignal()
        {

            int numDust = 2;
            for(int i = 0; i < numDust; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(4, 4);
                vel.Y -= 8;
                DustParticle.Spawn(NPC.Center - new Vector2(0, 16), vel, default);
            }

            //Find the nearest call 
            Point tilePoint = NPC.position.ToTileCoordinates();
            int xRange = 6;
            int yRange = 32;
            NPC elevatorNPC = null;
            for(int i = -xRange; i <= xRange; i++)
            {
                for(int j = -yRange; j <= yRange; j++)
                {
                    Point offsetTile = tilePoint + new Point(i, j);
                    if (!WorldGen.InWorld(offsetTile.X, offsetTile.Y))
                        continue;

                    int callTileType = ModContent.TileType<PunkerElevatorCallTile>();
                    Tile tile = Main.tile[offsetTile];
                    if(tile.HasTile && tile.TileType == callTileType)
                    {
                        //Found call tile
                        elevatorNPC = PunkerElevatorCallTile.GetElevator(offsetTile.X, offsetTile.Y);
                    }
                }
            }

            if (elevatorNPC == null)
                return;
            elevatorNPC.ai[3] = 3;
            elevatorNPC.netUpdate = true;
        }

        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                State = state;
                Timer = 0;
                NPC.netUpdate = true;
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            SwitchState(AIState.Bobble);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawArm(spriteBatch, screenPos, drawColor);
            Texture2D leverBaseTexture = TextureAssets.Npc[Type].Value;
            Vector2 origin = leverBaseTexture.Size() / 2f;
            screenPos.Y -= 22;
            spriteBatch.Draw(leverBaseTexture, NPC.Center - screenPos, null, drawColor, 0, origin, 1, SpriteEffects.None, 0);
            return false;
        }
        public override void OnKill()
        {
            base.OnKill();
        }

        private void DrawArm(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D leverlArmTexture = ModContent.Request<Texture2D>(Texture + "_Arm").Value;
            Vector2 drawOrigin = new Vector2(leverlArmTexture.Width / 2f, leverlArmTexture.Height);
            screenPos.Y -= 8;
            spriteBatch.Draw(leverlArmTexture, NPC.Center - screenPos, null, lightColor, _armRotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
        }
        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Color outlineColor = Color.Lerp(Color.Transparent, Color.White, (int)ExtraMath.Osc(0f, 2f, speed: 3));
         
            Vector2 v = Vector2.UnitX * 2;
            Vector2 h = Vector2.UnitY * 2;
            DrawArm(spriteBatch, screenPos + v, outlineColor);
            DrawArm(spriteBatch, screenPos - v, outlineColor);
            DrawArm(spriteBatch, screenPos + h, outlineColor);
            DrawArm(spriteBatch, screenPos - h, outlineColor);
        }
    }

    public class PunkerElevatorLeverTile : ModTile
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<PunkerElevatorLeverTileEntity>().Hook_AfterPlacement, -1, 0, true);

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

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
            ModContent.GetInstance<PunkerElevatorLeverTileEntity>().Kill(i, j);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            base.KillMultiTile(i, j, frameX, frameY);
            // ModTileEntity.Kill() handles checking if the tile entity exists and destroying it if it does exist in the world for you
            // The tile coordinate parameters already refer to the top-left corner of the multitile
            ModContent.GetInstance<PunkerElevatorLeverTileEntity>().Kill(i, j);
        }


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

    public class PunkerElevatorLeverTileEntity : ModTileEntity
    {
        public override void Update()
        {
            base.Update();
            //We don't need to check for an elevator every frame
            //That's completely unnecessary
            if (Main.GameUpdateCount % 60 != 0)
                return;

            Vector2 worldPosition = Position.ToWorldCoordinates();
            Vector2 spawnCenter = worldPosition;
            if (AlreadyHasAttachedElevator(worldPosition))
                return;

            //If we make it down here we need to spawn an elevator    
            int spawnX = (int)spawnCenter.X;
            int spawnY = (int)spawnCenter.Y;
            NPC.NewNPC(new EntitySource_TileBreak(Position.X, Position.Y), spawnX, spawnY, ModContent.NPCType<PunkerElevatorLever>());
        }

        private bool AlreadyHasAttachedElevator(Vector2 position)
        {
            //Look above and below the tile entity, checking for every npc for the punker elevator
            //If one doesn't exist, we spawn one here
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.type != ModContent.NPCType<PunkerElevatorLever>())
                    continue;
                float distance = Vector2.Distance(position, npc.Center);
                if (distance <= 64)
                    return true;
            }
            return false;
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
            return tile.HasTile && tile.TileType == ModContent.TileType<PunkerElevatorLeverTile>();
        }
    }
}
