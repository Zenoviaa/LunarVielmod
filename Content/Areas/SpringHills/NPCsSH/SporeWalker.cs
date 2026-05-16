using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.NPCsSH;

public class SporeShroomClouds : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer % 16 == 0)
        {
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
            var fs = FaintSmokeParticle.SpawnInAlphaLayer(pos, Vector2.Zero);
            fs.color = Color.OrangeRed * 0.8f;
            fs.fadeToColor = Color.Lerp(fs.color, Color.Black, 0.8f) * 0.8f;
            fs.Scale *= 0.1f * Projectile.scale;
        }
        Projectile.velocity.Y -= 0.05f;
    }

    private void DrawPixelatedSmog(SpriteBatch sb , Vector2 screenPos)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        float ratio = Timer / 60f;
        float ease = EasingFunction.QuadraticBump(ratio);
        drawer.color = Color.Lerp(Color.Transparent, Color.OrangeRed, ease) * 0.6f;
        drawer.color.A = 0;
        drawer.scale *= 0.5f;
        sb.Draw(drawer);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedSmog, DrawLayer.OverNPCsWithOutline);
        return false;
      //  return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class SporeWalker : ModNPC,
    IDrawOutlines
{
    private enum AIState
    {
        Idle,
        Run,
        Hide,
        Hidden,
        UnHide
    }
    private Color _outlineColor;
    private bool _warn;
    private bool _contactDamage;
    private Vector2 _squishScale;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private ref float UnHideTimer => ref NPC.ai[2];
    private Animator _animator;
    private Animator Animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = new Animator();
                var idle = new SpriteAnimation(0, 3, isLooping: true);
                _animator.AddAnimation(Anim_Idle, idle);

                var running = new SpriteAnimation(4, 9, isLooping: true, frameSpeed: 0.15f);
                _animator.AddAnimation(Anim_Run, running);

                var jumpStartup = new SpriteAnimation(10, 14, isLooping: false, frameSpeed: 0.15f);
                _animator.AddAnimation(Anim_Hide, jumpStartup);

                var jump = new SpriteAnimation(15, 17, isLooping: true);
                _animator.AddAnimation(Anim_Hidden, jump);

                var unHide = new SpriteAnimation(10, 14, isLooping: false, frameSpeed: 0.15f);
                unHide.reverse = true;
                _animator.AddAnimation(Anim_Unhide, unHide);

            }
            return _animator;
        }
    }
    private Player Target => Main.player[NPC.target];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 18;
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.TrailingMode[Type] = 3;
        this.AddToSpringHills();
        // this.ModifySpawnWeight(0.5f);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 20;
        NPC.height = 20;
        NPC.lifeMax = 26;
        NPC.damage = 14;
        NPC.defense = 0;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.HitSound = SoundID.NPCHit15;
        NPC.DeathSound = SoundID.NPCDeath11;
    }

    private const string Anim_Idle = "idle";
    private const string Anim_Run = "run";
    private const string Anim_Hide = "hide";
    private const string Anim_Hidden = "hidden";
    private const string Anim_Unhide = "unhide";
    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = _animator.GetFrameY(frameHeight);
    }
    

    public override void AI()
    {
        base.AI();
        if (!NPC.HasValidTarget)
            NPC.TargetClosest(faceTarget: false);

        _warn = false;
        _contactDamage = false;
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Run:
                AI_Run();
                break;
            case AIState.Hide:
                AI_Hide();
                break;
            case AIState.Hidden:
                AI_Hidden();
                break;
            case AIState.UnHide:
                AI_UnHide();
                break;
        }

        Color targetOutlineColor;
        if (_contactDamage)
            targetOutlineColor = Color.Red;
        else if (_warn)
            targetOutlineColor = Color.Yellow;
        else
            targetOutlineColor = Color.Transparent;
        _outlineColor = Color.Lerp(_outlineColor, targetOutlineColor, 0.2f);
    }


    private void AI_Idle()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest(faceTarget: false);
 
        }
        _squishScale = Vector2.One;
        if (NPC.direction == 0)
            NPC.direction = -1;
        NPC.velocity.X *= 0.94f;
        NPC.velocity.Y *= 0.94f;
        Animator.PlayAnimation(Anim_Idle);
        if(Timer >= 60)
        {
            SwitchState(AIState.Run);
        }
    }

    private void AI_Run()
    {
        Timer++;
        if(Timer == 1)
        {
            NPC.direction *= -1;
            NPC.TargetClosest(faceTarget: false);
        }
        _squishScale = Vector2.One;
        float xSpeed = NPC.direction * 1;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, xSpeed, 0.1f);
        Animator.PlayAnimation(Anim_Run);
        if(Timer >= 360)
        {
            SwitchState(AIState.Idle);
        }

        float distToTarget = Vector2.Distance(Target.Center, NPC.Center);
        if(distToTarget < 128)
        {
            SwitchState(AIState.Hide);
        }
    }

    private void AI_Hide()
    {
        _squishScale = Vector2.One;
        Timer++;
        NPC.velocity.X *= 0.9f;
        Animator.PlayAnimation(Anim_Hide);
        if (Animator.IsFinished())
        {
            SwitchState(AIState.Hidden);
        }
        UnHideTimer = 0;
        _warn = true;
    }

    private void AI_Hidden()
    {
        Timer++;
        if(Timer == 1)
        {
            SoundStyle gasSound = new SoundStyle("Stellamod/Assets/Sounds/ExplosionGaseous");
            gasSound.PitchVariance = 0.3f;
            gasSound.Volume = 0.5f;
            SoundEngine.PlaySound(gasSound, NPC.position);
            NPC.TargetClosest(faceTarget: true);
        }

        _squishScale = Vector2.Lerp(new Vector2(1.5f, 0.5f), Vector2.One, EasingFunction.InOutSine(Timer / 120f));
        NPC.velocity.X *= 0.9f;
        Animator.PlayAnimation(Anim_Hidden);

        if(MultiplayerHelper.IsHost && Timer % 30 == 0)
        {
            Vector2 pos = NPC.Center;
            pos += Main.rand.NextVector2Circular(64, 64);

            Vector2 vel = Main.rand.NextVector2Circular(1, 1);
            Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, vel,
                ModContent.ProjectileType<SporeShroomClouds>(), 10, 1, Main.myPlayer);
        }

        _contactDamage = true;
        float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
        if(distanceToTarget > 128)
        {
            UnHideTimer++;
        }
        else
        {
            UnHideTimer = 0;
        }
        if(UnHideTimer >= 120)
        {
            SwitchState(AIState.UnHide);
        }
    }
    private void AI_UnHide()
    {
        _squishScale = Vector2.One;
        Timer++;
        NPC.velocity.X *= 0.9f;
        Animator.PlayAnimation(Anim_Unhide);
        if (Animator.IsFinished())
        {
            SwitchState(AIState.Idle);
        }
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if(State != AIState.Hide && State != AIState.Hidden)
        {
            SwitchState(AIState.Hide);
        }
        if (Main.netMode == NetmodeID.Server)
            return;
        if (NPC.life <= 0)
        {
            //Create gores
            for (int k = 0; k < 2; k++)
            {
                Vector2 pos = NPC.position;
                pos.X += Main.rand.Next(0, NPC.width);
                pos.Y += Main.rand.Next(0, NPC.height);
                DustParticle dp = Particle<DustParticle>.Spawn(pos, Vector2.UnitX * hit.HitDirection * Main.rand.NextFloat(1f, 4f), Scale: 0.5f);
                dp.outerColor = Color.DarkGray;
                dp.gravity = 0.01f;
                dp.fast = true;
            }


            int headGore = Mod.Find<ModGore>($"{Name}_Gore_Top").Type;
            int legGore = Mod.Find<ModGore>($"{Name}_Gore_Bottom").Type;

            // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
        }
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Mushroom>(), minimumDropped: 2, maximumDropped: 3));
    }

    private Vector2 DrawOrigin()
    {
        Vector2 drawOrigin = NPC.frame.Size() * 0.5f;
        drawOrigin.Y += 12;
        return drawOrigin;
    }

    private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawOrigin = DrawOrigin();
        SpriteEffects spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Vector2 drawCenter = NPC.Center - screenPos;
        float rotation = NPC.rotation;
        Vector2 scale = Vector2.One * NPC.scale;
        scale.X *= ExtraMath.Osc(1.1f, 0.9f, speed: 2);
        scale.Y *= ExtraMath.Osc(0.9f, 1.1f, speed: 2);
        scale *= _squishScale;
        spriteBatch.Draw(texture, drawCenter, NPC.frame, drawColor, rotation, drawOrigin, scale, spriteEffects, 0);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawSprite(spriteBatch, screenPos, drawColor);
        return false;
    }
    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        Vector2 v = Vector2.UnitY * 2;
        Vector2 h = Vector2.UnitX * 2;
        DrawSprite(spriteBatch, screenPos + v, _outlineColor);
        DrawSprite(spriteBatch, screenPos - v, _outlineColor);
        DrawSprite(spriteBatch, screenPos + h, _outlineColor);
        DrawSprite(spriteBatch, screenPos - h, _outlineColor);
    }

}
