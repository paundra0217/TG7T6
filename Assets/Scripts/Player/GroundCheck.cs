using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private LayerMask PlatformLayer;
    private bool isGrounded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("InteractableArea")) return;

        CheckTrigger(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("InteractableArea")) return;

        CheckTrigger(collision);
    }

    private void CheckTrigger(Collider2D collision)
    {
        isGrounded = collision != null && (((1 << collision.gameObject.layer) & PlatformLayer) != 0);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("InteractableArea")) return;

        isGrounded = false;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}
