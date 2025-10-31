using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.IgnitersNPowders;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.IgniterExplosions
{
    public class IgniterCardProjectile : ModProjectile
    {
        private enum CardState
        {
            Thrown,
            Exploding
        }

        private bool _init;
        private int _powderIndex;
        private Vector2 _explosionPos;
        private float _dustTimer;
        private ref float Timer => ref Projectile.ai[0];
        private CardState State
        {
            get
            {
                return (CardState)Projectile.ai[1];
            }
            set
            {
                Projectile.ai[1] = (float)value;
            }
        }

        private ref float ExplosionTime => ref Projectile.ai[2];
        public BaseIgniterCard Card;

        private Player Owner => Main.player[Projectile.owner];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_powderIndex);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _powderIndex = reader.ReadInt32();
        }

        public override void AI()
        {
            base.AI();
            if (!_init)
            {
                Card = Owner.HeldItem.ModItem as BaseIgniterCard;
                _init = true;
            }

            switch (State)
            {
                case CardState.Thrown:
                    AI_Thrown();
                    break;
                case CardState.Exploding:
                    AI_Exploding();
                    break;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        private void AI_Thrown()
        {
            _dustTimer++;
            if (_dustTimer % 16 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.WhiteTorch);
            }
        }

        private void AI_Exploding()
        {
            Timer++;

            if (Timer >= ExplosionTime)
            {
                if (_powderIndex < Card.Powders.Count)
                {
                    BasePowder powder = Card.Powders[_powderIndex].ModItem as BasePowder;
                    while (powder == null && _powderIndex < Card.Powders.Count - 1)
                    {
                        _powderIndex++;
                        powder = Card.Powders[_powderIndex].ModItem as BasePowder;
                    }

                    if (Main.myPlayer == Projectile.owner && powder != null)
                    {
                        Projectile p = powder.NewProjectile(Projectile, _explosionPos);
                        ExplosionTime = p.timeLeft / 2;
                        Projectile.netUpdate = true;
                        _powderIndex++;
                    }
                }

                Timer = 0;
            }


            Projectile.velocity = Vector2.Zero;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            SoundStyle ignitionSound = new SoundStyle("Stellamod/Assets/Sounds/clickk");
            ignitionSound.PitchVariance = 0.15f;
            SoundEngine.PlaySound(ignitionSound, Projectile.position);

            _explosionPos = target.Center;
            State = CardState.Exploding;
            Projectile.netUpdate = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            _explosionPos = Projectile.Center;
            State = CardState.Exploding;
            Projectile.netUpdate = true;
            return false;
        }

        private bool DrawCard => State != CardState.Exploding && Card != null;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Card == null)
                return false;
            string texturePath = Card.Texture;
            //Draw Trail
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int trailLength = Projectile.oldPos.Length;
            Vector2 drawOrigin = texture.Size() / 2f;

            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = 1f;


            if (DrawCard)
            {
                for (int t = 0; t < trailLength; t++)
                {
                    float l = trailLength;
                    float interpolant = (float)t / l;
                    Vector2 oldPos = Projectile.oldPos[t];
                    oldPos -= Main.screenPosition;
                    oldPos += Projectile.Size / 2f;
                    spriteBatch.Draw(texture, oldPos, null, drawColor * MathHelper.SmoothStep(0.5f, 0f, interpolant), Projectile.oldRot[t], drawOrigin, drawScale, SpriteEffects.None, 0);
                }
            }

            //Throw the Card
            if (DrawCard)
            {
                Vector2 drawPos = Projectile.Center - Main.screenPosition;
                spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, texture.Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            return false;
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 10; i++)
            {
                Vector2 vel = -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(33)) * Main.rand.NextFloat(0.2f, 1f) * 0.5f;
                Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustID.WhiteTorch, vel, Scale: Main.rand.NextFloat(0.5f, 2f));
            }
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGray, Color.Black);
        }
    }
}
