using Stellamod.Common.ArmorRework;
using Stellamod.Content.Armors.Veldrin;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.PicturePerfect;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Paint;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Artisan
{
    public class ArtisanPlayer : ModPlayer
    {
        public bool hasSetBonus;

        //--------------------------------------- Picture perfect stuff
        public int PPDefense = 0;
        public int PPDMG = 0;
        public float PPPaintDMG = 0;
        public int PPPaintDMG2 = 0;
        public bool PPPaintI = false;
        public bool PPPaintII = false;
        public bool PPPaintIII = false;
        public float PPSpeed = 0;
        public int PPCrit = 0;
        public int PPPaintTime = 0;
        public int PPFrameTime = 0;
        public bool Cameraaa = false;
        public float CameraaaTime;
        public bool ThreeTwoOneSmile;
        public int ThreeTwoOneSmileBCooldown = 1440;
        public int PaintdropBCooldown = 3;



        public override void ResetEffects()
        {
            base.ResetEffects();
            hasSetBonus = false;

            PPDefense = 0;
            PPDMG = 0;
            PPPaintDMG = 0;
            PPPaintDMG2 = 0;
            PPPaintI = false;
            PPPaintII = false;
            PPPaintIII = false;
            PPSpeed = 0;
            PPCrit = 0;
            PPPaintTime = 0;
            PPFrameTime = 0;
            Cameraaa = false;

        }
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (Player.dead)
            {
                CameraaaTime = 0;
                Cameraaa = false;
            }


            if (ThreeTwoOneSmile && ThreeTwoOneSmileBCooldown > 1480)
            {
                //Player.GetDamage(DamageClass.Generic) += PPPaintDMG;
                Player.GetCritChance(DamageClass.Generic) = 100f;


                if (PPPaintI)
                {
                    PPPaintDMG2 = 15;
                }

                if (PPPaintI && PPPaintII)
                {
                    PPPaintDMG2 = 50;
                }

                if (PPPaintI && PPPaintII && PPPaintIII)
                {
                    PPPaintDMG2 = 150;
                }
            }


            if (Cameraaa)
            {
                CameraaaTime++;
                if (CameraaaTime <= 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Vomit3"), Player.position);
                    var EntitySource = Player.GetSource_FromThis();

                    if(Main.myPlayer == Player.whoAmI)
                    {
                        Projectile.NewProjectile(EntitySource, Player.Center.X, Player.Center.Y, 0, 0,
                            ModContent.ProjectileType<SmileForCamera>(), Player.HeldItem.damage * 0, 1, Player.whoAmI, 0, 0);
                    }

                    Player.AddBuff(ModContent.BuffType<CameraMinBuff>(), 99999);
                }

            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<CameraMinBuff>());
                CameraaaTime = 0;
            }



            if (ThreeTwoOneSmile && ThreeTwoOneSmileBCooldown == 180)
            {
                CombatText.NewText(Player.getRect(), Color.White, 3);
                if (Main.myPlayer == Player.whoAmI)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Three"));
                    for (int j = 0; j < 5; j++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);

                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 5,
                            ModContent.ProjectileType<Paint2>(), 25, 1f, Player.whoAmI);
                    }
                }

            }

            if (ThreeTwoOneSmile && ThreeTwoOneSmileBCooldown == 120)
            {
                CombatText.NewText(Player.getRect(), Color.Yellow, 2);
                if (Main.myPlayer == Player.whoAmI)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Two"));
                    for (int j = 0; j < 5; j++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 5, ModContent.ProjectileType<Paint3>(), 25, 1f, Player.whoAmI);
                    }
                }

            }

            if (ThreeTwoOneSmile && ThreeTwoOneSmileBCooldown == 60)
            {
                CombatText.NewText(Player.getRect(), Color.Orange, 1);
                if (Main.myPlayer == Player.whoAmI)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/One"));
                    for (int j = 0; j < 5; j++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 5, ModContent.ProjectileType<Paint2>(), 25, 1f, Player.whoAmI);
                    }

                }
            }

            if (ThreeTwoOneSmile && ThreeTwoOneSmileBCooldown == 0)
            {
                CombatText.NewText(Player.getRect(), Color.Red, 0);
                if (Main.myPlayer == Player.whoAmI)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/zero"));
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Binding_Abyss_Spawn"));
                    for (int j = 0; j < 5; j++)
                    {
                        Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 5, ModContent.ProjectileType<Paint3>(), 25, 1f, Player.whoAmI);


                    }
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity, ModContent.ProjectileType<Artbar>(), 0, 1f, Player.whoAmI);
                }
                   
                ThreeTwoOneSmileBCooldown = 1720 + PPPaintTime;
            }

            if (ThreeTwoOneSmile && PaintdropBCooldown == 0)
            {

                if (Main.myPlayer == Player.whoAmI)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Player.velocity * 0f,
                    ModContent.ProjectileType<Meatball4>(), 0, 1f, Player.whoAmI);
                }
                PaintdropBCooldown = 25;
            }


        }
    }
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class ArtisanMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorSetSystem.RegisterArmorSet<ArtisanMask, ArtisanBreastplate, ArtisanThighs>(ArmorGroup.Act_II);
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.LightPurple;// The rarity of the item
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.criticalStrikeDamage += 3;
            stats.defenseBonus+=7;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ArtisanBreastplate>() && legs.type == ModContent.ItemType<ArtisanThighs>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
        //    player.setBonus = LangText.SetBonus(this);//"3, 2, 1 .. Smile! Act like you're on stage will ya :p" + "\nEvery little while you'll get a countdown, and when you hear Zero," + "\nyour crit is 100% and damage output is doubled! " + "\nSmall bits of paint left in your tracks." + "\nCrit chance and armor penetration increased by 20!");// This is the setbonus tooltip
            player.GetModPlayer<ArtisanPlayer>().ThreeTwoOneSmile = true;
            player.GetModPlayer<ArtisanPlayer>().ThreeTwoOneSmileBCooldown--;
            player.GetModPlayer<ArtisanPlayer>().PaintdropBCooldown--;
        }
    }  


    [AutoloadEquip(EquipType.Body)]
    public class ArtisanBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.LightPurple; // The rarity of the item
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.defenseBonus += 10;
            stats.accessorySlots++;
        }
    }

 
    [AutoloadEquip(EquipType.Legs)]
    public class ArtisanThighs : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.LightPurple; // The rarity of the item
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.defenseBonus += 9;
            stats.criticalStrikeChance += 0.02f;
            stats.accessorySlots++;
        }
    }
}