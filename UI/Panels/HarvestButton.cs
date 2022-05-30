using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria;
using Terraria.ModLoader;
using Stellamod.Buffs;
using Terraria.GameContent.UI.Elements;
using System;

namespace Stellamod.UI.Panels
{
    public class HarvestButton : UIState
    {
        public UIImageButton harvestButton;
        public override void Draw(SpriteBatch spriteBatch)
        {
            harvestButton.Draw(spriteBatch);
        }
        public override void OnInitialize()
        {
            Top.Set(-48, 1f);
            Height.Set(48, 0f);
            Width.Set(48, 0f);
            Left.Set(0, 0.5f);
            OnMouseOver += Onclick;           
            harvestButton = new UIImageButton(ModContent.Request<Texture2D>("Stellamod/UI/Panels/buttonharvest"));
            harvestButton.Left.Set(0, 0.5f);
            harvestButton.Top.Set(-48, 1f);
            harvestButton.Width.Set(48f, 0f);
            harvestButton.Height.Set(48, 0f);
            harvestButton.SetVisibility(1f, 1f);
        }

        public void Onclick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (IsMouseHovering)
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.HasBuff<Harvester>())
                    {
                        npc.life = 0;
                    }
                }
            }
        }
    }
    }
//