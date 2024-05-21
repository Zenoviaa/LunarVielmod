using Microsoft.Xna.Framework;
using Stellamod.Buffs.Charms;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Brooches
{
    internal class BroochPlayer : ModPlayer
    {
        public bool hasAmethystBrooch;
        public bool hasAurelusBlightBrooch;
        public bool hasBearBrooch;
        public bool hasBurningGBrooch;
        public int burningGBCooldown;
        public bool hasDiariBrooch;
        public bool hasDreadBrooch;
        public bool hasFlyfishBrooch;
        public bool hasFrileBrooch;
        public int frileBroochCooldown;
        public bool hasGintzlBrooch;
        public bool hasGovheilHolsterBrooch;
        public bool hasMalShieldBrooch;
        public bool hasMorrowedBrooch;
        public bool hasSlimeBrooch;
        public bool hasSpragaldBrooch;
        public bool hasStoneBrooch;
        public bool hasVerliaBrooch;
        public int verliaBroochCooldown;
        public bool hasVixedBrooch;
        public bool hasLuckyWBrooch;
        public bool hasEyeBrooch;
        public bool hasBonedBrooch;
        public bool hasJellyBrooch;
        public bool hasSandyBrooch;
        public bool hasCelestia;
        public bool hasRoseBrooch;
        public bool hasMagicalBrooch;
        public bool hasRadiantBrooches;
        public bool hasAdvancedBrooches;
        public bool hasIgniteron;
        public bool hasVillagersBrooch;
        public bool hasAmberBrooch;
        public bool hasWoodyBrooch;


        public bool hasHeatVer;

        public bool hasDaedDredRose;

        public bool hasSandyBear;

        public bool hasGinzteling;

        public bool hasStonefly;

        public bool hasAurock;

        public bool hasDefensiveJS;
        public bool hasIllurianBrooch;

        public override void ResetEffects()
        {
            base.ResetEffects();
            hasAmethystBrooch = false;
            hasAurelusBlightBrooch = false;
            hasBearBrooch = false;
            hasBurningGBrooch = false;
            hasDiariBrooch = false;
            hasDreadBrooch = false;
            hasFlyfishBrooch = false;
            hasFrileBrooch = false;
            hasGintzlBrooch = false;
            hasGovheilHolsterBrooch = false;
            hasMalShieldBrooch = false;
            hasMorrowedBrooch = false;
            hasSpragaldBrooch = false;
            hasStoneBrooch = false;
            hasVerliaBrooch = false;
            hasVixedBrooch = false;
            hasCelestia = false;
            hasSlimeBrooch = false;
            hasLuckyWBrooch = false;
            hasEyeBrooch = false;
            hasBonedBrooch = false;
            hasJellyBrooch = false;
            hasSandyBrooch = false;
            hasRoseBrooch = false;
            hasMagicalBrooch = false;
            hasAdvancedBrooches = false;
            hasIgniteron = false;
            hasVillagersBrooch = false;
            hasAmberBrooch = false;
            hasWoodyBrooch = false;


            hasHeatVer = false;
            hasGinzteling = false;
            hasAurock = false;
            hasSandyBear = false;
            hasDefensiveJS = false;
            hasStonefly = false;
            hasDaedDredRose = false;
            hasRadiantBrooches = false;
            hasIllurianBrooch = false;
        }


        private void AdvancedBroochEffects()
        {
            //Advanced Brooch Effects
            if (hasAdvancedBrooches && !hasRadiantBrooches)
            {
                MyPlayer myPlayer = Player.GetModPlayer<MyPlayer>();//.LuckyW = true;
                //Lucky Winner Brooch
                if (hasLuckyWBrooch)
                {
                    KeepBroochAlive<LuckyWinnerBrooch, LuckyB>(ref hasLuckyWBrooch);
                    myPlayer.LuckyW = true;
                }

                //Boned Throw Brooch
                if (hasBonedBrooch)
                {
                    KeepBroochAlive<BonedBrooch, BonedB>(ref hasBonedBrooch);
                    Player.GetDamage(DamageClass.Throwing) *= 1.2f;
                    Player.ThrownVelocity += 5;
                }

                //Burning GB Brooch
                if (hasBurningGBrooch)
                {
                    KeepBroochAlive<BurningGBrooch, BurningGB>(ref hasBurningGBrooch);
                    burningGBCooldown--;
                }

                if (hasGovheilHolsterBrooch)
                {
                    KeepBroochAlive<GovheilHolsterBrooch, GovheilB>(ref hasGovheilHolsterBrooch);
                }

                if (hasMagicalBrooch)
                {
                    KeepBroochAlive<MagicalBrooch, MagicalBroo>(ref hasMagicalBrooch);
                    Player.GetDamage(DamageClass.Magic) *= 1.2f;
                }

                if (hasVillagersBrooch)
                {
                    KeepBroochAlive<VillagersBrooch, VillagersB>(ref hasVillagersBrooch);
                    Player.GetDamage(DamageClass.Generic) *= 1.12f;
                }

                if (hasWoodyBrooch)
                {
                    KeepBroochAlive<WoodyBrooch, WoodyB>(ref hasWoodyBrooch);
                }


            }







            if (hasRadiantBrooches)
           {
                MyPlayer myPlayer = Player.GetModPlayer<MyPlayer>();


                if (hasWoodyBrooch)
                {
                    KeepBroochAlive<WoodyBrooch, WoodyB>(ref hasWoodyBrooch);
                }

                if (hasLuckyWBrooch)
                {
                    KeepBroochAlive<LuckyWinnerBrooch, LuckyB>(ref hasLuckyWBrooch);
                    myPlayer.LuckyW = true;
                }

                //Boned Throw Brooch
                if (hasBonedBrooch)
                {
                    KeepBroochAlive<BonedBrooch, BonedB>(ref hasBonedBrooch);
                    Player.GetDamage(DamageClass.Throwing) *= 1.2f;
                    Player.ThrownVelocity += 5;
                }

                //Burning GB Brooch
                if (hasBurningGBrooch)
                {
                    KeepBroochAlive<BurningGBrooch, BurningGB>(ref hasBurningGBrooch);
                    burningGBCooldown--;
                }

                if (hasGovheilHolsterBrooch)
                {
                    KeepBroochAlive<GovheilHolsterBrooch, GovheilB>(ref hasGovheilHolsterBrooch);
                }

                if (hasMagicalBrooch)
                {
                    KeepBroochAlive<MagicalBrooch, MagicalBroo>(ref hasMagicalBrooch);
                    Player.GetDamage(DamageClass.Magic) *= 1.2f;
                }

                if (hasVillagersBrooch)
                {
                    KeepBroochAlive<VillagersBrooch, VillagersB>(ref hasVillagersBrooch);
                    Player.GetDamage(DamageClass.Generic) *= 1.12f;
                }

                //___------------------------------------------- RADIANT BROOCHES

             
                          //Lucky Winner Brooch
                    if (hasHeatVer)
                    {
                        KeepBroochAlive<BurningGBrooch, BurningGB>(ref hasBurningGBrooch);
                        burningGBCooldown--;
                        KeepBroochAlive<VerliaBrooch, VerliaBroo>(ref hasVerliaBrooch);
                        verliaBroochCooldown--;

                         Player.GetDamage(DamageClass.Ranged) *= 1.15f;
                         Player.GetDamage(DamageClass.Throwing) *= 1.15f;
                }


                    if (hasStonefly)
                    {
                        KeepBroochAlive<FlyfishBrooch, Flyfish>(ref hasFlyfishBrooch);
                        KeepBroochAlive<SlimeBrooch, Slimee>(ref hasSlimeBrooch);
                        KeepBroochAlive<StoneBrooch, StoneB>(ref hasStoneBrooch);

                    // the stone brooch nerfs it remember
                          Player.GetDamage(DamageClass.Generic) *= 1.25f;
                     }


                if (hasAurock)
                {
                    KeepBroochAlive<AmberBrooch, AmberB>(ref hasAmberBrooch);
                    KeepBroochAlive<AmethystBrooch, AmethystBroo>(ref hasAmethystBrooch);
                    KeepBroochAlive<AurelusBlightBrooch, AurelusB>(ref hasAurelusBlightBrooch);


                }


                if (hasSandyBear)
                {
                    KeepBroochAlive<SandyBrooch, SandyB>(ref hasSandyBrooch);
                    KeepBroochAlive<BearBrooch, BearB>(ref hasBearBrooch);

                    Player.maxMinions += 2;

                    Player.GetDamage(DamageClass.Summon) *= 1.20f;
                }

                if (hasDefensiveJS)
                {
                    KeepBroochAlive<JellyBrooch, JellyB>(ref hasJellyBrooch);
                    KeepBroochAlive<SpragaldBrooch, Spragald>(ref hasSpragaldBrooch);

                    Player.GetDamage(DamageClass.Generic) *= 0.70f;
                }

                if (hasDaedDredRose)
                {
                    KeepBroochAlive<VixedBrooch, VixedB>(ref hasVixedBrooch);
                    KeepBroochAlive<RoseBrooch, RoseB>(ref hasRoseBrooch);
                    KeepBroochAlive<DreadBrooch, DreadB>(ref hasDreadBrooch);
                   
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<RoseShield>()] == 0)
                    {
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * -1f,
                            ModContent.ProjectileType<RoseShield>(), 0, 1f, Player.whoAmI);
                    }

                }

                if (hasGinzteling)
                {
                    KeepBroochAlive<GovheilHolsterBrooch, GovheilB>(ref hasGovheilHolsterBrooch);
                    KeepBroochAlive<GintzlBrooch, GintBroo>(ref hasGintzlBrooch);
                    KeepBroochAlive<VillagersBrooch, VillagersB>(ref hasVillagersBrooch);
                   
                }

                if (hasIllurianBrooch)
                {
                    KeepBroochAlive<IllurianBrooch, IllurianB>(ref hasIllurianBrooch);
                    Player.moveSpeed += 0.05f;
                    Player.jumpSpeedBoost *= 1.2f;
                    Player.accRunSpeed *= 1.5f;
                    Player.maxRunSpeed *= 1.5f;
                    Player.hasMagiluminescence = true;
                    Player.statDefense -= 10;
                }
            }
        }

        public override void PostUpdateEquips()
        {
            AdvancedBroochEffects();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //FRILE BROOCH HIT EFFECT
            if (hasFrileBrooch && frileBroochCooldown <= 0)
            {

                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<FrileBroochP>(), 3, 1f, Player.whoAmI);
                frileBroochCooldown = 3;
            }


            //VERLIA BROOCH HIT EFFECT
            if (hasVerliaBrooch && verliaBroochCooldown <= 0)
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


            //BURNING GB BROOCH HIT EFFECT
            if (hasBurningGBrooch && hasAdvancedBrooches && burningGBCooldown <= 0)
            {

                for (int d = 0; d < 4; d++)
                {
                    float speedXa = Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-1f, 1f);
                    float speedYa = Main.rand.Next(10, 15) * 0.01f + Main.rand.Next(-1, 1);


                    Vector2 speedea = Main.rand.NextVector2Circular(0.5f, 0.5f);

                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa, speedYa, ProjectileID.IchorSplash, 10, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1f, speedYa * 1.5f, ModContent.ProjectileType<AlcadizBombExplosion>(), 30, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.7f, speedYa * 0.6f, ProjectileID.IchorSplash, 55, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 0.5f, speedYa * 0.3f, ProjectileID.IchorSplash, 45, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1.3f, speedYa * 0.3f, ProjectileID.IchorSplash, 65, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(target), (int)target.Center.X, (int)target.Center.Y, speedXa * 1f, speedYa * 1.5f, ProjectileID.IchorSplash, 40, 1f, Player.whoAmI);
                }

                burningGBCooldown = 220;
            }
        }


        public void KeepBroochAlive<BroochProjectile, BroochBuff>(ref bool hasBrooch) 
            where BroochProjectile : ModProjectile 
            where BroochBuff : ModBuff
        {
            hasBrooch = true;
            var player = Player;
            player.AddBuff(ModContent.BuffType<BroochBuff>(), 2);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BroochProjectile>()] == 0)
            {
            
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, player.velocity * -1f,
                    ModContent.ProjectileType<BroochProjectile>(), 0, 1f, player.whoAmI);
            }

           

               
            
        }
    }
}
