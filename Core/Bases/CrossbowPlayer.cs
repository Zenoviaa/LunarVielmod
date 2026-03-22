using Microsoft.Xna.Framework;
using ReLogic.Content;
using Stellamod.Items.Accessories.Players;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    public class CrossbowPlayer : ModPlayer
    {
        private float _burstTimer;
        public bool takeAim;
        public bool usingStamina;

        public int burstCount;
        public float burstRate;
        public float burstChargeStrength;
        public Vector2 burstVelocity;
        public Asset<Texture2D> magicCircleTextureAsset;
        public Color magicCircleColor;

        public Asset<Texture2D> magicBigCircleTextureAsset;
        public Color magicBigCircleColor;
        public override void ResetEffects()
        {
            base.ResetEffects();
            takeAim = false;
            usingStamina = false;
            magicCircleTextureAsset = null;
            magicCircleColor = Color.White;
            magicBigCircleTextureAsset = null;
            magicBigCircleColor = Color.White;

            if (Player.HeldItem.ModItem is not BaseCrossbowItem crossbowItem)
                return;
            MagicCircle magicCircle = crossbowItem.GetMagicCircle();
            magicBigCircleTextureAsset = magicCircle.textureAsset;
            magicBigCircleColor = magicCircle.color;
        }
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();

        }

        public override void PostItemCheck()
        {
            base.PostItemCheck();
            if (Player.HeldItem.ModItem is not BaseCrossbowItem crossbowItem)
                return;
            int crossbowHoldType = ModContent.ProjectileType<CrossbowHold>();
            if (Main.myPlayer == Player.whoAmI &&
                Player.ownedProjectileCounts[crossbowHoldType] == 0 && takeAim)
            {
                bool fireStaminaShot = usingStamina;
                if (fireStaminaShot)
                {
                    DashPlayer dashPlayer = Player.GetModPlayer<DashPlayer>();
                    if (dashPlayer.CanConsume(crossbowItem.staminaCost))
                    {
                        dashPlayer.Consume(crossbowItem.staminaCost);
                    }
                    else
                    {
                        fireStaminaShot = false;
                    }
                }
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, crossbowHoldType, 1, 1, Player.whoAmI,
                    ai2: fireStaminaShot ? 1 : 0);
            }
            if(burstCount > 0)
            {
                _burstTimer++;
                if(_burstTimer >= burstRate)
                {
                    Player.PickAmmo(Player.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId);
                    ShootParams @params = new ShootParams
                    {
                        position = Player.Center,
                        velocity = burstVelocity,
                        chargeStrength = burstChargeStrength,
                        damage = damage,
                        knockBack = knockBack,
                        projToShoot = projToShoot,
                        speed = speed,
                        useAmmoItemId = usedAmmoItemId
                    };
                    crossbowItem.ShootBow(Player, new Terraria.DataStructures.EntitySource_ItemUse_WithAmmo(Player, Player.HeldItem, usedAmmoItemId), @params);
                    burstCount--;
                    _burstTimer = 0;
                }
             
            }
        }

        public void BurstShot(int amount, float rate, Vector2 velocity, float strength)
        {
            burstCount += amount;
            burstRate = rate;
            _burstTimer += rate;
            burstVelocity = velocity;
            burstChargeStrength = strength;
        }
        public override void PostUpdate()
        {
            base.PostUpdate();
        }
    }
}
