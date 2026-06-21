using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.AccWD;

public class StarStorm : AbstractMeleeAddon
{
    public override void AI(BaseSwingProjectileV2 projectile)
    {
        base.AI(projectile);
        if (!projectile.OwnedByLocalClient())
            return;
        if (projectile.MeleeWeaponType != Core.Bases.MeleeWeaponType.Scythe)
            return;
        if (projectile.Timer % 24 == 0)
        {
            var proj = projectile.Projectile;
            Vector2 randomPosition = Vector2.Lerp(projectile.Projectile.Center, projectile.Owner.Center, Main.rand.NextFloat(0.00f, 1.00f));
            Projectile.NewProjectile(proj.GetSource_FromAI(), randomPosition, Main.rand.NextVector2Circular(4, 4),
                ModContent.ProjectileType<StarStormDust>(), proj.damage, proj.knockBack, proj.owner);
        }
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<HypnotizedSoul, BlankAccessory>();
    }
}

public class StarStormDust : ModProjectile,
    IDrawToRenderTarget
{
    private float Alpha => EasingFunction.InOutSine(Timer / 30f) * EasingFunction.InOutSine(Projectile.timeLeft / 30f);
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 20;
        Projectile.timeLeft = 90;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.velocity *= 0.96f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer smokeDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        smokeDrawer.color = Color.Purple * Alpha;
        smokeDrawer.color.A = 0;
        Main.spriteBatch.Draw(smokeDrawer);

        SpritebatchDrawer sparkleDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sparkleDrawer.color = Color.White * ExtraMath.Osc(0.6f, 1f, speed: 16, Projectile.identity) * Alpha;
        sparkleDrawer.color.A = 0;
        Main.spriteBatch.Draw(sparkleDrawer);

        return false;
        //return base.PreDraw(ref lightColor);
    }
    private void DrawOutline(SpriteBatch sb)
    {
        /*
        SpritebatchDrawer smokeDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        smokeDrawer.color = Color.Yellow * Alpha * ExtraMath.Osc(0.6f, 1f, speed: 16, Projectile.identity);
        smokeDrawer.color.A = 0;
        Main.spriteBatch.Draw(smokeDrawer);*/
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {
        OutlineRenderer.Queue(DrawOutline);
    }
}
