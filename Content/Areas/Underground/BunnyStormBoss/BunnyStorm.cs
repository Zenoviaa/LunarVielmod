using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Collosseum.BossesCL.EliteCommander.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.BunnyStormBoss;




public class BunnyStormBunny : ModProjectile
{

    private ref float Timer => ref Projectile.ai[0];
    private ref float Style => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 6;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.timeLeft = 300;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return base.OnTileCollide(oldVelocity);
    }
    public override bool ShouldUpdatePosition()
    {
        return base.ShouldUpdatePosition();
    }
    public override bool CanHitPlayer(Player target)
    {
        return base.CanHitPlayer(target);
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        switch (Style)
        {
            case 0:
                AI_Falling();
                break;
        }
    }

    private void AI_Falling()
    {
        if(Timer % 8 == 0)
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Projectile.width);
            pos.Y = Main.rand.Next(0, Projectile.height);
            pos += Projectile.position;
            var sp = SparkleParticle.Spawn(pos, -Vector2.UnitY * 0.3f, Scale: 0.3f);
            sp.outerColor = Color.SkyBlue;
            sp.noTileCollide = true;
            sp.gravity = 0;
        }

        if(Timer % 24 == 0)
        {
            var fx = FXUtil.GlowStretch(Projectile.Center + Main.rand.NextVector2Circular(32, 32), -Projectile.velocity * 0.05f);
            fx.VectorScale *= 0.5f;
        }
        if(Projectile.velocity.Y < 15)
            Projectile.velocity.Y += 0.15f;
        Projectile.rotation += Projectile.velocity.Length() * 0.025f;
        Projectile.rotation += 0.015f;
        Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 2048);
        if (player != null && Projectile.Bottom.Y > player.Top.Y && Projectile.velocity.Y > 5)
            Projectile.tileCollide = true;
        Projectile.frameCounter++;
        if(Projectile.frameCounter >= 5)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= 2)
                Projectile.frame = 0;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            SpritebatchDrawer afDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            afDrawer.worldPosition = pos;
            afDrawer.rotation = Projectile.oldRot[i];
            afDrawer.color = Color.Lerp(Color.White, Color.Black, (float)i / (float)Projectile.oldPos.Length) * 0.05f;
            afDrawer.color.A = 0;
            Main.spriteBatch.Draw(afDrawer);
        }

        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);


        Main.spriteBatch.Draw(sbDrawer);

        sbDrawer.VerticalFrame(Projectile.frame + 4, 6);
        sbDrawer.color = Color.Red;
        Main.spriteBatch.Draw(sbDrawer);

        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class BunnyStormShader : CrystalShader<BunnyStormShader>
{
    private EffectParameter _tilingParam;
    private EffectParameter _mixTextureParam;
    private EffectParameter _offsetParam;
    public Vector2 Tiling
    {
        set
        {
            _tilingParam = Effect.Parameters["tiling"];
            _tilingParam.SetValue(value);
        }
    }
    public Vector2 Offset
    {
        set
        {
            _offsetParam = Effect.Parameters["offset"];
            _offsetParam.SetValue(value);
        }
    }
    public Texture2D MixTexture
    {
        set
        {
            _mixTextureParam = Effect.Parameters["mixTexture"];
            _mixTextureParam.SetValue(value);
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();

    }
}
public class BunnyStorm : ScarletBoss
{
    private enum AIState
    {
        Spawn,
        Despawn,
        Idle,
        Death,

        Hand_Wiggle_Bunny_Drop,
        Bunny_Fist,
        Bunny_Gun,
        Bunny_Remerge,
    }


    private int _stormFrame;
    private float _stormRotation;
    private bool _contactDamage;
    private Vector2 _initialVelocity;
    private Vector2 _boundPoint;
    private Vector2 _stormScale;
    private Outliner _outliner;
    private Asset<Texture2D> _bunnyNoiseTextureAsset;
    private Asset<Texture2D> _bunnyMaskTextureAsset;
    private Asset<Texture2D> _earTextureAsset;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];

    private PatternManager<AIState> _patternManagerBackingField;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            if(_patternManagerBackingField == null)
            {
                _patternManagerBackingField = new();
                _patternManagerBackingField.AddPattern(AIState.Hand_Wiggle_Bunny_Drop, 1f);
                _patternManagerBackingField.AddPattern(AIState.Bunny_Fist, 1f);
                _patternManagerBackingField.AddPattern(AIState.Bunny_Gun, 1f);
            }

            return _patternManagerBackingField;
        }
    }

    private float IdleTime => 400;
    private float SpawnTime => 200;
    private float DespawnTime => 100f;

    private float MaxIdleHoverDistance => 180;

    private int ShockwaveDamage => 24;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_boundPoint);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _boundPoint = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 3;
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 100;
        NPC.height = 100;
        NPC.damage = 50;
        NPC.defense = 10;
        NPC.lifeMax = 2000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = Item.buyPrice(gold: 12);
        NPC.npcSlots = 10f;
        NPC.scale = 1f;
        NPC.aiStyle = -1;

        // The following code assigns a music track to the boss in a simple way.
        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/DeadlyFoe");
        }
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void AI()
    {
        base.AI();
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget && State != AIState.Despawn)
                SwitchState(AIState.Despawn);
        }

        if (Main.rand.NextBool(16))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, NPC.width);
            pos.Y = Main.rand.Next(0, NPC.height);
            pos += NPC.position;
            var sp = SparkleParticle.Spawn(pos, -Vector2.UnitY * 0.3f, Scale: 0.3f);
            sp.outerColor = Color.SkyBlue;
            sp.noTileCollide = true;
            sp.gravity = 0;
        }

        _contactDamage = false;
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Spawn:
                AI_Spawn();
                break;
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.Death:
                AI_Death();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Hand_Wiggle_Bunny_Drop:
                AI_HandWiggleBunnyDrop();
                break;
            case AIState.Bunny_Fist:
                AI_BunnyFist();
                break;
            case AIState.Bunny_Gun:
                AI_BunnyGun();
                break;
            case AIState.Bunny_Remerge:
                AI_BunnyRemerge();
                break;
        }
        _outliner.Update();
    }


    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            SwitchState(PatternManager.NextPattern());
        }
        SwitchState(AIState.Bunny_Fist);
    }

    private void AI_BunnyRemerge()
    {

    }

    private void AI_BunnyGun()
    {

    }
    private bool IsGrounded()
    {
        Point solidTileBelow = NPC.Bottom.ToTileCoordinates();
        solidTileBelow.Y++;
        bool tileSolid = Main.tileSolid[Main.tile[solidTileBelow].TileType] || Main.tileSolidTop[Main.tile[solidTileBelow].TileType];
        bool isGrounded = Main.tile[solidTileBelow].HasTile && tileSolid;
        return isGrounded;

    }
    private void AI_BunnyFist()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        _initialVelocity = NPC.velocity;
                        NPC.TargetClosest();
                    }

                    float speedUp = MathHelper.Lerp(1f, 0.75f, AttackCounter / 3f);
                    float fistingTime = 150 * speedUp;
                    float ratio = Timer / fistingTime;
                    float ease = EasingFunction.InOutSine(ratio);
                    Vector2 s1 = Vector2.Lerp(Vector2.One, Vector2.Zero, ease);
                    Vector2 s2 = Vector2.Lerp(Vector2.Zero, Vector2.One, ease);
                    _stormScale = Vector2.Lerp(s1, s2, ease);


                    float inTime = 120f * speedUp;
                    float ratio2 = Timer / inTime;
                    Vector2 startupPoint = _boundPoint - Vector2.UnitY * MathHelper.Lerp(100, 50, EasingFunction.InOutSine(ratio2));
                    float dir = AttackCounter % 2 == 0 ? 1 : -1;
                    if (AttackCounter >= 3)
                        dir = 0;
                    startupPoint -= Vector2.UnitX * MathHelper.Lerp(0, 300, EasingFunction.InOutSine(ratio2)) * dir;
                    startupPoint.Y -= MathHelper.Lerp(0f, 100, EasingFunction.InExpo(ratio));
                    Vector2 velocityToPoint = (startupPoint - NPC.Center);

                    NPC.velocity = Vector2.Lerp(_initialVelocity, velocityToPoint, EasingFunction.InOutExpo(ratio2));

                    if (Timer >= fistingTime / 2f)
                    {
                        _stormFrame = 3;
                    }
                    _outliner.warning = true;
          
                  
                    _stormRotation = MathHelper.Lerp(0, MathHelper.TwoPi * 2, EasingFunction.InOutExpo(ratio)) + MathHelper.PiOver2;
                    if(Timer >= fistingTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
   
                break;
            case 1:
                {
                    _contactDamage = true;
                    _outliner.attacking = true;
                    if (NPC.velocity.Y < 25)
                        NPC.velocity.Y += MathHelper.Lerp(0.05f, 4f, EasingFunction.InExpo(Timer / 60f));
                    else
                    {
                        if(Timer % 4 == 0)
                        {
                            LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -Vector2.UnitY);
                        }

                        NPC.velocity.Y *= 1.05f;
                    }
                    if (NPC.Bottom.Y < MyTarget.Top.Y)
                        NPC.noTileCollide = true;
                    else
                        NPC.noTileCollide = false;
                    if (IsGrounded() && Timer > 15)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if(Timer == 1)
                    {
                        FXUtil.ShakeCamera(NPC.Center, 1024, 8);
               
                        SoundStyle boom = SoundID.DD2_ExplosiveTrapExplode;
                        boom.PitchVariance = 0.3f;
                        SoundEngine.PlaySound(boom, NPC.position);
                        for (int i = 0; i < 16; i++)
                        {
                            float radius = 150;
                            Vector2 offset = Vector2.UnitX * Main.rand.Next(-1, 1);
                            offset *= Main.rand.NextFloat(1f, radius);
                            offset += new Vector2(radius / 2, 0);

                            Vector2 velocity = Vector2.UnitX * Main.rand.Next(-1, 1);
                            velocity *= Main.rand.NextFloat(1f, 2f);
                            Dust.NewDustPerfect(NPC.Bottom + offset, ModContent.DustType<Dusts.TSmokeDust>(), velocity, 0, Color.Black * 0.5f,
                                Main.rand.NextFloat(0.3f, 0.7f));
                        }

                        FXUtil.GlowCircleBoom(NPC.Bottom,
                           innerColor: Color.White,
                           glowColor: Color.Black,
                           outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
                        for (float i = 0; i < 4; i++)
                        {
                            float progress = i / 4f;
                            float rot = progress * MathHelper.ToRadians(240);
                            Vector2 offset = rot.ToRotationVector2() * 24;
                            var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Bottom,
                                innerColor: Color.White,
                                glowColor: Color.Black,
                                outerGlowColor: Color.Black,
                                baseSize: 0.24f);
                            particle.Rotation = rot + MathHelper.ToRadians(45);
                        }

                        for (int i = 0; i < 7; i++)
                        {
                            Vector2 velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(15f, 35f);
                            var particle = FXUtil.GlowStretch(NPC.Bottom, velocity);
                            particle.InnerColor = Color.White;
                            particle.GlowColor = Color.LightCyan;
                            particle.OuterGlowColor = Color.Black;
                            particle.Duration = Main.rand.NextFloat(25, 50);
                            particle.BaseSize = Main.rand.NextFloat(0.045f, 0.09f);
                            particle.VectorScale *= 0.5f;
                        }


                        if (MultiplayerHelper.IsHost)
                        {
                            //This is the part where you spawn the cool ahh shockwaves
                            //But we have to make cool ahh shockwaves :(
                            int shockwaveDamage = ShockwaveDamage;
                            int knockback = 1;
                            Vector2 velocity = Vector2.UnitX;
                            velocity *= 4;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, velocity,
                                ModContent.ProjectileType<WindShockwave>(), shockwaveDamage, knockback, Main.myPlayer);
                            velocity = -velocity;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, velocity,
                           ModContent.ProjectileType<WindShockwave>(), shockwaveDamage, knockback, Main.myPlayer);
                            for(int i = 0; i < 8; i++)
                            {
                                Vector2 fireVelocity = Vector2.Lerp(-Vector2.UnitX, Vector2.UnitX, (float)i / 8f) * 8;
                                fireVelocity.Y -= 12;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, fireVelocity,
                                    ModContent.ProjectileType<BunnyStormBunny>(), shockwaveDamage, knockback, Main.myPlayer, ai1: 0);
                            }
                        }
                    }

                    ShakeScreenPosition.Shake = MathHelper.Lerp(8, 1, EasingFunction.InOutSine(Timer / 100f));

      
                    NPC.velocity.Y = 0;
                    NPC.noTileCollide = true;
                    
                    if (AttackCounter < 3)
                    {
                        Vector2 s1 = Vector2.Lerp(Vector2.One, new Vector2(0.5f), EasingFunction.OutExpo(Timer / 50f));
                        Vector2 s2 = Vector2.Lerp(new Vector2(0.5f), Vector2.One, EasingFunction.InOutSine(Timer / 50f));
                        Vector2 s = Vector2.Lerp(s1, s2, Timer / 50f);
                        _stormScale =s;
                        if(Timer >= 25)
                        {
                            _stormFrame = 0;
                        }
                        if (Timer >= 50)
                        {

                            Timer = 0;
                            AttackCycle = 0;
                            AttackCounter++;
                        }
              
                    }
                    else
                    {
                        if (Timer >= 100)
                        {
                            SwitchState(AIState.Idle);
                        }
                    }

                }
                break;
        }
    }
    private void AI_HandWiggleBunnyDrop()
    {

    }
    private void AI_Idle()
    {
        Timer++;
        if(Timer == 1)
        {
            NPC.TargetClosest();
        }

        //For the idle state, the storm moves around its binding point semi randomly, but it follows a nice path, probably just have it circle around it
        Vector2 vecFromBind = (NPC.Center - _boundPoint);
        vecFromBind = vecFromBind.RotatedBy(0.015);
        if(vecFromBind.Length() > MaxIdleHoverDistance)
        {
            vecFromBind = vecFromBind.Resize(MaxIdleHoverDistance);
        } else if (vecFromBind.Length() < 1)
        {
            vecFromBind += Vector2.UnitY * MaxIdleHoverDistance * 0.5f;
        }

      //  vecFromBind *= MathHelper.Lerp(0.5f, 1f, MathF.Sin(Timer * 0.05f) * 0.5f + 0.5f);
        Vector2 newPoint = _boundPoint + vecFromBind;
        Vector2 velocityToNewPoint = (newPoint - NPC.Center);
        NPC.velocity = NPC.velocity.MoveTowards(velocityToNewPoint, MathHelper.Lerp(0f, 2f, EasingFunction.InOutExpo(Timer / 120f)));
        NPC.rotation = NPC.velocity.X * 0.05f;
        _stormFrame = 0;
        _stormScale = Vector2.Lerp(_stormScale, Vector2.One, 0.1f);
        _stormRotation += 0.05f;
        if(Timer >= IdleTime)
        {
            ChooseAttack();
        }
    }
    private void AI_Spawn()
    {
        Timer++;
        if (Timer == 1)
        {
            _boundPoint = NPC.Center;
            NPC.netUpdate = true;
        }

        if (Timer % 2 == 0)
        {
            float range = Main.rand.NextFloat(252, 512);
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
            Vector2 vel = (NPC.Center - pos);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.VectorScale *= 0.5f;
        }

        if (Timer % 2 == 0)
        {
            float range = Main.rand.NextFloat(384, 666);
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
            Vector2 vel = (NPC.Center - pos);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 1f));
            fx.VectorScale *= 0.25f;
        }

        CameraTargetSystem.AddTarget(NPC.Center);
        CameraTargetSystem.SetLingerTime(120);
        NPC.velocity.Y = MathHelper.Lerp(-1f, 0f, EasingFunction.InOutSine(Timer / SpawnTime));
        _stormScale = Vector2.Lerp(Vector2.Zero, Vector2.One, Timer / SpawnTime);
        if (Timer >= SpawnTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_Death()
    {

    }

    private void AI_Despawn()
    {
        Timer++;
        NPC.velocity.X *= 0.98f;
        NPC.velocity.Y += 0.05f;
        if(Timer >= DespawnTime)
        {
            NPC.active = false;
        }
    }


    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            AttackCycle = 0;
            AttackCounter = 0;
            NPC.netUpdate = true;
        }
    }
    public override BossLevel GetBossLevel()
    {
        return BossLevel.Miniboss;
    }


    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }
    private void DrawMask(SpriteBatch sb, int frameOffset)
    {
        _bunnyMaskTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Hand");
        SpritebatchDrawer maskDrawer = SpritebatchDrawer.FromTextureAsset(_bunnyMaskTextureAsset, NPC.Center);
        maskDrawer.VerticalFrame(_stormFrame + frameOffset, 12);
        maskDrawer.CenterOrigin();
        maskDrawer.scale = _stormScale * ExtraMath.Osc(0.85f, 1.1f, speed: 3);
        maskDrawer.rotation = _stormRotation;
        sb.Draw(maskDrawer);

    }
    private void DrawMask(SpriteBatch sb, int frameOffset, Color color)
    {
        _bunnyMaskTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Hand");
        SpritebatchDrawer maskDrawer = SpritebatchDrawer.FromTextureAsset(_bunnyMaskTextureAsset, NPC.Center);
        maskDrawer.VerticalFrame(_stormFrame + frameOffset, 12);
        maskDrawer.CenterOrigin();
        maskDrawer.scale = _stormScale * ExtraMath.Osc(0.85f, 1.1f, speed: 3);
        maskDrawer.rotation = _stormRotation;
        maskDrawer.color = color;
        sb.Draw(maskDrawer);

    }
    private void DrawCrystal(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, _boundPoint);
        glowDrawer.scale *= 0.3f;
        glowDrawer.color = Color.SkyBlue * ExtraMath.Osc(0.2f, 0.6f, speed: 3);
        glowDrawer.color.A = 0;
        spriteBatch.Draw(glowDrawer);

        NPC.spriteDirection = 1;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.worldPosition = _boundPoint;
        drawer.worldPosition.Y += ExtraMath.Osc(-2f, 2f, speed: 3);
        spriteBatch.Draw(drawer);

        drawer.color = Color.Green * ExtraMath.Osc(0.1f, 0.25f, speed: 2);
        drawer.color.A = 0;
        drawer.VerticalFrame(1, 3);
        spriteBatch.Draw(drawer);
    }
    private Color DashTrailColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio);
    }

    private float DashTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(64, 64, completionRatio);
    }
    private void RenderPixelatedDashTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader laserShader = BasicLaserShader.Instance;
        laserShader.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.DarkGray;
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, DashTrailColorFunction, DashTrailWidthFunction, laserShader, NPC.Size * 0.5f);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        _bunnyNoiseTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Noise");
        _bunnyMaskTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Hand");
        _earTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Ear");

        SpritebatchDrawer spiralVortexDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SpiralVortex, NPC.Center);
        spiralVortexDrawer.rotation = Main.GlobalTimeWrappedHourly * 8;
        spiralVortexDrawer.color = Color.White * 0.12f;
        spiralVortexDrawer.color.A = 0;
        spriteBatch.Draw(spiralVortexDrawer);
        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            Vector2 pos = NPC.oldPos[i];

            SpritebatchDrawer maskDrawer = SpritebatchDrawer.FromTextureAsset(_bunnyMaskTextureAsset, pos + NPC.Size * 0.5f);
            maskDrawer.VerticalFrame(_stormFrame, 12);
            maskDrawer.CenterOrigin();
            maskDrawer.scale = _stormScale * ExtraMath.Osc(0.85f, 1.1f, speed: 3);
            maskDrawer.rotation = _stormRotation;
            maskDrawer.color = Color.Lerp(Color.White, Color.Black, (float)i / (float)NPC.oldPos.Length) * 0.05f;
            maskDrawer.color.A = 0;
            spriteBatch.Draw(maskDrawer);
        }
        DrawCrystal(spriteBatch);

        DrawMask(spriteBatch, 4, Color.Black);

        DrawMask(spriteBatch, 4, _outliner.outlineColor);
        BunnyStormShader combineShader = ShaderContent.GetInstance<BunnyStormShader>();
        combineShader.MixTexture = _bunnyNoiseTextureAsset.Value;
        combineShader.Offset = new Vector2(Main.GlobalTimeWrappedHourly, 0);
        combineShader.Tiling = new Vector2(1f, 12f) * 2;
        spriteBatch.Restart(SpriteSortMode.Immediate,effect: combineShader.Effect, samplerState: SamplerState.PointClamp);

        combineShader.Offset = new Vector2(-Main.GlobalTimeWrappedHourly, 0.3f);
        DrawMask(spriteBatch, 0, Color.DarkGray);

        combineShader.Offset = new Vector2(Main.GlobalTimeWrappedHourly, 0);
        DrawMask(spriteBatch, 0, Color.White);


  
        spriteBatch.RestartDefaults();


        DrawMask(spriteBatch, 8, Color.Black * 0.5f);

        float offset = 32;
        SpritebatchDrawer earDrawer = SpritebatchDrawer.FromTextureAsset(_earTextureAsset, NPC.Center);
        earDrawer.BottomLeftOrigin();
        earDrawer.drawOrigin.X += 32;
        earDrawer.drawOrigin.Y -= 16;
        earDrawer.worldPosition.X += offset;
        earDrawer.worldPosition.Y -= 64;
        earDrawer.rotation = NPC.velocity.X * 0.05f + ExtraMath.Osc(-0.3f, 0.3f);
        earDrawer.scale *= _stormScale;
        spriteBatch.Draw(earDrawer);


        earDrawer.spriteEffects = SpriteEffects.FlipHorizontally;
        earDrawer.drawOrigin.X = earDrawer.texture.Width - earDrawer.drawOrigin.X;
        earDrawer.worldPosition.X -= offset * 2;
        earDrawer.rotation = NPC.velocity.X * -0.05f + ExtraMath.Osc(0.3f, -0.3f, offset: 1);
        spriteBatch.Draw(earDrawer);


        PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedDashTrail);
        /*
        drawer.color = Color.LightGreen * ExtraMath.Osc(0.5f, 1f, speed: 16);
        drawer.color.A = 0;
        drawer.VerticalFrame(2, 3);
        spriteBatch.Draw(drawer);
        */
        return false;
    }

    private void DrawBunnyMask(SpriteBatch sb)
    {
        _bunnyMaskTextureAsset ??= ModContent.Request<Texture2D>($"{Texture}_Hand");
        DrawMask(sb, 0);
    }

}
