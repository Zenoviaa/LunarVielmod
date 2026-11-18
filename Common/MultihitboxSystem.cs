using Mono.Cecil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common;

public static class MultihitboxSets
{
    extension(NPCID.Sets)
    {
        public static bool[] UsesMultiHitbox
        {
            get
            {
                return MultihitboxHelper.UsesMultiHitbox;
            }
            set
            {
                MultihitboxHelper.UsesMultiHitbox = value;
            }
        }
    }
}
public interface IUseMultihitbox
{
    int NeededHitboxes { get; }
    void AddExtraHitboxes(ref Rectangle[] hitboxes, in int start, in int end);
}

public record struct ExtraHitboxKey(int Left, int Right);

public static class MultihitboxHelper
{
    public static readonly List<IUseMultihitbox> usingHitboxes = new();
    public static bool[] UsesMultiHitbox;
    public static Rectangle[] extraHitboxes;
    public static ExtraHitboxKey[] npcExtraHitboxRange;

    public static bool IsProjectileCollidingWithNPC(Projectile projectile, NPC npc, bool isAlreadyColliding)
    {
        if (isAlreadyColliding)
            return isAlreadyColliding;
        if (NPCID.Sets.UsesMultiHitbox[npc.type])
        {
            var isColliding = false;

            var projRect = projectile.getRect();
            var key = npcExtraHitboxRange[npc.whoAmI];
            for (var i = key.Left; i < key.Right; i++)
            {
                ref Rectangle extraNpcHitbox = ref extraHitboxes[i];
                if (projectile.Colliding(projRect, extraNpcHitbox))
                {
                    isColliding = true;
                    break;
                }
            }
            return isColliding;
        }
        return false;
    }

}
public class MultihitboxSystem : ModSystem
{

    public override void Load()
    {
        base.Load();
        IL_Projectile.Damage += CheckForMultihitboxes;
    }

    private void CheckForMultihitboxes(ILContext il)
    {
        try
        {

            ILCursor c = new ILCursor(il);

            if(c.TryGotoNext(
                MoveType.After,
                i => i.MatchCall<Projectile>($"{nameof(Projectile.Colliding)}")
            ))
            {
                if (c.TryGotoNext(
                    MoveType.After,
                    i => i.MatchCall<Projectile>($"{nameof(Projectile.Colliding)}")
                ))
                {
                    if(c.TryGotoNext(
                        MoveType.After, 
                        i => i.MatchStloc(23)))
                    {
                        /*
                        if(c.TryGotoPrev(MoveType.Before, i => i.MatchLdarg0()))
                        {
                            c.RemoveRange(8);
                        }*/
                        //Right after the normal hitbox call we call this
                        //Remove the rectangle thingies
                        //projectile parameter
                        c.EmitLdarg0();

                        //index into npc array
                        c.EmitDelegate(() => Main.npc);
                        c.EmitLdloc(15);
                        c.EmitLdelemRef();

                        //flag11 which is the original collision check
                        //so we skip code if it's alr colliding
                        c.EmitLdloc(23);
                        c.EmitDelegate(MultihitboxHelper.IsProjectileCollidingWithNPC);
                        
                        //set it back
                        c.EmitStloc(23);
                    }

                }
            }


        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(ModContent.GetInstance<Stellamod>(), il);
        }

    }

    public override void ResizeArrays()
    {
        base.ResizeArrays();
        MultihitboxHelper.UsesMultiHitbox = NPCID.Sets.Factory.CreateBoolSet(false);
        MultihitboxHelper.npcExtraHitboxRange = new ExtraHitboxKey[Main.maxNPCs];
    }

    public override void PostUpdateNPCs()
    {
        base.PostUpdateNPCs();
        int neededHitboxes = 0;
        MultihitboxHelper.usingHitboxes.Clear();
        foreach (var npc in Main.ActiveNPCs)
        {
            if(npc.ModNPC is IUseMultihitbox mb)
            {
                MultihitboxHelper.npcExtraHitboxRange[npc.whoAmI].Left = neededHitboxes;
                MultihitboxHelper.npcExtraHitboxRange[npc.whoAmI].Right = neededHitboxes + mb.NeededHitboxes;
                neededHitboxes += mb.NeededHitboxes;
                MultihitboxHelper.usingHitboxes.Add(mb);
            }
        }
        if (neededHitboxes <= 0)
            return;

        if (MultihitboxHelper.extraHitboxes == null || MultihitboxHelper.extraHitboxes.Length != neededHitboxes)
        {
            MultihitboxHelper.extraHitboxes = new Rectangle[neededHitboxes];
        }

        var start = 0;
        foreach(var usingHitbox in MultihitboxHelper.usingHitboxes)
        {
            usingHitbox.AddExtraHitboxes(ref MultihitboxHelper.extraHitboxes, start, start + usingHitbox.NeededHitboxes);
            start += usingHitbox.NeededHitboxes;
        }
    }
}
