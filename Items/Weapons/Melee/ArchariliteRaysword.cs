using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Slashers.ArchariliteRaysword;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    public class ArchariliteRaysword : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 16, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<ArchariliteRaysWave>();
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
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
        public int AttackCounter = 1;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 0)
            {

                type = ModContent.ProjectileType<ArchariliteRayswordSlash>();

            }

        }
       

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            int dir = AttackCounter;
            if (player.direction == 1)
            {
                player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter;
            }
            else
            {
                player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter * -1;

            }
            AttackCounter = -AttackCounter;

            if (player.GetModPlayer<MyPlayer>().ArchariliteSC)
            {
                Item.shootSpeed = 38f;
                Item.useTime = 14;
                Item.useAnimation = 14;
                Item.damage = 24;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1, dir);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ArchariliteRaysWaveSC>(), damage * 2, knockback, player.whoAmI, 1, dir);
            }
            else
            {
                Item.shootSpeed = 30f;
                Item.useTime = 19;
                Item.useAnimation = 19;
                Item.damage = 13;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1, dir);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ArchariliteRaysWave>(), damage * 2, knockback, player.whoAmI, 1, dir);
            }

            return false;
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
                    int newDust = Dust.NewDust(new Vector2(targetExplosionPos.X - (dist / 2), targetExplosionPos.Y - (dist / 2)), dist, dist, DustID.CopperCoin, 0f, 0f, 40, default(Color), 2.5f);
                    Main.dust[newDust].noGravity = true;
                    Main.dust[newDust].velocity *= 5f;
                    newDust = Dust.NewDust(new Vector2(targetExplosionPos.X - (dist / 2), targetExplosionPos.Y - (dist / 2)), dist, dist, DustID.CopperCoin, 0f, 0f, 40, default(Color), 1.5f);
                    Main.dust[newDust].velocity *= 3f;
                }
            }
        }
    }
}