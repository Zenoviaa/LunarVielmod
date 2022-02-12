using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Stellamod.Items.Materials;
using Stellamod.Buffs;

using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;

namespace Stellamod.npcs
{
    public class DeathmultiBL : GlobalNPC
    {


        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (npc.HasBuff(ModContent.BuffType<DeathmultiplierBloodLamp>()))
            {
                damage = (int)(damage * 2.1f);
            }
        }
    }
}