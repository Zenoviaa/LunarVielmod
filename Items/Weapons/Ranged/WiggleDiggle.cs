using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Gores;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class WiggleDiggle : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Magic;
        public override void SetClassSwappedDefaults()
        {
            Item.mana = 8;
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.width = 96;
            Item.height = 42;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = 50;
            Item.useTime = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.DD2_KoboldExplosion;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<WiggleDiggleProj>();
            Item.shootSpeed = 19;

        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/ConfettiShot1");
            soundStyle.PitchVariance = 0.3f;
            soundStyle.Volume = 0.8f;
            SoundEngine.PlaySound(soundStyle, position);

            float rot = velocity.ToRotation();
            float spread = 0.4f;
            Vector2 offset = new Vector2(6, -0.1f * player.direction).RotatedBy(rot);

            //Funny Screenshake
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 32f);
            int numProjectiles = Main.rand.Next(8, 15);
            float distance = 12;
            for (int p = 0; p < numProjectiles; p++)
            {
                //Particles and stuff
                Dust.NewDustPerfect(position + offset * distance, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
                Dust.NewDustPerfect(player.Center + offset * distance, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));
                
                //Get a random velocity
                Vector2 startVelocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 2);

                //Get a random
                float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                ParticleManager.NewParticle<StarParticle2>(position + offset * distance, startVelocity * Main.rand.NextFloat(0.5f, 1f), Color.DarkGoldenrod, randScale);

                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
                for(int k = 0; k < Main.rand.Next(2, 7); k++)
                {
                    int[] goreTypes = new int[]
                    {
                        ModContent.GoreType<RibbonBlue>(),
                        ModContent.GoreType<RibbonPink>(),
                        ModContent.GoreType<RibbonWhite>(),
                        ModContent.GoreType<RibbonYellow>()
                    };

                    int goreType = goreTypes[Main.rand.Next(0, goreTypes.Length)];
                    Gore.NewGore(source, position + offset.RotatedByRandom(MathHelper.PiOver4) * distance * Main.rand.NextFloat(0.5f, 1f),
                        newVelocity.RotatedByRandom(MathHelper.PiOver4),
                      goreType);
                }
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Shotgun);
            recipe.AddIngredient(ModContent.ItemType<Teraciz>(), 1);
            recipe.AddIngredient(ModContent.ItemType<IshtarCandle>(), 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
