using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.GunHolster;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged.GunSwapping
{
    internal abstract class MiniGun : ModItem
    {
        public bool LeftHand;
        public bool RightHand;
        public bool TwoHands;
        public int GunHolsterProjectile = -1;
        public int GunHolsterProjectile2 = -1;
        public bool IsSpecial => LeftHand && RightHand;
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "WeaponType", "Gun Holster Weapon Type")
            {
                OverrideColor = ColorFunctions.GunHolsterWeaponType
            };
            tooltips.Add(line);

            if (IsSpecial)
            {
     
                line = new TooltipLine(Mod, "LeftHanded", "Use to equip to your gun holster's left hand!");
                tooltips.Add(line);
                line = new TooltipLine(Mod, "RightHanded", "OR right click to equip to your gun holster's right hand!");
                tooltips.Add(line);
                if (TwoHands)
                {
                    line = new TooltipLine(Mod, "BothHanded", "Can be in both hands at the same time!");
                    tooltips.Add(line);
                }
            }
            else
            {
    
                if (LeftHand)
                {
                    line = new TooltipLine(Mod, "LeftHanded", "Use to equip to your gun holster's left hand!");
                    tooltips.Add(line);
                }
                if (RightHand )
                {
                    line = new TooltipLine(Mod, "RightHanded", "Use to equip to your gun holster's right hand!");
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

        public override bool AltFunctionUse(Player player)
        {
            return IsSpecial;
        }

        public override bool? UseItem(Player player)
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();
            if (IsSpecial)
            {
                if(player.altFunctionUse == 2)
                {
                    if (gunPlayer.LeftHand == Type && !TwoHands)
                        gunPlayer.LeftHand = -1;
                    gunPlayer.RightHand = Type;
                }
                else
                {
                    if (gunPlayer.RightHand == Type && !TwoHands)
                        gunPlayer.RightHand = -1;
                    gunPlayer.LeftHand = Type;
                }
            }
            else
            {
                if (LeftHand)
                {
                    gunPlayer.LeftHand = Type;
                }

                if (RightHand)
                {
                    gunPlayer.RightHand = Type;
                }
            }

            //Remember to code this slightly differently for the special sirestias one that can go on both hands
            //Actually you could just make it right clickable lol, left click for primary hand, right click for secondary hand
            //Don't allow more than one of course

            //Left-Handed Pulsing
            //Left-Handed Eagle
            //L
            //Right-Handed Burn Blast
            //Right-Handed Poison Pistol
            //Right-Handed Cannon
            //Right-Handed Rocket Launcher
            return base.UseItem(player);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D iconTexture = null;
            const string Base_Path = "Stellamod/Items/Weapons/Ranged/GunSwapping/";

            if (IsSpecial)
            {
                iconTexture = ModContent.Request<Texture2D>($"{Base_Path}LR").Value;
            } else if(LeftHand)
            {
                iconTexture = ModContent.Request<Texture2D>($"{Base_Path}L").Value;
            } else if(RightHand)
            {
                iconTexture = ModContent.Request<Texture2D>($"{Base_Path}R").Value;
            }
            Vector2 drawOrigin = iconTexture.Size() / 2;
            Vector2 drawPosition = position - new Vector2(drawOrigin.X, -drawOrigin.Y);
            spriteBatch.Draw(iconTexture, drawPosition, null, drawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0);
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
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterPulsingProj>();
           
        }
    }

    internal class Eagle : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Setting this to width and height of the texture cause idk
            Item.damage = 24;
            Item.width = 56;
            Item.height = 30;

            LeftHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterEagleProj>();

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;
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
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterBurnBlastProj>();
        }       
    }

    internal class PoisonPistol : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 70;
            RightHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterPoisonPistolProj>();
        }
    }

    internal class RocketLauncher : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 50;
            Item.value = Item.buyPrice(gold: 15);
            RightHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterRocketLauncherProj>();
        }
    }

    internal class MintyBlast : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 10;
            RightHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterMsFreezeProj>();


            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/HarmonicBlasphemy1");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.Anvils);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 1);
            recipe.AddIngredient(ModContent.ItemType<WinterbornShard>(), 9);

            recipe.Register();
        }
    }

    internal class MsFreeze : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 52;
            LeftHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterMintyBlastProj>();

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunLaser");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;
        }
    }

    internal class Electrifying : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 16;
            LeftHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterElectrifyingProj>();

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunElectric");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;
        }
    }

    internal class  RavestBlast : MiniGun
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
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterRavestBlastLeftProj>();
            GunHolsterProjectile2 = ModContent.ProjectileType<GunHolsterRavestBlastRightProj>();


            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunRaving");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;
        }
    }

    internal class STARBUST : MiniGun
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 42;
            LeftHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterSTARBUSTProj>();
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
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterDevolverProj>();
        }
    }


    internal class CinderNeedle : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 20;
            LeftHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterCinderNeedleProj>();

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Gunsotp");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;
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
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterShottyPitolProj>();
        }
    }

    internal class BubbleBussy : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 50;
            RightHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterBubbleBussyProj>();

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/TentacleBubbleOut");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 15);
            recipe.AddIngredient(ModContent.ItemType<ConvulgingMater>(), 30);
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 9);
            recipe.AddIngredient(ModContent.ItemType<AlcadizMetal>(), 9);
            recipe.AddIngredient(ModContent.ItemType<WickofSorcery>(), 1);
            recipe.Register();
        }
    }

    internal class AssassinsRecharge : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 92;
            RightHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterAssassinsRechargeProj>();
        }
    }

    internal class CarrotPatrol : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 82;
            LeftHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterCarrotPatrolProj>();
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
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterMeredaXLeftProj>();
            GunHolsterProjectile2 = ModContent.ProjectileType<GunHolsterMeredaXRightProj>();



            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/GunBlasting");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;
        }
    }

    internal class SrTetanus : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 36;
            RightHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterSrTetanusProj>();
        }
    }

    internal class TheReaving : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 136;
            LeftHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterTheReavingProj>();
        }
    }

    internal class AzureWrath : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 49;
            RightHand = true;
            GunHolsterProjectile = ModContent.ProjectileType<GunHolsterAzureWrathProj>();

            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/TON618");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;
        }
    }
}
