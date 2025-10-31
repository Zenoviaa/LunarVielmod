using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles.Thrown.Jugglers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown.Jugglers
{
    public class StickyCards : BaseJugglerItem
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 90;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 24;
            Item.height = 24;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(gold: 5);
            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 29, 50, 0);
            Item.rare = ModContent.RarityType<SirestiasSpecialRarity>();
            Item.crit = 16;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<StickyCardsProj>();
            Item.shootSpeed = 24;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankJuggler>(), material: ModContent.ItemType<PearlescentScrap>());
        }
    }
}
