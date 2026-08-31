using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.GunSystem;
using Stellamod.Common.Shaders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Dock.WeaponsDK
{
    public class BubbleBlasher : BaseGun
    {
        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item66;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PrismaticBubble>();
            Item.shootSpeed = 5;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            muzzleOrigin = new Vector2(58, 9);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        public override void ShootEffects(Vector2 position, Vector2 velocity)
        {
            BasicMuzzleFlash(position, velocity, Color.SkyBlue, Color.DarkBlue);
            //base.ShootEffects(position, velocity);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MusicalHarmonise, BlankGun>();
        }
    }

    public class PrismaticBubbledBuff : ModBuff
    {

    }
    public class PrismaticBubbleGlobalNPC : GlobalNPC
    {
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (npc.HasBuff<PrismaticBubbledBuff>())
                return false;
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.HasBuff<PrismaticBubbledBuff>())
                return false;

            return base.PreAI(npc);
        }
    }

    public class BubbleBoom : ModProjectile
    {
        private Vector2 _scale;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle explosionSound1 = new SoundStyle("Stellamod/Assets/Sounds/JellyTome");
                explosionSound1.PitchVariance = 0.2f;
                SoundEngine.PlaySound(explosionSound1, Projectile.position);

                SoundStyle explosionSound2 = new SoundStyle("Stellamod/Assets/Sounds/Starexplosion");
                explosionSound2.PitchVariance = 0.2f;
                SoundEngine.PlaySound(explosionSound2, Projectile.position);

                FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
                float boomSize = Main.rand.NextFloat(0.025f, 0.08f);
                var p = FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);
                p.Scale *= 5;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SparklyBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                for (int i = 0; i < 64; i++)
                {
                    Vector2 fragVelocity = Main.rand.NextVector2CircularEdge(32, 32);
                    var part = FXUtil.GlowFragmentParticle(Projectile.Center + fragVelocity, fragVelocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 5),
                    innerColor: Color.White,
                    outerColor: Color.Cyan,
                    fadeToColor: Color.Purple,
                    distortOut: true);
                    part.gravity = true;

                }

            }


            _scale = Vector2.One * 0.3f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TrailRegistry.BeamTrail.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            var shader = RadialBlastShader.Instance;

            float prog = Timer / 30f;
            float interp = EasingFunction.OutExpo(prog);
            shader.Offset = Vector2.Lerp(Vector2.One * 0.25f, -Vector2.One * 0.25f, interp);
            shader.Tiling = Vector2.Lerp(Vector2.One * 4, Vector2.One * 32, interp);
            shader.InnerColor = Color.Lerp(Color.White, Color.Black, EasingFunction.OutSine(prog));
            shader.OuterColor = Color.Lerp(Color.Cyan, Color.Black, EasingFunction.OutSine(prog));
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale * 0.4f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale * 0.8f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, _scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }
    }

    public class PrismaticBubble : ModProjectile
    {

        private int _targetNPCIndex = -1;
        private int _lastHP;
        private Vector2 _stretchScale;
        private Vector2 _explodeScale;
        private Vector2 _captureScale;
        private float _flashTimer;
        private float _bubbleScale;
        private bool _doExplode;
        private ref float Timer => ref Projectile.ai[0];
        private ref float ExplodeTimer => ref Projectile.ai[1];
        private ref float WobbleTimer => ref Projectile.ai[2];
        private Player Owner => Main.player[Projectile.owner];
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_targetNPCIndex);
            writer.Write(_lastHP);
            writer.Write(_doExplode);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _targetNPCIndex = reader.ReadInt32();
            _lastHP = reader.ReadInt32();
            _doExplode = reader.ReadBoolean();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 480;
        }

        private bool HasCapturedTarget()
        {
            NPC target = GetTarget();
            return target != null && target.active;
        }

        private bool HasReleasedTarget()
        {
            NPC target = GetTarget();
            return target != null && !target.active;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (HasCapturedTarget())
                return false;
            return base.CanHitNPC(target);
        }
        private NPC GetTarget()
        {
            if (_targetNPCIndex == -1)
                return null;
            return Main.npc[_targetNPCIndex];
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            WobbleTimer++;
            if (Timer == 1)
            {
                var donut = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity, Color.Cyan);
                donut.Scale *= 0.3f;
                _stretchScale = new Vector2(1.5f, 0.3f);
            }
            else
            {
                _stretchScale = Vector2.Lerp(_stretchScale, Vector2.One, 0.06f);
            }

            if (Timer % 8 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust>(), newColor: Color.LightBlue, Scale: 0.35f);
            }
            if (_bubbleScale == 0f)
            {
                _bubbleScale = Main.rand.NextFloat(0.75f, 1f);
            }
            if (Projectile.velocity.Length() > 1 && HasCapturedTarget())
            {
                Projectile.velocity *= 0.98f;
                Projectile.velocity = Projectile.velocity.RotatedBy(0.01f);
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, Owner.Center, 2);
            }
            else if (Projectile.velocity.Length() < 10 && !HasCapturedTarget())
            {
                Projectile.velocity *= 1.05f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            NPC target = GetTarget();

            if (HasCapturedTarget())
            {
                target.AddBuff(ModContent.BuffType<PrismaticBubbledBuff>(), 2);
                PrismaticBubbleGlobalNPC bubbleNpc = target.GetGlobalNPC<PrismaticBubbleGlobalNPC>();
                if (target.life != _lastHP)
                {
                    _doExplode = true;
                }

                _flashTimer = MathHelper.Lerp(_flashTimer, 1f, 0.1f);
                _captureScale = Vector2.Lerp(_captureScale, Vector2.One, 0.1f);
                target.Center = Projectile.Center;
            }
            else if (HasReleasedTarget())
            {
                Projectile.Kill();
            }
            else
            {
                _captureScale = Vector2.Lerp(_captureScale, Vector2.One * 0.5f, 0.1f);
                NPC nearestNPC = NPCHelper.FindClosestNPC(Projectile.position, 64);
                if (nearestNPC != null)
                {
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearestNPC.Center, 18);

                }
            }

            if (_doExplode)
            {
                WobbleTimer++;
                ExplodeTimer++;

                float lerp = ExplodeTimer / 12f;
                float interp = EasingFunction.OutSine(lerp);
                _explodeScale = Vector2.Lerp(Vector2.One, Vector2.One * 1.6f, interp);
                if (ExplodeTimer >= 12f)
                {
                    //KABOOM!

                    Projectile.Kill();
                }
            }
            else
            {
                _explodeScale = Vector2.One;
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if (_doExplode)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BubbleBoom>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
            }
            for (float f = 0; f < 4; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(32, 32);
                FXUtil.GlowStretch(Projectile.Center, velocity);
            }
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            var bubbleShader = PrismaticBubbleShader.Instance;

            float lerp = Timer / 60f;
            float scaleOsc = ExtraMath.Osc(0.75f, 1f, speed: 3, Projectile.whoAmI);
            Vector2 drawScale = Projectile.scale * EasingFunction.OutExpo(lerp) * _stretchScale * scaleOsc;
            drawScale *= _captureScale;
            drawScale *= _explodeScale;
            drawScale *= _bubbleScale;
            bubbleShader.Distortion = MathHelper.Lerp(0f, 0.11f, EasingFunction.InOutSine(lerp));
            bubbleShader.Time = WobbleTimer * 0.05f;          //  bubbleShader.Power = MathHelper.Lerp(1f, 5f, ExtraMath.Osc(0f, 1f, speed: 3));
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: bubbleShader.Effect);
            for (int i = 0; i < 4; i++)
            {
                spriteBatch.Draw(texture, drawPos, null,
                    Color.White,
                    Projectile.rotation, texture.Size() / 2f,
                    drawScale, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();


            //Draw Shine
            SparkyShader sparkyShader = SparkyShader.Instance;
            sparkyShader.InnerColor = Color.White;
            sparkyShader.OuterColor = Main.DiscoColor;
            sparkyShader.Time = Timer * 0.3f;
            sparkyShader.Distortion = -0.15f;
            spriteBatch.Restart(effect: sparkyShader.Effect, blendState: BlendState.Additive);

            texture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            spriteBatch.Draw(texture, drawPos, null,
                Color.White,
                Timer * 0.005f, texture.Size() / 2f,
                drawScale * 0.8f * _flashTimer, SpriteEffects.None, 0);

            spriteBatch.Restart(blendState: BlendState.Additive, effect: null);

            texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, drawPos, null,
             Main.DiscoColor * 0.3f,
             0, texture.Size() / 2f,
            drawScale, SpriteEffects.None, 0);

            spriteBatch.Draw(texture, drawPos, null,
                 Color.Blue * 0.5f,
                 0, texture.Size() / 2f,
                drawScale * 0.5f, SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>(Texture + "_Shine").Value;
            spriteBatch.Draw(texture, drawPos, null,
                Color.White,
                0, texture.Size() / 2f,
                drawScale, SpriteEffects.None, 0);


            spriteBatch.RestartDefaults();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (_targetNPCIndex != -1)
                return;
            if (NPCSets.CannotBeBubbled[target.type])
                return;
            if (target.boss)
                return;
            if (target.HasBuff<PrismaticBubbledBuff>())
            {
                _doExplode = true;
                Projectile.netUpdate = true;
                return;
            }


            _lastHP = target.life;
            _targetNPCIndex = target.whoAmI;
            Projectile.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Item85, Projectile.position);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            _stretchScale = new Vector2(1.1f, 0.9f);
            return false;
        }
    }
}