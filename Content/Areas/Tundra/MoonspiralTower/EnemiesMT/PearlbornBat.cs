using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Common.ShockCircleSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;
public class PearlbornClone : ModProjectile,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Prepare,
        Dash,
        Out
    }

    bool _warning;
    bool _hostile;
    AIState _state;
    Vector2 _dashVelocity;
    Vector2 _initialVelocity;
    Vector2 _targetPoint;
    float _side;
    float _alpha;
    ref float Timer => ref Projectile.ai[0];
    NPC Parent
    {
        get => Main.npc[(int)Projectile.ai[1]];
    }
    Player Target
    {
        get => Main.player[(int)Projectile.ai[2]];
    }

    float DashTime => 70;
    float PrepTime => 60;
    float OutTime => 30;
    float AnimationSpeed => 5;
    Vector2 SideOffset => Vector2.UnitY.RotatedBy(_side) * 192;
    public override string Texture => ModContent.GetInstance<PearlbornBat>().Texture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_initialVelocity);
        writer.WriteVector2(_dashVelocity);
        writer.WriteVector2(_targetPoint);
        writer.Write((byte)_state);
        writer.Write(_side);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _initialVelocity = reader.ReadVector2();
        _dashVelocity = reader.ReadVector2();
        _targetPoint = reader.ReadVector2();
        _state = (AIState)reader.ReadByte();
        _side = reader.ReadSingle();
    }

    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        _initialVelocity = Projectile.velocity;
        _side = Main.rand.NextFloat(6.28f);
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 7;
        Projectile.SetTrailCacheLength(12);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
        Projectile.ignoreWater = true;
        Projectile.light = 0.6f;
    }

    public override void AI()
    {
        base.AI();
        _warning = false;
        _hostile = false;
        switch (_state)
        {
            case AIState.Prepare:
                _alpha = MathHelper.Lerp(_alpha, 1f, 0.1f);
                AI_Prepare();
                break;
            case AIState.Dash:
                _alpha = 1f;
                AI_Dash();
                break;
            case AIState.Out:
                _alpha = MathHelper.Lerp(_alpha, 0f, 0.1f);
                AI_Out();
                break;
        }

        Projectile.frameCounter++;
        if (Projectile.frameCounter >= AnimationSpeed)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= Main.projFrames[Type])
                Projectile.frame = 0;
        }

        if (Projectile.velocity.X < 0)
            Projectile.spriteDirection = -1;
        else
            Projectile.spriteDirection = 1;
        Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.X * 0.025f, 0.1f);
    }

    void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            _state = state;
            Projectile.netUpdate = true;
        }
    }


    void SpawnInParticles()
    {
        var pos = Projectile.Center + Main.rand.NextVector2Circular(4, 4);
        var vel = Main.rand.NextVector2Circular(8, 8);
        Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
        {
            position = pos,
            velocity = vel,
            innerColor = Color.SkyBlue.ToVector4(),
            outerColor = Color.DarkBlue.ToVector4(),
            scale = new Vector2(Main.rand.NextFloat(0.5f, 1f)),
            timeLeft = 120
        });
    }
    void DisperseEffect()
    {
        for (var i = 0; i < 16; i++)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(24, 24);
            var vel = (pos - Projectile.Center);
            vel = vel.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5, 10);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.SkyBlue.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 1f)),
                timeLeft = 120
            });
        }
    }

    void AI_Prepare()
    {
        Timer++;
        if (Timer == 1)
        {
            if(Main.netMode != NetmodeID.Server)
            {
                ShockCircles.CreateQuickWhiteFlash(Projectile.Center);
            }
            _targetPoint = Target.Center + SideOffset;
            DisperseEffect();
            var sound = AssetReferences.Assets.Sounds.VoidHit.Asset with { PitchVariance = 0.6F, Volume = 0.5F };
            SoundEngine.PlaySound(sound, Projectile.position);
        }

        if (Main.rand.NextBool(8))
        {
            SpawnInParticles();
        }
        var targetVelocity = _targetPoint - Projectile.Center;
        var ratio = Timer / PrepTime;
        var easing = EasingFunction.InSine(ratio);
        var newVelocity = Vector2.Lerp(_initialVelocity, targetVelocity, easing);
        Projectile.hostile = false;
        Projectile.velocity = newVelocity;
        if (Timer >= PrepTime)
        {
            SwitchState(AIState.Dash);
        }
    }

    void AI_Dash()
    {
        Timer++;
        if (Timer == 1)
        {
            _dashVelocity = (Target.Center - Projectile.Center);
            _dashVelocity = _dashVelocity.SafeNormalize(Vector2.Zero);
            _dashVelocity *= 25;
        }
        if(Timer >= DashTime / 2f)
        {
            _hostile = true;
            Projectile.hostile = true;
        }
        else
        {
            _warning = true;
            Projectile.hostile = false;
        }
        
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Lerp(-_dashVelocity * 0.5f, _dashVelocity, EasingFunction.Anticipation2(Timer / (DashTime / 2f))), 0.03f);
        if (Timer >= DashTime)
        {
            SwitchState(AIState.Out);
        }
    }

    void AI_Out()
    {
        Projectile.hostile = false;
        Projectile.velocity *= 0.8f;
        Timer++;
        if (Timer >= OutTime)
        {
            Projectile.Kill();
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.color *= 0.3f;
        drawer.color *= ExtraMath.Osc(0.9f, 1f, speed: 16, offset: Projectile.identity);
        drawer.color *= _alpha;

        foreach(OldPosition oldPos in Projectile.IterateOldPosBackwards())
        {
            var afDrawer = drawer;
            afDrawer.color = Color.Lerp(Color.SkyBlue, Color.Transparent, oldPos.progress) * 0.1f;
            afDrawer.worldPosition = oldPos.position + Projectile.Size * 0.5f;
            Main.spriteBatch.Draw(afDrawer);
        }
        Main.spriteBatch.Draw(drawer);
        return false;
    }

    void DrawOutline(SpriteBatch spriteBatch)
    {
        var color = Color.Transparent;
        if (_warning)
            color = Color.Yellow;
        if (_hostile)
          color = Color.Red;
        var drawer = Projectile.Drawer;
        drawer.color = color;
        drawer.color *= _alpha;
        spriteBatch.Draw(drawer);
    }
    void DrawExclusionMask(SpriteBatch spriteBatch)
    {
        var drawer = Projectile.Drawer;
        drawer.scale *= 1.05f;
        spriteBatch.Draw(drawer);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        DisperseEffect();
    }

    public void DrawToRenderTargets()
    {
        OutlineRenderer.Queue(DrawOutline);
        MoonEffect.PrepareForExclusionRendering(DrawExclusionMask);
    }
}

public class PearlbornBat : ModNPC,
    IDrawToRenderTarget
{
    enum AIState
    {
        Idle,
        Chase,
        Summon
    }
    int _frame;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    Vector2 _randomPointToTrack;
    float ChaseTime => 120;
    float AttackDistance => 128;
    float ChaseDistance => 384;
    int CloneCount => 14;
    int TimeBetweenClones => 60;
    int Damage_Clone => 25;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 7;
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.TrailingMode[Type] = 3;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 20;
        NPC.height = 20;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0;
        NPC.damage = 24;
        NPC.lifeMax = 252;
        NPC.defense = 15;
        NPC.noGravity = true;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_randomPointToTrack);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _randomPointToTrack = reader.ReadVector2();
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PearlescentScrap>(), minimumDropped: 2, maximumDropped: 4));
    }


    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.Summon:
                AI_Summon();
                break;
        }
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
        Lighting.AddLight(NPC.position, TorchID.Ice);
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
        Timer++;
        if (Timer == 1 || !NPC.HasValidTarget)
        {
            NPC.TargetClosest();
        }

        if (Timer % 80 == 0 || _randomPointToTrack == default(Vector2))
        {
            if (MultiplayerHelper.IsHost)
            {
                _randomPointToTrack = NPC.Center + Main.rand.NextVector2Circular(128, 128);
            }
        }

        var pointToMoveTo = _randomPointToTrack;
        var targetVelocity = pointToMoveTo - NPC.Center;
        targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
        var maxSpeed = 3;
        var speed = MathF.Min(Vector2.Distance(pointToMoveTo, NPC.Center), maxSpeed);
        targetVelocity *= speed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.15f);
        if (NPC.velocity.X < 0)
            NPC.spriteDirection = -1;
        else
            NPC.spriteDirection = 1;
        var distance = Vector2.Distance(NPC.Center, Main.player[NPC.target].Center);
        if (distance <= ChaseDistance)
        {
            SwitchState(AIState.Chase);
        }
    }


    void AI_Chase()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }

        if (NPC.HasValidTarget)
        {

            var myTarget = Main.player[NPC.target];
            var targetPos = myTarget.Center;
            targetPos += new Vector2(0, -90).RotatedBy(Timer * 0.006f);
            var targetVelocity = (targetPos - NPC.Center);
            targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
            var speed = 8;
            var dist = Vector2.Distance(NPC.Center, targetPos);
            targetVelocity *= MathF.Min(speed, dist);
            NPC.velocity = NPC.velocity.MoveTowards(targetVelocity, 0.3f);
            NPC.SpriteFaceTarget();
            if (Timer >= ChaseTime && Vector2.Distance(NPC.Center, targetPos) < AttackDistance)
            {
                SwitchState(AIState.Summon);
            }
        }
        else
        {
            SwitchState(AIState.Idle);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var glowDrawer2 = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, NPC.Center);
        glowDrawer2.color = Color.SkyBlue * 0.6f;
        glowDrawer2.color.A = 0;
        glowDrawer2.scale *= 0.45f;
        spriteBatch.Draw(glowDrawer2);
        var drawer = SpritebatchDrawer.FromNPC(NPC);

        for (var i = 0; i < NPC.oldPos.Length; i++)
        {
            var posToDrawAt = NPC.oldPos[i] + NPC.Size * 0.5f;
            var afDraw = drawer;
            var progress = i / (float)NPC.oldPos.Length;
            afDraw.worldPosition = posToDrawAt;
            afDraw.rotation = NPC.oldRot[i];
            afDraw.color = Color.Lerp(Color.SkyBlue, Color.Transparent, progress) * 0.15f;
            spriteBatch.Draw(afDraw);
        }
        spriteBatch.Draw(drawer);


        var glow = AssetReferences.Content.Areas.Tundra.MoonspiralTower.EnemiesMT.PearlbornBat_Glow.Asset;
        var glowDrawer = drawer with { texture = glow.Value };
        glowDrawer.color = Color.White * ExtraMath.Osc(0.3f, 0.6f, speed: 2, offset: NPC.whoAmI);
        glowDrawer.color.A = 0;
        spriteBatch.Draw(glowDrawer);
        return false;
        //return base.PreDraw(spriteBatch, screenPos, drawColor);
    }
    void AI_Summon()
    {
        Timer++;
        if (Timer % TimeBetweenClones == 0)
        {
            AttackCycle++;
            if (MultiplayerHelper.IsHost)
            {
                var cloneFirer = ProjFirer.From<PearlbornClone>(NPC);
                cloneFirer.velocity = Main.rand.NextVector2CircularEdge(8, 8);
                cloneFirer.damage = Damage_Clone;
                cloneFirer.New();
            }
            if (AttackCycle >= CloneCount)
            {
                SwitchState(AIState.Idle);
            }
        }
        NPC.velocity *= 0.96f;
        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.05f) * 0.5f, 0.2f);
        NPC.SpriteFaceTarget();
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
        {
            int leftWingGore = Mod.Find<ModGore>($"{Name}_Gore_LeftWing").Type;
            int rightWingGore = Mod.Find<ModGore>($"{Name}_Gore_RightWing").Type;
            int headGore = Mod.Find<ModGore>($"{Name}_Gore_Head").Type;
            int fureGore = Mod.Find<ModGore>($"{Name}_Gore_Fur").Type;

            // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, leftWingGore, 1f);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(34, 0), NPC.velocity, rightWingGore);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(17, 0), NPC.velocity, headGore);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(17, 17), NPC.velocity, fureGore);
        }
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
        //   return base.CanHitPlayer(target, ref cooldownSlot);
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frameCounter += 0.2f;
        if (NPC.frameCounter >= 1f)
        {
            NPC.frameCounter = 0;
            _frame++;
            _frame %= Main.npcFrameCount[Type];
        }
        NPC.frame.Y = frameHeight * _frame;
    }

    public void DrawToRenderTargets()
    {
        MoonEffect.PrepareForExclusionRendering((SpriteBatch sb) =>
        {
            var drawer = SpritebatchDrawer.FromNPC(NPC);
            drawer.scale *= 1.05f;
            sb.Draw(drawer);
        });
    }
}

