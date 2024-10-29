using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystem : MonoBehaviour
{
    public int iterations = 5;
    public float angle = 35f;
    public float length = 10f;

    public string axiom = "F";
    private Dictionary<char, string> rules = new Dictionary<char, string>();
    private string currentString;

    void Start()
    {
        rules.Add('F', "F[+FF][-FF]F[-F][+F]F");
        currentString = axiom;

        for (int i = 0; i < iterations; i++)
        {
            currentString = GenerateNextString(currentString);
        }

        DrawLSystem(currentString);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            angle += 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            angle -= 1f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            length += 0.1f;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            length -= 0.1f;
        }
        DrawLSystem(currentString);
    }

    string GenerateNextString(string input)
    {
        string result = "";
        foreach (char c in input)
        {
            result += rules.ContainsKey(c) ? rules[c] : c.ToString();
        }
        return result;
    }

    void DrawLSystem(string input)
    {
        Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
        Vector3 position = Vector3.zero;
        Vector3 direction = Vector3.up;

        foreach (char c in input)
        {
            if (c == 'F')
            {
                Vector3 newPosition = position + direction * length;
                Debug.DrawLine(position, newPosition, Color.green, 1000f, false);
                position = newPosition;
            }
            else if (c == '+')
            {
                direction = Quaternion.Euler(0, 0, angle) * direction;
            }
            else if (c == '-')
            {
                direction = Quaternion.Euler(0, 0, -angle) * direction;
            }
            else if (c == '[')
            {
                transformStack.Push(new TransformInfo
                {
                    position = position,
                    direction = direction
                });
            }
            else if (c == ']')
            {
                TransformInfo ti = transformStack.Pop();
                position = ti.position;
                direction = ti.direction;
            }
        }
    }

    private struct TransformInfo
    {
        public Vector3 position;
        public Vector3 direction;
    }
}
