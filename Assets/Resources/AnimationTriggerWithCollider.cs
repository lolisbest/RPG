using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTriggerWithCollider : MonoBehaviour
{
    public new Animation animation;
    public bool EnterTrigger;
    public bool ExitTrigger;

    public AnimationClip EnterAnimation;
    public AnimationClip ExitAnimation;

    void Awake()
    {
        animation = GetComponent<Animation>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animation.IsPlaying(EnterAnimation.name) || animation.IsPlaying(ExitAnimation.name))
            return;

        if(EnterTrigger)
        {
            EnterTrigger = false;

            animation.clip = EnterAnimation;
            animation.Play();
        }
        else if (ExitTrigger)
        {
            ExitTrigger = false;

            animation.clip = ExitAnimation;
            animation.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            EnterTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ExitTrigger = true;
        }
    }
}
