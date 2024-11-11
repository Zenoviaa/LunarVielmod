using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Trails;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.IgniterExplosions
{
    internal class IgniterCardProjectile : ModProjectile
    {
        private enum CardState
        {
            Thrown,
            Exploding
        }
        private Item _cardItem;
        private List<BasePowder> _powders;
        private int _powderIndex;
        private Vector2 _explosionPos;
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
        public List<BasePowder> Powders
        {
            get
            {
                _powders ??= new List<BasePowder>();
                return _powders;
            }
            set
            {
                _powders = value;
            }
        }

        public Item CardItem
        {
            get
            {
                if (_cardItem == null)
                {
                    _cardItem = new Item();
                    _cardItem.SetDefaults(0);
                }
                return _cardItem;
            }
            set
            {
                _cardItem = value;
            }
        }

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
            writer.Write(Powders.Count);
            for (int i = 0; i < Powders.Count; i++)
            {
                writer.Write(Powders[i].Type);
            }
            writer.Write(CardItem.type);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            Powders.Clear();
            _powderIndex = reader.ReadInt32();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                BasePowder powder = ModContent.GetModItem(reader.ReadInt32()) as BasePowder;
                Powders.Add(powder);
            }
            CardItem = ModContent.GetModItem(reader.ReadInt32()).Item;
        }

        public override void AI()
        {
            base.AI();
            if (Timer < 2)
            {
                Timer++;
                if (Timer == 1 && Main.myPlayer == Projectile.owner)
                {
                    Projectile.netUpdate = true;
                }
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

        }

        private void AI_Exploding()
        {
            Timer++;

            if (Timer >= ExplosionTime)
            {
                if (_powderIndex < Powders.Count)
                {
                    BasePowder powder = Powders[_powderIndex];
                    while (powder == null && _powderIndex < Powders.Count - 1)
                    {
                        _powderIndex++;
                        powder = Powders[_powderIndex];

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

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 2.2f;
            return MathHelper.SmoothStep(baseWidth, baseWidth, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.White;
            return Color.Lerp(startColor, Color.Transparent, completionRatio);
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

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override bool PreDraw(ref Color lightColor)
        {
            //Draw Trail
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            }

            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.Dashtrail);
            Vector2 trailOffset = -Main.screenPosition + Projectile.Size / 2;
            TrailDrawer.DrawPrims(Projectile.oldPos, trailOffset, 155);

            if (State != CardState.Exploding && CardItem.ModItem != null)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                string texturePath = CardItem.ModItem.Texture;
                Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
                Vector2 drawOrigin = texture.Size() / 2f;

                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Projectile.Size / 2;
                    float drawScale = 1f;
                    Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.3f;
                    spriteBatch.Draw(texture, drawPos, null, color, Projectile.rotation, texture.Size() / 2f, drawScale, SpriteEffects.None, 0);
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }


            //Throw the Card
            if (State != CardState.Exploding && CardItem.ModItem != null)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                string texturePath = CardItem.ModItem.Texture;
                Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
                Vector2 drawPos = Projectile.Center - Main.screenPosition;
                Color drawColor = Color.White.MultiplyRGB(lightColor);
                float drawScale = 1f;
                spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, texture.Size() / 2f, drawScale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 12; i++)
            {
                Vector2 vel = -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(33)) * Main.rand.NextFloat(0.2f, 1f) * 0.5f;
                Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustID.WhiteTorch, vel, Scale: Main.rand.NextFloat(0.5f, 2f));
            }
        }
    }
}
