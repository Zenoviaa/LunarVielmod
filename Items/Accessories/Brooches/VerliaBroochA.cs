using Stellamod.Brooches;
using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class VerliaBroochPlayer : ModPlayer
    {
        public bool VerliaBroochActive => Player.GetModPlayer<BroochSpawnerPlayer>().BroochActive(ModContent.ItemType<VerliaBroochA>());
        public int verliaBroochCooldown;

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            verliaBroochCooldown--;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //VERLIA BROOCH HIT EFFECT
            if (VerliaBroochActive && verliaBroochCooldown <= 0)
            {
                for (int d = 0; d < 4; d++)
                {
                    float speedXa = Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-1f, 1f);
                    float speedYa = Main.rand.Next(10, 15) * 0.01f + Main.rand.Next(-1, 1);

                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa, speedYa, ModContent.ProjectileType<VerliaBroochP>(), 10, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.7f, speedYa * 0.6f, ModContent.ProjectileType<VerliaBroochP>(), 10, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.5f, speedYa * 0.3f, ModContent.ProjectileType<VerliaBroochP2>(), 15, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1.3f, speedYa * 0.3f, ModContent.ProjectileType<VerliaBroochP2>(), 15, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1f, speedYa * 1.5f, ModContent.ProjectileType<VerliaBroochP3>(), 20, 1f, Player.whoAmI);
                }

                verliaBroochCooldown = 220;
            }
        }
    }

    public class VerliaBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 0, 90);
            Item.rare = ItemRarityID.Orange;
            Item.buffType = ModContent.BuffType<VerliaBroo>();
            Item.accessory = true;
        }
    }
}