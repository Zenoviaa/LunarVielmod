using ReLogic.Content;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.SummonerSystem
{
    public class BellMinionExpandableTooltip : AbstractExpandingTooltip
    {
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            BellMinionGlobalItem minionGlobal = item.GetGlobalItem<BellMinionGlobalItem>();
            if (minionGlobal.isGuardian)
            {
                string path = this.GetType().DirectoryHere() + "/UI/GuardianSymbol";
                Asset<Texture2D> iconTextureAsset = ModContent.Request<Texture2D>(path);
                Vector2 drawOrigin = iconTextureAsset.Size() * 0.5f;
                spriteBatch.Draw(iconTextureAsset.Value, position + new Vector2(12, 0), null, Color.White, 0, drawOrigin, scale * 0.75f, SpriteEffects.None, 0);
            }
        }

        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            BellMinionGlobalItem bellMinion = item.GetGlobalItem<BellMinionGlobalItem>();
            if (bellMinion.isGuardian)
            {
                TooltipLine line = new TooltipLine(Mod, "GuardianHelp", LangText.Common("GuardianHelp"));
                lines.Add(line);
            }
            else if (bellMinion.isBellMinion)
            {
                TooltipLine line = new TooltipLine(Mod, "MinionHelp", LangText.Common("BellMinionHelp"));
                lines.Add(line);
            }
        }
    }

    public class BellMinionGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool isBellMinion;
        public bool isGuardian;
        public float addedCastingTime;
        public int health;
        public override bool CanUseItem(Item item, Player player)
        {
            if (isBellMinion)
                return false;
            return base.CanUseItem(item, player);
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            if (isBellMinion)
            {
                float seconds = addedCastingTime / 60f;
                string secondsString = seconds.ToString("#.#");
                TooltipLine line = new TooltipLine(Mod, "AmountOfCastingTime",
                    LangText.Common("CastingTime", secondsString));
                line.OverrideColor = Color.Lerp(new Color(80, 187, 180), Color.Black, 0.25f);
                tooltips.Add(line);

                line = new TooltipLine(Mod, "Lifetime",
                    LangText.Common("MinionLifetime", health));
                line.OverrideColor = Color.Lerp(new Color(80, 187, 180), Color.Black, 0.25f);
                tooltips.Add(line);
            }
        }
    }
}
