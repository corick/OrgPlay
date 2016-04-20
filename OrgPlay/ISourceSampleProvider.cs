using System;

namespace OrgPlay
{
    public interface ISourceSampleProvider 
    {
        float[] RequestBuffer(long lengthSamples);
    }
}

