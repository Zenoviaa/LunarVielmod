using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Ranged
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
            /*
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
            */
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D iconTexture = null;
            const string Base_Path = "Urdveil/Items/Weapons/Ranged/GunSwapping/";

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
        {/*
            for (int i = 0; i < 1; i++)
            {
                Gore.NewGore(player.GetSource_FromThis(), position, velocity * -3,
                    ModContent.GoreType<BulletCasing>());
            }
            */
        }
    }
}

