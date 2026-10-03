using Stellamod.Core;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;

public class AuraTrappingPlayer : ModPlayer
{
    public int trappedTo = -1;
}

public class MoonAura : ModNPC
{
    float Ease
    {
        get
        {
            var inRatio = EasingFunction.InOutSine(Timer / 60f);
            var outRatio = EasingFunction.InOutSine(LifeTime / 60f);
            return inRatio * outRatio;
        }
    }
    float AuraRadius => 512;
    ref float Timer => ref NPC.ai[0];
    ref float LifeTime => ref NPC.ai[1];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
 
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.damage = 1;
        NPC.lifeMax = 1;
        NPC.dontTakeDamage = true;
        NPC.dontCountMe = true;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.noGravity = true;
        NPC.defense = 1;
    }
    
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Main.rand.NextBool(16))
        {
            var pos = NPC.Center + Main.rand.NextVector2Circular(512, 512);
            var spr = SparkleParticle.Spawn(pos, Vector2.Zero, Scale: Main.rand.NextFloat(0.4f, 0.6f));
            spr.gravity = 0;
            spr.noTileCollide = true;
            spr.innerColor = Color.SkyBlue;
            spr.outerColor = Color.Blue;
        }

        foreach(var player in Main.ActivePlayers)
        {
            var trappingPlayer = player.GetModPlayer<AuraTrappingPlayer>();
            if (trappingPlayer.trappedTo == NPC.whoAmI)
            {
                var dist = Vector2.Distance(NPC.Center, player.Center);
                if (dist > AuraRadius)
                {
                    var dir = (player.Center - NPC.Center);
                    dir = dir.Resize(AuraRadius);
                    var edgePoint = NPC.Center + dir;
                    var playerVel = (edgePoint - player.Center);
                    player.velocity = playerVel;
                    player.velocity += (NPC.Center - player.Center).SafeNormalize(Vector2.Zero) * 8;
                    //trappingPlayer.trappedTo = NPC.whoAmI;
                }
            }
            else
            {
                var dist = Vector2.Distance(NPC.Center, player.Center);
                if(dist < AuraRadius)
                {
                    trappingPlayer.trappedTo = NPC.whoAmI;
                }
            }
        }
        if(Timer == 1)
        {
            var sound = AssetReferences.Assets.Sounds.SingularityFragment_TPIn.Asset;
            SoundEngine.PlaySound(sound, NPC.position);
        }
        LifeTime--;
        if(LifeTime == 20)
        {
            foreach (var player in Main.ActivePlayers)
            {
                var trappingPlayer = player.GetModPlayer<AuraTrappingPlayer>();
                if (trappingPlayer.trappedTo == NPC.whoAmI)
                {
                    trappingPlayer.trappedTo = -1;
                }
         
            }
            var sound = AssetReferences.Assets.Sounds.SingularityFragment_TPOut.Asset;
            SoundEngine.PlaySound(sound, NPC.position);
        }
        if (LifeTime <= 0)
            NPC.active = false;
        var buffType = ModContent.BuffType<Pearlflame>();
        var pearlEnemies = new int[3]
        {
            ModContent.NPCType<PearlbornSoul>(),
            ModContent.NPCType<PearlbornSlime>(),
            ModContent.NPCType<PearlbornBat>()
        };
        if (LifeTime < 20)
            return;
        bool lfoundEnemy = false;
        foreach(var npc in Main.ActiveNPCs)
        {
            var sqrDst = Vector2.DistanceSquared(NPC.Center, npc.Center);
            if(sqrDst <= AuraRadius * AuraRadius * Ease)
            {
                npc.AddBuff(buffType, 120);
                for (var i = 0; i < pearlEnemies.Length; i++)
                {
                    if (pearlEnemies[i] == npc.type)
                    {
                        lfoundEnemy = true;
                    }
                }
            }
        }
        if (lfoundEnemy)
        {
            LifeTime++;
        }
        else if(LifeTime > 60 && Timer > 180)
        {
            LifeTime = 60;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var moonEffect = ModContent.GetInstance<MoonEffect>();
        moonEffect.isActive = true;
        MoonEffect.PrepareForRenderering(DrawMask);
        return false;
    }

    void DrawMask(SpriteBatch spriteBatch)
    {
        var inRatio = EasingFunction.InOutSine(Timer / 60f);
        var outRatio = EasingFunction.InOutSine(LifeTime / 60f);
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.NoiseTextures.CircleGradient.Asset, NPC.Center);
        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.scale = Vector2.One * inRatio * outRatio * 1.3f;
        spriteBatch.Draw(drawer);
    }
}

