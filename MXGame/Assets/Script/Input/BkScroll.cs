using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class BkScroll : MonoBehaviour
{
    public float moveLength;
    public float Speed;
    public List<GameObject> Images;
    public float movedDis = 0f;
    public float imageLength;
    public float startPos;
    
    private int moveNumber = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Speed * Time.deltaTime * Vector3.right;
        
        movedDis -= Speed * Time.deltaTime;

        if (movedDis >= moveLength)
        {
            movedDis = 0f;

            moveNumber = moveNumber % Images.Count;

            startPos = Images[moveNumber].transform.localPosition.x;
            
            Images[moveNumber].transform.localPosition = new Vector3(imageLength * Images.Count + startPos, Images[moveNumber].transform.localPosition.y, 0);

            moveNumber += 1;
        }
    }
}
