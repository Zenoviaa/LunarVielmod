using Stellamod.Core.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.Niivi;


public class NiiviSnatched : ModBuff
{
    public override string Texture => AssetReferences.Assets.Textures.Buffs.NiiviSnatched.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.debuff[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        base.Update(player, ref buffIndex);
    }
}

public class NiiviSnatchPlayer : ModPlayer
{
    public int snatchNpc;
    public override void ResetEffects()
    {
        base.ResetEffects();
        snatchNpc = -1;
    }

    public override void PreUpdateMovement()
    {
        base.PreUpdateMovement();
        if(snatchNpc != -1)
        {
            var npc = Main.npc[snatchNpc];
            var velocity = (npc.position - npc.oldPosition);
            Player.position += velocity;
        }
    }

}
public partial class Niivi
{
    float RainbowRoad_Time => 1200;
    float RainbowRoad_OutPrepTime => 70;
    float SqrSnatchDistance => 100 * 100;
    void AI_RainbowRoad()
    {
        void SnatchPlayers()
        {

        }
        SnatchPlayers();
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                    }

                    _outliner.warning = true;
                    _simpleChainRendering = true;
                    LimpIKTarget();


                    //Niivi flies off to the right of the screen
                    //Just increase x velocity, also make her go up slightly
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 16, 0.3f);
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, -0.5f, 0.2f);
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.2f);
                    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.3f));
                    
                    //After some time, she needs to turn around and bee line straight towards you
                    //So just directly move towards the player
                    if(Timer >= RainbowRoad_OutPrepTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 1:
                {
                    _simpleChainRendering = true;
                    LimpIKTarget();
                    //So at this point, niivi needs ot go straigh fo rthe playe
                    var velToPlayer = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    var targetVel = velToPlayer * MathF.Min(35, Vector2.Distance(NPC.Center, MyTarget.Center));
                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVel, 0.25F);
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.2f);
                    CameraTargetSystem.AddTarget(Vector2.Lerp(Main.LocalPlayer.Center, NPC.Center, 0.3f));

                    var sqrDistToPlayer = Vector2.Distance(NPC.Center, MyTarget.Center);
                    if(sqrDistToPlayer < SqrSnatchDistance)
                    {
                        Timer = 0;
                        AttackCycle++;
                        foreach(var player in Main.ActivePlayers)
                        {
                            var sqrDistToThisPlayer = Vector2.DistanceSquared(player.Center, NPC.Center);
                            if(sqrDistToThisPlayer < SqrSnatchDistance)
                            {
                                player.AddBuff(ModContent.BuffType<NiiviSnatched>(), (int)RainbowRoad_Time);
                            }
                        }
                    }
                }
                break;

            case 2:
                {

                }
                break;
        }
    }
}
