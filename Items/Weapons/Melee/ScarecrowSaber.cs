using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.Weapons.Bow;
using Terraria.DataStructures;
using Mono.Cecil;
using static Terraria.ModLoader.PlayerDrawLayer;
using Stellamod.Projectiles.Weapons.Swords;
using Stellamod.Projectiles.Weapons.Magic;
using Stellamod.Items.Accessories.Runes;

using Stellamod.Projectiles.Weapons.Spears;

namespace Stellamod.Items.Weapons.Swords
{
    public class ScarecrowSaber : ModItem
    {
        public override void SetStaticDefaults()
        {

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 16, 0);
            Item.rare = ItemRarityID.Gray;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<ScarecrowSaberProg>();
            Item.shootSpeed = 30f;
            Item.DamageType = DamageClass.Melee;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            float numberProjectiles = 3;
            float rotation = MathHelper.ToRadians(25);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(2))
            {
                // Emit dusts when the sword is swung
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.CopperCoin);
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var entitySource = player.GetSource_FromThis();
            if (Main.rand.Next(7) == 0)
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