using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    public enum meState
    {
        NORMAL,
        MUNTED
    }

    public meState currentState;

    bool isMoving;
    [SerializeField] GameObject cameraBuddy;
    [SerializeField] float moveSpeed, jumpForce;
    bool grounded;
    Rigidbody rb;
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody[] meRagdollRigidBodies;
    [SerializeField] Collider[] meRagdollColliders;
    CapsuleCollider meCollider;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == meState.NORMAL)
        {
            var vert = Input.GetAxisRaw("Vertical");
            var horiz = Input.GetAxisRaw("Horizontal");
            Vector3 movement = new Vector3(horiz, 0, vert);
            isMoving = movement.magnitude > 0;
            var rotation = new Quaternion(0, 0, 0, 0);
            rotation = Quaternion.LookRotation(cameraBuddy.transform.forward, Vector3.up);

            var realMove = rotation * movement;

            transform.LookAt(transform.position + realMove);

            anim.SetBool("meMoving", isMoving);
            anim.SetBool("grounded", grounded);
            anim.SetFloat("jumpySpeed", rb.velocity.y);

            if (Input.GetKeyDown(KeyCode.Space) && grounded == true)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                grounded = false;
            }

            if (Input.GetKey(KeyCode.LeftShift) && !grounded)
            {
                anim.SetBool("flip", true);
            }
            else
            {
                anim.SetBool("flip", false);
            }
        }


        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("x pressed");
            if(currentState == meState.NORMAL)
            {
                goRagdollBuddy();
                return;
            }
            if(currentState == meState.MUNTED)
            {
                snapOutOfItBuddy();
                return;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            grounded = true;
            anim.SetTrigger("hitGround");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            grounded = false;
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            rb.velocity = new Vector3(transform.forward.x * moveSpeed, rb.velocity.y, transform.forward.z * moveSpeed);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x * 0.9f, rb.velocity.y, rb.velocity.z * 0.9f);
        }
    }

    //RAGDOLL STATE SHIFT STUFF
    void goRagdollBuddy()
    {
        isMoving = false;
        Debug.Log("going ragdoll");
        currentState = meState.MUNTED;
        anim.speed = 0;
        anim.enabled = false;
        foreach(Rigidbody arbee in meRagdollRigidBodies)
        {
            arbee.isKinematic = false;
            arbee.AddForce(Vector3.Scale(rb.velocity, rb.velocity));
        }
        foreach(Collider collismo in meRagdollColliders)
        {
            collismo.enabled = true;
        }
        rb.isKinematic = true;
        meCollider.enabled = false;
    }

    void snapOutOfItBuddy()
    {
        meCollider.enabled = true;
        rb.isKinematic = false;
        Debug.Log("snapping out of it");
        currentState = meState.NORMAL;
        anim.speed = 1;
        anim.enabled = true;
        foreach (Rigidbody arbee in meRagdollRigidBodies)
        {
            arbee.isKinematic = true;
        }
        foreach (Collider collismo in meRagdollColliders)
        {
            collismo.enabled = false;
        }
    }
}
