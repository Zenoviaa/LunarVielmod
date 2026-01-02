using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.PunkerTown.TilesPT;
using Stellamod.Content.Biomes;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.EnemiesPT
{
    public class Beehive : ModNPC
    {
        private float _inTimer;
        private int SpawnCount;
        private bool KindaHomeless;
        private bool Fall;
        private ref float Timer => ref NPC.ai[0];
        private int HomeTileX
        {
            get
            {
                return (int)NPC.ai[1];
            }
            set
            {
                NPC.ai[1] = value; 
            }
        }
        private int HomeTileY
        {
            get
            {
                return (int)NPC.ai[2];
            }
            set
            {
                NPC.ai[2] = value;
            }
        }

        private Vector2 HomeOffset;
        private Tile HomeTile => Main.tile[HomeTileX, HomeTileY];
        private Vector2 HomeTilePosition => new Point(HomeTileX, HomeTileY).ToWorldCoordinates();
        private bool Mad
        {
            get
            {
                return NPC.ai[3] == 1;
            }
            set
            {
                NPC.ai[3] = value ? 1 : 0;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(HomeOffset);
            writer.Write(KindaHomeless);
            writer.Write(Fall);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            HomeOffset = reader.ReadVector2();
            KindaHomeless = reader.ReadBoolean();
            Fall = reader.ReadBoolean();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 64;
            NPC.height = 80;
            NPC.lifeMax = 150;
            NPC.defense = 4;
            NPC.damage = 1;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit15;
            NPC.DeathSound = SoundID.NPCDeath11;
            NPC.dontTakeDamageFromHostiles = true;
        
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        private void FindHome()
        {
            if (!MultiplayerHelper.IsHost)
                return;

            if (HomeTileX == 0 || HomeTileY == 0)
            {
                //Find nearest mangrove tree
                //If none then fall to the ground and die
                Point tilePoint = NPC.position.ToTileCoordinates();

                int searchWidth = 32;
                int searchHeight = 32;

                tilePoint.X -= searchWidth / 2;
                tilePoint.Y -= searchHeight / 2;



                bool found = false;
                Point endPoint = tilePoint + new Point(searchWidth, searchHeight);
                for (int x = tilePoint.X; x < endPoint.X; x++)
                {
                    for (int y = tilePoint.Y; y < endPoint.Y; y++)
                    {
                        if (!WorldGen.InWorld(x, y))
                            continue;

                        Tile tile = Main.tile[x, y];
                        if (tile.TileType == ModContent.TileType<MangroveTreeTop>() || tile.TileType == ModContent.TileType<AcaciaTreeTop>())
                        {
                            HomeTileX = x;
                            HomeTileY = y;
                            found = true;
                        }

                        if (found)
                            break;
                    }

                    if (found)
                        break;
                }



                HomeOffset = new Vector2(Main.rand.NextFloat(-64f, 64f), 0f);
                NPC.netUpdate = true;
            }
        }
        public override void AI()
        {
            base.AI();
            FindHome();
            if (HomeTile.HasTile && !Fall)
            {
                NPC.Center = HomeTilePosition + HomeOffset - new Vector2(0, 32);
                NPC.TargetClosest();
            }
            _inTimer++;
            NPC.scale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(_inTimer / 100f));

            Timer++;
            if(Timer % 15 == 0)
            {
                int i = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Hive);
                Main.dust[i].noGravity = false;
            }
            if(Timer >= 120)
            {
                Timer = 0;
                if (MultiplayerHelper.IsHost)
                {
                    if(SpawnCount < 6)
                    {
                        int beedId = NPCID.BeeSmall; ;
                        if (Main.rand.NextBool(2))
                            beedId = NPCID.Bee;
                        if (Main.rand.NextBool(8))
                            beedId = NPCID.Hornet;
                        NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, beedId);
                        SpawnCount++;
                    }

                }
            }

            if (KindaHomeless)
                NPC.active = false;

            if (!HomeTile.HasTile || Fall)
            {
                //Fall and break
                NPC.noGravity = false;
                if (NPC.collideY)
                {
                    if (MultiplayerHelper.IsHost)
                    {
                        for(int i =0; i < 2; i++)
                        {
                            int beedId = NPCID.Bee;
                            if (Main.rand.NextBool(2))
                                beedId = NPCID.Bee;
                            if (Main.rand.NextBool(8))
                                beedId = NPCID.Hornet;
                            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, beedId);
                        }
                    }

                    NPC.SimpleStrikeNPC(200, 1);
                }
            }
            NPCID.Sets.HurtingBees[Type] = true;
            float range = MathHelper.ToRadians(7);
            NPC.rotation = MathHelper.Lerp(-range, range, ExtraMath.Osc(0f, 1f, offset: HomeTileX));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            if(NPC.life <= NPC.lifeMax / 2f)
            {
                Fall = true;
                NPC.netUpdate = true;
            }
            if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
            {
                for (int k = 0; k < 2; k++)
                {
                    Vector2 pos = NPC.position;
                    pos.X += Main.rand.Next(0, NPC.width);
                    pos.Y += Main.rand.Next(0, NPC.height);
                    Particle<SmokeParticle>.SpawnInAlphaLayer(pos, -Vector2.UnitY);
                }

                int headGore = Mod.Find<ModGore>($"{Name}_Gore_Top").Type;
                int legGore = Mod.Find<ModGore>($"{Name}_Gore_Bottom").Type;

                // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
            }
        }
        public override void OnKill()
        {
            base.OnKill();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.Common(ItemID.HoneyBlock, minimumDropped: 5, maximumDropped: 10));
            npcLoot.Add(ItemDropRule.Common(ItemID.Hive, minimumDropped: 5, maximumDropped: 10));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.GetModPlayer<BiomePlayer>().ZoneMarsh)
            {
                return 0.1f;
            }
            return base.SpawnChance(spawnInfo);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width / 2f, 0f);
         
            spriteBatch.Draw(texture, NPC.Center - screenPos, null, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
