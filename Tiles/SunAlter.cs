using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
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
            Player player = Main.LocalPlayer;
            if (!NPC.AnyNPCs(ModContent.NPCType<SunStalkerPreSpawn>()) && 
                !NPC.AnyNPCs(ModContent.NPCType<SunStalker>()))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Main.NewText(LangText.Misc("SunAlter.2"), Color.Gold);
                    int npcID = NPC.NewNPC(new Terraria.DataStructures.EntitySource_TileUpdate(i, j), i * 16 + Main.rand.Next(-10, 10), j * 16, ModContent.NPCType<SunStalkerPreSpawn>(), 0, 0, 0, 0, 0, Main.myPlayer);
                    Main.npc[npcID].netUpdate2 = true;
                }
                else
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        return false;

                    StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, 
                        ModContent.NPCType<SunStalkerPreSpawn>(), i * 16, (j * 16) - 5);
                }

                return true;
            }
            else
            {
                Main.NewText("...", Color.Gold);
            }

            return true;
        }
    }
}