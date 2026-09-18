using Stellamod.Buffs.Minions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Minions;

public class VoidMinionProj : ModProjectile
{
    Player Owner => Main.player[Projectile.owner];
    ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Jelly Minion");
        // Sets the amount of frames this minion has on its spritesheet
        // This is necessary for right-click targeting
        ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        // These below are needed for a minion
        // Denotes that this projectile is a pet or minion
        Main.projPet[Projectile.type] = true;
        // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
        ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
        ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
    }

    public sealed override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 28;
        // Makes the minion go through tiles freely
        Projectile.tileCollide = false;

        // These below are needed for a minion weapon
        // Only controls if it deals damage to enemies on contact (more on that later)
        Projectile.friendly = true;
        // Only determines the damage type
        Projectile.minion = true;
        // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
        Projectile.minionSlots = 2f;
        // Needed so the minion doesn't despawn on collision with enemies or tiles
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    // Here you can decide if your minion breaks things like grass or pots
    public override bool? CanCutTiles()
    {
        return false;
    }

    // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
    public override bool MinionContactDamage()
    {
        return true;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }

    public override void AI()
    {
        if (!SummonHelper.CheckMinionActive<VoidMinionBuff>(Owner, Projectile))
            return;

        SummonHelper.SearchForTargets(Owner, Projectile,
            out bool foundTarget,
            out float distanceFromTarget,
            out Vector2 targetCenter);
        if (foundTarget)
        {
            Timer++;
            Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
            Vector2 offset = -directionToTarget * 200;
            Projectile.velocity = VectorHelper.VelocitySlowdownTo(Projectile.Center, targetCenter + offset, 8);
            if (Timer > 20 && Timer % 7 == 0 && Timer < 100)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
                Vector2 velocity = directionToTarget * 16;
                velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<VoidMinionSparkProj>(), Projectile.damage, Projectile.knockBack, Owner: Projectile.owner);
            }

            if (Timer > 100 && Timer < 150)
            {
                //Idle
                SummonHelper.CalculateIdleValues(Owner, Projectile,
                    out Vector2 vectorToIdlePosition,
                    out float distanceToIdlePosition);
                SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
            }

            if (Timer >= 150)
            {
                Timer = 0;
            }
        }
        else
        {
            //Idle
            SummonHelper.CalculateIdleValues(Owner, Projectile,
                out Vector2 vectorToIdlePosition,
                out float distanceToIdlePosition);
            SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
        }
        Visuals();
    }

    private void Visuals()
    {
        // So it will lean slightly towards the direction it's moving
        Projectile.rotation = Projectile.velocity.X * 0.05f;

        // This is a simple "loop through all frames from top to bottom" animation
        int frameSpeed = 5;
        Projectile.frameCounter++;
        if (Projectile.frameCounter >= frameSpeed)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
        }

        // Some visuals here
        Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
    }
}
