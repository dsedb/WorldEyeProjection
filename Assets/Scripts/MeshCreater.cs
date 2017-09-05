using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ {

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class MeshCreater : MonoBehaviour {

	const int WIDTH = 64;
	const int HEIGHT = 64;
	const float SIZE = 1f;
	public bool ConsiderAspect = false;
	private Mesh mesh_;
	public enum Type {
		Front,
		Back,
		Left,
		Right,
		Up,
		Down,
	}
	public Type type_;

	/*
	 * http://mathworld.wolfram.com/AzimuthalEquidistantProjection.html
	 * x	=	k^'cosphisin(lambda-lambda_0)	
	 * y	=	k^'[cosphi_1sinphi-sinphi_1cosphicos(lambda-lambda_0)].	
	 *  k^'=c/(sinc) 	
	 *  cosc=sinphi_1sinphi+cosphi_1cosphicos(lambda-lambda_0), 
	 */
	Vector3 map(Vector3 vertex, Type type)
	{
		var v = vertex;
		var n = v.normalized;

		float lambda = 0f;
		float phi = 0f;
		var len = Mathf.Sqrt(n.x*n.x + n.z*n.z);
		switch (type) {
			case Type.Front:
				lambda = Mathf.Atan2(n.x, n.z);
				phi = Mathf.Atan2(n.y, len);
				break;
			case Type.Back:
				lambda = Mathf.Atan2(n.x, n.z);
				phi = Mathf.Atan2(n.y, len);
				break;
			case Type.Left:
				lambda = Mathf.Atan2(n.x, n.z);
				phi = Mathf.Atan2(n.y, len);
				break;
			case Type.Right:
				lambda = Mathf.Atan2(n.x, n.z);
				phi = Mathf.Atan2(n.y, len);
				break;
			case Type.Up:
				lambda = -Mathf.Atan2(n.z, n.x);
				phi = Mathf.Atan2(n.y, len);
				break;
			case Type.Down:
				lambda = -Mathf.Atan2(n.z, n.x);
				phi = Mathf.Atan2(n.y, len);
				break;
		}
		// lambda = Mathf.Repeat(lambda, Mathf.PI*2f);
		// if (lambda > Mathf.PI) {
		// 	lambda -= Mathf.PI*2;
		// }
		// phi = Mathf.Repeat(phi, Mathf.PI*2f);
		// if (phi > Mathf.PI) {
		// 	phi -= Mathf.PI*2;
		// }

		const float lambda0 = 0;
		const float phi1 = 0;
		float c = Mathf.Acos( Mathf.Sin(phi1) * Mathf.Sin(phi) + Mathf.Cos(phi1) * Mathf.Cos(phi) * Mathf.Cos(lambda - lambda0) );
		float k = 0f;
		if (c > 0f) {
			k = c/Mathf.Sin(c);
		}
		float x = k * Mathf.Cos(phi) * Mathf.Sin(lambda - lambda0);
		float y = k * (Mathf.Cos(phi1) * Mathf.Sin(phi) - Mathf.Sin(phi1) * Mathf.Cos(phi) * Mathf.Cos( lambda - lambda0 ));

		if (ConsiderAspect) {
			var ratio = (float)Screen.width/(float)Screen.height;
			x *= ratio;
		}
		return new Vector3(x, y, 0f);
	}

	Vector3[] map(Vector3[] vertices, float radius, Type type)
	{
		var rot = Quaternion.identity;
		switch (type) {
			case Type.Front:
				rot = Quaternion.identity;
				break;
			case Type.Back:
				rot = Quaternion.Euler(0f, 180f, 0f);
				break;
			case Type.Left:
				rot = Quaternion.Euler(0f, -90f, 0f);
				break;
			case Type.Right:
				rot = Quaternion.Euler(0f, 90f, 0f);
				break;
			case Type.Up:
				rot = Quaternion.Euler(-90f, 0f, 90f);
				break;
			case Type.Down:
				rot = Quaternion.Euler(90f, 0f, -90f);
				break;
		}
		var result = new Vector3[vertices.Length];
		for (var i = 0; i < result.Length; ++i) {
			result[i] = map(rot * new Vector3(vertices[i].x, vertices[i].y, radius), type);
		}
		return result;
	}

	void Start()
	{
		var vertices = new Vector3[(WIDTH+1)*(HEIGHT+1)];
		for (var y = 0; y < HEIGHT+1; ++y) {
			for (var x = 0; x < WIDTH+1; ++x) {
				vertices[y*(WIDTH+1)+x] = new Vector3((x-WIDTH/2)*(SIZE/WIDTH), (y-HEIGHT/2)*(SIZE/HEIGHT), 0f);
			}			
		}
		vertices = map(vertices, SIZE/2, type_);

		var uvs = new Vector2[(WIDTH+1)*(HEIGHT+1)];
		for (var y = 0; y < HEIGHT+1; ++y) {
			for (var x = 0; x < WIDTH+1; ++x) {
				uvs[y*(WIDTH+1)+x] = new Vector3(x*(1f/WIDTH), y*(1f/HEIGHT), 0f);
			}
		}

		int removed_num = 0;
		if (type_ == Type.Back) {
			removed_num = 4;
		}
		var triangles = new int[(WIDTH*HEIGHT-removed_num)*6];
		var idx = 0;
		var ridx = (HEIGHT/2)*(WIDTH+1) + (WIDTH/2);
		for (var y = 0; y < HEIGHT; ++y) {
			for (var x = 0; x < WIDTH; ++x) {
				int v0 = y*(WIDTH+1)+x;
				int v1 = (y+1)*(WIDTH+1)+x;
				int v2 = y*(WIDTH+1)+x+1;
				int v3 = (y+1)*(WIDTH+1)+x;
				int v4 = (y+1)*(WIDTH+1)+x+1;
				int v5 = y*(WIDTH+1)+x+1;
				if (removed_num > 0 &&
					(v0 == ridx ||
					 v1 == ridx ||
					 v2 == ridx ||
					 v3 == ridx ||
					 v4 == ridx ||
					 v5 == ridx)) {
					continue;
				}
				triangles[idx*6+0] = v0;
				triangles[idx*6+1] = v1;
				triangles[idx*6+2] = v2;
				triangles[idx*6+3] = v3;
				triangles[idx*6+4] = v4;
				triangles[idx*6+5] = v5;
				++idx;
			}
		}

		mesh_ = new Mesh();
		mesh_.vertices = vertices;
		mesh_.uv = uvs;
		mesh_.triangles = triangles;
		GetComponent<MeshFilter>().sharedMesh = mesh_;
	}
}

} // namespace UTJ {
