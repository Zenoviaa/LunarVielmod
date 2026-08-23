using ReLogic.Content;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;

public class MoonBlade : ModProjectile
{
    private bool _lodged;
    private float _randScale;
    private float _rotOffset;
    private Vector2 _pullOffset;
    private Vector2 _startPullOffset;
    private Vector2 _scale;
    private Vector2 _outScale;
    private Vector2 _initialVelocity;
    private Asset<Texture2D> _outlineTextureAsset;
    private ref float Timer => ref Projectile.ai[0];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_pullOffset);
        writer.WriteVector2(_startPullOffset);
        writer.WriteVector2(_initialVelocity);
        writer.WriteVector2(_scale);
        writer.Write(_lodged);
        writer.Write(_randScale);
        writer.Write(_rotOffset);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _pullOffset = reader.ReadVector2();
        _startPullOffset = reader.ReadVector2();
        _initialVelocity = reader.ReadVector2();
        _scale = reader.ReadVector2();
        _lodged = reader.ReadBoolean();
        _randScale = reader.ReadSingle();
        _rotOffset = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.timeLeft = 600;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (!_lodged)
        {
            float numDust = 8;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 vel = -oldVelocity;
                vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
                vel = vel.SafeNormalize(Vector2.Zero);
                vel *= Main.rand.NextFloat(6, 12f);
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.Blue;
                var dp = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                dp.fast = true;
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
            }
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
            _lodged = true;
        }

        return true;
    }
    public override void AI()
    {
        base.AI();
        _outScale = Vector2.Lerp(Vector2.Zero, Vector2.One, EasingFunction.InOutSine(Projectile.timeLeft / 30f));
        if (_lodged)
        {
            Projectile.extraUpdates = 0;
            Projectile.velocity *= 0f;
            return;
        }
        Timer++;
        if (Timer == 1)
        {
            SoundStyle inSound = new SoundStyle($"Stellamod/Assets/Sounds/SoftSummon2");
            inSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(inSound, Projectile.position);
            if (this.OwnedByLocalClient())
            {
                float dist = Main.rand.NextFloat(140, 400);
                float dir = Main.rand.NextBool(2) ? -1 : 1;
                _startPullOffset = Vector2.UnitX * dist * dir;//Vector2.Lerp(-Vector2.UnitX * 128, Vector2.UnitX * 128, Main.rand.NextFloat(0f,
                _randScale = Main.rand.NextFloat(0.66f, 1.5f);
                Projectile.netUpdate = true;
            }
            _initialVelocity = Projectile.velocity;
            //MoonSpiralParticle.Spawn(Projectile.Center, Vector2.Zero);
        }

        if (Timer < 70f)
        {
            _pullOffset = Vector2.Lerp(_startPullOffset, Vector2.Zero, EasingFunction.InOutSine(Timer / 70f));
            _rotOffset = MathHelper.Lerp(MathHelper.TwoPi * 2, 0, EasingFunction.OutExpo(Timer / 70f));
            if (Projectile.velocity.Length() > 0.2f)
                Projectile.velocity *= 0.2f;
        }
        else if (Timer == 71)
        {
            Projectile.velocity = _initialVelocity * 0.5f;
            SoundStyle outSound = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower1");
            outSound.PitchVariance = 0.3f;
            outSound.Volume = 0.3f;
            SoundEngine.PlaySound(outSound, Projectile.position);
        }
        else
        {
            if (Projectile.velocity.Length() < _initialVelocity.Length())
            {
                Projectile.velocity *= 1.1f;
                if (Projectile.velocity.Length() >= _initialVelocity.Length())
                {
                    LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity);
                }
            }
            else
            {
                if (Timer % 24 == 0)
                {
                    var sp = FaintSmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(2, 2));
                    sp.Scale *= Main.rand.NextFloat(0.125f, 0.25f);
                    sp.behindLayer = true;
                    sp.noShrink = true;
                    sp.fadeToColor = Color.Black;
                    sp.color = Color.Lerp(Color.Blue, Color.Black, 0.75f);

                }
                Projectile.extraUpdates = 2;
            }
        }
        float ratio = Timer / 60f;
        float ease = EasingFunction.OutExpo(ratio);
        _scale = Vector2.Lerp(Vector2.Zero, new Vector2(1f, 0.46f), ease);
        Projectile.rotation = Projectile.velocity.ToRotation() + _rotOffset;
    }
    private void DrawPixelatedSwords(SpriteBatch sb, Vector2 screenPos)
    {
        _outlineTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Outline");
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.color = Color.Lerp(Color.Blue, Color.DarkBlue, ExtraMath.Osc(0f, 1f, 0, Projectile.whoAmI));
        sbDrawer.color.A = 0;
        sbDrawer.scale *= _scale * _outScale * _randScale;
        sbDrawer.worldPosition += _pullOffset;
        Main.spriteBatch.Draw(sbDrawer);
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            sbDrawer.worldPosition = pos;
            float ratio = i / (float)Projectile.oldPos.Length;
            sbDrawer.color = Color.Lerp(Color.Blue, Color.DarkBlue, ratio);
            sbDrawer.color *= MathHelper.SmoothStep(1f, 0f, EasingFunction.OutExpo(ratio));
            sbDrawer.color.A = 0;
            //   sbDrawer.scale *= _scale;
            sbDrawer.worldPosition += _pullOffset;
            Main.spriteBatch.Draw(sbDrawer);
        }
        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.color = Color.White * ExtraMath.Osc(0.35f, 0.6f, speed: 6, Projectile.whoAmI);
        sbDrawer.color.A = 0;
        sbDrawer.scale *= _scale * _outScale * _randScale;
        sbDrawer.scale *= 0.9f;
        sbDrawer.worldPosition += _pullOffset;
        Main.spriteBatch.Draw(sbDrawer);

        sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        sbDrawer.scale *= _scale * _outScale * _randScale;
        sbDrawer.color = Color.White * ExtraMath.Osc(0f, 1f, speed: 12, offset: Projectile.whoAmI);
        sbDrawer.color.A = 0;
        sbDrawer.texture = _outlineTextureAsset.Value;
        sbDrawer.worldPosition += _pullOffset;
        Main.spriteBatch.Draw(sbDrawer);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedSwords);

        //Main.spriteBatch.Draw(sbDrawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
