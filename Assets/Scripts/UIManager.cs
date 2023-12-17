using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    static UIManager instance;
    public static UIManager Instance { get => instance; }

    [SerializeField] Toggle pearlSize8mm;
    [SerializeField] Toggle pearlSize10mm;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
