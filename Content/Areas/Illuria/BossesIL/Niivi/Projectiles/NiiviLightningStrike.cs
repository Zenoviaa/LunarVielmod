using Stellamod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;

public class NiiviLightningStrike : ModProjectile
{
    enum AIState
    {
        Telegraph,
        Strike
    }
    public override string Texture => ModType.NoTexture;
    float _telegraphAlpha;
    float _randIndex;
    int NumStrikes => 16;

    Vector2 Top => Projectile.Center + new Vector2(0, -StrikeHeight);
    Vector2 TopLeft => Top + new Vector2(-StrikeRadius, 0);
    Vector2 TopRight => Top + new Vector2(StrikeRadius, 0);
    float _bottomY;
    float StrikeHeight => 768;
    float StrikeRadius => 1024;
    float StrikeTime => 30;
    ref float TelegraphTime => ref Projectile.ai[2];
    AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.timeLeft = 1200;
        Projectile.light = 0.5f;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        _randIndex = Main.rand.Next(NumStrikes / 2 - 4, NumStrikes / 2 + 2);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        for(var i = 0; i < NumStrikes; i++)
        {
            if (_randIndex == i)
                continue;
            var ratio = (float)i / (float)NumStrikes;
            var startPOsition = Vector2.Lerp(TopLeft, TopRight, ratio);
            var endPosition = startPOsition + new Vector2(0, _bottomY);
            if (CollisionHelper.LaserCollision(projHitbox, targetHitbox, startPOsition, endPosition, 32))
                return true;
        }
        return false;
    }

    public override bool CanHitPlayer(Player target)
    {
        return base.CanHitPlayer(target) && State == AIState.Strike;
    }


    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Telegraph:
                AI_Telegraph();
                break;
            case AIState.Strike:
                AI_Strike();
                break;
        }
    }

    void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            Projectile.netUpdate = true;
            Timer = 0;
            State = state;
        }
    }
    void AI_Telegraph()
    {

        Timer++;
        _telegraphAlpha = Timer / TelegraphTime;
        if (Timer >= TelegraphTime)
        {
            SwitchState(AIState.Strike);
        }
    }

    void AI_Strike()
    {
        
        Timer++;
        _telegraphAlpha = MathHelper.Lerp(1f, 0f, Timer / StrikeTime);
        if (Timer >= StrikeTime)
        {
            Projectile.Kill();
        }
    }
    
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
}
