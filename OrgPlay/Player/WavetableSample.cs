using System;


namespace OrgPlay.Player
{
    /// <summary>
    /// A wavetable sample, which is sampled at a specific point frequency to get the value ...
    /// </summary>
    public class WavetableSample //FIXME: Shouldn't this just be a float[]?
    {
        public readonly float[] Samples;

        public WavetableSample(float[] samples)
        {
            Samples = samples;
        }
    }
}

