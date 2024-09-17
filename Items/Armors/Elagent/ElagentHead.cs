using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Buffs;
using Stellamod.Items.Materials;
using Stellamod.NPCs.Bosses.Niivi;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Elagent
{
    public class ElegantPlayer : ModPlayer
    {
        public bool hasSetBonus;
        public override void ResetEffects()
        {
            hasSetBonus = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Check if we can proc the effect on this NPC
            if (!hasSetBonus)
                return;

            target.AddBuff(ModContent.BuffType<HolyAnxiety>(), 360);

            //This might look weird, but certain NPCs are immune to this buff :P
            if (!target.HasBuff(ModContent.BuffType<HolyAnxiety>()))
                return;

            if (hit.DamageType != DamageClass.Summon)
                return;

            //Angel stuff
            if (Main.rand.NextBool(50))
            {
                var entitySource = Player.GetSource_FromThis();
                int projType = ModContent.ProjectileType<HaloOfDeathProj>();

                //Deal a % of their health as damage
                int damageToDo = (int)(target.life * 0.9f);

                //NPC to attack
                int targetNpc = target.whoAmI;
                Projectile.NewProjectile(entitySource, target.Center, Vector2.Zero,
                   projType, damageToDo, 1, Player.whoAmI, targetNpc);
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class ElagentHead : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Orange;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.2f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ElagentBody>() 
                && legs.type == ModContent.ItemType<ElagentLegs>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);//"+1 Max Minions\n+45 Max Life \nHit enemies have a rare chance to be assaulted by angelic power!");
            player.maxMinions += 1;
            player.statLifeMax2 += 45;
            player.GetModPlayer<ElegantPlayer>().hasSetBonus = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<StarSilk>(), 8);
            recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 8);
            recipe.AddIngredient(ItemID.Feather, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
