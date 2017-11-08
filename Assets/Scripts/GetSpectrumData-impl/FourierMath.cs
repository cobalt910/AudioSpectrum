// ==============================
// Copyright (c) cobalt910
// Neogene Games
// http://youtube.com/cobalt9101/
// ==============================

using System;
using UnityEngine;

public static class FourierMath 
{
    public struct Complex
    {
        public float Re;
        public float Im;

        public Complex(float re = 0, float im = 0)
        {
            Re = re;
            Im = im;
        }

        public float Magnitude() => Mathf.Sqrt((Re * Re) + (Im * Im));

        #region filters
        public void Hamming(int n, int frameSize)
        {
            Re *= (0.54f - 0.46f * Mathf.Cos(2 * Mathf.PI * n / (frameSize - 1)));
        }

        public void Hanning(int n, int frameSize)
        {
            Re *= (0.5f * (1 - Mathf.Cos(2 * Mathf.PI * n / (frameSize - 1))));
        }

        public void Welch(int n, int frameSize)
        {
            float a = n - ((frameSize - 1) / 2);
            float b = (frameSize - 1) / 2;
            Re *= 1 - (a / b) * (a / b);
        }

        public void BlackmanHarris(int n, int frameSize)
        {
            Re *= (287 / 800 - 0.48829f *
                Mathf.Cos(2 * Mathf.PI * n / (frameSize - 1)) + 0.14128f *
                Mathf.Cos(4 * Mathf.PI * n / (frameSize - 1)) - 0.01168f *
                Mathf.Cos(6 * Mathf.PI * n / (frameSize - 1)));
        }

        public void BlackmanNattall(int n, int frameSize)
        {
            float a = Mathf.Cos((2 * Mathf.PI * n) / (frameSize - 1)) * 0.487396f;
            float b = Mathf.Cos((4 * Mathf.PI * n) / (frameSize - 1)) * 0.144232f;
            float c = Mathf.Cos((6 * Mathf.PI * n) / (frameSize - 1)) * 0.012604f;
            float d = 0.355768f - a + b - c;
            Re *= d;
        }

        public void Cosine(int n, int frameSize)
        {
            float a = (Mathf.PI * n) / (frameSize - 1);
            float b = Mathf.PI / 2;
            float c = Mathf.Cos(a - b);
            Re *= c;
        }

        public void TriangularA(int n, int frameSize)
        {
            float a = n - (frameSize - 1) / 2;
            float b = frameSize / 2;
            float c = Mathf.Abs(1 - a / b);
            Re *= c;

        }

        public void TriangularB(int n, int frameSize)
        {
            float a = n - (frameSize - 1) / 2;
            float b = (frameSize + 1) / 2;
            float c = Mathf.Abs(1 - a / b);
            Re *= c;
        }

        public void TriangularC(int n, int frameSize)
        {
            float a = n - (frameSize - 1) / 2;
            float b = (frameSize - 1) / 2;
            float c = Mathf.Abs(1 - a / b);
            Re *= c;
        }
        #endregion
    }

    public static void FastFourierTransform(bool forward, int m, Complex[] data)
    {
        int num1 = 1;
        for (int index = 0; index < m; ++index)
            num1 *= 2;
        int num2 = num1 >> 1;
        int index1 = 0;
        for (int index2 = 0; index2 < num1 - 1; ++index2)
        {
            if (index2 < index1)
            {
                float x = data[index2].Re;
                float y = data[index2].Im;
                data[index2].Re = data[index1].Re;
                data[index2].Im = data[index1].Im;
                data[index1].Re = x;
                data[index1].Im = y;
            }
            int num3 = num2;
            while (num3 <= index1)
            {
                index1 -= num3;
                num3 >>= 1;
            }
            index1 += num3;
        }
        float num4 = -1f;
        float num5 = 0.0f;
        int num6 = 1;
        for (int index2 = 0; index2 < m; ++index2)
        {
            int num3 = num6;
            num6 <<= 1;
            float num7 = 1f;
            float num8 = 0.0f;
            for (int index3 = 0; index3 < num3; ++index3)
            {
                int index4 = index3;
                while (index4 < num1)
                {
                    int index5 = index4 + num3;
                    float num9 = (float)((double)num7 * (double)data[index5].Re - (double)num8 * (double)data[index5].Im);
                    float num10 = (float)((double)num7 * (double)data[index5].Im + (double)num8 * (double)data[index5].Re);
                    data[index5].Re = data[index4].Re - num9;
                    data[index5].Im = data[index4].Im - num10;
                    data[index4].Re += num9;
                    data[index4].Im += num10;
                    index4 += num6;
                }
                float num11 = (float)((double)num7 * (double)num4 - (double)num8 * (double)num5);
                num8 = (float)((double)num7 * (double)num5 + (double)num8 * (double)num4);
                num7 = num11;
            }
            num5 = (float)Math.Sqrt((1.0 - (double)num4) / 2.0);
            if (forward)
                num5 = -num5;
            num4 = (float)Math.Sqrt((1.0 + (double)num4) / 2.0);
        }
        if (!forward)
            return;
        for (int index2 = 0; index2 < num1; ++index2)
        {
            data[index2].Re /= (float)num1;
            data[index2].Im /= (float)num1;
        }
    }

    public enum Filter
    {
        Hamming = 1,
        Hanning = 2,
        Welch = 3,
        BlackmanHarris = 4,
        BlackmanNuttall = 5,
        Cosine = 6,
        TriangularA = 7,
        TriangularB = 8,
        TriangularC = 9
    }
}