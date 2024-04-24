using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BkScroll : MonoBehaviour
{
    const float MoveLength = 19f;
    public float Speed;
    public GameObject Image1;
    public GameObject Image2;

    private int MoveNumber = 0;
    public float MovedDis = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Speed * Time.deltaTime * Vector3.right;
        
        MovedDis -= Speed * Time.deltaTime;

        if (MovedDis >= MoveLength)
        {
            MoveNumber += 1;
            MovedDis = 0f;

            if (MoveNumber % 2 == 1)
            {
                Image1.transform.localPosition = new Vector3(19.2f * (MoveNumber + 1), 0, 0);
            }
            else
            {
                Image2.transform.localPosition = new Vector3(19.2f * (MoveNumber + 1), 0, 0);
            }
        }
    }
}
