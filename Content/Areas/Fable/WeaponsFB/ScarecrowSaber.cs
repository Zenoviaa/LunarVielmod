using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Trailing;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class ScarecrowSaber : BaseSwingItemV2
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 10;
            Item.shoot = ModContent.ProjectileType<ScarecrowSaberBasicSlash>();
            staminaProjectileShoot = ModContent.ProjectileType<ScarecrowSaberSlash>();
            meleeWeaponType = MeleeWeaponType.Spear;
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<ScarecrowSaberPlayer>().CooldownTimer <= 0;
        }


        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(2))
            {
                // Emit dusts when the sword is swung
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.CopperCoin);
            }
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcadizScrap, BlankSword>();
        }
    }


    public class ScarecrowSaberBasicSlash : BaseSwingProjectileV2
    {
        public override void DefineCombo()
        {
            base.DefineCombo();
            SwingV2Helper.AddSpearSwingStyle(this);
            Trailer = TrailPresets.LightSpand;
            useAfterImage = true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            SoundStyle spearHit = SoundRegistry.SpearHit1;
            spearHit.PitchVariance = 0.5f;
            SoundEngine.PlaySound(spearHit, Projectile.position);
            target.AddBuff(BuffID.OnFire, 120);
            if (ComboIndex == 5)
            {
                modifiers.FinalDamage += 0.5f;
            }
        }
    }


    public class ScarecrowSaberPlayer : ModPlayer
    {
        public Vector2? DashVelocity { get; set; } = null;
        public float SlowdownTimer { get; set; }
        public bool DashRotation { get; set; }
        public float DashDirection { get; set; } = 1f;

        public float CooldownTimer { get; set; }
        public float FixRotationTimer { get; set; }
        public float FixRotationDuration { get; set; } = 15;
        public override void CopyClientState(ModPlayer targetCopy)
        {
            base.CopyClientState(targetCopy);
            ScarecrowSaberPlayer clone = targetCopy as ScarecrowSaberPlayer;
            clone.SlowdownTimer = SlowdownTimer;
            clone.DashRotation = DashRotation;
            clone.DashDirection = DashDirection;
            clone.CooldownTimer = CooldownTimer;
            clone.FixRotationTimer = FixRotationTimer;
            clone.FixRotationDuration = FixRotationDuration;
            clone.Player.velocity = Player.velocity;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            base.SyncPlayer(toWho, fromWho, newPlayer);
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.ScarecrowPlayerSync);
            packet.Write((byte)Player.whoAmI);
            packet.Write(SlowdownTimer);
            packet.Write(DashRotation);
            packet.Write(DashDirection);
            packet.Write(CooldownTimer);
            packet.Write(FixRotationTimer);
            packet.Write(FixRotationDuration);
            packet.WriteVector2(Player.velocity);
            packet.Send(toWho, fromWho);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            base.SendClientChanges(clientPlayer);
            ScarecrowSaberPlayer clone = clientPlayer as ScarecrowSaberPlayer;
            if (CooldownTimer != clone.CooldownTimer)
            {
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }
        }
        public void ReceivePlayerSync(BinaryReader reader)
        {
            SlowdownTimer = reader.ReadSingle();
            DashRotation = reader.ReadBoolean();
            DashDirection = reader.ReadSingle();
            CooldownTimer = reader.ReadSingle();
            FixRotationTimer = reader.ReadSingle();
            FixRotationDuration = reader.ReadSingle();
            Player.velocity = reader.ReadVector2();
        }

        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            //Very simple dash
            if (DashVelocity != null)
            {
                Player.velocity = DashVelocity.Value;
                DashVelocity = null;
                FixRotationTimer = 0;
            }

            if (DashRotation)
            {
                Player.fullRotation += Player.velocity.Length() * 0.015f * DashDirection;
                Player.fullRotationOrigin = Player.Size / 2;
            }

            if (FixRotationTimer > 0)
            {
                FixRotationTimer--;
                float progress = FixRotationTimer / FixRotationDuration;
                Player.fullRotation = MathHelper.Lerp(0, Player.fullRotation, progress);
            }

            if (SlowdownTimer > 0)
            {
                Player.velocity *= 0.95f;
                SlowdownTimer--;
            }

            if (CooldownTimer > 0)
            {
                CooldownTimer--;
                if (CooldownTimer == 0)
                {
                    float num = 24;
                    for (int i = 0; i < num; i++)
                    {
                        float progress = (float)i / num;
                        float rot = progress * MathHelper.TwoPi;
                        Vector2 vel = rot.ToRotationVector2() * 3;
                        Dust.NewDustPerfect(Player.Center, DustID.InfernoFork, vel, Scale: 1);
                        Dust.NewDustPerfect(Player.Center, DustID.Torch, vel * 0.75f, Scale: 1);
                    }


                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                    soundStyle.PitchVariance = 0.1f;
                    SoundEngine.PlaySound(soundStyle, Player.position);
                }
            }
        }

    }

    public class ScarecrowSaberSlash : ModProjectile
    {
        private bool _recoiled;
        private float _swingRot;
        private Vector2[] _oldSwingPos;
        private ref float Timer => ref Projectile.ai[0];
        private ref float SwingDirection => ref Projectile.ai[1];
        private ref float DeathTimer => ref Projectile.ai[2];
        private Player Owner => Main.player[Projectile.owner];
        public float holdOffset = 30;

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
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = int.MaxValue;
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
            ScarecrowSaberPlayer scarecrowSaberPlayer = Owner.GetModPlayer<ScarecrowSaberPlayer>();
            scarecrowSaberPlayer.DashRotation = true;

            Timer++;
            if (Timer == 1)
            {
                SwingDirection = 1;
                //Thrust the player
                scarecrowSaberPlayer.DashVelocity = Projectile.velocity * 2;

                //Dust Particles
                for (int k = 0; k < 7; k++)
                {
                    Vector2 newVelocity = Owner.velocity.RotatedByRandom(MathHelper.ToRadians(7));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Dust.NewDust(Owner.Bottom, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
                    Dust.NewDust(Owner.Bottom, 0, 0, DustID.InfernoFork, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
                }

                SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
            }
            if (Timer % 12 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, vel, Scale: 1);
                d.noGravity = true;
            }
            if (Timer % 6 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Torch, vel, Scale: 1);
                d.noGravity = true;
            }

            //Invincible at the start of it
            if (Timer < 20)
            {
                Owner.immune = true;
                Owner.immuneTime = 6;
            }

            //I want it to keep swinging until you stop moving basically
            //Swing Direction/Velocity
            int dir = (int)SwingDirection;
            scarecrowSaberPlayer.DashDirection = dir;

            //LMAOOOO
            _swingRot += Owner.velocity.Length() * 0.015f * dir;
            if (_recoiled)
            {
                DeathTimer++;
            }
            Point point = new Vector2(Owner.BottomLeft.X, Owner.BottomLeft.Y).ToTileCoordinates();
            Tile? floorTile = Player.GetFloorTile(point.X, point.Y);
            if ((Timer > 8 && Owner.velocity.Length() < 5 && floorTile.HasValue) || DeathTimer >= 25)
            {
                //Fix the player's orientation
                scarecrowSaberPlayer.DashRotation = false;
                scarecrowSaberPlayer.FixRotationTimer = 15;
                scarecrowSaberPlayer.CooldownTimer = 35;
                Projectile.Kill();
            }


            AI_OrientBlade();

            for (int i = _oldSwingPos.Length - 1; i > 0; i--)
            {
                _oldSwingPos[i] = _oldSwingPos[i - 1];
            }
            if (_oldSwingPos.Length > 0)
                _oldSwingPos[0] = Owner.Center + Projectile.rotation.ToRotationVector2() * holdOffset * 0.5f;
            Lighting.AddLight(Projectile.position, Color.White.ToVector3() * 0.78f);
        }

        private void AI_OrientBlade()
        {
            //Position the blade
            Vector2 position = Owner.Center;
            position += _swingRot.ToRotationVector2() * holdOffset;
            Projectile.Center = position;
            Projectile.rotation = (position - Owner.Center).ToRotation() + MathHelper.PiOver4;

            float rotation = Projectile.rotation;
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            Owner.itemRotation = rotation * Owner.direction;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (Timer < 20)
            {
                float num = 24;
                for (int i = 0; i < num; i++)
                {
                    float progress = (float)i / num;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 6;
                    Dust.NewDustPerfect(target.Center, DustID.InfernoFork, vel, Scale: 1);
                    Dust.NewDustPerfect(target.Center, DustID.Torch, vel * 0.75f, Scale: 1);
                }

                //We need some cool sounds

                //Burn the target too
                target.AddBuff(BuffID.OnFire, 120);

                //If you hit at the start of the dash, you have a damage multiplier
                modifiers.FinalDamage *= 12;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!_recoiled)
            {
                ScarecrowSaberPlayer scarecrowSaberPlayer = Owner.GetModPlayer<ScarecrowSaberPlayer>();
                scarecrowSaberPlayer.SlowdownTimer = 15;
                _recoiled = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            //Draw Trail
            Projectile.oldPos = _oldSwingPos;
            Texture2D spinTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Spiin").Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            Vector2 drawPos = Owner.Center - Main.screenPosition;
            Color drawColor = Color.LightGoldenrodYellow;

            float glowProgress = Timer / 40f;
            glowProgress = 1f - glowProgress;
            glowProgress = MathHelper.Clamp(glowProgress, 0f, 1f);
            drawColor *= glowProgress;
            float drawRotation = Projectile.rotation;
            float drawScale = 0.35f;

            for (int i = 0; i < _oldSwingPos.Length; i++)
            {
                drawPos = _oldSwingPos[i];
                float p = (float)i / (float)_oldSwingPos.Length;
                p = 1 - p;
                Color afterImageColor = drawColor * p;
                afterImageColor *= 0.15f;
                afterImageColor.A = 0;
                spriteBatch.Draw(spinTexture, drawPos - Main.screenPosition, null, afterImageColor, drawRotation, spinTexture.Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
        }
    }
}