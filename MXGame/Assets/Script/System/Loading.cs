using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Scrollbar loadingBar;
    public float progress;

    private const float LoadingTime = 3.0f;
    private bool isLoadingOver = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        progress += Time.deltaTime;
        loadingBar.size = progress / (LoadingTime - 1.0f);

        if (progress >= LoadingTime && !isLoadingOver)
        {
            isLoadingOver = true;
            EventMsgCenter.SendMsg(EventName.LoadingOver);
        }
    }
}
