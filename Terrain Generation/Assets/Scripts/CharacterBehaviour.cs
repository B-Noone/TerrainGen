using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    public float movementSpeed = 1f;

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axi

    // Start is called before the first frame update
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    // Update is called once per frame
    void Update()
    {
        //Character Movement
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0.0f, 0.0f, 2.0f * movementSpeed));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0.0f, 0.0f, -2.0f * movementSpeed));
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-2.0f * movementSpeed, 0.0f, 0.0f));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(2.0f * movementSpeed, 0.0f, 0.0f));
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }
}
