using Mono.Cecil.Cil;
using MonoMod.Cil;
using Stellamod.Core.LoadingSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

/*
namespace Stellamod.Core.Waters
{
    class WaterAddonHandler : HookGroup
    {
        public static List<WaterAddon> addons = new();

        public static WaterAddon activeAddon;

        public override float Priority => 1.1f;

        public override void Load()
        {
            WaterPlayer.PostUpdateEvent += UpdateActiveAddon;
            IL_Main.DoDraw += AddWaterShader;

            //IL.Terraria.Main.DrawTiles += SwapBlockTexture;//TODO: Figure out where this logic moved in vanilla
        }

        private void UpdateActiveAddon(Player Player)
        {
            activeAddon = addons.FirstOrDefault(n => n.Visible);
        }

        public override void Unload()
        {
            WaterPlayer.PostUpdateEvent -= UpdateActiveAddon;
            IL_Main.DoDraw -= AddWaterShader;
            addons = null;
            activeAddon = null;
        }

        private void AddWaterShader(ILContext il)
        {
            var c = new ILCursor(il);

            //back target
            c.TryGotoNext(n => n.MatchLdfld<Main>("backWaterTarget"));

            c.TryGotoNext(n => n.MatchCallvirt<SpriteBatch>("Draw"));
            c.Index++;
            ILLabel label = il.DefineLabel(c.Next);

            c.TryGotoPrev(n => n.MatchLdfld<Main>("backWaterTarget"));
            c.Index -= 1;
            c.Emit(OpCodes.Pop);
            c.EmitDelegate<Action>(NewDrawBack);
            c.Emit(OpCodes.Br, label);

            //front target
            c.TryGotoNext(n => n.MatchLdsfld<Main>("waterTarget"));

            c.TryGotoNext(n => n.MatchCallvirt<SpriteBatch>("Draw"));
            c.Index++;
            ILLabel label2 = il.DefineLabel(c.Next);

            c.TryGotoPrev(n => n.MatchLdsfld<Main>("waterTarget"));
            c.Emit(OpCodes.Pop);
            c.EmitDelegate<Action>(NewDraw);
            c.Emit(OpCodes.Br, label2);
        }

        private void NewDrawBack()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (activeAddon != null && activeAddon.changeBack)
            {
                spriteBatch.End();
                activeAddon.SpritebatchChangeBack();
            }
            spriteBatch.Draw(Main.instance.backWaterTarget, Main.sceneBackgroundPos - Main.screenPosition, Color.White);
            if (activeAddon != null && activeAddon.changeBack)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
        }

        private void NewDraw()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (activeAddon != null)
            {
                spriteBatch.End();
                activeAddon.SpritebatchChange();
            }

            spriteBatch.Draw(Main.waterTarget, Main.sceneWaterPos - Main.screenPosition, Color.White);
            if (activeAddon != null)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
        }
    }
}
*/