using UnityEngine;

public class BasePlayerMovement : MonoBehaviour
{
    [SerializeField] protected float movementSpeed = 6.0f;
    protected Vector3 movementVector;

    protected void Update()
    {
        movementVector = transform.right * Input.GetAxis("Horizontal") + Input.GetAxis("Vertical") * transform.forward;
    }
}
