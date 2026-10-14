using Stellamod.Common.Particles;
using Stellamod.Common.StaminaSystem;
using Stellamod.Common.XixianFlaskSystem;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH;

public class BeastRagePlayer : ModPlayer
{
    public bool hasBeastRage;
    public override void Load()
    {
        base.Load();
        FlaskPlayer.OnProc += ApplyBeastRage;
    }

    public override void ResetEffects()
    {
        base.ResetEffects();
        hasBeastRage = false;
    }
    public override void Unload()
    {
        base.Unload();
        FlaskPlayer.OnProc -= ApplyBeastRage;
    }
    private void ApplyBeastRage(Player player)
    {
        BeastRagePlayer beastRagePlayer = player.GetModPlayer<BeastRagePlayer>();
        if (!beastRagePlayer.hasBeastRage)
            return;
        player.AddBuff(ModContent.BuffType<RagingBeast>(), 240);
        SoundStyle rageSound = new SoundStyle("Stellamod/Assets/Sounds/DMHeart__Spawn");
        rageSound.Volume = 0.4f;
        SoundEngine.PlaySound(rageSound, player.position);
        FXUtil.ShakeCamera(player.position, 1024, 8);
        ModContent.GetInstance<ScreenShaderSystem>().TintScreen(Color.Red, 0.2f, 15);
    }

}

public class RagingBeast : ModBuff
{
    public override string Texture => AssetReferences.Assets.Textures.Buffs.RagingBeast.KEY;
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        player.GetAttackSpeed(DamageClass.Generic) += 0.2f;
        player.GetDamage(DamageClass.Generic) += 0.2f;
        player.GetModPlayer<StaminaNDashPlayer>().DashRegenerationBonus += 0.5f;
        if (Main.rand.NextBool(3))
        {
            var vel = Main.rand.NextVector2Circular(4, 4);
            var pos = player.Center;
            var scaleRange = new Vector2(0.5f, 1.5f);
            pos += Main.rand.NextVector2Circular(16, 16);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.OrangeRed.ToVector4(),
                outerColor = Color.DarkRed.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(scaleRange.X, scaleRange.Y)),
                timeLeft = Main.rand.Next(30, 55)
            });
        }
    }
}
public class BeastRage : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.BeastRage.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<BeastRagePlayer>().hasBeastRage = true;
    }
}
