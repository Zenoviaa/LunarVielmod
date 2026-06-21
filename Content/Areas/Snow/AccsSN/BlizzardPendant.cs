using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.SwingSystem;
using Stellamod.Effects.Generic;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Snow.AccsSN;

public class BlizzardPendant : AbstractMeleeAddon
{
    public override void AI(BaseSwingProjectileV2 projectile)
    {
        base.AI(projectile);
        foreach (NPC npc in NPCHelper.FindNPCsInRange(projectile.Projectile.Center, 100))
        {
            npc.AddBuff(ModContent.BuffType<BlizzardChill>(), 90);
        }
    }

    public override void OnHitNPC(BaseSwingProjectileV2 projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(projectile, target, hit, damageDone);
        if (projectile.IsFinishingSwing())
        {
            target.AddBuff(ModContent.BuffType<BlizzardFreeze>(), 60);
        }
    }

    public override void PreDrawEffects(BaseSwingProjectileV2 projectile, ref Color lightColor)
    {
        base.PreDrawEffects(projectile, ref lightColor);
        SpritebatchDrawer backGlow = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, projectile.Projectile.Center);
        backGlow.color = Color.LightSkyBlue * ExtraMath.Osc(0.5f, 1f, speed: 6);
        backGlow.color.A = 0;
        backGlow.scale *= 0.5f;
        Main.spriteBatch.Draw(backGlow);
    }

    public override void PostDrawEffects(BaseSwingProjectileV2 projectile, ref Color lightColor)
    {
        base.PostDrawEffects(projectile, ref lightColor);
        AuraShader blizzardShader = ShaderContent.GetInstance<AuraShader>();
        SpritebatchParams spritebatchParams = SpritebatchParams.InWorldAndZoomed() with { effect = blizzardShader };
        using (SpritebatchStarter.Begin(Main.spriteBatch, spritebatchParams))
        {
            Asset<Texture2D> blizzardNoise = ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/StarNoise");
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(blizzardNoise, projectile.Projectile.Center);
            Main.spriteBatch.Draw(drawer);
        }
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<WinterbornShard, BlankAccessory>();
    }
}

public class BlizzardFreezeGlobalNPC : GlobalNPC
{
    public override bool PreAI(NPC npc)
    {
        if (npc.boss)
            return base.PreAI(npc);
        if (npc.HasBuff<BlizzardFreeze>())
            return false;
        return base.PreAI(npc);
    }

    public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(npc, spriteBatch, screenPos, drawColor);
    }
}

public class BlizzardFreeze : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }
    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        npc.lifeRegen -= 8;

        float slowdownMult = 0f;
        npc.velocity *= slowdownMult;
    }
}

public class BlizzardChill : ModBuff
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        npc.lifeRegen -= 8;

        float slowdownMult = npc.boss ? 0.995f : 0.96f;
        npc.velocity *= slowdownMult;

        if (Main.rand.NextBool(5))
        {
            SmokeParticle sp = Particle<SmokeParticle>.Spawn(npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), -Vector2.UnitY, Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
            sp.initialColor = Color.Lerp(Color.LightSkyBlue, Color.DarkSlateBlue, Main.rand.NextFloat(0f, 1f)) * 0.4f;
            sp.expand = true;
        }

        if (Main.rand.NextBool(5))
        {
            LegacyParticle.NewParticle<EmberParticle>(npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), -Vector2.UnitY.RotatedByRandom(1.5f), Color.OrangeRed, Main.rand.NextFloat(0.9f, 1.5f));
        }
    }
}