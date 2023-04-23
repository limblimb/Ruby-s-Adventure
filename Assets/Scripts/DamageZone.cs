using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RubyController controller = other.GetComponent<RubyController>();
            if (other != null)
            {
                controller.ChangeHealth(-1);
                //controller.damageEffect.Play();
            }
        }
        
    }
}
