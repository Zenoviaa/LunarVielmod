using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.GothinFlames;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;

public class FlameSwirl : ModProjectile,
    IDrawToRenderTarget
{

    private float _circleRadius;
    private float _insideRadius;
    private ref float Timer => ref Projectile.ai[0];
    private ref float DeadZoneMidAngle => ref Projectile.ai[1];
    private ref float DeadZoneAngleRadius => ref Projectile.ai[2];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        Vector2 targetCenter = targetHitbox.Center();
        Vector2 center = projHitbox.Center();


        float distance = Vector2.Distance(center, targetCenter);
        if (distance < _circleRadius && distance > _insideRadius)
        {
            Vector2 vecToTarget = (targetCenter - center);
            float angleToTarget = vecToTarget.ToRotation();
            float angleDiff = ExtraMath.AngleDiff(angleToTarget, DeadZoneMidAngle);
            if (angleDiff > DeadZoneAngleRadius)
            {
                return true;
            }
        }
        return false;
        //return base.Colliding(projHitbox, targetHitbox);
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_circleRadius);
        writer.Write(_insideRadius);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _circleRadius = reader.ReadSingle();
        _insideRadius = reader.ReadSingle();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 180;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle fireIn = new SoundStyle("Stellamod/Assets/Sounds/Fire/FireballShoot1") with { PitchVariance = 0.66f };
            SoundEngine.PlaySound(fireIn, Projectile.position);
        }

        if(Timer < 60f)
        {
            _circleRadius = MathHelper.Lerp(0f, MathHelper.Lerp(120f, 130f, Timer / 60f), EasingFunction.InOutExpo(Timer / 60f));

        } else if (Timer < 180f)
        {
            if(Timer == 78)
            {
                SoundStyle fireIn = AssetRegistry.Sounds.Fire.FlameoutWheel;
                SoundEngine.PlaySound(fireIn, Projectile.position);
            }

            float t = Timer - 60f;
        
            float ease = EasingFunction.InExpo(t / 120f);
            float ease2 = EasingFunction.OutExpo(t / 120f);
            float ease3 = MathHelper.Lerp(ease, ease2, EasingFunction.InExpo(t / 35f));
            _circleRadius = MathHelper.Lerp(130f, 1000f, ease3);
        }

        _insideRadius = _circleRadius - 64;
        DeadZoneMidAngle -= 0.01f;
        DeadZoneMidAngle = MathHelper.WrapAngle(DeadZoneMidAngle);
        Projectile.rotation -= 0.05f;
    }

    private float CalculateRadius(float timestep)
    {
        if (timestep < 60f)
        {
            return MathHelper.Lerp(0f, MathHelper.Lerp(120f, 130f, Timer / 60f), EasingFunction.InOutExpo(Timer / 60f));

        }
        else if (timestep < 180f)
        {

            float t = timestep - 60f;

            float ease = EasingFunction.InExpo(t / 120f);
            float ease2 = EasingFunction.OutExpo(t / 120f);
            float ease3 = MathHelper.Lerp(ease, ease2, EasingFunction.InExpo(t / 35f));
            return MathHelper.Lerp(130f, 1000f, ease3);
        }
        return 1;

    }

    public override bool PreDraw(ref Color lightColor) => false;
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        target.GetModPlayer<GothiviaPlayer>().AddSunStack();
    }


    private void DrawFlameSwirl(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        FlameSwirlShader flameSwirlShader = ShaderContent.GetInstance<FlameSwirlShader>();
        flameSwirlShader.AngleCenter = DeadZoneMidAngle;
        flameSwirlShader.AngleRadius = DeadZoneAngleRadius;
        flameSwirlShader.InsideColor = Color.Lerp(Color.White, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 12));
        flameSwirlShader.BloomColor = Color.Red;
        flameSwirlShader.Time = Main.GlobalTimeWrappedHourly * 12;
  
        SpritebatchParams flameSwirlSparams = SpritebatchParams.InWorldAndZoomed() with { effect = flameSwirlShader };
     


        void DrawSwirlInner(float timestep, float alpha = 1f)
        {
            float easeIn = EasingFunction.InOutSine(timestep / 60f);
            float easeOuth = EasingFunction.InOutSine((float)Projectile.timeLeft / 120f);
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.Noise.InvertedVoronoi.Asset.Value, Projectile.Center);
            Vector2 size = drawer.texture.Size();
            float pct = CalculateRadius(timestep) / size.X;
            drawer.scale = Vector2.One * pct * 2;

            drawer.color = Color.White * easeOuth * easeIn * alpha;
            drawer.color.A = 0;
            //  drawer.rotation = Projectile.rotation;
            spriteBatch.Draw(drawer);

            drawer.color = Color.Red * 0.2f * easeOuth * easeIn * alpha;
            drawer.color.A = 0;
            drawer.scale *= 1.2f;
            spriteBatch.Draw(drawer);

            drawer.scale *= 0.7f;
            spriteBatch.Draw(drawer);
        }
        using (SpritebatchStarter.Begin(spriteBatch, flameSwirlSparams))
        {
            float afterImageAlpha = 0.5f;
            for(float f = Timer - 3; f > 0 && afterImageAlpha > 0; f-= 3)
            {
                afterImageAlpha -= 0.1f;
                DrawSwirlInner(f,afterImageAlpha );
            }
            DrawSwirlInner(Timer);
        }
    }

    public void DrawToRenderTargets()
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawFlameSwirl);
    }
}
