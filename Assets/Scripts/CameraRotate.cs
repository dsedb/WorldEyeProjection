using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ {

public class CameraRotate : MonoBehaviour {

	void Update()
    {
        var hori = Input.GetAxisRaw("Horizontal");
        var vert = Input.GetAxisRaw("Vertical");
        transform.Rotate(new Vector3(vert*2f, -hori*2f, 0f));
	}
}

} // namespace UTJ {
