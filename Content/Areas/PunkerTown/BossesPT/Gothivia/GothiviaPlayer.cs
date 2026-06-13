using Stellamod.Assets;
using Stellamod.Buffs;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;
using Stellamod.Core.Pixelation;
using Stellamod.Effects.GothinFlames;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia;

public class GothiviaPlayer : ModPlayer
{
    public int maxStacks;
    public int sunStacks;
    public override void ResetEffects()
    {
        base.ResetEffects();
        maxStacks = 3;
    }

    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (!NPC.AnyNPCs(ModContent.NPCType<Gothivia>()))
            sunStacks = 0;
        if (Main.myPlayer != Player.whoAmI)
            return;
        if (sunStacks <= 0)
            return;
        if (Player.ownedProjectileCounts[ModContent.ProjectileType<MiniSun>()] >= 1)
            return;

        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + new Vector2(0, -256), Vector2.Zero,
            ModContent.ProjectileType<MiniSun>(), 1, 1, Player.whoAmI);
    }

    public override void UpdateDead()
    {
        base.UpdateDead();
        sunStacks = 0;
    }

    public void AddSunStack()
    {
        sunStacks++;
    }
}



public class MiniSun : ModProjectile,
    IDrawToRenderTarget
{
    private enum AIState
    {
        Hover,
        Crash
    }

    private Vector2 _startOffset;
    private ref float Timer => ref Projectile.ai[0];
    private AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => TextureRegistry.EmptyTexture;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 1800;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.light = 0.7f;
    }

    public override void AI()
    {
        base.AI();

        switch (State)
        {
            case AIState.Hover:
                AI_Hover();
                break;
            case AIState.Crash:
                AI_Crash();
                break;
        }

        GothiviaPlayer gPlayer = Owner.GetModPlayer<GothiviaPlayer>();
        float statcks = gPlayer.sunStacks;
        if (statcks >= gPlayer.maxStacks && State != AIState.Crash)
            SwitchState(AIState.Crash);
    }

    private void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            Timer = 0;
            State = state;
            Projectile.netUpdate = true;
        }
    }
    private void AI_Hover()
    {
        Timer++;
        Vector2 offset = new Vector2(0, -100);
        offset = offset.RotatedBy(Timer * 0.05f);
        Vector2 pos = Owner.Center + offset;
        pos.Y += ExtraMath.Osc(-4f, 4f);
        Projectile.Center = pos;
    }

    private void AI_Crash()
    {
        Timer++;
        if (Timer == 1)
        {
            _startOffset = (Projectile.Center - Owner.Center);
        }

        float crashTime = 1;
        float ratio = Timer / crashTime;
        float ease1 = EasingFunction.OutExpo(ratio);
        float ease2 = EasingFunction.InExpo(ratio);
        Vector2 offset1 = Vector2.Lerp(_startOffset, _startOffset - Vector2.UnitY * 128, ease1);
        Vector2 offset2 = Vector2.Lerp(_startOffset - Vector2.UnitY * 128, Vector2.Zero, ease2);
        Vector2 offset3 = Vector2.Lerp(offset1, offset2, ratio);
        Vector2 positionToMoveTo = Owner.Center + offset3;
        Projectile.Center = positionToMoveTo;
        if (Timer >= crashTime)
        {
            Owner.KillMe(new Terraria.DataStructures.PlayerDeathReason(), 10000000, 1);

            if (this.OwnedByLocalClient())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero,
                    ModContent.ProjectileType<RedSunBoom>(), 1, 1, Projectile.owner);
            }

            Projectile.Kill();
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        float statcks = Owner.GetModPlayer<GothiviaPlayer>().sunStacks;
        float sizeMult = MathHelper.Lerp(1f, 3f, statcks / 3f);
        Vector2 scale = Vector2.One * 0.1f * sizeMult;
        RedSunShader redSunShader = ShaderContent.GetInstance<RedSunShader>();
        redSunShader.Time = Main.GlobalTimeWrappedHourly * 9;
        redSunShader.InsideColor = Color.Yellow;
        redSunShader.BloomColor = Color.DarkRed;
        redSunShader.FlameNoiseTexture = AssetManager.Noise.PerlinBlurred.Value;
        Main.spriteBatch.Restart(SpriteSortMode.Immediate, effect: redSunShader.Effect, blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointWrap);
        SpritebatchDrawer redSunDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/WaterTrail"), Projectile.Center);
        redSunDrawer.scale *= scale * 2;
        redSunDrawer.color = Color.White;
        redSunDrawer.color.A = 0;
        //
        Main.spriteBatch.Draw(redSunDrawer);
        Main.spriteBatch.RestartDefaults();


        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.JumbledGlowCircle.Asset.Value, Projectile.Center);
        glowDrawer.color = Color.Red * 0.16f;
        glowDrawer.color.A = 0;
        glowDrawer.scale *= scale * 6;
        Main.spriteBatch.Draw(glowDrawer);

        SpritebatchDrawer glowDrawerCircle = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
        glowDrawerCircle.scale *= scale * 1.2f;
        glowDrawerCircle.color = Color.Red;
        glowDrawerCircle.color.A = 0;
        Main.spriteBatch.Draw(glowDrawerCircle);

        glowDrawerCircle.color = Color.Lerp(Color.White, Color.Yellow, ExtraMath.Osc(0f, 1f));
        glowDrawerCircle.scale *= 0.95f;
        glowDrawerCircle.color.A = 0;
        Main.spriteBatch.Draw(glowDrawerCircle);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {

    }
}