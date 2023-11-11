using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Skies
{
    class VignettePlayer : ModPlayer
	{
		private bool _lastTickVignetteActive;
		private bool _vignetteActive;
		private Vector2 _targetPosition;
		private float _opacity;
		private float _radius;
		private float _fadeDistance;
		private Color _color;

		public override void ResetEffects()
		{
			_lastTickVignetteActive = _vignetteActive;
			_vignetteActive = false;
		}

		public void SetVignette(float radius, float colorFadeDistance, float opacity) => SetVignette(radius, colorFadeDistance, opacity, Color.Black, Main.screenPosition);

		public void SetVignette(float radius, float colorFadeDistance, float opacity, Color color, Vector2 targetPosition)
		{
			_radius = radius;
			_targetPosition = targetPosition;
			_fadeDistance = colorFadeDistance;
			_color = color;
			_opacity = opacity;
			_vignetteActive = true;
		}

	
	}
}