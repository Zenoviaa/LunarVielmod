using Microsoft.Xna.Framework;
using Stellamod.Common;
using Stellamod.Content.Dusts;
using Stellamod.Helpers;

using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class SirestiasPlayer : ModPlayer
    {
        public bool HasSirestiasMask;
        public override void ResetEffects()
        {
            HasSirestiasMask = false;
        }

        public override void PostUpdateEquips()
        {
            if (HasSirestiasMask)
            {
                if (Player.statLife == Player.statLifeMax2)
                {
                    Player.GetDamage(DamageClass.Generic) += 0.2f;
                    float num = 16;
                    for (int i = 0; i < num; i++)
                    {
                        float progress = (float)i / num;
                        Vector2 velocity = Vector2.Zero;
                        Vector2 offset = -Vector2.UnitY;
                        offset = offset.RotatedBy(progress * MathHelper.TwoPi);
                        offset *= 80;
                        Vector2 position = Player.Center + offset;
                        Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), velocity, newColor: Color.White, Scale: 0.3f);
                    }
                }
            }
        }
    }

    public class SirestiasMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ItemSets.IsSoldBySirestias[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 36;
            Item.accessory = true;
            Item.shopSpecialCurrency = Stellamod.NoHitCrystalCurrencyID;
            Item.shopCustomPrice = 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<SirestiasPlayer>().HasSirestiasMask = true;
        }
    }
}
