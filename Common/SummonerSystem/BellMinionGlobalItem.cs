using ReLogic.Content;
using Stellamod.Core;
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

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (isBellMinion || isGuardian)
            {
                if (isGuardian)
                {
                    var auraDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Main.screenPosition + position);
                    auraDrawer.color = Color.SkyBlue;
                    auraDrawer.color.A = 0;
                    auraDrawer.color *= ExtraMath.Osc(0.45f, 0.55f, speed: 2, item.type) * 0.4f;
                    auraDrawer.scale *= 0.2f;
                    spriteBatch.Draw(auraDrawer);
                }
                var scrollDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Common.SummonerSystem.SongScroll.Asset, Main.screenPosition + position);
                scrollDrawer.color = drawColor;
                scrollDrawer.worldPosition += new Vector2(-8);
                spriteBatch.Draw(scrollDrawer);

                var drawer = SpritebatchDrawer.FromItemInUI(item);
                drawer.worldPosition = Main.screenPosition + position;
                drawer.color = drawColor;
                if(!isGuardian)
                    drawer.color *= ExtraMath.Osc(0.5f, 1f, speed: 2, item.type);
                drawer.sourceRect = frame;
                drawer.CenterOrigin();
                drawer.scale = Vector2.One * scale;
                spriteBatch.Draw(drawer);

                var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Main.screenPosition + position);
                glowDrawer.color = Color.SkyBlue;
                glowDrawer.color.A = 0;
                glowDrawer.color *= ExtraMath.Osc(0.45f, 0.55f, speed: 2, item.type) * 0.4f;
                glowDrawer.scale *= 0.1f;
                spriteBatch.Draw(glowDrawer);

                return false;
            }
            else
            {
                return true;
            }

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
