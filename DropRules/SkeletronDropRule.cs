using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Stellamod.DropRules
{
    public class SkeletronDropRule : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedBoss3;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => "After skeletron is defeated";
    }
}
