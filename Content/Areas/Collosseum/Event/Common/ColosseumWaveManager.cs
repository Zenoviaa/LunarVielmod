using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia;
using Stellamod.Content.Areas.Collosseum.BossesCL.EliteCommander;
using Stellamod.Content.Areas.Collosseum.BossesCL.Gustbeak;
using Stellamod.Content.Areas.Collosseum.Event;
using Stellamod.Core.TitleSystem;
using Stellamod.Helpers;
using Stellamod.Items.Ores;
using Stellamod.NPCs;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.Event.Common
{
    public class ColosseumWaveManager : ModNPC
    {
        private bool _broadcastWave;
        private ref float Timer => ref NPC.ai[0];
        private int ColosseumIndex => (int)NPC.ai[1];
        private ref float Progresser => ref NPC.ai[2];
        public override string Texture => TextureRegistry.ZuiEffect;

        private static Rectangle _colosseumRectangle;
        private Point _startTile;
        private int _enemyCount;
        private int _waveIndex;
        private int _maxWave;
        private bool _shouldDie;
        public static bool goAwayGintzia;
        public static Vector2 GongSpawnWorld
        {
            get
            {
                if (_colosseumRectangle == Rectangle.Empty)
                    _colosseumRectangle = Structurizer.ReadRectangle("Struct/Colosseum/TheColosseum");
                NPCPointSpawnSystem npcPointSpawnSystem = ModContent.GetInstance<NPCPointSpawnSystem>();
                Point colosseumOriginTile = npcPointSpawnSystem.GetStructureTile("Struct/Colosseum/TheColosseum");
                Point centerOffset = (_colosseumRectangle.Size() / 2).ToPoint();
                Point colosseumCenterTile = colosseumOriginTile + new Point(centerOffset.X, -centerOffset.Y);
                Vector2 gongSpawnWorld = colosseumCenterTile.ToWorldCoordinates();
                return gongSpawnWorld;
            }
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 1;
            NPC.height = 1;
            NPC.lifeMax = 100;
            NPC.defense = 10;
            NPC.damage = 10;
            NPC.npcSlots = 10f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.ShowNameOnHover = false;
        }
        public override bool CheckActive()
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_shouldDie);
            writer.Write(_broadcastWave);
            writer.Write(_enemyCount);
            writer.Write(_waveIndex);
            writer.Write(_maxWave);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _shouldDie = reader.ReadBoolean();
            _broadcastWave = reader.ReadBoolean();
            _enemyCount = reader.ReadInt32();  
            _waveIndex = reader.ReadInt32();
            _maxWave = reader.ReadInt32();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }



        public override void AI()
        {
            base.AI();
            if (_shouldDie)
            {
                NPC.active = false;
            }
            NPC.scale = ExtraMath.Osc(0.8f, 1f);

            if (_broadcastWave && Main.netMode != NetmodeID.Server)
            {
                TitleCardUISystem uiSystem = ModContent.GetInstance<TitleCardUISystem>();
                uiSystem.OpenUI($"Wave {_waveIndex}", duration: 3);
                _broadcastWave = false;
            }

            if (!MultiplayerHelper.IsHost)
                return;

            if (Progresser > 0)
            {
                Progress();
                Progresser--;
            }

            if (AllPlayersDead() || AllPlayersTooFarAway())
            {
                _shouldDie = true;
                NPC.netUpdate = true;
            }

            Timer++;
            if (Timer == 1)
            {

                if (MultiplayerHelper.IsHost)
                {
                    _enemyCount = 0;
                    _waveIndex = 0;
                    _maxWave = 7;
                    _startTile = NPC.position.ToTileCoordinates();
                    goAwayGintzia = false;
                    Progress();

                    //Spawn Chains so you can't leave
                    Projectile.NewProjectile(new EntitySource_WorldEvent(), GongSpawnWorld + new Vector2(0, -266), Vector2.Zero,
                        ModContent.ProjectileType<GoldChain>(), 25, 4, Main.myPlayer);
                    Projectile.NewProjectile(new EntitySource_WorldEvent(), GongSpawnWorld + new Vector2(0, 412), Vector2.Zero,
                        ModContent.ProjectileType<GoldChain>(), 25, 4, Main.myPlayer);
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y - 180,
                        ModContent.NPCType<CommanderGintziaTaunting>());
                }
            }
        }

        public void Progress()
        {
            _enemyCount--;
            if (_enemyCount < 0)
            {
                Spawn();
                _waveIndex++;
            }
        }

        public void SpawnWave(int index)
        {
            if (!MultiplayerHelper.IsHost)
                return;

            switch (ColosseumIndex)
            {
                case 0:
                    SpawnBronzeWave(index);
                    break;
                case 1:
                    SpawnSilverWave(index);
                    break;
                case 2:
                    SpawnGoldWave(index);
                    break;
            }
        }

        private void SpawnBronzeWave(int index)
        {
            switch (index)
            {
                case 0:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeSoldier>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeSoldier>());
                    break;
                case 1:
                    Spawn(new Point(-37, 0), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeSoldier>());
                    Spawn(new Point(-27, 0), ModContent.NPCType<GintzeSoldier>());
                    break;
                case 2:
                    Spawn(new Point(37, 0), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(33, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(27, 0), ModContent.NPCType<Gintzling>());
                    break;
                case 3:
                    Spawn(new Point(-33, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-15, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(33, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(15, 0), ModContent.NPCType<Gintzling>());
                    break;
                case 4:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeSoldier>());
                    Spawn(new Point(-15, 0), ModContent.NPCType<GintzeSoldier>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeSoldier>());
                    Spawn(new Point(15, 0), ModContent.NPCType<GintzeSoldier>());
                    Spawn(new Point(33, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(15, 10), ModContent.NPCType<Gintzling>());
                    break;
                case 5:
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeSoldier>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeSoldier>());
                    Spawn(new Point(-15, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(15, 10), ModContent.NPCType<Gintzling>());
                    break;
                case 6:
                    Spawn(new Point(27, 0), ModContent.NPCType<EliteCommander>());
                    break;
            }
        }

        private void SpawnSilverWave(int index)
        {
            switch (index)
            {
                case 0:
                    Spawn(new Point(-33, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(33, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(33, 10), ModContent.NPCType<Gintzling>());
                    break;
                case 1:
                    Spawn(new Point(-33, 0), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(33, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeWindRider>());
                    break;
                case 2:
                    Spawn(new Point(-33, -10), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(-15, -10), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(15, -10), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(33, -10), ModContent.NPCType<GintzeWindRider>());
                    break;
                case 3:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeSpearman>());
                    break;
                case 4:
                    Spawn(new Point(33, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-15, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(15, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-33, -10), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(33, -10), ModContent.NPCType<GintzeWindRider>());
                    break;

                case 5:
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(-15, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(15, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(-27, -13), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(27, -13), ModContent.NPCType<GintzeWindRider>());
                    break;
                case 6:
                    Spawn(new Point(-33, -64), ModContent.NPCType<Gustbeak>());
                    break;
            }
        }

        private void SpawnGoldWave(int index)
        {
            switch (index)
            {
                case 0:
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(-15, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(15, 10), ModContent.NPCType<Gintzling>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(-27, -13), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(27, -13), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(35, 0), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(35, 0), ModContent.NPCType<GintzeWindRider>());
                    break;
                case 1:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    break;
                case 2:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(-27, -13), ModContent.NPCType<GintzeWindRider>());
                    Spawn(new Point(27, -13), ModContent.NPCType<GintzeWindRider>());

                    break;
                case 3:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeSpearman>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeTumbleWeed>());
                    break;
                case 4:
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(-33, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(0, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(33, 10), ModContent.NPCType<GintzeCaptain>());
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(33, -10), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(-33, -10), ModContent.NPCType<GintzeTumbleWeed>());
                    break;
                case 5:
                    Spawn(new Point(33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(-33, 0), ModContent.NPCType<GintzeTumbleWeed>());
                    Spawn(new Point(35, 10), ModContent.NPCType<EliteCommander>());
                    Spawn(new Point(-35, 10), ModContent.NPCType<EliteCommander>());
                    break;
                case 6:
                    goAwayGintzia = true;
                    Spawn(new Point(0, -30), ModContent.NPCType<CommanderGintzia>());
                    break;
            }
        }
        public void Spawn(Point tileOffset, int npcType)
        {
            _enemyCount++;
            Point spawnPoint = _startTile + tileOffset;
            Vector2 spawnWorld = spawnPoint.ToWorldCoordinates();
            NPC.NewNPC(new EntitySource_WorldEvent(), (int)spawnWorld.X, (int)spawnWorld.Y,
                ModContent.NPCType<SpawnerNPC>(), ai0: npcType);
        }

        private void Spawn()
        {
            SpawnWave(_waveIndex);
            if (_waveIndex >= _maxWave)
            {
                CompleteColosseum();
            }
            _broadcastWave = true;
            NPC.netUpdate = true;
        }

        public void CompleteColosseum()
        {
            _shouldDie = true;
            NPC.netUpdate = true;
            ColosseumSystem colosseumSystem = ModContent.GetInstance<ColosseumSystem>();
            switch (ColosseumIndex)
            {
                case 0:
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y,
                        ModContent.NPCType<CoinSpawnerNPC>(), ai1: 500, ai3: ItemID.SilverCoin);
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y,
                        ModContent.NPCType<CoinSpawnerNPC>(), ai1: 30, ai3: ModContent.ItemType<GintzlMetal>());
                    colosseumSystem.completedBronzeColosseum = true;
                    break;
                case 1:
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y,
                        ModContent.NPCType<CoinSpawnerNPC>(), ai1: 750, ai3: ItemID.SilverCoin);
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y,
                        ModContent.NPCType<CoinSpawnerNPC>(), ai1: 50, ai3: ModContent.ItemType<GintzlMetal>());
                    colosseumSystem.completedSilverColosseum = true;
                    break;
                case 2:
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y,
                        ModContent.NPCType<CoinSpawnerNPC>(), ai1: 1000, ai3: ItemID.SilverCoin);
                    NPC.NewNPC(new EntitySource_WorldEvent(), (int)GongSpawnWorld.X, (int)GongSpawnWorld.Y,
                        ModContent.NPCType<CoinSpawnerNPC>(), ai1: 100, ai3: ModContent.ItemType<GintzlMetal>());
                    colosseumSystem.completedGoldColosseum = true;
                    break;
                case 3:
                    colosseumSystem.completedTrueColosseum = true;
                    break;
            }
        }

        private bool AllPlayersDead()
        {
            foreach (var player in Main.ActivePlayers)
            {
                if (!player.dead)
                    return false;
            }
            return true;
        }

        private bool AllPlayersTooFarAway()
        {
            foreach (var player in Main.ActivePlayers)
            {
                float distance = Vector2.Distance(GongSpawnWorld, player.Center);
                if (distance < 1280)
                    return false;
            }
            return true;
        }

        public static bool IsActive()
        {
            return NPC.AnyNPCs(ModContent.NPCType<ColosseumWaveManager>());
        }

        public static void ColosseumEnemyKilled()
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.type != ModContent.NPCType<ColosseumWaveManager>())
                    continue;
                npc.ai[2]++;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = NPC.Center - screenPos;
            Color glowColor = Color.Lerp(Color.White, Color.Goldenrod, ExtraMath.Osc(0f, 1f));
            glowColor.A = 0;
            spriteBatch.Draw(texture, drawPosition, null, glowColor, NPC.rotation, texture.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
