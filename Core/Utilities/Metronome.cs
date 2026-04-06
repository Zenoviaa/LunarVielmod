namespace Stellamod.Core.Utilities
{
    public class Metronome
    {
        public Metronome(float bpm)
        {
            this.bpm = bpm;
        }
        public float bpm;
        public bool beatHit;
        public float beatCounter;
        public float localBeatCounter;
        public float beatTimer;

        public void Update()
        {
            float beatsPerTick = 150f / 60f / 60f;
            beatTimer += beatsPerTick;

            beatHit = false;
            while (beatTimer >= 1f)
            {
                beatTimer -= 1f;
                beatCounter++;
                localBeatCounter++;
                beatHit = true;
            }
        }
    }
}
