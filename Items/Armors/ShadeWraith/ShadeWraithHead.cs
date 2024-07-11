using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.ShadeWraith
{
    internal class ShadeWraithPlayer : ModPlayer
    {
        public bool hasSetBonus;
        public override void ResetEffects()
        {
            hasSetBonus = false;
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (!hasSetBonus)
                return;

            //Nuh uh
            if (Player.HasBuff<ShadeWrathCooldown>())
                return;


            float percentOfLife = (float)Player.statLife / (float)Player.statLifeMax;
            if(percentOfLife <= 0.4f)
            {
                //Trigger the buff
                int time = 300;
                Player.AddBuff(ModContent.BuffType<ShadeWrath>(), time);

                int cooldownTime = 55 * 60;
                Player.AddBuff(ModContent.BuffType<ShadeWrathCooldown>(), cooldownTime);

                //Idk some effects here or something
                //Some sounds
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/OverGrowth_TP1"));
                for (int i = 0; i < 16; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    Color color = default(Color).MultiplyAlpha(0.1f);
                    Particle p = ParticleManager.NewParticle(Player.Center, speed, ParticleManager.NewInstance<Ink2>(), color, Main.rand.NextFloat(0.2f, 0.8f));
                    p.layer = Particle.Layer.BeforePlayersBehindNPCs;
                }
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class ShadeWraithHead : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.02f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == Mod.Find<ModItem>("ShadeWraithBody").Type && legs.type == Mod.Find<ModItem>("ShadeWraithLegs").Type;
        }
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);//"Become greatly empowered for a short time when low on health!\nJust one last breath...");
            player.GetModPlayer<ShadeWraithPlayer>().hasSetBonus = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<DarkEssence>(), 8);
            recipe.AddRecipeGroup(nameof(ItemID.DemoniteBar), 4);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
