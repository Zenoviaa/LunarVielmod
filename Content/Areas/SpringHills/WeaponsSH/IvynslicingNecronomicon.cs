using Stellamod.Common;
using Stellamod.Common.Particles;
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
        Item.DefaultToNecronomicon(1, hintColor: Color.DarkGreen);
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
    ref float Speed => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(8);
        Main.projFrames[Type] = 2;
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

        Projectile.rotation += Projectile.velocity.Length() * 0.04f;
        if(Timer == 15)
        {
            var useSound = SoundID.Item43 with { PitchVariance = 0.4f, Volume = 0.1f };
            SoundEngine.PlaySound(useSound, Projectile.position);
        }

        if(Timer >= 15)
        {
            var npc = MoonUtils.TargetClosestEnemy(Projectile.Center, 1024);
            if(npc != null)
            {
                var vel = (npc.Center - Projectile.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Speed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, vel, 0.045f);
                if(Speed < 30)
                    Speed += 0.65f;
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
        DrawUtilities.DrawAdditiveFadingTrail(Projectile, Color.DarkGreen, Color.Transparent, 0.75f);
        Main.spriteBatch.Draw(drawer);

        var drawer2 = Projectile.Drawer;
        drawer2.VerticalFrame(1, 2);
        drawer2.CenterOrigin();
        drawer2.color = Color.Lerp(Color.Transparent, Color.Green, ExtraMath.Osc(0.66f, 1f, speed: 3));
        drawer2.color.A = 0;
        Main.spriteBatch.Draw(drawer2);
        return false;
    }


    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        DeathTimer++;
        for(var i = 0; i < 10; i++)
        {
            var vel = (Projectile.Center - target.Center);
            vel = vel.SafeNormalize(Vector2.Zero);
            vel = vel.RotatedByRandom(0.3f);
            vel *= Main.rand.NextFloat(5, 15f);
            var pos = target.Center;
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.LightGreen.ToVector4(),
                outerColor = Color.DarkGreen.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f)),
                timeLeft = Main.rand.Next(30, 55)
            });
        }

    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
