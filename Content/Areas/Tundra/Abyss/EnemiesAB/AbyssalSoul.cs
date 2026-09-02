
using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Core.NPCHelpers;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;

public class AbyssalSoul : ModNPC
{
    private Vector2 _wanderPos;
    private ref float Timer => ref NPC.ai[0];
    private ref float WanderTimer => ref NPC.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
        this.AddToAbyss();
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
        NPC.HitSound = SoundID.NPCHit38;
        NPC.DeathSound = SoundID.NPCDeath41;
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
        _wanderPos.X += Main.rand.Next(-32, 32);
        _wanderPos.Y += Main.rand.Next(-8, 8);
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Vector2 targetPos = _wanderPos;
        targetPos.X += MathF.Sin(Timer * 0.005f) * 32;
        targetPos.Y += MathF.Sin(Timer * 0.01f) * 16;
        Vector2 targetVelocity = targetPos - NPC.Center;
        targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
        float speed = 4f;
        targetVelocity *= speed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.03f);
        FaceMovement();
        WanderTimer--;
        if (WanderTimer <= 0 && MultiplayerHelper.IsHost)
        {
            NewWanderPos();
            WanderTimer = 240;
            NPC.netUpdate = true;
        }

        this.AseAnimator.PlayAnimation("Idle", AnimationParams.Default);
        this.AseAnimator.drawEffects.DrawOrigin = new Vector2(18, 38);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(spriteBatch, screenPos, drawColor);
        Texture2D glowCircle = AssetManager.GlowMask.SimpleGlowCircle.Value;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(glowCircle, NPC.Center);
        drawer.color = Color.OrangeRed * ExtraMath.Osc(0.5f, 1f, speed: 3) * 0.2f;
        drawer.color.A = 0;
        drawer.scale *= 0.5f;
        spriteBatch.Draw(drawer);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        AbyssEnemyCommon.HitAndDeathEffects(NPC);
    }
}