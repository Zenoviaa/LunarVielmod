using Stellamod.Common.Metaballs;
using Stellamod.Core.ProjectileHelpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

public class RekFireBreath : ModProjectile
{
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private ref float Timer => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float lineWidth = 64;
        float collisionPoint = 0;
        Vector2 position = Projectile.Center;
        Vector2 previousPosition = position + Projectile.velocity;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, lineWidth, ref collisionPoint);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.AddCommonDebuff(DebuffFlags.Burning_Serpent);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.light = 0.78f;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        if(!Parent.active || Parent.ModNPC is not RekBoss)
        {
            Projectile.active = false;
            return;
        }

        Timer++;
        if (Timer % 2 == 0)
        {
            var rekmetaballs = MetaballContent.RekFireMetaball;
            Vector2 position = Projectile.Center;
            position += Main.rand.NextVector2Circular(48, 48);
            Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
            velocity *= Main.rand.NextFloat(10, 19);
            float timeLeft = Main.rand.NextFloat(90, 130);
            rekmetaballs.Spawn(new RekFireMetaballData { position = position, velocity = velocity, radius = 0.06f, timeLeft = timeLeft });
        }
        Projectile.Center = Parent.Center;
        Projectile.velocity = Parent.rotation.ToRotationVector2() * Projectile.velocity.Length();
        Projectile.rotation = Parent.rotation;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
