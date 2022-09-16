using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class RigidbodyMovement : BasePlayerMovement
{
    private new Rigidbody rigidbody;
    private Animator anim;
    
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(transform.position + movementVector * movementSpeed * Time.fixedDeltaTime);
        //anim.SetFloat("Hor", Input.GetAxis("Horizontal"));
        //anim.SetFloat("Vert", Input.GetAxis("Vertical"));
        //anim.SetFloat("Speed", rigidbody.velocity.x + rigidbody.velocity.z);
    }
}
