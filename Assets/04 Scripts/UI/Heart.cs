using System;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    [SerializeField] private Image inside;
    [SerializeField] private ParticleSystem hitPS;
    [SerializeField] private Animator heartAnimator;
    
    public void Full(Color color)
    {
        inside.enabled = true;
        inside.color = color;
       
    }

    public void Empty()
    {
       
        inside.enabled = false;
        hitPS.Play();
        heartAnimator.SetTrigger("Low_Health");
    }

   
}
