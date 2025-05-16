using System.Collections;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public PlayerAction playerAction;

    [Header("Camera Details")]
    public LayerMask occlusionLayers;
    public float collisionRadius = 0.3f;
    public float smoothSpeed = 5f;

    [Header("Third Person Camera")]
    public GameObject thirdPersonCamera;
    public Transform player;
    public Transform cameraTarget;
    public float defaultDistance = 4f;
    public float minDistance = 0.5f;
    public float maxDistance = 4.5f;
    public float rotationSpeed = 3f;
    public Vector2 rotationLimits = new Vector2(-40f, 80f);
    private float currentDistance;
    private Vector2 rotation;

    [Header("Aim Camera")]
    public GameObject aimCamera;
    public bool isAiming = false;
    public Transform cameraAimPosition;
    public Transform cameraAimTarget;

    [Header("Handheld Effect")]
    public bool enableHandheldEffect = true;
    public float handheldIntensity = 0.02f;
    public float handheldSpeed = 3f;
    private Vector3 handheldOffset;

    void Start()
    {
        currentDistance = defaultDistance;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(StartEffect());
    }

    void Update()
    {
        if (playerAction.CanControl())
        {
            HandleInput();
        }

        if (player == null) return;
        thirdPersonCamera.SetActive(!isAiming);
        aimCamera.SetActive(isAiming);
        if (isAiming) AimMode();
        else FreeMode();
    }

    void HandleInput()
    {
        float mouseX = InputManager.Instance.cam.x;
        // Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = InputManager.Instance.cam.y;
        // Input.GetAxis("Mouse Y") * rotationSpeed;

        rotation.x += mouseX;
        rotation.y -= mouseY;
        rotation.y = Mathf.Clamp(rotation.y, rotationLimits.x, rotationLimits.y);

        if (Input.GetMouseButtonDown(1)) isAiming = true;
        if (Input.GetMouseButtonUp(1)) isAiming = false;
        if (Input.GetKeyDown(KeyCode.R)) ResetCamera();
    }

    void FreeMode()
    {
        Quaternion targetRotation = Quaternion.Euler(rotation.y, rotation.x, 0);
        Vector3 targetPosition = player.position - (targetRotation * Vector3.forward * currentDistance);

        RaycastHit hit;
        if (Physics.SphereCast(player.position, collisionRadius, -transform.forward, out hit, maxDistance, occlusionLayers))
        {
            currentDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            currentDistance = Mathf.Lerp(currentDistance, defaultDistance, Time.deltaTime * smoothSpeed);
        }

        if (enableHandheldEffect)
        {
            handheldOffset = new Vector3(
                Mathf.PerlinNoise(Time.time * handheldSpeed, 0) - 0.5f,
                Mathf.PerlinNoise(0, Time.time * handheldSpeed) - 0.5f,
                0
            ) * handheldIntensity;
        }

        transform.position = player.position - (targetRotation * Vector3.forward * currentDistance) + handheldOffset;
        transform.LookAt(player.position);
    }

    void AimMode()
    {
        if (cameraAimTarget == null) return;

        aimCamera.transform.position = cameraAimPosition.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(cameraAimTarget.position - aimCamera.transform.position);
        aimCamera.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1f);
    }

    public void ResetCamera()
    {
        rotation = new Vector2(player.eulerAngles.y, 10f);
        currentDistance = defaultDistance;
    }

    public IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }

    private IEnumerator StartEffect()
    {
        float elapsed = 0f;
        float transitionDuration = 1.5f;
        float startDistance = 0.5f;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            currentDistance = Mathf.Lerp(startDistance, defaultDistance, t);
            Quaternion targetRotation = Quaternion.Euler(rotation.y, rotation.x, 0);
            transform.position = player.position - (targetRotation * Vector3.forward * currentDistance);
            transform.LookAt(player.position);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
