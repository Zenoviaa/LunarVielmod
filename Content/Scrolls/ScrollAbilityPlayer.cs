using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Scrolls;

public class ScrollAbilityPlayer : ModPlayer
{
    private float _angerTimer;
    private float _enduranceTimer;
    private float _flameTimer;
    private float _poisonTimer;
    private Asset<Texture2D> _angerSymbol;
    private Asset<Texture2D> _shield;
    public override void PostUpdateBuffs()
    {
        base.PostUpdateBuffs();
        ManageBuffTimer<Anger>(ref _angerTimer);
        ManageBuffTimer<Endurance>(ref _enduranceTimer);
        ManageBuffTimer<Flame>(ref _flameTimer);
        ManageBuffTimer<Poison>(ref _poisonTimer);
    }

    private void ManageBuffTimer<T>(ref float timer) where T : ModBuff
    {
        timer += Player.HasBuff<T>() ? 1 : -1;
        timer = MathHelper.Clamp(timer, 0f, 60f);
    }
    
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (Player.HasBuff<Flame>())
        {
            target.AddBuff(BuffID.OnFire3, 300);
        }

        if (Player.HasBuff<Poison>())
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }
    }


    private void RequestTextures()
    {
        _angerSymbol ??= ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "AngerSymbol");
        _shield ??= ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "Shield");
    }
    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        if (drawInfo.shadow != 0f)
            return;
        RequestTextures();
        SpriteBatch spriteBatch = Main.spriteBatch;
        if (_angerTimer > 0)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_angerSymbol, drawInfo.drawPlayer.Center + new Vector2(18, -36));
            drawer.color = Color.Red * EasingFunction.InOutSine(_angerTimer / 60f) * ExtraMath.Osc(0.5f, 1f, speed: 12);
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);
        }
        if (_enduranceTimer > 0)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_shield, drawInfo.drawPlayer.Center + new Vector2(0, -80));
            drawer.color = Color.SkyBlue * EasingFunction.InOutSine(_enduranceTimer / 60f) * ExtraMath.Osc(0.5f, 1f, speed: 12);
            drawer.scale *= 0.5f;
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);
        }

        if(_flameTimer > 0)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, drawInfo.drawPlayer.Center);
            drawer.color = Color.OrangeRed * EasingFunction.InOutSine(_flameTimer / 60f) * ExtraMath.Osc(0.5f, 1f, speed: 12) * 0.5f;
            drawer.scale *= 0.5f;
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);
        }

        if(_poisonTimer > 0)
        {
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, drawInfo.drawPlayer.Center);
            drawer.color = Color.Green * EasingFunction.InOutSine(_poisonTimer / 60f) * ExtraMath.Osc(0.5f, 1f, speed: 12) * 0.5f;
            drawer.scale *= 0.5f;
            drawer.color.A = 0;
            Main.spriteBatch.Draw(drawer);
        }
    }
}
