//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Movement : MonoBehaviour
//{

//   public float speed = 10f;
//   public float rotateSpeed = 10f;

//  private Vector3 movement;
//    private float rotation;

//   void Update()
//    {
//      movement.z = Input.GetAxis("Vertical") * speed * Time.deltaTime;
//        rotation = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;
//    }

////    void FixedUpdate()
////    {
////        transform.Translate(movement, Space.Self);
////        transform.Rotate(0f, rotation, 0f);
////    }

////}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10f;
    public float rotateSpeed = 10f;

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float rotation = horizontalInput * rotateSpeed * Time.deltaTime;
        transform.Rotate(0f, rotation, 0f);

        Vector3 movement = transform.forward * verticalInput * speed * Time.deltaTime;

        float playerHeight = 2f;
        Vector3 capsuleBottom = transform.position - Vector3.up * playerHeight / 2f;
        Vector3 capsuleTop = transform.position + Vector3.up * playerHeight / 2f;

        float playerRadius = 3f;
        bool canMove = true;

        
        RaycastHit hit;
        if (Physics.CapsuleCast(capsuleBottom, capsuleTop, playerRadius, movement.normalized, out hit, movement.magnitude))
        {
         
            if (!hit.collider.isTrigger)
            {
                canMove = false; 
            }
        }

        if (canMove)
        {
            transform.Translate(movement, Space.World);
        }
    }
}
