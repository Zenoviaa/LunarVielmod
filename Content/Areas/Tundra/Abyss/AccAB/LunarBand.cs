using Stellamod.Content.CommonMaterials;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.AccAB
{
    public class LunarBand : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale);
        }

        private float CalculateDamageBoost()
        {
            if (Main.dayTime)
                return 0;
            float nightProgress = (float)(Main.time / Main.nightLength);
            float nightEasing = EasingFunction.QuadraticBump(nightProgress);
          
            float damageBoost = MathHelper.Lerp(0f, 0.15f, nightEasing);
            return damageBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            TooltipLine line = new TooltipLine(Mod, "Strength", LangText.Common("LunarStrength", CalculateDamageBoost().ToString("P2")));
            tooltips.Add(line);
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            float damageBoost = CalculateDamageBoost();
            player.GetDamage(DamageClass.Generic) += damageBoost;



            if (Main.dayTime)
                return;
            if (hideVisual)
                return;
            if (!Main.rand.NextBool(24))
                return;

            Vector2 spawnPos = player.position;
            spawnPos.X += Main.rand.Next(0, player.width);
            spawnPos.Y += Main.rand.Next(0, player.height);
            DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
            {
                innerColor = Color.LightCyan,
                outerColor = Color.DarkBlue,
                gravity = 0f
            };
            Vector2 risingVelocity = -Vector2.UnitY;
            DustParticle.Spawn(spawnPos, risingVelocity, spawnParams);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<ConvulgingMater>());
        }
    }
}