using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData weaponData;
    public WeaponType weaponType;
    
    public List<Monster> hitMonsterList;
    public float attackElapsedTime;
    
    public Vector3 initialVelocity;
    public Vector3 currentVelocity;
    public Vector3 targetPosition;
    public bool isMoving;
    
    public void WeaponInit()
    {
        hitMonsterList.Clear();
        attackElapsedTime = 0.0f;
    }
    
    private void Awake()
    {
        hitMonsterList = new List<Monster>();
    }
    
    public void Initialize(Vector3 velocity, Vector3 target)
    {
        initialVelocity = velocity;
        currentVelocity = velocity;
        targetPosition = target;
        isMoving = true;
    }

}