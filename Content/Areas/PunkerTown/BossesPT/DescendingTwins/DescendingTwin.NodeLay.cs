using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public partial class DescendingTwin
    {
        private NPC GetNextNode()
        {
            int type = ModContent.NPCType<DescendingNode>();
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.type == type)
                {
                    if (npc.ai[3] == 0)
                        return npc;
                }
            }
            return null;
        }
        private void AI_RetinaNodeLaySlowStart()
        {
            Timer++;
            LayMovement();
            NPC.rotation += MathHelper.Lerp(0f, 0.1f, Timer / 120f);
            if (Timer >= 120)
            {
                SwitchState(TwinAIState.RetinaNodeLayWindup);
            }
        }
        private void AI_RetinaNodeLayWindup()
        {
            if (Timer < 1)
            {
                Timer++;
            }

            if (Timer == 1)
            {

                SoundStyle beepSound = AssetRegistry.Sounds.SteamPunking.DescendingBeep;
                beepSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(beepSound, NPC.position);
            }

            NPC nextNode = GetNextNode();
            if (nextNode == null && Commander.StopFiringAtNodes)
            {
                SwitchState(TwinAIState.NodeEnd);
            }
            if (nextNode != null)
            {
                Timer++;
                Vector2 targetNormal = (nextNode.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                Vector2 myNormal = NPC.rotation.ToRotationVector2();
                float dp = Vector2.Dot(myNormal, targetNormal);
                if (dp > 0.99f && Timer > 15)
                {
                    SwitchState(TwinAIState.RetinaNodeLayShoot);
                }
                NPC.rotation = Utils.AngleTowards(NPC.rotation, targetNormal.ToRotation(), 0.1f);

                //Aim the telegraph

                _afterImageAlpha = 0f;
                _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, 1f, 0.01f);
                _telegraphLineRot = NPC.rotation;
                TargetOutlineColor = Color.Yellow;
            }

            LayMovement();
        }

        private void AI_RetinaNodeLayShoot()
        {
            Timer++;
            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    NPC nextNode = GetNextNode();
                    if (nextNode != null)
                    {
                        nextNode.ai[3] = 1;
                        nextNode.netUpdate = true;

                        Vector2 fireVelocity = NPC.rotation.ToRotationVector2() * 8;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, fireVelocity,
                            ModContent.ProjectileType<DescendingNodeTriggeringBeam>(), DescendingNodeLaserDamage, 1, Main.myPlayer, ai1: nextNode.whoAmI);
                    }
                }
            }
            float shootTime = 5;
            float completionRatio = Timer / shootTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, ease);
            _afterImageAlpha = 0f;
            TargetOutlineColor = Color.Yellow;

            LayMovement();
            if (Timer >= shootTime)
            {
                SwitchState(TwinAIState.RetinaNodeLayWindup);
            }
        }

        private Vector2 _layStartCenter;
        private void LayMovement()
        {

            //So we should slowly move towards the player if they're far, if not we'll just hover in place.
            //Step 1. Look towards the player, we can do this by calculating a target normal, calculating an angle and then lerping to it

            //So how do we want this attack to look?
            //I think the twins should orbit around a circle for a bit, on opposite points
            //Then after a while, they look towards you and dash to the point, when they touch each other
            //They'll burst into the dash
            //Alright so

            //First we need to create a circle around our target
            _rotationTimer++;
            if (_rotationTimer == 1)
            {
                _layStartCenter = Target.Center;
                _simpleDashNormal = NPC.velocity;
            }

            float circleRadius = 250f;
            Vector2 initialDirection = -Vector2.UnitY;
            Vector2 dashVector = initialDirection * circleRadius;

            //Get an offset based on the variant that this goober is
            float radiansOffset = Variant == TwinVariant.Spazz ? MathHelper.Pi : 0;
            radiansOffset -= MathHelper.PiOver2;
            radiansOffset += _rotationTimer * 0.05f;

            Vector2 positionToMoveTo = _layStartCenter + dashVector.RotatedBy(radiansOffset);
            Vector2 velThere = positionToMoveTo - NPC.Center;
            NPC.velocity = Vector2.Lerp(_simpleDashNormal, velThere, EasingFunction.InOutSine(_rotationTimer / 120f));
        }

        private void AI_SpazzNodeLayWindup()
        {
            //For this attack we'll use an NPC for the nodes, it'll shoot a node NPC
            //Then retina will look for these npcs as long as they exist he'll be shooting them
            //Yeah, ok
            //SO first we choose a random direction
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                if (MultiplayerHelper.IsHost)
                {
                    _simpleDashNormal = TargetNormal.RotatedByRandom(1.5f);
                    NPC.netUpdate = true;
                }
            }

            if (Timer % 5 == 0)
            {
                SpawnFlameDust();
            }

            float targetAngle = _simpleDashNormal.ToRotation();

            LayMovement();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            float windUpTime = 15f;
            float completionRatio = Timer / windUpTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, ease);
            _telegraphLineRot = NPC.rotation;
            _afterImageAlpha = 0f;

            TargetOutlineColor = Color.Yellow;
            if (Timer >= windUpTime)
            {
                SwitchState(TwinAIState.SpazzNodeLayShoot);
            }
        }

        private void AI_SpazzNodeLayShoot()
        {
            Timer++;
            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    int x = (int)NPC.Center.X;
                    int y = (int)NPC.Center.Y;
                    float fireRotation = NPC.rotation;
                    NPC.NewNPC(NPC.GetSource_FromThis(), x, y,
                        ModContent.NPCType<DescendingNode>(), ai1: fireRotation);
                }
            }
            float shootTime = 5f;
            float completionRatio = Timer / shootTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, ease);
            LayMovement();
            TargetOutlineColor = Color.Yellow;
            if (Timer >= shootTime)
            {
                AttackNumber++;
                if (AttackNumber >= 12f)
                {
                    SwitchState(TwinAIState.NodeEnd);
                }
                else
                {
                    SwitchState(TwinAIState.SpazzNodeLayWindup);
                }
            }
        }

        private void AI_NodeEnd()
        {
            Timer++;
            NPC.velocity *= 0.9f;

            //Rotate towards the twarget
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);
            _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, 0f, 0.1f);
            TargetOutlineColor = Color.Transparent;
            if (Timer >= 15)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
    }
}
