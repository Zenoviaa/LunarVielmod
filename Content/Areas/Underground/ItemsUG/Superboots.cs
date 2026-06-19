using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Items.Accessories.Players;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.ItemsUG;

public class SuperbootsPlayer : ModPlayer
{
    public bool hasSuperBoots;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasSuperBoots = false;
    }
    public override void UpdateLifeRegen()
    {
        base.UpdateLifeRegen();
    }

    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (!hasSuperBoots)
            return;

        if (MathF.Abs(Player.velocity.X) + 0.5f >= Player.accRunSpeed)
        {
            Player.GetModPlayer<DashPlayer>().DashRegenerationBonus += 0.5f;
            Player.lifeRegen *= 2;
            if (Main.GameUpdateCount % 8 == 0)
            {
                var sp = SparkleParticle.Spawn(Player.Bottom, -Vector2.UnitY);
                sp.Scale *= 0.35f;
                sp.gravity = 0;
                sp.outerColor = Color.Gold;
                sp.innerColor = Color.White;
                sp.fast = true;
                sp.flickering = true;
            }
        }
    }
}

public class Superboots : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<SuperbootsPlayer>().hasSuperBoots = true;
       
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MinersGold, BlankAccessory>();
    }
}
