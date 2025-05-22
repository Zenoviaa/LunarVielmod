using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Stellamod.Helpers;
using Stellamod.Dusts;
using System.IO;
using Terraria.Audio;
using Stellamod.Items.Materials.Molds;

namespace Stellamod.Items.Weapons.Mage
{ 
    public class TheAurora : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Ranged;

        //Defaults for the other class
        public override void SetClassSwappedDefaults()
        {
            //Do if(IsSwapped) if you want to check for the alternate class
            //Stats to have when in the other class
            Item.mana = 0;
            Item.damage = 10;
        }
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.damage = 19;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 4;
            Item.useAnimation = 12;
            Item.useStyle = 5;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 20, 0);
            Item.rare = 4;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<AuroraStar>();
            Item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Dust Burst Towards Mouse

            int Sound = Main.rand.Next(1, 3);
            SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol");
            if (Sound == 1)
            {
         

            }
            else
            {
                shootSound = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3");
            }
            shootSound.PitchVariance = 0.1f;
            shootSound.Volume = 0.3f;
            SoundEngine.PlaySound(shootSound, position);

            shootSound = new SoundStyle("Stellamod/Assets/Sounds/Starblast");
            shootSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(shootSound, position);
            float rot = velocity.ToRotation();
            float spread = 0.4f;

            Vector2 offset = new Vector2(1,0f).RotatedBy(rot);
            for (int k = 0; k < 5; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);

                Dust.NewDustPerfect(position + offset * 80, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, Color.Goldenrod, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position + offset * 80, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.Goldenrod, 1);
            Dust.NewDustPerfect(player.Center + offset * 80, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));

            for(int k = 0; k < 2; k++)
            {
                Projectile.NewProjectile(source, position, -velocity.RotatedByRandom(MathHelper.ToRadians(65)), type, damage, knockback, player.whoAmI);
            }

            return false;

        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankGun>(), material: ModContent.ItemType<AuroreanStarI>());
        }
    }
}