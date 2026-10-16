using Stellamod.Common;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.NPCsJN;

public class DungeonSkeletonBone : ModProjectile
{
    public override string Texture => AssetReferences.Assets.Textures.Projectile.DungeonSkeletonBone.KEY;
    ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(16);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.timeLeft = 120;
        Projectile.light = 0.4f;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
    
        base.AI();
        Timer++;
        if(Timer >= 15)
        {
            Projectile.tileCollide = true;
        }
        Projectile.velocity.Y += 0.3f;
        Projectile.rotation += MathF.Sign(Projectile.velocity.X) * Projectile.velocity.Length() * 0.05f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Main.spriteBatch.Draw(Projectile.Drawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
public class DungeonSkeleton : ModNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Idle,
        Chase,
        Jump,
        Throw
    }

    Outliner _outliner;
    bool _contactDamage;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    const string ANIM_IDLE = "Idle";
    const string ANIM_THROW = "Throw";
    const string ANIM_WALKING = "Walkin";
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToMarsh();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.damage = 10;
        NPC.width = 58;
        NPC.height = 66;
        NPC.lifeMax = 25;
        NPC.defense = 3;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.65f;
        NPC.noGravity = false;
    }
    public override void AI()
    {
        base.AI();
        _outliner.SetDefaults();
        switch (State)
        {

        }
        _outliner.Update();
        NPC.DrawOrigin = new Vector2(9, 25);
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
        //return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    public void DrawToRenderTargets()
    {
        NPC.DrawOutlineByAnimator(_outliner);
    }
}
