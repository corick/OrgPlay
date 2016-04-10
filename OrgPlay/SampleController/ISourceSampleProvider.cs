using System;

namespace OrgPlay.SampleController
{
    /// <summary>
    /// Represents an instrument, managed by the Synth controller.
    /// </summary>
    public interface ISourceSampleProvider 
    {
        float[] RequestBuffer(long offsetInSamples);
    }
}

