using Stellamod.Common.Particles;
using Stellamod.Content.CommonMaterials;

using Stellamod.Core.Bases;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class MushroomGreatbow : BaseCrossbowItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.MushroomGreatbow.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 3;
    }

    public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        base.StaminaShootBow(player, source, shootParams);
        CrossbowPlayer crossbowPlayer = player.GetModPlayer<CrossbowPlayer>();
        crossbowPlayer.BurstShot(5, 3, shootParams.velocity, shootParams.chargeStrength);

        for (var i = 0; i < 3; i++)
        {
            var firer = ProjFirer.From<MushroomStaveMinionProg>(player);
            firer.velocity = shootParams.fireVelocity;
            firer.velocity.Y -= 4f;
            firer.velocity = firer.velocity.RotatedByRandom(0.6f);
            firer.damage = shootParams.damage;
            firer.New();
        }
        for (var i = 0; i < 9; i++)
        {
            var pos = player.Center + Main.rand.NextVector2Circular(48, 48);
            pos += shootParams.fireVelocity * 7f;
            Particles.RoughSmoke.Spawn(new()
            {
                position = pos,
                timeLeft = 35,
                color = DrawUtilities.InterpolateColorArray(Main.rand.NextFloat(0f, 1f), Color.OrangeRed, Color.DarkRed, Color.Gold, Color.Red, Color.DarkOrange, Color.DarkGoldenrod),
                scale = Main.rand.NextFloat(0.8f, 1f),
                rotation = Main.rand.NextFloat(6f)
            });
        }
        SoundStyle gasSound = new SoundStyle("Stellamod/Assets/Sounds/ExplosionGaseous");
        gasSound.PitchVariance = 0.3f;
        gasSound.Volume = 0.5f;
        SoundEngine.PlaySound(gasSound, player.position);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankBow>(), material: ModContent.ItemType<Mushroom>());
    }
}
