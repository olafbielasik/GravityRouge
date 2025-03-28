using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GrappleHook : MonoBehaviour
{
    public LayerMask grappleMask;
    public float maxDistance = 15f;
    public float scrollSpeed = 2f;
    public float minRopeLength = 1f;
    public float maxRopeLength = 20f;
    public LineRenderer lineRenderer;

    private SpringJoint2D springJoint;
    private Rigidbody2D rb;
    private Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        springJoint = gameObject.AddComponent<SpringJoint2D>();
        springJoint.enabled = false;
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.autoConfigureDistance = false;
        springJoint.frequency = 0.5f;
        springJoint.dampingRatio = 0.9f;
        springJoint.enableCollision = true;

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;
            lineRenderer.widthMultiplier = 0.05f;
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TryGrapple();
        }

        if (Input.GetMouseButtonUp(1))
        {
            Detach();
        }

        if (springJoint.enabled)
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0f)
            {
                springJoint.distance -= scroll * scrollSpeed;
                springJoint.distance = Mathf.Clamp(springJoint.distance, minRopeLength, maxRopeLength);
            }

            UpdateLineVisual();

            if (springJoint.distance >= maxRopeLength)
            {
                Detach();
            }
        }
    }

    void TryGrapple()
    {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorldPos - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, maxDistance, grappleMask);

        if (hit.collider != null)
        {
            springJoint.connectedBody = null;
            springJoint.connectedAnchor = hit.point;
            springJoint.distance = Vector2.Distance(transform.position, hit.point);
            springJoint.enabled = true;

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hit.point);
        }
    }

    void UpdateLineVisual()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, springJoint.connectedAnchor);

        float actualDistance = Vector2.Distance(transform.position, springJoint.connectedAnchor);
        float tension = actualDistance / maxRopeLength;

        Color ropeColor;
        if (tension < 0.5f)
            ropeColor = Color.green;
        else if (tension < 0.9f)
            ropeColor = Color.yellow;
        else
            ropeColor = Color.red;

        lineRenderer.startColor = ropeColor;
        lineRenderer.endColor = ropeColor;

        if (tension >= 1f)
        {
            Detach();
        }
    }


    void Detach()
    {
        springJoint.enabled = false;
        lineRenderer.enabled = false;
    }
}


