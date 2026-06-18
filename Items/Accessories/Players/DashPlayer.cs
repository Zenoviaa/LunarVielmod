using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Players
{

    public class DashProjectile : ModProjectile
    {
        private Vector2[] _oldSwingPos;
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            _oldSwingPos = new Vector2[32];
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 64;
            Projectile.width = 64;
            Projectile.friendly = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            Projectile.Center = Owner.Center;
            for (int i = _oldSwingPos.Length - 1; i > 0; i--)
            {
                _oldSwingPos[i] = _oldSwingPos[i - 1];
            }
            if (_oldSwingPos.Length > 0)
                _oldSwingPos[0] = Owner.Center;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Owner.height * 0.62f;
            return MathHelper.SmoothStep(baseWidth, baseWidth, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, MathHelper.SmoothStep(0f, 1f, completionRatio)) * EasingFunction.QuadraticBump(completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw Trail
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelTrail);
            return false;
        }

        private void DrawPixelTrail(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            float alpha = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine((float)Projectile.timeLeft / 60f));
            shader.LaserColor = Color.Lerp(Color.White, Color.Black, alpha);
            shader.InnerColor = Color.Lerp(Color.LightGray, Color.Black, alpha);
            shader.OuterColor = Color.Lerp(Color.DarkGray, Color.Black, alpha);
            shader.LaserTexture = TrailRegistry.Dashtrail;
            TrailDrawer.Draw(Main.spriteBatch, _oldSwingPos, ColorFunction, WidthFunction, shader);
        }
    }

    public class StaminaDoubleDamage : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            base.OnSpawn(projectile, source);
            Player player = Main.player[projectile.owner];
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            if(dashPlayer.justConsumedStamina && dashPlayer.doubleStaminaCost)
            {
                projectile.damage *= 2;
            }
        }
    }

    public class DashPlayer : ModPlayer
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
        public bool IsDashing { get; private set; }
        public BaseDashItem DashItem;
        public float DashCountTimer;
        public float MaxDashCountTimer;
        public int DashCount;
        public int MaxDashCount;
        public bool ShouldFlicker => DashCountTimer > MaxDashCountTimer / 2f;

        public float DashRegenerationBonus;
        public float DashRegenerationPenalty;
        public float DashVelocityBonus;
        public int ExtraImmunityFramesBonus;
        public bool DashedThisFrame;
        public bool doubleStaminaCost;
        public bool justConsumedStamina;
        public int extraStaminaCost;
        public int dashRestoreChance;
        private HashSet<NPC> _dashedThroughSetBacking;
        public HashSet<NPC> DashedThroughSet
        {
            get
            {
                _dashedThroughSetBacking ??= new HashSet<NPC>();
                return _dashedThroughSetBacking;
            }
        }
        public static event Action<Player> OnDash;
        public static event Action<Player, int> OnUseStamina;
        public override void ResetEffects()
        {
            DashedThisFrame = false;
            ExtraImmunityFramesBonus = 0;
            DashRegenerationBonus = 0f;
            DashRegenerationPenalty = 0f;
            MaxDashCountTimer = 140;
            MaxDashCount = 3;
            DashItem = null;
            DashAugmentEquipped = false;
            DoubleTapped = false;
            DashVelocity = 10;
            DashDuration = 40;
            DashCooldown = 44;
            doubleStaminaCost = false;
            justConsumedStamina = false;
            IsDashing = DashTimer > 0;
            extraStaminaCost = 0;
            dashRestoreChance = 0;
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

        public override void CopyClientState(ModPlayer targetCopy)
        {
            base.CopyClientState(targetCopy);
            DashPlayer clone = targetCopy as DashPlayer;
            clone.DashDelay = DashDelay;
            clone.DashTimer = DashTimer;
            clone.Player.velocity = Player.velocity;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            base.SyncPlayer(toWho, fromWho, newPlayer);
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.DashPlayerSync);
            packet.Write((byte)Player.whoAmI);
            packet.Write(DashDelay);
            packet.Write(DashTimer);
            packet.WriteVector2(Player.velocity);
            packet.Send(toWho, fromWho);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            base.SendClientChanges(clientPlayer);
            DashPlayer clone = clientPlayer as DashPlayer;
            if (DashDelay != clone.DashDelay || DashTimer != clone.DashTimer)
            {
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
        }
        public void ReceivePlayerSync(BinaryReader reader)
        {
            DashDelay = reader.ReadInt32();
            DashTimer = reader.ReadInt32();
            Player.velocity = reader.ReadVector2();
        }
        // This is the perfect place to apply dash movement, it's after the vanilla movement code, and before the player's position is modified based on velocity.
        // If they double tapped this frame, they'll move fast this frame
        public override void PreUpdateMovement()
        {
            // if the player can use our dash, has double tapped in a direction, and our dash isn't currently on cooldown
            if (Main.myPlayer == Player.whoAmI && CanUseDash() && (LunarVeilKeybinds.DashKeybind.JustPressed || DoubleTapped) && DashDir != -1 && DashDelay == 0 && DashCount > extraStaminaCost)
            {
                float dashVelocity = DashVelocity;
                dashVelocity *= (1.0f + DashRegenerationBonus);
                DashedThroughSet.Clear();
                Consume(1 + extraStaminaCost);
                //DashCount-= (1 + extraStaminaCost);
                DashCountTimer = 0;
                DashedThisFrame=true;

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


                Player.SetImmuneTimeForAllTypes(DashDuration + ExtraImmunityFramesBonus);
                DashItem?.BeginDash(Player);
                OnDash?.Invoke(Player);
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<DashProjectile>(), 0, 0, Player.whoAmI);

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

        public bool CanUseDash()
        {
            return !Player.setSolar && !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
        }

        public bool CanConsume(int amount)
        {
            if (doubleStaminaCost)
                amount *= 2;
            return DashCount >= amount;
        }
        public void Consume(int amount)
        {
            if (doubleStaminaCost)
                amount *= 2;
            int rand = Main.rand.Next(0, 100);
            if (rand < dashRestoreChance)
                DashCount++;
            justConsumedStamina = true;
            DashCount -= amount;
            OnUseStamina?.Invoke(Player, amount);
        }

        public override bool CanUseItem(Item item)
        {
            return !_isImmune;
        }

        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
           
        }
        public override void PostUpdate()
        {
            base.PostUpdate();

            if (DashCount < MaxDashCount)
            {
                DashCountTimer++;

                float maxDashCountTimer = MaxDashCountTimer;


                maxDashCountTimer *= MathHelper.Lerp(1f, 0f, ExtraMath.Saturate(DashRegenerationBonus));

                float add = MaxDashCountTimer * DashRegenerationPenalty;
                maxDashCountTimer += add;
                if (DashCountTimer >= maxDashCountTimer)
                {
                    DashCount++;
                    DashCountTimer = 0;
                }

            }
            if (DashCount >= MaxDashCount)
            {
                DashCount = MaxDashCount;
            }
        }
    }
}
