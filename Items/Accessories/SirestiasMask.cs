using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;

using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class SirestiasPlayer : ModPlayer
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
                    Player.GetDamage(DamageClass.Generic) += 0.30f;
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

    internal class SirestiasMask : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 36;
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 12);
            Item.maxStack = 1;
            Item.rare = ModContent.RarityType<SirestiasSpecialRarity>();
            Item.value = Item.sellPrice(0, 15, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<SirestiasPlayer>().HasSirestiasMask = true;
        }
    }
}
