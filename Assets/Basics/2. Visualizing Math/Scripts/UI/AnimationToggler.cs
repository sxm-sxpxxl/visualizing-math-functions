using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationToggler : MonoBehaviour
{
    private static readonly int IsOpenHash = Animator.StringToHash("isOpen");

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    
    public void Toggle()
    {
        var currentOpenState = _animator.GetBool(IsOpenHash);
        var nextOpenState = !currentOpenState;
        
        _animator.SetBool(IsOpenHash, nextOpenState);
    }
}
