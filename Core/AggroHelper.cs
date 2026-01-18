using Stellamod.Core.ZTileSystem;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Core
{
    public class NPCAggroRework : ModSystem
    {
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_NPC.TargetClosest += TargetClosestBetter;
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_NPC.TargetClosest -= TargetClosestBetter;
        }

        private void TargetClosestBetter(On_NPC.orig_TargetClosest orig, NPC self, bool faceTarget)
        {
            self.TargetClosestByAggro();
            if(faceTarget && self.HasValidTarget)
            {
                Player target = Main.player[self.target];
                Rectangle targetRect = target.getRect();
                self.direction = 1;
                if ((float)(targetRect.X + targetRect.Width / 2) < self.position.X + (float)(self.width / 2))
                    self.direction = -1;
            }
        }
    }

    public class AggroSystem : ModSystem
    {
        private float _reseedTimer;
        public int[] aggro;
        public int seed;
        public override void OnModLoad()
        {
            base.OnModLoad();
            aggro = new int[256];
        }
        public override void PreUpdateEntities()
        {
            base.PreUpdateEntities();
            if (MultiplayerHelper.IsHost)
            {
                _reseedTimer++;
                if(_reseedTimer >= 360)
                {
                    seed = Main.rand.Next(0, int.MaxValue);
                    //Sync the seed
                    if(Main.netMode != NetmodeID.SinglePlayer)
                    {
                        Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.AggroSync, (int)seed).Send();
                    }

                    _reseedTimer = 0;
                  //  Main.NewText($"NEW SEED {seed}");
                }
            }

            //Reset aggro values
            for (int i = 0; i < aggro.Length; i++)
            {
                aggro[i] = 0;
            }
        }
    }

    public static class AggroHelper
    {
        private static UnifiedRandom _aggroRandom;

        public static void TargetClosestByAggro(this NPC npc, float maxSearchRange = 2000)
        {
            AggroSystem aggroSystem = ModContent.GetInstance<AggroSystem>();
            _aggroRandom ??= new UnifiedRandom();
            _aggroRandom.SetSeed(aggroSystem.seed);

            int totalWeight = 0;
            List<Point> weights = new List<Point>();

            foreach (var player in Main.ActivePlayers)
            {
                float distance = Vector2.Distance(player.Center, npc.Center);
                if (distance > maxSearchRange)
                    continue;

                //Get the aggro variable
                //Base 100 aggro
                int localAggro = aggroSystem.aggro[player.whoAmI];
                int adjustedAggro = 100 + localAggro;
                int playerAggro = Math.Max(1, adjustedAggro);


                Point aggro = new Point(playerAggro, player.whoAmI);
                weights.Add(aggro);
                totalWeight += playerAggro;
            }

            if (weights.Count <= 0)
                return;

            int currentWeight = 0;
            int randWeight = _aggroRandom.Next(0, totalWeight);
            for (int i = 0; i < weights.Count; i++)
            {
                Point weight = weights[i];
                currentWeight += weight.X;
                if (randWeight <= currentWeight)
                {
                    //We found our target :)
                    npc.target = weight.Y;
               //     Main.NewText($"{npc.FullName} Target ${Main.player[npc.target].name}");
                    break;
                }
            }
        }
    }
}
