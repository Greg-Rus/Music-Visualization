﻿using System.Drawing;
using UnityEngine;
using CSCore;
using CSCore.SoundIn;
using CSCore.Codecs.WAV;
using WinformsVisualization.Visualization;
using CSCore.DSP;
using CSCore.Streams;
using Color = System.Drawing.Color;



public interface IBarData
{
    float[] BarValues { get; }
    int NumberOfBars { get; }
}

public class SoundCapture : MonoBehaviour, IBarData
{
    // Use this for initialization

    public int numBars = 30;

    public int minFreq = 5;
    public int maxFreq = 4500;
    public int barSpacing = 0;
    public bool logScale = true;
    public bool isAverage = false;

    public float highScaleAverage = 2.0f;
    public float highScaleNotAverage = 3.0f;

    public AudioSourceType audioType;



    LineSpectrum lineSpectrum;

    WasapiCapture capture;
    WaveWriter writer;
    FftSize fftSize;
    float[] fftBuffer;

    SingleBlockNotificationStream notificationSource;

    BasicSpectrumProvider spectrumProvider;

    IWaveSource finalSource;

    public float[] barData;

    void Awake()
    {

        barData = new float[numBars];
        // This uses the wasapi api to get any sound data played by the computer
        switch (audioType)
        {
            case AudioSourceType.Microphone: capture = new WasapiCapture();
                break;
            case AudioSourceType.Speakers: capture = new WasapiLoopbackCapture();
                break;
        }

        capture.Initialize();

        // Get our capture as a source
        IWaveSource source = new SoundInSource(capture);


        // From https://github.com/filoe/cscore/blob/master/Samples/WinformsVisualization/Form1.cs

        // This is the typical size, you can change this for higher detail as needed
        fftSize = FftSize.Fft4096;

        // Actual fft data
        fftBuffer = new float[(int)fftSize];

        // These are the actual classes that give you spectrum data
        // The specific vars of lineSpectrum are changed below in the editor so most of these aren't that important here
        spectrumProvider = new BasicSpectrumProvider(capture.WaveFormat.Channels,
                    capture.WaveFormat.SampleRate, fftSize);

        lineSpectrum = new LineSpectrum(fftSize)
        {
            SpectrumProvider = spectrumProvider,
            UseAverage = isAverage,
            BarCount = numBars,
            BarSpacing = 2,
            IsXLogScale = false,
            ScalingStrategy = ScalingStrategy.Linear
        };

        // Tells us when data is available to send to our spectrum
        var notificationSource = new SingleBlockNotificationStream(source.ToSampleSource());

        notificationSource.SingleBlockRead += NotificationSource_SingleBlockRead;

        // We use this to request data so it actualy flows through (figuring this out took forever...)
        finalSource = notificationSource.ToWaveSource();

        capture.DataAvailable += Capture_DataAvailable;
        capture.Start();
    }

    private void Capture_DataAvailable(object sender, DataAvailableEventArgs e)
    {
        finalSource.Read(e.Data, e.Offset, e.ByteCount);
    }

    private void NotificationSource_SingleBlockRead(object sender, SingleBlockReadEventArgs e)
    {
        spectrumProvider.Add(e.Left, e.Right);
    }

    void Update()
    {

        int numBars = barData.Length;

        float[] resData = GetFFtData();

        if (resData == null)
        {
            return;
        }

        lock (barData)
        {
            for (int i = 0; i < numBars && i < resData.Length; i++)
            {
                // Make the data between 0.0 and 1.0
                barData[i] = resData[i] / 100.0f;
            }

            for (int i = 0; i < numBars && i < resData.Length; i++)
            {
                if (lineSpectrum.UseAverage)
                {
                    // Scale the data because for some reason bass is always loud and treble is soft
                    barData[i] = barData[i] + highScaleAverage * Mathf.Sqrt(i / (numBars + 0.0f)) * barData[i];
                }
                else
                {
                    barData[i] = barData[i] + highScaleAverage * Mathf.Sqrt(i / (numBars + 0.0f)) * barData[i];
                }
            }
        }

    }

    public float[] GetFFtData()
    {
        lock (barData)
        {
            lineSpectrum.BarCount = numBars;
            if (numBars != barData.Length)
            {
                barData = new float[numBars];
            }
        }

        if (spectrumProvider.IsNewDataAvailable)
        {
            lineSpectrum.MinimumFrequency = minFreq;
            lineSpectrum.MaximumFrequency = maxFreq;
            lineSpectrum.IsXLogScale = logScale;
            lineSpectrum.BarSpacing = barSpacing;
            lineSpectrum.SpectrumProvider.GetFftData(fftBuffer, this);
            return lineSpectrum.GetSpectrumPoints(100.0f, fftBuffer);
        }
        else
        {
            return null;
        }
    }

    void OnApplicationQuit()
    {
        if (enabled)
        {
            capture.Stop();
            capture.Dispose();
        }
    }

    public float[] BarValues
    {
        get
        {
            lock (barData)
            {
                return barData;
            }
        }
    }

    public int NumberOfBars
    {
        get { return numBars; }
    }
}

public enum AudioSourceType
{
    Speakers,
    Microphone
}
