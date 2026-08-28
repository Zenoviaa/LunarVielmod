using ReLogic.Content;
using Stellamod.Core.Particles;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.AccPT;

public class Steaming : ModBuff
{
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
        int denom = (int)MathHelper.Lerp(10, 2, player.GetModPlayer<SteamHatterPlayer>().stacks / 5f);
        int denom2 = (int)MathHelper.Lerp(24, 12, player.GetModPlayer<SteamHatterPlayer>().stacks / 5f);
        if (Main.rand.NextBool(denom))
        {
            Vector2 spawnPosition = player.Center;
            spawnPosition += Main.rand.NextVector2Circular(36, 36);
            spawnPosition.Y -= 32;
            Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }

        if (Main.rand.NextBool(denom2))
        {
            var zap = LegacyParticle.NewParticle<ZapParticle>(player.Center + Main.rand.NextVector2Circular(32, 32) + new Vector2(0, -32), Main.rand.NextVector2Circular(1, 1), Color.White, 1f);
            zap.innerColor = Color.Red;
            zap.outerColor = Color.Lerp(zap.innerColor, Color.Black, 0.5f);
            zap.fadeToColor = Color.Lerp(zap.outerColor, Color.Black, 0.5f);
        }
    }
}

public class SteamHatDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => true;
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.GetModPlayer<SteamHatterPlayer>().hasSteamHatter && !drawInfo.drawPlayer.GetModPlayer<SteamHatterPlayer>().hideVisual;

    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        var hatDrawData = CostumeUtilities.GetHatDrawData(ref drawInfo, 
            TextureAssets.Item[ModContent.ItemType<SteamHatter>()],
            HatDrawParameters.Default with { hatOffset = new Vector2(0, -32)});
        drawInfo.DrawDataCache.Add(hatDrawData);
    }
}
public class SteamHatterPlayer : ModPlayer
{

    public bool hasSteamHatter;
    public bool hideVisual;
    public float stacks;
    public override void ResetEffects()
    {
        base.ResetEffects();
        hasSteamHatter = false;
    }
    public override void PostUpdateEquips()
    {
        base.PostUpdateEquips();
        if (!hasSteamHatter)
            return;
        if (!Player.HasBuff<Steaming>())
        {
            stacks = 0;
            return;
        }
        Player.GetAttackSpeed(DamageClass.Generic) += 0.1f * stacks;
        Player.GetDamage(DamageClass.Generic) += 0.1f * stacks;
    }

    public override void PostHurt(Player.HurtInfo info)
    {
        base.PostHurt(info);
        if (!hasSteamHatter)
            return;
        stacks++;
        if (stacks >= 5)
            stacks = 5;
        CombatText.NewText(Player.getRect(), Color.White, $"x{stacks}", true);
        Player.AddBuff(ModContent.BuffType<Steaming>(), 60 * 8);
        SoundStyle steamingSound = AssetRegistry.Sounds.SteamPunking.MechSteaming;
        steamingSound.PitchVariance = 0.3f;
        SoundEngine.PlaySound(steamingSound, Player.position);
    }
}
public class SteamHatter : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<SteamHatterPlayer>().hasSteamHatter = true;
        player.GetModPlayer<SteamHatterPlayer>().hideVisual = hideVisual;
    }
}
