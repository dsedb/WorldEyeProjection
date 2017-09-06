using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ {

public class AzimuthalEquidistantProjection : MonoBehaviour {

	public static bool sConsiderAspect = true;
	public bool ConsiderAspect;
	private int rt_width_ = 256;
	private int rt_height_ = 256;
	private RenderTexture[] rt_list_;
	private Material[] mat_list_;

	string[] names = new string[] { "Front", "Left", "Right", "Up", "Down", "Back", };

	void Awake()
	{
		sConsiderAspect = ConsiderAspect;
	}

	void Start()
	{
		const int NUM = 6;

		rt_list_ = new RenderTexture[NUM];
		for (var i = 0; i < NUM; ++i) {
			rt_list_[i] = new RenderTexture(rt_width_, rt_height_, 24 /* depth */, RenderTextureFormat.ARGB32);
		}

		mat_list_ = new Material[NUM];
		var shader = Shader.Find("Custom/blit");
		for (var i = 0; i < NUM; ++i) {
			var mat = new Material(shader);
			mat.SetTexture("_MainTex", rt_list_[i]);
			mat_list_[i] = mat;
		}

		for (var i = 0; i < NUM; ++i) {
			var tfm = transform.Find("Camera"+names[i]);
			if (tfm != null) {
				tfm.GetComponent<Camera>().targetTexture = rt_list_[i];
			}
		}

		for (var i = 0; i < NUM; ++i) {
			var go = GameObject.Find("Mesh"+names[i]) as GameObject;
			if (go == null) {
				var tfm = transform.Find("Camera"+names[i]);
				if (tfm != null) {
					tfm.gameObject.SetActive(false);
				}
			} else {
				go.GetComponent<MeshRenderer>().sharedMaterial = mat_list_[i];
			}
		}
	}
}

} // namespace UTJ {
