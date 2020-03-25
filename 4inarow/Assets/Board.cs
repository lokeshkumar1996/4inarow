using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class Board : MonoBehaviour
{
    public static Board instance;

    public GameObject boardtile;
    public GameObject maincamera;
    public GameObject redpiece;
    public GameObject yellopiece;

    public int noOfRow ;
    public int noOfCol ;   
    
    public GameObject[,] piece; 
    
    public int clickedpieceX ;
    public int clickedpieceY ;
    public int lastcoinplacedX ;
    public int lastcoinplacedY ;
    

    public bool playerturn = true;

    private int[] xcordinatesfilled;
    
    /*void Awake()
    {      

        if(instance == null)
            instance =this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
      
    }*/


   
    // Start is called before the first frame update
    void Start()
    {
        instance =this;       
        genarateBoard(noOfRow,noOfCol);
    }

    void Update () 
    {


    } 


    private void genarateBoard(int x, int y)
    {
        piece = new GameObject[x,y];
        xcordinatesfilled = new int[x];
        float maincameraXcord = 0;
        float maincameraYcord = 0;

        for(int i=0; i<x; i++)
        {
            for(int j=0; j<y; j++)
            {
               piece[i,j] = Instantiate(boardtile, new Vector3(i * 10F, j*10f, 0f), Quaternion.identity);
               piece[i,j].transform.parent = gameObject.transform;
               piece[i,j].GetComponent<boardPiece>().xCord = i;
               piece[i,j].GetComponent<boardPiece>().yCord = j;
               maincameraYcord = j;
            }
            maincameraXcord = i;
        }
  
      // maincamera.transform.position = new Vector3(maincameraXcord/2 * 10F, maincameraYcord/2 * 10F, -(maincameraXcord * 20f /(float)Math.Sqrt(3)));
       maincamera.transform.position = new Vector3(maincameraXcord/2 * 10F, maincameraYcord/2 * 10F, -(maincameraXcord * 20F)); 
    }





    public void spawnplayercoin()
    {
        if(playerturn)
        {

        Instantiate(yellopiece, new Vector3(clickedpieceX * 10F, xcordinatesfilled[clickedpieceX]*10f, 0f), Quaternion.identity);
        piece[clickedpieceX,xcordinatesfilled[clickedpieceX]].GetComponent<boardPiece>().coin=1;
        lastcoinplacedX=clickedpieceX;
        lastcoinplacedY=xcordinatesfilled[clickedpieceX];
        xcordinatesfilled[clickedpieceX] += 1;        
        }
        else
        {
         Instantiate(redpiece, new Vector3(clickedpieceX * 10F, xcordinatesfilled[clickedpieceX]*10f, 0f), Quaternion.identity);
         piece[clickedpieceX,xcordinatesfilled[clickedpieceX]].GetComponent<boardPiece>().coin=2;
         lastcoinplacedX=clickedpieceX;
         lastcoinplacedY=xcordinatesfilled[clickedpieceX];
         xcordinatesfilled[clickedpieceX] += 1;         
        }
    }


    public void checkwinning(bool player)
    {
        int coinvalue;

        if(player)
        coinvalue =1;
        else coinvalue =2;

        //checking rightside
        if(lastcoinplacedX < noOfRow-3)
        {
        if(piece[lastcoinplacedX+1,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue)
        {
           if(piece[lastcoinplacedX+2,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue)
           {
               if(piece[lastcoinplacedX+3,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue)
               {
                   Debug.Log("player"+coinvalue+" win");
               }
           }
        }
        }

        //checking leftside
        if(lastcoinplacedX > 3)
        {
        if(piece[lastcoinplacedX-1,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue)
        {
           if(piece[lastcoinplacedX-2,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue)
           {
               if(piece[lastcoinplacedX-3,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue)
               {
                   Debug.Log("player"+coinvalue+" win");
               }
           }
        }
        }

        //checking down
        if(lastcoinplacedY>3)
        {
         if(piece[lastcoinplacedX,lastcoinplacedY-1].GetComponent<boardPiece>().coin == coinvalue)
        {
           if(piece[lastcoinplacedX,lastcoinplacedY-2].GetComponent<boardPiece>().coin == coinvalue)
           {
               if(piece[lastcoinplacedX,lastcoinplacedY-3].GetComponent<boardPiece>().coin == coinvalue)
               {
                   Debug.Log("player"+coinvalue+" win");
               }
           }
        }
        }

        //checking right diagonal
        if(lastcoinplacedY >3 && lastcoinplacedX < noOfRow-3)
        {
         if(piece[lastcoinplacedX+1,lastcoinplacedY-1].GetComponent<boardPiece>().coin == coinvalue)
        {
           if(piece[lastcoinplacedX+2,lastcoinplacedY-2].GetComponent<boardPiece>().coin == coinvalue)
           {
               if(piece[lastcoinplacedX+3,lastcoinplacedY-3].GetComponent<boardPiece>().coin == coinvalue)
               {
                   Debug.Log("player"+coinvalue+" win");
               }
           }
        }
        }

        //checking left diagonal
        if(lastcoinplacedY > 3 && lastcoinplacedX > noOfRow+3)
        {
         if(piece[lastcoinplacedX-1,lastcoinplacedY-1].GetComponent<boardPiece>().coin == coinvalue)
        {
           if(piece[lastcoinplacedX-2,lastcoinplacedY-2].GetComponent<boardPiece>().coin == coinvalue)
           {
               if(piece[lastcoinplacedX-3,lastcoinplacedY-3].GetComponent<boardPiece>().coin == coinvalue)
               {
                   Debug.Log("player"+coinvalue+" win");
               }
           }
        }
        }


    }


    
}
