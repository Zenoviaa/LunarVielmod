using Stellamod.Common.Particles;
using Stellamod.Core.NPCHelpers;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.NPCsUG;

public class LilFirefly : ModNPC
{
    Vector2 _home;
    Vector2 _posToMoveTo;
    ref float Timer => ref NPC.ai[0];
    const string ANIM_FLY = "Fly";
    float HomeRange => 128;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.UsesAseprite[Type] = true;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.TrailCacheLength[Type] = 16;
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_home);
        writer.WriteVector2(_posToMoveTo);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _home = reader.ReadVector2();
        _posToMoveTo = reader.ReadVector2();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 80;
        NPC.height = 40;
        NPC.lifeMax = 50;
        NPC.defense = 3;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.noGravity = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (_home == default(Vector2))
        {
            _home = NPC.Center;
            _posToMoveTo = _home;
            NPC.netUpdate = true;
        }

        var targetVelocity = (_posToMoveTo - NPC.Center) * 0.2f;
        NPC.velocity = targetVelocity;
        NPC.PlayAnimationLooped(ANIM_FLY);
        if (MultiplayerHelper.IsHost && Main.rand.NextBool(128))
        {
            _posToMoveTo = _home + Main.rand.NextVector2Circular(HomeRange, HomeRange);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    void DeathFX()
    {
        FXUtil.GlowCircleBoom(NPC.Center, Color.Gold, Color.DarkOrange, Color.Black, duration: 25, baseSize: 0.18f);
        foreach (PositionVelocity posVel in new CircleOrientation(NPC.Center, 32, 16))
        {
            Particles.GlowDust.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f),
                timeLeft = Main.rand.Next(45, 120),
                scale = Main.rand.NextFloat(0.25f, 0.5f),
                color = Color.DarkOrange
            });
        }
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        DeathFX();
    }
}
