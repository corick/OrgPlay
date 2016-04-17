using System;

namespace OrgPlay.SampleController
{
    public interface ISourceSampleProvider 
    {
        float[] RequestBuffer(long lengthSamples);
    }
}

