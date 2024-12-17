using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CarController : MonoBehaviour
{

    private float moveInput;
    private float turnInput;
    private bool isCarGrounded;

    public float forwardSpeed;
    public float reverseSpeed;
    public float turnSpeed;
    public Rigidbody sphereRB;
    public LayerMask groundLayer;

    public float airDrag;
    public float groundDrag;

    void Start()
    {
        sphereRB.transform.parent = null;



    }
    private void Update()
    {

        // Son du moteur
        // GetComponent<AudioSource>().pitch = Mathf.Clamp(speed / maxSpeed + 0.5f, 0f, 1.5f);

        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");
        moveInput *= moveInput > 0 ? forwardSpeed : reverseSpeed;

        //Set car position to sphere
        transform.position = sphereRB.transform.position;

        float newRotation = turnInput * turnSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical");
        transform.Rotate(0, newRotation, 0, Space.World);

        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);

        transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
       
        if(isCarGrounded)
        {
            sphereRB.linearDamping = groundDrag;
            sphereRB.linearDamping = groundDrag;
        }
        else
        {
            sphereRB.linearDamping = airDrag;

        }
    }

    private void FixedUpdate()
    {

        if (isCarGrounded)
        {
            sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
        }
        else
        {
            sphereRB.AddForce(transform.up * -20f);
        }
    }

}
