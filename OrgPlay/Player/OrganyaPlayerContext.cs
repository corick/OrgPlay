using System;


namespace OrgPlay.Player
{
    public class OrganyaPlayerContext 
    {
        public readonly int StepMsec;
        public int StepSamples
        {
            get { return Utilities.StepsToSamples(1, StepMsec, Config.SampleRate); }
        }

        public readonly SampleProviderConfiguration Config;
        public readonly WavetableSampleLoader SampleLoader;

        public readonly int LoopBeginStep, LoopEndStep;

        public OrganyaPlayerContext(SampleProviderConfiguration config, Organya.OrganyaSong song, WavetableSampleLoader sampleLoader)
        {
            Config = config;
            SampleLoader = sampleLoader;
            StepMsec = song.StepLengthMsec;
            LoopBeginStep = song.LoopStartPositionSteps;
            LoopEndStep = song.LoopEndPositionSteps;
        }
    }
}

