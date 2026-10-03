using Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using System.IO;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.Content.Areas.Tundra.Snow.EnemiesSN;


public class WinterSoul : ModNPC
{
    enum AIState : byte
    {
        MoveTowardsPlayer,
        DashAtPlayer
    }

    Outliner _outliner;
    Vector2 _dashDirection;
    bool _contactDamage;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    ref float AttackCycle => ref NPC.ai[2];
    public int Style = -1;
    float GetCloseDistance => 128;
    float DashTime => 60;
    float IdleTime => 100;
    float ChaseSpeed => 8;
    int _frame = 0;
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_dashDirection);
        writer.Write(Style);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _dashDirection = reader.ReadVector2();
        Style = reader.ReadInt32();
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 5;
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[NPC.type] = 0;
    }

    public override void SetDefaults()
    {
        NPC.width = 16;
        NPC.height = 16;
        NPC.defense = 3;
        NPC.lifeMax = 40;
        NPC.damage = 15;
        NPC.lavaImmune = false;
        NPC.noGravity = true;
        NPC.noTileCollide = true;

        NPC.HitSound = SoundID.NPCHit30;
        NPC.DeathSound = SoundID.NPCDeath38;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        float chance = SpawnCondition.OverworldNightMonster.Chance + SpawnCondition.Underground.Chance + SpawnCondition.Cavern.Chance;
        if (!spawnInfo.Player.ZoneSnow)
            return 0f;
        return chance;
    }


    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter += 0.25f;
        if (NPC.frameCounter >= 1)
        {
            _frame++;
            NPC.frameCounter = 0;
        }
        if (_frame >= 5)
        {
            _frame = 0;
        }
        NPC.frame.Y = frameHeight * _frame;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        int d = DustID.BlueTorch;
        for (int k = 0; k < 8; k++)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);
        }
        if (NPC.life <= 0)
        {
            for (int i = 0; i < 20; i++)
            {
                int num = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != NPC.Center)
                    Main.dust[num].velocity = NPC.DirectionTo(Main.dust[num].position) * 6f;
            }
        }
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WinterbornShard>(), minimumDropped: 2, maximumDropped: 4));
    }


    public override void AI()
    {
        if (MultiplayerHelper.IsHost && Style == -1)
        {
            Style = Main.rand.Next(0, 3);
            NPC.netUpdate = true;
        }
        if (Main.rand.NextBool(12))
        {
            var pos = NPC.Center + Main.rand.NextVector2Circular(10, 10);
            var p = Dust.NewDustPerfect(pos, DustID.GemDiamond, Main.rand.NextVector2Circular(1, 1), Scale: Main.rand.NextFloat(0.66f, 1f));
            p.noGravity = true;
        }

        _contactDamage = false;
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.MoveTowardsPlayer:
                AI_MoveTowardsPlayer();
                break;
            case AIState.DashAtPlayer:
                AI_DashAtPlayer();
                break;
        }
        _outliner.Update();
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
        NPC.SpriteFaceTarget();
    }

    void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            AttackCycle = 0;
            NPC.netUpdate = true;
        }
    }

    void AI_MoveTowardsPlayer()
    {
        if (NPC.HasBuff<Pearlflame>())
        {
            WinterbornCommon.TransformEffect(NPC.Center);
            if (MultiplayerHelper.IsHost)
            {
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PearlbornSoul>());
            }
            NPC.active = false;
        }
        if (!NPC.HasValidTarget)
            NPC.TargetClosest();
        var target = Main.player[NPC.target];
        var targetPosition = target.Center;
        var distanceToTarget = Vector2.Distance(NPC.Center, targetPosition);

        if (!NPC.HasValidTarget)
        {
            NPC.velocity *= 0.97f;
        }
        else
        {
            if (distanceToTarget <= GetCloseDistance)
            {
                var direction = targetPosition - NPC.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                _dashDirection = direction;
                NPC.velocity *= 0.96f;
                if (Timer >= IdleTime * 0.5f)
                {
                    NPC.velocity -= _dashDirection * 0.1f;
                    _outliner.warning = true;
                }
                Timer++;
                if (Timer >= IdleTime)
                {
               
             
                    SwitchState(AIState.DashAtPlayer);
                }
            }
            else
            {
                var targetVelocity = (target.Center - NPC.Center);
                targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
                targetVelocity *= ChaseSpeed;
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.04f);
            }
        }

    }

    void AI_DashAtPlayer()
    {
        Timer++;
        if (Timer >= DashTime)
        {
            SwitchState(AIState.MoveTowardsPlayer);
        }
        _contactDamage = true;
        _outliner.attacking = true;
        var ratio = Timer / DashTime;
        var ease = EasingFunction.Anticipation2(ratio);
        var vel = Vector2.Lerp(-_dashDirection * 2, _dashDirection * 15, ease);
        NPC.velocity = Vector2.Lerp(NPC.velocity, vel, 0.06f);

    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        string drawTexturePath = Texture;
        if (Style == 1)
        {
            drawTexturePath += "_2";
        }
        if (Style == 2)
        {
            drawTexturePath += "_3";
        }
        Texture2D drawTexture = ModContent.Request<Texture2D>(drawTexturePath).Value;

        var drawer = SpritebatchDrawer.FromNPC(NPC);
      
        drawer.texture = drawTexture;
        for(var i = 0; i < NPC.oldPos.Length; i++)
        {
            var pos = NPC.oldPos[i] + NPC.Size * 0.5f;
            var afDrawer = drawer;
            afDrawer.worldPosition = pos;
            var ratio = (float)i / (float)NPC.oldPos.Length;
            afDrawer.color = Color.Lerp(Color.White, Color.Transparent, ratio) * 0.1f;
            spriteBatch.Draw(afDrawer);
        }
        var npcDrawer = drawer;
        spriteBatch.Draw(drawer);


        drawer.color = Color.White * ExtraMath.Osc(0.6f, 1f, speed: 3, NPC.whoAmI);
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);

        var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, NPC.Center);
        glowDrawer.color = Color.SkyBlue * 0.2f * ExtraMath.Osc(0.7f, 1f, speed: 3, NPC.whoAmI);
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.2f;
        spriteBatch.Draw(glowDrawer);

        var eyes = AssetReferences.Content.Areas.Tundra.Snow.EnemiesSN.WinterSoul_Glow.Asset;
        var eyeDrawer = npcDrawer;
        eyeDrawer.texture = eyes.Value;
        spriteBatch.Draw(eyeDrawer);
        OutlineRenderer.Queue(DrawOutline);
        return false;
    }

    void DrawOutline(SpriteBatch spriteBatch)
    {
        string drawTexturePath = Texture;
        if (Style == 1)
        {
            drawTexturePath += "_2";
        }
        if (Style == 2)
        {
            drawTexturePath += "_3";
        }
        Texture2D drawTexture = ModContent.Request<Texture2D>(drawTexturePath).Value;

        var drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = drawTexture;
        drawer.color = _outliner.outlineColor;
        spriteBatch.Draw(drawer);
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(spriteBatch, screenPos, drawColor);
        Lighting.AddLight(NPC.Center, Color.Blue.ToVector3() * Main.essScale);
    }
}