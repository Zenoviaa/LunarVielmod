using Stellamod.Core;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellamod.Content.Areas.Illuria.NiiviBoss;

public class NiiviBoss : ScarletBoss
{
    public override string Texture => TextureRegistry.EmptyTexture;
    private enum AIState
    {
        Spawn,
        Despawn,
        Death,
        Idle,

        Frost_Breath,
        Comet_Star_Rain,
        Lightning_Rain,
        Prismatic_Ray,
        Prismatic_Bomb,
        Gravity_Field,
        Super_Prismatic_Ray
    }

    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return base.PreDraw(spriteBatch, screenPos, drawColor);
    }
    public override void OnKill()
    {
        base.OnKill();
    }
}
