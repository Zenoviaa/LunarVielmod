using Stellamod.Common.Particles;
using Stellamod.Core;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

public class STARDISC : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.hostile = true;
        Projectile.timeLeft = 360;
        Projectile.light = 0.5f;
        Projectile.penetrate = -1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;

        if(Projectile.velocity.Length() < 15)
            Projectile.velocity *= 1.03f;
        Projectile.frame = 0;
        if(Timer % 12 == 0)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = Projectile.Center,
                velocity = Main.rand.NextVector2Circular(5, 5),
                innerColor = Color.LightGoldenrodYellow.ToVector4(),
                outerColor = Color.DarkGoldenrod.ToVector4(),
                scale = Vector2.One * 0.4f
            });
        }

        if(Timer % 8 == 0)
        {
            var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(48, 48), Vector2.Zero);
            sp.innerColor = Color.PaleGoldenrod;
            sp.outerColor = Color.DarkOrange;
            sp.gravity = 0;
            sp.dampening = 0f;
            sp.Scale *= 0.4f;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X)
        {
            Projectile.velocity.X = -oldVelocity.X;
            BounceEffect();
        }
      
        if (Projectile.velocity.Y != oldVelocity.Y)
        {
            Projectile.velocity.Y = -oldVelocity.Y;
            BounceEffect();
        }
      
        return false;
    }

    private void BounceEffect()
    {
        for(int i = 0; i < 8; i++)
        {
            var sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(48, 48), Main.rand.NextVector2CircularEdge(12, 12));
            sp.innerColor = Color.PaleGoldenrod;
            sp.outerColor = Color.DarkOrange;
            sp.gravity = 0;
            sp.dampening = 0.04f;
            sp.Scale *= 0.4f;
            sp.fast = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        foreach(OldPosition oldPos in Projectile.IterateOldPosBackwards())
        {
            SpritebatchDrawer discOldDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            discOldDrawer.worldPosition = oldPos.position + Projectile.Size * 0.5f;
            discOldDrawer.color = Color.Lerp(Color.Gold, Color.Transparent, oldPos.progress) * 0.3f;
            discOldDrawer.color.A = 0;
            Main.spriteBatch.Draw(discOldDrawer);
        }

        SpritebatchDrawer discDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(discDrawer);

        discDrawer.color = Color.Gold * ExtraMath.Osc(0.5f, 1f, speed: 12);
        discDrawer.color.A = 0;
        Main.spriteBatch.Draw(discDrawer);
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
