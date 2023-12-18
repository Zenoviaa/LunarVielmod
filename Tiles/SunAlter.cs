using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Consumables;
using Stellamod.NPCs.Bosses.SunStalker;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Tiles
{
    internal class SunAlter : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            MineResist = 4f;
            MinPick = 25;

            TileObjectData.addTile(Type);
            DustType = DustID.Stone;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Sun Alter");
            AddMapEntry(new Color(220, 126, 58));
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = .354f * 3;
            g = .200f * 3;
            b = .055f * 3;
        }

        public override bool CanExplode(int i, int j) => false;
        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {

            Tile tile = Main.tile[i, j];
            var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            int height = tile.TileFrameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/SunAlter_Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void MouseOver(int i, int j)
        {
            //shows the Cryptic Crystal icon while mousing over this tile
            Main.player[Main.myPlayer].cursorItemIconEnabled = true;
            Main.player[Main.myPlayer].cursorItemIconID = ModContent.ItemType<SunClaw>();
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 2);
            if (!tileBelow.HasTile || tileBelow.IsHalfBlock || tileBelow.TopSlope)
            {
                WorldGen.KillTile(i, j);
            }

            return true;
        }
        public override bool RightClick(int i, int j)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<SunStalkerPreSpawn>()) || NPC.AnyNPCs(ModContent.NPCType<SunStalkerPreSpawn>())) //Do nothing if the boss is alive
                return false;

            Player player = Main.LocalPlayer;










            if (!NPC.AnyNPCs(ModContent.NPCType<SunStalkerPreSpawn>()))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {

                    if (player.HasItem(ModContent.ItemType<SunClaw>()) && !NPC.AnyNPCs(ModContent.NPCType<SunStalker>()) && !NPC.AnyNPCs(ModContent.NPCType<SunStalkerPreSpawn>()))
                    {
                        Main.NewText("Sun Stalker has awoken!", Color.Gold);
                        var entitySource = player.GetSource_FromThis();
                        int SSSpawn = NPC.NewNPC(new Terraria.DataStructures.EntitySource_TileUpdate(i, j), i * 16 + Main.rand.Next(-10, 10), j * 16, ModContent.NPCType<SunStalkerPreSpawn>(), 0, 0, 0, 0, 0, Main.myPlayer);
                        Main.npc[SSSpawn].netUpdate = true;
                    }
                    if (!player.HasItem(ModContent.ItemType<SunClaw>()))
                    {
                        Main.NewText("Come back with a Sun Stone to fight the warrior of the desert.", Color.Gold);

                    }

                 
                }
                else
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        return false;

                    if (player.HasItem(ModContent.ItemType<SunClaw>()) && !NPC.AnyNPCs(ModContent.NPCType<SunStalker>()) && !NPC.AnyNPCs(ModContent.NPCType<SunStalkerPreSpawn>()))
                    {
                        Main.NewText("Sun Stalker has awoken!", Color.Gold);
                        var entitySource = player.GetSource_FromThis();
                        int SSSpawn = NPC.NewNPC(new Terraria.DataStructures.EntitySource_TileUpdate(i, j), i * 16 + Main.rand.Next(-10, 10), j * 16, ModContent.NPCType<SunStalkerPreSpawn>(), 0, 0, 0, 0, 0, Main.myPlayer);
                        Main.npc[SSSpawn].netUpdate = true;
                    }
                    if (!player.HasItem(ModContent.ItemType<SunClaw>()))
                    {
                        Main.NewText("Come back with a Sun Stone to fight the warrior of the desert.", Color.Gold);

                    }
                }

                return true;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<SunStalkerPreSpawn>()))
            {

                Main.NewText("...", Color.Gold);



            }





            return true;
        }
    }
}