using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class BurningGBBroochPlayer : ModPlayer
    {
        public bool BurningGBActive => Player.GetModPlayer<BroochSpawnerPlayer>().BroochActive(ModContent.ItemType<BurningGBroochA>());

        public int burningGBCooldown;
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            burningGBCooldown--;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //BURNING GB BROOCH HIT EFFECT
            if (BurningGBActive && burningGBCooldown <= 0)
            {

                for (int d = 0; d < 4; d++)
                {
                    float speedXa = Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-1f, 1f);
                    float speedYa = Main.rand.Next(10, 15) * 0.01f + Main.rand.Next(-1, 1);


                    Vector2 speedea = Main.rand.NextVector2Circular(0.5f, 0.5f);

                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa, speedYa,
                        ProjectileID.IchorSplash, 10, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1f, speedYa * 1.5f,
                        ModContent.ProjectileType<AlcadizBombExplosion>(), 30, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.7f, speedYa * 0.6f,
                        ProjectileID.IchorSplash, 55, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.5f, speedYa * 0.3f,
                        ProjectileID.IchorSplash, 45, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1.3f, speedYa * 0.3f,
                        ProjectileID.IchorSplash, 65, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1f, speedYa * 1.5f,
                        ProjectileID.IchorSplash, 40, 1f, Player.whoAmI);
                }

                burningGBCooldown = 220;
            }
        }
    }

    public class BurningGBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
            BroochType = BroochType.Advanced;
        }
    }
}