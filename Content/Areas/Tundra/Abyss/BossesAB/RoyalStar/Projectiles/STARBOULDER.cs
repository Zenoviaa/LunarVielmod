using Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Gores;
using Stellamod.Content.Rendering.GenericEffects;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar.Projectiles;

public class STARBOULDER : ModProjectile, IDrawToRenderTarget
{
    private float _timer;
    private float _spawnTimer;
    public bool _isFlying;
    private int Style
    {
        get => (int)Projectile.ai[0];
    }
    private ref float Index => ref Projectile.ai[1];
    private ref float Kick => ref Projectile.ai[2];
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_isFlying);
        writer.Write(_timer);
        writer.Write(_spawnTimer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _isFlying = reader.ReadBoolean();
        _timer = reader.ReadSingle();
        _spawnTimer = reader.ReadSingle();
    }


    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);
        _spawnTimer -= Index * 70;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 24;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = true;
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.hostile = false;
        Projectile.timeLeft = 1500;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override bool ShouldUpdatePosition()
    {
        return _isFlying;
    }

    public override void AI()
    {
        base.AI();
        _spawnTimer++;
        Projectile.frame = Style;
        Projectile.hostile = _isFlying;
        if (Kick >= 1)
        {
            if (!_isFlying)
            {
                if (this.OwnedByLocalClient())
                {
                    ProjFirer firer = ProjFirer.From<STARFLASH>(Projectile);
                    firer.New();
                }
            }
 
            _isFlying = true;
            Projectile.netUpdate = true;
        }

        switch (Style)
        {
            case 0:
                Projectile.rotation += MathF.Sign(Projectile.velocity.X);
                break;
        }


        if (!_isFlying)
            return;
        switch (Style)
        {
            case 0:
                Projectile.tileCollide = false;
                AI_MachSpeed();
                break;
            case 1:
                AI_Shatter();
                break;
            case 2:
                AI_Bouncy();
                break;
        }
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if(Style == 2 && _isFlying)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            SoundStyle bellHitSound = Main.rand.NextBool(2) ? AssetRegistry.Sounds.Bishinine.BellHit1 : AssetRegistry.Sounds.Bishinine.BellHit2;
            bellHitSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(bellHitSound, Projectile.position);

            SoundStyle sound = AssetRegistry.Sounds.Bishinine.BigBellGroundhit with { PitchVariance = 0.6f };
            SoundEngine.PlaySound(sound, Projectile.position);



            var p3 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.UnitY);
            FXUtil.ShakeCamera(Projectile.position, 1024, 24);
        }
        return false;
    }

    private void PlayGongSound()
    {
        SoundStyle sound = AssetRegistry.Sounds.Bishinine.BigBellGroundhit;
        SoundEngine.PlaySound(sound, Projectile.position);
    }

    private void PlaySlideSound()
    {
        var soundStyle = AssetReferences.Assets.Sounds.STARR.RockSlide.Asset with { PitchVariance = 0.5f };
        SoundEngine.PlaySound(soundStyle, Projectile.position);
    }

    private void PlayBreakSound()
    {
        var soundStyle = AssetReferences.Assets.Sounds.STARR.RockSmash.Asset with { PitchVariance = 0.5f };
        SoundEngine.PlaySound(soundStyle, Projectile.position);
    }

    private void AI_MachSpeed()
    {
        _timer++;
        if (_timer == 1)
        {
            PlayGongSound();
        }
        if (_timer == 4)
        {
            Projectile.velocity *= 0.1f;
            PlaySlideSound();
        }
        Projectile.rotation += Projectile.velocity.X * 0.05f;
        Projectile.velocity *= 1.03f;
        Projectile.extraUpdates = 1;
        if (_timer % 9 == 0)
        {
            var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity.SafeNormalize(Vector2.Zero) * 3);
            p2.Scale *= 0.5f;
            p2.innerColor = Color.Gold;
            p2.outerColor = Color.DarkGoldenrod;
            p2.fadeToColor = Color.DarkOrange;
        }
        if (_timer >= 120)
        {
            Projectile.Kill();
        }
    }

    private void AI_Shatter()
    {
        _timer++;
        if(_timer == 1)
        {
            PlayGongSound();
        }
        if (_timer == 4)
        {
            PlaySlideSound();
            if (this.OwnedByLocalClient())
            {
                float numShatters = 6;
                float minRadians = MathHelper.ToRadians(-30);
                float maxRadians = MathHelper.ToRadians(30);
                for(float f = 0; f < numShatters; f++)
                {
                    float ratio = f / numShatters;
                    float radians = MathHelper.Lerp(minRadians, maxRadians, ratio);
                    Vector2 vel = Projectile.velocity.RotatedBy(radians);
                    vel *= Main.rand.NextFloat(0.5f, 1f);
                    ProjFirer firer = ProjFirer.From<STAREGG>(Projectile);
                    firer.velocity = vel;
                    firer.velocity.Y -= 12;
                    firer.New();
                }
            }
            Projectile.Kill();
        }
    }

    private void AI_Bouncy()
    {
        _timer++;
        if(_timer == 1)
        {
            PlayGongSound();
            Projectile.velocity.Y = -14;
        }
        if (Projectile.velocity.Y < 20)
            Projectile.velocity.Y += 1;
        if (MathF.Abs(Projectile.velocity.X) > 8)
            Projectile.velocity.X *= 0.96f;
        Projectile.rotation += Projectile.velocity.X * 0.05f;
        if (_timer == 4)
        {
            PlaySlideSound();
        }
        if (_timer >= 190)
        {
            Projectile.Kill();
        }
    }
    private float GetSpiralDashTrailWidth(float completionRatio)
    {
        return MathHelper.SmoothStep(128, 96, completionRatio) * 0.16f;
    }
    private float GetSpiralDashTrailWidth2(float completionRatio)
    {
        return GetSpiralDashTrailWidth(completionRatio) * 1.3f;
    }
    private Color GetSpiralDashTrailColor(float completionRatio)
    {
        return Color.Lerp(Color.DarkOrange, Color.Transparent, completionRatio);
    }
    private Vector2 Offset
    {
        get
        {
            Vector2 maxOffset = -Vector2.UnitY * (800);
            Vector2 offset = Vector2.Lerp(maxOffset, Vector2.Zero, EasingFunction.InExpo(_spawnTimer / 100));
            return offset;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (_isFlying)
        {
            SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            foreach (OldPosition oldPos in Projectile.IterateOldPosBackwards())
            {
                afDrawer.worldPosition = oldPos.position + Projectile.Size * 0.5f;
                afDrawer.color = Color.Lerp(Color.White, Color.Transparent, oldPos.progress) * 0.12f;
                afDrawer.color.A = 0;
                Main.spriteBatch.Draw(afDrawer);
            }
        }

        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);

        drawer.worldPosition += Offset;
        drawer.scale *= EasingFunction.InSine(_spawnTimer / 60f);
        Main.spriteBatch.Draw(drawer);


        OutlineRenderer.Queue(DrawWhites);
        return false;
    }

    private void DrawWhites(SpriteBatch spriteSB)
    {
        Color color = Projectile.hostile ? Color.Red : Color.Yellow;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        Vector2 maxOffset = -Vector2.UnitY * (800);
        Vector2 offset = Vector2.Lerp(maxOffset, Vector2.Zero, EasingFunction.InExpo(_spawnTimer / 140));
        drawer.worldPosition += Offset;
        drawer.scale *= EasingFunction.InSine(_spawnTimer / 60f);
        drawer.color = color;
        Main.spriteBatch.Draw(drawer);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (Main.netMode == NetmodeID.Server)
            return;
        PlayBreakSound();
        FXUtil.ShakeCamera(Projectile.position, 128, 4);
        for(int i = 0; i < 4; i++)
        {
            Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
            Gore.NewGore(Projectile.GetSource_FromThis(), position, velocity, ModContent.GoreType<StarGongGore>());
        }
        
        for (int f = 0; f < 2; f++)
        {
            Vector2 spawnPosition = Projectile.Center;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }

        for(int i = 0; i < 16; i++)
        {
            Vector2 spawnPosition = Projectile.Center;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Main.rand.NextVector2Circular(8, 8);
            Dust.NewDustPerfect(spawnPosition, DustID.Stone, spawnVelocity);
        }
    }

    public void DrawToRenderTargets()
    {
        //Cool wind trail thing
        if (_isFlying && Style == 0)
        {
            var verts1 = DrawUtilities.PrepareSimpleTrailing(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth, Projectile.Size * 0.5f);
            var verts2 = DrawUtilities.PrepareSimpleTrailing(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth2, Projectile.Size * 0.5f);
            ModContent.GetInstance<SpiralingWindTrailRenderer>().PrepareForBigRendering(verts2);
            ModContent.GetInstance<SpiralingWindTrailRenderer>().PrepareForRendering(verts1);
        }
    }
}
