using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.TheFalling.AccTF;

public class GhastlyShield : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToShield(ModContent.ProjectileType<GhastlyShieldHeld>());
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<GhastlySpirit, BlankCard>();
    }
}

public class GhastlyShieldHeld : AbstractShieldProjectile
{
    public override void OnBlockMovement(NPC npc)
    {
        base.OnBlockMovement(npc);
        npc.AddBuff(ModContent.BuffType<GhastlyWeakness>(), 60);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void PostDraw(Color lightColor)
    {
        base.PostDraw(lightColor);
    }
}

public class GhastlyWeakness : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        if (Main.rand.NextBool(16))
        {
            Vector2 pos = npc.RandomPositionInNPCRect();
            var ms = MoonSpiralParticle.Spawn(pos, Vector2.Zero, Scale: 0.5f);
            ms.color = Color.GhostWhite;
            ms.color *= 0.5f;
        }
    }
}

public class GhastlyShieldGlobalNPC : GlobalNPC
{
    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
    {
        base.ModifyIncomingHit(npc, ref modifiers);
        if (npc.HasBuff<GhastlyWeakness>())
        {
            modifiers.FinalDamage *= 1.25f;
        }
    }
}