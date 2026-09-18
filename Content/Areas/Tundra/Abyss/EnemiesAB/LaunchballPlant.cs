using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.NPCHelpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;

public class LaunchballBoom : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 5;

    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 128;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 30;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            for (int i = 0; i < 24; i++)
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = Projectile.Center,
                    timeLeft = Main.rand.Next(60, 120),
                    velocity = Main.rand.NextVector2Circular(15, 15),
                    innerColor = Color.LightCyan.ToVector4(),
                    outerColor = Color.DarkBlue.ToVector4(),
                    scale = Vector2.One * Main.rand.NextFloat(1f, 1.5f)
                });
            }
            for (int i = 0; i < 8; i++)
            {
                var sp = SmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(3, 3), Color.DarkGray);
                sp.initialColor = Color.White;
                sp.fadeToColor = Color.Black;
                sp.dampening = 0.05f;
                sp.Scale *= 1.5f;
                sp.behindLayer = true;
            }
            for (int i = 0; i < 3; i++)
            {
                var fx = FXUtil.GlowStretch(Projectile.Center, Main.rand.NextVector2Circular(32, 32));
            }
            var sound = AssetReferences.Assets.Sounds.Crysalizer5.Asset with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(sound, Projectile.position);
            FXUtil.ShakeCamera(Projectile.Center, 256, 8);
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White * 0.15f, Color.SkyBlue * 0.15f, Color.DarkBlue * 0.15f, duration: 25, baseSize: 0.2f);
        }
        Projectile.frameCounter++;
        if (Projectile.frameCounter >= 4 && Projectile.frame < Main.projFrames[Type])
        {
            Projectile.frame++;
            Projectile.frameCounter = 0;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer projDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        //Main.spriteBatch.Draw(projDrawer);
        projDrawer.color = Color.White * 0.95f;
        projDrawer.color.A = 0;
        Main.spriteBatch.Draw(projDrawer);
        return false;
        //    return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}


public class Launchball : ModProjectile,
    IWaterSilhouette
{
    private ref float Timer => ref Projectile.ai[0];
    private ref float BounceCount => ref Projectile.ai[1];
    private ref float SquishTimer => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 18;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 240;
    }
    public override void AI()
    {
        base.AI();
        if (BounceCount >= 3)
        {
            Timer++;
            Projectile.velocity *= 0.96f;
            if (Timer >= 60)
            {

                Projectile.Kill();
            }
            if (Timer >= 30)
                Projectile.scale *= 1.01f;
        }

        if (Main.rand.NextBool(32))
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = Projectile.Center,
                timeLeft = Main.rand.Next(60, 120),
                velocity = Main.rand.NextVector2Circular(15, 15),
                innerColor = Color.LightCyan.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                scale = Vector2.One * 0.4f
            });
        }

        SquishTimer++;
        Projectile.velocity.X *= 0.98f;
        Projectile.velocity.Y += 0.5f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.velocity.X != oldVelocity.X)
        {
            Projectile.velocity.X = -oldVelocity.X;
            BounceCount++;
            ImpactFX();
        }

        if (Projectile.velocity.Y != oldVelocity.Y)
        {
            Projectile.velocity.Y = -oldVelocity.Y;
            ImpactFX();
            BounceCount++;
        }

        return false;
    }

    private void ImpactFX()
    {
        for (int i = 0; i < 4; i++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = Projectile.Center,
                timeLeft = Main.rand.Next(60, 120),
                velocity = Main.rand.NextVector2Circular(15, 15),
                innerColor = Color.LightCyan.ToVector4(),
                outerColor = Color.DarkBlue.ToVector4(),
                scale = Vector2.One * Main.rand.NextFloat(1f, 1.5f)
            });
        }
        SquishTimer = 0;
        var hitSound = AssetReferences.Assets.Sounds.Abyss.Smallbounce.Asset with { PitchVariance = 0.3f };
        SoundEngine.PlaySound(hitSound, Projectile.position);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer projDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        float ease = EasingFunction.OutExpo(SquishTimer / 60f);
        Vector2 v = Vector2.Lerp(new Vector2(1.5f, 1f), new Vector2(0.8f, 1.5f), ease);
        Vector2 v2 = Vector2.Lerp(new Vector2(0.8f, 1.5f), Vector2.One, ease);
        Vector2 v3 = Vector2.Lerp(v, v2, SquishTimer / 60f);
        projDrawer.scale *= v3;

        foreach (OldPosition oldPos in Projectile.IterateOldPosBackwards())
        {
            projDrawer.worldPosition = oldPos.position + Projectile.Size * 0.5f;
            projDrawer.color = Color.Lerp(Color.SkyBlue, Color.Transparent, oldPos.progress) * 0.1f;
            projDrawer.color.A = 0;
            Main.spriteBatch.Draw(projDrawer);
        }
        projDrawer.color = lightColor;
        projDrawer.worldPosition = Projectile.Center;
        Main.spriteBatch.Draw(projDrawer);

        projDrawer.color *= 0.1f;
        projDrawer.color.A = 0;
        Main.spriteBatch.Draw(projDrawer);
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Projectile.Center);
        glowDrawer.color = Color.White * 0.05f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.4f;
        Main.spriteBatch.Draw(glowDrawer);
        OutlineRenderer.Queue(DrawOutlineWhite);
        if (Timer > 0)
        {
            projDrawer.color = Color.Lerp(Color.Transparent, Color.Red, ExtraMath.Osc(0f, 0.5f, speed: 48));
            Main.spriteBatch.Draw(projDrawer);

        }
        return false;
        //    return base.PreDraw(ref lightColor);
    }
    private void DrawOutlineWhite(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer projDrawer = SpritebatchDrawer.FromProjectile(Projectile);
        float ease = EasingFunction.OutExpo(SquishTimer / 60f);
        Vector2 v = Vector2.Lerp(new Vector2(1.5f, 1f), new Vector2(0.8f, 1.5f), ease);
        Vector2 v2 = Vector2.Lerp(new Vector2(0.8f, 1.5f), Vector2.One, ease);
        Vector2 v3 = Vector2.Lerp(v, v2, SquishTimer / 60f);
        projDrawer.scale *= v3;
        projDrawer.color = Color.Red;
        Main.spriteBatch.Draw(projDrawer);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            ProjFirer firer = ProjFirer.From<LaunchballBoom>(Projectile);
            firer.New();
        }
    }

    public void PrepareSilhouetteDrawing(RekSilhouetteSystem system)
    {
        void DrawSilhouette(SpriteBatch spriteBatch)
        {
            SpritebatchDrawer projDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            float ease = EasingFunction.OutExpo(SquishTimer / 60f);
            Vector2 v = Vector2.Lerp(new Vector2(1.5f, 1f), new Vector2(0.8f, 1.5f), ease);
            Vector2 v2 = Vector2.Lerp(new Vector2(0.8f, 1.5f), Vector2.One, ease);
            Vector2 v3 = Vector2.Lerp(v, v2, SquishTimer / 60f);
            projDrawer.scale *= v3;
            projDrawer.color = Color.DarkBlue;
            projDrawer.worldPosition = Projectile.Center;
            Main.spriteBatch.Draw(projDrawer);
        }
        system.SilhouettesToDraw.Add(DrawSilhouette);
    }
}

public class LaunchballPlant : ModNPC,
    IWaterSilhouette
{
    private enum AIState
    {
        Idle,
        Cower,
        Shoot,
        Naked
    }
    private Outliner _outliner;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private Player MyTarget => Main.player[NPC.target];
    public override string Texture => TextureRegistry.EmptyTexture;
    private const string ANIM_IDLE = "Idle";
    private const string ANIM_HIDE = "Hide";
    private const string ANIM_SHOOT = "Shoot";
    private const string ANIM_NAKED = "Naked";
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 1;
        this.AddToAbyss();
        this.PreferLand();
        NPCSets.Heavy[Type] = true;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 32;
        NPC.height = 64;
        NPC.damage = 1;
        NPC.defense = 8;
        NPC.lifeMax = 90;
        NPC.knockBackResist = 0;
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
            case AIState.Cower:
                AI_Cower();
                break;
            case AIState.Shoot:
                AI_Shoot();
                break;
            case AIState.Naked:
                AI_Naked();
                break;
        }
        _outliner.Update();
        NPC.velocity.X *= 0.5f;
        NPC.rotation = ExtraMath.Osc(-0.02f, 0.02f, speed: 6, NPC.whoAmI);
    }

    private void AI_Idle()
    {
        Timer++;
        NPC.TargetClosest();
        if(NPC.HasValidTarget && Vector2.Distance(MyTarget.Center, NPC.Center) < 196)
        {
            SwitchState(AIState.Cower);
        }
        this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
    }

    private void AI_Cower()
    {
        _outliner.warning = true;
        Timer++;
        if(Timer == 1)
        {
            var jumpSound = AssetReferences.Assets.Sounds.LilJump.Asset with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(jumpSound, NPC.position);
        }

        this.AseAnimator.PlayAnimation(ANIM_HIDE, AnimationParams.NoLooping);
        if(Timer >= 90)
        {
            SwitchState(AIState.Shoot);
        }
    }

    private void AI_Shoot()
    {
        Timer++;
        if(Timer == 1)
        {
            var throwSound = AssetReferences.Assets.Sounds.Jack_Throw.Asset with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(throwSound, NPC.position);
        }

        if(Timer == 1 && MultiplayerHelper.IsHost)
        {
            ProjFirer firer = ProjFirer.From<Launchball>(NPC);
            firer.damage = 21;
            firer.velocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 3;
            firer.velocity.Y -= 12;
            firer.New();
        }
        _outliner.warning = true;
        this.AseAnimator.PlayAnimation(ANIM_SHOOT, AnimationParams.NoLooping);
        if(Timer >= 30)
        {
            SwitchState(AIState.Naked);
        }
    }

    private void AI_Naked()
    {
        this.AseAnimator.PlayAnimation(ANIM_NAKED, AnimationParams.NoLooping);
    }


    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            AttackCycle = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        OutlineRenderer.Queue(DrawOutlineWhite);
        return false;
    }

    private void DrawOutlineWhite(SpriteBatch spriteBatch)
    {
        NPC.DrawAnimator(spriteBatch, _outliner.outlineColor);
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        AbyssEnemyCommon.HitAndDeathEffectsPlanty(NPC);
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ConvulgingMater>(), minimumDropped: 1, maximumDropped: 4));
    }

    public void PrepareSilhouetteDrawing(RekSilhouetteSystem system)
    {
        void DrawWhite(SpriteBatch spriteBatch)
        {
            NPC.DrawAnimator(spriteBatch, Color.Black);
        }
        system.SilhouettesToDraw.Add(DrawWhite);
    }
}
