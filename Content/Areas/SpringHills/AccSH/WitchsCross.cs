using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH;

public class WitchsCrossPlayer : ModPlayer
{
    public bool hasWitchsCross;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasWitchsCross = false;
    }
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        var cauldronPlayer = Player.GetModPlayer<CauldronPlayer>();
        Player.GetDamage(DamageClass.Generic) += 0.05f * cauldronPlayer.CompletedMaterials;
    }
}

public class WitchsCross : ModItem
{
    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        if (Main.rand.NextBool(32) && !Main.gameInactive)
        {
            SparkleParticle dp = SparkleParticle.SpawnInUI(position + Main.rand.NextVector2Circular(32, 32), -Vector2.UnitY, Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
            dp.gravity = 0;
            dp.innerColor = Color.White;
            dp.outerColor = Color.White;
            dp.fast = true;
            dp.Scale *= 0.2f;
        }

        return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        base.ModifyTooltips(tooltips);
        var amt = Main.LocalPlayer.GetModPlayer<CauldronPlayer>().CompletedMaterials;
        var pct = amt * 0.05f;
        var tooltipLine = new TooltipLine("CompletedCrafts", LangText.Common($"CompletedCrafts", amt, pct.ToString("P2")));
        tooltipLine.OverrideColor = Color.Gold;
        tooltips.Add(tooltipLine);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
        Item.defense = 5;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<WitchsCrossPlayer>().hasWitchsCross = true;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankAccessory>();
    }
}