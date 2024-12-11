using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CarController : MonoBehaviour
{
    public TextMeshProUGUI txtSpeed;
    public WheelCollider front_left;
    public WheelCollider front_right;
    public WheelCollider rear_left;
    public WheelCollider rear_right;
    public float torque;
    public float speed;
    public float maxSpeed = 200f;
    public int brake = 10000;
    public float coafAcceleration = 10f;
    public float wheelAngleMax = 10f;

    private void Update()
    {

        //Affichage vitesse
        speed = GetComponent<Rigidbody>().linearVelocity.magnitude * 3.6f;
        txtSpeed.text = "Speed : " + (int)speed;
        //Accelération
        if(Input.GetKey(KeyCode.UpArrow) && speed < maxSpeed)
        {
            rear_left.brakeTorque = 0;
            rear_right.brakeTorque = 0;
            rear_left.motorTorque = Input.GetAxis("Vertical") * torque * coafAcceleration * Time.deltaTime;
            rear_right.motorTorque = Input.GetAxis("Vertical") * torque * coafAcceleration * Time.deltaTime;
        }

        //Décélération
        if(!Input.GetKey(KeyCode.UpArrow) || speed > maxSpeed) {
            rear_left.motorTorque = 0;
            rear_right.motorTorque = 0;
            rear_left.brakeTorque = brake * coafAcceleration * Time.deltaTime;
            rear_right.brakeTorque = brake * coafAcceleration * Time.deltaTime;
        }

        //Direction
        front_left.steerAngle = Input.GetAxis("Horizontal") * wheelAngleMax;
        front_right.steerAngle = Input.GetAxis("Horizontal") * wheelAngleMax;
        
    }
    void Start()
    {

    }


    
}
