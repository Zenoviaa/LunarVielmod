using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.Bow;
using Terraria.DataStructures;
using Mono.Cecil;
using static Terraria.ModLoader.PlayerDrawLayer;
using Stellamod.Projectiles.Swords;
using Stellamod.Projectiles.Magic;
using Stellamod.Items.Accessories.Runes;

using Stellamod.Projectiles.Spears;

namespace Stellamod.Items.Weapons.Melee
{
    public class Myocardia : ModItem
    {
        public override void SetStaticDefaults()
        {

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 16, 0);
            Item.rare = ItemRarityID.Gray;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<DreadFireG>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Melee;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.RedTorch);
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {


            return true;
        }
    }
}