using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeed : MonoBehaviour
{
    [SerializeField] private float Speed = 100f;

    private float defaultSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        defaultSpeed = Speed;
    }

    public void SetSpeed(float speed)
    {
        Speed = speed;
    }

    public void ResetSpeed()
    {
        Speed = defaultSpeed;
    }

    public float GetSpeed()
    {
        return Speed;
    }
}
