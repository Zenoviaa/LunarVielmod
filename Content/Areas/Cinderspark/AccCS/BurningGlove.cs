using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Cinderspark.WeaponsCS;
using Stellamod.Content.Areas.PunkerTown.ItemsPT;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.AccCS;

public class BurningGlove : AbstractMeleeAddon
{
    private Dictionary<int, (float, float)> _fireTimer = new Dictionary<int, (float, float)>();
    private Dictionary<int, bool> _hasShotSwingProj = new Dictionary<int, bool>();
    public override void OnSpawn(BaseSwingProjectileV2 projectile)
    {
        base.OnSpawn(projectile);
        int id = projectile.Projectile.identity;
        if (_hasShotSwingProj.ContainsKey(id))
            _hasShotSwingProj[id] = false;
        else
            _hasShotSwingProj.Add(id, false);


        if (_fireTimer.ContainsKey(id))
            _fireTimer[id] = (0, 0);
        else
            _fireTimer.Add(id, (0, 0));
    }


    public override void AI(BaseSwingProjectileV2 projectile)
    {
        base.AI(projectile);
        if (!projectile.OwnedByLocalClient())
            return;
        var proj = projectile.Projectile;
        int id = projectile.Projectile.identity;

        if (_fireTimer.ContainsKey(id))
        {
            (float oldRot, float traveled) = _fireTimer[id];
            traveled += MathF.Abs(proj.rotation - oldRot);
            oldRot = proj.rotation;

            if (traveled >= 0.4f)
            {
                Projectile.NewProjectile(proj.GetSource_FromAI(), proj.Center, proj.rotation.ToRotationVector2() * 2,
                ModContent.ProjectileType<IncineratorProj>(), (int)(proj.damage * 0.25f), proj.knockBack, proj.owner);
                traveled = 0;
                _fireTimer[id] = (oldRot, traveled);
            }
        }

        if (!projectile.IsThrust())
            return;

        if (!_hasShotSwingProj.ContainsKey(id))
            return;

        if (!_hasShotSwingProj[id] && projectile.Interpolant >= 0.1f)
        {
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(projectile.Projectile.GetSource_FromAI(), projectile.Owner.Center,
                              projectile.Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.1) * 15, ModContent.ProjectileType<MoltenManaBlast>(),
                              (int)(projectile.Projectile.damage * 0.45f), projectile.Projectile.knockBack,
                              projectile.Projectile.owner);
            }

            _hasShotSwingProj[id] = true;
        }
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Cinderscrap, BlankAccessory>();
    }
}