using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.CommonMaterials;
using Stellamod.Helpers;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH
{
    public class IvynthornChoker : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 30;
            Item.height = 40;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<IvynthornChokerPlayer>().hasChoker = true;
        }


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Ivythorn, BlankAccessory>();
        }
    }

    public class IvynthornChokerPlayer : ModPlayer
    {
        private float _timer;
        public bool hasChoker;
        public int thornsDamage;

        private float TimeBetweenDamageIncreases = 15;
        private int MaxThornsDamage => 30;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasChoker = false;
        }

        public override void UpdateEquips()
        {
            base.UpdateEquips();
            if (!hasChoker)
                return;
            _timer++;
            if (_timer >= TimeBetweenDamageIncreases)
            {
                _timer = 0;
                thornsDamage++;
                if (thornsDamage >= MaxThornsDamage)
                {
                    thornsDamage = MaxThornsDamage;
                }
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            base.OnHurt(info);
            if (!hasChoker)
                return;

            float numToSpawn = Main.rand.Next(3, 5);
            for (float f = 0; f < numToSpawn; f++)
            {
                float rot = Main.rand.NextFloat(0f, 1f) * MathHelper.TwoPi;
                rot += Main.rand.NextFloat(-1f, 1f);
                Vector2 vel = rot.ToRotationVector2() * 2;
                Vector2 spawnPoint = Player.Center + vel * 36;
                Projectile.NewProjectile(Player.GetSource_FromThis(), spawnPoint, vel,
                    ModContent.ProjectileType<IvynthornChokerVine>(), thornsDamage, 0, Player.whoAmI);
            }
            thornsDamage = 0;
            SoundStyle soundStyle = AssetRegistry.Sounds.Magic.VineWrap;
            soundStyle.PitchVariance = 0.3f;
            SoundEngine.PlaySound(soundStyle, Player.position);
            FXUtil.ShakeCamera(Player.position, 32, 1);
        }
    }

    public class IvynthornChokerVine : ModProjectile
    {
        private int LifeTime => 45;
        private ref float Timer => ref Projectile.ai[0];
        private ref float State => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 5;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = LifeTime;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //Check if the sword is colliding, this does a line check instead of terraria default box.
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            float length = texture.Width / 2 + texture.Height / 2;

            Vector2 start = Projectile.Center - Projectile.rotation.ToRotationVector2() * length;
            Vector2 end = Projectile.Center + Projectile.rotation.ToRotationVector2() * length;
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.scale, ref collisionPoint);
        }


        public override void AI()
        {
            base.AI();
            Projectile.Center = Main.player[Projectile.owner].Center;
            Projectile.Center += Projectile.velocity.SafeNormalize(Vector2.Zero) * 36;
            Projectile.Center -= new Vector2(24, 0);
            Timer++;
            if (Timer == 1)
            {
                FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Brown, Color.RosyBrown, 8, 0.05f);
                float num = 8;
                for (float i = 0; i < num; i++)
                {
                    float l = (i) / num;
                    float rot = l * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 2;
                    Dust.NewDustPerfect(Projectile.Center, DustID.t_LivingWood, vel);
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            float inFrameSpeed = 3;
            float outFrameSpeed = 7;
            switch (State)
            {
                case 0:
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= inFrameSpeed)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;

                        if (Projectile.frame >= Main.projFrames[Projectile.type])
                        {
                            Projectile.frame = Main.projFrames[Projectile.type] - 1;
                            State = 1;
                        }
                    }

                    break;
                case 1:
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= outFrameSpeed)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame--;
                        if (Projectile.frame <= 0)
                        {
                            Projectile.frame = 0;
                        }
                    }
                    break;
            }

        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float num = 8;
            for (float i = 0; i < num; i++)
            {
                float l = (i) / num;
                float rot = l * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 2;
                Dust.NewDustPerfect(Projectile.Center, DustID.t_LivingWood, vel);
            }
        }
    }
}
