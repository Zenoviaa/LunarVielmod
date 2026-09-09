
using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Content.Areas.Cinderspark.BossesCS.Rek;
using Stellamod.Core;
using Stellamod.Core.NPCHelpers;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;

public class AbyssLittleMoth : ModNPC,
    IWaterSilhouette
{
    private float _alpha;
    private bool _spawned;
    private Vector2 _wanderPos;
    private float Glow => ExtraMath.Osc(0.1f, 0.7f, offset: NPC.whoAmI);
    private ref float Timer => ref NPC.ai[0];
    private ref float WanderTimer => ref NPC.ai[1];
    private ref float Style => ref NPC.ai[2];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
        NPCSets.Heavy[Type] = true;
        this.AddToAbyssCritter();
        this.PreferLand();
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_wanderPos);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _wanderPos = reader.ReadVector2();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = NPC.height = 32;
        NPC.lifeMax = 32;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = AssetReferences.Assets.Sounds.Abyss.MothFeatherDeath.Asset with { PitchVariance = 0.3f };
        NPC.aiStyle = -1;
        NPC.noGravity = true;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    private void FaceMovement()
    {
        if (NPC.velocity.X < 0)
            NPC.spriteDirection = -1;
        else
            NPC.spriteDirection = 1;
    }

    private void NewWanderPos()
    {
        _wanderPos = NPC.Center;
        _wanderPos.X += Main.rand.Next(-128, 128);
        _wanderPos.Y += Main.rand.Next(-8, 8);
    }

    public override void AI()
    {
        NPCSets.Heavy[Type] = true;
        base.AI();
        if(Style == 0)
        {
            _alpha = 1f;
            Timer++;
            Vector2 targetPos = _wanderPos;
            targetPos.X += MathF.Sin(Timer * 0.005f) * 9;
            targetPos.Y += MathF.Sin(Timer * 0.01f) * 9;
            Vector2 targetVelocity = targetPos - NPC.Center;
            targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
            float speed = 1f;
            targetVelocity *= speed;
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.03f);
            FaceMovement();
            WanderTimer--;
            if ((WanderTimer <= 0 || Vector2.DistanceSquared(NPC.Center, targetPos) < 64) && MultiplayerHelper.IsHost)
            {
                NewWanderPos();
                WanderTimer = 120;
                NPC.netUpdate = true;
            }

        }
        else
        {
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.dontTakeDamageFromHostiles = true;
            Timer++;
            if(!_spawned && MultiplayerHelper.IsHost)
            {
                Vector2 upwardVelocity = -Vector2.UnitY;
                upwardVelocity = upwardVelocity.RotatedBy(MathHelper.Lerp(-1f, 1f, (Style - 1) / (float)BellFlowerSystem.MaxBellFlowers));
                NPC.velocity = upwardVelocity * 8;
                NPC.netUpdate = true;
                _spawned = true;
            }
            if(Timer < 0)
            {
                _alpha = 1f;
                NPC.velocity *= 0.96f;
                FaceMovement();
            }
            else
            {
                _alpha = MathHelper.Lerp(1f, 0f, Timer / 100f);
                int index = (int)(Style - 1);
                Vector2 target = BellFlowerSystem.BellFlowers[index].spawnPosition;
                Vector2 movementVelocity = (target - NPC.Center).SafeNormalize(Vector2.Zero);
                movementVelocity *= 16;
                NPC.velocity = Vector2.Lerp(NPC.velocity, movementVelocity, 0.03f);
                FaceMovement();
                if(Timer % 8 == 0)
                {
                    var sp = SparkleParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(72, 72), Vector2.Zero);
                    sp.gravity = 0;
                    sp.Scale *= 0.5f;
                    sp.fast = true;
                    sp.outerColor = Color.Blue;
                }
                if (Timer >= 100)
                {
                    NPC.active = false;
                }
            }
        
        }

        this.AseAnimator.PlayAnimation("Idle", AnimationParams.Default);
        this.AseAnimator.drawEffects.DrawOrigin = new Vector2(28, 28);
        Lighting.AddLight(NPC.Center, Vector3.One * 0.2f);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (Style != 0)
            drawColor.A = 0;
        NPC.DrawAnimator(spriteBatch, drawColor * _alpha);



        Color glowColor = Color.White * Glow;
        glowColor.A = 0;
        NPC.DrawAnimator(spriteBatch, glowColor * _alpha);
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(spriteBatch, screenPos, drawColor);
        Texture2D glowCircle = AssetManager.GlowMask.SimpleGlowCircle.Value;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(glowCircle, NPC.Center);
        drawer.color = Color.White * Glow * 0.2f * _alpha;
        drawer.color.A = 0;
        drawer.scale *= 0.5f;
        spriteBatch.Draw(drawer);

        if (Style == 0)
            return;
        drawer.color *= 2;
        spriteBatch.Draw(drawer);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        AbyssEnemyCommon.HitAndDeathEffects(NPC);
    }
    public void PrepareSilhouetteDrawing(RekSilhouetteSystem system)
    {
        void DrawWhite(SpriteBatch spriteBatch)
        {
            NPC.DrawAnimator(spriteBatch, Color.Black * _alpha);
        }
        system.SilhouettesToDraw.Add(DrawWhite);
    }
}
