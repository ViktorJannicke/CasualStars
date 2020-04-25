using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoInput : MonoBehaviour
{
    MakeItCasualInput input;

    private void Awake()
    {
        input = new MakeItCasualInput();

        input.Game.Move.performed += _ => { Debug.Log(_.ReadValue<Vector2>()); };
    }

    void test()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
