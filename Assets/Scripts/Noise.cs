﻿using UnityEngine;
using System.Collections;

public static class Noise {

	public enum NormalizeMode {Local, Global};

	public static float[,] GenerateNoiseMap(int mapSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode, AnimationCurve noiseFilterCurve) {
		float[,] noiseMap = new float[mapSize,mapSize];
		AnimationCurve noiseFilter = new AnimationCurve(noiseFilterCurve.keys);

		System.Random prng = new System.Random (seed);
		Vector2[] octaveOffsets = new Vector2[octaves];

		float maxPossibleHeight = 0;
		float amplitude = 1;
		float frequency = 1;

		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) - offset.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= persistance;
		}

		if (scale <= 0) {
			scale = 0.0001f;
		}

		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight = float.MaxValue;

		float halfWidth = mapSize / 2f;
		float halfHeight = mapSize / 2f;


		for (int y = 0; y < mapSize; y++) {
			for (int x = 0; x < mapSize; x++) {

				amplitude = 1;
				frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++) {
					float sampleX = (x-halfWidth + octaveOffsets[i].x) / scale * frequency;
					float sampleY = (y-halfHeight + octaveOffsets[i].y) / scale * frequency;

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxLocalNoiseHeight) {
					maxLocalNoiseHeight = noiseHeight;
				} else if (noiseHeight < minLocalNoiseHeight) {
					minLocalNoiseHeight = noiseHeight;
				}
				noiseMap [x, y] = noiseFilter.Evaluate(noiseHeight);
			}
		}

		for (int y = 0; y < mapSize; y++) {
			for (int x = 0; x < mapSize; x++) {
				if (normalizeMode == NormalizeMode.Local) {
					noiseMap [x, y] = Mathf.InverseLerp (minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap [x, y]);
				} else {
					float normalizedHeight = (noiseMap [x, y] + 1) / (maxPossibleHeight/0.9f);
					noiseMap [x, y] = Mathf.Clamp(normalizedHeight,0, int.MaxValue);
				}
			}
		}

		return noiseMap;
	}

}
