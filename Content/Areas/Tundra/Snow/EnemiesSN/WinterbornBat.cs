using Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using System.IO;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.Content.Areas.Tundra.Snow.EnemiesSN;

public class WinterbornBat : ModNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Chase,
        Swoop
    }

    Vector2 _dashDirection;
    Outliner _outliner;
    bool _contactDamage;
    float _timer;
    AIState _state;
    float _attackCycle;
    float DashTime => 90;
    Player MyTarget => Main.player[NPC.target];
    int _frame = 0;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_dashDirection);
        writer.Write(_timer);
        writer.Write((byte)_state);
        writer.Write(_attackCycle);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _dashDirection = reader.ReadVector2();
        _timer = reader.ReadSingle();
        _state = (AIState)reader.ReadByte();
        _attackCycle = reader.ReadSingle();
    }
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.TrailingMode[Type] = 0;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        float chance = SpawnCondition.Overworld.Chance + SpawnCondition.Underground.Chance + SpawnCondition.Cavern.Chance;
        if (!spawnInfo.Player.ZoneSnow)
            return 0f;
        return chance;
    }


    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter += 1.1f;
        if (NPC.frameCounter >= 6)
        {
            _frame++;
            NPC.frameCounter = 0;
        }
        
        _frame %= Main.npcFrameCount[Type];
        NPC.frame.Y = frameHeight * _frame;
    }

    public override void SetDefaults()
    {
        NPC.width = NPC.height = 30;
        NPC.lifeMax = 40;
        NPC.damage = 13;
        NPC.defense = 3;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath15;
        NPC.knockBackResist = 0.65f;
        NPC.noGravity = true;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, NPC.Center);
        glowDrawer.color = Color.SkyBlue;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.2f;
        spriteBatch.Draw(glowDrawer);
        var drawer = SpritebatchDrawer.FromNPC(NPC);
        for(var i = 0; i < NPC.oldPos.Length; i++)
        {
            var afDrawer = drawer;
            var ratio = (float)i / (float)NPC.oldPos.Length;
            afDrawer.color = Color.Lerp(Color.SkyBlue, Color.Transparent, ratio) * 0.15f;
            afDrawer.rotation = NPC.oldRot[i];
            spriteBatch.Draw(afDrawer);
        }

        spriteBatch.Draw(drawer);
        return false;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        int d = 180;
        for (int k = 0; k < 9; k++)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.Green, 0.7f);
        }
    }


    public override void AI()
    {
        NPC.spriteDirection = NPC.direction;
        NPC.rotation = NPC.velocity.X * 0.03f;
        if (NPC.HasBuff<Pearlflame>())
        {
            WinterbornCommon.TransformEffect(NPC.Center);
            if (MultiplayerHelper.IsHost)
            {
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PearlbornBat>());
            }
            NPC.active = false;
        }
        if (Main.rand.NextBool(16))
        {
            var pos = NPC.Center + Main.rand.NextVector2Circular(24, 24);
            var d = Dust.NewDustPerfect(pos, DustID.GemDiamond, Scale: Main.rand.NextFloat(0.3f, 0.6f));
            d.noGravity = true;
        }

        _contactDamage = false;
        _outliner.SetDefaults();
        switch (_state)
        {
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.Swoop:
                AI_Swoop();
                break;
        }
        _outliner.Update();
        Lighting.AddLight(NPC.Center, new Vector3(0.1f, 0.1f, 0.3f));
    }

    void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            _timer = 0;
            _attackCycle = 0;
            _state = state;
            NPC.netUpdate = true;
        }
    }

    void AI_Chase()
    {
        _timer++;
        if (_timer == 1 || !NPC.HasValidTarget)
            NPC.TargetClosest();
        if (Collision.CanHitLine(NPC.position, 1, 1, MyTarget.position, 1, 1))
        {
            var positionToTrack = MyTarget.Center + new Vector2(0, -32);
            var targetVelocity = positionToTrack - NPC.Center;
            targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
            targetVelocity *= 4;
            NPC.aiStyle = -1;
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.06f);
            NPC.SpriteFaceTarget();
            _dashDirection = Vector2.UnitY;
            var dist = Vector2.Distance(NPC.Center, positionToTrack);
            if(dist <= 100)
            {
                SwitchState(AIState.Swoop);
            }
        }
        else
        {
            NPC.aiStyle = NPCAIStyleID.Bat;
            NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;
        }
    }

    void AI_Swoop()
    {
        _timer++;
        switch (_attackCycle)
        {
            case 0:
                {
                    _outliner.warning = true;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, -Vector2.UnitY, 0.05f);
                    NPC.velocity.X *= 0.9f;
                    if(_timer >= 30)
                    {
                        _timer = 0;
                        _attackCycle++;
                    }
                }
                break;
            case 1:
                {
                    _contactDamage = true;
                    _outliner.attacking = true;
                    var ratio = _timer / DashTime;
                    var ease = EasingFunction.Anticipation2(ratio);
                    var vel = Vector2.Lerp(-_dashDirection * 2, _dashDirection * 15, ease);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, vel, 0.06f);
                    if (_timer >= DashTime)
                    {
                        SwitchState(AIState.Chase);
                    }
                }
                break;
        }
    }


    void DrawOutline(SpriteBatch spriteBatch)
    {
        var drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.color = _outliner.outlineColor;
        spriteBatch.Draw(drawer);
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WinterbornShard>(), minimumDropped: 2, maximumDropped: 4));
    }

    public void DrawToRenderTargets()
    {
        if (_outliner.outlineColor.A < 5)
            return;

        OutlineRenderer.Queue(DrawOutline);
    }
}
