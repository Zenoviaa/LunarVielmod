using Stellamod.Assets;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Players;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Stellamod.Content.Areas.SpringHills.AccSH;

public class WoodlandTalisman : AbstractDashItem
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
        if (_timer == 1)
        {
            player.velocity.X *= -1;
            player.velocity.Y -= 1;
            SoundStyle minervaSpin = AssetRegistry.Sounds.Minerva.MinervaSpin with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(minervaSpin, player.position);
        }

        if(_timer % 4 == 0)
        {
            Vector2 pos = player.position;
            pos.X += Main.rand.Next(0, player.width);
            pos.Y += Main.rand.Next(0, player.height);
            Dust.NewDustPerfect(pos, DustID.JungleGrass, Scale: 0.75f);
        }

        player.velocity.Y -= 0.8f;
        if(_timer >= player.GetModPlayer<DashPlayer>().DashDuration / 2)
        {
            player.velocity.Y *= 0.96f;
        }
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
        dashPlayer.ExtraImmunityFramesBonus += 5;
        dashPlayer.DashCooldown += 30;
        player.jumpSpeedBoost += 0.05f;
    }
}
