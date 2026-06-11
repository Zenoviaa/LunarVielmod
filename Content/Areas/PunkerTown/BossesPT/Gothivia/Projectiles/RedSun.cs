using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.WeaponUpgrade.UI;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.GothinFlames;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;

public class RedSun : ModProjectile,
    IDrawToRenderTarget
{
    private enum AIState
    {
        GrowOne,
        GrowTwo,
        GrowThree,

        FourWay,
        Cross,
        Eightway,

        Shotgun,
        Idle,
        Shrink
    }
    private float _rotation;
    private float _rotationDirection;
    private float _squishScale;
    private float _scale;
    private float _targetScale;
    private float _blowtorchTimer;
    private float _telegraphAlpha;
    private bool _hitboxActive;
    private AnimationFramer _sunAnimationFrame;
    private List<float> _fireRotations;
    private List<float> FireRotations
    {
        get
        {
            _fireRotations ??= new List<float>();
            return _fireRotations;
        }
    }

    private float _attackCounter;
    private int HorizontalFrameCount => 16;
    private int VerticalFrameCount => 18;
    private float BlowtorchTelegraphTime => 40;
    private float TimeBetweenGrows => 90;
    private float TimeBetweenBlasts => 5;
    private float BlowtorchTime => 24;
    private ref float Timer => ref Projectile.ai[0];
    private AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    private NPC Parent => Main.npc[(int)Projectile.ai[2]];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_blowtorchTimer);
        writer.Write(_attackCounter);
        writer.Write(_rotation);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _blowtorchTimer = reader.ReadSingle();
        _attackCounter = reader.ReadSingle();
        _rotation = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if(_hitboxActive)
        {
            for (int i = 0; i < FireRotations.Count; i++)
            {
                float lineWidth = 12;
                float collisionPoint = 0;
                float rot = FireRotations[i];
                rot += Projectile.rotation;
                Vector2 newVel = rot.ToRotationVector2() * 2400;
                Vector2 pos = Projectile.Center;
                Vector2 attackPos = pos + newVel;
                bool colliding = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), pos, attackPos, lineWidth, ref collisionPoint);
                if (colliding)
                    return true;
            }
        }
        return base.Colliding(projHitbox, targetHitbox);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.light = 0.6f;
        Projectile.ignoreWater = true;
       
    }
    
    public override void AI()
    {
        base.AI();
        _sunAnimationFrame.frameSpeed = 1;
        _sunAnimationFrame.maxFrame = HorizontalFrameCount * VerticalFrameCount;
        _sunAnimationFrame.UpdateTick();
        _targetScale = 0f;
        if (_blowtorchTimer < BlowtorchTime)
            _blowtorchTimer++;
        _squishScale = 1f;
        FireRotations.Clear();
        _hitboxActive = false;
        switch (State)
        {
            case AIState.GrowOne:
                AI_GrowOne();
                break;
            case AIState.GrowTwo:
                AI_GrowTwo();
                break;
            case AIState.GrowThree:
                AI_GrowThree();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Eightway:
                AI_Eightway();
                break;
            case AIState.FourWay:
                AI_Fourway();
                break;
            case AIState.Cross:
                AI_Cross();
                break;
        }

        _rotation += 0.015f * _rotationDirection;
        Projectile.rotation = _rotation;
        Projectile.Center = Parent.Center + new Vector2(0, -256);
        _scale = MathHelper.Lerp(_scale, _targetScale, 0.1f);
    }

    private void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }
    }

    private void CreateShrinkingCircle()
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        PixelPrimitiveCircleFactory.CreateGenericInBoom(Projectile.Center, Color.OrangeRed, Color.OrangeRed, 45, 900);
    }

    private void CreateInwardParticles()
    {
        Vector2 center = Projectile.Center;
        if (Timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(768, 768);
            Vector2 vel = (center - pos);
            vel *= 0.09f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.VectorScale *= 0.5f;
        }

        if (Timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(768, 768);
            Vector2 vel = (center - pos);
            vel *= 0.09f;
            var fx = FXUtil.GlowStretch(pos, vel);
        }

        if (Timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(768, 768);
            Vector2 vel = (center - pos);
            vel *= 0.09f;
            var fx = FXUtil.GlowStretch(pos, vel);
        }

        if (Timer % 4 == 0)
        {
            Vector2 pos = center + Main.rand.NextVector2CircularEdge(768, 768);
            Vector2 vel = (center - pos);
            vel *= 0.09f;
            var dp = DustParticle.Spawn(pos, vel);
            dp.dampening = 0.1f;
            dp.noTileCollide = true;
            dp.Scale *= 0.35f;
            dp.outerColor = Color.Red;
            dp.gravity = 0;
        }
    }

    private void AI_BlastReady()
    {
        _targetScale = 1f;
        Timer++;
        _telegraphAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / BlowtorchTelegraphTime));
        if(Timer == BlowtorchTelegraphTime)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothingBow") { PitchVariance = 0.5f }, Projectile.Center);
            SoundStyle fireballShoot = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireballShoot1") with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(fireballShoot, Projectile.position);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);

            for(int i = 0; i < FireRotations.Count; i++)
            {
                float radians = FireRotations[i];
                radians += _rotation;
                for (float f = 0; f < 8; f++)
                {
                    Vector2 vel = radians.ToRotationVector2();
                    //vel = vel.SafeNormalize(Vector2.Zero);
                    vel *= Main.rand.NextFloat(5f, 100);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(24));
                   // vel = vel.RotatedBy(radians);
                    var dp = DustParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(24, 24), vel);
                    dp.gravity *= 0.5f;
                    dp.noTileCollide = true;
                    dp.dampening = 0.05f;
                    dp.Scale *= Main.rand.NextFloat(0.5f, 2f);
                }
            }

            _blowtorchTimer = 0;
        }
        
        if(Timer >= BlowtorchTelegraphTime)
        {
            float a = MathHelper.Lerp(1f, 1.25f, EasingFunction.OutExpo(_blowtorchTimer / BlowtorchTime));
            float b = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(_blowtorchTimer / BlowtorchTime));
            float c = a * b;
            _squishScale = MathHelper.Lerp(1f, 1.35f, c);
            _telegraphAlpha = 0;
            _hitboxActive = true;
            if (_blowtorchTimer >= BlowtorchTime)
            {
                SwitchState(AIState.Idle);
            }
        }
    }

    private void AI_Cross()
    {

        for(float f =0; f < MathHelper.TwoPi; f+= MathHelper.PiOver2)
        {
            float angle = f + MathHelper.PiOver4;
            FireRotations.Add(angle);
        }
        AI_BlastReady();
    }

    private void AI_Fourway()
    {
        for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2)
        {
            float angle = f;
            FireRotations.Add(angle);
        }
        AI_BlastReady();

    }

    private void AI_Eightway()
    {
     
        for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver4)
        {
            float angle = f;
            FireRotations.Add(angle);
        }
        AI_BlastReady();
    }

    private void AI_Idle()
    {
        if(_attackCounter >= 8)
        {
            SwitchState(AIState.Shrink);
        }
        _targetScale = 1f;
        Timer++;
        if(Timer == 1 && this.OwnedByLocalClient())
        {
            _rotationDirection = Main.rand.NextBool(2) ? 1 : -1;
            Projectile.netUpdate = true;
        }

        if(Timer >= TimeBetweenBlasts)
        {
            Timer = 0;
            _attackCounter++;
            ChooseBlast();
        }
    }

    private void ChooseBlast()
    {

        if (this.OwnedByLocalClient())
        {
            switch (Main.rand.Next(3))
            {
                case 0:
                    SwitchState(AIState.FourWay);
                    break;
                case 1:
                    SwitchState(AIState.Cross);
                    break;
                case 2:
                    SwitchState(AIState.Eightway);
                    break;
            }
        }
    }
    private void AI_GrowOne()
    {
        Timer++;
        if(Timer == 1)
        {
            CreateShrinkingCircle();
        }
        CreateInwardParticles();
        _targetScale = 0.2f;
        if(Timer >= TimeBetweenGrows)
        {
            SwitchState(AIState.GrowTwo);
        }
    }

    private void AI_GrowTwo()
    {
        Timer++;
        if (Timer == 1)
        {
            CreateShrinkingCircle();
        }
        CreateInwardParticles();
        _targetScale = 0.5f;
        if (Timer >= TimeBetweenGrows)
        {
            SwitchState(AIState.GrowThree);
        }
    }

    private void AI_GrowThree()
    {
        Timer++;
        if (Timer == 1)
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            shaderSystem.TintScreen(Color.OrangeRed, 0.1f, timer: 680);
            shaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.001f, 0.001f), blend: 0.025f, timer: 560);
            shaderSystem.VignetteScreen(-1f, timer: 560);
            CreateShrinkingCircle();
        }
        CreateInwardParticles();
        _targetScale = 1f;
        if (Timer >= TimeBetweenGrows)
        {
            SwitchState(AIState.Idle);
        }
    }
    private void DrawTelegraphLine(SpriteBatch spriteBatch)
    {
        Asset<Texture2D> bloomLineTextureAsset = ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/BloomLine");
        void DrawLineInner(Vector2 direction)
        {
            SpritebatchDrawer lineDrawer = SpritebatchDrawer.FromTextureAsset(bloomLineTextureAsset, Projectile.Center);
            lineDrawer.rotation = direction.ToRotation() - MathHelper.PiOver2 ;
            lineDrawer.color = Color.White * _telegraphAlpha * ExtraMath.Osc(0.4f, 1f, speed: 32);
            lineDrawer.color.A = 0;
            lineDrawer.TopCenterOrigin();
            lineDrawer.scale.Y *= 4;
            lineDrawer.scale.X *= 0.4f;
            lineDrawer.worldPosition += direction.SafeNormalize(Vector2.Zero) * 96;


            spriteBatch.Draw(lineDrawer);

            SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
            glowDrawer.worldPosition += direction.SafeNormalize(Vector2.Zero) * 128;
            glowDrawer.color = Color.Lerp(Color.Orange, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 24)) * _telegraphAlpha;
            glowDrawer.color.A = 0;
            glowDrawer.scale = new Vector2(0.25f, 0.5f) * 0.75f;
            glowDrawer.rotation = direction.ToRotation();
            spriteBatch.Draw(glowDrawer);
        }

        for (int i = 0; i <FireRotations.Count; i++)
        {
            float rot = FireRotations[i];
            rot += Projectile.rotation;
            Vector2 offset = rot.ToRotationVector2();
            DrawLineInner(offset);
        }
    }

    private void DrawBlowtorch(SpriteBatch spriteBatch, Vector2 sp)
    {
        if (!_hitboxActive)
            return;

        float progress = _blowtorchTimer / BlowtorchTime;
        BlowTorchShader torchShader = ShaderContent.GetInstance<BlowTorchShader>();
        torchShader.Time = EasingFunction.OutExpo(progress);
        torchShader.FlameNoiseTexture = AssetManager.Noise.FlameVortexNoise;

        Color bloomColor = Color.Lerp(Color.Red, Color.Blue, EasingFunction.OutExpo(progress));
        torchShader.BloomColor = Color.Lerp(bloomColor, Color.Black, EasingFunction.InExpo(progress));
        torchShader.InsideColor = Color.Lerp(Color.Yellow, Color.OrangeRed, EasingFunction.OutExpo(progress));

        //Drawing all the blowtorches in one projectile so it's optimized and not restarting the spritebatch multiple times times
        //Also not eating up projectile slots
        //I'm so smart guys
        //This also means we can have infinite torches
        spriteBatch.Restart(effect: torchShader.Effect);
        for (int i = 0; i < FireRotations.Count; i++)
        {
            DrawBlowtorchInner(spriteBatch, sp, (FireRotations[i]+Projectile.rotation).ToRotationVector2(), progress);
        }

        spriteBatch.RestartDefaults();
    }

    private void DrawBlowtorchInner(SpriteBatch spriteBatch, Vector2 sp, Vector2 direction, float progress)
    {
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.JumbledGlowCircle.Asset, Projectile.Center);
        glowDrawer.Origin(0.1f, 0.5f);
        glowDrawer.scale.X *= MathHelper.Lerp(1f, 3.5f, EasingFunction.OutExpo(progress));
        glowDrawer.scale.Y *= MathHelper.Lerp(1f, 0f, EasingFunction.OutSine(progress));
        glowDrawer.color = Color.Yellow;
        glowDrawer.color.A = 0;
        glowDrawer.rotation = direction.ToRotation();
        glowDrawer.worldPosition += direction.SafeNormalize(Vector2.Zero) * 96;


        spriteBatch.Draw(glowDrawer);
        spriteBatch.Draw(glowDrawer);

        glowDrawer.color = Color.DarkRed;
        glowDrawer.color.A = 0;
        glowDrawer.scale.Y *= 5;
        spriteBatch.Draw(glowDrawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawTelegraphLine(Main.spriteBatch);

        float scale = _scale * 0.6f * _squishScale;
        Asset<Texture2D> sunTextureAsset = TextureAssets.Projectile[Type];
        Rectangle sunFrame = sunTextureAsset.Value.GetFrame(_sunAnimationFrame.frame, HorizontalFrameCount, VerticalFrameCount);
        SpritebatchDrawer sunDrawer = SpritebatchDrawer.FromTextureAsset(sunTextureAsset, Projectile.Center);
        sunDrawer.color = Color.White * 0.75f;
        sunDrawer.color.A = 0;
        sunDrawer.scale *= scale;
        sunDrawer.sourceRect = sunFrame;
        sunDrawer.CenterOrigin();
        sunDrawer.scale *= 2;
        Main.spriteBatch.Draw(sunDrawer);


        RedSunShader redSunShader = ShaderContent.GetInstance<RedSunShader>();
        redSunShader.Time = Main.GlobalTimeWrappedHourly * 9;
        redSunShader.InsideColor = Color.Yellow;
        redSunShader.BloomColor = Color.DarkRed;
        redSunShader.FlameNoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        Main.spriteBatch.Restart(SpriteSortMode.Immediate, effect: redSunShader.Effect, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointWrap);
        SpritebatchDrawer redSunDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/WaterTrail"), Projectile.Center);
        redSunDrawer.scale *= scale * 2;
        redSunDrawer.color = Color.White;
        redSunDrawer.color.A = 0;
        //
        Main.spriteBatch.Draw(redSunDrawer);
        Main.spriteBatch.RestartDefaults();


        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.JumbledGlowCircle.Asset.Value, Projectile.Center);
        glowDrawer.color = Color.Red * 0.16f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= scale * 6;
        Main.spriteBatch.Draw(glowDrawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawBlowtorch);
    }
}
