using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;

using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Stellamod.Projectiles.Gun;
using Stellamod.Projectiles.GunHolster;
using Stellamod.Projectiles.Steins;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Items.Ores;

namespace Stellamod.Items.Weapons.Ranged.GunSwapping
{
    internal abstract class MiniGun : ModItem
    {
        public bool LeftHand;
        public bool RightHand;
        public bool TwoHands;
        public bool IsSpecial => LeftHand && RightHand;

        public string HeldTexture => Texture + "_Held";


        public float AttackSpeed = 10;
        public Vector2 HolsterOffset;

        public float RecoilRotation = MathHelper.PiOver4;
        public float RecoilDistance = 5;
        public float RecoilRotationMini = MathHelper.ToRadians(15);
        public float ShootCount;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
            Item.shootSpeed = 10f; // The speed of the projectile (measured in pixels per frame.) This value equivalent to Handgun
            Item.useAmmo = AmmoID.Bullet; //
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "WeaponType", "Gun Holster Weapon Type")
            {
                OverrideColor = ColorFunctions.GunHolsterWeaponType
            };
            tooltips.Add(line);


            Color tooltipColor = Color.Gray;
            if (IsSpecial)
            {

                line = new TooltipLine(Mod, "LeftHanded", "Equip to your gun holster's or scorpion's left hand!");
                line.OverrideColor = tooltipColor;
                tooltips.Add(line);
                line = new TooltipLine(Mod, "RightHanded", "OR equip to your gun holster's or scorpion's right hand!");
                line.OverrideColor = tooltipColor;
                tooltips.Add(line);
                if (TwoHands)
                {
                    line = new TooltipLine(Mod, "BothHanded", "Can be in both hands at the same time for both your scorpion and gun holster!");
                    line.OverrideColor = tooltipColor;
                    tooltips.Add(line);
                }
            }
            else
            {

                if (LeftHand)
                {
                    line = new TooltipLine(Mod, "LeftHanded", "Equip to your gun holsters' or scorpions' left hand!");
                    line.OverrideColor = tooltipColor;
                    tooltips.Add(line);
                }
                if (RightHand)
                {
                    line = new TooltipLine(Mod, "RightHanded", "Equip to your gun holsters' or scorpions' right hand!");
                    line.OverrideColor = tooltipColor;
                    tooltips.Add(line);
                }
            }

            if (!Main.LocalPlayer.HasItemInAnyInventory(ModContent.ItemType<GunHolster>()))
            {
                line = new TooltipLine(Mod, "WompWomp", "You do not have a Gun Holster...")
                {
                    OverrideColor = Color.Gray
                };
                tooltips.Add(line);

                line = new TooltipLine(Mod, "WompWomp2", "Buy one from Delgrim!")
                {
                    OverrideColor = Color.Gray
                };
                tooltips.Add(line);
            }

        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D iconTexture = null;
            const string Base_Path = "Stellamod/Items/Weapons/Ranged/GunSwapping/";

            if (IsSpecial)
            {
                iconTexture = ModContent.Request<Texture2D>($"{Base_Path}LR").Value;
            }
            else if (LeftHand)
            {
                iconTexture = ModContent.Request<Texture2D>($"{Base_Path}L").Value;
            }
            else if (RightHand)
            {
                iconTexture = ModContent.Request<Texture2D>($"{Base_Path}R").Value;
            }
            Vector2 drawOrigin = iconTexture.Size() / 2;
            Vector2 drawPosition = position - new Vector2(drawOrigin.X, -drawOrigin.Y);
            spriteBatch.Draw(iconTexture, drawPosition, null, drawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0);
        }


        public virtual void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            for (int i = 0; i < 1; i++)
            {
                Gore.NewGore(player.GetSource_FromThis(), position, velocity * -3,
                    ModContent.GoreType<BulletCasing>());
            }

        }
    }

    internal class Pulsing : MiniGun
    {
        //Damage of this gun holster

        public override void SetDefaults()
        {
            base.SetDefaults();

            //Setting this to width and height of the texture cause idk
            Item.damage = 15;
            Item.width = 56;
            Item.height = 30;
            Item.value = Item.buyPrice(gold: 5);
            LeftHand = true;
            RightHand = true;

            //Higher is faster
            AttackSpeed = 12;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
            {
                float spread = 0.4f;
                for (int k = 0; k < 7; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
                }
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
                if (Main.myPlayer == player.whoAmI)
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, projToShoot, damage, knockback, player.whoAmI);
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol"), position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3"), position);
                }
            }
        }
    }

    internal class Eagle : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Setting this to width and height of the texture cause idk
            Item.damage = 9;
            Item.width = 56;
            Item.height = 30;

            LeftHand = true;

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;


            //Higher is faster
            AttackSpeed = 12;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
            {
                //Treat this like a normal shoot function
                float spread = 0.4f;
                for (int k = 0; k < 7; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
                }
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
                if (Main.myPlayer == player.whoAmI)
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, projToShoot, damage, knockback, player.whoAmI);
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol"), position);
                }
                else
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3");
                    soundStyle.PitchVariance = 0.5f;
                    SoundEngine.PlaySound(soundStyle, position);
                }
            }
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<GintzlMetal>());
        }
    }

    internal class BurnBlast : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 50;

            //Setting this to width and height of the texture cause idk
            Item.width = 62;
            Item.height = 38;

            RightHand = true;

            //This number is in ticks
            AttackSpeed = 60;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 14; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Orange, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 16,
                ModContent.ProjectileType<BurnBlastProj>(), damage, knockback, player.whoAmI);
            }

            player.GetModPlayer<MyPlayer>().ShakeAtPosition(position, 1024f, 16f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), position);
        }
    }

    internal class PoisonPistol : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 70;
            RightHand = true;

            //This number is in ticks
            AttackSpeed = 60;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 14; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Purple, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkViolet, 1);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 16,
                ModContent.ProjectileType<SSShot>(), damage, knockback, player.whoAmI);
            }

            player.GetModPlayer<MyPlayer>().ShakeAtPosition(position, 1024f, 5f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), position);
        }
    }

    internal class RocketLauncher : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 50;
            Item.value = Item.buyPrice(gold: 15);
            LeftHand = true;
            RightHand = true;
            TwoHands = true;
            //This number is in ticks
            AttackSpeed = 60;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);

            float spread = 0.4f;
            for (int k = 0; k < 14; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Orange, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);

            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 16,
                ProjectileID.RocketI, damage, knockback, player.whoAmI);
            }
            player.GetModPlayer<MyPlayer>().ShakeAtPosition(position, 1024f, 16f);
            SoundEngine.PlaySound(SoundID.Item11, position);
        }
    }

    internal class MintyBlast : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 10;
            RightHand = true;

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/HarmonicBlasphemy1");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //Higher is slower
            AttackSpeed = 3;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 0;
            RecoilRotation = 0;
            RecoilRotationMini = 0;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (!player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
                return;
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.DarkBlue, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.Blue, 1);
            projToShoot = Main.rand.Next(new int[] { ModContent.ProjectileType<FroBall2>(), ModContent.ProjectileType<FroBall1>() });

            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, projToShoot, damage, knockback, player.whoAmI);
            }

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/HarmonicBlasphemy1");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, position);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<WinterbornShard>());
        }
    }

    internal class MsFreeze : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 14;
            LeftHand = true;

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunLaser");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //This number is in ticks
            AttackSpeed = 2;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            float spread = 0.4f;
            for (int k = 0; k < 4; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.LightCyan, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.White, 1);
            for (int i = 0; i < Main.rand.Next(2, 5); i++)
            {
                Vector2 vel = velocity * 16;
                vel = vel.RotatedByRandom(MathHelper.PiOver4 / 3);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity,
                    ModContent.ProjectileType<MintyBlastProj>(), damage, knockback, player.whoAmI);
                }
            }

            player.GetModPlayer<MyPlayer>().ShakeAtPosition(position, 1024f, 2f);

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunLaser");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, position);
        }
    }


    internal class Piken : MiniGun
    {
        private int _comboCounter;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 50;
            LeftHand = true;

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew1");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //This number is in ticks
            AttackSpeed = 30;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (!player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
                return;
            float rot = velocity.ToRotation();
            float spread = 0.4f;

            Vector2 offset = new Vector2(1.5f, -0.1f * player.direction).RotatedBy(rot);

            _comboCounter++;
            if (_comboCounter > 100)
            {
                for (int k = 0; k < 7; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.IndianRed, Main.rand.NextFloat(0.2f, 0.8f));
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol2"));
                AttackSpeed = 30;
                _comboCounter = 0;
            }
            if (_comboCounter > 75)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.TSmokeDust>(), new Vector2(0, 0) + offset.RotatedByRandom(spread), 150, Color.IndianRed * 0.5f, Main.rand.NextFloat(0.5f, 1));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.White, 1);
            if (AttackSpeed > 2)
            {
                AttackSpeed--;

            }

            for (int p = 0; p < 1; p++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 14, projToShoot, damage, knockback, player.whoAmI);
                }
            }

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 8f);
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GunShootNew6"));
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GunShootNew6"));
            }

            //Dust Burst Towards Mouse


            for (int k = 0; k < 7; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);


                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, new Color(180, 50, 40), Main.rand.NextFloat(0.2f, 0.5f));
            }

            Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
            Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));
        }
    }

    internal class Electrifying : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 16;
            LeftHand = true;
            RightHand = true;
            TwoHands = true;

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunElectric");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //Higher is faster
            AttackSpeed = 4;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
            {
                float spread = 0.4f;
                for (int k = 0; k < 4; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.LightCyan, Main.rand.NextFloat(0.2f, 0.5f));
                }
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.LightCyan, 1);
                int numProjectiles = Main.rand.Next(1, 2);
                for (int p = 0; p < numProjectiles; p++)
                {
                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 vel = velocity * 8;
                    Vector2 newVelocity = vel.RotatedByRandom(MathHelper.ToRadians(6));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectileDirect(player.GetSource_FromThis(), position, newVelocity,
                        ModContent.ProjectileType<ElectrifyingProj>(), damage, knockback, player.whoAmI);
                    }
                }

                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew9");
                soundStyle.PitchVariance = 0.5f;
                SoundEngine.PlaySound(soundStyle, position);
            }
        }
    }

    internal class RavestBlast : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Setting this to width and height of the texture cause idk
            Item.damage = 202;
            Item.width = 56;
            Item.height = 30;
            Item.rare = ModContent.RarityType<SirestiasSpecialRarity>();
            LeftHand = true;
            RightHand = true;

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunRaving");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;


            //Higher is faster
            AttackSpeed = 39;
            ShootCount = 3;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
            {
                float spread = 0.4f;
                for (int k = 0; k < 7; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
                }
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, ModContent.ProjectileType<RavestblastProj>(), damage, knockback, player.whoAmI);
                }
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunRaving");
                soundStyle.PitchVariance = 0.5f;
                SoundEngine.PlaySound(soundStyle, position);
            }
        }
    }

    internal class STARBUST : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 42;
            LeftHand = true;

            //Higher is faster
            AttackSpeed = 1;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
            Vector2 vel = velocity * 16;
            vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                ModContent.ProjectileType<STARBULLING>(), damage, knockback, player.whoAmI);
            }
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol"), position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3"), position);
            }
        }
    }


    internal class Devolver : MiniGun
    {
        //Damage of this gun
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 207;
            LeftHand = true;

            //Higher is faster
            AttackSpeed = 45;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;

        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            float spread = 0.4f;
            for (int k = 0; k < 20; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.OrangeRed, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, ProjectileID.Bullet,
                damage, knockback, player.whoAmI);
            }
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol"), position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3"), position);
            }
        }
    }


    internal class CinderNeedle : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 20;
            LeftHand = true;

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Gunsotp");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //Higher is faster
            AttackSpeed = 6;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Vector2 vel = velocity * 16;
                vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                    ModContent.ProjectileType<CinderNeedleProj>(), damage, knockback, player.whoAmI);
                }
            }

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Gunsotp");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, position);
        }
    }

    internal class Drygan : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 90;
            LeftHand = true;
            RightHand = true;
            TwoHands = true;

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew5");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //Higher is faster
            AttackSpeed = 15;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 4;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.LightGoldenrodYellow, 1);

            Vector2 vel = velocity * 16;
            vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                    ModContent.ProjectileType<DryganProj>(), damage, knockback, player.whoAmI);
            }

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew5");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, position);
        }
    }


    internal class Obel : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 40;
            RightHand = true;


            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew10");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //Higher is faster
            AttackSpeed = 19;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 4;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.AliceBlue, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.LightGoldenrodYellow, 1);

            Vector2 vel = velocity * 16;
            vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                    ModContent.ProjectileType<EnergyBall>(), damage, knockback, player.whoAmI);
            }

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew10");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, position);
        }
    }

    internal class Piranha : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 4;
            RightHand = true;


            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew4");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //Higher is faster
            AttackSpeed = 24;
            ShootCount = 6;
            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Vector2 vel = velocity * 16;
                vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                    ModContent.ProjectileType<PiranhaProj>(), damage, knockback, player.whoAmI);
                }
            }

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew4");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, position);
        }
    }

    internal class ShottyPitol : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 9;
            Item.value = Item.buyPrice(gold: 15);
            RightHand = true;

            //This number is in ticks
            AttackSpeed = 60;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
            {
                //Treat this like a normal shoot function
                float spread = 0.4f;
                for (int k = 0; k < 14; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.White, Main.rand.NextFloat(0.4f, 0.8f));
                }

                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.White, 1);
                for (int i = 0; i < Main.rand.Next(3, 7); i++)
                {
                    Vector2 vel = velocity * 16;
                    vel = vel.RotatedByRandom(MathHelper.PiOver4 / 2);
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                        projToShoot, damage, knockback, player.whoAmI);
                    }
                }

                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(position, 1024f, 16f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), position);
            }
        }
    }

    internal class BubbleBussy : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 50;
            RightHand = true;

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/TentacleBubbleOut");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //This number is in ticks
            AttackSpeed = 8;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            float spread = 0.4f;
            for (int k = 0; k < 14; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.DarkBlue, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkBlue, 1);
            Vector2 vel = velocity * 12;
            vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                ModContent.ProjectileType<BubbleBussyProj>(), damage, knockback, player.whoAmI);
            }
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/TentacleBubbleOut");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, position);
        }
    }

    internal class BasterParty : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 120;
            RightHand = true;
            LeftHand = true;
            TwoHands = true;

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/TentacleBubbleOut");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //This number is in ticks
            AttackSpeed = 5;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew11");
            soundStyle.PitchVariance = 0.3f;
            soundStyle.Volume = 0.8f;
            SoundEngine.PlaySound(soundStyle, position);

            float rot = velocity.ToRotation();
            float spread = 0.4f;
            Vector2 offset = new Vector2(2, -0.1f * player.direction).RotatedBy(rot);
            Vector2 newDirection = velocity.RotatedByRandom(spread);

            //Funny Screenshake
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 5f);
            int numProjectiles = Main.rand.Next(1, 3);
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

                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), position, newDirection * 2 * Main.rand.NextFloat(12), ModContent.ProjectileType<BasterPartyProj>(), damage, knockback, player.whoAmI);
                for (int k = 0; k < Main.rand.Next(1, 3); k++)
                {
                    int[] goreTypes = new int[]
                    {
                        ModContent.GoreType<RibbonBlue>(),
                        ModContent.GoreType<RibbonPink>(),
                        ModContent.GoreType<RibbonWhite>(),
                        ModContent.GoreType<RibbonYellow>()
                    };

                    int goreType = goreTypes[Main.rand.Next(0, goreTypes.Length)];
                    Gore.NewGore(player.GetSource_FromThis(), position + offset.RotatedByRandom(MathHelper.PiOver4) * distance * Main.rand.NextFloat(0.5f, 1f),
                        newVelocity.RotatedByRandom(MathHelper.PiOver4),
                      goreType);
                }
            }
        }
    }
        internal class AssassinsRecharge : MiniGun
        {
            public override void SetDefaults()
            {
                base.SetDefaults();
                Item.damage = 92;
                RightHand = true;

                //This number is in ticks
                AttackSpeed = 60;

                //Offset it so it doesn't hold gun by weird spot
                HolsterOffset = new Vector2(0, -6);
            }

            public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
            {
                base.Fire(player, position, velocity, damage, knockback);
                float spread = 0.4f;
                for (int k = 0; k < 14; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Orange, Main.rand.NextFloat(0.4f, 0.8f));
                }

                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 16,
                    ModContent.ProjectileType<AssassinsRechargeShot>(), damage, knockback, player.whoAmI);
                }
                player.GetModPlayer<MyPlayer>().ShakeAtPosition(position, 1024f, 16f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), position);
            }

            public override void AddRecipes()
            {
                base.AddRecipes();
                this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<TerrorFragments>());
            }
        }


        internal class CarrotPatrol : MiniGun
        {
            public override void SetDefaults()
            {
                base.SetDefaults();
                Item.damage = 82;
                LeftHand = true;

                //Higher is faster
                AttackSpeed = 7;

                //Offset it so it doesn't hold gun by weird spot
                HolsterOffset = new Vector2(15, -6);

                //Recoil
                RecoilDistance = 3;
            }

            public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
            {
                base.Fire(player, position, velocity, damage, knockback);
                //Treat this like a normal shoot function
                float spread = 0.4f;
                for (int k = 0; k < 7; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.DarkOrange, Main.rand.NextFloat(0.2f, 0.5f));
                }
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkOrange, 1);
                int projectileType = ModContent.ProjectileType<CarrotPatrolProj>();
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 16, projectileType, damage, knockback, player.whoAmI);
                }

                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol"), position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3"), position);
                }
            }

        }

        internal class MeredaX : MiniGun
        {
            public override void SetDefaults()
            {
                base.SetDefaults();
                Item.damage = 86;
                LeftHand = true;
                RightHand = true;
                TwoHands = true;

                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunBlasting");
                soundStyle.PitchVariance = 0.5f;
                Item.UseSound = soundStyle;

                //Higher is faster
                AttackSpeed = 5;

                //Offset it so it doesn't hold gun by weird spot
                HolsterOffset = new Vector2(15, -6);

                //Recoil
                RecoilDistance = 3;
            }

            public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
            {
                base.Fire(player, position, velocity, damage, knockback);
                if (player.HeldItem.ModItem is not GunHolster gunHolster)
                    return;

                if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
                {
                    if (gunHolster.HeldLeftHandGun == this)
                    {
                        //Treat this like a normal shoot function
                        float spread = 0.4f;
                        for (int k = 0; k < 7; k++)
                        {
                            Vector2 newDirection = velocity.RotatedByRandom(spread);
                            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Black, Main.rand.NextFloat(0.2f, 0.5f));
                        }
                        Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.Black, 1);
                        if (Main.myPlayer == player.whoAmI)
                        {
                            Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, ModContent.ProjectileType<PINKX>(), damage, knockback, player.whoAmI);
                        }
                        SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunBlasting");
                        soundStyle.PitchVariance = 0.5f;
                        SoundEngine.PlaySound(soundStyle);
                    }
                    else
                    {
                        float spread = 0.4f;
                        for (int k = 0; k < 7; k++)
                        {
                            Vector2 newDirection = velocity.RotatedByRandom(spread);
                            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Black, Main.rand.NextFloat(0.2f, 0.5f));
                        }
                        Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.Black, 1);
                        if (Main.myPlayer == player.whoAmI)
                        {
                            Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, ModContent.ProjectileType<BLACKX>(), damage, knockBack, player.whoAmI);
                        }

                        SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunBlasting");
                        soundStyle.PitchVariance = 0.5f;
                        SoundEngine.PlaySound(soundStyle);
                    }
                }
            }
        }

        internal class Gordon : MiniGun
        {
            public override void SetDefaults()
            {
                base.SetDefaults();
                Item.damage = 21;
                LeftHand = true;
                RightHand = true;
                TwoHands = true;

                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunBlasting");
                soundStyle.PitchVariance = 0.5f;
                Item.UseSound = soundStyle;


                //Higher is faster
                AttackSpeed = 10;

                //Offset it so it doesn't hold gun by weird spot
                HolsterOffset = new Vector2(15, -6);
                //  ShootCount = 1;
                //Recoil
                RecoilDistance = 3;
            }

            public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
            {
                base.Fire(player, position, velocity, damage, knockback);
                if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
                {
                    //Treat this like a normal shoot function
                    float spread = 0.4f;
                    for (int k = 0; k < 7; k++)
                    {
                        Vector2 newDirection = velocity.RotatedByRandom(spread);
                        Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Turquoise, Main.rand.NextFloat(0.2f, 0.5f));
                    }
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.Turquoise, 1);
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, ModContent.ProjectileType<NLUX>(), damage, knockback, player.whoAmI);
                    }
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunBlasting");
                    soundStyle.PitchVariance = 0.5f;
                    SoundEngine.PlaySound(soundStyle);
                }
            }
        }

        internal class Rhino : MiniGun
        {
            public override void SetDefaults()
            {
                base.SetDefaults();
                Item.damage = 8;
                LeftHand = true;
                RightHand = true;
                TwoHands = true;

                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunBlasting");
                soundStyle.PitchVariance = 0.5f;
                Item.UseSound = soundStyle;

                //Higher is faster
                AttackSpeed = 4;

                //Offset it so it doesn't hold gun by weird spot
                HolsterOffset = new Vector2(15, -6);
                ShootCount = 2;
                //Recoil
                RecoilDistance = 3;
            }

            public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
            {
                base.Fire(player, position, velocity, damage, knockback);
                if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
                {
                    float spread = 0.4f;
                    for (int k = 0; k < 7; k++)
                    {
                        Vector2 newDirection = velocity.RotatedByRandom(spread);
                        Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
                    }
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.Red, 1);
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, projToShoot, damage, knockBack, player.whoAmI);
                    }

                    int Sound = Main.rand.Next(1, 3);
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol"), position);
                    }
                    else
                    {
                        SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3");
                        soundStyle.PitchVariance = 0.5f;
                        SoundEngine.PlaySound(soundStyle, position);
                    }
                }
            }
        }

        internal class SrTetanus : MiniGun
        {
            public override void SetDefaults()
            {
                base.SetDefaults();
                Item.damage = 36;
                RightHand = true;

                //This number is in ticks
                AttackSpeed = 20;
                ShootCount = 2;

                //Offset it so it doesn't hold gun by weird spot
                HolsterOffset = new Vector2(0, -6);
            }

            public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
            {
                base.Fire(player, position, velocity, damage, knockback);
                //Treat this like a normal shoot function
                float spread = 0.4f;
                for (int k = 0; k < 14; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.DarkGreen, Main.rand.NextFloat(0.4f, 0.8f));
                }

                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkGreen, 1);
                Vector2 vel = velocity * 16;
                vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                    ModContent.ProjectileType<SrTetanusProj>(), damage, knockback, player.whoAmI);
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), position);
            }
        }

        internal class TheReaving : MiniGun
        {
            public override void SetDefaults()
            {
                base.SetDefaults();
                Item.damage = 136;
                LeftHand = true;

                //This number is in ticks
                AttackSpeed = 120;

                //Offset it so it doesn't hold gun by weird spot
                HolsterOffset = new Vector2(0, -6);
            }

            public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
            {
                base.Fire(player, position, velocity, damage, knockback);
                if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
                {
                    //Treat this like a normal shoot function
                    float spread = 0.4f;
                    for (int k = 0; k < 14; k++)
                    {
                        Vector2 newDirection = velocity.RotatedByRandom(spread);
                        Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.LightGoldenrodYellow, Main.rand.NextFloat(0.4f, 0.8f));
                    }

                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, projToShoot, damage, knockback, player.whoAmI);
                    }
                    player.GetModPlayer<MyPlayer>().ShakeAtPosition(position, 1024f, 16f);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), position);


                    float rot = velocity.ToRotation();
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3"), position);

                    Vector2 offset = new Vector2(2, -0.1f * player.direction).RotatedBy(rot);
                    for (int k = 0; k < 15; k++)
                    {
                        Vector2 direction2 = offset.RotatedByRandom(spread);

                        Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction2 * Main.rand.NextFloat(8), 125, new Color(150, 80, 40), Main.rand.NextFloat(0.2f, 0.5f));
                    }


                    int numProjectiles = Main.rand.Next(10, 30);
                    for (int p = 0; p < numProjectiles; p++)
                    {


                        Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
                        Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));



                        // Rotate the velocity randomly by 30 degrees at max.
                        Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(25));
                        newVelocity *= 1f - Main.rand.NextFloat(0.3f);


                        Projectile.NewProjectile(player.GetSource_FromThis(), position, newVelocity * 12, projToShoot, damage, knockback, player.whoAmI);
                    }
                }
            }
        }

        internal class AzureWrath : MiniGun
        {
            public override void SetDefaults()
            {
                base.SetDefaults();
                Item.damage = 49;
                RightHand = true;

                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/TON618");
                soundStyle.PitchVariance = 0.5f;
                Item.UseSound = soundStyle;


                //Higher is faster
                AttackSpeed = 1;

                //Offset it so it doesn't hold gun by weird spot
                HolsterOffset = new Vector2(15, -6);

                //Recoil
                RecoilDistance = 1;
            }

            public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
            {
                base.Fire(player, position, velocity, damage, knockback);
                //Treat this like a normal shoot function
                float spread = 0.4f;
                for (int k = 0; k < 7; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.MediumPurple, Main.rand.NextFloat(0.2f, 0.5f));
                }
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.BlueViolet, 1);

                Vector2 vel = velocity * 16;
                vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity, ModContent.ProjectileType<AzurewrathProj>(), damage, knockback, player.whoAmI);
                }
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/TON618");
                soundStyle.PitchVariance = 0.5f;
                SoundEngine.PlaySound(soundStyle, position);
            }
        }
    }

