using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BooleanLogicParser;

public class MyTest : MonoBehaviour {

    [SerializeField]
    string myExpression;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            print(myExpression + " = " + CanParseSingleToken(myExpression));
        }

	}

    bool CanParseSingleToken(string expression)
    {
        var tokens = new Tokenizer(expression).Tokenize();
        var parser = new Parser(tokens);
        return parser.Parse();
    }
}
