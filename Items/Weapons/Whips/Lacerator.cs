using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Whipfx;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Whips;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Whips
{
    public class Lacerator : ModItem
    {
        private int _attackStyle;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LaceratorDebuff.TagDamage);
        public override void SetStaticDefaults()
        {
			ItemID.Sets.ItemNoGravity[Item.type] = true;
		}

        public override void SetDefaults()
		{
			// This method quickly sets the whip's properties.
			// Mouse over to see its parameters.
			Item.DefaultToWhip(ModContent.ProjectileType<LaceratorProj>(), 100, 3, 24);
			Item.width = 40;
			Item.height = 34;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 5);
		}

		private void ChangeForm(int newForm)
		{
            _attackStyle = newForm;
            if (_attackStyle == 1)
            {
                Item.DefaultToWhip(ModContent.ProjectileType<LaceratorMiracleProj>(), 100, 3, 24, 15);
            }
            else if (_attackStyle == 0)
            {
                Item.DefaultToWhip(ModContent.ProjectileType<LaceratorProj>(), 100, 3, 24);
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            if (_attackStyle == 0)
            {
                line = new TooltipLine(Mod, "Brooch of the TaGo", "Right click to change form, requires a Sewing Kit")
                {
                    OverrideColor = Color.Magenta
                };
                tooltips.Add(line);
            }
            else
            {
                line = new TooltipLine(Mod, "Brooch of the TaGo", "Changed by Sewing Kit, effects may be incorrect...")
                {
                    OverrideColor = Color.Magenta
                };
                tooltips.Add(line);
            }

        }

        public override bool CanUseItem(Player player)
        {
            if (!player.GetModPlayer<SewingKitPlayer>().hasSewingKit && _attackStyle == 1)
            {
                ChangeForm(0);
            }

            if (player.altFunctionUse == 2 && player.GetModPlayer<SewingKitPlayer>().hasSewingKit)
            {
                if (_attackStyle == 0)
                {
                    ChangeForm(1);
                }
                else
                {
                    ChangeForm(0);
                }

                int sound = Main.rand.Next(0, 3);
                switch (sound)
                {
                    case 0:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia1"), player.position);
                        break;
                    case 1:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia2"), player.position);
                        break;
                    case 2:
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia3"), player.position);
                        break;
                }
                return false;
            }

            return base.CanUseItem(player);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Purple);
            if (_attackStyle == 1)
            {
                Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Weapons/Summon/LaceratorMiracle").Value;
                Vector2 size = new Vector2(36, 46);
                Vector2 drawOrigin = size / 2;
                spriteBatch.Draw(iconTexture, position, null, drawColor, 0f, drawOrigin, scale, SpriteEffects.None, 0);
                return false;
            }

            return true;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SwordWhip, 1)
				.AddIngredient(ModContent.ItemType<MiracleThread>(), 12)
				.AddIngredient(ModContent.ItemType<AlcaricMush>(), 4)
				.AddIngredient(ModContent.ItemType<EldritchSoul>(), 4)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}

		// Makes the whip receive melee prefixes
		public override bool MeleePrefix()
		{
			return true;
		}


		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
			return true;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			//The below code makes this item hover up and down in the world
			//Don't forget to make the item have no gravity, otherwise there will be weird side effects
			float hoverSpeed = 5;
			float hoverRange = 0.2f;
			float y = VectorHelper.Osc(-hoverRange, hoverRange, hoverSpeed);
			Vector2 position = new Vector2(Item.position.X, Item.position.Y + y);
			Item.position = position;
		}
	}
}
