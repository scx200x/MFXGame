using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRecord : MonoBehaviour
{
    public GameObject RoundImageObj;
    public GameObject EmptyObj;
    public Text RoundTxt;
    public Text GameTimeTxt;
    public Text PlayTimeTxt;

    private long roleID;

    public long RoleID
    {
        set { roleID = value;}
        get { return RoleID; }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        EmptyObj.SetActive(true);
        RoundImageObj.SetActive(false);
        GameTimeTxt.text = "";
        PlayTimeTxt.text = "";
        roleID = 0;
    }

    public void Play()
    {
        AudioManager.GetInstance().PlaySound();
    }

    public void UpdateUI(Cs.AccSimpleInfo simpleInfo)
    {
        simpleInfo.RoleId = simpleInfo.RoleId;
    }
    
}
