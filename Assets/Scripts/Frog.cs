using UnityEngine;

public class Frog : MonoBehaviour
{
    private Animator anim;
    private Collider2D cell;
    private Rigidbody2D rb2d;

    [SerializeField] private LayerMask ground;
    [SerializeField] private float leftCap = 3f;
    [SerializeField] private float rightCap = 14f;
    [SerializeField] private float jumpLength = 6f;
    [SerializeField] private float jumpHeight = 6f;

    private bool facingLeft = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
        cell = GetComponent<Collider2D>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (anim.GetBool("Jumping"))
        {
            if (rb2d.velocity.y < .1)
            {
                anim.SetBool("Falling", true);
                anim.SetBool("Jumping", false);
            }
        }
        if (cell.IsTouchingLayers(ground) && anim.GetBool("Falling"))
        {
            anim.SetBool("Falling", false); 
        }
    }

    private void Movement()
    {
        if (facingLeft)
        {
            if (transform.position.x > leftCap)
            {
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }

                if (cell.IsTouchingLayers(ground))
                {
                    rb2d.velocity = new Vector2(-jumpLength, jumpHeight);
                    anim.SetBool("Jumping", true);
                }
            }
            else
            {
                facingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightCap)
            {
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }

                if (cell.IsTouchingLayers(ground))
                {
                    rb2d.velocity = new Vector2(jumpLength, jumpHeight);
                    anim.SetBool("Jumping", true);
                }
            }
            else
            {
                facingLeft = true;
            }
        }
    }

    public void JumpedOn()
    {
        anim.SetTrigger("Death");
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}