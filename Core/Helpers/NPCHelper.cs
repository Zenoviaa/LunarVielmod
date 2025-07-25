﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Helpers
{
    public static class NPCHelper
    {
        public static void Kill(this NPC npc)
        {
            npc.life = 0;
            npc.NPCLoot();
            npc.active = false;

            //npc.netUpdate = true;
        }

        public static void OpenShop(NPC npc)
        {
            Main.LocalPlayer.SetTalkNPC(npc.whoAmI);
            string shopName = null;

            if (npc.ModNPC != null)
            {
                npc.ModNPC.OnChatButtonClicked(false, ref shopName);
                SoundEngine.PlaySound(SoundID.MenuTick);

                if (shopName != null)
                {
                    // Copied from Main.OpenShop
                    Main.playerInventory = true;
                    Main.stackSplit = 9999;
                    Main.npcChatText = "";
                    Main.SetNPCShopIndex(1);
                    Main.instance.shop[Main.npcShop].SetupShop(NPCShopDatabase.GetShopName(npc.type, shopName), npc);
                }
            }
        }
        /// <summary>
        /// Returns whether a boss is alive
        /// </summary>
        /// <returns></returns>
        public static bool IsBossAlive()
        {
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC npc = Main.npc[k];
                if (npc.boss)
                    return true;
            }
            return false;
        }

        public static float TilesToDistance(this float tiles)
        {
            return tiles * 16;
        }

        public static bool IsHealthLowerThanPercent(this NPC npc, float healthPercent)
        {
            float lifeMax = npc.lifeMax;
            float life = npc.life;
            float lifeFactor = life / lifeMax;
            return lifeFactor <= healthPercent;
        }

        public static int ScaleFromContactDamage(this NPC npc, float damageMultiplier)
        {
            float damage = npc.damage;
            float factor = 1f;
            if (Main.masterMode)
            {
                factor = 0.33f;
            }
            else if (Main.expertMode)
            {
                factor = 0.5f;
            }

            return (int)(damage * damageMultiplier * factor);
        }

        public static NPC[] FindNPCsInRange(Vector2 position, float maxDetectDistance, int npcType)
        {
            List<NPC> npcs = new List<NPC>();
            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;


            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                if (!target.active)
                    continue;
                if (npcType != -1 && target.type != npcType)
                    continue;

                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, position);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        npcs.Add(target);
                    }
                }
            }

            return npcs.ToArray();
        }

        /// <summary>
        /// Finds the closest NPC to a point that can be chased by a minion/projectile
        /// </summary>
        /// <param name="position"></param>
        /// <param name="maxDetectDistance"></param>
        /// <returns></returns>
        public static NPC FindClosestNPC(Vector2 position, float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                if (!target.active)
                    continue;

                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, position);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
}
