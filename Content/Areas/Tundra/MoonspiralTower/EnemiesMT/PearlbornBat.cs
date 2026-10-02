using Stellamod.Common.Particles;
using Stellamod.Common.ScreenEffectsSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;

public class PearlbornCrystal : ModNPC
{
    enum AIState : byte
    {
        Idle,
        PickedUp,
        Throw
    }
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    Player PickedUpTarget
    {
        get => Main.player[(int)NPC.ai[2]];
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = NPC.height = 32;
        NPC.damage = 1;
        NPC.defense = 9999;
        NPC.lifeMax = 15;

    }

    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.PickedUp:
                AI_PickedUp();
                break;
        }

        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
    }
    void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    void AI_Idle()
    {
        Timer++;
        NPC.velocity.X *= 0.94f;
    }

    void AI_PickedUp()
    {
        Timer++;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var drawer = SpritebatchDrawer.FromNPC(NPC);
        spriteBatch.Draw(drawer);
        return false;
    }
    
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    public override void OnKill()
    {
        base.OnKill();
    }
}
public class MoonAura : ModNPC
{
    float AuraRadius => 1024;
    ref float Timer => ref NPC.ai[0];
    ref float LifeTime => ref NPC.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
 
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.damage = 1;
        NPC.lifeMax = 1;
        NPC.dontTakeDamage = true;
        NPC.dontCountMe = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.noGravity = true;
        NPC.defense = 1;
    }
    
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    
    public override void AI()
    {
        base.AI();
        Timer++;
        LifeTime--;
        if (LifeTime <= 0)
            NPC.active = false;
        var buffType = ModContent.BuffType<Pearlflame>();
        foreach(var npc in Main.ActiveNPCs)
        {
            var sqrDst = Vector2.DistanceSquared(NPC.Center, npc.Center);
            if(sqrDst <= AuraRadius * AuraRadius)
            {
                npc.AddBuff(buffType, 120);
            }
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var moonEffect = ModContent.GetInstance<MoonEffect>();
        moonEffect.isActive = true;
        MoonEffect.PrepareForRenderering(DrawMask);
        return false;
    }

    void DrawMask(SpriteBatch spriteBatch)
    {
        var inRatio = EasingFunction.InOutSine(Timer / 60f);
        var outRatio = EasingFunction.InOutSine(LifeTime / 60f);
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, NPC.Center);
        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.scale = Vector2.One * inRatio * outRatio;
        spriteBatch.Draw(drawer);
    }
}

public class MoonEffect : AScreenEffect
{
    static readonly Queue<Action<SpriteBatch>> _drawQueue = new();
    public override ScreenEffectPriority Priority => ScreenEffectPriority.Very_Late;
    public override void Apply(SpriteBatch spriteBatch, RenderTarget2D src, RenderTarget2D dst)
    {
        using var temp = RT.Context(RenderTargets.ScreenTarget);
        using (RT.Clear(temp, Color.Transparent))
        {
            var beginner = SpritebatchParams.InWorldAndZoomed();
            spriteBatch.Begin(beginner);
            while (_drawQueue.Count > 0)
            { 
                _drawQueue.Dequeue()(spriteBatch);
            }
            spriteBatch.End();
        }

        var pass = AssetReferences.Effects.Generic.MoonAuraMask.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.maskSampler = new()
        {
            Sampler = SamplerState.PointClamp,
            Texture = temp
        };
        pass.Parameters.noiseSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture = AssetReferences.Assets.NoiseTextures.PerlinNoise.Asset.Value
        };
        pass.Parameters.distortionStrength = 0.03f;
        pass.Apply();
        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            pass.Shader);
        spriteBatch.Draw(src, Vector2.Zero, Color.SkyBlue);
        spriteBatch.End();
    }

    public static void PrepareForRenderering(Action<SpriteBatch> drawAction)
    {
        _drawQueue.Enqueue(drawAction);
    }
}
public class Pearlflame : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
    }
}
public class PearlbornClone : ModProjectile,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Prepare,
        Dash,
        Out
    }

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
            _targetPoint = Target.Center + SideOffset;
            DisperseEffect();
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
        Projectile.hostile = true;
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Lerp(-_dashVelocity * 0.5f, _dashVelocity, EasingFunction.Anticipation2(Timer / 30f)), 0.03f);
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
        drawer.color *= 0.6f;
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
        var color = !Projectile.hostile ? Color.Yellow : Color.Red;
        var drawer = Projectile.Drawer;
        drawer.color = color;
        drawer.color *= _alpha;
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
    }
}

public class PearlbornBat : ModNPC
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
    int TimeBetweenClones => 30;
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
        NPC.lifeMax = 200;
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
}


[Autoload(Side = ModSide.Client)]
public class PearlbornSoulRenderer : ModSystem
{
    public record struct SoulDrawData(Vector2[] TrailCache, Vector2 TrailOffset);
    static Queue<SoulDrawData> _soulDrawQueue = new();
    static Queue<SoulDrawData> _yellowOutlineSoulDrawQueue = new();
    static Queue<SoulDrawData> _redOutlineSoulDrawQueue = new();
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += PreparePixelatedRendering;
    }

    private void PreparePixelatedRendering(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;
        if (_redOutlineSoulDrawQueue.Count > 0)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawSoulTrailOutline, DrawLayer.OverNPCs);
        }
        if (_yellowOutlineSoulDrawQueue.Count > 0)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawSoulTrailOutlineYellow, DrawLayer.OverNPCs);
        }


        if (_soulDrawQueue.Count > 0)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawSoulTrail, DrawLayer.OverNPCs);
        }
    }

    float GetTrailWidth(float progress)
    {
        return MathHelper.SmoothStep(18, 0, progress);
    }

    Color GetTrailColor(float progress)
    {
        return DrawUtilities.InterpolateColorArray(progress, Color.White, Color.SkyBlue);
    }


    void DrawSoulTrailInner(
        SpriteBatch sb, 
        ref Queue<SoulDrawData> drawQueue, 
        List<VertexPositionColorTexture> batch,
        List<Vector3> points)
    {
        while (drawQueue.Count > 0)
        {
            var drawData = drawQueue.Dequeue();
            var vertices = DrawUtilities.PrepareSimpleTrailing(drawData.TrailCache, GetTrailColor, GetTrailWidth, drawData.TrailOffset);
            batch.AddRange(vertices);

            var drawPoint = new Vector3(drawData.TrailCache[1], (drawData.TrailCache[1] - drawData.TrailCache[0]).ToRotation());
            points.Add(drawPoint);
        }

        var indices = DrawUtilities.PrepareIndicesForDrawing(batch.Count / 4);
        var pass = AssetReferences.Effects.Generic.SoulTrail.CreatePrimitivesPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.spriteSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture = AssetReferences.Assets.LaserTextures.SpectralSoulSuck.Asset.Value
        };
        pass.Parameters.noiseSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture = AssetReferences.Assets.LaserTextures.FlameTrail.Asset.Value
        };
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Apply();

        //Now we just need to make and draw the shader
        sb.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(batch.ToArray(), indices, pass.Shader);
    }

    void DrawSoulTrailOutlineYellow(SpriteBatch sb, Vector2 sp)
    {
        using var temp = RT.Context(RenderTargets.ScreenTarget);
        sb.EndOut(out var oldParameters);

        using (RT.Clear(temp, Color.Transparent))
        {
            var batch = new List<VertexPositionColorTexture>();
            var points = new List<Vector3>();
            DrawSoulTrailInner(sb, ref _yellowOutlineSoulDrawQueue, batch, points);
            sb.Begin(oldParameters with { matrix = Matrix.Identity });
            foreach (var p in points)
            {
                var textureAsset = AssetReferences.Content.Areas.Tundra.MoonspiralTower.EnemiesMT.PearlbornSoul.Asset;
                var drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, p.XY() + new Vector2(8));
                drawer.VerticalFrame(0, 2);
                drawer.CenterOrigin();
                drawer.scale *= 0.35f;
                drawer.scale *= new Vector2(1.65f, 1f);
                drawer.scale *= 1.1f;
                drawer.color = Color.White;
                drawer.rotation = p.Z;
                sb.Draw(drawer);

                drawer.VerticalFrame(1, 2);
                drawer.scale = Vector2.One;
                sb.Draw(drawer);
            }

            sb.End();
        }

        var outliner = AssetReferences.Effects.Generic.OutlinerNoTransparencyThreshold.CreatePixelPass();
        outliner.Parameters.texelSize = temp.Target.GetTexelSize() * 2f;
        outliner.Parameters.threshold = 0.7f;
        outliner.Apply();
        using (sb.Ctx(oldParameters with { effect = outliner.Shader }))
        {
            sb.Draw(temp, Vector2.Zero, Color.Yellow);
        }
        sb.Begin(oldParameters);
    }

    void DrawSoulTrailOutline(SpriteBatch sb, Vector2 sp)
    {
        using var temp = RT.Context(RenderTargets.ScreenTarget);
        sb.EndOut(out var oldParameters);
     
        using(RT.Clear(temp, Color.Transparent))
        {
            var batch = new List<VertexPositionColorTexture>();
            var points = new List<Vector3>();
            DrawSoulTrailInner(sb, ref _redOutlineSoulDrawQueue, batch, points);
            sb.Begin(oldParameters with { matrix = Matrix.Identity });
            foreach (var p in points)
            {
                var textureAsset = AssetReferences.Content.Areas.Tundra.MoonspiralTower.EnemiesMT.PearlbornSoul.Asset;
                var drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, p.XY() + new Vector2(8));
                drawer.VerticalFrame(0, 2);
                drawer.CenterOrigin();
                drawer.scale *= 0.35f;
                drawer.scale *= new Vector2(1.65f, 1f);
                drawer.scale *= 1.1f;
                drawer.color = Color.White;
                drawer.rotation = p.Z;
                sb.Draw(drawer);

                drawer.VerticalFrame(1, 2);
                drawer.scale = Vector2.One;
                sb.Draw(drawer);
            }

            sb.End();
        }

        var outliner = AssetReferences.Effects.Generic.OutlinerNoTransparencyThreshold.CreatePixelPass();
        outliner.Parameters.texelSize = temp.Target.GetTexelSize() * 2f;
        outliner.Parameters.threshold = 0.7f;
        outliner.Apply();
        using (sb.Ctx(oldParameters with { effect = outliner.Shader }))
        {
            sb.Draw(temp, Vector2.Zero, Color.Red);
        }
        sb.Begin(oldParameters);
    }

    void DrawSoulTrail(SpriteBatch sb, Vector2 sp)
    {
        using var temp = RT.Context(RenderTargets.ScreenTarget);
        sb.EndOut(out var oldParameters);
        using(RT.Clear(temp, Color.Transparent))
        {
            var batch = new List<VertexPositionColorTexture>();
            var points = new List<Vector3>();
            DrawSoulTrailInner(sb, ref _soulDrawQueue, batch, points);
            sb.Begin(oldParameters with { matrix = Matrix.Identity });
            foreach(var p in points)
            {
                var textureAsset = AssetReferences.Content.Areas.Tundra.MoonspiralTower.EnemiesMT.PearlbornSoul.Asset;
                var drawer = SpritebatchDrawer.FromTextureAsset(textureAsset, p.XY() + new Vector2(8));
                drawer.VerticalFrame(0, 2);
                drawer.CenterOrigin();
                drawer.scale *= 0.35f;
                drawer.scale *= new Vector2(1.65f, 1f);
                drawer.scale *= 1.1f;
                drawer.color = Color.White;
                drawer.rotation = p.Z;
                sb.Draw(drawer);

                drawer.VerticalFrame(1, 2);
                drawer.scale = Vector2.One;
                sb.Draw(drawer);
            }
            sb.End();

        }

        var outliner = AssetReferences.Effects.Generic.OutlinerNoTransparencyThreshold.CreatePixelPass();
        outliner.Parameters.texelSize = temp.Target.GetTexelSize() * 2f;
        outliner.Parameters.threshold = 0.7f;
        outliner.Apply();
        using(sb.Ctx(oldParameters with { effect =  outliner.Shader }))
        {
            sb.Draw(temp, Vector2.Zero, Color.Lerp(Color.SkyBlue, Color.Black, 0.5f));
        }

        sb.Begin(oldParameters);
    }
    public static void PrepareForRenderingRed(in SoulDrawData drawData)
    {
        _redOutlineSoulDrawQueue.Enqueue(drawData);
    }

    public static void PrepareForRenderingYellow(in SoulDrawData drawData)
    {
        _yellowOutlineSoulDrawQueue.Enqueue(drawData);
    }
    
    public static void PrepareForRendering(in SoulDrawData drawData)
    {
        _soulDrawQueue.Enqueue(drawData);
    }
}

public class PearlbornDash : ModProjectile
{
    NPC Parent => Main.npc[(int)Projectile.ai[0]];
    ref float Time => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(48);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.light = 0.7f;
        Projectile.ignoreWater = true;
        Projectile.hostile = true;
    }
    
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        var points = DrawUtilities.PruneFarPoints(Projectile.oldPos);
        return ProjectileHelper.OldPosColliding(points, projHitbox, targetHitbox);
    }

    public override void AI()
    {
        base.AI();
        Time--;
        Projectile.Center = Parent.Center;
        if (Time <= 0 || !Parent.active || Parent.type != ModContent.NPCType<PearlbornSoul>())
        {
            Projectile.Kill();
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
}
public class PearlbornSoul : ModNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Idle,
        Chase,
        ZigZagPrepare,
        ZigZag
    }

    float _side;
    Vector2 _startDashPoint;
    Vector2 _endDashPoint;
    Vector2 _circleOriginPoint;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    float ChaseDistance => 384;
    float AttackDistance => 64;
    float CircleOffset => 80;
    float ChaseSpeed => 6;
    float ChaseTime => 120;
    float ZigZagPrepareTime => 30;
    float ZigZagDashTime => 45;
    float ZigZagRange => 192;
    int Damage_PearlbornDash => 25;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_startDashPoint);
        writer.WriteVector2(_endDashPoint);
        writer.WriteVector2(_circleOriginPoint);
        writer.Write(_side);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startDashPoint = reader.ReadVector2();
        _endDashPoint = reader.ReadVector2();
        _circleOriginPoint = reader.ReadVector2();
        _side = reader.ReadSingle();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 2;
        NPCID.Sets.TrailCacheLength[Type] = 96;
        NPCID.Sets.TrailingMode[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 20;
        NPC.height = 20;
        NPC.knockBackResist = 0;
        NPC.damage = 24;
        NPC.lifeMax = 200;
        NPC.defense = 15;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.HitSound = SoundID.NPCHit30;
        NPC.DeathSound = SoundID.NPCDeath38;
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
            case AIState.ZigZagPrepare:
                AI_ZigZagPrepare();
                break;
            case AIState.ZigZag:
                AI_ZigZag();
                break;
        }
        NPC.rotation = NPC.velocity.ToRotation();
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
        if (!NPC.HasValidTarget || Timer == 1)
        {
            NPC.TargetClosest();
        }

        if (Timer == 1 || _circleOriginPoint == default(Vector2))
        {
            _circleOriginPoint = NPC.Center;
        }


        var posToTrack = _circleOriginPoint + new Vector2(0, CircleOffset).RotatedBy(Timer * 0.05f);
        if (NPC.HasValidTarget)
        {
            var directionToPlayer = Main.player[NPC.target].Center - NPC.Center;
            directionToPlayer = directionToPlayer.SafeNormalize(Vector2.Zero);
            _circleOriginPoint += directionToPlayer * 1.5f;
        }

        var targetVelocity = posToTrack - NPC.Center;
        targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
        var moveSpeed = 3;
        var speed = MathF.Min(moveSpeed, Vector2.Distance(posToTrack, NPC.Center));
        targetVelocity *= speed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
    

        if (NPC.HasValidTarget && Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) <= ChaseDistance)
        {
            SwitchState(AIState.Chase);
        }
    }

    void AI_Chase()
    {
        Timer++;
        if (!NPC.HasValidTarget)
            SwitchState(AIState.Idle);

        //Just move towards the player while moving up and down

        //honestly no clue what this gonna do lol
        //I'll see what it looks like first
        var directionToTarget = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero);
        var perpDirection = directionToTarget.RotatedBy(MathHelper.PiOver2);
        var targetDirection = Vector2.Lerp(directionToTarget, perpDirection, MathF.Sin(Timer * 0.05f) * 0.5f + 0.5f);
        var targetVelocity = targetDirection * ChaseSpeed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.3f);
        if (Timer >= ChaseTime)
        {
            SwitchState(AIState.ZigZagPrepare);
        }
    }

    void AI_ZigZagPrepare()
    {
        Timer++;
        if (Timer == 1)
        {
            var sound = AssetReferences.Assets.Sounds.StormDragon_CloudBolt.Asset with
            {
                PitchVariance = 0.4f,
                Pitch = 0.5f,
                Volume = 0.25f
            };

            SoundEngine.PlaySound(sound, NPC.position);
            if (MultiplayerHelper.IsHost)
            {
                _side = Main.rand.NextBool(2) ? -1 : 1;
                _startDashPoint = NPC.Center;
                _endDashPoint = Main.player[NPC.target].Center;
                _endDashPoint += new Vector2(0, _side * 256);
                NPC.netUpdate = true;
            }
        }

        if (Timer == 1)
        {
            PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.SkyBlue, 25, 64);
            FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 20, baseSize: 0.16f);
        }

        //this can be snappy.
        var startPos = _startDashPoint;
        var endPos = _endDashPoint;
        var ratio = Timer / ZigZagPrepareTime;
        var ease = EasingFunction.InOutSine(ratio);
        var posToMoveTo = Vector2.Lerp(startPos, endPos, ease);
        var targetVelocity = posToMoveTo - NPC.Center;
        NPC.velocity = targetVelocity;

        if (Timer >= ZigZagPrepareTime * 1.2f)
        {
            SwitchState(AIState.ZigZag);
        }
    }

    void AI_ZigZag()
    {
        Timer++;
        if (Timer == 1)
        {
            _startDashPoint = NPC.Center;
            _endDashPoint = _startDashPoint + new Vector2(0, 384 * -_side);
            if (MultiplayerHelper.IsHost)
            {
                var firer = ProjFirer.From<PearlbornDash>(NPC);
                firer.ai0 = NPC.whoAmI;
                firer.ai1 = ZigZagDashTime;
                firer.damage = Damage_PearlbornDash;
                firer.New();
            }
        }
        if(Timer == 7)
        {
            var sound = AssetReferences.Assets.Sounds.StormDragon_FlyingIn.Asset with { 
                PitchVariance = 0.4f,
                Pitch = 0.5f,
                Volume = 0.25f };
            SoundEngine.PlaySound(sound, NPC.position);
        }

        var ratio = Timer / ZigZagDashTime;
        var ease = ratio;
        var pos = Vector2.Lerp(_startDashPoint, _endDashPoint, ease);
        pos.X += MathF.Sin(ratio * 12f) * ZigZagRange;
        var targetVelocity = pos - NPC.Center;
        NPC.velocity = Vector2.Lerp(Vector2.Zero, targetVelocity, EasingFunction.InSine(Timer / 40f));
        if (Timer >= ZigZagDashTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
        {
            PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.SkyBlue, 25, 64);
            FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.DarkBlue, 20, baseSize: 0.16f);
            for (var i = 0; i < 16; i++)
            {
                var pos = NPC.Center;
                pos += Main.rand.NextVector2Circular(24, 24);
                var vel = (pos - NPC.Center);
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(9, 15f);
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
    }
    public override void OnKill()
    {
        base.OnKill();
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return false;
    }

    public void DrawToRenderTargets()
    {

        if(State == AIState.ZigZag)
        {
            PearlbornSoulRenderer.PrepareForRenderingRed(new()
            {
                TrailCache = NPC.oldPos,
                TrailOffset = NPC.Size * 0.5f
            });
        } 
        else if(State == AIState.ZigZagPrepare)
        {
            PearlbornSoulRenderer.PrepareForRenderingYellow(new()
            {
                TrailCache = NPC.oldPos,
                TrailOffset = NPC.Size * 0.5f
            });
        } 
        else
        {
            PearlbornSoulRenderer.PrepareForRendering(new()
            {
                TrailCache = NPC.oldPos,
                TrailOffset = NPC.Size * 0.5f
            });

        }
    }
}

[Autoload(Side = ModSide.Client)]
public class PearlbornSlimeRenderer : ModSystem
{
    public record struct SlimeDrawData(Vector2[] Points, Func<float, Color> GetTrailColor, Func<float, float> GetTrailWidth, Vector2 TrailOffset);
    static readonly Queue<SlimeDrawData> _drawQueue = new();
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += DrawPixelated;
    }

    private void DrawPixelated(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;
        if (_drawQueue.Count <= 0)
            return;
        PixelationManager.QueuePrimitivesDrawAction(RenderSlime, DrawLayer.OverPlayers);
    }

    void RenderSlime(GraphicsDevice gDevice)
    {
        //May need to render this to a render target first so we can outline it, will see how it looks first.
        var batch = new List<VertexPositionColorTexture>();
        while (_drawQueue.Count > 0)
        {
            var drawData = _drawQueue.Dequeue();
            var vertices = DrawUtilities.PrepareSimpleTrailing(drawData.Points, drawData.GetTrailColor, drawData.GetTrailWidth, drawData.TrailOffset);
            batch.AddRange(vertices);
        }

        var indices = DrawUtilities.PrepareIndicesForDrawing(batch.Count / 2);
        var pass = AssetReferences.Effects.Generic.SoulTrail.CreatePrimitivesPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.spriteSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture = AssetReferences.Content.Areas.Tundra.MoonspiralTower.EnemiesMT.PearlbornSlime_Spike.Asset.Value
        };
        pass.Parameters.noiseSampler = new()
        {
            Sampler = SamplerState.PointWrap,
            Texture = AssetReferences.Assets.LaserTextures.FlameTrail.Asset.Value
        };
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Apply();

        //Now we just need to make and draw the shader
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(batch.ToArray(), indices, pass.Shader);
    }

    public static void PrepareForRendering(SlimeDrawData drawData)
    {
        _drawQueue.Enqueue(drawData);
    }
}

public class PearlbornSpear : ModProjectile
{
    Vector2[] TrailPoints
    {
        get
        {
            if (field == null)
            {
                field = new Vector2[32];
                var start = Projectile.Center;
                var end = Projectile.Center + Projectile.velocity;
                for (var i = 0; i < field.Length; i++)
                {
                    ref var pos = ref field[i];

                    pos = Vector2.Lerp(start, end, i / (float)field.Length);
                }
            }
            return field;
        }
    }
    ref float Timer => ref Projectile.ai[0];
    NPC Parent => Main.npc[(int)Projectile.ai[1]];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        var start = Projectile.Center;
        var end = Projectile.Center + Projectile.velocity;
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
        Projectile.hostile = true;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 60;
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
        Projectile.Center = Parent.Center;
    }

    float GetTrailWidth(float progress)
    {
        return MathHelper.SmoothStep(32, 0, progress);
    }

    Color GetTrailColor(float progress)
    {
        return Color.Lerp(Color.White, Color.DarkGray, Timer / 60f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PearlbornSlimeRenderer.PrepareForRendering(new()
        {
            GetTrailWidth = GetTrailWidth,
            GetTrailColor = GetTrailColor,
            TrailOffset = Vector2.Zero,
            Points = TrailPoints
        });
        return false;
    }
}

public class PearlbornSlime : ModNPC
{
    enum AIState : byte
    {
        Idle,
        Jump,
        JumpTo,
        CircularSpike,
        SniperSpike
    }

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
    float ChaseDistance => 384;
    float JumpHeight => -256;
    float CircularSquishTime => 60;
    float SniperSpikeSquishTime => 120;
    int DamagePearlbornSpike => 30;
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
            _frame++;
            _frame %= Main.npcFrameCount[Type];
        }
        NPC.frame.Y = frameHeight * _frame;
    }

    public override void AI()
    {
        base.AI();

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
            _jumpEndPosition = target.Center;
            _jumpEndPosition = TileUtilities.FallToSolidTile(_jumpEndPosition);
            _jumpEndPosition.Y -= 8;
            SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
            bellHit.PitchVariance = 0.2f;
            SoundEngine.PlaySound(bellHit, NPC.position);
            MakeDonut(NPC.Bottom, Vector2.UnitY);
        }


        var jumpTicks = Vector2.Distance(_jumpStartPosition, _jumpEndPosition) / 16f;
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
        _squishScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 1f), EasingFunction.QuadraticBump(ratio));
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
                    rotatedSpikeFirer.velocity = rotationOffset * 128;
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
        _squishScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 1f), EasingFunction.QuadraticBump(ratio));
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
                vel *= 384;
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
        var drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.scale = _squishScale;
        spriteBatch.Draw(drawer);
        return false;

    }
}