using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    /// <summary>
    /// Base class for the magic tome attack style, it'll automatically set some defaults for you
    /// </summary>
    public abstract class BaseTome : ModItem
    {
        public sealed override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 24;
            Item.damage = 42;
            Item.knockBack = 1;
            Item.DamageType = DamageClass.Magic;
            Item.shootSpeed = 15f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.UseSound = SoundID.Item20;

            Item.rare = ItemRarityID.Green;
            Item.mana = 5;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            SetDefaults2();
        }

        /// <summary>
        /// Sets the color of the little dust particles that come off of the tome, defaults to white
        /// </summary>
        /// <returns></returns>
        public virtual Color GetTomeHintColor()
        {
            return Color.White;
        }

        public virtual void SetDefaults2()
        {

        }
    }
}
