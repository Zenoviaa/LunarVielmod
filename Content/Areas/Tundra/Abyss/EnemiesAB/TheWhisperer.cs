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
        player.statLife -= 2;
        player.lifeRegen = -5;
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
        Despawn,

        Death
    }

    private Vector2 _shakePos;
  
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private HairRenderer _hairRendererBackingField;
    private HairRenderer HairRenderer
    {
        get
        {
            _hairRendererBackingField ??= new HairRenderer(NPC.Center, 9, 128);
            return _hairRendererBackingField;
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

    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);

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
            system.VignetteScreen(5f * ratio, 1f, 60);
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
            if(distanceToPlayerSquared < Suck_Distance_Squared)
            {
                player.AddBuff(ModContent.BuffType<WhisperingDeath>(), 2);
            }
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
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.Death:
                AI_Death();
                break;
        }
        HairRenderer.SimulateHair(NPC.Center + new Vector2(0, -36));
        Lighting.AddLight(NPC.Center, new Vector3(0.3f));
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
        NPC.velocity.Y -= 0.5f;
        if (Timer >= 90)
            NPC.active = false;
    }

    private void AI_Chase()
    {
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
                speed: MoveSpeed,
                lerp: 0.05f);
            MovementUtilities.FaceMovementVelocity(NPC);
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
        float range = 24;
        DrawUtilities.DrawBasicGlow(spriteBatch, NPC.Center + _shakePos, 0.7f, Color.Blue * 0.3f * _alpha);


        var pass = AssetReferences.Effects.Abyss.WhispererAura.CreateSpritePass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly * 0.5f;
        pass.Parameters.alpha = (float)BellFlowerSystem.RungBellFlowerCount / (float)BellFlowerSystem.MaxBellFlowers;
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

        using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with 
            {  effect = pass.Shader }))
        {
            spriteBatch.Draw(skullDrawer);

            SpritebatchDrawer headDrawer = skullDrawer;
            headDrawer.VerticalFrame(1, Main.npcFrameCount[Type]);
            headDrawer.color *= OscAlpha;
            headDrawer.worldPosition.Y += ExtraMath.Osc(-range, range, offset: 6);
            spriteBatch.Draw(headDrawer);

            SpritebatchDrawer eyeDrawer = skullDrawer;
            eyeDrawer.VerticalFrame(3, Main.npcFrameCount[Type]);
            Vector2 directionToTarget = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            eyeDrawer.worldPosition += directionToTarget * 4f;
            eyeDrawer.worldPosition.Y += ExtraMath.Osc(-range, range, offset: 6);
            eyeDrawer.color = Color.White * ExtraMath.Osc(0.75f, 2f, speed: 2) * OscAlpha;
            eyeDrawer.color.A = 0;
            spriteBatch.Draw(eyeDrawer);

            spriteBatch.Draw(lanternDrawer);
        }


        DrawUtilities.DrawBasicGlow(spriteBatch, lanternWorldPos + _shakePos, 0.3f, Color.Blue * 0.3f * _alpha);
        return false;
       // return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    public override void OnKill()
    {
        base.OnKill();
        DownedBossTracker.ClearFlag(DownedBossFlag.TheWhisperer);
    }

    private void DrawHair(GraphicsDevice graphicsDevices)
    {
        HairShader shader = ShaderContent.GetInstance<HairShader>();
        shader.LaserTexture = AssetReferences.Assets.LaserTextures.SpectralHair.Asset;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
        shader.WaveFrequency = 8;
        shader.XOffset = 12;
        HairRenderer.Render(shader);
    }
    private void DrawHair2(GraphicsDevice graphicsDevices)
    {
        HairShader shader = ShaderContent.GetInstance<HairShader>();
        shader.LaserTexture = AssetReferences.Assets.LaserTextures.SpectralHair.Asset;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f + 8f; 
        shader.WaveFrequency = 8;
        shader.XOffset = 12;
        HairRenderer.Render(shader);
    }
    private void DrawHair3(GraphicsDevice graphicsDevices)
    {
        HairShader shader = ShaderContent.GetInstance<HairShader>();
        shader.LaserTexture = AssetReferences.Assets.LaserTextures.SpectralHair.Asset;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f + 16f;
        shader.WaveFrequency = 8;
        shader.XOffset = 12;
        HairRenderer.Render(shader);
    }
    public void DrawToRenderTargets()
    {
        HairRenderer.ghostAlpha = _alpha;
        PixelationManager.QueueSpritebatchDrawAction(DrawVortexGlow);
        PixelationManager.QueuePrimitivesDrawAction(DrawHair, DrawLayer.BehindNPCsWithOutline);
        PixelationManager.QueuePrimitivesDrawAction(DrawHair2, DrawLayer.OverNPCsAdditive);
        PixelationManager.QueuePrimitivesDrawAction(DrawHair3, DrawLayer.OverNPCsAdditive);
    }

}
