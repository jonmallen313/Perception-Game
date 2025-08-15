using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private Animator anim;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.Play("Idle");
    }
}
