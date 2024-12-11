using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float motorForce = 1500f;       // Force appliquée aux roues motrices
    public float steeringAngle = 30f;     // Angle maximal de direction
    public Transform centerOfMass;        // Centre de gravité ajusté pour la stabilité

    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    public Transform frontLeftVisual;
    public Transform frontRightVisual;
    public Transform rearLeftVisual;
    public Transform rearRightVisual;

    private float forwardInput;           // Entrée pour avancer/reculer
    private float turnInput;              // Entrée pour tourner
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Abaisse le centre de gravité pour plus de stabilité
        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.localPosition;
        }
    }

    void Update()
    {
        // Récupérer les entrées utilisateur
        forwardInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        // Mettre à jour la position des roues visuelles
        UpdateVisuals(frontLeftWheel, frontLeftVisual);
        UpdateVisuals(frontRightWheel, frontRightVisual);
        UpdateVisuals(rearLeftWheel, rearLeftVisual);
        UpdateVisuals(rearRightWheel, rearRightVisual);
    }

    void FixedUpdate()
    {
        // Contrôler les roues avant pour la direction
        frontLeftWheel.steerAngle = turnInput * steeringAngle;
        frontRightWheel.steerAngle = turnInput * steeringAngle;

        // Appliquer la force motrice aux roues arrière
        rearLeftWheel.motorTorque = forwardInput * motorForce;
        rearRightWheel.motorTorque = forwardInput * motorForce;
    }

    void UpdateVisuals(WheelCollider collider, Transform visual)
    {
        // Synchroniser la position/rotation des roues visuelles avec les WheelColliders
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        visual.position = pos;
        visual.rotation = rot;
    }
}
