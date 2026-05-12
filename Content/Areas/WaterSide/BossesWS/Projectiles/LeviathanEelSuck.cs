using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS.Projectiles;

public class SuckingTrailShader : CrystalShader<SuckingTrailShader>
{
    private EffectParameter _laserTextureParam;
    private EffectParameter _timeParam;
    private EffectParameter _tilingParam;
    public Asset<Texture2D> LaserTexture
    {
        set
        {
            _laserTextureParam ??= Effect.Parameters["laserTexture"];
            _laserTextureParam.SetValue(value.Value);
        }
    }

    public Vector2 Tiling
    {
        set
        {
            _tilingParam ??= Effect.Parameters["tiling"];
            _tilingParam.SetValue(value);
        }
    }

    public float Time
    {
        set
        {
            _timeParam ??= Effect.Parameters["time"];
            _timeParam.SetValue(value);
        }
    }


    public override void SetDefaults()
    {
        base.SetDefaults();
        BlendState = BlendState.AlphaBlend;
        LaserTexture = TrailRegistry.BeamTrail;
        Time = Main.GlobalTimeWrappedHourly * 24;
        Tiling = Vector2.One;
    }
}
public class LeviathanRock : ModProjectile
{
    private Asset<Texture2D> _outlineTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];
    private ref float Style => ref Projectile.ai[2];
    private int Frame
    {
        get => (int)Projectile.ai[2];
        set => Projectile.ai[2] = value;
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Frame < 4)
        {
            if (Timer == 1 && this.OwnedByLocalClient())
            {
                Frame = Main.rand.Next(3);
                Projectile.netUpdate = true;
            }
            Projectile.frame = Frame;
        }

        if(Timer >= 60f && Frame < 4)
        {
            Projectile.hostile = true;
        }

        if(Timer % 15 == 0)
        {
            var b = BubbleParticle.Spawn(Projectile.Center, Vector2.Zero, Scale: 0.2f);
            b.gravity = 0;
        }
        Vector2 targetVelocity = (Parent.Center - Projectile.Center);
        Vector2 newVelocity = targetVelocity.SafeNormalize(Vector2.Zero) * 15;

        Projectile.velocity = Vector2.Lerp(Vector2.Zero, newVelocity, EasingFunction.InExpo(Timer / 60f));
        Projectile.rotation += MathHelper.Lerp(0f, 0.05f, EasingFunction.InExpo(Timer / 30f));
        float distanceToParent = Vector2.Distance(Projectile.Center, Parent.Center);
        if (distanceToParent <= 16)
            Projectile.Kill();
    }
    
    private void DrawWhite(SpriteBatch sb)
    {
        SpritebatchDrawer projDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        if (Frame == 4)
        {
            projDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Projectile[ModContent.ProjectileType<BouncingBall>()], Projectile.Center);
            projDrawer.rotation = Projectile.rotation;

        }

        float distanceToParent = Vector2.Distance(Projectile.Center, Parent.Center);
        float outRatio = distanceToParent / 1000f;
        projDrawer.scale = Vector2.One * MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(outRatio)) * EasingFunction.InOutSine(Timer / 30f);
        if (Frame == 4)
        {
            projDrawer.scale *= 1.5f;

        }
        projDrawer.color = Color.Yellow;
        Main.spriteBatch.Draw(projDrawer);

    }
    public override bool PreDraw(ref Color lightColor)
    {
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        SpritebatchDrawer projDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        if (Frame == 4)
        {
            projDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Projectile[ModContent.ProjectileType<BouncingBall>()], Projectile.Center);
            projDrawer.rotation = Projectile.rotation;
      
        }
          
        float distanceToParent = Vector2.Distance(Projectile.Center, Parent.Center);
        float outRatio = distanceToParent / 1000f;
        projDrawer.scale = Vector2.One * MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(outRatio)) * EasingFunction.InOutSine(Timer/30f);
    if(Frame == 4)
        {
            projDrawer.scale *= 1.5f;
            OutlineRenderer.Queue(DrawWhite);
        }
        Main.spriteBatch.Draw(projDrawer);

        if(Frame < 4)
        {
            projDrawer.texture = _outlineTextureAsset.Value;
            projDrawer.color = Projectile.hostile ? Color.Red : Color.Yellow;
            Main.spriteBatch.Draw(projDrawer);
        }

        return false;
    }
}
public class LeviathanEelSuck : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private NPC Parent => Main.npc[(int)Projectile.ai[1]];
    private ref float Style => ref Projectile.ai[2];
    private TexturedQuad _quad;
    public TexturedQuad Quad
    {
        get
        {
            _quad ??= new TexturedQuad();
            return _quad;
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 480;
        Projectile.hostile = false;

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
            if(Style == 0)
            {
                SoundStyle bigSuck = AssetRegistry.Sounds.LeviathanEel.LeviBigSuck with { PitchVariance = 0.2f };
                SoundEngine.PlaySound(bigSuck, Projectile.position);
            }
            else
            {
                SoundStyle smallSuck = AssetRegistry.Sounds.LeviathanEel.LeviSmallSuck with { PitchVariance = 0.2f };
                SoundEngine.PlaySound(smallSuck, Projectile.position);
            }
        }

        if (Timer % 6 == 0)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2();
            offset = offset.RotatedByRandom(MathHelper.ToRadians(30));
            offset *= Main.rand.NextFloat(485, 512);
            Vector2 startPos = Projectile.Center + offset;
            Vector2 vel = Projectile.Center - startPos;
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(startPos, vel);
            fx.VectorScale *= 0.5f;
            fx.OuterGlowColor = Color.DarkGray;
        }

        if (Timer % 3 == 0)
        {
            Vector2 offset = Projectile.rotation.ToRotationVector2();
            offset = offset.RotatedByRandom(MathHelper.ToRadians(36));
            offset *= Main.rand.NextFloat(485, 666);
            Vector2 startPos = Projectile.Center + offset;
            Vector2 vel = Projectile.Center - startPos;
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(startPos, vel);
            fx.VectorScale *= 0.25f;
            fx.OuterGlowColor = Color.DarkGray;
        }

        if(Style == 0)
        {
            if (this.OwnedByLocalClient() && Timer % 15 == 0 && Projectile.timeLeft > 100)
            {
                Vector2 offset = Projectile.rotation.ToRotationVector2();
                offset = offset.RotatedByRandom(MathHelper.ToRadians(45));
                offset *= Main.rand.NextFloat(1200, 1400);
                Vector2 startPos = Projectile.Center + offset;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), startPos, (Projectile.Center - startPos).SafeNormalize(Vector2.Zero),
                    ModContent.ProjectileType<LeviathanRock>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Parent.whoAmI);
            }
        } else if (Style == 1)
        {
            if (Projectile.timeLeft > 100)
                Projectile.timeLeft = 100;
            if(Timer == 5 && this.OwnedByLocalClient())
            {
                Vector2 offset = Projectile.rotation.ToRotationVector2();
                offset = offset.RotatedByRandom(MathHelper.ToRadians(10));
                offset *= 800;
                Vector2 startPos = Projectile.Center + offset;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), startPos, (Projectile.Center - startPos).SafeNormalize(Vector2.Zero),
                    ModContent.ProjectileType<LeviathanRock>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Parent.whoAmI, ai2: 4);
            }
        }
  

        foreach(Player player in Main.ActivePlayers)
        {
            player.GetModPlayer<MovePlayer>().pullVelocity = -Parent.rotation.ToRotationVector2() * 1.5f;
        }
        ShakeScreenPosition.Shake = 4;
        Projectile.Center = Parent.Center;
        Projectile.rotation = Parent.rotation;
    }

    private void DrawPixelatedCone(GraphicsDevice gDevice)
    {
        Quad.Cone(Projectile.Center,
            minWidth: 128,
            maxWidth: 512,
            length: 1024, Projectile.rotation);

        float inAlpha = EasingFunction.InOutSine(Timer / 30f);
        float outAlpha = EasingFunction.InOutSine(Projectile.timeLeft / 30f);
        Color quadColor = Color.Lerp(Color.Transparent, Color.White, inAlpha * outAlpha) * 0.5f;
        quadColor.A = 0;
        Quad.SetColor(quadColor);

        SuckingTrailShader trailShader = ShaderContent.GetInstance<SuckingTrailShader>();
        trailShader.LaserTexture = TextureAssets.Projectile[Type];
        trailShader.Tiling = new Vector2(7, 1);
        trailShader.Time = Main.GlobalTimeWrappedHourly * -96;
        Quad.DrawWithShader(trailShader);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedCone);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}