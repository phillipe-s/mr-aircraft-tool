using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUp : MonoBehaviour
{
    // public makes sure you can access it in the game engine
    // and we are setting it to 0.1 now
    public float scale = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) == true)
        
        //when you press the button "u" it should scale up
        if(Input.GetKey("u"))
        {
            // this scales the object up
            transform.localScale = transform.localScale + new Vector3(scale,scale,scale);
        }

        //when you press the button "d" it should scale down
        if(Input.GetKey("d"))
        {
            // this scales the object up
            transform.localScale = transform.localScale + new Vector3(-scale,-scale,-scale);
        }
        
    }
}
