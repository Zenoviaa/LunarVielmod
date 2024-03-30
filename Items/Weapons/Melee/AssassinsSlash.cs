using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    public class AssassinsSlash : ModItem
    {
        public int Hits;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ivyen Saber");
            // Tooltip.SetDefault("Has a chance to poison enemies.");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.noUseGraphic = false;
            Item.DamageType = DamageClass.Melee;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }

            return true;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.GetModPlayer<MyPlayer>().AssassinsSlashnpc != null && Hits == 3)
                {
                    player.GetModPlayer<MyPlayer>().AssassinsSlash = true;
                    Hits = 0;
                    Item.noUseGraphic = true;
                }


            }
            else
            {
                Item.noUseGraphic = false;
            }
  
            return base.CanUseItem(player);
        }
        
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                // Emit dusts when the sword is swung
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Plantera_Green);
            }
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Hits != 3)
            {
                Hits += 1;
            }

            if(Hits == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsSlashCharge"), player.position);              
                Hits = 3;
            }
            player.GetModPlayer<MyPlayer>().AssassinsSlashnpc = target;
            player.AddBuff(ModContent.BuffType<AssassinsSlashBuff>(), 480);
            target.AddBuff(ModContent.BuffType<AssassinsSlashBuff>(), 480);
        }
    }
}