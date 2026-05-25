using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace Stellamod.Common;

public class HelpPlayer : ModPlayer
{
    private float _helpTimer;
    public float helpAlpha;
    public bool dashHelp;
    public bool flaskHelp;
    public override void ResetEffects()
    {
        base.ResetEffects();
    }
    public override void PostUpdateMiscEffects()
    {
        base.PostUpdateMiscEffects();
        if (Player.whoAmI != Main.myPlayer)
            return;

        if (dashHelp && LunarVeilKeybinds.DashKeybind.JustPressed)
        {
            dashHelp = false;
        }

        if(flaskHelp && LunarVeilKeybinds.FlaskKeybind.JustPressed)
        {
            flaskHelp = false;
        }

        if (HasAnyHelp())
        {
            _helpTimer++;
        }
        else
        {
            _helpTimer--;
        }
        _helpTimer = MathHelper.Clamp(_helpTimer, 0, 60f);
        helpAlpha = _helpTimer / 60f;
    }

    public string GetHelpText()
    {
        if (dashHelp)
        {
            string keyToUse = LunarVeilKeybinds.DashKeybind.AssignedKeybindString();
            if (string.IsNullOrEmpty(keyToUse))
            {
                return LangText.Common("AssignKey", LunarVeilKeybinds.DashKeybind.DisplayName);
            }

            return LangText.Common("DashHelp", keyToUse);
        }

        if (flaskHelp && Player.GetModPlayer<FlaskPlayer>().HasEquippedflask())
        {
            string keyToUse = LunarVeilKeybinds.FlaskKeybind.AssignedKeybindString();
            if (string.IsNullOrEmpty(keyToUse))
            {
                return LangText.Common("AssignKey", LunarVeilKeybinds.FlaskKeybind.DisplayName);
            }

            return LangText.Common("XixianHelp", keyToUse);
        }
        return string.Empty;
    }

    public bool HasAnyHelp()
    {
        if (dashHelp)
            return true;
        if (flaskHelp)
            return true;
        return false;
    }
    public override void SaveData(TagCompound tag)
    {
        base.SaveData(tag);
    }
    public override void LoadData(TagCompound tag)
    {
        base.LoadData(tag);
    }
}

[Autoload(Side = ModSide.Client)]
public class HelpDrawSystem : ModSystem
{
    public override void Load()
    {
        base.Load();
        On_Main.DrawPlayers_AfterProjectiles += DrawHelpLayer;
    }

    private void DrawHelpLayer(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;
        Player player = Main.LocalPlayer;
        HelpPlayer helpPlayer = player.GetModPlayer<HelpPlayer>();
        float alpha = helpPlayer.helpAlpha;
        if(alpha > 0)
        {
          //  Main.NewText("e");
            string helpText = helpPlayer.GetHelpText();
            SpriteBatch sb = Main.spriteBatch;
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, helpText,
            player.Center + new Vector2(0, -64) - Main.screenPosition, Color.White * alpha * ExtraMath.Osc(0.6f, 1f, speed: 3), 0f, FontAssets.MouseText.Value.MeasureString(helpText) * new Vector2(0.5f), Vector2.One, -1, 1f);
            sb.End();
        }

        //throw new NotImplementedException();
    }
}
