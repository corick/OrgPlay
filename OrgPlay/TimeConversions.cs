using System;

namespace OrgPlay
{
    public static class TimeConversions
    {
        public static TimeSpan SamplesToTimeSpan(int lengthInSamples, int sampleRate)
        {
            // fraction of second = n samples / sampleRate 
            float lengthSeconds = ((float) lengthInSamples / (float) sampleRate);
            return TimeSpan.FromSeconds(lengthSeconds);
        }

        public static TimeSpan ToTimeSpan(this int lengthInSamples, int sampleRate)
        {
            return SamplesToTimeSpan (lengthInSamples, sampleRate);
        }

        public static int TimeSpanToSamples(TimeSpan timeSpan, int sampleRate)
        {
            return sampleRate * timeSpan.TotalSeconds;
        }

        public static int ToSamples(this TimeSpan timeSpan, int sampleRate)
        {
            return TimeSpanToSamples(timeSpan, sampleRate);
        }

        public static int BeatsToSamples(int beats, int 
    }
}

