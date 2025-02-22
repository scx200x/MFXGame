using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class MainLevel : MonoBehaviour
{
     public Text Gold;
     public Text Stone;
     public Text GameTime;
     public Transform Canvas;

     public GameObject CurrentSystemUI = null;
     
     public void Fight()
     {
          AudioManager.GetInstance().PlaySound();
     }

     public void Bag()
     {
          AudioManager.GetInstance().PlaySound();
     }

     public void Task()
     {
          AudioManager.GetInstance().PlaySound();
     }

     public void Lineup()
     {
          AudioManager.GetInstance().PlaySound();
     }

     public void Role()
     {
          AudioManager.GetInstance().PlaySound();

          LoadUI(SystemID.Hero);
     }

     public void LianQi()
     {
          AudioManager.GetInstance().PlaySound();
          
     }

     public void ChuanGong()
     {
          AudioManager.GetInstance().PlaySound();
     }

     public void YunYouSiHai()
     {
          AudioManager.GetInstance().PlaySound();
     }

     public void LianLu()
     {
          AudioManager.GetInstance().PlaySound();
     }

     public void Shop()
     {
          AudioManager.GetInstance().PlaySound();
     }

     public void JiuGuan()
     {
          AudioManager.GetInstance().PlaySound();
     }

     private void LoadUI(SystemID ID)
     {
          if (CurrentSystemUI != null)
          {
               CurrentSystemUI.SetActive(false);
               CurrentSystemUI.transform.parent = null;
               Destroy(CurrentSystemUI);
               ResourcesManager.GetInstance().DestroyAsset(GameConfig.SystemConfigDict[ID].Path);
          }
          
          ResourceInfo resourceInfo = ResourcesManager.GetInstance().LoadAsset(GameConfig.SystemConfigDict[ID].Path);

          if (resourceInfo.LoadObj != null)
          {
               CurrentSystemUI = (GameObject)resourceInfo.CloneGameObject(Vector3.zero,quaternion.identity);
               CurrentSystemUI.transform.parent = Canvas;
          }
     }
}