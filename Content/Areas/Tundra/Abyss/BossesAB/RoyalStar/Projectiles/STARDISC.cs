using Stellamod.Common.Particles;
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
        Main.projFrames[Type] = 4;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.hostile = true;
        Projectile.timeLeft = 180;
        Projectile.light = 0.5f;
        Projectile.penetrate = -1;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        Main.projFrames[Type] = 4;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        Projectile.frame = (int)((Timer / 4) % Main.projFrames[Type]);
        if(Timer % 3 == 0)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = Projectile.Center,
                velocity = Main.rand.NextVector2Circular(5, 5),
                innerColor = Color.LightGoldenrodYellow.ToVector4(),
                outerColor = Color.DarkGoldenrod.ToVector4(),
            });
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
