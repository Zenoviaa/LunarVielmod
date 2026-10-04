using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;


namespace Stellamod.Content.Areas.Tundra.Snow.EnemiesSN;

public class WinterBornSlippery : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);

    }
}

public class WinterBornSlipPlayer : ModPlayer
{
    public bool superSlippy;
    public override void ResetEffects()
    {
        base.ResetEffects();
        superSlippy = false;
    }

    public override void PostUpdateRunSpeeds()
    {
        base.PostUpdateRunSpeeds();
        if (Player.HasBuff<WinterBornSlippery>())
        {
            Player.runAcceleration *= 9f;
            Player.maxRunSpeed *= 3;
        }
    }
    public override void PreUpdateMovement()
    {
        base.PreUpdateMovement();

    }
}
public class WinterBornSlimeSlime : ModProjectile
{
    float Slime_Radius => 36;
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        
    }
    
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.timeLeft = 60;
        Projectile.light = 0.4f;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    
    public override void AI()
    {
        base.AI();
       
        foreach(var player in Main.ActivePlayers)
        {
            var sqrDistance = Vector2.DistanceSquared(player.Center, Projectile.Center);
            if(sqrDistance < Slime_Radius * Slime_Radius)
            {
                player.AddBuff(ModContent.BuffType<WinterBornSlippery>(), 30);
            }
        }
        Timer++;
        if(Timer % 15 == 0)
        {
            var pos = Projectile.Center;
            Particles.SplatDust.Spawn(new()
            {
                position = pos,
                timeLeft = 180,
                color = Color.Lerp(new Color(98, 192, 213), Color.White, 0.4f),
                scale = Main.rand.NextFloat(0.8f, 1.2f)
            });
        }
        if(Timer % 4 == 0)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(20, 20);
            var d = Dust.NewDustPerfect(pos, DustID.GemDiamond, Main.rand.NextVector2Circular(1, 1), Scale: Main.rand.NextFloat(0.4f, 0.6f) * 2);
            d.noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
        //return base.PreDraw(ref lightColor);
    }
}
public class WinterBornSlime : ModNPC
{
    float _direction;
    ref float Timer => ref NPC.ai[0];
    int _frame;
    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frameCounter += 0.15f;
        if (NPC.frameCounter >= 1f)
        {
            NPC.frameCounter = 0;
            _frame++;
            _frame %= Main.npcFrameCount[Type];
        }

        NPC.frame.Y = frameHeight * _frame;
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.BlueSlime];
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        float chance = SpawnCondition.Overworld.Chance + SpawnCondition.Underground.Chance + SpawnCondition.Cavern.Chance;
        if (!spawnInfo.Player.ZoneSnow)
            return 0f;
        return chance;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetDefaults()
    {
        NPC.damage = 10;
        NPC.width = 34; 
        NPC.height = 25;
        NPC.lifeMax = 55;
        NPC.defense = 3;
        NPC.lifeMax = 40;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath15;
        NPC.value = 60f;
        NPC.knockBackResist = 0.65f;
        NPC.noGravity = false;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        int d = 180;
        for (int k = 0; k < 8; k++)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, d, 2.5f * hit.HitDirection, -2.5f, 0, Color.Green, 0.7f);
        }
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer % 5 == 0)
        {
            var pos = NPC.Center + Main.rand.NextVector2Circular(16, 16);
            var d = Dust.NewDustPerfect(pos, DustID.GemDiamond, Scale: Main.rand.NextFloat(0.4f, 0.6f));
            d.scale *= 0.5f;
            d.noGravity = true;
        }
        if(Timer % 30 == 0)
        {
            if (MultiplayerHelper.IsHost)
            {
                var firer = ProjFirer.From<WinterBornSlimeSlime>(NPC);
                firer.New();
            }
        }

        if (Timer >= 100)
        {
            if (MultiplayerHelper.IsHost)
            {
                _direction = Main.rand.NextBool(2) ? -1 : 1;
                NPC.netUpdate = true;
            }

      
            Timer = 0;
        }
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, _direction, 0.02f);

        if (NPC.HasBuff<Pearlflame>())
        {
            WinterbornCommon.TransformEffect(NPC.Center);
            if (MultiplayerHelper.IsHost)
            {
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PearlbornSlime>());
            }
            NPC.active = false;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Vector2 center = NPC.Center + new Vector2(0f, NPC.height * -0.1f);
        Lighting.AddLight(NPC.Center, Color.LightSkyBlue.ToVector3() * 0.25f * Main.essScale);
        var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, NPC.Center);
        glowDrawer.color = Color.SkyBlue * ExtraMath.Osc(0.9f, 1f, offset: NPC.whoAmI) * 0.2f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= 0.3f;
        spriteBatch.Draw(glowDrawer);
        var drawer = SpritebatchDrawer.FromNPC(NPC);
        spriteBatch.Draw(drawer);
        return false;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WinterbornShard>(), minimumDropped: 2, maximumDropped: 4));
        npcLoot.Add(ItemDropRule.Common(ItemID.Gel, minimumDropped: 0, maximumDropped: 2));
    }
}