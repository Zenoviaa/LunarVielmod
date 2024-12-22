using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Items.Discs
{
    internal abstract class BaseRecordTile : ModTile
    {
        public override string Texture => (GetType().Namespace + "." + "TileRecordPlayer").Replace('.', '/');
        public string DiscTexture => (GetType().Namespace + "." + Name).Replace('.', '/').Replace("Tile", "");

        public string DiscItem => Name.Replace("Tile", "");
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(191, 142, 111), Language.GetText("ItemName.MusicBox"));

            ModItem item = ModContent.Find<ModItem>("Stellamod/" + DiscItem);
            RegisterItemDrop(item.Type);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            base.PostDraw(i, j, spriteBatch);
            bool left = Framing.GetTileSafely(i - 1, j).TileType == Type;
            bool right = Framing.GetTileSafely(i + 1, j).TileType == Type;
            bool up = Framing.GetTileSafely(i, j - 1).TileType == Type;
            bool down = Framing.GetTileSafely(i, j + 1).TileType == Type;
            if (!up && !left)
            {

                Vector2 coords = new Vector2(i, j);
                coords += new Vector2(12);
                coords *= 16;
                Vector2 drawPos = coords - Main.screenPosition;
                drawPos += new Vector2(46 / 2, 0f);
                drawPos.X -= 7;
                drawPos.Y += VectorHelper.Osc(0f, -4f);
                drawPos.Y -= 20;
                Texture2D discTexture = ModContent.Request<Texture2D>(DiscTexture).Value;
                Color drawColor = Color.White;
                Color lightColor = Lighting.GetColor(i, j);
                drawColor = drawColor.MultiplyRGB(lightColor);


                spriteBatch.Restart(blendState: BlendState.Additive);

                for (float f = 0f; f < 1f; f += 0.2f)
                {
                    Color backGlowColor = drawColor * VectorHelper.Osc(0.5f, 1f);
                    float rot = f * MathHelper.TwoPi;
                    rot += Main.GlobalTimeWrappedHourly;
                    Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(2f, 4f);
                    spriteBatch.Draw(discTexture, drawPos + offset, null, backGlowColor, 0, discTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                }

                spriteBatch.RestartDefaults();

                spriteBatch.Draw(discTexture, drawPos, null, drawColor, 0, discTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                spriteBatch.Restart(blendState: BlendState.Additive);

                Color glowColor = drawColor * VectorHelper.Osc(0.15f, 0.75f);
                spriteBatch.Draw(discTexture, drawPos, null, glowColor, 0, discTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                spriteBatch.RestartDefaults();
            }

        }
    }
}
