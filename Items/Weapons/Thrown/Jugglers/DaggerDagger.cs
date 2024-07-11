using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.Thrown.Jugglers;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown.Jugglers
{
    internal class DaggerDagger : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Alcarishasd",  Helpers.LangText.Common("Juggler"))
            {
                OverrideColor = ColorFunctions.JugglerWeaponType
            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.damage = 232;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 24;
            Item.height = 24;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 5);
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Lime;
            Item.crit = 16;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DaggerDaggerProj>();
            Item.shootSpeed = 15;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            if(player.altFunctionUse == 2)
            {
                JugglerPlayer jugglerPlayer = player.GetModPlayer<JugglerPlayer>();
                if (jugglerPlayer.CatchCount > 0)
                {
                    velocity *= 2;
                    type = ModContent.ProjectileType<DaggerDaggerProj2>();
                    jugglerPlayer.CatchCount--;

                    int combatText = CombatText.NewText(player.getRect(), Color.White, $"x{jugglerPlayer.CatchCount + 1}", true);
                    CombatText numText = Main.combatText[combatText];
                    numText.lifeTime = 60;

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg"), position);
                }
            }

        }
    }
}
