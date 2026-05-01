using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.EveroseVillage.CelestiaBoss.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.EveroseVillage.CelestiaBoss;

internal class Celestia : ScarletBoss,
    IDrawOutlines
{
    private enum AIState
    {
        Spawn,
        Despawn,
        Idle,
        Death,

        Horse_Ride_Backflip_Shot,
        Horse_Ride_Big_Bow_Shot,
        Project_Away,
        Bow_Spin,
        Backflip_Bow_Rain,
        Projection_Dash,
    }

    private bool _contactDamage;
    private bool _warning;
    private bool _attacking;
    private bool _showTrail;
    private bool _show;
    private float _ghostAlpha;

    private float _trailAlpha;
    private Color _outlineColor;
    private Vector2 _teleportPosition;
    private Vector2 _squishScale;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];
    private const string ANIM_CONFUSED = "Confused";
    private const string ANIM_IDLE = "Idle";
    private const string ANIM_DISAPPEAR = "Disappear";
    private const string ANIM_THROWBOW = "ThrowBow";
    private const string ANIM_HOLDINGBOW = "HoldingBow";
    private const string ANIM_BOWOUT = "BowOut";
    private const string ANIM_BACKFLIPREADY = "BackflipReady";
    private const string ANIM_BACKFLIP = "Backflip";
    private const string ANIM_AIRTIME = "Airtime";
    private const string ANIM_LANDBACKFLIP = "LandBackflip";

    private Animator _animatorBackingField;
    private Animator Animator
    {
        get
        {
            if (_animatorBackingField == null)
            {
                _animatorBackingField = CreateAnimator();
                _animatorBackingField.PlayAnimation(ANIM_IDLE);
            }
            return _animatorBackingField;
        }
    }

    private int Backflip_Bow_Damage => 35;
    public Animator CreateAnimator()
    {
        Animator animator = new Animator();
        Vector2 drawOrigin = new Vector2(55, 123);
        var confused = new SpriteAnimation(0, 5, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_CONFUSED, confused);

        var idle = new SpriteAnimation(0, 5, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_IDLE, idle);

        var disappear = new SpriteAnimation(0, 8, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_DISAPPEAR, disappear);

        var throwBow = new SpriteAnimation(0, 6, isLooping: false, drawOrigin, frameSpeed: 0.25f);
        animator.AddAnimation(ANIM_THROWBOW, throwBow);

        var holdingBow = new SpriteAnimation(0, 0, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_HOLDINGBOW, holdingBow);

        var bowOut = new SpriteAnimation(0, 5, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_BOWOUT, bowOut);

        var backflipReady = new SpriteAnimation(0, 5, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_BACKFLIPREADY, backflipReady);

        var backflip = new SpriteAnimation(0, 10, isLooping: false, drawOrigin, frameSpeed: 0.25f);
        animator.AddAnimation(ANIM_BACKFLIP, backflip);

        var airtime = new SpriteAnimation(0, 0, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_AIRTIME, airtime);

        var landBackflip = new SpriteAnimation(0, 5, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_LANDBACKFLIP, landBackflip);
        return animator;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_teleportPosition);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _teleportPosition = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _squishScale = Vector2.One;
        NPC.width = 32;
        NPC.height = 64;
        NPC.damage = 50;
        NPC.defense = 15;
        NPC.lifeMax = 12000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.value = Item.buyPrice(gold: 12);
        NPC.npcSlots = 10f;
        NPC.scale = 1f;
        NPC.aiStyle = -1;

        // The following code assigns a music track to the boss in a simple way.
        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Moonwalker");
        }
    }
    private void FaceTarget()
    {
        NPC.spriteDirection = MyTarget.Center.X > NPC.Center.X ? -1 : 1;
        NPC.direction = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
    }
    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            AttackCycle = 0;
            AttackCounter = 0;
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }
    public override void AI()
    {
        base.AI();
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget && State != AIState.Despawn)
            {
                SwitchState(AIState.Despawn);
            }
        }

        if(_teleportPosition != Vector2.Zero)
        {
            NPC.Center = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }

        _contactDamage = false;
        _warning = false;
        _attacking = false;
        _showTrail = false;
        _show = true;
        switch (State)
        {
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.Spawn:
                AI_Spawn();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Horse_Ride_Backflip_Shot:
                AI_HorseRideBackflipShot();
                break;
            case AIState.Death:
                break;
        }

        _ghostAlpha = MathHelper.Lerp(_ghostAlpha, (_show ? 1f : 0f), 0.02f);
        float targetTrailAlpha = _showTrail ? 1f : 0f;
        _trailAlpha = MathHelper.Lerp(_trailAlpha, targetTrailAlpha, 0.1f);
        Color targetOutlineColor = Color.Transparent;
        if (_attacking)
        {
            targetOutlineColor = Color.Red;
        } else if (_warning)
        {
            targetOutlineColor = Color.Yellow;
        }
        _outlineColor = Color.Lerp(_outlineColor, targetOutlineColor, 0.1f);
    }

    private void ProjectOut()
    {
        _show = false;
        NPC.velocity.X += 0.1f;
        Animator.PlayAnimation(ANIM_DISAPPEAR);
        if (Animator.IsFinished())
        {
            SwitchState(AIState.Idle);
        }
    }

    private void ProjectIn()
    {

    }

    private void Teleport(Vector2 teleportPosition)
    {
        if (MultiplayerHelper.IsHost)
        {
            _teleportPosition = teleportPosition;
            NPC.netUpdate = true;
        }
    }

    private Vector2 Fall(Vector2 startPosition)
    {
        Point tile = startPosition.ToTileCoordinates();
        tile.Y -= 1;
        tile = TileUtilities.FallToSolidTile(tile);
        return tile.ToWorldCoordinates();
    }

    private float DirectionToTarget()
    {
        return MyTarget.Center.X > NPC.Center.X ? 1 : -1;
    }

    private void AI_HorseRideBackflipShot()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();

                        //Get the starting position and teleport there
                        Vector2 startFrom = MyTarget.Center;
                        startFrom.X += 1024;
                        startFrom = Fall(startFrom);
                        startFrom.Y -= 100;
                        Teleport(startFrom);
                    }

                    //Ride in from whatever sides
                    _warning = true;
                    FaceTarget();
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.velocity.Y = MathF.Sin(Timer * 0.5f) * 0.5f;

                    float dist = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
                    float direction = DirectionToTarget();
                    float gallopSpeed = MathHelper.Lerp(5, 10, EasingFunction.Clamp(dist / 384f));
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, gallopSpeed * direction, 0.1f);
                    Animator.PlayAnimation(ANIM_BACKFLIPREADY);

                    _showTrail = true;
                    if (dist <= 384 && Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if(Timer == 1)
                    {
                        NPC.velocity.Y = -15;
                    }
                    _squishScale = Vector2.Lerp(new Vector2(0.9f, 1.2f), Vector2.One, EasingFunction.InOutSine(Timer / 30f));
                    _showTrail = true;
                    _attacking = true;
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;

                    if(Timer == 15 || Timer == 45)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 vel = NPC.velocity;
                            vel.X += Main.rand.NextFloat(-4f, 4f);
                            Projectile.NewProjectile(SourceFromThis, NPC.Center - Vector2.UnitY * 4, vel,
                                ModContent.ProjectileType<CelestialBow>(), Backflip_Bow_Damage, 1, Main.myPlayer, ai1: MyTarget.whoAmI);
                        }
                    }
                    
                    if(NPC.velocity.Y < 15)
                        NPC.velocity.Y += 0.5f;
                    NPC.velocity.X *= 0.94f;
                    Animator.PlayAnimation(ANIM_BACKFLIP);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _squishScale = Vector2.Lerp(Vector2.One, new Vector2(0.9f, 1.2f), EasingFunction.InOutSine(Timer / 30f));
                    _showTrail = true;
                    NPC.noGravity = true;
                    NPC.noTileCollide = false;

                    if (NPC.velocity.Y < 15)
                        NPC.velocity.Y += 0.5f;
                    NPC.velocity.X *= 0.94f;

                    Animator.PlayAnimation(ANIM_AIRTIME);
                    if (NPC.collideY)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    _squishScale = Vector2.Lerp( new Vector2(1.3f, 0.9f), Vector2.One, EasingFunction.InOutSine(Timer / 30f));
                    if (Timer == 5)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 vel = NPC.velocity;
                            Projectile.NewProjectile(SourceFromThis, NPC.Center - Vector2.UnitY * 4, vel,
                                ModContent.ProjectileType<CelestialBow>(), Backflip_Bow_Damage, 1, Main.myPlayer, ai1: MyTarget.whoAmI);
                        }
                    }

                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                    Animator.PlayAnimation(ANIM_LANDBACKFLIP);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.2f);
                    FaceTarget();
                    NPC.noGravity = true;
                    NPC.noTileCollide = false;

                    NPC.velocity.X *= 0.94f;
                    Animator.PlayAnimation(ANIM_THROWBOW);
                    if(Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 5:
                {
                    FaceTarget();
                    NPC.noGravity = true;
                    NPC.noTileCollide = false;

                    NPC.velocity.X *= 0.94f;
                    Animator.PlayAnimation(ANIM_BOWOUT);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 6:
                {
                    ProjectOut();
                }
                break;
        }
    }

    private void AI_Despawn()
    {
        Timer++;
        ProjectOut();
        if (Timer >= 90)
        {
            NPC.active = false;
        }
    }

    private void AI_Spawn()
    {

        Timer++;
        if (Timer == 1)
        {
            ShowNamePlate();
        }

        if (Timer >= 120)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            AIState state = AIState.Horse_Ride_Backflip_Shot;
            SwitchState(state);
        }
    }
    private void AI_Idle()
    {
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.2f);
        NPC.velocity.X *= 0.5f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;

        _show = false;
        Timer++;
        FaceTarget();
        if(Timer >= 120)
        {
            ChooseAttack();
        }
    }

    private void AI_Death()
    {

    }


    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(frameHeight);
    }
    private void DrawAfterImage(SpriteBatch spriteBatch)
    {
        SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
        spriteBatch.Restart(effect: whiteShader.Effect);

        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = ModContent.Request<Texture2D>($"{Texture}_{Animator.GetAnimation()}").Value;
        drawer.sourceRect = NPC.frame;
        drawer.drawOrigin = Animator.GetDrawOrigin().Value;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            Vector2 pos = NPC.oldPos[i];
            Vector2 oldCenter = pos + new Vector2(NPC.Size.X * 0.5f, NPC.Size.Y);
            drawer.worldPosition = oldCenter;
            drawer.worldPosition.Y -= 2;
            drawer.color = Color.Lerp(Color.Lerp(Color.White, Color.Turquoise, 0.85f), Color.DarkGreen, i / (float)NPC.oldPos.Length);
            drawer.color *= MathHelper.Lerp(1f, 0f, i / (float)NPC.oldPos.Length);
            drawer.color *= _trailAlpha;
            drawer.color *= 0.35f;
            spriteBatch.Draw(drawer);

        }
        spriteBatch.RestartDefaults();
    }
    private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = ModContent.Request<Texture2D>($"{Texture}_{Animator.GetAnimation()}").Value;
        drawer.sourceRect = NPC.frame;
        drawer.drawOrigin = Animator.GetDrawOrigin().Value;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        drawer.color = drawColor * _ghostAlpha;
        drawer.worldPosition =  NPC.Bottom + screenPos;
        drawer.worldPosition.Y -= 2;
        drawer.scale *= _squishScale;
        spriteBatch.Draw(drawer);

    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawAfterImage(spriteBatch);
        DrawSprite(spriteBatch, Vector2.Zero, drawColor);
        return false;
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        Vector2 v = Vector2.UnitX * 2;
        Vector2 h = Vector2.UnitY * 2;

        DrawSprite(spriteBatch, v, _outlineColor);
        DrawSprite(spriteBatch, -v, _outlineColor);
        DrawSprite(spriteBatch, h, _outlineColor);
        DrawSprite(spriteBatch, -h, _outlineColor);
    }
}
