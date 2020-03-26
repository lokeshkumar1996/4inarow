using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchdetect : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject mainCamera;
    void Start()
    {
        
    }

    // Update is called once per frame
    	/*void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 v =Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, -136.5f));
            
            Debug.Log(v);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //Select Stage
                if (hit.transform.tag == "piece")
                {
                    Debug.Log("SceneTwo");
                }
            }
        }

    }*/

       void Update () {

        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Input.mousePosition;
            Debug.Log("Mouse pressed " + pos);

            Ray ray = Camera.main.ScreenPointToRay(pos);
             RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //Select Stage
                if (hit.transform.tag == "piece")
                {
                    Debug.Log("SceneTwo");
                }
            }

        }
    }




}
