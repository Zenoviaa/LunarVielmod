using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;



/// <summary>
/// Wrapper struct for new projectile calls, reduces some of the boiler plate with spawning projectiles
/// </summary>
public record struct ProjFirer
{
    public IEntitySource source;
    public Vector2 position;
    public Vector2 velocity;
    public int type;
    public int damage;
    public float knockback;
    public int owner;
    public float ai0;
    public float ai1;
    public float ai2;

    public int New()
    {
        return Projectile.NewProjectile(source, position, velocity, type, damage, knockback, owner, ai0, ai1, ai2);
    }

    public ModProjectile NewDirect()
    {
        return Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, owner, ai0, ai1, ai2).ModProjectile;
    }

    public ProjectileType NewDirect<ProjectileType>() where ProjectileType : ModProjectile
    {
        return (ProjectileType)Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, owner, ai0, ai1, ai2).ModProjectile;
    }

    public static ProjFirer From<ProjectileType>(NPC npc)
        where ProjectileType : ModProjectile
    {
        return new ProjFirer
        {
            source = npc.GetSource_FromAI(),
            type = ModContent.ProjectileType<ProjectileType>(),
            owner = Main.myPlayer,
            position = npc.Center,
            velocity = Vector2.Zero,
            damage = npc.damage,
            knockback = 1,
            ai0 = 0,
            ai1 = 0,
            ai2 = 0
        };
    }
    public static ProjFirer Copy(Projectile projectile)
    {
        return new ProjFirer
        {
            source = projectile.GetSource_FromAI(),
            type = projectile.type,
            owner = projectile.owner,
            position = projectile.position,
            velocity = projectile.velocity,
            damage = projectile.damage,
            knockback = projectile.knockBack,
            ai0 = projectile.ai[0],
            ai1 = projectile.ai[1],
            ai2 = projectile.ai[2]
        };
    }

    public static ProjFirer From<ProjectileType>(Player player)
        where ProjectileType : ModProjectile
    {
        return new ProjFirer
        {
            source = player.GetSource_FromThis(),
            type = ModContent.ProjectileType<ProjectileType>(),
            owner = player.whoAmI,
            position = player.Center,
            velocity = Vector2.Zero,
            damage = 1,
            knockback = 1,
            ai0 = 0,
            ai1 = 0,
            ai2 = 0
        };
    }

    public static ProjFirer From<ProjectileType>(Projectile projectile)
        where ProjectileType : ModProjectile
    {
        return new ProjFirer
        {
            source = projectile.GetSource_FromThis(),
            type = ModContent.ProjectileType<ProjectileType>(),
            owner = projectile.owner,
            position = projectile.Center,
            velocity = Vector2.Zero,
            damage = projectile.damage,
            knockback = projectile.knockBack,
            ai0 = 0,
            ai1 = 0,
            ai2 = 0
        };
    }
}
