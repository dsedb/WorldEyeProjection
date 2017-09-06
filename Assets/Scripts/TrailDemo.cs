using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailDemo : MonoBehaviour {

	Transform child_;

	void Start()
	{
		transform.position = Random.onUnitSphere * 10f;
		child_ = transform.Find("child");
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
			var force = diff.normalized / len2 * 10f;
			rb.AddForce(force);
		}
		rb.AddForce(-transform.position*0.1f);
		child_.position = transform.position.normalized * 10f;
	}
}
