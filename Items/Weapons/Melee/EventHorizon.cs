using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    public class EventHorizon : ModItem
    {
        public int WinterboundArrow;
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Damage reduces the farther you are away from the target");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 35, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<HorizonBolt>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Melee;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(2))
            {
                // Emit dusts when the sword is swung
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Snow);
            }
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            WinterboundArrow += 1;
            if (WinterboundArrow >= 4)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FrostBringer"), player.position);
                WinterboundArrow = 0;
                type = ModContent.ProjectileType<HorizonBomb>();
            }


        }
    }
}