using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;
public class STARBOULDER : ModProjectile
{
    public bool _isFlying;
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private ref float Index => ref Projectile.ai[1];
    private ref float Kick => ref Projectile.ai[2];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_isFlying);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _isFlying = reader.ReadBoolean();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
        if(Kick >= 10)
        {
            Kick -= 10;
            Vector2 vel = Kick.ToRotationVector2();
            _isFlying = true;
            Projectile.netUpdate = true;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
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
