using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Common.Shaders;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;

public class BlindMothOrb : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 180;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            SoundStyle softSummon = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2");
            softSummon.PitchVariance = 0.3f;
            SoundEngine.PlaySound(softSummon, Projectile.position);
        }
        if (Timer < 60)
        {
            Projectile.velocity.X *= 0.95f;
            Projectile.velocity.Y *= 0.99f;
        }
        else if (Timer < 90)
        {
            Projectile.velocity *= 0.5f;
        }
        else if (Timer == 91)
        {
            SoundStyle castSound = new SoundStyle("Stellamod/Assets/Sounds/Frosty");
            castSound.PitchVariance = 0.3f;
            castSound.Volume = 0.5f;
            SoundEngine.PlaySound(castSound, Projectile.position);
            Projectile.velocity = -Vector2.UnitY;
            Player closestPlayer = PlayerHelper.FindClosestPlayer(Projectile.Center, 2000);
            if (closestPlayer != null)
                Projectile.velocity = (closestPlayer.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
        }
        else
        {
            if(Timer < 200)
            {
                Vector2 targetPosition = Projectile.Center;
                Player closestPlayer = PlayerHelper.FindClosestPlayer(Projectile.Center, 2000);
                if (closestPlayer != null)
                    targetPosition = closestPlayer.Center;

                Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, targetPosition, degreesToRotate: 1.5f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, homingVelocity, 0.2f);
                if (Projectile.velocity.Length() < 10)
                {
                    Projectile.velocity *= 1.1f;
                }
            }

        }

        if (Timer % 8 == 0)
        {
            SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, Vector2.Zero, Color.White, Scale: 0.4f);
            sp.gravity = 0;
            sp.fast = true;
        }
    }

    private void DrawSparklePixelated(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Texture2D textureToDraw = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = textureToDraw.Size() * 0.5f;
        Color drawCoolr = Color.White;
        drawCoolr *= ExtraMath.Osc(0.5f, 1f, speed: 8, offset: Projectile.whoAmI);
        drawCoolr.A = 0;


        //Draw trail
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 oldPosition = Projectile.oldPos[i];
            Vector2 oldCenter = oldPosition + Projectile.Size * 0.5f;
            float ease = (float)i / (float)Projectile.oldPos.Length;
            float alpha = MathHelper.SmoothStep(1f, 0f, ease);
            spriteBatch.Draw(textureToDraw, oldCenter - screenPos, null, drawCoolr * alpha, 0, drawOrigin, 1f, SpriteEffects.None, 0);
        }
        spriteBatch.Draw(textureToDraw, Projectile.Center - screenPos, null, drawCoolr, 0, drawOrigin, 1f, SpriteEffects.None, 0);
        spriteBatch.Draw(textureToDraw, Projectile.Center - screenPos, null, drawCoolr, 0, drawOrigin, 1f, SpriteEffects.None, 0); ;
    //    spriteBatch.Draw(textureToDraw, Projectile.Center - screenPos, null, drawCoolr, 0, drawOrigin, 1f, SpriteEffects.None, 0); ;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PixelationManager.QueueSpritebatchDrawAction(DrawSparklePixelated);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (float f = 0; f < 3; f++)
        {
            DustParticle dp = Particle<DustParticle>.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(0.3f), Color.White, Scale: 0.5f);
            dp.dampening = 0.1f;
            dp.outerColor = Color.SkyBlue;
            dp.Scale *= Projectile.scale;
        }
        for (float i = 0; i < 4; i++)
        {
            float progress = i / 4f;
            float rot = progress * MathHelper.ToRadians(360);
            rot += Main.rand.NextFloat(-0.5f, 0.5f);
            Vector2 offset = rot.ToRotationVector2() * 24;
            var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightBlue,
                outerGlowColor: Color.DarkBlue,
                baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                duration: Main.rand.NextFloat(15, 25));
            particle.Rotation = rot + MathHelper.ToRadians(45);
            particle.Scale *= 0.5f * Projectile.scale;
        }
        FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Black,
                outerGlowColor: Color.Black, duration: 25, baseSize: 0.12f);
    }
}
public class BlindMoth : ModNPC,
    IDrawOutlines
{
    private int _frame;
    private enum AIState : byte
    {
        Idle,
        Fly_Around,
        Chase,
        Summon_Homing_White_Orbs,
        Charge_Attack
    }

    private AIState State
    {
        get => (AIState)NPC.ai[0];
        set => NPC.ai[0] = (float)value;
    }
    private ref float Timer => ref NPC.ai[1];
    private ref float AttackCycle => ref NPC.ai[2];
    private float _dashLineRotation;
    private float _dashLineAlpha;
    private bool _pause;
    private Vector2 _targetWanderDirection;
    private Vector2 _wanderDirection;
    private Vector2 _dashVelocity;
    private Color _targetOutlineColor;
    private Color _outlineColor;
    private bool _contactDamage;
    private bool _followCharge;
    private Player PlayerTarget => Main.player[NPC.target];
    private int BlindMothOrbDamage => 20;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_targetWanderDirection);
        writer.WriteVector2(_wanderDirection);
        writer.WriteVector2(_dashVelocity);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _targetWanderDirection = reader.ReadVector2();
        _wanderDirection = reader.ReadVector2();
        _dashVelocity = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 10;
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCSets.Heavy[Type] = true;
        this.AddToAbyss();
        SpawnSets.ModifiedWeights[Type] = 0.1f;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 64;
        NPC.height = 64;
  
        NPC.damage = 32;
        NPC.defense = 8;
        NPC.lifeMax = 250;
        NPC.HitSound = SoundID.NPCHit16;
        NPC.DeathSound = SoundID.NPCDeath46;
        NPC.value = Item.buyPrice(silver: 50);
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.npcSlots = 10f;
        NPC.noTileCollide = true;
      
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        if (_pause)
            return;
        NPC.frameCounter += 0.2f;
        if (NPC.frameCounter >= 1f)
        {
            _frame++;
            NPC.frameCounter = 0f;
        }
        _frame %= Main.npcFrameCount[Type];
        NPC.frame.Y = frameHeight * _frame;
    }



    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        AbyssEnemyCommon.HitAndDeathEffects(NPC);
        if(NPC.life <= 0)
        {
            PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.White, Color.SkyBlue, 30, 128);
        }
    }

    public override void AI()
    {
        base.AI();
        _targetOutlineColor = Color.Transparent;
        _pause = false;
        if(Timer % 60 == 0)
        {
            SoundEngine.PlaySound(SoundRegistry.Niivi_WingFlap, NPC.position);
        }
        _contactDamage = false;
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Fly_Around:
                AI_FlyAround();
                break;
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.Summon_Homing_White_Orbs:
                AI_SummonHomingWhiteOrbs();
                break;
            case AIState.Charge_Attack:
                AI_ChargeAttack();
                break;
        }
        if (Main.rand.NextBool(16))
        {
            Vector2 spawnPosition = new Vector2();
            spawnPosition.X = NPC.position.X + Main.rand.Next(0, NPC.width);
            spawnPosition.Y = NPC.position.Y + Main.rand.Next(0, NPC.height);
            SparkleParticle.Spawn(spawnPosition, Vector2.Zero, Scale: 0.3f);
        }
        _outlineColor = Color.Lerp(_outlineColor, _targetOutlineColor, 0.1f);
        Lighting.AddLight(NPC.position, Color.White.ToVector3() * 0.5f);
        LerpToXVelocityRotation();
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

    private void LerpToXVelocityRotation()
    {
        float targetRotation = NPC.velocity.X * 0.05f;
        NPC.rotation = MathHelper.Lerp(NPC.rotation, targetRotation, 0.1f);
    }

    private void AI_Idle()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }

        NPC.velocity.X *= 0.98f;
        float targetYSpeed = MathHelper.Lerp(-1f, 1f, ExtraMath.Osc(0f, 1f));
        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, targetYSpeed, 0.1f);

        float distanceToPlayerTarget = Vector2.Distance(NPC.Center, PlayerTarget.Center);
        if (Timer >= 120)
        {
            SwitchState(AIState.Fly_Around);
        }
        else if (distanceToPlayerTarget < 500)
        {
            SwitchState(AIState.Chase);
        }

    }

    private void AI_FlyAround()
    {
        Timer++;
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                _targetWanderDirection = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                _targetWanderDirection = _targetWanderDirection.SafeNormalize(Vector2.Zero);
                NPC.netUpdate = true;
            }

        }

        _wanderDirection = Vector2.Lerp(_wanderDirection, _targetWanderDirection, 0.1f);
        Vector2 wanderVelocity = _wanderDirection * 0.5f;
        Vector2 lerpedVelocity = Vector2.Lerp(NPC.velocity, wanderVelocity, 0.1f);
        NPC.velocity = lerpedVelocity;
        NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;

        float distanceToPlayerTarget = Vector2.Distance(NPC.Center, PlayerTarget.Center);
        if (distanceToPlayerTarget < 500)
        {
            SwitchState(AIState.Chase);
        }
        else if (Timer > 240)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_Chase()
    {
        Timer++;
        Vector2 directionToPlayer = (PlayerTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        Vector2 chaseVelocity = directionToPlayer * 5;
        NPC.velocity = Vector2.Lerp(NPC.velocity, chaseVelocity, 0.1f);
        NPC.spriteDirection = NPC.velocity.X > 0 ? 1 : -1;

        if (!NPC.HasValidTarget)
            SwitchState(AIState.Fly_Around);
        float distanceToPlayerTarget = Vector2.Distance(NPC.Center, PlayerTarget.Center);
        if (distanceToPlayerTarget < 250 && Timer > 30)
        {

            if(AttackCycle == 0)
            {
                SwitchState(AIState.Summon_Homing_White_Orbs);
            } else if (AttackCycle == 1)
            {
                SwitchState(AIState.Charge_Attack);
            }
           // SwitchState(AIState.Charge_Attack);
            AttackCycle++;
            AttackCycle %= 2;
 
        }
    }

    private void AI_SummonHomingWhiteOrbs()
    {
        Timer++;
        if (Timer < 60)
        {
            _pause = true;
            _targetOutlineColor = Color.Yellow;
        }
        else
        {
            NPC.spriteDirection = PlayerTarget.Center.X > NPC.Center.X ? 1 : -1;
            _targetOutlineColor = Color.Red;
        }

        NPC.velocity *= 0.95f;

        float targetYSpeed = MathHelper.Lerp(-1f, 1f, ExtraMath.Osc(0f, 1f));
        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, targetYSpeed, 0.1f);

        if (Timer > 90 && Timer < 240)
        {
            if (Timer % 30 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 initialVelocity = -Vector2.UnitY * Main.rand.NextFloat(3f, 6f);
                    initialVelocity = initialVelocity.RotatedByRandom(MathHelper.ToRadians(45));
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, initialVelocity,
                        ModContent.ProjectileType<BlindMothOrb>(), BlindMothOrbDamage, 1, Main.myPlayer);
                }
            }
        }

        if (Timer > 360)
        {
            SwitchState(AIState.Chase);
        }
    }

    private void AI_ChargeAttack()
    {
        Timer++;
        if(Timer == 1)
        {
            _followCharge = true;
        }
        if (Timer < 120)
        {
            //Line up on y axis
            float targetY = PlayerTarget.Center.Y - NPC.Center.Y;
            float yDist = MathF.Abs(targetY);
            if (yDist == 0)
                yDist += 1;
            float yNormal = targetY / yDist;
            float ySpeed = 8;
            if (yDist < 8)
                ySpeed = yDist;

            if (!_followCharge)
            {
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 0, 0.03f);
            }
            if (_followCharge)
            {
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, ySpeed * yNormal, 0.05f);
            }
      
            if (yDist < 8)
            {
                _followCharge = false;
            } else if (yDist > 24)
            {
                _followCharge = true;
            }

            NPC.velocity.X *= 0.98f;

            float xDiff = (PlayerTarget.Center.X - NPC.Center.X);
            float xDist = MathF.Abs(xDiff);
            float xNormal = xDiff / xDist;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, xNormal, 0.1f);

            float xDir = PlayerTarget.Center.X > NPC.Center.X ? 1 : -1;
            if (xDist < 16)
            {
                NPC.velocity.X -= xDir;
            }
            _targetOutlineColor = Color.Yellow;
            NPC.spriteDirection = PlayerTarget.Center.X > NPC.Center.X ? 1 : -1;
            Vector2 targetVelocity = PlayerTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
            _dashLineRotation = targetVelocity.ToRotation();
            _dashLineAlpha = MathHelper.Lerp(_dashLineAlpha, 1f, 0.1f);
        }
        else if (Timer < 180)
        {
            NPC.spriteDirection = PlayerTarget.Center.X > NPC.Center.X ? 1 : -1;
            Vector2 targetVelocity = PlayerTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
            targetVelocity *= 15;
            _dashVelocity = targetVelocity;
            _dashLineAlpha *= 0.9f;

            float xDir = PlayerTarget.Center.X > NPC.Center.X ? 1 : -1;
            if(Timer < 210)
            {
                NPC.velocity.Y *= 0.8f;
                NPC.velocity.X -= xDir * 0.08f;
            }
            else
            {

                NPC.velocity = Vector2.Lerp(NPC.velocity, _dashVelocity, 0.05f);
            }


            if(Timer == 139)
            {
                SoundStyle chargeSound = AssetRegistry.Sounds.SanguineSingularity.SanguineCharge;
                SoundEngine.PlaySound(chargeSound, NPC.position);
            }
        }
        else if (Timer < 220)
        {
            _dashLineAlpha = 0f;

            NPC.velocity = Vector2.Lerp(NPC.velocity, _dashVelocity * 2, MathHelper.Lerp(0.02f, 0.14f, EasingFunction.InOutSine((Timer - 180) / 40f)));
            _contactDamage = true;
            _targetOutlineColor = Color.Red;
        }
        else if (Timer < 280)
        {
            NPC.velocity *= 0.96f;
            float targetYSpeed = MathHelper.Lerp(-1f, 1f, ExtraMath.Osc(0f, 1f));
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, targetYSpeed, 0.1f);
        }
        else
        {
            SwitchState(AIState.Chase);
        }
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawPos = NPC.Center - Main.screenPosition;
        Vector2 drawOrigin = NPC.frame.Size() / 2f;
        float drawRotation = NPC.rotation;
        float drawScale = NPC.scale;
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


        float outlineOffset = 2;
        Vector2 left = drawPos + Vector2.UnitX * -outlineOffset;
        Vector2 right = drawPos + Vector2.UnitX * outlineOffset;
        Vector2 up = drawPos + Vector2.UnitY * -outlineOffset;
        Vector2 down = drawPos + Vector2.UnitY * outlineOffset;
        Color outlineColor = _outlineColor;

        spriteBatch.Draw(texture, left, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        spriteBatch.Draw(texture, right, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        spriteBatch.Draw(texture, up, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        spriteBatch.Draw(texture, down, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
    }

    private void DrawDashLine(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (State != AIState.Charge_Attack)
            return;
        float rotation = _dashLineRotation;
        Texture2D lineTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
        Vector2 drawOrigin = new Vector2(lineTexture.Width / 2, 0);
        Vector2 drawCenter = NPC.Center - Main.screenPosition;
        drawColor = Color.Red;
        drawColor.A = 0;
        drawColor *= 0.5f;
        drawColor *= Timer / 30f;
        drawColor *= ExtraMath.Osc(0f, 1f, speed: 12);
        drawColor *= _dashLineAlpha;

        Vector2 scale = Vector2.One;
        scale.Y = 3;
        scale.X *= 0.5f;
        spriteBatch.Draw(lineTexture, drawCenter, null, drawColor, rotation - MathHelper.ToRadians(90), drawOrigin, scale, SpriteEffects.None, 0);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D glowCircle = AssetManager.GlowMask.SimpleGlowCircle.Value;
        Vector2 glowCircleDrawOrigin = glowCircle.Size() * 0.5f;
   
       // DrawDashLine(spriteBatch, screenPos, drawColor);
        Texture2D npcTexture = TextureAssets.Npc[Type].Value;
        Vector2 drawOrigin = NPC.frame.Size() * 0.5f;
        Vector2 drawCenter = NPC.Center - screenPos;
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            Vector2 oldPosition = NPC.oldPos[i];
            Vector2 oldCenter = oldPosition + NPC.Size * 0.5f;
            Vector2 oldCenterDraw = oldCenter - screenPos;

            float ease = (float)i / (float)NPC.oldPos.Length;
            float alpha = MathHelper.SmoothStep(1f, 0f, ease);
            spriteBatch.Draw(npcTexture, oldCenterDraw, NPC.frame, drawColor * alpha * 0.25f, NPC.oldRot[i], drawOrigin, NPC.scale, spriteEffects, 0);
        }
        spriteBatch.Draw(npcTexture, drawCenter, NPC.frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);

        drawColor *= ExtraMath.Osc(0.15f, 0.5f, offset: NPC.whoAmI);
        drawColor.A = 0;
        spriteBatch.Draw(npcTexture, drawCenter, NPC.frame, drawColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0);

        spriteBatch.Draw(glowCircle, drawCenter, null, drawColor, NPC.rotation, glowCircleDrawOrigin, NPC.scale * 0.5f, spriteEffects, 0);

        return false;
    }
}
