using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Melee.Spears;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    public class SwordOfGlactia : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Magic;

        //Defaults for the other class
        public override void SetClassSwappedDefaults()
        {
            //Do if(IsSwapped) if you want to check for the alternate class
            //Stats to have when in the other class
            Item.damage = 140;
            Item.mana = 20;
        }
        public override void SetStaticDefaults()
        {

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 95;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 16, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<SwordOfGlactiaProj>();
            Item.shootSpeed = 30f;
            Item.DamageType = DamageClass.Melee;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(2))
            {
                // Emit dusts when the sword is swung
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.CopperCoin);
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ItemID.Gladius, 1);
            recipe.AddIngredient(ModContent.ItemType<Gallasis>(), 1);
            recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 10);
            recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 10);
            recipe.AddIngredient(ModContent.ItemType<GladiatorSpear>(), 1);
            recipe.AddIngredient(ModContent.ItemType<ArchariliteRaysword>(), 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }


        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var entitySource = player.GetSource_FromThis();
            if (Main.rand.NextBool(7))
            {
                int dist = 40;
                Vector2 targetExplosionPos = target.Center;
                for (int i = 0; i < 200; ++i)
                {
                    if (Main.npc[i].active && (Main.npc[i].Center - targetExplosionPos).Length() < dist)
                    {
                        Main.npc[i].HitEffect(0, damageDone);
                    }
                }
                for (int i = 0; i < 15; ++i)
                {
                    target.AddBuff(BuffID.OnFire, 300, true);
                    int newDust = Dust.NewDust(new Vector2(targetExplosionPos.X - (dist / 2), targetExplosionPos.Y - (dist / 2)), dist, dist, DustID.Electric, 0f, 0f, 40, default(Color), 2.5f);
                    Main.dust[newDust].noGravity = true;
                    Main.dust[newDust].velocity *= 5f;
                    newDust = Dust.NewDust(new Vector2(targetExplosionPos.X - (dist / 2), targetExplosionPos.Y - (dist / 2)), dist, dist, DustID.Electric, 0f, 0f, 40, default(Color), 1.5f);
                    Main.dust[newDust].velocity *= 3f;
                }
            }
        }
    }
}