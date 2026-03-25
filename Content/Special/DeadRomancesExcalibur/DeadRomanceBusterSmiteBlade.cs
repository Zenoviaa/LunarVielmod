using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Special.DeadRomancesExcalibur;

public class DeadRomanceBusterSmiteBlade : ModProjectile
{
    private float _scale;
    private float _lineRot;
    private float _lineRotLerp;
    private Vector2 _targetCenter;
    private int Target
    {
        get => (int)Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }
    private ref float Timer => ref Projectile.ai[1];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_targetCenter);
    }
    
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _targetCenter = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 1;
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.ignoreWater = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.timeLeft = 180;
        Projectile.extraUpdates = 2;
        Projectile.light = 2f;
    }

    public override void AI()
    {
        base.AI();
        if(_scale == 0f)
        {
            _scale = Projectile.scale = Main.rand.NextFloat(0.5f, 1f);
        }

        if (Target != -1)
        {
            NPC targetNPC = Main.npc[Target];
            if (targetNPC.active)
            {
                _targetCenter = targetNPC.Center;
            }
            else
            {
                Target = -1;
            }
        }

        int denom = 16 * (Projectile.extraUpdates+1);
        if (Timer % denom == 0)
        {
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            SirestiasSparkleParticle sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero);
            sp.gravity = 0;
            sp.noTileCollide = true;
            sp.Scale *= 0.25f * Projectile.scale;
            sp.fast = true;
            sp.outerColor = Color.Yellow;
        }
        denom = 8 * (Projectile.extraUpdates + 1);
        if (Timer % denom == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                SirestiasSparkleParticle sp = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero);
                sp.gravity = 0;
                sp.noTileCollide = true;
                sp.Scale *= 0.1f * Projectile.scale;
                sp.fast = true;
                sp.outerColor = Color.Yellow;
            }
        }

        Projectile.velocity = Projectile.velocity.RotatedBy(0.05f);
        Projectile.scale *= 0.98f;
        SmokeParticles();
    }

    private void SmokeParticles()
    {
        Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
        SirestiasSmokeParticle sp = SirestiasSmokeParticle.SpawnInAlphaLayer(spawnPos, Vector2.Zero);
        sp.color = Color.Lerp(Color.Lerp(Color.Black, Color.Blue, 0.15f), Color.Black, Main.rand.NextFloat(0f, 1f));
        sp.gravity = 0;
        sp.noTileCollide = true;
        sp.Scale *= 0.4f * Projectile.scale;
        sp.offsetRot = Main.rand.NextFloat(0f, MathHelper.TwoPi);

        spawnPos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
        Vector2 spawnPos2 = Projectile.Center + Main.rand.NextVector2Circular(32, 32); ;
        Vector2 spawnVelocity = spawnPos2 - spawnPos;
        spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
        spawnVelocity *= 24;

        int denom = 2 * (Projectile.extraUpdates + 1);
        if (Main.rand.NextBool(denom))
        {
            Color color = new Color(41, 43, 66);
            var sp2 = SirestiasSmokeParticle2.SpawnInAlphaLayer(spawnPos + Main.rand.NextVector2Circular(32, 32), spawnVelocity * 0.02f);
            sp2.color = Color.Lerp(color, Color.White, 0.25f);
            sp2.gravity = 0;
            sp2.noTileCollide = true;
            sp2.Scale *= 0.66f * Projectile.scale;
            sp2.stretchScale2 = new Vector2(1f, 0.5f);
            sp2.offsetRot = 0;
            sp2.noRot = true;
        }

    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        base.ModifyHitNPC(target, ref modifiers);
        
    }

    private void DrawPixelatedBlade(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);       
        for(int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i];
            Vector2 worldPos = pos + Projectile.Size * 0.5f;
            drawer.worldPosition = worldPos;
            drawer.rotation = Projectile.oldRot[i];
            float ratio = (float)i / (float)Projectile.oldPos.Length;
            float ease = EasingFunction.InOutSine(ratio);
            Color bladeColor = Color.Lerp(Color.Goldenrod, Color.Black, ease);
            bladeColor.A = 0;
            drawer.color = bladeColor;
            spriteBatch.Draw(drawer);
        }

        drawer.color = Color.LightGoldenrodYellow;
        drawer.color.A = 0;

        SpritebatchDrawer bloomLineDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.ShootingStarTrail, Projectile.Center);
        bloomLineDrawer.rotation = _lineRot;
        bloomLineDrawer.scale = new Vector2(16, 1);
        bloomLineDrawer.color = Color.Goldenrod;
        bloomLineDrawer.color *= MathHelper.Lerp(0f, 1f, _lineRotLerp);
        bloomLineDrawer.color.A = 0;
        bloomLineDrawer.RightCenterOrigin();
        bloomLineDrawer.worldPosition += _lineRot.ToRotationVector2() * 128 * MathHelper.Lerp(1f, 0f, _lineRotLerp);
        spriteBatch.Draw(bloomLineDrawer);
        spriteBatch.Draw(drawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedBlade);
        return false;
    }
}
