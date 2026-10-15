using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.NPCsUG;

public class BlastingBombBoom : ModProjectile
{
    float ExplodeTime => 30;
    public override string Texture => AssetReferences.Assets.Textures.Projectile.SkullBoom.KEY;
    ref float Timer => ref Projectile.ai[0];
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.WidthAndHeight = 128;
        Projectile.hostile = true;
        Projectile.timeLeft = 30;
        Projectile.ignoreWater = true;
        Projectile.light = 0.8f;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            FXUtil.ShakeCamera(Projectile.Center, 1024, 16);
            
            var sound = AssetReferences.Assets.Sounds.HeavyExplosion1.Asset with { PitchVariance = 0.6f };
            SoundEngine.PlaySound(sound, Projectile.position);

            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Fiery, new Vector2(5, 15f));
            ParticleUtils.CreateSwirlingDustCircle(fac);

            foreach(PositionVelocity posVel in new CircleOrientation(Projectile.Center, 64, 32))
            {
                Particles.GlowDust.Spawn(new()
                {
                    position = posVel.position,
                    velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f),
                    color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat()),
                    timeLeft = Main.rand.Next(45, 120),
                    scale = Main.rand.NextFloat(0.15f, 0.25f)
                });
            }

            var color = Color.Lerp(Color.OrangeRed, Color.Black, 0.75f);
            foreach (PositionVelocity posVel in new CircleOrientation(Projectile.Center, 64, 32))
            {
                Particles.BackCloud.Spawn(new()
                {
                    position = posVel.position,
                    velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f),
                    color = color,
                    timeLeft = Main.rand.Next(60, 120),
                    scale = Main.rand.NextFloat(0.5f, 1f)
                });
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        var drawer = Projectile.Drawer;
        drawer.scale = Vector2.Lerp(new Vector2(0.2f), Vector2.One, EasingFunction.OutCirc(Timer / ExplodeTime));
        drawer.color = Color.DarkRed;
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class BlastingBomb : ModNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Idle,
        Ticking,
        Boom
    }
    const string ANIM_IDLE = "Idle";
    const string ANIM_BEEP = "Beep";
    Outliner _outliner;
    Color _aboutToBlowColor = Color.Transparent;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    ref float BeepTime => ref NPC.ai[2];
    float ExplosionDist => 196 * 196;
    int Damage_Boom => 30;
    float BlowTime => 60;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToUnderground();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _aboutToBlowColor = Color.Transparent;
        NPC.damage = 10;
        NPC.width = 34;
        NPC.height = 36;
        NPC.lifeMax = 25;
        NPC.defense = 3;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.65f;
        NPC.noGravity = false;
    }

    public override void AI()
    {
        base.AI();
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Ticking:
                AI_Ticking();
                break;
            case AIState.Boom:
                AI_Boom();
                break;
        }
        if (BeepTime > 0)
        {
            NPC.PlayAnimationLooped(ANIM_BEEP);
            BeepTime--;
        }
        else
        {
            NPC.PlayAnimationLooped(ANIM_IDLE);
        }
        _outliner.Update();
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }

    void HandleRotation()
    {
        if (MoonUtils.IsGrounded(NPC))
        {
            NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.01f);
        }
        else
        {
            NPC.rotation += NPC.velocity.X * 0.05f;
        }
    }
    
    void PlayBeepSound()
    {
        var beep = AssetReferences.Assets.Sounds.ITBeep.Asset with { PitchVariance = 0.1f, Volume = 0.2f };
        SoundEngine.PlaySound(beep, NPC.position);
    }

    void AI_Idle()
    {
        Timer++;
        NPC.TargetClosest();

        HandleRotation();
        NPC.velocity.X *= 0.96f;
        if (Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center) <= ExplosionDist)
        {
            SwitchState(AIState.Ticking);
        }
    }

    void AI_Ticking()
    {
        _outliner.warning = true;
        Timer++;
        if (Timer % 15 == 0)
        {
            PlayBeepSound();
            BeepTime = 7;
        }
        _aboutToBlowColor = Color.Lerp(Color.Transparent, Color.Yellow, EasingFunction.InOutSine(Timer / BlowTime));
        NPC.velocity.X *= 0.96f;
        if (Timer >= BlowTime)
        {
            SwitchState(AIState.Boom);
        }
    }

    void AI_Boom()
    {
        Timer++;
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                var firer = ProjFirer.From<BlastingBombBoom>(NPC);
                firer.damage = Damage_Boom;
                firer.New();
            }
            NPC.Kill();
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (State == AIState.Idle)
        {
            SwitchState(AIState.Ticking);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        if(_aboutToBlowColor.A > 3)
        {
            var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.JumbledGlowCircle.Asset, NPC.Center);
            drawer.color = _aboutToBlowColor;
            drawer.color.A = 0;
            drawer.scale = Vector2.Lerp(new Vector2(0.2f), new Vector2(0.6f), ((float)_aboutToBlowColor.A) / 255f);
            drawer.rotation = Main.GlobalTimeWrappedHourly;
            spriteBatch.Draw(drawer);
        }
        return false;
    }

    public void DrawToRenderTargets()
    {
        if (!_outliner.VisibleEnough)
            return;
        void DrawOutline(SpriteBatch spriteBatch)
        {
            NPC.DrawAnimator(spriteBatch, _outliner.outlineColor);
        }
        OutlineRenderer.Queue(DrawOutline);
        //throw new NotImplementedException();
    }
}
