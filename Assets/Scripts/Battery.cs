using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Battery : MonoBehaviour
{
    [SerializeField] public MeshFilter MeshFilter;
    public MeshRenderer MeshRenderer;
    void Start()
    {
        MeshFilter = GetComponentInChildren<MeshFilter>();
        if (!TryGetComponent<MeshRenderer>(out MeshRenderer))
            MeshRenderer = GetComponentInChildren<MeshRenderer>();
    }
}
