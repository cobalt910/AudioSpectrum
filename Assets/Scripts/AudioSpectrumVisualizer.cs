// ==============================
// Copyright (c) cobalt910
// Neogene Games
// http://youtube.com/cobalt9101/
// ==============================

using UnityEngine;

namespace AudioSpectrum {
    [RequireComponent(typeof(AudioSpectrum), typeof(AudioSource)), DisallowMultipleComponent]
    public class AudioSpectrumVisualizer : MonoBehaviour
    {
        public GameObject PeerPrefab;
        public float EnergyMultiply;
        public float SpawnOffset;
        public float PeerLerpSpeed;
        [Range(-3, 3)] public float PeerScaleCorrection;

        private AudioSpectrum spectrumData;
        private Transform[] peerTr;
        private float[] audioSpectrum;
        private new Transform transform;

        private void Awake()
        {
            transform = GetComponent<Transform>();
            spectrumData = GetComponent<AudioSpectrum>();
            peerTr = new Transform[AudioSpectrum.fragSize];
            CreatePeers();
        }

        private void Update()
        {
            audioSpectrum = spectrumData.GetAudioSpectrum();
            for (int i = 0; i < peerTr.Length; i++)
            {
                float scaleY = Mathf.Clamp(audioSpectrum[i] * EnergyMultiply, 0.2f, float.MaxValue);
                scaleY = Mathf.Pow(scaleY, PeerScaleCorrection);
                Vector3 newScale = new Vector3(1, scaleY, 1);
                peerTr[i].localScale = Vector3.Lerp(peerTr[i].localScale, newScale, PeerLerpSpeed * Time.deltaTime);
            }
        }

        private void CreatePeers()
        {
            Transform prev = null;
            for (int i = 0; i < peerTr.Length; i++)
            {
                if (prev == null)
                    peerTr[i] = Instantiate(PeerPrefab).transform;
                else
                {
                    peerTr[i] = Instantiate(PeerPrefab).transform;
                    peerTr[i].position = new Vector3(prev.position.x + SpawnOffset, 0f, 0f);
                }
                prev = peerTr[i];
                peerTr[i].parent = transform;
            }
        }
    }
}