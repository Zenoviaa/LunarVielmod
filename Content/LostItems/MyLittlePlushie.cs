using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace Stellamod.Content.LostItems
{
    [Autoload(Side = ModSide.Client)]
    public class MyLittlePlushieRenderer : ModSystem
    {
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Player.ResizeHitbox += MiniPlayerHitbox;
            On_LegacyPlayerRenderer.DrawPlayer += MiniPlayer;
            On_Mount.SetMount += ChangeMountedPlayerHitbox;
        }


        public override void OnModUnload()
        {
            base.OnModUnload();
  
            On_Player.ResizeHitbox -= MiniPlayerHitbox;
            On_LegacyPlayerRenderer.DrawPlayer -= MiniPlayer;
            On_Mount.SetMount -= ChangeMountedPlayerHitbox;
        }
        private void ChangeMountedPlayerHitbox(On_Mount.orig_SetMount orig, Mount self, int m, Player mountedPlayer, bool faceLeft)
        {
            MyLittlePlushiePlayer plushiePlayer = mountedPlayer.GetModPlayer<MyLittlePlushiePlayer>();
            if (plushiePlayer.hasLittlePlushie)
            {
                self._data.heightBoost *= 0;

            }
            orig(self, m, mountedPlayer, faceLeft);
  


        }

        private void MiniPlayerHitbox(On_Player.orig_ResizeHitbox orig, Player self)
        {
            MyLittlePlushiePlayer plushiePlayer = self.GetModPlayer<MyLittlePlushiePlayer>();
            if (plushiePlayer.hasLittlePlushie)
            {
                self.width = 12;
                self.height = 16;

                /*
                self.position.Y += self.height;
                self.height = 16 + self.HeightOffsetBoost;
                self.position.Y -= self.height;*/
            }
            else
            {
                orig(self);
            }
           //
        }

        private void MiniPlayer(On_LegacyPlayerRenderer.orig_DrawPlayer orig, LegacyPlayerRenderer self, Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow, float scale)
        {
            MyLittlePlushiePlayer plushiePlayer = drawPlayer.GetModPlayer<MyLittlePlushiePlayer>();
            if (plushiePlayer.hasLittlePlushie)
            {
                scale *= 0.5f;
            }
            orig(self, camera, drawPlayer, position, rotation, rotationOrigin, shadow, scale);
        }
    }

    public class MyLittlePlushiePlayer : ModPlayer
    {
        public bool hasLittlePlushie;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasLittlePlushie = false;
        }

        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();

        }
        public override void PreUpdate()
        {
            base.PreUpdate();
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            base.ModifyHurt(ref modifiers);
            if (hasLittlePlushie)
            {
                modifiers.FinalDamage *= 2f;
            }
        }
    }

    public class MyLittlePlushie : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            MyLittlePlushiePlayer myLittlePlushiePlayer = player.GetModPlayer<MyLittlePlushiePlayer>();
            myLittlePlushiePlayer.hasLittlePlushie = true;
            player.jumpSpeedBoost += 2;
            player.hasMagiluminescence = true;
        }
    }
}
