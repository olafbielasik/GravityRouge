
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AstroPlayerController : MonoBehaviour
{
    public float pushForce = 10f;
    public float gravityScale = 0.5f;
    public LayerMask groundLayer;

    [Header("Jetpack")]
    public float maxFuel = 100f;
    public float currentFuel;
    public float fuelUsePerBoost = 20f; // 20% of maxFuel
    public float fuelRegenRate = 50f; // faster regeneration
    public bool jetpackEnabled = true;

    private Rigidbody2D rb;
    private GravityAttractor currentAttractor;
    private Vector2 inputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        currentFuel = maxFuel;
    }

    void Update()
    {
        ReadInput();

        if (Input.GetKeyDown(KeyCode.Space) && inputDirection != Vector2.zero && currentFuel >= fuelUsePerBoost)
        {
            Push(inputDirection);
            currentFuel -= fuelUsePerBoost;
        }

        if (currentFuel < maxFuel)
        {
            currentFuel += fuelRegenRate * Time.deltaTime;
            currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
        }
    }

    void FixedUpdate()
    {
        HandleAttraction();
    }

    void ReadInput()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A/D
        float v = Input.GetAxisRaw("Vertical");   // W/S

        inputDirection = new Vector2(h, v).normalized;
    }

    void Push(Vector2 direction)
    {
        rb.AddForce(direction * pushForce, ForceMode2D.Impulse);
    }

    void HandleAttraction()
    {
        if (currentAttractor == null || Vector2.Distance(transform.position, currentAttractor.transform.position) > 10f)
        {
            GravityAttractor[] attractors = FindObjectsOfType<GravityAttractor>();
            float closestDistance = Mathf.Infinity;

            foreach (var attractor in attractors)
            {
                float dist = Vector2.Distance(transform.position, attractor.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    currentAttractor = attractor;
                }
            }
        }

        if (currentAttractor != null)
        {
            Vector3 direction = (currentAttractor.transform.position - transform.position).normalized;

            rb.AddForce(direction * currentAttractor.gravityStrength * gravityScale);

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -direction) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
}





