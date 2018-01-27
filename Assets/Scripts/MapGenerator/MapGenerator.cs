using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
	public int width;
	public int height;

	[Range(0,100)]
	public int randomFillPercent;

	public bool useRandomSeeed;
	public string seed;
	[Range(0,10)]
	public int smoothIterations;
	//map 0 - terrain // map 1 - wall
	int [,] map;

	void Start () {
		GenerateMap();
	}

	private void Update () {
		if (Input.GetMouseButtonDown(0)) {
			GenerateMap();
		}
	}

	private void GenerateMap() {
		map = new int[width, height];

		RandomFillMap();
		for (int i = 0; i < smoothIterations; i++) {
			SmoothMap();
		}

		int borderSize = 1;
		int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

		for (int x = 0; x < borderedMap.GetLength(0); x++) {
			for (int y = 0; y < borderedMap.GetLength(1); y++) {
				if ((x >= borderSize) && (x < width+borderSize) &&
					(y >= borderSize) && (y < height + borderSize)) {
					borderedMap[x, y] = map[x - borderSize, y - borderSize];
				} else {
					borderedMap[x, y] = 1;
				}
				//borderedMap[x,y]
			}
		}

		MeshGenerator meshGen = GetComponent<MeshGenerator>();
		if (meshGen == null)
			meshGen = this.gameObject.AddComponent<MeshGenerator>();
		meshGen.generateMesh(borderedMap, 1f);
	}

	void RandomFillMap() {
		if (useRandomSeeed)
			seed = Time.deltaTime.ToString();

		System.Random prng = new System.Random(seed.GetHashCode());
		
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if ((x == 0) || (y == 0) ||
					(x == width - 1) || (y == height - 1)) {
					// guarantee that borders are wall.
					map[x, y] = 1;
				} else {
					map[x, y] = (prng.Next(0, 100) < randomFillPercent) ? 1 : 0;
				}
			}
		}
	}

	void SmoothMap () {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int walls = SurroundingWallCount(x, y);
				if (walls > 4)
					map[x, y] = 1;
				else if (walls < 4)
					map[x, y] = 0;
			}
		}
	}

	int SurroundingWallCount (int gridX, int gridY) {
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
				if ((neighbourX < 0) || (neighbourX >= width) ||
					(neighbourY < 0) || (neighbourY >= height)) {
					wallCount++;
				} else {
					if ((gridX == neighbourX) && (gridY == neighbourY))
						continue;

					wallCount += map[neighbourX, neighbourY];
				}
			}
		}
		return wallCount;
	}

	private void OnDrawGizmos () {
		if (map != null) {

			//for (int x = 0; x < width; x++) {
			//	for (int y = 0; y < height; y++) {
			//		Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
			//		Vector3 pos = new Vector3(-width / 2 + x + .5f, -height / 2 + y + .5f);
			//		Gizmos.DrawCube(pos, Vector3.one);
			//	}
			//}
		}
	}
}
