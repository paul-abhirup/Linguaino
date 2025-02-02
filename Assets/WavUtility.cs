using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    // Converts an AudioClip to WAV format byte array
    public static byte[] FromAudioClip(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("AudioClip is null");
            return null;
        }

        MemoryStream stream = new MemoryStream();
        int sampleCount = clip.samples * clip.channels;
        int frequency = clip.frequency;

        // Write WAV header
        WriteWavHeader(stream, clip.channels, frequency, sampleCount);

        // Write audio data
        float[] samples = new float[sampleCount];
        clip.GetData(samples, 0);

        byte[] bytes = new byte[sampleCount * 2];
        int rescaleFactor = 32767;  // Convert float [-1.0f, 1.0f] to 16-bit PCM value range

        for (int i = 0; i < samples.Length; i++)
        {
            short value = (short)(samples[i] * rescaleFactor);
            byte[] byteArr = BitConverter.GetBytes(value);
            bytes[i * 2] = byteArr[0];
            bytes[i * 2 + 1] = byteArr[1];
        }

        stream.Write(bytes, 0, bytes.Length);

        // Return the full WAV byte array
        return stream.ToArray();
    }

    // Write the WAV file header
    private static void WriteWavHeader(Stream stream, int channels, int frequency, int samples)
    {
        // Chunk ID "RIFF"
        stream.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"), 0, 4);

        // Chunk Size (file size minus 8 bytes for the RIFF header)
        stream.Write(BitConverter.GetBytes(36 + samples * 2), 0, 4);

        // Format "WAVE"
        stream.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"), 0, 4);

        // Subchunk ID "fmt "
        stream.Write(System.Text.Encoding.ASCII.GetBytes("fmt "), 0, 4);

        // Subchunk Size (16 for PCM)
        stream.Write(BitConverter.GetBytes(16), 0, 4);

        // Audio Format (1 for PCM - integer samples)
        stream.Write(BitConverter.GetBytes((short)1), 0, 2);

        // Number of channels
        stream.Write(BitConverter.GetBytes((short)channels), 0, 2);

        // Sample rate
        stream.Write(BitConverter.GetBytes(frequency), 0, 4);

        // Byte rate (Sample Rate * Number of Channels * Bits per Sample / 8)
        stream.Write(BitConverter.GetBytes(frequency * channels * 16 / 8), 0, 4);

        // Block align (Number of Channels * Bits per Sample / 8)
        stream.Write(BitConverter.GetBytes((short)(channels * 16 / 8)), 0, 2);

        // Bits per sample
        stream.Write(BitConverter.GetBytes((short)16), 0, 2);

        // Subchunk ID "data"
        stream.Write(System.Text.Encoding.ASCII.GetBytes("data"), 0, 4);

        // Subchunk Size (Number of Samples * Number of Channels * Bits per Sample / 8)
        stream.Write(BitConverter.GetBytes(samples * 2), 0, 4);
    }
}
