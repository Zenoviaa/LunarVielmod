using Stellamod.Assets;
using Stellamod.Common.IgnitersNPowders;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Projectiles.IgniterExplosions
{
    public class IgniterBoom : ModProjectile
    {

        private int _powderIndex;
        private bool _netUpdated;
        public BaseIgniterCard Card;

        private ref float Timer => ref Projectile.ai[0];
        private ref float ExplosionTime => ref Projectile.ai[2];
        protected Player Owner => Main.player[Projectile.owner];
        protected IgniterPlayer IgniterPlayer => Owner.GetModPlayer<IgniterPlayer>();
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_powderIndex);
            ItemIO.Send(Card.Item, writer);
        }
        
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _powderIndex = reader.ReadInt32();
            Item item = ItemIO.Receive(reader);
            Card = (BaseIgniterCard)item.ModItem;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Card == null)
                return;
            if (!_netUpdated)
            {
                Projectile.netUpdate = true;
                _netUpdated = true;
            }
            if (Timer >= ExplosionTime)
            {
                if (_powderIndex < Card.Powders.Count)
                {
                    BasePowder powder = Card.Powders[_powderIndex].ModItem as BasePowder;
                    while ((powder == null || powder.Item.IsAir) && _powderIndex < Card.Powders.Count - 1)
                    {
                        powder = Card.Powders[_powderIndex].ModItem as BasePowder;
                        _powderIndex++;
                    }

                    if (Main.myPlayer == Projectile.owner && powder != null)
                    {
                        Projectile p = powder.NewProjectile(Projectile, Projectile.Center);
                        if (IgniterPlayer.lucky && Main.rand.NextBool(4))
                        {
                            powder.NewProjectile(Projectile, Projectile.Center + Main.rand.NextVector2Circular(64, 64));
                        }

                        foreach (var addon in IgniterPlayer.addons)
                        {
                            addon.OnExplode(this);
                        }

                        ExplosionTime = p.timeLeft / 2;
                        Projectile.netUpdate = true;

                    }
                    _powderIndex++;
                }
                else
                {
                    Projectile.Kill();
                }

                Timer = 0;
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
    public class IgniterCardProjectile : ModProjectile,
        IDrawToRenderTarget
    {
        private enum CardState
        {
            Thrown,
            Exploding
        }

        private float _bounceCounter;
        private bool _init;
        private int _powderIndex;
        private Vector2 _explosionPos;
        private float _dustTimer;
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

        public BaseIgniterCard Card;

        protected Player Owner => Main.player[Projectile.owner];
        protected IgniterPlayer IgniterPlayer => Owner.GetModPlayer<IgniterPlayer>();
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
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
            Projectile.localNPCHitCooldown = 24;
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_powderIndex);
            writer.Write(_bounceCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _powderIndex = reader.ReadInt32();
            _bounceCounter = reader.ReadSingle();
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

            if (IgniterPlayer.boomerang && _dustTimer > 30)
            {
                Vector2 vel = ProjectileHelper.SimpleHomingVelocity(Projectile, Owner.Center, degreesToRotate: 7);
                Projectile.velocity = vel;
                float dist = Vector2.Distance(Projectile.Center, Owner.Center);
                if(dist <= 48)
                {
                    Projectile.Kill();
                }
            }
            if (IgniterPlayer.bouncing)
            {
                Projectile.velocity.Y += 0.3f;
            }

            if (IgniterPlayer.reverie)
            {
                if (Main.rand.NextBool(16))
                {
                    var sp = SparkleParticle.Spawn(Projectile.Center, Main.rand.NextVector2Circular(2, 2));
                    sp.outerColor = Color.Gold;
                    sp.Scale *= 0.5f;
                    sp.flickering = true;
                    sp.gravity = 0;
                    sp.noTileCollide = true;
                    sp.dampening = 0.05f;
                }
                NPC npc = NPCHelper.FindClosestNPC(Projectile.position, 256);
                if (npc != null)
                {
                    Vector2 vel = ProjectileHelper.SimpleHomingVelocity(Projectile, Owner.Center, degreesToRotate: 3);
                    Projectile.velocity = vel;
                }
            }
        }

        private void AI_Exploding()
        {
            if (this.OwnedByLocalClient() && Card != null)
            {
                var d = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                    ModContent.ProjectileType<IgniterBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                if(d.ModProjectile is IgniterBoom boom)
                {
                    boom.Card = Card;
                }
                OnExplode();
                State = CardState.Thrown;
            }
      
        }

        protected virtual void OnExplode()
        {
            Projectile.Kill();
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
            if (IgniterPlayer.bouncing)
            {
                if(_bounceCounter < 2)
                {
                    if (Projectile.velocity.X != oldVelocity.X)
                        Projectile.velocity.X *= -1;
                    if (Projectile.velocity.Y != oldVelocity.Y)
                        Projectile.velocity.Y *= -1;
                    _bounceCounter++;
                    return false;
                }
            }
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

            //Draw Trail
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Item[Card.Type].Value;
            int trailLength = Projectile.oldPos.Length;
            Vector2 drawOrigin = texture.Size() / 2f;

            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = 1f;
            if (DrawCard)
            {
                if (IgniterPlayer.reverie)
                {
                    SpritebatchDrawer goldenAuraDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, Projectile.Center);
                    goldenAuraDrawer.color = Color.Gold * ExtraMath.Osc(0.5f, 0.7f, speed: 9) * 0.4f;
                    goldenAuraDrawer.color.A = 0;
                    goldenAuraDrawer.scale *= 0.26f * ExtraMath.Osc(0.8f, 1f, speed: 6);
                    Main.spriteBatch.Draw(goldenAuraDrawer);
                }
                for (int t = 0; t < trailLength; t++)
                {
                    float l = trailLength;
                    float interpolant = t / l;
                    Vector2 oldPos = Projectile.oldPos[t];
                    oldPos -= Main.screenPosition;
                    oldPos += Projectile.Size / 2f;
                    spriteBatch.Draw(texture, oldPos, null, drawColor * MathHelper.SmoothStep(0.25f, 0f, interpolant), Projectile.oldRot[t], drawOrigin, drawScale, SpriteEffects.None, 0);
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

        }

        public virtual void DrawToRenderTargets()
        {
        
        }
    }
}
