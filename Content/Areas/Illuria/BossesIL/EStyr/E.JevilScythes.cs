using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr;

public class BlackSwordGeyser : ModProjectile,
    IDrawBlackStar
{
    private float _inScale;
    private float _outScale;
    private Vector2[] LinePos = new Vector2[4];
    private TexturedQuad _quadBackingField;
    private TexturedQuad TexturedQuad
    {
        get
        {
            if (_quadBackingField == null)
                _quadBackingField = new TexturedQuad();
            return _quadBackingField;
        }
    }
    private ref float Timer => ref Projectile.ai[0];
    private ref float SpeedUp => ref Projectile.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 8000;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 20;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float hitboxWidth = MathHelper.Lerp(64, 32, SpeedUp);
        return ProjectileHelper.OldPosColliding(LinePos, projHitbox, targetHitbox, hitboxWidth);
    }
    public override bool CanHitPlayer(Player target)
    {
        return base.CanHitPlayer(target) && Timer < 5;
    }

    public override void AI()
    {
        base.AI();
     
        Timer++;
        if (Timer == 1)
        {
            SoundStyle hurriboom = AssetRegistry.Sounds.E.Hurriboom;
            hurriboom.PitchVariance = 0.3f;
            SoundEngine.PlaySound(hurriboom);
        }

        _inScale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 5f));
        _outScale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine((float)Projectile.timeLeft / 15f));
    
        LinePos[0] = Projectile.Center;
        LinePos[1] = Projectile.Center;
        LinePos[2] = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 8000;
        LinePos[3] = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 8000;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    private float GetWidth()
    {
        return MathHelper.Lerp(200, 100, SpeedUp);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
        flamingTrailShader.OuterColor = Color.White;
        flamingTrailShader.InnerColor = Color.White;
        flamingTrailShader.Power = 0.3f;
        flamingTrailShader.Distortion = 6;
        flamingTrailShader.Tiling = new Vector2(1, 3);
        flamingTrailShader.BlendState = BlendState.Additive;
        flamingTrailShader.PrimaryTexture = TrailRegistry.BeamTrail;

        float smooth = _inScale * _outScale;

        float width = MathHelper.SmoothStep(0f, GetWidth(), smooth);
        TexturedQuad.CalculateVertices(Projectile.Center, Projectile.velocity,
            8000, width);
        TexturedQuad.DrawWithShader(flamingTrailShader);
        BlackStarRenderer.QueueBlackStarDraw(this);
        return false;
    }

    public void DrawBlackStar(SpriteBatch spriteBatch)
    {
        
        FlamingTrailShader flamingTrailShader = FlamingTrailShader.Instance;
        flamingTrailShader.OuterColor = Color.White;
        flamingTrailShader.InnerColor = Color.White;
        flamingTrailShader.Power = 0.3f;
        flamingTrailShader.Distortion = 6;
        flamingTrailShader.Tiling = new Vector2(1, 3);
        flamingTrailShader.BlendState = BlendState.Additive;

        float smooth = _inScale * _outScale;
        float width = MathHelper.SmoothStep(0f, GetWidth() + 50, smooth);
        TexturedQuad.CalculateVertices(Projectile.Center, Projectile.velocity,
            8000, width);
        TexturedQuad.DrawWithShader(flamingTrailShader);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        BlackStars.AddBuff(target, 75);
    }
}

public class BlackSword : ScarletProjectile,
     IDrawOutlines,
     IDrawBlackStar
{
    private float _traveledDistance;
    private ref float Timer => ref Projectile.ai[0];
    private ref float SpeedUp => ref Projectile.ai[1];
    private bool FirstOne => Projectile.ai[2] == 1;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        TrailCacheLength = 16;
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.hostile = true;
        Projectile.timeLeft = 360;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.extraUpdates = 2;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        BlackStars.AddBuff(target, 65);
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            ShakeScreenPosition.Shake = 6;
            FXUtil.ShakeCamera(Projectile.position, 1024, 4);

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 startPosition = Projectile.Center - direction * 1200;

            for (float i = 0; i < 3; i++)
            {
                var donutParticle = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -direction * MathHelper.Lerp(15, 1f, i / 3f));
                donutParticle.Scale *= MathHelper.Lerp(1f, 3f, i / 3f);
            }

            var strike = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, direction);
            strike.xMult = 6;
            strike.rotOffset += MathHelper.PiOver2;

            var strike2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, direction);
            strike2.xMult = 32;
            strike2.rotOffset += MathHelper.PiOver2;

            SoundStyle hurriSlash = AssetRegistry.Sounds.E.Hurrislash;
            hurriSlash.PitchVariance = 0.3f;
            SoundEngine.PlaySound(hurriSlash, Projectile.position);
        }

        float outScale = (float)Projectile.timeLeft / 30f;
        outScale = EasingFunction.InOutSine(outScale);
        Projectile.scale = 1f * outScale;
        if (Timer >= 30)
        {
            float speed = MathHelper.Lerp(20, 30, SpeedUp);
            if (FirstOne)
            {
                if (Projectile.velocity.Length() < speed)
                    Projectile.velocity *= 1.0365f;
            }
            else
            {
                if (Projectile.velocity.Length() < speed)
                    Projectile.velocity *= 1.0365f;
            }
           
        }

        if(Timer % 5 == 0)
        {
            LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity, Color.White);
        }

        if(Timer > 5)
        {
            float distance = Vector2.Distance(Projectile.position, Projectile.oldPosition);
            _traveledDistance += distance;
            if (_traveledDistance >= 2000)
            {
                Projectile.Kill();
            }
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Timer % 32 == 0)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), Scale: 0.4f);
        }
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 1000, -Projectile.velocity.SafeNormalize(Vector2.Zero) * 2000, 
            ModContent.ProjectileType<BlackSwordGeyser>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: SpeedUp);
    }

    private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawOrigin = texture.Size() / 2f;
        Vector2 drawCenter = Projectile.Center - screenPos;
        float rotation = Projectile.rotation;

        Vector2 drawScale = Vector2.One * Projectile.scale;
        spriteBatch.Draw(texture, drawCenter, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
    }

    private void DrawAfterImages(SpriteBatch spriteBatch)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawOrigin = texture.Size() / 2f;
        for (int i = 0; i < TrailCacheLength; i++)
        {
            float completionRatio = (float)i / (float)TrailCacheLength;

            Vector2 drawCenter = OldCenterPos[i] - Main.screenPosition;
            float rotation = OldCenterRot[i];

            Color drawColor = Color.Lerp(Color.White, Color.Transparent, completionRatio);
            drawColor *= 0.3f;
            Vector2 drawScale = Vector2.One * Projectile.scale;
            spriteBatch.Draw(texture, drawCenter, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        DrawSprite(Main.spriteBatch, Main.screenPosition, Color.White);
        BlackStarRenderer.QueueBlackStarDraw(this);
        return false;
    }

    public void DrawBlackStar(SpriteBatch spriteBatch)
    {
        DrawAfterImages(spriteBatch);
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        float outlineOffset = 2;
        Vector2 v = Vector2.UnitY * outlineOffset;
        Vector2 h = Vector2.UnitX * outlineOffset;
        DrawSprite(spriteBatch, Main.screenPosition + v, Color.White);
        DrawSprite(spriteBatch, Main.screenPosition - v, Color.White);
        DrawSprite(spriteBatch, Main.screenPosition + h, Color.White);
        DrawSprite(spriteBatch, Main.screenPosition - h, Color.White);
    }
}


public partial class E
{
    private int ScytheDamage => 30;
    private float ScytheSpeedUpRatio => _attackNumber / ScytheAttackNumber;
    private float ScytheAttackNumber => 32f;
    /*
     * 
     * Sideways jevil special, without the big scythe at the end y'know, 
     * and they explosions hurt you obv
     */
    private void AI_JevilScythesStart()
    {
        Timer++;
        if(Timer == 1)
        {
            NPC.TargetClosest();
            float offsetDirection = MyTarget.Center.X > NPC.Center.X ? 1 : -1;

            _forwardVector = NPC.velocity;
            TargetVector = offsetDirection * Vector2.UnitX * 300 + new Vector2(0, -64);
        }

        //What we want to is he'll get on one of side of you
        float startupTime = 60f;
        float completionRatio = Timer / startupTime;
        float ease = EasingFunction.InOutExpo7(completionRatio);
        Vector2 start = MyTarget.Center;
        Vector2 end = start + TargetVector;
        Vector2 positionToMoveTo = Vector2.Lerp(start, end, ease);
        Vector2 targetVelocity = positionToMoveTo - NPC.Center;
        Vector2 easeVelocity = Vector2.Lerp(_forwardVector, targetVelocity, ease);
        NPC.velocity = easeVelocity;
        NPC.direction = TargetDirection;

        Animator.PlayAnimation(Anim_BattleIdle);
        if (Timer >= startupTime)
        {
            SwitchState(AIState.JevilScythes_Prepare);
        }
    }

    private void AI_JevilScythesPrepare()
    {
        Timer++;
        NPC.velocity *= 0.9f;
        float preparationTime = MathHelper.Lerp(30, 15, ScytheSpeedUpRatio);
        if(_attackNumber >= ScytheAttackNumber / 2f)
        {
            preparationTime *= 0.5f;
        }

        if(Timer >= preparationTime)
        {
            SwitchState(AIState.JevilScythes_Loop);
        }
    }

    private void AI_JevilScythesLoop()
    {
        Timer++;
        if(Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                Vector2 spawnPosition = MyTarget.Center;
                spawnPosition.X = NPC.Center.X;
                spawnPosition.Y += Main.rand.NextFloat(-32f, 32f);
                Vector2 fireVelocity = Vector2.UnitX * NPC.direction;
                fireVelocity *= 4;
                bool isMegaSlow = _attackNumber == 0;
                if (isMegaSlow)
                {
                    fireVelocity *= 0.1f;
                }

                spawnPosition += -fireVelocity * 100;

            
                Projectile.NewProjectile(SourceFromThis, spawnPosition, fireVelocity, 
                    ModContent.ProjectileType<BlackSword>(), ScytheDamage, 1, Main.myPlayer, ai1: ScytheSpeedUpRatio, ai2: isMegaSlow ? 1 : 0);
            }
        }

        float swingTime = 30f;
        swingTime = MathHelper.Lerp(30F, 15, ScytheSpeedUpRatio);
        if(_attackNumber == 0)
        {
            swingTime *= 2.3f;
        }
        if (_attackNumber >= ScytheAttackNumber / 2f)
        {
            swingTime *= 0.5f;
        }
        if (_attackNumber % 2 == 0)
        {
            Animator.PlayAnimation(Anim_ForwardSlash);
        }
        else
        {
            Animator.PlayAnimation(Anim_BackSlash);
        }

        Vector2 positionToMoveTo = MyTarget.Center + TargetVector;
        Vector2 targetVelocity = positionToMoveTo - NPC.Center;
        NPC.direction = TargetDirection;
        NPC.velocity = targetVelocity;

    
        if(Timer >= swingTime)
        {
            SwitchState(AIState.JevilScythes_End);
        }
    }

    private void AI_JevilScythesEnd()
    {
        Timer++;
        NPC.velocity *= 0.9f;
        if(Timer >= 5)
        {
            _attackNumber++;
            if(_attackNumber >= ScytheAttackNumber)
            {
                _attackNumber = 0;
                SwitchState(AIState.JevilScythes_Quick);
            }
            else
            {
                SwitchState(AIState.JevilScythes_Prepare);
            }
        }
    }
    private void AI_JevilScythesQuick()
    {
        Timer++;
        if(Timer == 1)
        {
            if(_attackNumber == 0)
            {
                TargetVector = NPC.velocity;
            }
            else
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 spawnPosition = MyTarget.Center;
                    spawnPosition.X = NPC.Center.X;
                    spawnPosition.Y += Main.rand.NextFloat(-32f, 32f);
                    Vector2 fireVelocity = Vector2.UnitX * NPC.direction;
                    fireVelocity *= 4;

                    spawnPosition += -fireVelocity * 100;
                    Projectile.NewProjectile(SourceFromThis, spawnPosition, fireVelocity,
                        ModContent.ProjectileType<BlackSword>(), ScytheDamage, 1, Main.myPlayer, ai1: ScytheSpeedUpRatio);
                }
            }
        }

        if(_attackNumber > 0)
        {
            if (_attackNumber % 2 == 0)
            {
                Animator.PlayAnimation(Anim_ForwardSlash);
            }
            else
            {
                Animator.PlayAnimation(Anim_BackSlash);
            }
        }
        else
        {
            Animator.PlayAnimation(Anim_Holding);
        }


           
        NPC.velocity *= 0.9f;
        float swingTime = 6;
        if(_attackNumber == 0)
        {
            swingTime *= 10;

            if (Timer == 1)
            {
                NPC.TargetClosest();
                float offsetDirection = MyTarget.Center.X > NPC.Center.X ? 1 : -1;

                _forwardVector = NPC.velocity;
                TargetVector = -offsetDirection * Vector2.UnitX * 300 + new Vector2(0, -64);
            }

            //What we want to is he'll get on one of side of you
            float startupTime = 60f;
            float cRatio = Timer / startupTime;
            float ease = EasingFunction.InOutExpo7(cRatio);
            Vector2 start = MyTarget.Center;
            Vector2 end = start + TargetVector;
            Vector2 positionToMoveTo = Vector2.Lerp(start, end, ease);
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            Vector2 easeVelocity = Vector2.Lerp(_forwardVector, targetVelocity, ease);
            NPC.velocity = easeVelocity;
        }

        if (Timer >= swingTime)
        {
            _attackNumber++;

            if (_attackNumber >= 4)
            {
                SwitchState(AIState.Idle);
            }
            else
            {
                SwitchState(AIState.JevilScythes_Quick);

            }
        }
    }
}
