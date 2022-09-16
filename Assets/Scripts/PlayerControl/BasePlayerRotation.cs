using UnityEngine;

public class BasePlayerRotation : MonoBehaviour
{
    [SerializeField] protected float sensitivity = 1.5f;
    [SerializeField] protected float smooth = 10.0f;
    [SerializeField] protected Transform player;
    protected float yRotation;
    protected float xRotation;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected void Update()
    {
        yRotation += Input.GetAxis("Mouse X") * sensitivity;
        xRotation -= Input.GetAxis("Mouse Y") * sensitivity;

        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);
    }

    protected void RotatePlayer()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(xRotation, yRotation, 0), Time.deltaTime * smooth);
        player.rotation = Quaternion.Lerp(player.rotation, Quaternion.Euler(0, yRotation, 0), Time.deltaTime * smooth);
    }
}
