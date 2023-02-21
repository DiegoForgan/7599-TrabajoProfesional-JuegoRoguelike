using CustomizableCharacters;
using System;
using System.Collections;
using UnityEngine;

public class CharactersAnimator : MonoBehaviour
{
    [SerializeField] private CustomizableCharacter customizableCharacter;
    [SerializeField] private Animator animator;
    private GameObject currentDirectionGameObject;
    private int animatorDirection = 2;
    private Vector2 previousDirection;
    //This solution kind of sucks because it relies on this boolean to not trigger another animation
    //This is neccesary in order to let the death animation finish and therefore run the event function
    private bool isDead = false;

    public void ResetRigs()
    {
        customizableCharacter.UpRig.SetActive(false);
        customizableCharacter.SideRig.SetActive(false);
        currentDirectionGameObject = customizableCharacter.DownRig;
        currentDirectionGameObject.SetActive(true);
        animator.SetFloat("Direction", animatorDirection);
        isDead = false;
    }
    public void HandleMovementAnimation(Vector2 direction, float currentSpeed)
    {
        if (isDead) return;
        HandleDirectionAnimation(direction);
        HandleSpeed(direction, currentSpeed);
    }
    public void SetShowWeapon(bool shouldShowWeapon) {
        if (isDead) return;
        animator.SetBool("Showing Weapon", shouldShowWeapon);
    }
    private void HandleSpeed(Vector2 direction, float currentSpeed) {
        //Partially fixed, should take into account the real speed of the movement
        float animatorSpeed = (direction == Vector2.zero || currentSpeed == 0f) ? 0f : 1f;
        animator.SetFloat("Speed", animatorSpeed);
    }
    private void HandleDirectionAnimation(Vector2 direction)
    {
        if (direction == Vector2.zero || previousDirection == direction) return;

        currentDirectionGameObject?.SetActive(false);

        if (direction == Vector2.right)
        {
            currentDirectionGameObject = customizableCharacter.SideRig;
            var scale = customizableCharacter.SideRig.transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            currentDirectionGameObject.transform.localScale = scale;
            animatorDirection = 1;
        }
        else if (direction == Vector2.left)
        {
            currentDirectionGameObject = customizableCharacter.SideRig;
            var scale = customizableCharacter.SideRig.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -1;
            currentDirectionGameObject.transform.localScale = scale;
            animatorDirection = 1;
        }
        else if (direction == Vector2.up)
        {
            currentDirectionGameObject = customizableCharacter.UpRig;
            animatorDirection = 0;
        }
        else if (direction == Vector2.down)
        {
            currentDirectionGameObject = customizableCharacter.DownRig;
            animatorDirection = 2;
        }

        currentDirectionGameObject?.SetActive(true);
        animator.SetFloat("Direction", animatorDirection);
    }
    internal void setAttackAnimation()
    {
        if (isDead) return;
        animator.SetTrigger("Attack 1");
    }
    internal void setSpellCastingAnimation()
    {
        if (isDead) return;
        animator.SetTrigger("Spell");
    } 
    internal void setHurtAnimation()
    {
        if (isDead) return;
        animator.SetTrigger("Hurt");
    }
    internal void setDeadAnimation()
    {
        //if (isDead) return;
        isDead = true;
        animator.SetTrigger("Die");
    }
    internal void SetSpellCastingWithStaffAnimation()
    {
        if (isDead) return;
        animator.SetTrigger("Stab");
    }
    internal void SetArrowThrowingAnimation()
    {
        if (isDead) return;
        StartCoroutine(ArrowThrowingAnimation());
    }
    private IEnumerator ArrowThrowingAnimation()
    {
        // enter animator to enter bow load state
        animator.SetTrigger("Bow Load");
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Bow Load") == false)
            yield return null;

        animator.ResetTrigger("Bow Load");

        // wait for bow load animation
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Bow Load")
               && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        // enter animator to enter bow release state
        animator.SetTrigger("Bow Release");
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Bow Release") == false)
            yield return null;

        animator.ResetTrigger("Bow Release");
    }
}
