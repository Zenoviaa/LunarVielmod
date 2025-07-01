using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Helpers;
using Stellamod.Common.Helpers.Math;
using Stellamod.Items.Forms;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.MagicSystem
{
    internal class MagicProjectile : ModProjectile
    {
        private List<Enchantment> _enchantments;
        private Element _element;
        private Form _form;
        private bool _initialized;
        private float _easeInTimer;


        private Player Owner => Main.player[Projectile.owner];

        public override string Texture => AssetHelper.EmptyTexture;
        public Element Element
        {
            get
            {
                if (_element == null)
                    _element = new NoElement();
                return _element;
            }
            set
            {
                _element = value;
            }
        }

        //Lazy loading so it's never null
        public List<Enchantment> Enchantments
        {
            get
            {
                if (_enchantments == null)
                    _enchantments = new List<Enchantment>();
                return _enchantments;
            }

        }


        public Form Form
        {
            get
            {
                if(_form == null)
                {
                    //Default to hammer form
                    _form = new Hammer();
                }
                return _form;
            }
        }

        public float EaseInLerp { get; private set; }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //Just in case we want to render trails, but I don't think we will need it.
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            base.AI();
            _easeInTimer++;
            EaseInLerp = EasingFunction.OutExpo(_easeInTimer / 80f);

            //By initializing this way, it guarantees that when netcoding everyone can see what's happening
            //Without even needing to do a big netsync
            if (!_initialized)
            {
                if (Owner.HeldItem.ModItem is Staff staff)
                {
                    Element = staff.Element;
                    Enchantments.Clear();
                    foreach (var enchantment in staff.GetEnchantments())
                    {
                        Enchantments.Add(enchantment);
                    }
                    _initialized = true;
                }
            }

            //This is where we'll put all the custom AI         
            //Update the element ai first.
            Element.AI(this);
            for (int i = 0; i < Enchantments.Count; i++)
            {
                Enchantment enchantment = Enchantments[i];
                enchantment.AI(this);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Element.DrawForm(this, spriteBatch, ref lightColor);
            DrawFormSprite(ref lightColor);
            return false;
        }

        public void DrawFormSprite(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            //Now we draw the form
            float drawScale = MathHelper.Lerp(0.125f, 1f, EaseInLerp);
            Asset<Texture2D> formTexture = ModContent.Request<Texture2D>(Form.Texture);
            Vector2 drawOrigin = formTexture.Size() / 2f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.velocity.ToRotation();
            spriteBatch.Draw(formTexture.Value, drawPosition, null, Color.Black, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }
    }
}
