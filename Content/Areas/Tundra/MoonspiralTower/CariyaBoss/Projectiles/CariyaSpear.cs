using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.CariyaBoss.Projectiles;


public class CariyaSpear : ModProjectile
{
    private bool _spears;
    private Asset<Texture2D> _mirageAsset;
    private ref float Timer => ref Projectile.ai[0];
    private Vector2 _mirageOffset;
    private float[] _spearRotationOffsets;
    private Vector2[] _spearPositionOffsets;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _spearPositionOffsets = new Vector2[15];
        _spearRotationOffsets = new float[15];
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
        Projectile.ignoreWater = true;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }


    public override void AI()
    {
        base.AI();
        if (!_spears)
        {
            for(int i = 0; i <_spearPositionOffsets.Length; i++)
            {
                ref Vector2 offset = ref _spearPositionOffsets[i];
                ref float rotationOffset  = ref _spearRotationOffsets[i];
                offset.X = ExtraMath.Osc(-200f, 200f, 0, offset: i);
                offset.X += Main.rand.NextFloat(-10f, 10f);

                float max = MathHelper.ToRadians(60);
                float min = MathHelper.ToRadians(30);
                rotationOffset = MathHelper.Lerp(min, max, MathF.Abs(offset.X) / 200f);
                float dir = offset.X > 0 ? 1 : -1;
                rotationOffset *= dir;
            }

            _spears = true;
        }
        Timer++;
        if(Timer % 4 == 0)
        {
            Vector2 pos = Projectile.Center;
            pos.X += Main.rand.NextFloat(-200f, 200f);
            var fx = FXUtil.GlowStretch(pos, -Vector2.UnitY * Main.rand.NextFloat(2f, 5f));
            fx.VectorScale *= 0.5f;
        }

        if(Timer % 4 == 0)
        {
            _mirageOffset = Main.rand.NextVector2Circular(4, 4);
        }
        if (Timer % 7 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(96, 96);
            var d = Dust.NewDustPerfect(pos, DustID.GemSapphire, Scale: 1f);
            d.noGravity = true;
        }

        if (Timer % 6 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(64, 64);
            Vector2 vel = -Projectile.velocity * 0.3f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Blue;
            fx.VectorScale *= 0.5f;
        }

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        _mirageAsset ??= ModContent.Request<Texture2D>(Texture + "_Mirage");

        float outAlpha = EasingFunction.InOutSine((float)Projectile.timeLeft / 30f);
        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BulbTrail;
        shader.Distortion = 0.02f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = MathHelper.Lerp(4f, 0.8f, EasingFunction.InOutSine(Timer / 60f));
        shader.Tiling = Vector2.One * 0.75f;
        shader.InnerColor = Color.Lerp(Color.LightBlue, Color.Lerp(Color.LightBlue, Color.Blue, 0.4f), ExtraMath.Osc(0f, 1f, 12)) * 0.5f * outAlpha;
        shader.OuterColor = Color.DarkBlue * 0.5f * outAlpha;
        Main.spriteBatch.Restart(effect: shader.Effect);


        
        float baseRotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        void DrawSpear(int i, SpritebatchDrawer drawer)
        {
            Vector2 positionOffset = _spearPositionOffsets[i];
            float rotationOffset = _spearRotationOffsets[i];
            drawer.BottomLeftOrigin();
            drawer.worldPosition += positionOffset;
            drawer.color *= outAlpha * Main.rand.NextFloat(0.28f, 1f);
            drawer.rotation = baseRotation + rotationOffset;

            Vector2 rotOffset = (drawer.rotation - MathHelper.PiOver4).ToRotationVector2();
            float offsetAmount = MathF.Abs(positionOffset.X) / 200f;
            float time = (Timer) - (offsetAmount * 20);
            float ratio = time / 25f;
            float ease = EasingFunction.OutExpo(ratio);
            drawer.worldPosition -= rotOffset * MathHelper.Lerp(1f, 0.1f, ease) * 128;
            drawer.color *= ease;
            Main.spriteBatch.Draw(drawer);
        }

        for(int i = 0; i < _spearRotationOffsets.Length; i++)
        {
            Vector2 positionOffset = _spearPositionOffsets[i];
            float rotationOffset = _spearRotationOffsets[i];

            SpritebatchDrawer afDrawer = SpritebatchDrawer.FromTextureAsset(_mirageAsset, Projectile.Center);
            afDrawer.color.A = 0;
            DrawSpear(i, afDrawer);
        }

        Main.spriteBatch.RestartDefaults();


        for (int i = 0; i < _spearRotationOffsets.Length; i++)
        {
            Vector2 positionOffset = _spearPositionOffsets[i];
            float rotationOffset = _spearRotationOffsets[i];

            SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            DrawSpear(i, afDrawer);
   
        }
        return false;
    //    return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
