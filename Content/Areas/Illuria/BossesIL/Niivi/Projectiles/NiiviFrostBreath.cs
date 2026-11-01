using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi.Projectiles;

public class NiiviFrostSpike : ModProjectile,
    IDrawToRenderTarget
{

    Vector2 _scale;
    float _rot;
    float Scale
    {
        get
        {
            float inScale = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / 60F));
            float outScale = MathHelper.Lerp(1f, 0f, EasingFunction.InSine(Timer / 120F));
            return inScale * outScale;
        }
    }
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Illuria.NPCs.NiiviIceRupture.KEY;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_scale);
        writer.Write(_rot);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _scale = reader.ReadVector2();
        _rot = reader.ReadSingle();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 10;
    }

    public override bool CanHitPlayer(Player target)
    {
        if (Timer < 30)
            return false;
        if (Scale < 0.2f)
            return false;
        return base.CanHitPlayer(target) ;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.light = 0.7F;
        Projectile.width = 64;
        Projectile.height = 256;
        Projectile.hostile = true;
        Projectile.timeLeft = 120;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
    }
   
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                var scale = Main.rand.NextFloat(0.7f, 1f);
                _scale = new Vector2(scale);
                _rot = Main.rand.NextFloat(-0.3f, 0.3f);
                Projectile.netUpdate = true;
            }
        }

        if (Main.rand.NextBool(30))
        {
            var pos = Projectile.Bottom + Main.rand.NextVector2Circular(64, 64);
            pos.Y -= 128;

            Particles.FaintSmokeDust.Spawn(new()
            {
                position = pos,
                velocity = Vector2.Zero,
                timeleft = 45,
                color = Color.SkyBlue * 0.14f
            });
        }

        if(Projectile.frame < Main.projFrames[Type] - 1)
        {
            Projectile.frameCounter++;
            if(Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
        }
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    void DrawSpike(SpriteBatch sb, Color lightColor)
    {

        var drawer = Projectile.Drawer;
        drawer.BottomCenterOrigin();
        drawer.worldPosition = Projectile.Bottom;
        drawer.rotation = _rot;
        drawer.scale = _scale * Scale;
        drawer.color = lightColor;
        Main.spriteBatch.Draw(drawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        DrawSpike(Main.spriteBatch, lightColor);
        return false;
    }

    public void DrawToRenderTargets()
    {
        void DrawSprite(SpriteBatch sb)
        {
            DrawSpike(sb, Color.Red);
        }
        OutlineRenderer.Queue(DrawSprite);
    }
}
public class NiiviFrostBreath : ModProjectile
{
    Vector2 _leftStart;
    Vector2 _rightStart;
    Vector2 _leftImpactPoint;
    Vector2 _rightImpactPoint;
    ref float Timer => ref Projectile.ai[0];
    NPC Parent => Main.npc[(int)Projectile.ai[1]];
    ref float FrostBreathTime => ref Projectile.ai[2];
    public override string Texture => ModType.NoTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.width = 2560;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.light = 0.6f;
        Projectile.timeLeft = 600;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0;
        var lineWidth = 80;
        Vector2 position = Projectile.Center + Projectile.velocity;
        Vector2 previousPosition = Projectile.Center;
        if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, lineWidth, ref collisionPoint))
            return true;
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;

        if(Timer >= FrostBreathTime)
        {
            Projectile.Kill();
        }

        ShakeScreenPosition.Shake = 2;
        Vector2 parentLook = Parent.rotation.ToRotationVector2();
        Projectile.Center = Parent.Center + parentLook * new Vector2(54, 67);
        Vector2 perpVel = Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
        Projectile.velocity = parentLook * 1100;

        _leftStart = Projectile.Center - perpVel * 0.4f + parentLook;
        _rightStart = Projectile.Center + perpVel * 0.4f + parentLook;


        var leftVelocity = _leftStart - Projectile.Center;
        leftVelocity = leftVelocity.SafeNormalize(Vector2.Zero);

        var rightVelocity = _rightStart - Projectile.Center;
        rightVelocity = rightVelocity.SafeNormalize(Vector2.Zero);

        _leftImpactPoint = CollisionHelper.SolidTileRayCast(_leftStart, leftVelocity, 2400);
        _rightImpactPoint = CollisionHelper.SolidTileRayCast(_rightStart, rightVelocity, 2400);



        Vector2 RandLaserPoint()
        {
            var rand1 = Main.rand.NextFloat(0f, 1f);
            var rand2 = Main.rand.NextFloat(0f, 1f);
            Vector2 top1 = Vector2.Lerp(_leftStart, _leftImpactPoint, rand1);
            Vector2 bottom1 = Vector2.Lerp(_rightStart, _rightImpactPoint, rand2);

            var rand3 = Main.rand.NextFloat(0f, 1f);
            var rand4 = Main.rand.NextFloat(0f, 1f);

            Vector2 pos = new();
            pos.X = MathHelper.Lerp(top1.X, bottom1.X, rand3);
            pos.Y = MathHelper.Lerp(top1.Y, bottom1.Y, rand4);

            return pos;
        }

        void MakeIceSpikeParticles()
        {
            for (var j = 0; j < 1; j++)
            {
                {
                    var ratio = Main.rand.NextFloat(0.2f, 0.8f);
                    var pos2 = Vector2.Lerp(_leftImpactPoint, _rightImpactPoint, ratio);
                    var vel2 = Vector2.Lerp(leftVelocity, rightVelocity, ratio);

                    Particles.IceSpike.Spawn(new()
                    {
                        position = pos2 + vel2 * 46,
                        rotation = Main.rand.NextFloat(-0.3f, 0.3f),
                        timeLeft = 60,
                        timediv = 60,
                    });
                }
            }
        }
        void MakeImpactParticles()
        {
            for (var i = 0; i < 3; i++)
            {
                var ratio = Main.rand.NextFloat(0.2f, 0.8f);
                var pos = Vector2.Lerp(_leftImpactPoint, _rightImpactPoint, ratio);
                var vel = Vector2.Lerp(leftVelocity, rightVelocity, ratio);

                for (var j = 0; j < 1; j++)
                {
                    {
                        ratio = Main.rand.NextFloat(0.2f, 0.8f);
                        var pos2 = Vector2.Lerp(_leftImpactPoint, _rightImpactPoint, ratio);
                        var vel2 = Vector2.Lerp(leftVelocity, rightVelocity, ratio);

                        Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                        {
                            position = pos2 + vel2 * 46,
                            velocity = -Vector2.UnitY * 7,
                            innerColor = Color.White.ToVector4(),
                            outerColor = Color.Cyan.ToVector4(),
                            timeLeft = 30,
                            scale = new Vector2(1f)
                        });
                    }
                }

                pos -= vel * 128;

                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = pos,
                    velocity = vel * 25 * Main.rand.NextFloat(0.85f, 1f),
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Cyan.ToVector4(),
                    timeLeft = 60,
                    scale = new Vector2(Main.rand.NextFloat(0.4f, 0.7f))
                });


                if (Main.rand.NextBool(2))
                {
                    pos += vel * 212;
                    Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                    {
                        position = pos,
                        velocity = -vel * 12 * Main.rand.NextFloat(0.85f, 1f),
                        innerColor = Color.White.ToVector4(),
                        outerColor = Color.Cyan.ToVector4(),
                        timeLeft = 60,
                        scale = new Vector2(Main.rand.NextFloat(0.6F, 1f))
                    });
                }
                if (Main.rand.NextBool(2))
                {

                    Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                    {
                        position = pos,
                        velocity = -vel * 12 * Main.rand.NextFloat(0.85f, 1f),
                        innerColor = Color.White.ToVector4(),
                        outerColor = Color.Cyan.ToVector4(),
                        timeLeft = 60,
                        scale = new Vector2(Main.rand.NextFloat(0.6F, 1f))
                    });
                }
            }
        }

        if (Main.rand.NextBool(15))
            MakeIceSpikeParticles();



        MakeImpactParticles();
        if(this.OwnedByLocalClient() && Timer % 60 == 0)
        {
            var firer = ProjFirer.From<NiiviFrostSpike>(Projectile);
            firer.position = Vector2.Lerp(_leftImpactPoint, _rightImpactPoint, 0.5f);
            firer.velocity = -Vector2.UnitY;
            firer.New();
        }
        _leftImpactPoint += leftVelocity * 196;
        _rightImpactPoint += rightVelocity * 196;
        {
            for (var i = 0; i < 4; i++)
            {

                Particles.FaintSmokeDust.Spawn(new()
                {
                    position = RandLaserPoint(),
                    velocity = Vector2.Zero,
                    timeleft = 45,
                    color = Color.SkyBlue * 0.14f
                });
            }
            for (var i = 0; i < 4; i++)
            {

                Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = RandLaserPoint(),
                    timeLeft = 40,
                    velocity = Vector2.Zero,
                    innerColor = Color.Cyan.ToVector4(),
                    outerColor = Color.DarkBlue.ToVector4(),
                    scale = new Vector2(0.5F)
                    
                });
            }
            Dust.NewDustPerfect(RandLaserPoint(), DustID.GemDiamond);
        }

        if (!Parent.active)
            Projectile.Kill();
    }

    Color GetTrailColor(float ratio)
    {
        var grad = DrawUtilities.InterpolateColorArray(ratio, Color.White, Color.SkyBlue, Color.Cyan, Color.White);
        grad = Color.Lerp(grad, Color.White, 0.3F);
        grad *= MathHelper.Lerp(1f, 0F, ratio);
        return grad;
    }

    void DrawPixelatedFrostBreath(GraphicsDevice gDevice)
    {
        var pass = AssetReferences.Effects.Niivi.NiiviFrostBreath.CreatePrimitivesPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
        pass.Parameters.spriteSize = AssetReferences.Assets.LaserTextures.TexturedLaser.Asset.Value.Size();
        pass.Parameters.noiseTexelSize = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset.Value.GetTexelSize();
        pass.Apply();
        gDevice.Register(0, AssetReferences.Assets.LaserTextures.TexturedLaser.Asset.Value, SamplerState.LinearWrap);
        gDevice.Register(1, AssetReferences.Assets.Noise.TornadoNoise.Asset.Value, SamplerState.LinearWrap);
        gDevice.Register(2, AssetReferences.Assets.NoiseTextures.IceWaterCaustics.Asset.Value, SamplerState.LinearWrap);
        gDevice.Register(3, AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset.Value, SamplerState.LinearWrap);
       
        var vertices = DrawUtilities.FillVertexArray(128, GetTrailColor, _leftStart, _rightStart, _leftImpactPoint, _rightImpactPoint);
        var indices = DrawUtilities.PrepareIndicesForDrawing(128);
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(vertices, indices, pass.Shader);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedFrostBreath);
        return false;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
