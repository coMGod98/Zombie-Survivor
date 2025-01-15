using UnityEngine;

public class WeaponIK : MonoBehaviour
{
    public Animator playerAnimator;
    public Transform leftHandIKTarget;
    public Transform leftElbowIKTarget; 
    public float leftHandWeight;
    public float leftElbowWeight;
    
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTarget.position);
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
        playerAnimator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowIKTarget.position);
        playerAnimator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftElbowWeight);
        
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKTarget.rotation);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);

        if (!playerAnimator.GetBool("IsReload")) return;
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        playerAnimator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
    }
}
