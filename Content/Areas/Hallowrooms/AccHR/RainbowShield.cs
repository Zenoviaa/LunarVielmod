using Stellamod.Common.WeaponTypes;
using Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Dusts;
using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Hallowrooms.AccHR;

public class RainbowShield : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToShield(ModContent.ProjectileType<RainbowShieldHeld>());
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<KaleidoscopicInk, BlankCard>();
    }
}

public class RainbowShieldHeld : AbstractShieldProjectile
{
    public override void OnBlockMovement(NPC npc)
    {
        base.OnBlockMovement(npc);
        if (npc.boss)
            return;
        if (!npc.HasBuff<Charm>())
        {
            Dust.NewDustPerfect(npc.Center, ModContent.DustType<GlowHeartDust>(), Vector2.Zero, Scale: 0.5f, newColor: Color.Pink);
        }        

        npc.AddBuff(ModContent.BuffType<Charm>(), 600);
    }
}