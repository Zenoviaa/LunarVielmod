using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.BossesCS.Rek.Projectiles;

public class ZoomWheel : ModProjectile
{
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
        Projectile.friendly = false;
        Projectile.light = 0.78f;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        base.AI();
        if (Parent.ModNPC is not RekBoss boss)
        {
            Projectile.active = false;
            return;
        }
        if(Projectile.velocity.Length() < 25)
            Projectile.velocity *= 1.03f;
    }

    public override bool ShouldUpdatePosition()
    {
        return true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class StartupWheel : ModProjectile
{
    private enum SpinState
    {
        Startup,
        SpinFastNRam
    }
    private float _timer;
    private Vector2 _initialPosition;
    private NPC Parent => Main.npc[(int)Projectile.ai[0]];
    private ref float MoveTime => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_timer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _timer = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 384;
        Projectile.height = 384;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
        Projectile.friendly = false;
        Projectile.light = 0.78f;
    }
    public override void AI()
    {
        base.AI();
        if (Parent.ModNPC is not RekBoss boss)
        {
            Projectile.active = false;
            return;
        }

        AI_Startup();
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    private void AI_Startup()
    {
        _timer++;
        if (_timer == 1)
        {
            _initialPosition = Projectile.Center;
        }
        float ratio = _timer / MoveTime;
        float ease = EasingFunction.InOutExpo(ratio);
        Vector2 pointToMoveTo = Vector2.Lerp(_initialPosition, Projectile.velocity, ease);
        Projectile.Center = pointToMoveTo;
        if (_timer >= MoveTime)
        {
            Projectile.Kill();
        }
    }


    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
        //    return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
