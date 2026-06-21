using Stellamod.Content.CommonMaterials;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.PunkerTown.AccPT;

public class ThrowingFinish : AbstractMeleeAddon
{
    public override void DefineCombo(BaseSwingProjectileV2 projectile)
    {
        base.DefineCombo(projectile);
        if (projectile.MeleeWeaponType != Core.Bases.MeleeWeaponType.Greatsword)
            return;

        SoundStyle swingSound3 = SoundRegistry.NSwordSpin1;
        swingSound3.PitchVariance = 0.5f;
        swingSound3.Volume = 0.5f;

        var swing = new OvalSwing
        {
            Duration = 100,
            XSwingRadius = 1,
            YSwingRadius = 1,
            SwingDegrees = 2000,
            SpinThrowDistance = 40,
            SpinDegrees = 1,
            AlwaysShowTrail = true,
            Easing = (float lerpValue) => lerpValue,
            Sound = swingSound3,
            HitCount = 6
        };

        projectile.MakeFinisher(swing);
    }

    public override void OnModifyHitNPC(BaseSwingProjectileV2 projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        base.OnModifyHitNPC(projectile, target, ref modifiers);
        if (projectile.IsFinishingSwing())
        {
            modifiers.FinalDamage += 0.25f;
        }
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MarshScrap, BlankAccessory>();
    }
}
