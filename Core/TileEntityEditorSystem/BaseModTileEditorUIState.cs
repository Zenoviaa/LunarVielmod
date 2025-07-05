using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.TileEntityEditorSystem
{
    internal abstract class BaseModTileEditorUIState : UIState
    {

        public BaseModTileEditor ui;
        public virtual void Open()
        {
            ui.Load(TileEntitySelector.TargetTileEntity);
        }

        public virtual void Close()
        {
            if (TileEntitySelector.SaveNClose)
            {
                ModTileEntity modTileEntity = TileEntitySelector.TargetTileEntity;
                ui.Apply(modTileEntity);
                Main.NewText("Saved Tile Entity");
                for (float i = 0; i < 12; i++)
                {
                    float rot = MathHelper.TwoPi * Main.rand.NextFloat(0f, 1f);
                    Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(5f, 25f);
                    var particle = FXUtil.GlowStretch(modTileEntity.Position.ToWorldCoordinates(), velocity);
                    particle.InnerColor = Color.White;
                    particle.GlowColor = Color.LightCyan;
                    particle.OuterGlowColor = Color.Black;
                    particle.Duration = Main.rand.NextFloat(25, 50);
                    particle.BaseSize = Main.rand.NextFloat(0.04f, 0.07f);
                    particle.VectorScale *= 0.5f;
                }
            }

        }
    }
}
