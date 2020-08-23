using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camScript : MonoBehaviour
{
    [SerializeField] GameObject camPart2, followTarget;
    public float xSensitivity, ySensitivity;
    float camCurrentY;
    [SerializeField] float camMinY, camMaxY;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        var cameraAngle = camPart2.transform.rotation.eulerAngles;
        camCurrentY += Input.GetAxis("Mouse Y") * ySensitivity;
        camCurrentY = Mathf.Clamp(camCurrentY, camMinY, camMaxY);
        cameraAngle.x = camCurrentY;
        camPart2.transform.rotation = Quaternion.Euler(cameraAngle);

        transform.Rotate(new Vector3(0, Input.GetAxisRaw("Mouse X") * xSensitivity, 0));
        transform.position = followTarget.transform.position;
    }
}