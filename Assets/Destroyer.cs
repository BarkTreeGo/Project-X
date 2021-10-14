using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] GameObject targetToDesttroy = null;

    public void DestroyTarget()
    {
        Destroy(targetToDesttroy);
    }
}
