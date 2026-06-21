using Stellamod.Content.Areas.Abyss.AccAB;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.TheFalling.AccTF;

public class FallenBladesExtender : AbstractMeleeAddon
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
        if (!_hasShotSwingProj.ContainsKey(projectile))
            return;

        if (!_hasShotSwingProj[projectile] && projectile.Interpolant >= 0.1f)
        {
            Projectile.NewProjectile(projectile.Projectile.GetSource_FromAI(), projectile.Owner.Center,
                projectile.Projectile.velocity.SafeNormalize(Vector2.Zero) * 15, ModContent.ProjectileType<FinalExtenderBlade>(),
                (int)(projectile.Projectile.damage * 0.45f), projectile.Projectile.knockBack, projectile.Projectile.owner,
                ai1: projectile.Projectile.identity);
            _hasShotSwingProj[projectile] = true;
        }
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Gold);
        return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
    }


    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<FallenEyes, BlankAccessory>();
    }
}

public class FinalExtenderBlade : ModProjectile,
    IDrawToRenderTarget
{

    private Vector2 _hitboxStart;
    private Vector2 _hitboxSwordEnd;

    private ref float Timer => ref Projectile.ai[0];
    private int Parent => (int)Projectile.ai[1];
    private ref float DeathTimer => ref Projectile.ai[2];
    private Projectile ParentProjectile
    {
        get
        {
            foreach(var proj in Main.ActiveProjectiles)
            {
                if (proj.identity == Parent)
                    return proj;
            }
            return Projectile;
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.timeLeft = 7200;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {

        //Check if the sword is colliding, this does a line check instead of terraria default box.

        float collisionPoint = 0f;
        bool check = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
            _hitboxStart, _hitboxSwordEnd, 16, ref collisionPoint);
        return check;
    }

    private void UpdateHitbox()
    {
     //   Texture2D texture = GetTexture();
        float swordLength = 300;
        float rotation = Projectile.rotation;
        rotation -= MathHelper.PiOver4;

        Vector2 rotationVec = rotation.ToRotationVector2();
        _hitboxStart = Projectile.Center - rotationVec * swordLength;
        _hitboxSwordEnd = Projectile.Center + rotationVec * swordLength;
    }
    public override void AI()
    {
        base.AI();
        if (DeathTimer > 0 || !ParentProjectile.active)
        {
            DeathTimer++;
            if (DeathTimer >= 30)
                Projectile.Kill();
        }

        UpdateHitbox();
        Timer++;
        Projectile.Center = ParentProjectile.Center;
        Projectile.rotation = ParentProjectile.rotation;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        DrawBlade(Main.spriteBatch, Main.screenPosition);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    private void DrawBlade(SpriteBatch sb, Vector2 sp)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color = Color.Gold * ExtraMath.Osc(0.5f, 1f, speed: 16);
        drawer.color.A = 0;
        sb.Draw(drawer);
    }

    public void DrawToRenderTargets()
    {
   
    }
}
