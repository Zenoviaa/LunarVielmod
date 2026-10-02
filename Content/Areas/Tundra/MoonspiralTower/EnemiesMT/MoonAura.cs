using Stellamod.Core;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;

public class MoonAura : ModNPC
{
    float AuraRadius => 1024;
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
        LifeTime--;
        if (LifeTime <= 0)
            NPC.active = false;
        var buffType = ModContent.BuffType<Pearlflame>();
        foreach(var npc in Main.ActiveNPCs)
        {
            var sqrDst = Vector2.DistanceSquared(NPC.Center, npc.Center);
            if(sqrDst <= AuraRadius * AuraRadius)
            {
                npc.AddBuff(buffType, 120);
            }
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
        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, NPC.Center);
        drawer.color = Color.White;
        drawer.color.A = 0;
        drawer.scale = Vector2.One * inRatio * outRatio;
        spriteBatch.Draw(drawer);
    }
}

