using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss.Projectiles;
using Stellamod.Core.Palettes;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.VerliaBoss;

public class VerliaClone : ModNPC,
    IDrawOutlines
{
    private Color _outlineColor;
    private bool _warning;
    private bool _attacking;

    private Vector2 _startVelocity;
    private float _dir;
    public string RootTexture => ModContent.GetInstance<Verlia>().Texture;
    public override string Texture => TextureRegistry.EmptyTexture;
    private enum AIState
    {
        Idle,
        Moon_Slash_Copy
    }

    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private Animator _animatorBackingField;
    private Animator Animator
    {
        get
        {
            if (_animatorBackingField == null)
            {

                _animatorBackingField = ModContent.GetInstance<Verlia>().CreateAnimator();
                _animatorBackingField.PlayAnimation(Verlia.ANIM_UNSUMMON);
            }
            return _animatorBackingField;
        }
    }
    private Player MyTarget => Main.player[NPC.target];
    private int Moon_Slash_Damage => 50;

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_startVelocity);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startVelocity = reader.ReadVector2();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
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
        NPC.width = 32;
        NPC.height = 64;
        NPC.damage = 1;
        NPC.defense = 15;
        NPC.dontCountMe = true;
        NPC.dontTakeDamage = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.lifeMax = 6750;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = Item.buyPrice(gold: 12);
        NPC.npcSlots = 10f;
        NPC.scale = 1f;
        NPC.aiStyle = -1;
    }

    public override void AI()
    {
        base.AI();
        _attacking = false;
        _warning = false;
        switch (State)
        {
            case AIState.Moon_Slash_Copy:
                AI_MoonSlashCopy();
                break;
        }
        if (_attacking)
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Red, 0.1f);
        }
        else if (_warning)
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Yellow, 0.1f);
        }
        else
        {
            _outlineColor = Color.Lerp(_outlineColor, Color.Transparent, 0.1f);
        }
    }
    private void FaceTarget()
    {
        NPC.spriteDirection = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
    }
    private void AI_MoonSlashCopy()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        _startVelocity = NPC.velocity;
                        _dir = Main.rand.NextBool(2) ? -1 : 1;
                    }

                    float time = 60;
                    float ratio = Timer / time;
                    float ease = EasingFunction.InOutSine(ratio);
                    Vector2 pos = MyTarget.Center + Vector2.UnitX * _dir * 128;
                    Vector2 targetVelocity = (pos - NPC.Center);
                    NPC.velocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
                    FaceTarget();
                    Animator.PlayAnimation(Verlia.ANIM_TELEPORTIN);
                    if (Animator.IsFinished() && Timer >= time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    //    CameraTargetSystem.AddTarget(NPC.Center);
                    if (Timer == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/SwordSheethe"), NPC.position);
                        NPC.TargetClosest();
                        _startVelocity = NPC.velocity;

                    }

                    FaceTarget();

                    if (Timer < 90f)
                    {
                        float ratio = Timer / 90f;
                        float ease = EasingFunction.InOutExpo(ratio);
                        Vector2 inverseDir = MyTarget.Center.X > NPC.Center.X ? -Vector2.UnitX : Vector2.UnitX;
                        Vector2 targetPosition = MyTarget.Center + inverseDir * 128;
                        Vector2 targetVelocity = targetPosition - NPC.Center;
                        Vector2 interpolatedVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
                        NPC.velocity = interpolatedVelocity;
                    }
                    else
                    {
                        NPC.velocity *= 0.9f;
                    }


                    _warning = true;
                    Animator.PlayAnimation(Verlia.ANIM_SWORD);
                    if (Animator.IsFinished() && Timer > 140)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer == 1)
                    {
                        Vector2 dir2 = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                        NPC.velocity = dir2;
                        if (MultiplayerHelper.IsHost)
                        {
                            float dir = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
                            Vector2 velocity = Vector2.UnitX * dir;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity, ModContent.ProjectileType<MoonSlash>(), Moon_Slash_Damage, 1, Main.myPlayer);
                        }

                        FXUtil.ShakeCamera(NPC.Center, 1024, 16);
                    }
                    /*
                    if (ModContent.GetInstance<LunarVeilClientConfig>().DramaticEffects)
                    {
                        SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                        effectsPlayer.darknessCurve = MathHelper.Lerp(0.75f, 0f, EasingFunction.InExpo(Timer / 30f));
                    }*/
                    if (NPC.velocity.Length() < 25)
                        NPC.velocity *= 1.5f;

                    _attacking = true;
                    Animator.PlayAnimation(Verlia.ANIM_SWORDSLASH);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                        NPC.active = false;
                    }
                }
                break;
        }
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit)
    {
        base.OnHitNPC(target, hit);
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(184);
        NPC.frame.Height = 184;
        NPC.frame.Width = 266;
    }

    private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        string path = $"{ModContent.GetInstance<Verlia>().Texture}_{Animator.GetAnimation()}";

        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = ModContent.Request<Texture2D>(path).Value;

        drawer.sourceRect = NPC.frame;
        drawer.drawOrigin = Animator.GetDrawOrigin().Value;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        drawer.color = drawColor * ExtraMath.Osc(0.25f, 0.75f, speed: 12, offset: NPC.whoAmI);
        drawer.worldPosition += screenPos;
        spriteBatch.Draw(drawer);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!Animator.GetDrawOrigin().HasValue)
            return false;
        DrawSprite(spriteBatch, Vector2.Zero, drawColor);
        return false;
    }

    public override void OnKill()
    {
        base.OnKill();
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        Vector2 v = Vector2.UnitX * 2;
        Vector2 h = Vector2.UnitY * 2;

        DrawSprite(spriteBatch, v, _outlineColor);
        DrawSprite(spriteBatch, -v, _outlineColor);
        DrawSprite(spriteBatch, h, _outlineColor);
        DrawSprite(spriteBatch, -h, _outlineColor);
        //      throw new System.NotImplementedException();
    }
}
