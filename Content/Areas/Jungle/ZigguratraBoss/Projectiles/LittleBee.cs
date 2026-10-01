using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss.Projectiles;

public class LittleBee : ModProjectile
{
    float _scale;
    float _fadingAlpha;
    Vector2 _startPosition;
    float _originalSpeed;
    public Vector2 attackStartupMidPosition;
    public Vector2 attackStartPosition;
    public Vector2 attackEndPosition;


    ref float Timer => ref Projectile.ai[0];
    ref float Style => ref Projectile.ai[1];
    ref float AttackCycle => ref Projectile.ai[2];
    
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(attackStartPosition);
        writer.WriteVector2(attackEndPosition);
        writer.WriteVector2(_startPosition);
        writer.Write(_originalSpeed);
        writer.WriteVector2(attackStartupMidPosition);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        attackStartPosition = reader.ReadVector2();

        attackEndPosition = reader.ReadVector2();
        _startPosition = reader.ReadVector2();
        _originalSpeed = reader.ReadSingle();
        attackStartupMidPosition = reader.ReadVector2();
    }
    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        _startPosition = Projectile.Center;
        _originalSpeed = Projectile.velocity.Length();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(8);
        Main.projFrames[Type] = 9;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.timeLeft = 600;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.light = 0.7f;

    }
    public override void AI()
    {
        base.AI();

        switch (Style)
        {
            case 0:
                AI_MoveTowardsPointThenMoveToEndPoint();
                break;
            case 1:
                AI_MoveTowardsPointThenMoveToEndPointRotating();
                break;
        }
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.X * 0.05f, 0.01f);
    }

    void AI_MoveTowardsPointThenMoveToEndPointRotating()
    {
        Timer++;
    }

    void AI_MoveTowardsPointThenMoveToEndPoint()
    {
        Timer++;
        _fadingAlpha = 1f;
        switch (AttackCycle)
        {
            case 0:
                {

                    Projectile.frameCounter++;
                    if(Projectile.frameCounter >= 4)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if(Projectile.frame >= 5)
                        {
                            Projectile.frame = 4;
                        }
                    }

                    Projectile.hostile = false;

                    var maxTicks = 90;
                    var ticksToMove = Vector2.Distance(_startPosition, attackStartPosition);
                    ticksToMove /= 10;
                    ticksToMove = MathHelper.Clamp(ticksToMove, 0, maxTicks);

                    var ratio = Timer / ticksToMove;
                    var ease = EasingFunction.InOutSine(ratio);

                    var lerp1 = Vector2.Lerp(_startPosition, attackStartupMidPosition, ratio);
                    var lerp2 = Vector2.Lerp(attackStartupMidPosition, attackStartPosition, ratio); 
                    var posToMoveTo = Vector2.Lerp(lerp1, lerp2, ease);
                    _scale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(ratio));
                    Projectile.Center = posToMoveTo;
                    Projectile.velocity = Vector2.Zero;
                    if (Timer >= maxTicks)
                    {
                        AttackCycle++;
                        Timer = 0;
                    }
                }
                break;
            case 1:
                {
                    if(Projectile.frame < 5)
                    {
                        Projectile.frame = 5;
                    }

                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= 4)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame >= 9)
                        {
                            Projectile.frame = 5;
                        }
                    }

                    _scale = 1f;
                    Projectile.hostile = true;
                    var normalDirection = (attackEndPosition - attackStartPosition).SafeNormalize(Vector2.Zero);
                    var velocity = normalDirection * _originalSpeed;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocity, 0.03f);

                    var dirToTarget = attackEndPosition - Projectile.Center;
                    dirToTarget = dirToTarget.SafeNormalize(Vector2.Zero);
                    var dp = Vector2.Dot(normalDirection, dirToTarget);
                    if (dp < 0)
                    {
                        AttackCycle++;
                        Timer = 0;
                    }
                }
                break;
            case 2:
                {
                    _fadingAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(Timer / 30f));
                    Projectile.hostile = false;
                }
                break;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.color *= _fadingAlpha;
        drawer.scale = new Vector2(_scale);
        Main.spriteBatch.Draw(drawer);
        OutlineRenderer.Queue(DrawOutline);
        return false;
    }

    void DrawOutline(SpriteBatch spriteBatch)
    {
        if (!Projectile.hostile)
            return;

        var drawer = Projectile.Drawer;
        drawer.color = Color.Red;
        drawer.color *= _fadingAlpha;
        Main.spriteBatch.Draw(drawer);
    }
}
