using System;

namespace OrgPlay
{
    /// <summary>
    /// Represents an sound sample provider, which returns blocks of samples to the Synth controller.
    /// </summary>
    public interface ISourceSampleProvider 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>A buffer containing all of the samples in a /.</returns>
        /// <param name="offsetInSamples">Offset in samples.</param>
        float[] RequestBuffer(long offsetInSamples);
    }
}

