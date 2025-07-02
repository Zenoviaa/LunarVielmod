using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace Stellamod.Core.DashSystem
{

    internal class DashPlayer : ModPlayer
    {
        private bool _isImmune;
        // These indicate what direction is what in the timer arrays used
        public const int DashDown = 0;
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public int DashCooldown = 67; // Time (frames) between starting dashes. If this is shorter than DashDuration you can start a new dash before an old one has finished
        public int DashDuration = 30; // Duration of the dash afterimage effect in frames

        // The initial velocity.  10 velocity is about 37.5 tiles/second or 50 mph
        public float DashVelocity = 16f;

        // The direction the player has double tapped.  Defaults to -1 for no dash double tap
        public int DashDir = -1;

        // The fields related to the dash accessory
        public int DashDelay = 0; // frames remaining till we can dash again
        public int DashTimer = 0; // frames remaining in the dash
        public bool DoubleTapped = false;
        public bool DashAugmentEquipped = false;
        public BaseDashItem DashItem;
        public float DashCountTimer;
        public float MaxDashCountTimer;
        public int DashCount;
        public int MaxDashCount;
        public bool ShouldFlicker => DashCountTimer > MaxDashCountTimer / 2f;

        public override void ResetEffects()
        {
            MaxDashCountTimer = 120;
            MaxDashCount = 3;
            DashItem = null;
            DashAugmentEquipped = false;
            DoubleTapped = false;
            DashVelocity = 10f;
            DashDuration = 30;
            DashCooldown = 67;
            // ResetEffects is called not long after player.doubleTapCardinalTimer's values have been set
            // When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
            // If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
            if (Player.controlRight)
            {
                DashDir = DashRight;
            }
            else if (Player.controlLeft)
            {
                DashDir = DashLeft;
            }
            else
            {
                DashDir = Player.direction == 1 ? DashRight : DashLeft;
            }

            if ((Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
                           || (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15))
            {
                DoubleTapped = true;
            }
        }

        // This is the perfect place to apply dash movement, it's after the vanilla movement code, and before the player's position is modified based on velocity.
        // If they double tapped this frame, they'll move fast this frame
        public override void PreUpdateMovement()
        {
            // if the player can use our dash, has double tapped in a direction, and our dash isn't currently on cooldown
            if (CanUseDash() && (StellamodKeybinds.DashKeybind.JustPressed || DoubleTapped) && DashDir != -1 && DashDelay == 0 && DashCount > 0 && Main.myPlayer == Player.whoAmI)
            {
                float dashVelocity = 8;
                DashCount--;
                DashCountTimer = 0;

                Vector2 newVelocity = Player.velocity;
                switch (DashDir)
                {
                    // Only apply the dash velocity if our current speed in the wanted direction is less than DashVelocity
                    case DashLeft when Player.velocity.X > -DashVelocity:
                    case DashRight when Player.velocity.X < DashVelocity:
                        {
                            // X-velocity is set here
                            float dashDirection = DashDir == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * dashVelocity;
                            break;
                        }
                    default:
                        return; // not moving fast enough, so don't start our dash
                }

                Player.SetImmuneTimeForAllTypes(DashDuration);
                DashItem?.BeginDash(Player);
            
                DashDelay = DashCooldown;
                DashTimer = DashDuration;
                Player.velocity = newVelocity;
            }

            if (DashDelay > 0)
            {
                DashDelay--;
            }

            if (DashTimer > 0)
            {
                DashTimer--;
                Player.immune = true;
                _isImmune = true;

                float rollProgress = (float)DashTimer / (float)DashDuration;
                rollProgress = 1f - rollProgress;
                float easedRollProgress = rollProgress;
                Player.fullRotation = Player.direction == -1 ? MathHelper.Lerp(MathHelper.TwoPi, 0, easedRollProgress) : MathHelper.Lerp(0, MathHelper.TwoPi, easedRollProgress);
                Player.fullRotationOrigin = new Vector2(Player.width / 2, Player.height / 2);
                Player.armorEffectDrawShadowEOCShield = true;
                Player.velocity *= 0.98f;
                DashItem?.UpdateDash(Player);
                if (DashTimer == 0)
                {
                    Player.immune = false;
                    _isImmune = false;
                    DashItem?.EndDash(Player);
                }
            }
        }

        private bool CanUseDash()
        {
            return !Player.setSolar && !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
        }

        public override bool CanUseItem(Item item)
        {
            return !_isImmune;
        }

        public override void PostUpdate()
        {
            base.PostUpdate();

            if (DashCount < MaxDashCount)
            {
                DashCountTimer++;
                if (DashCountTimer >= MaxDashCountTimer)
                {
                    DashCount++;
                    DashCountTimer = 0;
                }

                if (DashCount >= MaxDashCount)
                {
                    DashCount = MaxDashCount;
                }
            }
        }
    }
}
