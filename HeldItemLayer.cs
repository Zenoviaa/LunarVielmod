using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod
{
    internal class HeldItemLayer : PlayerDrawLayer
    {
        private static Dictionary<int, DrawOverHeldItemDelegate> data;

        public delegate void DrawOverHeldItemDelegate(ref PlayerDrawSet drawInfo);

        public static void RegisterItemGlowmask(int itemType, DrawOverHeldItemDelegate customDrawMethod = null)
        {
            data.Add(itemType, customDrawMethod);
        }

        // ...

        public override void Load()
        {
            data = new();
        }

        public override void Unload()
        {
            data.Clear();
            data = null;
        }

        public override Position GetDefaultPosition() => new AfterParent(Terraria.DataStructures.PlayerDrawLayers.HeldItem);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            var heldItem = drawInfo.heldItem;

            if (!data.ContainsKey(heldItem.type)) return false;

            var drawPlayer = drawInfo.drawPlayer;
            var usingItem = drawPlayer.itemAnimation > 0 && heldItem.useStyle != ItemUseStyleID.None;
            var holdingSuitableItem = heldItem.holdStyle != 0 && !drawPlayer.pulley;

            if (!drawPlayer.CanVisuallyHoldItem(heldItem))
            {
                holdingSuitableItem = false;
            }

            var ret = drawInfo.shadow != 0f || drawPlayer.JustDroppedAnItem || drawPlayer.frozen;
            ret |= !(usingItem || holdingSuitableItem) || heldItem.type <= ItemID.None || drawPlayer.dead;
            ret |= heldItem.noUseGraphic || drawPlayer.wet && heldItem.noWet || drawPlayer.happyFunTorchTime;
            ret |= drawPlayer.HeldItem.createTile == TileID.Torches && drawPlayer.itemAnimation == 0;

            return !ret;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            data[drawInfo.heldItem.type]?.Invoke(ref drawInfo);
        }
    }
}
