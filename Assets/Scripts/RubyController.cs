using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int maxHealth = 5;
    public float speed = 3.0f;
    public float timeInvincible = 2.0f;
    public ParticleSystem healingEffect;
    public ParticleSystem damageEffect;

    int currentHealth;
    public int health { get { return currentHealth; } }

    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;
    AudioSource footstepsAudioSource;
    public AudioClip throwingSound;
    public AudioClip stepsSound;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        footstepsAudioSource = transform.Find("FootstepsSound").gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();
            PlaySound(throwingSound);
        }


        /* 
        1.  The starting point in your example is an upward offset from Ruby’s position because you want to test from the centre of Ruby’s Sprite, not from her feet. 
        2.  The direction, which is the direction that Ruby is looking.
        3.  The maximum distance of your ray should be set to 1.5 so the Raycast doesn’t test intersections that are 1.5 units away from the starting point.
        4.  A layer mask which allows us to test only certain layers. Any layers that are not part of the mask will be ignored during the intersection test. Here, you will select the NPC layer, because that one contains your frog. 
        Finally, test to see if your Raycast has hit a Collider. If the Raycast didn’t intersect anything, this will be null so do nothing. Otherwise, RaycastHit2D will contain the Collider the Raycast intersected, so you will go inside your final if block to log the object you have just found with the Raycast.
        */
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NPC character = hit.collider.GetComponent<NPC>();
                if (character != null)
                {
                    character.DisplyDialog();
                }
            }
        }

        if (rigidbody2d.velocity.magnitude > 0)
        {
            if (!footstepsAudioSource.isPlaying)
            {
                footstepsAudioSource.PlayOneShot(stepsSound);
            }
        }
        else footstepsAudioSource.Stop();
        
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        

        rigidbody2d.MovePosition(position);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            damageEffect.Play();
            invincibleTimer = timeInvincible;
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
    }
}
