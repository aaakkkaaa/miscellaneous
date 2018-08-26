using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BooleanLogicParser;

public class LogicalParserTest : MonoBehaviour
{

    void Start()
    {
        print(CanParseSingleToken("true")); // ExpectedResult = true)
        //print(CanParseSingleToken(")")); // ExpectedException = (typeof(Exception)))
        //print(CanParseSingleToken("az")); // ExpectedException = (typeof(Exception)))
        //print(CanParseSingleToken("")); // ExpectedException = (typeof(Exception)))
        //print(CanParseSingleToken("()")); // ExpectedException = typeof(Exception))
        //print(CanParseSingleToken("true and")); // ExpectedException = typeof(Exception))
        print(CanParseSingleToken("false")); // ExpectedResult = false)
        print(CanParseSingleToken("true ")); // ExpectedResult = true)
        print(CanParseSingleToken("false ")); // ExpectedResult = false)
        print(CanParseSingleToken(" true")); // ExpectedResult = true)
        print(CanParseSingleToken(" false")); // ExpectedResult = false)
        print(CanParseSingleToken(" true ")); // ExpectedResult = true)
        print(CanParseSingleToken(" false ")); // ExpectedResult = false)
        print(CanParseSingleToken("(false)")); // ExpectedResult = false)
        print(CanParseSingleToken("(true)")); // ExpectedResult = true)
        print(CanParseSingleToken("true and false")); // ExpectedResult = false)
        print(CanParseSingleToken("false and true")); // ExpectedResult = false)
        print(CanParseSingleToken("false and false")); // ExpectedResult = false)
        print(CanParseSingleToken("true and true")); // ExpectedResult = true)
        print(CanParseSingleToken("!true")); // ExpectedResult = false)
        print(CanParseSingleToken("!(true)")); // ExpectedResult = false)
        //print(CanParseSingleToken("!(true")); // ExpectedException = typeof(Exception))
        print(CanParseSingleToken("!(!(true))")); // ExpectedResult = true)
        print(CanParseSingleToken("!false")); // ExpectedResult = true)
        print(CanParseSingleToken("!(false)")); // ExpectedResult = true)
        print(CanParseSingleToken("(!(false)) and (!(true))")); // ExpectedResult = false)
        print(CanParseSingleToken("!((!(false)) and (!(true)))")); // ExpectedResult = true)
        print(CanParseSingleToken("!false and !true")); // ExpectedResult = false)
        print(CanParseSingleToken("false and true and true")); // ExpectedResult = false)
        print(CanParseSingleToken("false or true or false")); // ExpectedResult = true)
    }

    public bool CanParseSingleToken(string expression)
    {
        var tokens = new Tokenizer(expression).Tokenize();
        var parser = new Parser(tokens);
        return parser.Parse();
    }
}
