using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB.Gores;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;

public class WhisperingPlayer : ModPlayer
{
    public float suckingAlpha;
    public override void PostUpdateBuffs()
    {
        base.PostUpdateBuffs();
        float targetAlpha = 0;
        if (Player.HasBuff<WhisperingDeath>())
        {
            targetAlpha = 1f;
        }
        suckingAlpha = MathHelper.Lerp(suckingAlpha, targetAlpha, 0.02f);
    }
}

public class WhisperingDeath : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        //Ignores life regen stat

        Vector2 pos = player.Center;
        foreach(var npc in Main.ActiveNPCs)
        {
            if(npc.type == ModContent.NPCType<TheWhisperer>())
            {
                pos = npc.Center;
                break;
            }
        }

        float dq = Vector2.DistanceSquared(player.Center, pos);
        float ratio = dq / (500 * 500);
        ratio = EasingFunction.OutExpo(ratio);
        player.lifeRegen -= (int)(MathHelper.Lerp(80, 0, ratio));
        if (Main.rand.NextBool(3))
        {
            SmokeParticle sp = Particle<SmokeParticle>.Spawn(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), -Vector2.UnitY, Color.Blue, Main.rand.NextFloat(0.9f, 1.5f));
            sp.initialColor = Color.Lerp(Color.White, Color.SkyBlue, Main.rand.NextFloat(0f, 1f)) * 0.4f;
            sp.expand = true;
        }
        if (Main.rand.NextBool(3))
        {
            var ember = LegacyParticle.NewParticle<EmberParticle>(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), -Vector2.UnitY.RotatedByRandom(1.5f), Color.SkyBlue, Main.rand.NextFloat(0.9f, 1.5f));
            ember.innerColor = Color.SkyBlue;
            ember.outerColor = Color.DarkBlue;
        }

    }
}

public class TheWhisperer : ModNPC,
    IDrawToRenderTarget
{
    private float _spawnTimer;
    private float _alpha;
    private enum AIState
    {
        Chase,

        CircleBack,
        Despawn,

        Death
    }


    private float _whisperingDirection;
    private Vector2 _shakePos;
    private Vector2 _circleBackStart;
    private Vector2 _circleBackCenter;
    private Vector2 _startVelocity;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private ref float SteamTimer => ref NPC.ai[2];
    private HairRenderer _hairRendererBackingField;
    private HairRenderer HairRenderer
    {
        get
        {
            _hairRendererBackingField ??= new HairRenderer(NPC.Center, 9, 128);
            return _hairRendererBackingField;
        }
    }

    private HairRenderer _hairRenderer2BackingField;
    private HairRenderer HairRenderer2
    {
        get
        {
            _hairRenderer2BackingField ??= new HairRenderer(NPC.Center, 9, 128);
            return _hairRenderer2BackingField;
        }
    }
    private const float Max_Chase_Distance_Squared = 1024 * 1024;
    private const float Suck_Distance_Squared = 128 * 128;
    private float OscAlpha => ExtraMath.Osc(0.8f, 1f);
    private float MoveSpeed => 2 + MathHelper.Lerp(0, 2, BellFlowerSystem.RungBellFlowerCount / 5f);    
    private Player MyTarget => Main.player[NPC.target];

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_circleBackStart);
        writer.WriteVector2(_circleBackCenter);
        writer.WriteVector2(_startVelocity);
        writer.Write(_whisperingDirection);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _circleBackStart = reader.ReadVector2();
        _circleBackCenter = reader.ReadVector2();
        _startVelocity = reader.ReadVector2();
        _whisperingDirection = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        Main.npcFrameCount[Type] = 4;
        NPCSets.Heavy[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = NPC.height = 100;
        NPC.lifeMax = 9999;
        NPC.defense = 9999;
        NPC.dontCountMe = true;
        NPC.dontTakeDamage = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.ShowNameOnHover = false;
    }


    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        if (Main.netMode != NetmodeID.Server)
        {
            float dq = Vector2.DistanceSquared(NPC.Center, Main.LocalPlayer.Center);
            float ratio = MathHelper.Clamp(dq / (444 * 444), 0, 1f);
            ratio = 1f - ratio;
            BellFlowerSystem.WhisperingDistanceAlpha = ratio;
            ScreenShaderSystem system = ModContent.GetInstance<ScreenShaderSystem>();
            float strength = MathHelper.Lerp(1f, 5f, ratio);
            system.VignetteScreen(strength, 1f, 60);
            system.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.003f), 0.02f * ratio, 60f);
            system.TintScreen(Color.White, 0.05f * ratio, 60f);
            ShakeScreenPosition.Shake = 4 * ratio;
            Main.GraveyardVisualIntensity = 1f * ratio;
            for (int j = 0; j < Main.musicFade.Length; j++)
            {
                Main.musicFade[j] = 1f - ratio;
            }
        }
        if (_spawnTimer < 60)
        {
            _spawnTimer++;
            if(_spawnTimer == 1)
            {
                for (int i = 0; i < 32; i++)
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(80, 80);
                    Vector2 vel = pos - NPC.Center;
                    vel = vel.SafeNormalize(Vector2.Zero);
                    vel *= Main.rand.NextFloat(8f, 16f);
                    Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                    {
                        position = pos,
                        velocity = vel,
                        innerColor = Color.White.ToVector4(),
                        outerColor = Color.Blue.ToVector4(),
                        scale = new Vector2(Main.rand.NextFloat(0.8f, 1.5f)),
                        timeLeft = Main.rand.Next(60, 120),
                    });
                }

                for (int i = 0; i < 32; i++)
                {
                    TinyWhiteMothEffect();
                }
            }

            if(_spawnTimer % 4 == 0)
            {
                TinyWhiteMothEffect();
            }

            if(_spawnTimer % 10 == 0)
            {
                Particles.InDonutDust.Spawn(new()
                {
                    position = NPC.Center,
                    timeLeft = 24
                });
            }

            ShakeScreenPosition.Shake = 4;
        }

        if(BellFlowerSystem.AllBellFlowersRung())
        {
            if(State != AIState.Death)
            {
                SwitchState(AIState.Death);
            }
        }
        _alpha += 0.01f;
        _alpha = MathHelper.Clamp(_alpha, 0f, 1f);
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                if(State != AIState.Despawn && State != AIState.Death)
                {
                    SwitchState(AIState.Despawn);
                }
            }
        }
        float distanceToTargetSquared = Vector2.DistanceSquared(NPC.Center, MyTarget.Center);
        if(distanceToTargetSquared > Max_Chase_Distance_Squared)
        {
            if(State != AIState.Despawn && State != AIState.Death)
            {
                SwitchState(AIState.Despawn);
            }
        }

        foreach(var player in Main.ActivePlayers)
        {
            Vector2 suckVelocity = (NPC.Center - player.Center).SafeNormalize(Vector2.Zero);
            suckVelocity.Y = 0;
            player.velocity = Vector2.Lerp(player.velocity, suckVelocity, 0.01f);


            float distanceToPlayerSquared = Vector2.DistanceSquared(NPC.Center, player.Center);
            if(distanceToPlayerSquared < 384 * 384)
            {
                player.AddBuff(ModContent.BuffType<WhisperingDeath>(), 2);
                Vector2 pos = player.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                Vector2 vel = NPC.Center - pos;
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(6f, 52);
                if (Main.rand.NextBool(3))
                {
                    FXUtil.GlowStretch(pos, vel);
                }
            }
        }

        if (Main.rand.NextBool(7))
        {
            Particles.TinyWhiteMothDust.Spawn(new()
            {
                position = NPC.Center,
                timeLeft = Main.rand.Next(60, 120),
                velocity = Main.rand.NextVector2Circular(8, 8)
            });
        }

        if (Main.rand.NextBool(16))
        {
            Vector2 pos = NPC.position;
            pos.X += Main.rand.Next(0, NPC.width);
            pos.Y += Main.rand.Next(0, NPC.height);
            var d = Dust.NewDustPerfect(pos, DustID.GemDiamond, Vector2.Zero, Scale: 0.8f);
            d.noGravity = true;
        }


        if (Main.rand.NextBool(16))
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = NPC.Center + Main.rand.NextVector2Circular(80, 80),
                velocity = Main.rand.NextVector2Circular(8, 8),
                innerColor = Color.White.ToVector4(),
                outerColor = Color.Blue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.8f, 1f)),
                timeLeft = Main.rand.Next(60, 120),
            });
        }
        
        switch (State)
        {
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.CircleBack:
                AI_CircleBack();
                break;
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.Death:
                AI_Death();
                break;
        }
        if (Main.netMode == NetmodeID.Server)
            return;

        HairRenderer.SimulateHair(NPC.Center + new Vector2(0, -36));
        HairRenderer2.SimulateHair(NPC.Center + new Vector2(0, 36));
        Lighting.AddLight(NPC.Center, new Vector3(0.3f));
     //  AbyssEffectsRenderer.OverWater.Add(DrawWhisperer);
    }

    private void TinyWhiteMothEffect()
    {
        Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(80, 80);
        Vector2 vel = pos - NPC.Center;
        vel = vel.SafeNormalize(Vector2.Zero);
        vel *= Main.rand.NextFloat(8f, 16f);
        Particles.TinyWhiteMothDust.Spawn(new()
        {
            position = pos,
            velocity = vel,
            timeLeft = Main.rand.Next(60, 120),
        });
    }

    private void AI_CircleBack()
    {

        Timer++;
        if(Timer == 1)
        {
            _circleBackStart = (NPC.Center - MyTarget.Center);
            _circleBackCenter = MyTarget.Center;
            _startVelocity = NPC.velocity;
            if (MyTarget.Center.X < NPC.Center.X)
                _whisperingDirection = -1;
            else
                _whisperingDirection = 1;

        }

        float circleTime = 180f;
        float ease = EasingFunction.Anticipation(Timer / circleTime);
        Vector2 newOffset = _circleBackStart.RotatedBy(ease * MathHelper.Pi * _whisperingDirection);
    
        Vector2 pos = _circleBackCenter + newOffset;

        //NPC.Center = Vector2.Lerp(NPC.Center, pos, 0.1f);
        NPC.velocity = Vector2.Lerp(_startVelocity, (pos - NPC.Center), EasingFunction.InOutQuad(Timer / 90f));
        if (MyTarget.Center.X < NPC.Center.X)
            NPC.spriteDirection = -1;
        else
            NPC.spriteDirection = 1;
        if(Timer >= circleTime)
        {
            SwitchState(AIState.Chase);
        }
    }
    private void AI_Death()
    {
        Timer++;
        NPC.velocity *= 0.96f;
        ShakeScreenPosition.Shake = 4;
        CameraTargetSystem.AddTarget(NPC.Center);
        if (Timer % 4 == 0)
        {
            _shakePos += Main.rand.NextVector2Circular(7, 7);
            float range = Main.rand.NextFloat(128, 256);
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
            Vector2 vel = (NPC.Center - pos);
            vel *= 0.1f;
            FXUtil.GlowStretch(pos, vel);
        }

        if (Timer % 4 == 0)
        {
            float range = Main.rand.NextFloat(252, 400);
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
            Vector2 vel = (NPC.Center - pos);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 1f));
            fx.VectorScale *= 0.5f;
        }

        if (Timer >= 240)
        {
            WhisperingDeathMessage();
            if (Main.netMode != NetmodeID.Server)
            {
                WhisperingDeathEffect();
            }
            NPC.Kill();
        }
    }

    private void WhisperingDeathMessage()
    {
        string message = LangText.Common("Whisperer");
        if (Main.netMode == NetmodeID.Server)
        {
            NetworkText txt = NetworkText.FromLiteral(message);
            ChatHelper.BroadcastChatMessage(txt, new Color(34, 121, 100));
        }
        else
        {
            Main.NewText(message, 34, 121, 100);
        }
    }

    private void WhisperingDeathEffect()
    {
        void SpawnGore(int index)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
            int g = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + velocity, velocity * 2, ModContent.GoreType<TheWhispererGore>());
            Gore gore = Main.gore[g];
            gore.frame = (byte)index;
        }

        for (int i = 0; i < 3; i++)
        {
            SpawnGore(i);
        }
        FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.SkyBlue, Color.Blue, 25, baseSize: 0.24f);
        for (int i = 0; i < 32; i++)
        {
            Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(80, 80);
            Vector2 vel = pos - NPC.Center;
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(8f, 16f);
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = pos,
                velocity = vel,
                innerColor = Color.White.ToVector4(),
                outerColor = Color.Blue.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.8f, 1.5f)),
                timeLeft = Main.rand.Next(60, 120),
            });
        }

        for (int i = 0; i < 16; i++)
        {
            Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(80, 80);
            Vector2 vel = pos - NPC.Center;
            vel = vel.SafeNormalize(Vector2.Zero);
            vel *= Main.rand.NextFloat(4f, 8f);
            var d = Dust.NewDustPerfect(pos, DustID.GemDiamond, vel, Scale: 0.8f);
            d.noGravity = true;
        }

        Particles.RoarDust.Spawn(RoarDustData.Default with { position = NPC.Center, timeLeft = 24 });
        ShakeScreenPosition.Shake = 2;
        var sound = AssetReferences.Assets.Sounds.DeathShotBomb2.Asset with { Pitch = 0.5f, PitchVariance = 0.3f };
        SoundEngine.PlaySound(sound, NPC.Center);
    }

    private void AI_Despawn()
    {
        _alpha -= 0.05f;
        Timer++;
        NPC.velocity.X *= 0.98f;
        NPC.velocity.Y -= 0.05f;
        if (Timer >= 90)
            NPC.active = false;
    }

    private void AI_Chase()
    {
        SteamTimer++;
        float disappearTime = MathHelper.Lerp(1800, 400, (float)BellFlowerSystem.RungBellFlowerCount / (float)BellFlowerSystem.MaxBellFlowers);;
        if (SteamTimer >= disappearTime)
        {
            SwitchState(AIState.Despawn);
            return;
        }
        Timer++;
        float distanceSquaredToTarget = Vector2.DistanceSquared(NPC.Center, MyTarget.Center);
        if(distanceSquaredToTarget < 100 * 100)
        {
            NPC.velocity *= 0.96f;
            NPC.velocity.Y += MathF.Sin(Timer * 0.02f) * 0.05f;
        }
        else
        {
            MovementUtilities.AIMoveTowardsTarget(
                NPC.Center,
                MyTarget.Center,
                ref NPC.velocity,
                speed: MoveSpeed * EasingFunction.InOutSine(Timer / 60f),
                lerp: 0.05f * EasingFunction.InOutSine(Timer / 60f));
            
            NPC.spriteDirection = MyTarget.Center.X < NPC.Center.X ? -1 : 1;
        }


        float circleTime = MathHelper.Lerp(800, 300, (float)BellFlowerSystem.RungBellFlowerCount / (float)BellFlowerSystem.MaxBellFlowers); ;
        if (Timer >= circleTime)
        {
            SwitchState(AIState.CircleBack);
        }
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    private void DrawVortexGlow(SpriteBatch spriteBatch, Vector2 sp)
    {
        var pass = AssetReferences.Effects.Generic.SpiralAura.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly * 0.1f;
        pass.Parameters.bloomColor = Color.Blue.ToVector4();
        var sampler = new HlslSampler();
        sampler.Sampler = SamplerState.PointWrap;
        sampler.Texture = AssetReferences.Assets.NoiseTextures.PerlinNoise.Asset.Value;
        pass.Parameters.noiseSampler = sampler;

        var sampler2 = new HlslSampler();
        sampler2.Sampler = SamplerState.PointClamp;
        sampler2.Texture = AssetReferences.Assets.GlowMasks.SpiralVortex.Asset.Value;
        pass.Parameters.spriteSampler = sampler2;
        pass.Parameters.spiralStrength = 0.95f;
        pass.Apply();
        using(new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with {  effect = pass.Shader, samplerState = SamplerState.AnisotropicClamp }))
        {
            SpritebatchDrawer vortex = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SpiralVortex.Asset, NPC.Center);
            vortex.rotation = Main.GlobalTimeWrappedHourly;
            vortex.color = Color.White * 0.3f* OscAlpha * _alpha; 
            vortex.color.A = 0;
            vortex.scale *= 1.5f;
            vortex.worldPosition += _shakePos;

            SpritebatchDrawer skullDrawer = SpritebatchDrawer.FromNPC(NPC);
            skullDrawer.color = Color.White * _alpha;
            skullDrawer.worldPosition.Y += ExtraMath.Osc(-4f, 4f, offset: 3);


            SpritebatchDrawer lanternDrawer = skullDrawer;
            lanternDrawer.VerticalFrame(2, Main.npcFrameCount[Type]);
            lanternDrawer.worldPosition.Y += ExtraMath.Osc(-8f, 8f, speed: 0.8f);
            Vector2 lanternCenterOrigin = new Vector2(127);
            if (NPC.spriteDirection == -1)
                lanternDrawer.Flip(ref lanternCenterOrigin.X);
            Vector2 lanternWorldPos = NPC.Center + lanternCenterOrigin - lanternDrawer.drawOrigin;
            vortex.worldPosition = lanternWorldPos;
            spriteBatch.Draw(vortex);

            vortex.color *= 0.2f;
            vortex.scale *= 1.5f;
            spriteBatch.Draw(vortex);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        return false;
        // return base.PreDraw(spriteBatch, screenPos, drawColor);
    }
    public void DrawWhisperer()
    {
        DrawWhisperer(Main.spriteBatch);
    }
    public void DrawWhisperer2(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        DrawWhisperer(Main.spriteBatch);
    }
    private void DrawWhisperer(SpriteBatch spriteBatch)
    {
        float range = 24;
        DrawUtilities.DrawBasicGlow(spriteBatch, NPC.Center + _shakePos, 0.7f, Color.Blue * 0.3f * _alpha);


        float a = (float)BellFlowerSystem.RungBellFlowerCount / (float)BellFlowerSystem.MaxBellFlowers;
        var pass = AssetReferences.Effects.Abyss.WhispererAura.CreateSpritePass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly * 0.5f;

     
        pass.Parameters.alpha = MathHelper.Lerp(1f, 0f, a );
        pass.Apply();

        SpritebatchDrawer skullDrawer = SpritebatchDrawer.FromNPC(NPC);
        skullDrawer.color = Color.White * _alpha * OscAlpha;
        skullDrawer.worldPosition.Y += ExtraMath.Osc(-range, range, offset: 3);
        skullDrawer.worldPosition += _shakePos;

        SpritebatchDrawer lanternDrawer = skullDrawer;
        lanternDrawer.VerticalFrame(2, Main.npcFrameCount[Type]);
        lanternDrawer.worldPosition.Y += ExtraMath.Osc(-range, range, speed: 0.8f);
        Vector2 lanternCenterOrigin = new Vector2(127);
        if (NPC.spriteDirection == -1)
            lanternDrawer.Flip(ref lanternCenterOrigin.X);
        Vector2 lanternWorldPos = NPC.Center + lanternCenterOrigin - lanternDrawer.drawOrigin;

        SpritebatchDrawer headDrawer = skullDrawer;
        headDrawer.VerticalFrame(1, Main.npcFrameCount[Type]);
        headDrawer.color *= OscAlpha;
        headDrawer.worldPosition.Y += ExtraMath.Osc(-range, range, offset: 6);



        SpritebatchDrawer eyeDrawer = skullDrawer;
        eyeDrawer.VerticalFrame(3, Main.npcFrameCount[Type]);
        Vector2 directionToTarget = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        eyeDrawer.worldPosition += directionToTarget * 4f;
        eyeDrawer.worldPosition.Y += ExtraMath.Osc(-range, range, offset: 6);
        eyeDrawer.color = Color.White * ExtraMath.Osc(0.75f, 2f, speed: 2) * OscAlpha;
        eyeDrawer.color.A = 0;

        using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { effect = pass.Shader }))
        {
            for(float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
            {
                void Draw(in SpritebatchDrawer drawer)
                {
                    var d = drawer;
                    d.color *= 0.4f * (1f -EasingFunction.OutExpo(a+0.2f));
                    d.worldPosition += (f+Main.GlobalTimeWrappedHourly).ToRotationVector2() * 6;
                    spriteBatch.Draw(d);
                }
                Draw(skullDrawer);
                Draw(headDrawer);
                Draw(eyeDrawer);
                Draw(lanternDrawer);
            }
            spriteBatch.Draw(skullDrawer);
            spriteBatch.Draw(headDrawer);
            spriteBatch.Draw(eyeDrawer);
            spriteBatch.Draw(lanternDrawer);
        }

        eyeDrawer.color = Color.Black * (1f - a);
        spriteBatch.Draw(eyeDrawer);

        DrawUtilities.DrawBasicGlow(spriteBatch, NPC.Center, 0.7f, Color.White * 0.6f * _alpha * (1f - a));

        DrawUtilities.DrawBasicGlow(spriteBatch, lanternWorldPos + _shakePos, 0.3f, Color.Blue * 0.3f * _alpha);
    }

    public override void OnKill()
    {
        base.OnKill();
        DownedBossTracker.ClearFlag(DownedBossFlag.TheWhisperer);
    }

    //TODO: batch this probably
    //Might not really be necessary it's only 3 draws
    private void DrawHair(GraphicsDevice graphicsDevices)
    {
        HairRenderer.additive = false;
        HairShader shader = ShaderContent.GetInstance<HairShader>();
        shader.LaserTexture = AssetReferences.Assets.LaserTextures.SpectralHair.Asset;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
        shader.WaveFrequency = 8;
        shader.XOffset = 12;
        HairRenderer.Render(shader);
    }

    private void DrawHair2(GraphicsDevice graphicsDevices)
    {
        HairRenderer.additive = false;
        HairShader shader = ShaderContent.GetInstance<HairShader>();
        shader.LaserTexture = AssetReferences.Assets.LaserTextures.SpectralHair.Asset;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f + 8f; 
        shader.WaveFrequency = 8;
        shader.XOffset = 12;
        HairRenderer.Render(shader);

        shader.LaserTexture = AssetReferences.Assets.LaserTextures.SpectralHair.Asset;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f + 16f;
        shader.WaveFrequency = 8;
        shader.XOffset = 12;
        HairRenderer.Render(shader);
    }

    private void DrawSuck(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        var pass = AssetReferences.Effects.Abyss.WhisperingSuck.CreatePass0();
        pass.Parameters.Time = Main.GlobalTimeWrappedHourly * -0.9f;
        pass.Parameters.bloomColor = Color.Cyan.ToVector4();
        pass.Apply();
        using(new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with
        {
            effect = pass.Shader,
            samplerState = SamplerState.PointWrap
        }))
        {
            foreach (var player in Main.ActivePlayers)
            {
                WhisperingPlayer whisperingPlayer = player.GetModPlayer<WhisperingPlayer>();
                if (whisperingPlayer.suckingAlpha < 0.005f)
                    continue;

                SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.LaserTextures.SpectralSoulSuck.Asset.Value, player.Center);
                drawer.LeftCenterOrigin();
                drawer.rotation = (NPC.Center - player.Center).ToRotation();
                float dist = Vector2.Distance(NPC.Center, player.Center) / (float)AssetReferences.Assets.LaserTextures.SpectralSoulSuck.Asset.Value.Width;
                drawer.scale = Vector2.One * new Vector2(dist, 0.4f);
                drawer.color = Color.White * whisperingPlayer.suckingAlpha;
                spriteBatch.Draw(drawer);
            }
        }
    }
    public void DrawToRenderTargets()
    {
        HairRenderer.ghostAlpha = _alpha;
        PixelationManager.QueueSpritebatchDrawAction(DrawVortexGlow, DrawLayer.OverWater);
        PixelationManager.QueueSpritebatchDrawAction(DrawSuck, DrawLayer.OverWater);
        PixelationManager.QueuePrimitivesDrawAction(DrawHair, DrawLayer.OverWater);
        PixelationManager.QueuePrimitivesDrawAction(DrawHair2, DrawLayer.OverWater);
        PixelationManager.QueueSpritebatchDrawAction(DrawWhisperer2, DrawLayer.OverWater);
        //PixelationManager.QueuePrimitivesDrawAction(DrawHair3, DrawLayer.OverNPCsAdditive);
    }


 
}
