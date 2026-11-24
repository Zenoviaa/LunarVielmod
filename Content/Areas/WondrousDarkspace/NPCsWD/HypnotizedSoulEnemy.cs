using Stellamod.Common;
using Stellamod.Common.ClassReworkSystem;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Astar;
using Stellamod.Core.Effects;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering;
using Stellamod.Trailing;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Content.Areas.WondrousDarkspace.NPCsWD;
public class HypnotizedSoulModPlayer : ModPlayer
{
    public Vector2? targetSuckPosition;
    public Vector2? resetVelocity;
    public override void PreUpdateMovement()
    {
        base.PreUpdateMovement();
        if (targetSuckPosition.HasValue)
        {
            Vector2 suckPosition = targetSuckPosition.Value;
            Vector2 velocityToPosition = (suckPosition - Player.Center);
            Player.velocity = Vector2.Lerp(Player.velocity, velocityToPosition, 0.5f);
            targetSuckPosition = null;
        }
        if (resetVelocity.HasValue)
        {
            Player.velocity = resetVelocity.Value;
            resetVelocity = null;
        }
    }
}
public class HypnotizedSoulEnemy : ModNPC,
    IDrawToRenderTarget
{
    Outliner _outliner;
    private int _frame;
    private ITrailer _trailer;
    private ref float Timer => ref NPC.ai[0];
    private enum AIState
    {
        Idle,
        Wander,
        Chase,
        Suck
    }

    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    float _trailingAlpha;
    private ref float WanderAngle => ref NPC.ai[2];
    private ref float TargetWanderAngle => ref NPC.ai[3];

    private Vector2 _scale;
    private Vector2 _suckOffset;
    private Color OutlineColor;
    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            TargetWanderAngle = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_suckOffset);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _suckOffset = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 12;
        NPCID.Sets.TrailCacheLength[Type] = 64;
        NPCID.Sets.TrailingMode[Type] = 1;
        this.AddToDarkspace();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _scale = Vector2.One;
        NPC.lifeMax = 120;
        NPC.width = 32;
        NPC.height = 32;
        NPC.defense = 0;
        NPC.damage = 20;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.HitSound = SoundID.NPCHit36;
        NPC.DeathSound = SoundID.NPCDeath39;
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frameCounter += 0.15f;
        if (NPC.frameCounter >= 1f)
        {
            _frame++;
            NPC.frameCounter = 0f;
        }

        switch (State)
        {
            default:
            case AIState.Idle:
                if (_frame >= 4f)
                {
                    _frame = 0;
                }
                break;
            case AIState.Wander:
                if (_frame >= 4f)
                {
                    _frame = 0;
                }
                break;
            case AIState.Chase:
                if (_frame < 4)
                {
                    _frame = 4;
                }

                if (_frame >= 8)
                {
                    _frame = 4;
                }
                break;
            case AIState.Suck:
                if (_frame < 8)
                {
                    _frame = 8;
                }
                if (_frame >= 12f)
                {
                    _frame = 8;
                }
                break;

        }

        NPC.frame.Y = frameHeight * _frame;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return State == AIState.Suck;
    }

    public override void AI()
    {
        base.AI();
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Wander:
                AI_Wander();
                break;
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.Suck:
                AI_Suck();
                break;
        }

        if (State == AIState.Chase)
        {
            _trailingAlpha = MathHelper.Lerp(_trailingAlpha, 1f, 0.1f);
        }
        else
        {
            _trailingAlpha *= 0.96F;
        }
        _outliner.Update();
        Lighting.AddLight(NPC.Center, Vector3.One);
    }
    private void AI_Idle()
    {
        Timer++;
        if (Timer >= 60)
        {
            SwitchState(AIState.Wander);
        }
        NPC.velocity *= 0.98f;
    }

    private void AI_Wander()
    {
        Timer++;
        if (MultiplayerHelper.IsHost && Timer % 200 == 0)
        {
            TargetWanderAngle = MoonUtils.Rand.NextFloat(1f) * MathHelper.TwoPi;
            NPC.netUpdate = true;
        }

        float rotAccel = 0.005f;
        if (WanderAngle < TargetWanderAngle)
        {
            WanderAngle += rotAccel;
        }
        else if (WanderAngle > TargetWanderAngle)
        {
            WanderAngle -= rotAccel;
        }

        Vector2 targetVelocity = WanderAngle.ToRotationVector2();
        Vector2 wanderVelocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
        NPC.velocity = wanderVelocity;
        NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
        NPC.rotation = NPC.velocity.X * 0.02f;
        NPC.spriteDirection = -NPC.direction;
        NPC.noTileCollide = true;
        NPC.TargetClosest();
        if (NPC.HasValidTarget && Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) < 180)
        {
            SwitchState(AIState.Chase);
        }
    }

    Projectile NearestShield()
    {
        Projectile nearest = null;
        var dist = 99999F;
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.ModProjectile is not MeleeShield shield)
                continue;

            var distTo = Vector2.DistanceSquared(NPC.Center, proj.Center);
            if (distTo < dist)
            {
                nearest = proj;
                dist = distTo;
            }

        }
        return nearest;
    }
    private void AI_Chase()
    {
        Timer++;
        _outliner.warning = true;
      //  OutlineColor = Color.Lerp(OutlineColor, Color.Yellow, 0.1f);
        if (MoonUtils.Rand.NextBool(8))
        {
            var pos = NPC.Center + Main.rand.NextVector2Circular(48, 48);
            PS.HypnoticSparkle.Spawn(new()
            {
                position = pos,
                timeLeft = 60,
                scale = 1,
                rot = 0,
            });
        }

        Player target = Main.player[NPC.target];
        Vector2 velocityToTarget = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        if (Timer == 1)
        {
            NPC.velocity -= velocityToTarget * 4;
            NPC.velocity.Y -= 2;
        }

        float distanceToTarget = Vector2.Distance(NPC.Center, target.Center);
        float maxSpeed = MathHelper.Clamp(distanceToTarget, 3, 6);
        if (Timer < 60)
        {
            maxSpeed *= 0.2f;
        }
        velocityToTarget *= maxSpeed;

        var nearest = NearestShield();
        if (nearest != null)
        {
            var shieldsFacingDirection = (nearest.Center - nearest.PlayerOwner.Center).SafeNormalize(Vector2.Zero);
            var inverseShieldFacingDirection = -shieldsFacingDirection;
            var myMovementDirection = velocityToTarget.SafeNormalize(Vector2.Zero);
            var dp = Vector2.Dot(myMovementDirection, inverseShieldFacingDirection);
            if (dp > 0.5F)
            {
                velocityToTarget = velocityToTarget.RotatedBy(MathHelper.PiOver2);
            }
        }
        NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToTarget, 0.1f);



        NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
        NPC.spriteDirection = -NPC.direction;
        NPC.rotation = NPC.velocity.X * 0.01f;
        if (Timer < 60)
        {
            NPC.rotation += ExtraMath.Osc(-0.7f, 0.7f, speed: 3) * MathHelper.Lerp(1f, 0f, Timer / 60F);
        }
        NPC.noTileCollide = true;
        if (!NPC.HasValidTarget)
        {
            SwitchState(AIState.Idle);
        }
        else if (distanceToTarget <= 4)
        {
            _suckOffset = target.Center - NPC.Center;
            SwitchState(AIState.Suck);
        }
    }

    private void AI_Suck()
    {
        Timer++;
        //OutlineColor = Color.Lerp(OutlineColor, Color.Red, 0.1f);
        _outliner.attacking = true;
        NPC.noTileCollide = false;


        if (!NPC.HasValidTarget)
        {
            SwitchState(AIState.Idle);
        }
        else
        {
            Player target = Main.player[NPC.target];
            HypnotizedSoulModPlayer hypnotizedSoulModPlayer = target.GetModPlayer<HypnotizedSoulModPlayer>();
            hypnotizedSoulModPlayer.targetSuckPosition = NPC.Center + Vector2.UnitY * 32;

            if (Timer % 8 == 0)
            {
                Vector2 spawnPoint = hypnotizedSoulModPlayer.targetSuckPosition.Value + Main.rand.NextVector2CircularEdge(128, 128);
                Vector2 velocity = (hypnotizedSoulModPlayer.targetSuckPosition.Value - spawnPoint) * 0.1f;
                var p = FXUtil.GlowStretch(spawnPoint, velocity);
                p.VectorScale *= 0.5f;
            }


            if (Timer >= 200)
            {
                //    NPC.velocity *= 0.5f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.UnitY * MathF.Sin(Timer * 0.1f), 0.1f);
            }
            else
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, -Vector2.UnitY * 1f + Vector2.UnitY * MathF.Sin(Timer * 0.1f), 0.1f);
            }

            //  NPC.Center = target.Center + _suckOffset;
            NPC.spriteDirection = -NPC.direction;
        }
    }
    private Color GetColorFunction(float completionRatio)
    {
        Color trailColor = DrawUtilities.InterpolateColorArray(completionRatio, Color.White, Color.SkyBlue, Color.DarkBlue, Color.DarkViolet);
        //trailColor.ScrollHue(ExtraMath.Osc())
        trailColor *= MathHelper.SmoothStep(1f, 0f, completionRatio);
        return trailColor;
    }

    private float GetWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(32, 0, completionRatio) * _trailingAlpha;
    }

    private float GetWidthFunction2(float completionRatio)
    {
        return GetWidthFunction(completionRatio) * 2f;
    }


    void DrawGlowyTrail(GraphicsDevice gDevice)
    {
        var drawColor = Color.Cyan * _trailingAlpha;
        _trailer ??= TrailPresets.HypnotizedSoul;
        _trailer.DrawTrail(ref drawColor, NPC.oldPos);
    }
    private void DrawTrail(GraphicsDevice gDevice)
    {

        var shader2 = FixedRichLaserShader.Instance;
        shader2.LaserColor = Color.DarkBlue;
        shader2.LaserTexture = AssetReferences.Assets.LaserTextures.TexturedLaser.Asset;
        shader2.InnerColor = Color.SkyBlue;
        shader2.OuterColor = Color.White;
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, GetColorFunction, GetWidthFunction, shader2, NPC.Size * 0.5f);

        var bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.SkyBlue * 0.5f;
        bloom.OuterColor = Color.DarkViolet;
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, GetColorFunction, GetWidthFunction2, bloom, NPC.Size * 0.5f);
    }
    void DrawWhite(SpriteBatch sb)
    {
        Vector2 drawOrigin = NPC.frame.Size() / 2;
        SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawPos = NPC.position - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
        sb.Draw(texture, drawPos, NPC.frame, _outliner.outlineColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0);
    }
    public void DrawToRenderTargets()
    {
        DrawLayerHooks.OverWaterDrawActions.Enqueue(DrawHypnotizedSoul);
        if (_outliner.VisibleEnough)
        {
            OutlineRenderer.Queue(DrawWhite);
        }
        if (_trailingAlpha < 0.02f)
            return;
        //PixelationManager.QueueSpritebatchDrawAction(DrawStar);
        PixelationManager.QueuePrimitivesDrawAction(DrawGlowyTrail, DrawLayer.OverNPCsAdditive);
        PixelationManager.QueuePrimitivesDrawAction(DrawTrail, DrawLayer.BehindNPCsWithOutline);
    }

    void DrawHypnotizedSoul(SpriteBatch spriteBatch)
    {
        Color drawColor = Lighting.GetColor(NPC.Center.ToTileCoordinates());
        if (State == AIState.Suck)
        {
            Player target = Main.player[NPC.target];
            if (NPC.HasValidTarget)
            {
                List<Vector2> suckPositions = new List<Vector2>();
                float num = 16f;
                for (float f = 0; f < num; f++)
                {
                    suckPositions.Add(Vector2.Lerp(NPC.Center, target.Center, f / num));
                }
                Vector2[] suckPos = suckPositions.ToArray();
                _trailer ??= TrailPresets.HypnotizedSoul;
                _trailer.DrawTrail(ref drawColor, suckPos);
            }
        }

        Vector2 drawOrigin = NPC.frame.Size() / 2;
        SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawPos = NPC.position - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
        spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(spriteBatch, screenPos, drawColor);

        Texture2D texture2D4 = AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset.Value;
        var drawer = SpritebatchDrawer.FromTextureWhite(texture2D4);
        drawer.worldPosition = NPC.Center;
        drawer.color = Color.SkyBlue * 0.15f;
        drawer.color.A = 0;
        drawer.scale *= 0.6F;
        spriteBatch.Draw(drawer);
        //in.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color(255, 128, 125, 0), NPC.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);

    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HypnotizedSoul>(), minimumDropped: 2, maximumDropped: 4));
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        //float boomSize = Main.rand.NextFloat(0.025f, 0.08f);
        var fx = FXUtil.GlowCircleBoom(NPC.Center,
            innerColor: Color.White,
            glowColor: Color.LightBlue,
            outerGlowColor: Color.Blue, duration: 25, baseSize: 0.12F);
        fx.Scale *= 1.4F;
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(NPC.Center, TriColorPalette.Moonly, new Vector2(5, 35));
        fac.particleCount *= 2;
        ParticleUtils.CreateSwirlingDustCircle(fac);
    }
    public override void OnKill()
    {
        base.OnKill();

    }
}
