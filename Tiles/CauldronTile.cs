using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Placeable;
using Stellamod.UI.CauldronSystem;
using System;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Tiles
{
    internal class CauldronTile : ModTile
    {
        private float _lastTime;
        private int _frameCounter;
        private int _frameTick;
        public override LocalizedText DefaultContainerName(int frameX, int frameY)
        {
            int option = frameX / 36;
            return this.GetLocalization("MapEntry" + option);
        }

        public int timer = 0;
        public override void SetStaticDefaults()
        {
            DustType = ModContent.DustType<Sparkle>();
            AdjTiles = new int[] { TileID.Containers };

            LocalizedText name = CreateMapEntryName();
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

            DustType = ModContent.DustType<Sparkle>();
            DustType = ModContent.DustType<Dusts.SalfaceDust>();
            AdjTiles = new int[] { TileID.Bookcases };
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 7;
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16 };

            MineResist = 8f;
            MinPick = 200;
            TileObjectData.newTile.DrawYOffset = 6;

            Main.tileBlockLight[Type] = true;
            TileObjectData.addTile(Type);
            TileID.Sets.HasOutlines[Type] = false;
            TileID.Sets.DisableSmartCursor[Type] = true;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override bool CanExplode(int i, int j) => false;
        public override bool RightClick(int i, int j)
        {
            CauldronUISystem cauldronUISystem = ModContent.GetInstance<CauldronUISystem>();
            cauldronUISystem.ToggleUI();
            cauldronUISystem.CauldronPos = new Vector2(i * 16, j * 16);
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {

        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int left = i;
            int top = j;

            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<MagicCauldron>();
            if (tile.TileFrameX % 36 != 0)
            {
                left--;
            }

            if (tile.TileFrameY != 0)
            {
                top--;
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public override void MouseOverFar(int i, int j)
        {
            MouseOver(i, j);
            Player player = Main.LocalPlayer;
            if (player.cursorItemIconText == "")
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.2f;
            g = 0.165f;
            b = 0.12f;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Vector2 pos = new Vector2(i, j) * 16;
            Lighting.AddLight(pos, new Vector3(0.1f, 0.32f, 0.5f) * 0.35f);

            if (Main.rand.NextBool(100))
            {
                if (!Main.tile[i, j - 1].HasTile)
                {
                    Dust.NewDustPerfect(pos + new Vector2(Main.rand.NextFloat(0, 16), Main.rand.NextFloat(-32, -16)),
                        ModContent.DustType<Sparkle>(), new Vector2(Main.rand.NextFloat(-0.02f, 0.4f), -Main.rand.NextFloat(0.1f, 2f)), 0, new Color(0.05f, 0.08f, 0.2f, 0f), Main.rand.NextFloat(0.25f, 2f));
                }
            }
        }
    }
}
