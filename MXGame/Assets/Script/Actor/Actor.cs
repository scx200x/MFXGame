using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fight(bool Enable)
    {
        animator.SetBool("Fight",Enable);
    }

    public void Hit(bool Enable)
    {
        animator.SetBool("Hit",Enable);
    }

    public void Die()
    {
        animator.SetTrigger("Die");
    }

    public void AnimFightEnd()
    {
        Fight(false);
    }

    public void AnimHitEnd()
    {
        Hit(false);
    }

    public void Run(bool Enable)
    {
        animator.SetBool("Run",Enable);
    }
}
