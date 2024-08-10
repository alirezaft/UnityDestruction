using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils
{
    public static float CalculateHypotenuse(float side1, float side2)
    {
        return Mathf.Sqrt(Mathf.Pow(side1, 2) + Mathf.Pow(side2, 2));
    }

    public static float CalculateOtherSideHavingHypotenuse(float hypotenuse, float side)
    {
        return Mathf.Sqrt(Mathf.Pow(hypotenuse, 2) - Mathf.Pow(side, 2));
    }
    
    
}
