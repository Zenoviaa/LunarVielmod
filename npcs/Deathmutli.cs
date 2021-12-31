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
using StellaMod.Dusts;

namespace Stellamod.npcs
{
    public class Deathmulti : GlobalNPC
    {


        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (npc.HasBuff(ModContent.BuffType<Deathmultiplier>()) && projectile.CountsAsClass(DamageClass.Melee))
            {
                damage = (int)(damage * 1.7f);
            }
        }
    }
}
