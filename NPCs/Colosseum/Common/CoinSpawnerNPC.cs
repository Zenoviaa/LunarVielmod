using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum.Common
{
    internal class CoinSpawnerNPC : ModNPC
    {
        private ref float Timer => ref NPC.ai[0];
        private ref float CoinsToSpawn => ref NPC.ai[1];
        private ref float CoinTimer => ref NPC.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 1;
            NPC.height = 1;
            NPC.lifeMax = 1;
            NPC.defense = 1;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer >= 2)
            {
                Timer = 0;
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 spawnPos = Vector2.Zero;
                    spawnPos.Y = NPC.Center.Y;
                    spawnPos.X = NPC.Center.X;

                    Point tileOffset = new Point(Main.rand.Next(-40, 40), 0);
                    Vector2 tileCoordinates = tileOffset.ToWorldCoordinates();
                    spawnPos.X += tileCoordinates.X;
                    spawnPos.Y -= Main.rand.Next(0, 40);

                    int itemIndex = Item.NewItem(NPC.GetSource_FromThis(), spawnPos, ItemID.SilverCoin, 1);
                    Item item = Main.item[itemIndex];
                    item.velocity = -Vector2.UnitY * 15;
                    item.velocity.X = Main.rand.NextFloat(-3f, 3f);
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);
                }
                CoinTimer++;
                if (CoinTimer >= CoinsToSpawn)
                {
                    NPC.Kill();
                }
            }
        }
    }
}
