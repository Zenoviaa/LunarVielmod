using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;

public class BackstepTalisman : AbstractDashItem
{
    private float _timer;
    public override void BeginDash(Player player)
    {
        base.BeginDash(player);
        _timer = 0;
    }
    public override void UpdateDash(Player player)
    {
        base.UpdateDash(player);
        _timer++;
        if(_timer == 1)
        {
            player.velocity.X *= -5f;
            int rand = Main.rand.Next(2);
            SoundStyle backStepSound;
            switch (rand)
            {
                default:
                case 0:
                    backStepSound = new SoundStyle("Stellamod/Assets/Sounds/WindCast1");
                    break;
                case 1:
                    backStepSound = new SoundStyle("Stellamod/Assets/Sounds/WindCast2");
                    break;
            }
            backStepSound = backStepSound with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(backStepSound, player.position);
        }
        player.velocity *= 0.8f;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
        dashPlayer.extraStaminaCost -= 1;
        dashPlayer.DashDuration -= 30;
        dashPlayer.noRoll = true;
        dashPlayer.ExtraImmunityFramesBonus -= 30;
        dashPlayer.DashCooldown -= 10;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MarshScrap, BlankAccessory>();
    }
}
