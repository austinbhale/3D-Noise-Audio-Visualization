//Copyright (c) 2018 Peter Olthof, Peer Play
//http://www.peerplay.nl, info AT peerplay.nl 
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class AudioPeer : MonoBehaviour
{
    AudioSource audioSource;
    public static float[] samples = new float[512];
    public static float[] freqBand = new float[8];
    public static float[] bandBuffer = new float[8];
    float[] bufferDecrease = new float[8];

    float[] freqBandHighest = new float[8];
    public static float[] audioBand = new float[8];
    public float[] audioBandBuffer = new float[8];

    public float Amplitude, AmplitudeBuffer;
    float AmplitudeHighest;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();    
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
        GetAmplitude();
    }

    void GetAmplitude()
    {
        float CurrentAmplitude = 0;
        float CurrentAmplitudeBuffer = 0;
        for (int i = 0; i < 8; i++)
        {
            CurrentAmplitude += audioBand[i];
            CurrentAmplitudeBuffer += audioBandBuffer[i];
        }
        if (CurrentAmplitude > AmplitudeHighest)
        {
            AmplitudeHighest = CurrentAmplitude;
        }
        Amplitude = CurrentAmplitude / AmplitudeHighest;
        AmplitudeBuffer = CurrentAmplitudeBuffer / AmplitudeHighest;
    }

    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (freqBand[i] > freqBandHighest[i])
            {
                freqBandHighest[i] = freqBand[i];
            }
            audioBand[i] = (freqBand[i] / freqBandHighest[i]);
            audioBandBuffer[i] = (bandBuffer[i] / freqBandHighest[i]);
        }
    }

    void BandBuffer()
    {
        for (int g = 0; g < 8; g++)
        {
            if (freqBand[g] > bandBuffer[g])
            {
                bandBuffer[g] = freqBand[g];
                bufferDecrease[g] = 0.005f;
            }
            if (freqBand[g] < bandBuffer[g])
            {
                bandBuffer[g] -= bufferDecrease[g];
                bufferDecrease[g] *= 1.2f;
            }
        }
    }

    void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

    void MakeFrequencyBands()
    {
        /*
         *  22050 hz / 512 bands - 43hz per sample
         *  
         *  20 - 60hz
         *  60 - 250hz
         *  250 - 500hz
         *  2000 - 4000hz
         *  4000 - 6000hz
         *  6000 - 20000hz
         *  
         *  0 - 2 samples = 86hz
         *  1 - 4 = 172hz - 87-258
         *  2 - 8 = 344hz - 259-602
         *  3 - 16 = 688hz - 603-1290
         *  4 - 32 = 1376hz - 1291-2666
         *  5 - 64 = 2752hz - 2667-5418
         *  6 - 128 = 5504hz - 5419-10922
         *  7 - 256 = 11008hz - 10923-21930
         *  510
         */

        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            // To match the upper sample count of each band.
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2; // To add 2 to 510 to get the whole audio spectrum.
            }
            
            for (int j = 0; j < sampleCount; j++)
            {
                average += samples[count] * (count + 1);
                count++;
            }

            average /= count;

            freqBand[i] = average * 10;
        }
    }
}
