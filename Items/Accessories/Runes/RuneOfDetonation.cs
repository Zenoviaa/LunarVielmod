using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Items.Accessories.Runes
{
    internal class RuneOfDetonationBomb : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer % 10 == 0)
            {
                Vector2 explosionPosition = Projectile.Center;
                explosionPosition += Main.rand.NextVector2Circular(32, 32);

                if(Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), explosionPosition, Vector2.Zero,
                        ModContent.ProjectileType<FableExSps>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }


                //Effects
                SoundStyle explosionSoundStyle = new SoundStyle($"Stellamod/Assets/Sounds/HeatExplosion");
                explosionSoundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(explosionSoundStyle, Projectile.position);

                MyPlayer myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
                myPlayer.ShakeAtPosition(Projectile.Center, 1024, 8);
            }
        }
    }

    internal class RuneOfDetonationPlayer : ModPlayer
    {
        private int _lastTargetIndex;
        private int _hitCount;
        public bool hasDetonationRune;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasDetonationRune = false;
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (!hasDetonationRune)
            {
                _lastTargetIndex = -1;
                _hitCount = 1;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!hasDetonationRune)
                return;
            if (_lastTargetIndex != target.whoAmI)
            {
                _hitCount = 1;
                _lastTargetIndex = target.whoAmI;
            }
            else
            {
                _hitCount++;
            }

            if (_hitCount >= 5)
            {
                float damage = damageDone;
                damage *= 0.33f;
                damage = MathHelper.Clamp(damage, 0, 33);

                Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<RuneOfDetonationBomb>(), (int)damage, hit.Knockback, Player.whoAmI);
                _hitCount = 1;
            }
        }
    }

    internal class RuneOfDetonation : BaseRune
    {
        public override void SetDefaults()
        {
            base.SetDefaults(); 
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            RuneOfDetonationPlayer detonationPlayer = player.GetModPlayer<RuneOfDetonationPlayer>();
            detonationPlayer.hasDetonationRune = true;
        }
    }
}