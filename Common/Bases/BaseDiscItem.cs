using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    internal abstract class BaseDiscItem : ClassSwapItem
    {
        public int Penetrate = 2;
        public int Music;
        public int TileToPlace;
        public Color TrailColor;

        public virtual string MusicPath { get; }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            SetDiscDefaults();
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, MusicPath), Type, TileToPlace);
        }

        public override DamageClass AlternateClass => DamageClass.Generic;
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 4;
        }

        public virtual void SetDiscDefaults()
        {

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            Item.DamageType = DamageClass.Throwing;
            Item.damage = 9;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<BasicDiscProjectile>();
            Item.shootSpeed = 12;
            Item.accessory = true;
            SetDiscDefaults();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            TooltipLine line = new TooltipLine(Mod, "Disc", LangText.Common("Disc"));
            line.OverrideColor = new Color(80, 187, 124);
            tooltips.Add(line);

            line = new TooltipLine(Mod, "DiscHelp", LangText.Common("DiscHelp"));
            line.OverrideColor = Color.Lerp(new Color(80, 187, 124), Color.Black, 0.5f);
            tooltips.Add(line);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.consumable = false;
                Item.autoReuse = true;
                Item.createTile = -1;
                Item.shoot = ModContent.ProjectileType<BasicDiscProjectile>();
                Item.shootSpeed = 12;
            }
            else
            {
  
                Item.createTile = TileToPlace;
                Item.consumable = true;
                Item.shoot = ProjectileID.None;
                Item.shootSpeed = 0;
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: Type);
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg"), position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg2"), position);
                }
            }

            return false;
        }
    }
}
