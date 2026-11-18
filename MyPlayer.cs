using Stellamod.Buffs;
using Stellamod.Content.Areas.WondrousDarkspace.ArmorWD;
using Stellamod.Content.Armors.Lovestruck;
using Stellamod.Content.Biomes;
using Stellamod.Content.Dusts;
using Stellamod.Items.Consumables;
using Stellamod.Items.Special.Sirestias;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Projectiles.Swords;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod
{


    public class MyPlayer : ModPlayer
    {

        public bool Dice;

        public bool ArcaneM;
        public int ArcaneMCooldown = 0;
        public int Timer = 0;
        public int SwordCombo;
        public int SwordComboR;
        public bool Lovestruck;
        public int LovestruckBCooldown = 0;
        public bool ADisease;
        public bool ZoneFable = false;
        public bool GovheilB;
        public bool GovheilC;
        public int GovheilBCooldown = 0;
        public bool Daedstruck;
        public int DaedstruckBCooldown = 1;

        //----------------------------------------- Pikmin stuff

        public int OnionDamage = 0;
        public bool Onion1 = false;
        public bool Onion2 = false;
        public bool Onion3 = false;
        public bool Onion4 = false;

        //---------------------------------------------------- Igniter effects and uh accessory stuff

        public float IgniterVelocity = 1f;

        //---------------------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------------------------

        private float shakeDrama;
        public bool ZoneAbyss;
        public bool ZoneAurelus;
        public bool ZoneGovheil;
        public bool ZoneAlcadzia;
        public bool ZoneVillage;
        public bool ZoneCinder;
        public bool ZoneDrakonic;
        public bool ZoneMechanics;
        public bool ZoneIlluria;
        public bool ZoneIshtar;
        public bool ZoneBloodCathedral;
        public bool ZoneAshotiTemple;
        public bool ZoneMineshaft;
        public bool ZoneColloseum;
        public bool ZoneMothlight;
        public bool ZoneWonder;

        public bool StealthRune;
        public bool SingularityFragment;
        public float StealthTime;

        public bool CorsageRune;
        public float CorsageTime;

        public bool GIBomb = false;
        public bool RadiantBomb = false;
        public int RadiantBombCooldown = 0;
        public int Bridget = 0;
        public bool HasAlcaliteSet;
        public bool Waterwhisps;

        public void ShakeAtPosition(Vector2 position, float distance, float strength)
        {
            LunarVeilClientConfig config = LunarVeilClientConfig.Instance;
            if (!config.ShakeToggle)
                return;

            var dq = distance * distance;
            shakeDrama = strength * (1f - Player.Center.DistanceSQ(position) / dq) * 0.5f;
        }

        public override void ModifyScreenPosition()
        {
            if (shakeDrama > 0.5f)
            {
                shakeDrama *= 0.92f;
                Vector2 shake = new Vector2(Main.rand.NextFloat(shakeDrama), Main.rand.NextFloat(shakeDrama));
                Main.screenPosition += shake;
            }
        }

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (RadiantBomb && RadiantBombCooldown <= 0)
            {
                for (int d = 0; d < 4; d++)
                {
                    float speedXa = Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-1f, 1f);
                    float speedYa = Main.rand.Next(10, 15) * 0.01f + Main.rand.Next(-1, 1);

                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0, speedYa * 0, ModContent.ProjectileType<GoldsSpawnEffect>(), 490, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0.7f, speedYa * 0.6f, ModContent.ProjectileType<GoldsSlashProj>(), 400, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0.5f, speedYa * 0.3f, ModContent.ProjectileType<GoldsSlashProj>(), 405, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 1.3f, speedYa * 0.3f, ModContent.ProjectileType<GoldsSlashProj>(), 405, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 1f, speedYa * 1.5f, ModContent.ProjectileType<GoldsSlashProj>(), 401, 1f, Player.whoAmI);
                }

                RadiantBombCooldown = 220;
            }
        }



        public override void ModifyHurt(ref Player.HurtModifiers modifiers)/* tModPorter Override ImmuneTo, FreeDodge or ConsumableDodge instead to prevent taking damage */
        {
            if (StealthRune && StealthTime >= 1800)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/StealthRune"), Player.position);
                for (int m = 0; m < 20; m++)
                {
                    int num1 = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Firework_Red, 0f, -2f, 0, default, .8f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num1].position != Player.Center)
                        Main.dust[num1].velocity = Player.DirectionTo(Main.dust[num1].position) * 6f;
                    int num = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Firework_Red, 0f, -2f, 0, default, .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != Player.Center)
                        Main.dust[num].velocity = Player.DirectionTo(Main.dust[num].position) * 6f;
                }
                StealthTime = 0;
            }
            if (CorsageRune && CorsageTime == 0)
            {

                if (Main.rand.NextBool(5))
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CorsageRune1"), Player.position);
                    for (int i = 0; i < 20; i++)
                    {
                        var entitySource = Player.GetSource_FromThis();
                        int num1 = Gore.NewGore(entitySource, new Vector2(Player.Center.X + Main.rand.Next(-10, 10), Player.Center.Y + Main.rand.Next(-10, 10)), Player.velocity, 911);
                        Main.gore[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        Main.gore[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        if (Main.dust[num1].position != Player.Center)
                        {
                            Main.dust[num1].velocity = Player.DirectionTo(Main.dust[num1].position) * 6f;
                        }

                        int num = Gore.NewGore(entitySource, new Vector2(Player.Center.X + Main.rand.Next(-10, 10), Player.Center.Y + Main.rand.Next(-10, 10)), Player.velocity, 911);

                        Main.gore[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        Main.gore[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        if (Main.dust[num].position != Player.Center)
                        {
                            Main.dust[num].velocity = Player.DirectionTo(Main.dust[num].position) * 6f;

                        }
                    }


                    CorsageTime = 1;
                }

            }
            if (CorsageTime >= 1 && CorsageRune)
            {
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CorsageRune2"), Player.position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CorsageRune3"), Player.position);
                }
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type != NPCID.TargetDummy)
                    {
                        int distance = (int)Main.npc[i].Distance(Player.Center);
                        if (distance < 320)
                        {
                            Main.npc[i].AddBuff(BuffID.Poisoned, 120);
                        }

                    }
                }
                for (int i = 0; i < 20; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type != NPCID.TargetDummy)
                    {
                        var entitySource = Main.npc[i].GetSource_FromThis();
                        int num1 = Gore.NewGore(entitySource, new Vector2(Main.npc[i].Center.X + Main.rand.Next(-10, 10), Main.npc[i].Center.Y + Main.rand.Next(-10, 10)), Main.npc[i].velocity, 911);
                        Main.gore[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        Main.gore[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        if (Main.dust[num1].position != Main.npc[i].Center)
                        {
                            Main.dust[num1].velocity = Main.npc[i].DirectionTo(Main.dust[num1].position) * 6f;
                        }

                        int num = Gore.NewGore(entitySource, new Vector2(Main.npc[i].Center.X + Main.rand.Next(-10, 10), Main.npc[i].Center.Y + Main.rand.Next(-10, 10)), Main.npc[i].velocity, 911);

                        Main.gore[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        Main.gore[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                        if (Main.dust[num].position != Main.npc[i].Center)
                        {
                            Main.dust[num].velocity = Main.npc[i].DirectionTo(Main.dust[num].position) * 6f;
                        }

                    }
                }
            }
        }


        public override void ResetEffects()
        {

            // Reset our equipped flag. If the accessory is equipped somewhere, ExampleShield.UpdateAccessory will be called and set the flag before PreUpdateMovement

            ArcaneM = false;
            Waterwhisps = false;
            Dice = false;

            Lovestruck = false;
            ADisease = false;
            GovheilB = false;
 
            GovheilC = false;
            Daedstruck = false;

            HasAlcaliteSet = false;


            CorsageRune = false;
            StealthRune = false;

            RadiantBomb = false;
            GIBomb = false;

            if (SwordComboR <= 0)
            {
                SwordCombo = 0;
                SwordComboR = 0;
            }
            else
            {
                SwordComboR--;
            }


            Onion1 = false;
            Onion2 = false;
            Onion3 = false;
            Onion4 = false;
            OnionDamage = 0;




            IgniterVelocity = 1f;
            if (ZoneColloseum)
                Player.ZoneDesert = true;
        }

        public static bool AuroreanBool;
        public static float Aurorean;
        public static float AuroreanB = 0.5f;
        public override void PostUpdateMiscEffects()
        {

            if (Main.netMode == NetmodeID.Server)
                return;

            Player.ManageSpecialBiomeVisuals("Stellamod:Illuria", ZoneIlluria);
        }


        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            return (IEnumerable<Item>)(object)new Item[1]
            {
                new Item(ModContent.ItemType<SiresMail>(), 1, 0),
            };
        }

        public override void OnEnterWorld()
        {
            Main.NewText(LangText.Misc("EnterWorld"));
        }


        public override void PostUpdate()
        {

            if (Aurorean >= 0.5f)
            {
                AuroreanBool = true;
            }
            if (Aurorean <= 0f)
            {
                AuroreanBool = false;

            }

            if (AuroreanBool)
            {
                AuroreanB += 0.02f;
                Aurorean -= 0.02f;

            }
            else
            {
                AuroreanB -= 0.02f;
                Aurorean += 0.02f;
            }


            Player player = Player;
            MyPlayer CVA = player.GetModPlayer<MyPlayer>();


            bool expertMode = Main.expertMode;
            if (ZoneIlluria)
            {
                SingularityFragment = true;
            }
            else
            {
                SingularityFragment = false;
            }

            if (SingularityFragment)
            {
                if (Main.shimmerAlpha <= 1)
                {
                    Main.shimmerAlpha += 0.02f;
                }
                else
                {
                    Main.shimmerAlpha = 1.02f;
                }
                if (Main.shimmerBrightenDelay <= 0.2f)
                {
                    Main.shimmerBrightenDelay += 0.05f;
                }
                else
                {
                    Main.shimmerBrightenDelay = 0.811f;
                }
                if (Main.shimmerDarken <= 1.4f)
                {
                    Main.shimmerDarken += 0.06f;
                }
                else
                {
                    Main.shimmerDarken = 1.41f;
                }
            }
            else
            {
                if (Main.shimmerAlpha >= 0)
                {
                    Main.shimmerAlpha -= 0.01f;
                }
                else
                {
                    Main.shimmerAlpha = 0f;
                }
                if (Main.shimmerBrightenDelay >= 0f)
                {
                    Main.shimmerBrightenDelay -= 0.01f;
                }
                else
                {
                    Main.shimmerBrightenDelay = 0f;
                }
                if (Main.shimmerDarken >= 0f)
                {
                    Main.shimmerDarken -= 0.01f;
                }
                else
                {
                    Main.shimmerDarken = 0f;
                }
            }





            if (CorsageTime >= 1)
            {
                var entitySource = Player.GetSource_FromThis();
                if (Main.rand.NextBool(5))
                {
                    int a = Gore.NewGore(entitySource, new Vector2(Player.Center.X + Main.rand.Next(-10, 10), Player.Center.Y + Main.rand.Next(-10, 10)), Player.velocity, 911);
                    Main.gore[a].timeLeft = 20;
                    Main.gore[a].scale = Main.rand.NextFloat(.5f, 1f);
                }
                Player.lifeRegen += 30;
                CorsageTime++;
                if (CorsageTime >= 300)
                {

                    CorsageTime = 0;
                }
            }


            if (Daedstruck && DaedstruckBCooldown == 0)
            {
                DaedstruckBCooldown = 600;
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * 0f, ModContent.ProjectileType<LightBomb>(), 30, 1f, Player.whoAmI);

            }



            if (GovheilB && GovheilBCooldown == 540)
            {
                GovheilBCooldown = 0;
            }



            if (GovheilC && GovheilBCooldown == 520)
            {
                GovheilBCooldown = 0;


            }

            if (Dice)
            {
                Timer++;
                if (Timer == 90)
                {
                    var entitySource = player.GetSource_FromThis();

                    switch (Main.rand.Next(5))
                    {

                        case 0:


                            CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Dice.1"), true, false);
                            for (int i = 0; i < player.inventory.Length; i++)

                            {

                                if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

                                {
                                    Item item = new Item();
                                    player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(1, 1));
                                    player.inventory[i].TurnToAir();
                                    player.inventory[i] = item;
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


                                    Dice = false;
                                    break;

                                }
                            }
                            break;

                        case 1:


                            CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Dice.2"), true, false);
                            for (int i = 0; i < player.inventory.Length; i++)

                            {

                                if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

                                {
                                    Item item = new Item();
                                    player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(1, 2));
                                    player.inventory[i].TurnToAir();
                                    player.inventory[i] = item;
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


                                    Dice = false;
                                    break;

                                }
                            }
                            break;

                        case 2:


                            CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Dice.3"), true, false);
                            for (int i = 0; i < player.inventory.Length; i++)

                            {

                                if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

                                {
                                    Item item = new Item();
                                    player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(0, 1));
                                    player.inventory[i].TurnToAir();
                                    player.inventory[i] = item;
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


                                    Dice = false;
                                    break;

                                }
                            }
                            break;

                        case 3:


                            CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Dice.4"), true, false);
                            for (int i = 0; i < player.inventory.Length; i++)

                            {

                                if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

                                {
                                    Item item = new Item();

                                    player.inventory[i].TurnToAir();
                                    player.inventory[i] = item;
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));

                                    Dice = false;
                                    break;

                                }
                            }
                            break;


                        case 4:

                            CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Dice.5"), true, false);
                            for (int i = 0; i < player.inventory.Length; i++)

                            {

                                if (player.inventory[i].type == ModContent.ItemType<GambitToken>())

                                {
                                    Item item = new Item();
                                    player.QuickSpawnItem(entitySource, ModContent.ItemType<GildedBag1>(), Main.rand.Next(2, 2));
                                    player.inventory[i].TurnToAir();
                                    player.inventory[i] = item;
                                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Kaboom"));


                                    Dice = false;
                                    break;

                                }
                            }
                            break;

                    }
                    Timer = 0;

                }



            }


            #region//--------------------------------------------------------------------- Bridget lmaooo (1000 lines)



            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == ModContent.ItemType<Bridget>())
                {
                    Bridget++;
                    if (Bridget > 1080)
                    {
                        int combatText = -1;
                        switch (Main.rand.Next(30))
                        {

                            case 0:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.1"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 1:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.2"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 2:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.3"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 4:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.4"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 5:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.5"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 6:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.6"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 7:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.7"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;


                            case 8:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.8"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;


                            case 9:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.9"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 10:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.10"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 11:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.11"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 12:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.12"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 13:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.13"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 14:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.14"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;


                            case 15:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.15"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 16:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.16"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 17:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.17"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 18:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.18"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 19:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.19"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 20:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.20"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 21:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.21"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 22:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.22"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 23:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.23"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;


                            case 24:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.24"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 25:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.25"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;


                            case 26:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.26"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 27:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.27"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 28:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.28"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                            case 29:

                                combatText = CombatText.NewText(player.getRect(), Color.YellowGreen, LangText.Misc("Bridget.29"), true, false);
                                Bridget = 0;
                                SoundEngine.PlaySound(SoundID.LucyTheAxeTalk, player.position);
                                break;

                        }

                        //Checking if combatText not equal to -1 just incase it somehow didn't get set
                        if (combatText != -1)
                        {
                            //360 ticks = 6 seconds
                            CombatText text = Main.combatText[combatText];
                            text.lifeTime = 360;
                        }
                    }



                    break;

                }













            }

            #endregion 
        }
        public const int CAMO_DELAY = 100;
        bool Sirestiastalk;
        bool Zuitalk;
        public override void SaveData(TagCompound tag)
        {
            tag["Sirestiastalk"] = Sirestiastalk;
            tag["Zuitalk"] = Zuitalk;
        }

        public override void LoadData(TagCompound tag)
        {
            Sirestiastalk = tag.GetBool("Sirestiastalk");
            Zuitalk = tag.GetBool("Zuitalk");
        }


        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Item, consider using OnHitNPC instead */
        {


            if (Lovestruck)
            {

                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<LovestruckP>(), 4, 1f, Player.whoAmI);


            }


        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Projectile, consider using OnHitNPC instead */
        {
            if (Lovestruck && LovestruckBCooldown <= 0)
            {

                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<LovestruckP>(), 4, 1f, Player.whoAmI);
                LovestruckBCooldown = 30;

            }
        }


        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {


            if (ADisease)
            {
                switch (Main.rand.Next(8))
                {
                    case 0:

                        npc.AddBuff((BuffID.Poisoned), 120);
                        break;
                    case 1:

                        npc.AddBuff((BuffID.Slow), 120);

                        break;
                    case 2:

                        npc.AddBuff((BuffID.OnFire3), 240);

                        break;
                    case 3:

                        npc.AddBuff((BuffID.OnFire), 120);

                        break;
                    case 4:

                        npc.AddBuff((BuffID.Frostburn2), 240);

                        break;
                    case 5:

                        npc.AddBuff((BuffID.BrainOfConfusionBuff), 240);

                        break;

                    case 6:

                        npc.AddBuff((BuffID.Lovestruck), 240);

                        break;
                    case 7:

                        break;
                }
            }
        }

        public override void PostUpdateEquips()
        {
            //Terric Setbonus


            //Sap Container's Effect
            if (ArcaneM)
            {
                if (ArcaneMCooldown == 601)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ArcaneExplode"));
                }

                if (ArcaneMCooldown > 600)
                {
                    Player.GetDamage(DamageClass.Magic) *= 1.25f;
                }

                if (ArcaneMCooldown > 720)
                {
                    ArcaneMCooldown = 0;
                }
            }
            else
            {
                ArcaneMCooldown = 0;
            }


            if (StealthRune)
            {
                if (StealthTime <= 1800)
                {
                    StealthTime++;
                }
                else
                {
                    if (Main.rand.NextBool(5))
                    {
                        int d = Dust.NewDust(Player.position, Player.width, Player.height,
                                ModContent.DustType<GlowDust>(), newColor: Color.Red, Scale: 0.8f);
                        Main.dust[d].noGravity = true;

                        if (Main.rand.NextBool(5))
                        {
                            Dust.NewDust(Player.position, Player.width, Player.height,
                              ModContent.DustType<GunFlash>(), newColor: Color.Red, Scale: 0.8f);

                        }
                    }
                }

                float maxDamageIncrease = 0.33f;
                float stealthProgress = StealthTime / 1800;
                float damageIncrease = stealthProgress * maxDamageIncrease;
                Player.GetDamage(DamageClass.Generic) += damageIncrease;
            }

            if (GovheilC && GovheilBCooldown > 300)
            {
                Player.GetDamage(DamageClass.Ranged) *= 1.5f;
                Player.GetDamage(DamageClass.Melee) *= 1.5f;

            }
            Player.ZoneLihzhardTemple = BiomeTileCounts.InAshotiTemple;
            if (GovheilB && GovheilBCooldown > 300)
            {
                Player.GetDamage(DamageClass.Magic) *= 1.1f;
                Player.GetDamage(DamageClass.Summon) *= 1.1f;

            }

        }
    }
}
