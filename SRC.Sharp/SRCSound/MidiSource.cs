using MeltySynth;
using System;
using System.IO;

namespace SRCSound
{
    /// <summary>
    /// MIDI を MeltySynth（SoundFont）で合成する。
    /// </summary>
    public sealed class MidiSource : ISampleSource
    {
        public const int Rate = 44100;

        private readonly MidiFileSequencer sequencer;
        private readonly MidiFile midi;
        private float[] left = new float[0];
        private float[] right = new float[0];

        public MidiSource(SoundFont soundFont, Stream stream)
        {
            midi = new MidiFile(stream);
            var synthesizer = new Synthesizer(soundFont, Rate);
            sequencer = new MidiFileSequencer(synthesizer);
            sequencer.Play(midi, false);
        }

        public int SampleRate => Rate;
        public int Channels => 2;

        public int Read(float[] buffer, int offset, int count)
        {
            if (sequencer.EndOfSequence)
            {
                return 0;
            }
            var frames = count / 2;
            if (left.Length < frames)
            {
                left = new float[frames];
                right = new float[frames];
            }
            sequencer.Render(new Span<float>(left, 0, frames), new Span<float>(right, 0, frames));
            for (var i = 0; i < frames; i++)
            {
                buffer[offset + i * 2] = left[i];
                buffer[offset + i * 2 + 1] = right[i];
            }
            return frames * 2;
        }

        public void Rewind()
        {
            sequencer.Play(midi, false);
        }
    }
}
