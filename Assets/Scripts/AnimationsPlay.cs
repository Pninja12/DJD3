using UnityEngine;

public class AnimationsPlay : MonoBehaviour
{
    //Codigo feito pelo carvalho porque voces nao trabalham, optimizar isto sff
    // (ta a dar trigger no PlayerMovement.cs)
    [SerializeField] public Animator animator;

    public void Walk()
    {
        animator.SetTrigger("Walk");
    }
    public void StopWalk()
    {
        animator.ResetTrigger("Walk");
    }
    public void Run()
    {
        animator.SetTrigger("Run");
    }
    public void StopRun()
    {
        animator.ResetTrigger("Run");
    }
    public void Jump()
    {
        animator.SetBool("Jump", true);
    }
    public void StopJump()
    {
        animator.SetBool("Jump", false);
    }
    public void Dead()
    {
        animator.Play("Dying");
    }
    public void Shoot()
    {
        animator.SetTrigger("Shoot");
    }
    public void StopShoot()
    {
        animator.ResetTrigger("Shoot");
    }
    public void Crouch()
    {
        animator.SetTrigger("Crouch");
    }
    public void StopCrouch()
    {
        animator.ResetTrigger("Crouch");
    }
    public void Heal()
    {
        animator.SetTrigger("Heal");
    }
    public void StopHeal()
    {
        animator.ResetTrigger("Heal");
    }
   
    

}
