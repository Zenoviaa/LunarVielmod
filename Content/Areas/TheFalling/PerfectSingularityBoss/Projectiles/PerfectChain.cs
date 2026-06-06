using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.IO;
using System.Net;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.TheFalling.PerfectSingularityBoss.Projectiles;

public class PerfectChain : ModProjectile,
    IDrawToRenderTarget
{
    private enum AIState
    {
        ChainWhip,
        ChainJail,
        ChainLinger
    }

    private bool _impactGround;
    private ref float Timer => ref Projectile.ai[0];
    private AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }

    private Vector2 _movementDirection;
    private Vector2 ChainWhip_Start => Projectile.Center;
    private Vector2 ChainWhip_End => Projectile.Center + Projectile.velocity;
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return base.Colliding(projHitbox, targetHitbox);
    }
    public override bool CanHitPlayer(Player target)
    {
        return base.CanHitPlayer(target);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.timeLeft = 300;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_movementDirection);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _movementDirection = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void AI()
    {
        base.AI();
       
        switch (State)
        {
            case AIState.ChainWhip:
                AI_ChainWhip();
                break;
        }
    }
    private void AI_ChainWhip()
    {
        Timer++;
        if (!_impactGround && Timer > 25)
        {
            _impactGround = true;
        }
        if (Main.rand.NextBool(64))
        {
            Vector2 pos = Vector2.Lerp(ChainWhip_Start, ChainWhip_End, Main.rand.NextFloat(0f, 1f));
            var dp = DustParticle.Spawn(pos + Main.rand.NextVector2Circular(32, 32), (ChainWhip_End - ChainWhip_Start).SafeNormalize(Vector2.Zero));
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.dampening = 0.05f;
        }
        if(Timer == 100)
        {
            Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 4000);
            if(player != null)
            {
                Vector2 dirToPlayer = (ChainWhip_End - player.Center);
                dirToPlayer = dirToPlayer.SafeNormalize(Vector2.Zero);
                _movementDirection = dirToPlayer;
            }
        }
        if(Timer >= 100)
        {
            Vector2 aimVelocity = Projectile.velocity.MoveTowards(_movementDirection, MathHelper.Lerp(0f, 1f, EasingFunction.QuickOutSlowIn(Timer / 60f)));
            float newLength = ProjectileHelper.PerformBeamHitscan(ChainWhip_Start, aimVelocity, 3000);
            Projectile.velocity = aimVelocity.Resize(newLength);
            Projectile.hostile = true;
        }
    }
    public override bool PreDraw(ref Color lightColor) => false;
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        float numDust = 32;
        for(float f = 0; f < numDust; f++)
        {
            float ratio = Main.rand.NextFloat(0f, 100f) / 100f;
            Vector2 pos = Vector2.Lerp(ChainWhip_Start, ChainWhip_End, ratio);
            Vector2 vel = (ChainWhip_End - ChainWhip_Start).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5f, 15f);
            var dp = DustParticle.Spawn(pos, vel);
            dp.dampening = 0.05f;
            dp.gravity = 0;
            dp.noTileCollide = true;
            dp.outerColor = Color.DarkGray;
            dp.Scale *= Main.rand.NextFloat(0.7f, 1.6f);
        }
    }
    private void DrawWhipInner(SpriteBatch sb, Color? overridecolor = null)
    {
        Color drawColor = Color.White;
        if (overridecolor.HasValue)
            drawColor = overridecolor.Value;
        Vector2 start = ChainWhip_Start;
        Vector2 end = ChainWhip_End;

        float scale = 0.5f;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.scale *= scale;
        float inRatio = EasingFunction.OutExpo(Timer / 30f);
        float spriteLength = drawer.texture.Width * scale;
        float dist = Vector2.Distance(start, end) * inRatio;
        float numPoints = dist / spriteLength;
        Vector2 p = start;
        Vector2 dir = (end - start).SafeNormalize(Vector2.Zero) * spriteLength;
        drawer.rotation = dir.ToRotation();

        for (float f = 0; f < numPoints; f++)
        {
            drawer.color = drawColor * 0.86f;
            drawer.color.A = 0;
            drawer.worldPosition = p;
            sb.Draw(drawer);
            p += dir;
        }
    }
    private void DrawWhipOutline(SpriteBatch sb) => DrawWhipInner(sb, Projectile.hostile ? Color.Red : Color.Yellow);
    private void DrawPixelatedWhip(SpriteBatch sb, Vector2 screenPos)
    {
        DrawWhipInner(sb);
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }

    public void DrawToRenderTargets()
    {
        //OutlineRenderer.Queue(DrawWhipOutline);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedWhip);
    }
}
