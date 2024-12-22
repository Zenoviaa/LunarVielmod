using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class ShadowCloak : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
            ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 36;
            Item.value = 2500;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);

            //Shadow Visual
            if (Main.rand.NextBool(5) && !hideVisual)
            {
                float radius = 16;
                int count = Main.rand.Next(6);
            }

            //Increased armor pen
            player.statDefense += 8;
            player.endurance += 0.04f;
            player.aggro -= 500;

            // Starting search distance
            // This code is required either way, used for finding a target
            float distanceFromTarget = 128f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                float between = Vector2.Distance(npc.Center, player.Center);
                bool inRange = between < distanceFromTarget;
                if (npc.chaseable && inRange && !npc.townNPC && !npc.dontTakeDamage && NPCID.Sets.ActsLikeTownNPC[npc.type] == false)
                {
                    npc.AddBuff(BuffID.Confused, 2);
                    npc.AddBuff(BuffID.CursedInferno, 2);
                    npc.AddBuff(BuffID.Ichor, 2);
                    npc.AddBuff(BuffID.ShadowFlame, 2);
                    npc.AddBuff(BuffID.OnFire3, 2);
                }
            }
        }
    }
}
