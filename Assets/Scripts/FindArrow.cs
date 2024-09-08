using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindArrow : MonoBehaviour
{
    // Start is called before the first frame update


    Vector3 FindMouse()
    {
        Vector3 vector3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return vector3;
    }

    void ReplacePlatform()
    {   /*
        razmerPlatform = 7; maxleft = -14.5f;
        razmerPlatform 3; maxleft = -16.5;
        razmrPlatform 1 = maxleft = -17.5;
        1 = 0.5f
        7-3=4,4/2 = 2 =
        */
        
        var position = FindMouse();
        if (position.x >= -18.4f + transform.localScale.x/2  && position.x <= 4f - transform.localScale.x / 2)
        {
            transform.position = new Vector3(position.x, -8.5f, 0);
        }
        
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ReplacePlatform();
    }
}
