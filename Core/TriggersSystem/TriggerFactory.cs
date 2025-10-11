using Stellamod.Core.TriggersSystem.Triggers;

namespace Stellamod.Core.TriggersSystem
{
    public static class TriggerFactory
    {
        public static Trigger Create(TriggerID id)
        {
            Trigger trigger;
            switch (id)
            {
                default:
                case TriggerID.NPCSpawnTrigger:
                    trigger = new NPCSpawnTrigger();
                    break;
                case TriggerID.BossSpawnTrigger:
                    trigger = new BossSpawnTrigger();
                    break;
            }
            trigger.id = (int)id;
            return trigger;
        }
    }
}
