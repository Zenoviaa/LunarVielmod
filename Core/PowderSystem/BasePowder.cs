using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.PowderSystem
{
    internal abstract class BasePowder : ModItem
    {
        public int ExplosionType;
        public float ExplosionScreenshakeAmt;
        public SoundStyle? ExplosionSound = null;
        public float DamageModifier;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
        }

        public virtual Projectile NewProjectile(Projectile igniterCardProjectile, Vector2 explosionPosition)
        {
            Vector2 offset = Main.rand.NextVector2Circular(64, 64);
            Projectile p = Projectile.NewProjectileDirect(igniterCardProjectile.GetSource_FromThis(), explosionPosition + offset, Vector2.Zero,
               ExplosionType, igniterCardProjectile.damage, igniterCardProjectile.knockBack, igniterCardProjectile.owner);

            if (ExplosionSound.HasValue)
            {
                SoundEngine.PlaySound(ExplosionSound, explosionPosition);
            }

            return p;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            /*
            TooltipLine line = new TooltipLine(Mod, "PowderDamageModifier", LangText.Common("PowderDamage", DamageModifier * 100));
            line.OverrideColor = new Color(80, 187, 124);
            tooltips.Add(line);

            line = new TooltipLine(Mod, "PowderEquip", LangText.Common("PowderEquip"));
            line.OverrideColor = Color.Lerp(new Color(80, 187, 124), Color.Black, 0.5f);
            tooltips.Add(line);*/
        }
    }
}
