using Stellamod.Common.GunSystem;
using Stellamod.Content.Areas.Fable.WeaponsFB;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS
{
    public class Hornet : BaseGun
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 62;
            Item.height = 32;
            Item.scale = 1f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/gun1");

            // Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 12;
            Item.knockBack = 4;
            Item.noMelee = true;

            // Gun Properties
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 8f;
            Item.useAmmo = AmmoID.Bullet; // Restrict the type of ammo the weapon can use, so that the weapon cannot use other ammos
            Item.value = 10000;
        }

        public override void SetMagazine(ref GunReloadParams fireParams)
        {
            base.SetMagazine(ref fireParams);
            fireParams.maxAmmo = 24;
            fireParams.reloadWindow = 60;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0f, 2f);
        }

        public override void ShootEffects(Vector2 position, Vector2 velocity)
        {
            SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew6");
            shootSound.PitchVariance = 0.3f;
            shootSound.Volume = 0.15f;
            SoundEngine.PlaySound(shootSound, position);
            BasicMuzzleFlash(position, velocity, Color.Yellow, Color.Red);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<HornetLob>() });
        }

        public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float rot = velocity.ToRotation();
            float spread = 0.4f;

            Vector2 offset = new Vector2(1.3f, -0f * player.direction).RotatedBy(rot);
            for (int k = 0; k < 15; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);

                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, new Color(150, 80, 40), Main.rand.NextFloat(0.2f, 0.5f));
            }


            Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
            Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));

            // Rotate the velocity randomly by 30 degrees at max.
            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
            newVelocity *= 1f - Main.rand.NextFloat(0.3f);
            Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<Cinderscrap>());
        }
    }

    public class HornetKaboom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 50;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 50;
            Projectile.height = 76;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 50;
            Projectile.scale = 1.3f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            Main.dust[dust].noGravity = true;

            Vector3 RGB = new(2.55f, 2.55f, 0.94f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
            overWiresUI.Add(index);
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 50)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }
    }
}