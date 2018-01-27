using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {
	public MeshFilter walls;
	public SquareGrid squareGrid;
	List<Vector3> vertices;
	List<int> triangles;

	Dictionary<int, List<Triangle> > triangleDictionary = new Dictionary<int, List<Triangle>>();
	List<List<int>> outlines;
	HashSet<int> checkedVertices;

	public void generateMesh(int[,] map, float squareSize) {
		vertices = new List<Vector3>();
		triangles = new List<int>();
		triangleDictionary.Clear();
		outlines = new List<List<int>>();
		checkedVertices = new HashSet<int>();

		squareGrid = new SquareGrid(map, squareSize);
		for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
			for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
				TriangulateMesh(squareGrid.squares[x, y]);
			}
		}

		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();

		CalculateMeshOutline();
		CreateWallMesh();
	}

	void CreateWallMesh() {
		List<Vector3> wallVertices = new List<Vector3>();
		List<int> wallTriangles = new List<int>();

		Mesh wallMesh = new Mesh();
		float wallHeight = 5;
		foreach (List<int> outline in outlines) {
			for (int i = 0; i < outline.Count - 1; i++) {
				int startIndex = wallVertices.Count;
				wallVertices.Add(vertices[outline[i]]); //left
				wallVertices.Add(vertices[outline[i + 1]]); //right
				wallVertices.Add(vertices[outline[i]] + Vector3.down * wallHeight); //bottom left
				wallVertices.Add(vertices[outline[i + 1]] + Vector3.down * wallHeight); //bottom right

				wallTriangles.Add(startIndex + 0);
				wallTriangles.Add(startIndex + 2);
				wallTriangles.Add(startIndex + 3);

				wallTriangles.Add(startIndex + 3);
				wallTriangles.Add(startIndex + 1);
				wallTriangles.Add(startIndex + 0);
			}
		}

		wallMesh.vertices = wallVertices.ToArray();
		wallMesh.triangles = wallTriangles.ToArray();
		walls.mesh = wallMesh;

	}

	void TriangulateMesh(Square square) {
		switch (square.configuration) {
			case 0:
				break;

			// 1 point
			case 1:
				MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft);
				break;
			case 2:
				MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight);
				break;
			case 4:
				MeshFromPoints(square.topRight, square.centerRight, square.centerTop);
				break;
			case 8:
				MeshFromPoints(square.centerTop, square.centerLeft, square.topLeft);
				break;

			// 2 points
			case 3:
				MeshFromPoints(square.bottomLeft, square.centerLeft, square.centerRight, square.bottomRight);
				break;
			case 6:
				MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
				break;
			case 9:
				MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
				break;
			case 12:
				MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
				break;
			case 5:
				MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
				break;
			case 10:
				MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
				break;

			//3 points
			case 7:
				MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
				break;
			case 11:
				MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
				break;
			case 13:
				MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
				break;
			case 14:
				MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
				break;

			// 4 points
			case 15:
				MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
				checkedVertices.Add(square.topLeft.vertexIndex);
				checkedVertices.Add(square.topRight.vertexIndex);
				checkedVertices.Add(square.bottomRight.vertexIndex);
				checkedVertices.Add(square.bottomLeft.vertexIndex);
				break;

			default:
				break;
		}
	}

	void MeshFromPoints(params Node[] points) {
		AssignVertices(points);

		if (points.Length >= 3)
			CreateTriangle(points[0], points[1], points[2]);
		if (points.Length >= 4)
			CreateTriangle(points[0], points[2], points[3]);
		if (points.Length >= 5)
			CreateTriangle(points[0], points[3], points[4]);
		if (points.Length >= 6)
			CreateTriangle(points[0], points[4], points[5]);

	}

	void CreateTriangle(Node a, Node b, Node c) {
		triangles.Add(a.vertexIndex);
		triangles.Add(b.vertexIndex);
		triangles.Add(c.vertexIndex);

		Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
		AddTriangleToDictionary(a.vertexIndex, triangle);
		AddTriangleToDictionary(b.vertexIndex, triangle);
		AddTriangleToDictionary(c.vertexIndex, triangle);
	}

	void AddTriangleToDictionary(int vertexIndex, Triangle triangle) {
		if (!triangleDictionary.ContainsKey(vertexIndex)) {
			List<Triangle> triangleList = new List<Triangle>();
			triangleList.Add(triangle);
			triangleDictionary.Add(vertexIndex, triangleList);
		} else {
			triangleDictionary[vertexIndex].Add(triangle);
		}
	}

	void CalculateMeshOutline() {
		for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++) {
			if (!checkedVertices.Contains(vertexIndex)) {
				int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
				if (newOutlineVertex != -1) {
					checkedVertices.Add(vertexIndex);

					List<int> newOutline = new List<int>();
					newOutline.Add(vertexIndex);
					outlines.Add(newOutline);
					FollowOutline(newOutlineVertex, outlines.Count - 1);
					outlines[outlines.Count - 1].Add(vertexIndex);
				}
			}
		}
	}

	void FollowOutline(int vertexIndex, int outlineIndex) {
		outlines[outlineIndex].Add(vertexIndex);
		checkedVertices.Add(vertexIndex);
		int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);
		if (nextVertexIndex != -1) {
			FollowOutline(nextVertexIndex, outlineIndex);
		}
		
	}

	int GetConnectedOutlineVertex(int vertexIndex) {
		List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

		for (int i = 0; i < trianglesContainingVertex.Count; i++) {
			Triangle triangle = trianglesContainingVertex[i];
			for (int j = 0; j < 3; j++) {
				int vertexB = triangle[j];
				if ((vertexB == vertexIndex) || checkedVertices.Contains(vertexB))
					continue;
				if (IsOutlineTriangle(vertexIndex, vertexB)) {
					return vertexB;
				}
			}
		}
		return -1;
	}

	bool IsOutlineTriangle(int vertexA, int vertexB) {
		List<Triangle> trianglesA = triangleDictionary[vertexA];

		int sharedTrianglesCount = 0;
		foreach (Triangle ta in trianglesA) {
			if (ta.Contains(vertexB)) {
				sharedTrianglesCount++;
				if (sharedTrianglesCount > 1)
					break;
			}
		}

		return (sharedTrianglesCount == 1);
	}

	void AssignVertices(Node[] points) {
		for (int i = 0; i < points.Length; i++) {
			if (points[i].vertexIndex == -1) {
				points[i].vertexIndex = vertices.Count;
				vertices.Add(points[i].position);
			}
		}
	}

	struct Triangle {
		public int vertexIndexA;
		public int vertexIndexB;
		public int vertexIndexC;

		int[] vertices;

		public Triangle(int a, int b, int c) {
			vertexIndexA = a;
			vertexIndexB = b;
			vertexIndexC = c;

			vertices = new int[3] { a, b, c};
		}

		public int this[int i] {
			get {
				return vertices[i];
			}
		}

		public bool Contains(int vertexIndex) {
			return ((vertexIndexA == vertexIndex) || (vertexIndexB == vertexIndex) || (vertexIndexC == vertexIndex));
		}
	}

	public class Node {
		public Vector3 position;
		public int vertexIndex = -1;

		public Node(Vector3 _pos) {
			position = _pos;
		}
	}

	public class ControlNode:Node {
		public bool active;
		public Node above, right;

		public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
			active = _active;
			above = new Node(position + Vector3.forward * squareSize / 2f);
			right = new Node(position + Vector3.right * squareSize / 2f);
		}
	}

	public class Square {
		public ControlNode topRight, topLeft, bottomRight, bottomLeft;
		public Node centerTop, centerRight, centerBottom, centerLeft;
		public int configuration;

		public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft) {
			topLeft = _topLeft;
			topRight = _topRight;
			bottomRight = _bottomRight;
			bottomLeft = _bottomLeft;

			centerTop = topLeft.right;
			centerRight = bottomRight.above;
			centerBottom = bottomLeft.right;
			centerLeft = bottomLeft.above;

			configuration = 0;
			if (topLeft.active)
				configuration += 8;
			if (topRight.active)
				configuration += 4;
			if (bottomRight.active)
				configuration += 2;
			if (bottomLeft.active)
				configuration += 1;
		}
	}

	public class SquareGrid {
		public Square[,] squares;

		public SquareGrid(int[,] map, float squareSize) {
			int nodeCountX = map.GetLength(0);
			int nodeCountY = map.GetLength(1);
			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;

			ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];
			for (int x = 0; x < nodeCountX; x++) {
				for (int y = 0; y < nodeCountY; y++) {
					Vector3 position = new Vector3(-mapWidth / 2f + x * squareSize + squareSize / 2f, 0f, -mapHeight / 2f + y * squareSize + squareSize / 2f);
					controlNodes[x, y] = new ControlNode(position, (map[x, y] == 1), squareSize);
				}
			}

			squares = new Square[nodeCountX - 1, nodeCountY - 1];

			for (int x = 0; x < nodeCountX-1; x++) {
				for (int y = 0; y < nodeCountY-1; y++) {
					squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
				}
			}
		}
	}
}
