using ReLogic.Content;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;

//Stamina will charge and then fire multiple bolts, it'll be so cool
//You'll hold it btw

public class Swingaling : BaseSwingItemV2
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 11;
        Item.shoot = ModContent.ProjectileType<SwingalingSlash>();
        staminaProjectileShoot = ModContent.ProjectileType<SwingalingCharge>();
        meleeWeaponType = MeleeWeaponType.Sword;
        staminaCost = 2;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<MarshScrap>());
    }
}

public class SwingalingSlash : BaseSwingProjectileV2
{
    public override void DefineCombo()
    {
        base.DefineCombo();
    }
    public override Asset<Texture2D> RequestHologramTexture()
    {
        return TextureRegistry.GlowSword_Chillrend;
    }

    public override void AI()
    {
        base.AI();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
    }
}

public class SwingalingCharge : ModProjectile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
    }

    public override bool ShouldUpdatePosition()
    {
        return base.ShouldUpdatePosition();
    }

    public override void AI()
    {
        base.AI();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}