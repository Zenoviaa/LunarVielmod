using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    public class GlobalNPCSucker : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public Vector2 SuckVelocity;
        public override bool PreAI(NPC npc)
        {
            if (SuckVelocity.Length() > 2)
            {
                SuckVelocity = Vector2.Lerp(SuckVelocity, Vector2.Zero, 0.2f);
                npc.velocity = SuckVelocity;
            }

            return base.PreAI(npc);
        }
    }
    public class NPCSuckerTarget
    {
        public int npcToPull;
        public Vector2 npcOffset;
    }

    public class NPCSucker
    {
        private List<NPCSuckerTarget> _targets;
        public NPCSucker()
        {
            _targets = new();
        }

        //  public bool IgnoreKBResist;
        public bool HasSuckerTarget(NPC npc)
        {
            return _targets.Find(x => x.npcToPull == npc.whoAmI) != null;
        }

        public void AddNPCSuckerTarget(Vector2 projectilePosition, NPC npc)
        {
            if (HasSuckerTarget(npc))
                return;
            int npcToPull = npc.whoAmI;
            Vector2 offset = npc.Center - projectilePosition;
            NPCSuckerTarget target = new NPCSuckerTarget
            {
                npcToPull = npcToPull,
                npcOffset = offset
            };
            _targets.Add(target);
        }

        public void AI(Vector2 position, float strength = 0.2f)
        {
            _targets = _targets.Where(x => Main.npc[x.npcToPull].active).ToList();
            foreach (NPCSuckerTarget target in _targets)
            {
                NPC npc = Main.npc[target.npcToPull];
                var sucker = npc.GetGlobalNPC<GlobalNPCSucker>();

                Vector2 diff = position - npc.Center;
                Vector2 velocity = Vector2.Lerp(Vector2.Zero, diff, strength) * npc.knockBackResist;
                Vector2 diffVelocity = velocity - sucker.SuckVelocity;
                sucker.SuckVelocity += diffVelocity;
            }
        }

        public void ResetVelocity()
        {
            _targets = _targets.Where(x => Main.npc[x.npcToPull].active).ToList();
            foreach (NPCSuckerTarget target in _targets)
            {
                NPC npc = Main.npc[target.npcToPull];
                npc.position += npc.velocity;
                npc.velocity = Vector2.Zero;
            }
        }
    }
}
