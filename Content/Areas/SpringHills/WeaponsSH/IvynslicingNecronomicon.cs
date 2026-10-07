using Stellamod.Common;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH;

public class IvynslicingNecronomicon :ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToNecronomicon(hintColor: Color.DarkGreen);
        Item.damage = 12;
        Item.shoot = ModContent.ProjectileType<Ivynslicer>();
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankRune>();
    }
}

public class Ivynslicer : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    ref float DeathTimer => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(8);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.WidthAndHeight = 24;
        Projectile.friendly = true;
        Projectile.LocalHitOnce = true;
        Projectile.tileCollide = false;
        Projectile.light = 0.6f;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1 && this.OwnedByLocalClient())
        {
            Projectile.velocity.X *= 0.05f;
            Projectile.velocity.Y = -15 * Main.rand.NextFloat(0.6f, 1f);
            Projectile.velocity = Projectile.velocity.RotatedByRandom(0.75f);
            Projectile.netUpdate = true;
        }
        Projectile.rotation = MathF.Sign(Projectile.velocity.X) * 0.15f;
        if(Timer == 30)
        {
            var useSound = SoundID.Item43 with { PitchVariance = 0.4f, Volume = 0.1f };
            SoundEngine.PlaySound(useSound, Projectile.position);
        }

        if(Timer >= 30)
        {
            var npc = MovementUtilities.TargetClosestEnemy(Projectile.Center, 1024);
            if(npc != null)
            {
                var vel = (npc.Center - Projectile.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= 12;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, vel, 0.02f);
            }
        }

        if (Main.rand.NextBool(12))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = Main.rand.NextVector2Circular(2, 2);
            Dust.NewDustPerfect(pos, DustID.Dirt, vel, Scale: Main.rand.NextFloat(0.75f, 0.95f));
        }
      
        if (DeathTimer > 0)
            DeathTimer++;
        if (DeathTimer >= 30)
            Projectile.Kill();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var inScale = EasingFunction.InOutSine(Timer / 30f);
        var outScale = EasingFunction.InOutSine(DeathTimer / 30f);
        outScale = 1f - outScale;
        Projectile.scale = inScale * outScale;
        var drawer = Projectile.Drawer;
        DrawUtilities.DrawAdditiveFadingTrail(Projectile, Color.DarkGreen, Color.Transparent, 0.15f);
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        DeathTimer++;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
