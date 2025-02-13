using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CarController2 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationPoint;

    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;

    private int[] wheelsIsGrounded = new int[4];
    bool isGrounded = false;

    [Header("Input")]
    private float moveInput = 10;
    private float steerInput = 10;

    [Header("Car settings")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private float dragCoefficient = 1f;

    private Vector3 currentCarLocalVelocity = Vector3.zero;
    private float carVelocityRatio = 0;

    private int score;
    public TextMeshProUGUI scoreText;

    public List<GameObject> checkpoints = new List<GameObject>();
    private int currentCheckpointIndex;

    [Header("Health System")]
    [SerializeField] private int health = 3;
    public TextMeshProUGUI healthText;

    [Header("Missile Settings")]
    public GameObject missilePrefab;
    public Transform missileSpawnPoint;
    public float missileSpeed = 50f;

    private void Start()
    {
        carRB = GetComponent<Rigidbody>();
        score = 0;
        SetScoreText();
        SetHealthText();
    }

    void Update()
    {
        GetPlayerInput();
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MenuScene");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ShootMissile();
        }
    }

    void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
    }

    #region Car Status Check
    private void GroundCheck()
    {
        int tempGroundedWheels = 0;

        for (int i = 0; i < wheelsIsGrounded.Length; i++)
        {
            tempGroundedWheels += wheelsIsGrounded[i];
        }
        
        if(tempGroundedWheels > 1)
        {
            isGrounded=true;
        }
        else
        {
            isGrounded=false;
        }
    }

    private void CalculateCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(carRB.linearVelocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }
    #endregion

    #region Input Handling
    private void GetPlayerInput()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }
    #endregion

    #region Movement
    private void Movement()
    {
        if(isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
            SidewaysDrag();
        }
    }

    private void Acceleration()
    {
        carRB.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Deceleration()
    {
        carRB.AddForceAtPosition(deceleration * moveInput * -transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Turn()
    {
        carRB.AddTorque(steerStrength * steerInput * turningCurve.Evaluate(carVelocityRatio) * Mathf.Sign(carVelocityRatio) * transform.up, ForceMode.Acceleration);
    }

    private void SidewaysDrag()
    {
        float currentSidewaysSpeed = currentCarLocalVelocity.x;

        float dragMagnitude = -currentSidewaysSpeed * dragCoefficient;

        Vector3 dragForce = transform.right * dragMagnitude;

        carRB.AddForceAtPosition(dragForce, carRB.worldCenterOfMass, ForceMode.Acceleration);
    }
    #endregion

    private void Suspension()
    {
        for (int i = 0; i < rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;

            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, maxLength + wheelRadius, drivable))
            {
                wheelsIsGrounded[i] = 1;

                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = restLength - currentSpringLength / springTravel;

                float springVelocity = Vector3.Dot(carRB.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float dampForce = damperStiffness * springVelocity;

                float springForce = springStiffness * springCompression;

                float netForce = springForce - dampForce;

                carRB.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);

                Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
            }
            else
            {
                wheelsIsGrounded[i] = 0;

                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position + (wheelRadius + maxLength) * -rayPoints[i].up, Color.green);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Gold"))
        {
            other.gameObject.SetActive(false);
            score += 1;
            SetScoreText();
        }

        if (other.gameObject.CompareTag("Checkpoint"))
        {
            CheckpointPassed(other.gameObject);
        }

        if (other.gameObject.CompareTag("Zombie"))
        {
            other.gameObject.SetActive(false);
            TakeDamage(1);
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;
        SetHealthText();

        if (health <= 0)
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    void SetHealthText()
    {
        healthText.text = "PV: " + health.ToString();
    }

    void SetScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    private void CheckpointPassed(GameObject checkpoint)
    {
        if (checkpoints[currentCheckpointIndex] == checkpoint)
        {
            Debug.Log("Checkpoint " + currentCheckpointIndex + " validé !");
            currentCheckpointIndex++;

            if (currentCheckpointIndex >= checkpoints.Count)
            {
                SceneManager.LoadScene("MenuScene");
            }
        }
    }

    // Fonction pour tirer le missile
    private void ShootMissile()
    {
        // Créer le missile au missileSpawnPoint
        GameObject missile = Instantiate(missilePrefab, missileSpawnPoint.position, missileSpawnPoint.rotation);

        // Appliquer une force au missile dans la direction de la voiture
        Rigidbody missileRB = missile.GetComponent<Rigidbody>();
        missileRB.linearVelocity = missileSpawnPoint.forward * missileSpeed;  // La direction de lancement est forward, avec la vitesse

        // Appliquer une rotation supplémentaire de 90° sur l'axe Z
        missile.transform.Rotate(90, 0, 0);  // Rotation de 90° autour de l'axe Z
    }
}