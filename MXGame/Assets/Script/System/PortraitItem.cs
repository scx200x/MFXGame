using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitItem : MonoBehaviour
{
    public Image coodTimeImage;
    public Text coodTimeText;
    
    private float coodTime;
    private float tempCoodTime;
    private Actor playerActor;

    public float CoodTime
    {
        set
        {
            tempCoodTime = value;
            coodTime = value;
        }

        get
        {
            return coodTime;
        }
    }

    public Actor PlayerActor
    {
        set
        {
            playerActor = value;
        }

        get
        {
            return playerActor;
        } 
    }
    
    // Start is called before the first frame update
    void Start()
    {
        coodTimeText.text = "";
        coodTimeImage.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (tempCoodTime > 0f)
        {
            tempCoodTime -= Time.deltaTime;

            if (tempCoodTime <= 0.0001f)
            {
                coodTime = tempCoodTime = 0f;
                coodTimeText.text = "";
                coodTimeImage.fillAmount = 0f;
            }
            else
            {
                coodTimeImage.fillAmount = tempCoodTime / coodTime;
                coodTimeText.text = Mathf.CeilToInt(coodTimeImage.fillAmount * coodTime).ToString();
            }
        }
    }
}
