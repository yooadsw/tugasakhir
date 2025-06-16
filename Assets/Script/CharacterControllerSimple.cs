using UnityEngine;

public class CharacterControllerSmooth : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;

    [Header("Pengaturan Gerakan")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float rotationSpeed = 10f; // Kecepatan rotasi
    public float acceleration = 10f; // Percepatan gerakan
    public float deceleration = 10f; // Perlambatan gerakan

    [Header("Pengaturan Deteksi Tanah")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    private bool isGrounded;
    private bool wasGrounded; // Untuk deteksi landing
    private Vector3 moveDirection;
    private Vector3 currentVelocity; // Kecepatan saat ini untuk smoothing
    private Vector3 velocityBeforeJump; // Simpan velocity sebelum jump

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Set interpolation untuk movement yang lebih smooth
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Deteksi landing
        if (!wasGrounded && isGrounded)
        {
            OnLanding();
        }

        // Gunakan GetAxis untuk input yang lebih smooth (bukan GetAxisRaw)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Simpan velocity horizontal sebelum jump
            velocityBeforeJump = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            animator.SetBool("isJumping", true);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Perhitungan arah berdasarkan kamera
        Camera cam = Camera.main;
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camRight * moveX + camForward * moveZ).normalized;

        // Smooth rotation menggunakan Slerp
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                                                rotationSpeed * Time.deltaTime);
        }

        animator.SetFloat("speed", currentVelocity.magnitude);
    }

    void FixedUpdate()
    {
        float currentYVelocity = rb.linearVelocity.y;

        // Jika sedang di udara, pertahankan momentum horizontal
        if (!isGrounded)
        {
            // Batasi kontrol di udara (air control)
            Vector3 airTargetVelocity = moveDirection * moveSpeed * 0.5f; // 50% kontrol di udara
            Vector3 currentHorizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            // Smooth air control
            Vector3 newHorizontalVel = Vector3.MoveTowards(currentHorizontalVel, airTargetVelocity,
                                                         acceleration * 0.3f * Time.fixedDeltaTime);

            rb.linearVelocity = new Vector3(newHorizontalVel.x, currentYVelocity, newHorizontalVel.z);
            return;
        }

        // Ground movement (sama seperti sebelumnya)
        Vector3 groundTargetVelocity = moveDirection * moveSpeed;

        // Smooth acceleration/deceleration
        if (moveDirection.magnitude > 0.1f)
        {
            // Accelerating
            currentVelocity = Vector3.MoveTowards(currentVelocity, groundTargetVelocity,
                                                acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Decelerating
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero,
                                                deceleration * Time.fixedDeltaTime);
        }

        // Apply movement
        rb.linearVelocity = new Vector3(currentVelocity.x, currentYVelocity, currentVelocity.z);
    }

    public void OnJumpAnimationEnd()
    {
        animator.SetBool("isJumping", false);
    }

    private void OnLanding()
    {
        // Reset animasi jumping
        animator.SetBool("isJumping", false);

        // Pastikan tidak ada velocity aneh setelah landing
        Vector3 currentVel = rb.linearVelocity;

        // Jika velocity Y masih negatif (jatuh), reset ke 0
        if (currentVel.y < 0)
        {
            rb.linearVelocity = new Vector3(currentVel.x, 0f, currentVel.z);
        }

        // Sinkronkan currentVelocity dengan velocity sebenarnya
        currentVelocity = new Vector3(currentVel.x, 0, currentVel.z);
    }

    // Visualisasi ground check di Scene view
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}