using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Jungle.AccJN;

public class DaedenProjectileReduction : ModSystem
{
    public override void Load()
    {
        base.Load();
      //  On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += ReducedDamage;
        On_Projectile.NewProjectileDirect += ReducedDamage;
        On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += ReducedDamge;
        On_Projectile.NewProjectile_IEntitySource_Vector2_Vector2_int_int_float_int_float_float_float += ReducedDamage;
    }

    private int ReducedDamage(On_Projectile.orig_NewProjectile_IEntitySource_Vector2_Vector2_int_int_float_int_float_float_float orig
        , IEntitySource spawnSource, Vector2 position, Vector2 velocity, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1, float ai2)
    {
        return orig(spawnSource, position, velocity, Type, Damage, KnockBack, Owner, ai0, ai1, ai2);
    }

    private Projectile ReducedDamage(On_Projectile.orig_NewProjectileDirect orig, IEntitySource spawnSource, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int owner, float ai0, float ai1, float ai2)
    {
        return orig(spawnSource, position, velocity, type, damage, knockback, owner, ai0, ai1, ai2);
    }


    private int ReducedDamge(On_Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig,
        IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1, float ai2)
    {

        if (spawnSource is EntitySource_Parent entityParent)
        {
            if (entityParent.Entity is NPC n)
            {
                if (n.HasBuff<SkinNBones>())
                {
                    Damage = (int)(Damage * 0.25f);
                }
            }
        }
        return orig(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1, ai2);
    }
}
public class DaedenShield : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToShield(ModContent.ProjectileType<DaedenShieldHeld>());
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<RadiantNectar, BlankCard>();
    }
}

public class DaedenShieldHeld : AbstractShieldProjectile
{
    public override void OnBlockMovement(NPC npc)
    {
        base.OnBlockMovement(npc);
        if (npc.boss)
            return;

        npc.AddBuff(ModContent.BuffType<SkinNBones>(), 60);
    }
}

public class SkinNBones : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        if (Main.rand.NextBool(4))
        {
            Vector2 pos = npc.RandomPositionInNPCRect();
            var ms = MoonSpiralParticle.Spawn(pos, Vector2.Zero, Scale: 0.5f);
            ms.color = Color.Gold;

            ms.Scale *= 0.9f;
        }
    }
}

public class SkinNBonesPlayer : ModPlayer
{
    public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
    {
        base.ModifyHitByProjectile(proj, ref modifiers);
        if(proj.TryGetNPCParent(out NPC parent))
        {
            if (parent.HasBuff<SkinNBones>())
            {
                modifiers.IncomingDamageMultiplier *= 0.25f;
            }
        }
    }
    public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
    {
        base.ModifyHitByNPC(npc, ref modifiers);

        //Take less from contact damage
        if (Main.npc.IndexInRange(modifiers.DamageSource.SourceNPCIndex) && npc.HasBuff<SkinNBones>())
        {
            modifiers.IncomingDamageMultiplier *= 0.25f;
        }
    }
}