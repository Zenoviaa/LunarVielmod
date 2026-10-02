using Stellamod.Common.Particles;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;

public class PearlbornSlime : ModNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Idle,
        Jump,
        JumpTo,
        CircularSpike,
        SniperSpike
    }

    Outliner _outliner;
    Vector2 _squishScale;
    Vector2 _jumpStartPosition;
    Vector2 _jumpEndPosition;
    int _frame;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    float IdleTime => 90;
    float ChaseDistance => 384 * 384;
    float JumpHeight => -256;
    float CircularSquishTime => 100;
    float SniperSpikeSquishTime => 120;
    int DamagePearlbornSpike => 30;

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PearlescentScrap>(), minimumDropped: 2, maximumDropped: 4));
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_jumpEndPosition);
        writer.WriteVector2(_jumpStartPosition);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        _jumpEndPosition = reader.ReadVector2();
        _jumpStartPosition = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 2;
        NPCID.Sets.TrailCacheLength[Type] = 32;
        NPCID.Sets.TrailingMode[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 32;
        NPC.height = 32;
        NPC.knockBackResist = 0;
        NPC.damage = 24;
        NPC.lifeMax = 200;
        NPC.defense = 15;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frameCounter += 0.15f;
        if (NPC.frameCounter >= 1f)
        {
            NPC.frameCounter = 0;
            _frame++;
            _frame %= Main.npcFrameCount[Type];
        }
        NPC.frame.Y = frameHeight * _frame;
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
            case AIState.Jump:
                AI_Jump();
                break;
            case AIState.JumpTo:
                AI_JumpTo();
                break;
            case AIState.CircularSpike:
                AI_CircularSpike();
                break;
            case AIState.SniperSpike:
                AI_SniperSpike();
                break;
        }
        _outliner.Update();
        if (Main.rand.NextBool(4))
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = NPC.Center + Main.rand.NextVector2Circular(24, 24),
                velocity = Main.rand.NextVector2Circular(6, 6),
                innerColor = Color.SkyBlue.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.7f, 1.2f)),
                timeLeft = 120
            });
        }
    }

    void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            AttackCycle = 0;
            NPC.netUpdate = true;
        }
    }

    void AI_Idle()
    {
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
        Timer++;
        if (Timer == 1 || !NPC.HasValidTarget)
            NPC.TargetClosest();

        var sqrDist = Vector2.DistanceSquared(NPC.Center, Main.player[NPC.target].Center);
        if (sqrDist <= ChaseDistance)
        {
            SwitchState(AIState.JumpTo);
        }
        NPC.velocity.X *= 0.96f;
        if (Timer >= IdleTime)
        {
            SwitchState(AIState.Jump);
        }
    }


    public GlowDonutParticle MakeDonut(Vector2 position, Vector2 velocity)
    {
        var p = LegacyParticle.NewParticle<GlowDonutParticle>(position, velocity);
        p.innerColor = Color.White;
        p.outerColor = Color.DarkBlue;
        p.fadeToColor = Color.Black;
        return p;
    }

    void AI_Jump()
    {
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
        Timer++;
        if (Timer == 1)
        {
            NPC.velocity.Y = -15;
            if (MultiplayerHelper.IsHost)
            {
                NPC.velocity.X = (Main.rand.NextBool(2) ? -1 : 1) * 7;
            }
            SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
            bellHit.PitchVariance = 0.2f;
            SoundEngine.PlaySound(bellHit, NPC.position);
            MakeDonut(NPC.Bottom, Vector2.UnitY);
        }
        NPC.velocity.X *= 0.96f;
        if (NPC.velocity.Y < -5)
            NPC.velocity.Y *= 0.96f;
        if (NPC.velocity.Y >= -0.5f)
        {
            SwitchState(AIState.Idle);
        }
    }

    void AI_JumpTo()
    {
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
        Timer++;
        if (Timer == 1)
        {
            var target = Main.player[NPC.target];
            _jumpStartPosition = NPC.Center;
            _jumpEndPosition = target.Center - new Vector2(0, 32);
            _jumpEndPosition = TileUtilities.FallToSolidTile(_jumpEndPosition);
            _jumpEndPosition.Y -= 32;
            SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
            bellHit.PitchVariance = 0.2f;
            SoundEngine.PlaySound(bellHit, NPC.position);
            MakeDonut(NPC.Bottom, Vector2.UnitY);
        }


        var jumpTicks = Vector2.Distance(_jumpStartPosition, _jumpEndPosition) / 6f;
        jumpTicks = MathF.Max(jumpTicks, 54);
        var ratio = Timer / jumpTicks;
        var posToMoveTo = Vector2.Lerp(_jumpStartPosition, _jumpEndPosition, ratio);
        var up = MathHelper.Lerp(0, JumpHeight, EasingFunction.OutExpo(ratio));
        var down = MathHelper.Lerp(JumpHeight, 0, EasingFunction.InExpo(ratio));
        var y = MathHelper.Lerp(up, down, ratio);
        posToMoveTo.Y += y;
        NPC.velocity = Vector2.Zero;
        NPC.noTileCollide = true;
        NPC.Center = posToMoveTo;
        if (Timer >= jumpTicks)
        {
            //decide attack;
            if (MultiplayerHelper.IsHost)
            {
                if (Main.rand.NextBool(2))
                {
                    SwitchState(AIState.CircularSpike);
                }
                else
                {
                    SwitchState(AIState.SniperSpike);
                }
            }
        }
    }

    void Squish(in float ratio)
    {
        var size1 = Vector2.Lerp(Vector2.One, new Vector2(1.25f), EasingFunction.OutExpo(ratio));
        var size2 = Vector2.Lerp(new Vector2(1.25f), Vector2.One, EasingFunction.InSine(ratio));
        var size3 = Vector2.Lerp(size1, size2, ratio);
        var size4 = Vector2.Lerp(new Vector2(1.3f, 0.5f), Vector2.One, EasingFunction.InSine(ratio / 0.5f));
        var size5 = Vector2.Lerp(new Vector2(0.8f, 1.2f), Vector2.One, EasingFunction.InSine(ratio));
        _squishScale = size3 * size4 * size5;
    }

    void AI_CircularSpike()
    {
        Timer++;
        if (Timer == 1)
        {
            for (var i = 0; i < 8; i++)
            {
                var pos = NPC.Center;
                pos += Main.rand.NextVector2Circular(24, 24);
                var vel = (pos - NPC.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(4, 9);
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel,
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Blue.ToVector4(),
                    scale = new Vector2(Main.rand.NextFloat(0.6f, 1f)),
                    timeLeft = 120
                });
            }
        }

        float ratio = Timer / CircularSquishTime;
        Squish(ratio);
        NPC.velocity *= 0.96f;
        NPC.noTileCollide = false;
        if (Timer == 30)
        {
            if (MultiplayerHelper.IsHost)
            {
                var spikeFirer = ProjFirer.From<PearlbornSpear>(NPC);
                spikeFirer.damage = DamagePearlbornSpike;
                spikeFirer.knockback = 1;
                spikeFirer.ai1 = NPC.whoAmI;
                for (var i = 0; i < 8; i++)
                {
                    var progress = i / 8f;
                    var rotatedSpikeFirer = spikeFirer;
                    var rot = progress * MathHelper.TwoPi;
                    var rotationOffset = rot.ToRotationVector2();
                    rotatedSpikeFirer.velocity = rotationOffset * 256;
                    rotatedSpikeFirer.New();
                }
            }
        }

        if (Timer >= CircularSquishTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    void AI_SniperSpike()
    {
        Timer++;
        if (Timer % 4 == 0)
        {
            var pos = NPC.Center;
            pos += Main.rand.NextVector2Circular(128, 128);
            var vel = (NPC.Center - pos) * 0.05f;
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.White.ToVector4(),
                outerColor = Color.Blue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.6f, 1f)),
                timeLeft = 120
            });
        }

        float ratio = Timer / SniperSpikeSquishTime;
        Squish(ratio);
        NPC.velocity *= 0.96f;
        NPC.noTileCollide = false;
        if (Timer == 60)
        {
            if (MultiplayerHelper.IsHost)
            {
                var spikeFirer = ProjFirer.From<PearlbornSpear>(NPC);
                spikeFirer.damage = DamagePearlbornSpike;
                spikeFirer.knockback = 1;
                spikeFirer.ai1 = NPC.whoAmI;
                var target = Main.player[NPC.target];
                var vel = (target.Center - NPC.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= 512;
                spikeFirer.velocity = vel;
                spikeFirer.New();
            }
        }

        if (Timer >= SniperSpikeSquishTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    public override void OnKill()
    {
        base.OnKill();
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        for(var i = 0; i < NPC.oldPos.Length; i++)
        {
            var oldPos = NPC.oldPos[i] + NPC.Size * 0.5f;
            var drawer2 = SpritebatchDrawer.FromNPC(NPC);
            drawer2.worldPosition = oldPos;
            drawer2.color = Color.Lerp(Color.SkyBlue, Color.Transparent, (float)i / (float)NPC.oldPos.Length) * 0.15f;
            drawer2.color.A = 0;
            spriteBatch.Draw(drawer2);
        }
        var aura = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SpiralVortex.Asset, NPC.Center);
        aura.scale *= 0.35f;
        aura.rotation = Main.GlobalTimeWrappedHourly * 3;
        aura.color = Color.SkyBlue;
        aura.color.A = 0;
        Main.spriteBatch.Draw(aura);

        var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, NPC.Center);
        glowDrawer.scale *= 0.35f;
        glowDrawer.rotation = Main.GlobalTimeWrappedHourly * 3;
        glowDrawer.color = Color.SkyBlue * ExtraMath.Osc(0.6f, 1f);
        glowDrawer.color.A = 0;
        Main.spriteBatch.Draw(glowDrawer);
        return false;

    }
    void DrawSprite(SpriteBatch spriteBatch)
    {
        var drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.scale = _squishScale;
        drawer.color *= 0.6f;
        drawer.BottomCenterOrigin();
        drawer.worldPosition.Y += NPC.height / 2;
        spriteBatch.Draw(drawer);
    }

    void DrawOutline(SpriteBatch spriteBatch)
    {
        var drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.scale = _squishScale;
        drawer.color = _outliner.outlineColor;
        drawer.BottomCenterOrigin();
        drawer.worldPosition.Y += NPC.height / 2;
        spriteBatch.Draw(drawer);
    }

    public void DrawToRenderTargets()
    {
        PearlbornSlimeRenderer.PrepareForRenderingSprite(DrawSprite);
        PearlbornSlimeRenderer.PrepareForRenderingSpriteOutline(DrawOutline);
    }
}

public class PearlbornSpear : ModProjectile,
    IDrawToRenderTarget
{
    float _ease;
    Vector2[] TrailPoints
    {
        get
        {
            if (field == null)
            {
                field = new Vector2[32];

            }



            var start = Projectile.Center;


            var end = Projectile.Center + Projectile.velocity * _ease;
            for (var i = 0; i < field.Length; i++)
            {
                ref var pos = ref field[i];

                pos = Vector2.Lerp(start, end, i / (float)field.Length);
            }
            return field;
        }
    }
    float _alpha;
    float Time => 90;
    ref float Timer => ref Projectile.ai[0];
    NPC Parent => Main.npc[(int)Projectile.ai[1]];
    ref float Rand => ref Projectile.ai[2];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (Timer < 25)
            return false;
        var start = Projectile.Center;
        var end = Projectile.Center + Projectile.velocity * _ease;
        var collisionPoint = 0F;
        var lineWidth = 6;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), end, start, lineWidth, ref collisionPoint))
            return true;
        return false;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = false;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = (int)Time;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.light = 0.6f;
        Projectile.ignoreWater = true;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1 && this.OwnedByLocalClient())
        {
            Rand = Main.rand.NextFloat(0.5f, 1f);
            Projectile.netUpdate = true;
        }
        Projectile.Center = Parent.Center;
        Projectile.Center += Projectile.velocity.SafeNormalize(Vector2.Zero) * 3;
        Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Sin(Timer * 0.1f + Projectile.identity) * 0.01f);
        var time = 24;
        if (Timer >= 25)
            Projectile.hostile = true;
        if(Timer <= time)
        {

            _ease = MathHelper.Lerp(0f, MathHelper.Lerp(0.35f, 0.1f, Timer / time), EasingFunction.OutExpo(Timer / time)) * Rand;
        }
        else if (Timer < 60)
        {
            _ease = MathHelper.Lerp(0.1f, 1f, EasingFunction.OutExpo((Timer - time) / 45f)) * Rand;
            _alpha = MathHelper.Lerp(_alpha, 1f, 0.2f);
        }
        else
        {
            _ease = MathHelper.Lerp(1f, 0f, EasingFunction.InSine((Timer - 60f) / 30f));
        }
    }

    float GetTrailWidth(float progress)
    {
        var startSize = 2;
        var endSize = 4;
        var size = MathHelper.Lerp(startSize, endSize, _ease);
        return MathHelper.SmoothStep(size, 0, progress);
    }

    Color GetTrailColor(float progress)
    {
        var startColor = Color.Cyan;
        var col = Color.Lerp(Color.White, Color.DarkGray, Timer / 60f);
        col = col.MultiplyRGB(Lighting.GetColor(Projectile.Center.ToTileCoordinates()));
        var easeColor = Color.Lerp(startColor, col, _alpha);
        return easeColor * 0.75f;
    }

    Color GetTrailOutlineColor(float progress)
    {
        if (Projectile.hostile)
            return Color.Red;
        return Color.Yellow;
    }

    public override bool PreDraw(ref Color lightColor)
    {

        return false;
    }

    public void DrawToRenderTargets()
    {
        PearlbornSlimeRenderer.PrepareForRendering(new()
        {
            GetTrailWidth = GetTrailWidth,
            GetTrailColor = GetTrailColor,
            TrailOffset = Vector2.Zero,
            Points = TrailPoints
        });
        
        PearlbornSlimeRenderer.PrepareForRenderingOutline(new()
        {
            GetTrailWidth = GetTrailWidth,
            GetTrailColor = GetTrailOutlineColor,
            TrailOffset = Vector2.Zero,
            Points = TrailPoints
        });
    }
}

