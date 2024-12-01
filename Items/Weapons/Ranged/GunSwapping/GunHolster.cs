using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.UI.GunHolsterSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Items.Weapons.Ranged.GunSwapping
{
    internal class GunPlayer : ModPlayer
    {
        private Item _leftHand;
        private Item _rightHand;
        public Item LeftHand
        {
            get
            {
                if (_leftHand == null)
                {
                    _leftHand = new Item();
                    _leftHand.SetDefaults(0);
                }

                return _leftHand;
            }
            set
            {
                _leftHand = value;
            }
        }

        public Item RightHand
        {
            get
            {
                if (_rightHand == null)
                {
                    _rightHand = new Item();
                    _rightHand.SetDefaults(0);
                }

                return _rightHand;
            }
            set
            {
                _rightHand = value;
            }
        }
        public ModItem RightHandItem => RightHand.ModItem;
        public ModItem LeftHandItem => LeftHand.ModItem;

        private void HolsterGun(Player player, int projectileType, int baseDamage, float knockBack)
        {
            int newDamage = (int)player.GetTotalDamage(player.HeldItem.DamageType).ApplyTo(baseDamage);
            if (player.ownedProjectileCounts[projectileType] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    projectileType, newDamage, knockBack, player.whoAmI);
            }
        }

        public override void PostUpdate()
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                int buffType = ModContent.BuffType<MarksMan>();
                int itemType = ModContent.ItemType<GunHolster>();
                if (!Player.HasBuff(buffType))
                {
                    if ((Player.HeldItem.type == itemType || Main.mouseItem.type == itemType))
                    {
                        Player.AddBuff(buffType, 2, false);
                    }
                }
                else
                {
                    if (!RightHand.IsAir)
                    {
                        if (RightHandItem is MiniGun miniGun)
                        {
                            int toHolster = miniGun.GunHolsterProjectile;
                            if (miniGun.GunHolsterProjectile2 != -1)
                            {
                                toHolster = miniGun.GunHolsterProjectile2;
                            }
                            HolsterGun(Player, toHolster, RightHandItem.Item.damage, RightHandItem.Item.knockBack);
                        }
                    }

                    if (!LeftHand.IsAir)
                    {
                        if (LeftHandItem is MiniGun miniGun)
                        {
                            HolsterGun(Player, miniGun.GunHolsterProjectile, LeftHandItem.Item.damage, LeftHandItem.Item.knockBack);
                        }
                    }

                    if ((Player.HeldItem.type != itemType) && Main.mouseItem.type != itemType)
                    {
                        Player.ClearBuff(buffType);
                        NetMessage.SendData(MessageID.PlayerBuffs);
                    }
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["lefthand"] = LeftHand;
            tag["righthand"] = RightHand;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.ContainsKey("lefthand"))
                LeftHand = tag.Get<Item>("lefthand");
            if (tag.ContainsKey("righthand"))
                RightHand = tag.Get<Item>("righthand");
        }
    }

    internal class GunHolster : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.mana = 2;
        }
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 36;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;

            Item.shootSpeed = 4f;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = null;
            Item.noUseGraphic = true;
            Item.consumable = false;
        }

        public override void RightClick(Player player)
        {
            base.RightClick(player);

            //Yay
            GunHolsterUISystem gunHolsterUISystem = ModContent.GetInstance<GunHolsterUISystem>();
            gunHolsterUISystem.ToggleUI();
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            GunPlayer gunPlayer = Main.LocalPlayer.GetModPlayer<GunPlayer>();
            var line = new TooltipLine(Mod, "WeaponType", "Gun Holster Weapon Type")
            {
                OverrideColor = ColorFunctions.GunHolsterWeaponType
            };
            tooltips.Add(line);

            var leftHand = new TooltipLine(Mod, "left", "");
            var rightHand = new TooltipLine(Mod, "right", "");

            const string Base_Path = "Stellamod/Items/Weapons/Ranged/GunSwapping/";
            if (!gunPlayer.LeftHand.IsAir)
            {
                string textureName = gunPlayer.LeftHandItem.Name.ToString().Replace("_", "");
                Texture2D texture = ModContent.Request<Texture2D>($"{Base_Path}{textureName}").Value;
                Color[] pixels = new Color[texture.Width * texture.Height];
                texture.GetData(pixels);
                Color lastColor = Color.White;
                Color tooltipColor = Color.White;
                for (int i = pixels.Length / 2; i < pixels.Length; i++)
                {
                    if (lastColor == Color.Black && pixels[i] != Color.Black)
                    {
                        tooltipColor = pixels[i];
                        break;
                    }
                    lastColor = pixels[i];
                }

                string gunName = gunPlayer.LeftHandItem.DisplayName.ToString().Replace("_", " ");
                leftHand.Text = $"Left Hand: [{gunName}]";
                leftHand.OverrideColor = tooltipColor;
                tooltips.Add(leftHand);
            }

            if (!gunPlayer.RightHand.IsAir)
            {
                string textureName = gunPlayer.RightHandItem.Name.ToString().Replace("_", "");
                Texture2D texture = ModContent.Request<Texture2D>($"{Base_Path}{textureName}").Value;
                Color[] pixels = new Color[texture.Width * texture.Height];
                texture.GetData(pixels);
                Color lastColor = Color.White;
                Color tooltipColor = Color.White;
                for (int i = pixels.Length / 2; i < pixels.Length; i++)
                {
                    if (lastColor == Color.Black && pixels[i] != Color.Black)
                    {
                        tooltipColor = pixels[i];
                        break;
                    }
                    lastColor = pixels[i];
                }

                string gunName = gunPlayer.RightHandItem.DisplayName.ToString().Replace("_", " ");
                rightHand.Text = $"Right Hand: [{gunName}]";
                rightHand.OverrideColor = tooltipColor;
                tooltips.Add(rightHand);
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            GunPlayer gunPlayer = Main.LocalPlayer.GetModPlayer<GunPlayer>();
            const string Base_Path = "Stellamod/Items/Weapons/Ranged/GunSwapping/";

            if (!gunPlayer.LeftHand.IsAir)
            {
                string textureName = gunPlayer.LeftHandItem.Name.ToString().Replace("_", "");
                Texture2D leftHandTexture = ModContent.Request<Texture2D>($"{Base_Path}{textureName}_Held").Value;
                Vector2 leftHandTextureSize = leftHandTexture.Size();
                Vector2 leftHandDrawOrigin = leftHandTextureSize / 2;

                Vector2 drawPosition = position;
                spriteBatch.Draw(leftHandTexture, drawPosition, null, drawColor, 0f, leftHandDrawOrigin, scale, SpriteEffects.None, 0);
            }

            if (!gunPlayer.RightHand.IsAir)
            {

                string textureName = gunPlayer.RightHandItem.Name.ToString().Replace("_", "");
                Texture2D rightHandTexture = ModContent.Request<Texture2D>($"{Base_Path}{textureName}_Held").Value;
                Vector2 rightHandTextureSize = rightHandTexture.Size();
                Vector2 rightHandDrawOrigin = rightHandTextureSize / 2;

                //Offset it a little
                Vector2 drawPosition = position + new Vector2(8, 8);
                spriteBatch.Draw(rightHandTexture, drawPosition, null, drawColor, 0f, rightHandDrawOrigin, scale, SpriteEffects.None, 0);
            }

            if (!gunPlayer.LeftHand.IsAir || !gunPlayer.RightHand.IsAir)
                return false;
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
