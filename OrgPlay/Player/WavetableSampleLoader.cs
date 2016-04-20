using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OrgPlay.Player
{
    public class WavetableSampleLoader
    {
        private static readonly int INSTRUMENT_BANK_SAMPLE_LENGTH_SAMPLES = 256;


        private Dictionary<int, WavetableSample> _samples;
        private readonly string _baseDir;
        public WavetableSampleLoader (string baseDir = "./Samples")
        {
            _samples = new Dictionary<int, WavetableSample>();
            _baseDir = baseDir;
        }

        public WavetableSample Load(int sampleBank)
        {
            if(_samples.ContainsKey(sampleBank))
                return _samples[sampleBank];

            if (sampleBank > 99)
            {
                var drum = LoadDrumSample(sampleBank);
                _samples.Add(sampleBank, drum);
                return drum;
            } 
            else
            {
                var inst = LoadInstrumentSample(sampleBank);
                _samples.Add(sampleBank, inst);
                return inst;
            }
        }

        private WavetableSample LoadInstrumentSample(int sampleBank)
        {
            //It's 8bit signed if it's in wave100.dat.
            var path = Path.Combine(_baseDir, "wave100.dat");
            var sampleUnsigned = File.ReadAllBytes(path)
                .Skip(sampleBank * INSTRUMENT_BANK_SAMPLE_LENGTH_SAMPLES)
                .Take(INSTRUMENT_BANK_SAMPLE_LENGTH_SAMPLES)
                .ToArray();

            sbyte[] sample = new sbyte[sampleUnsigned.Length];
            Buffer.BlockCopy(sampleUnsigned, 0, sample, 0, sampleUnsigned.Length);

            return new WavetableSample(
                sample
                .Select<sbyte, float> (
                    //Normalize sbytes to floats.
                    (s) => ((float) s) / ((float) sbyte.MaxValue)
                )
                .ToArray()
            );
        }

        private WavetableSample LoadDrumSample(int sampleBank)
        {
            //It's 16bit shorts for the drum tracks though.
            //TODO: Extract these samples from doukutsu.exe myself! I'm still not 100% where these came from.
            var path = Path.Combine(_baseDir, String.Format("{0}.dat", sampleBank.ToString())); //Always >100 so no padding necessary.
            var sample = File.ReadAllBytes(path);

            var sampleShortsConverted = new List<short>();
            for (int i = 0; i < sample.Length; i += 2)
            {
                byte hi = sample[i];
                byte lo = sample[i + 1];
                unchecked {
                    sampleShortsConverted.Add((short)(hi << 8 | lo)); //HACK: This assumes we're at 44100hz!
                    sampleShortsConverted.Add((short)(hi << 8 | lo)); //Sampled at a slower rate ... 
                }
            }

            return new WavetableSample(
                sampleShortsConverted
                .Select<short, float>(
                    //Normalize from (short.MinValue, short.MaxValue) -> (-1.0f, 1.0f)
                    (s) => ((float) s) / ((float) short.MaxValue)
                )
                .ToArray()
            );
        }

        public static readonly WavetableSampleLoader Default = new WavetableSampleLoader();
    }
}

