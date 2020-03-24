using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class Board : MonoBehaviour
{
    public GameObject boardtile;
    public GameObject maincamera;

    public int noOfRow ;
    public int noOfCol ;

    // Start is called before the first frame update
    void Start()
    {

        genarateBoard(noOfRow,noOfCol);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void genarateBoard(int x, int y)
    {
        float maincameraXcord = 0;
        float maincameraYcord = 0;

        for(int i=0; i<x; i++)
        {
            for(int j=0; j<y; j++)
            {
                Instantiate(boardtile, new Vector3(i * 10F, j*10f, 0f), Quaternion.identity); 
                maincameraYcord = j;  
            }
            maincameraXcord = i;
        }
       maincamera.transform.position = new Vector3(maincameraXcord/2 * 10F, maincameraYcord/2 * 10F, -(maincameraXcord * 20f /(float)Math.Sqrt(3)));
       //maincamera.transform.position = new Vector3(maincameraXcord/2 * 10F, maincameraYcord/2 * 10F, -(maincameraXcord * 20F )+ maincameraXcord Math.Sqrt(3)/2); 
    }
}
