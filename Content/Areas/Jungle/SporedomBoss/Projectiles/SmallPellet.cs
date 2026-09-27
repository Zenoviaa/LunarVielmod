using Stellamod.Core.Pixelation;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.SporedomBoss.Projectiles;

public class SmallPellet : ModProjectile,
    IDrawToRenderTarget
{
    private Vector2 FindTargetToMoveTo
    {
        get
        {
            var type = ModContent.ProjectileType<CorePellet>();
            foreach(var proj in Main.ActiveProjectiles)
            {
                if (proj.type != type)
                    continue;
                if (proj.ai[0] == Parent.whoAmI)
                {
                    return proj.Center;
                }
            }
            return Projectile.Center;
        }
    }
    private ref float InitialSpeed => ref Projectile.ai[2];
    private ref float Timer => ref Projectile.ai[1];
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 240;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            InitialSpeed = Projectile.velocity.Length();
        }
        var targetToMoveTo = FindTargetToMoveTo;
        var targetDirection = targetToMoveTo - Projectile.Center;
        targetDirection = targetDirection.SafeNormalize(Vector2.Zero);
        var targetVelocity = targetDirection * InitialSpeed;
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.2f);
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.03f);
        var distanceSquaredToTarget = Vector2.DistanceSquared(Projectile.Center, targetToMoveTo);
        var isCloseEnough = distanceSquaredToTarget < 24 * 24;
        if (isCloseEnough)
        {
            Eat();
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false; 
    }
    private void Eat()
    {
        var type = ModContent.ProjectileType<CorePellet>();
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.type != type)
                continue;
            if (proj.ai[0] == Parent.whoAmI)
            {
                proj.ai[2]++;
                proj.netUpdate = true;
                break;
            }
        }
        Projectile.Kill();
    }
    private void PollenDraw(SpriteBatch spriteBatch)
    {
        var drawer = Projectile.Drawer;
        var startScale = new Vector2(1.1f, 0.9f);
        var endScale = new Vector2(0.9f, 1.1f);
        var scale = Vector2.One;
        scale.X = MathHelper.Lerp(startScale.X, endScale.X, ExtraMath.Osc(0f, 1f, speed: 2));
        scale.Y = MathHelper.Lerp(startScale.Y, endScale.Y, ExtraMath.Osc(0f, 1f, speed: 2, offset: 3.14f));
        drawer.scale *= scale;
        spriteBatch.Draw(drawer);
    }
    private void PollenDrawEvil(SpriteBatch spriteBatch)
    {
        var drawer = Projectile.Drawer;
        var startScale = new Vector2(1.1f, 0.9f);
        var endScale = new Vector2(0.9f, 1.1f);
        var scale = Vector2.One;
        scale.X = MathHelper.Lerp(startScale.X, endScale.X, ExtraMath.Osc(0f, 1f, speed: 2));
        scale.Y = MathHelper.Lerp(startScale.Y, endScale.Y, ExtraMath.Osc(0f, 1f, speed: 2, offset: 3.14f));
        drawer.scale *= scale;
        drawer.color = Color.Red;
        spriteBatch.Draw(drawer);
    }
    public void DrawToRenderTargets()
    {
        PollenSpitRenderer.DrawActionQueue.Enqueue(PollenDraw);
        OutlineRenderer.Queue(PollenDrawEvil);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
