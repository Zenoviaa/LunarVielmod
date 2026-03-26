using ReLogic.Content;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class DeadRomancesExcalibur : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 10;
        Item.shoot = ModContent.ProjectileType<DeadRomancesExcaliburSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<DeadRomanceParryingBlade>();
        staminaCost = 2;
        staminaDamageMultiplier = 2;
        comboResetTime = 60;
        meleeWeaponType = MeleeWeaponType.Greatsword;

    }
    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        
        if (Main.LocalPlayer.HasBuff<HeavenlyLove>())
        {
            Asset<Texture2D> glowingSwordTextureAsset = ModContent.Request<Texture2D>(Texture + "_Ascended");
            drawColor *= ExtraMath.Osc(0.5f, 1f, speed: 3);
            spriteBatch.Draw(glowingSwordTextureAsset.Value, position, null, drawColor, 0, glowingSwordTextureAsset.Size() * 0.5f, scale, SpriteEffects.None, 0);

            drawColor *= ExtraMath.Osc(0.5f, 1f, speed: 3);
            drawColor.A = 0;
            spriteBatch.Draw(glowingSwordTextureAsset.Value, position, null, drawColor, 0, glowingSwordTextureAsset.Size() * 0.5f, scale, SpriteEffects.None, 0);

            if (Main.rand.NextBool(32) && !Main.gameInactive)
            {
                DustParticle dp = DustParticle.SpawnInUI(position + Main.rand.NextVector2Circular(32, 32), -Vector2.UnitY, Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                dp.gravity = 0;
                dp.innerColor = Color.White;
                dp.outerColor = Color.White;
            }
            //ItemSlot.DrawItemIcon(Item, 0, spriteBatch, position, scale, 32, drawColor);
            return false;
        }
        return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
    }
    public override void ShootSwing(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        DeadRomancePlayer romancePlayer = player.GetModPlayer<DeadRomancePlayer>();
        if (player.HasBuff<HeavenlyLove>())
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DeadRomanceAscendedDash>(),
                damage, knockback, player.whoAmI);
            return;
        }
        base.ShootSwing(player, source, position, velocity, type, damage, knockback);
    }

    public override void ShootSwingStamina(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.HasBuff<HeavenlyLove>())
        {
            type = ModContent.ProjectileType<DeadRomancesExcaliburParrySlash>();
            damage *= 3;
            player.GetModPlayer<DashPlayer>().DashCount += 2;
        }

        base.ShootSwingStamina(player, source, position, velocity, type, damage, knockback);

    }
}
