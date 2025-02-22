using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundBar : MonoBehaviour
{
    public List<GameObject> roundItems;
    public GameObject arrow;
    public GameObject boss;

    private int roundNumber;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetItemCount(int count)
    {
        roundNumber = count;
        
        for (int i = count; i < roundItems.Count ; i++)
        {
            roundItems[i].SetActive(false);
        }

        for (int i = 0; i < roundNumber; i++)
        {
            roundItems[i].transform.GetChild(1).gameObject.SetActive(false);
        }

        SetBossPosition();
        SetArrowPosition(0);
    }

    public void SetArrowPosition(int index)
    {
        arrow.transform.localPosition = new Vector3(139.2f + index * 141.2f, 37.9f, 0);
    }

    public void SetBossPosition()
    {
        boss.transform.localPosition = new Vector3(roundNumber * 141f, 37.9f, 0);
    }
}
