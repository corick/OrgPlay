using System;

namespace OrgPlay
{
    public static class Utilities
    {
        public static TimeSpan SamplesToTimeSpan(int lengthInSamples, int sampleRate)
        {
            // fraction of second = n samples / sampleRate 
            float lengthSeconds = ((float) lengthInSamples / (float) sampleRate);
            return TimeSpan.FromSeconds(lengthSeconds);
        }

        public static float SamplesToMsec(int lengthInSamples, int sampleRate)
        {
            float lengthSeconds = ((float) lengthInSamples / (float) sampleRate);
            return lengthSeconds / 1000;
        }

        public static TimeSpan ToTimeSpan(this int lengthInSamples, int sampleRate)
        {
            return SamplesToTimeSpan (lengthInSamples, sampleRate);
        }

        public static int TimeSpanToSamples(TimeSpan timeSpan, int sampleRate)
        {
            return (int)Math.Floor(sampleRate * timeSpan.TotalSeconds);
        }

        public static int ToSamples(this TimeSpan timeSpan, int sampleRate)
        {
            return TimeSpanToSamples(timeSpan, sampleRate);
        }

        public static int StepsToSamples(int steps, int stepTimeMsec, int sampleRate)
        {
            return TimeSpanToSamples(new TimeSpan(0, 0, 0, 0, stepTimeMsec), sampleRate) * stepTimeMsec;
        }

        public static float SamplesToStepsFloat(int samples, int stepTimeMsec, int sampleRate)
        {
            var seconds = ((double) samples) / ((double) sampleRate);
            var msec = seconds * 1000;
            return (float) (msec / stepTimeMsec);
        }

        public static int SamplesToSteps(int samples, int stepTimeMsec, int sampleRate)
        {
            return (int)SamplesToStepsFloat(samples, stepTimeMsec, sampleRate);
        }

        public static void SplitChannel(float inSample, float pan, out float left, out float right)
        {
            //0: No change
            //1: All Right
            //-1: All Left
            left = inSample;
            right = inSample;
        }
        
    }
}

