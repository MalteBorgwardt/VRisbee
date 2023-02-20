using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Hand : MonoBehaviour
{
    // Animation
    [SerializeField] private float animationSpeed;

    private Animator _animator;
    private float _gripTarget;
    private float _gripCurrent;

    private const string AnimatorGrabParam = "Grab";
    private static readonly int AnimatorGrab = Animator.StringToHash(AnimatorGrabParam);

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimateHand();
    }

    internal void SetGrab(float v)
    {
        _gripTarget = v;
    }

    void AnimateHand()
    {
        if (_gripCurrent != _gripTarget)
        {
            _gripCurrent = Mathf.MoveTowards(_gripCurrent, _gripTarget, Time.deltaTime * animationSpeed);
            _animator.SetFloat(AnimatorGrab, _gripCurrent);
        }
    }
}
