using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailDemo : MonoBehaviour {

	void Start()
	{
		transform.position = Random.onUnitSphere * 10f;
	}
	
	void FixedUpdate()
	{
		var rb = GetComponent<Rigidbody>();
		var go_list = GameObject.FindGameObjectsWithTag("Player");
		foreach (var go in go_list) {
			if (go.GetInstanceID() == gameObject.GetInstanceID()) {
				continue;
			}
			var diff = go.transform.position - transform.position;
			var len2 = diff.sqrMagnitude;
			var force = diff.normalized / len2 * 100f;
			rb.AddForce(force);
		}
		rb.AddForce(-transform.position);
	}
}
