// ==============================
// Copyright (c) cobalt910
// Neogene Games
// http://youtube.com/cobalt9101/
// ==============================

using System.Collections.Generic;
using UnityEngine;

namespace AudioSpectrum
{
    [RequireComponent(typeof(AudioSource)), DisallowMultipleComponent]
    public class AudioSpectrum : MonoBehaviour
    {
        [Header("FFT Setting"), Tooltip("x-leftCh, y-rightCh")]
        public Vector2 ChannelsNumber = new Vector2(0, 1);
        public FFTWindow Window = FFTWindow.BlackmanHarris;

        private AudioSource source;
        private float[] leftCh;
        private float[] rightCh;
        private float[] instEnergy;
        private float[] fragEnergy;
        private List<float[]> histEnergyBuffer;
        private float[] averageHistEnergy;
        
        private const int chSize = 512;
        public const int fragSize = 64;
        private const int histSize = 43;

        private void Awake() => source = GetComponent<AudioSource>();

        private void Start()
        {
            // инициализируем массивы
            leftCh = new float[chSize];
            rightCh = new float[chSize];

            instEnergy = new float[chSize];
            fragEnergy = new float[fragSize];
            histEnergyBuffer = new List<float[]>();
            averageHistEnergy = new float[fragSize];
        }

        public float[] GetAudioSpectrum()
        {
            source.GetSpectrumData(samples: leftCh, channel: (int)ChannelsNumber.x, window: Window);
            source.GetSpectrumData(samples: rightCh, channel: (int)ChannelsNumber.y, window: Window);
            FragmentedEnergy();
            AverageHistEnergy();
            return averageHistEnergy;
        }

        public void FragmentedEnergy()
        {
            // считаем моментальную энерегию
            for (int i = 0; i < chSize; i++)
                instEnergy[i] = Mathf.Pow(leftCh[i], 2) + Mathf.Pow(rightCh[i], 2);

            // фрагментируем энерегию (1024 в 64 на прмиер)
            for (int i = 0; i != fragSize; i++)
            {
                float val = 0;
                for (int j = i * fragSize; j != (i + 1) * fragSize; j++)
                    val += instEnergy[i];
                fragEnergy[i] = val / fragSize;
            }
        }

        public void AverageHistEnergy()
        {
            // ограничиваем размер массива (да, немного костыль!)
            if (histEnergyBuffer.Count == histSize)
                histEnergyBuffer.RemoveAt(0);

            // добавляем в историю
            histEnergyBuffer.Add(fragEnergy);

            // считаем среднюю энерегию за время
            for (int i = 0; i != fragSize; i++)
            {
                float e = 0;
                for (int j = 0; j != histSize; j++)
                {
                    if (histEnergyBuffer.Count == j) break;
                    e += histEnergyBuffer[j][i];
                }
                averageHistEnergy[i] = e / histSize;
            }
        }

        // implement beat detection
    }
}