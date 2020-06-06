using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    [SerializeField] float turnSpeed = 25.0f;
    [SerializeField] float speed = 10.0f;

    private float horizontalInput;
    private float farwardInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        horizontalInput = Input.GetAxis("Horizontal");
        farwardInput = Input.GetAxis("Vertical");


        transform.Translate(Vector3.forward * speed * Time.deltaTime * farwardInput);
        transform.Rotate(Vector3.up, turnSpeed * horizontalInput * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {

        }
        
    }
}
