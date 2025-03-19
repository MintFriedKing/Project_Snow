using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class LobbyInputManager : MonoBehaviour
    {
       
        void Update()
        {
            // 
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    Debug.Log(hit.transform.name);
                    if(hit.collider.CompareTag("Player"))
                    {
                        
                        LobbyManager.Instance.OnTuched();
                    }
                }
            }
            //
        }
    }
}
