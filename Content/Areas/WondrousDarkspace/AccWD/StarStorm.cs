using Stellamod.Assets;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Stellamod.Projectiles;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.AccWD;

public class StarStorm : AbstractMeleeAddon
{
    private Dictionary<BaseSwingProjectileV2, bool> _hasShotSwingProj = new Dictionary<BaseSwingProjectileV2, bool>();
    public override void OnSpawn(BaseSwingProjectileV2 projectile)
    {
        base.OnSpawn(projectile);
        if (_hasShotSwingProj.ContainsKey(projectile))
            _hasShotSwingProj[projectile] = false;
        else
            _hasShotSwingProj.Add(projectile, false);
    }
    public override void AI(BaseSwingProjectileV2 projectile)
    {
        base.AI(projectile);
        if (!projectile.OwnedByLocalClient())
            return;
        if (projectile.MeleeWeaponType != Core.Bases.MeleeWeaponType.Scythe)
            return;


        if (!_hasShotSwingProj.ContainsKey(projectile))
            return;

        if (!_hasShotSwingProj[projectile] && projectile.Interpolant >= 0.1f)
        {
            Projectile.NewProjectile(projectile.Projectile.GetSource_FromAI(), projectile.Owner.Center,
                projectile.Projectile.velocity.SafeNormalize(Vector2.Zero) * 35, ModContent.ProjectileType<StarStormDust>(),
                (int)(projectile.Projectile.damage * 0.45f), projectile.Projectile.knockBack, projectile.Projectile.owner, 
                ai1: projectile.Projectile.identity);
            _hasShotSwingProj[projectile] = true;
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
    private float _traveledSwingRotation;
    private float _oldSwingRot;
    private Vector2 _hitboxEnd;
    private Player Owner => Main.player[Projectile.owner];
    private float Alpha => EasingFunction.InOutSine(Timer / 30f) * EasingFunction.InOutSine(Projectile.timeLeft / 30f);
    private ref float Timer => ref Projectile.ai[0];
    private int ParentProjectileType => (int)Projectile.ai[1];
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
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {

        //Check if the sword is colliding, this does a line check instead of terraria default box.

        float collisionPoint = 0f;
        bool check = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
            Projectile.Center, _hitboxEnd, 16, ref collisionPoint);
        return check;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    private void Dust()
    {
        _traveledSwingRotation += MathF.Abs(Projectile.rotation - _oldSwingRot);
        _oldSwingRot = Projectile.rotation;
        if (!Main.rand.NextBool(2))
            return;
        _traveledSwingRotation = 0f;
        Vector2 spawnPos = Vector2.Lerp(Projectile.Center, _hitboxEnd, Main.rand.NextFloat(0f, 1f));
        FaintSmokeParticle sp = FaintSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
        sp.color = DrawUtilities.InterpolateColorArray(ExtraMath.Osc(0f, 1f, speed: 32), Color.Purple, Color.LightBlue, Color.Pink, Color.LightSkyBlue);
        sp.color = Color.Lerp(sp.color, Color.Black, 0.6f);
        sp.color *= 0.15f;
        sp.fadeToColor = Color.Black * 0.5f;
        sp.Scale *= 0.8f;

        Vector2 spawnPos2 = Vector2.Lerp(Projectile.Center, _hitboxEnd, Main.rand.NextFloat(0f, 1f));
        Vector2 spawnVelocity = spawnPos2 - spawnPos;
        spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
        spawnVelocity *= 24;

        if (Main.rand.NextBool(2))
        {
            Color color = new Color(41, 43, 66);
            var sp2 = SparkleParticle.Spawn(spawnPos + Main.rand.NextVector2Circular(32, 32), spawnVelocity * 0.02f);
            sp2.gravity = 0;
            sp2.color = Color.LightSkyBlue;
            sp2.fast = true;
            sp2.noTileCollide = true;
            sp2.Scale *= 0.5f;
        }
    }
    public override void AI()
    {
        base.AI();

        Projectile parent = Projectile;
        foreach(var proj in Main.ActiveProjectiles)
        {
            if(proj.identity == ParentProjectileType)
            {
                parent = proj;
                break;
            }
        }
        Projectile.Center = Owner.Center;
        _hitboxEnd = parent.Center;
        Dust();
        Timer++;
        Projectile.velocity *= 0.96f;
        Projectile.rotation = parent.rotation;
        if (!parent.active || parent == Projectile)
            Projectile.Kill();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 pos = Vector2.Lerp(Projectile.Center, _hitboxEnd, 0.8f);
        SpritebatchDrawer smokeDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        smokeDrawer.color = Color.Purple * Alpha ;
        smokeDrawer.color.A = 0;
        smokeDrawer.worldPosition = pos;
        smokeDrawer.VerticalFrame(1, 2);
        
        Main.spriteBatch.Draw(smokeDrawer);

        SpritebatchDrawer sparkleDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sparkleDrawer.color = Color.White * ExtraMath.Osc(0f, 1f, speed: 32, Projectile.identity) * Alpha;
        sparkleDrawer.color.A = 0;
        sparkleDrawer.scale *= 0.7f;
        sparkleDrawer.worldPosition = pos;
        Main.spriteBatch.Draw(sparkleDrawer);

        SpritebatchDrawer bloomLineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.Spotlight, Projectile.Center);
        bloomLineDrawer.LeftCenterOrigin();
        bloomLineDrawer.color = DrawUtilities.InterpolateColorArray(ExtraMath.Osc(0f, 0.9f, speed: 16), Color.Purple, Color.LightBlue, Color.Pink, Color.LightSkyBlue); 
        bloomLineDrawer.color.A = 0;

        float dist = Vector2.Distance(Projectile.Center, _hitboxEnd);
        float xWidth = bloomLineDrawer.texture.Width;

        float mult = dist / xWidth;
        bloomLineDrawer.scale.X *= mult;
        bloomLineDrawer.scale.Y *= 0.1f;
        bloomLineDrawer.rotation = (_hitboxEnd - Projectile.Center).ToRotation();
        Main.spriteBatch.Draw(bloomLineDrawer);
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
