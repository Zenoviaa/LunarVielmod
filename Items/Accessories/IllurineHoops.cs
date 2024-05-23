using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Buffs;
using Stellamod.Projectiles.Magic;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Accessories
{
    internal class LunarPlayer : ModPlayer
    {
        private int[] NpcTypes = new int[]
        {
            NPCID.SolarDrakomire,
            NPCID.SolarDrakomireRider,
            NPCID.SolarSolenian,
            NPCID.SolarSroller,
            NPCID.SolarCrawltipedeHead,
            NPCID.SolarCrawltipedeBody,
            NPCID.SolarCrawltipedeTail,
            NPCID.SolarSpearman,
            NPCID.VortexHornet,
            NPCID.VortexHornetQueen,
            NPCID.VortexLarva,
            NPCID.VortexRifleman,
            NPCID.VortexSoldier,
            NPCID.NebulaBeast,
            NPCID.NebulaBrain,
            NPCID.NebulaHeadcrab,
            NPCID.NebulaSoldier,
            NPCID.StardustCellBig,
            NPCID.StardustCellSmall,
            NPCID.StardustJellyfishBig,
            NPCID.StardustJellyfishSmall,
            NPCID.StardustSoldier,
            NPCID.StardustSpiderSmall,
            NPCID.StardustSpiderBig,
            NPCID.StardustWormBody,
            NPCID.StardustWormHead,
            NPCID.StardustWormTail,
        };

        public bool hasMoonflareBand;
        private int BuffType => ModContent.BuffType<MoonFlame>();
        private int Timer;
        public override void ResetEffects()
        {
            hasMoonflareBand = false;
        }

        private bool IsMatch(NPC npc)
        {
            for(int t = 0; t < NpcTypes.Length; t++)
            {
                if (npc.type == NpcTypes[t])
                    return true;
            }
            return false;
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            Timer--;
            if (hasMoonflareBand && Timer <= 0)
            {
                Timer = 30;
                float maxDetectRange = 1024;
                NPC[] npcs = NPCHelper.FindNPCsInRange(Player.position, maxDetectRange, -1);
                for(int n = 0; n < npcs.Length; n++)
                {
                    NPC npc = npcs[n];
                    if(IsMatch(npc) && !npc.HasBuff(BuffType))
                    {
                        Projectile.NewProjectile(Player.GetSource_FromThis(), npc.Center, Main.rand.NextVector2Circular(1, 1), 
                            ModContent.ProjectileType<MoonFlameSlashProj>(), 1, 1, Player.whoAmI);
                    }
                }
            }
        }
    }

    internal class IllurineHoops : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 2500;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<LunarPlayer>().hasMoonflareBand = true;
            player.GetDamage(DamageClass.Magic) *= 1.1f;
            player.manaCost -= 0.1f;
            player.manaRegen += 1;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<StarflareBand>(), 1);
            recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 100);
            recipe.AddIngredient(ModContent.ItemType<IllurineScale>(), 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
