using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Runes
{
    internal class RuneOfShadePlayer : ModPlayer
    {
        public bool hasShadeRune;
        public bool hideShadeRuneVisual;
        public float shadeTimer;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasShadeRune = false;
            hideShadeRuneVisual = false;
        }

        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (hideShadeRuneVisual || !hasShadeRune)
            {
                shadeTimer -= 0.01f;
            }
            else if (hasShadeRune)
            {
                shadeTimer += 0.01f;
            }

            shadeTimer = MathHelper.Clamp(shadeTimer, 0f, 1f);
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            float multiplier = MathHelper.Lerp(1f, 0.5f, shadeTimer);
            r *= multiplier;
            g *= multiplier;
            b *= multiplier;
            a *= multiplier;
        }
    }

    internal class RuneOfShade : BaseRune
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.05f;
            player.jumpSpeedBoost += 0.05f;
            player.aggro -= 300;

            RuneOfShadePlayer shadePlayer = player.GetModPlayer<RuneOfShadePlayer>();
            shadePlayer.hasShadeRune = true;
            shadePlayer.hideShadeRuneVisual = hideVisual;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankRune>(), material: ModContent.ItemType<MiracleThread>());
        }
    }
}