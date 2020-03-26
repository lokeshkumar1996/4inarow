using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Board : MonoBehaviour
{
    public static Board instance;

    public GameObject boardtile;
    public GameObject maincamera;
    public GameObject redpiece;
    public GameObject yellopiece;
    public GameObject Gamewinningpanel;
    public GameObject cpuwin;
    public GameObject playerwin;

    
    public int noOfRow ; // pieces in x cordinates , minrow 4
    public int noOfCol ; // pieces in y cordinates, min colum 4
    public int difficultychoice ;
    
    public GameObject[,] piece; 
    
    //public int clickedpieceX ;
    //public int clickedpieceY ;
    public int lastcoinplacedX ;
    public int lastcoinplacedY ;
    

    public bool playerturn = true;
    public bool gameover = false;

    private int[] xcordinatesfilled;


    //botvariables
    public int cpuCheckRightendX;
    public int cpuCheckRightendY;
    public int cpuCheckLeftendX;
    public int cpuCheckLeftendY;
    public bool hardmode =false;
    public List<int> columnstofill = new List<int>();
    
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

        for(int i=0; i<noOfRow; i++)
        {
            columnstofill.Add(i);
        }
        
    }

    void Update () 
    {
        if(!playerturn)
        {
            

            if(!hardmode)
            {
            Cpumoveeasy();
            }
            else
            {
             Cpumovehard();               
            }
            playerturn =true;
        }

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
       maincamera.transform.position = new Vector3(maincameraXcord/2 * 10F, maincameraYcord/2 * 10F +maincameraYcord, -10f); 
       Camera.main.orthographicSize = noOfRow*10f ;
       
    }





    public void spawnplayercoin(int clickedpieceX, int clickedpieceY )
    {
       if(xcordinatesfilled[clickedpieceX] < noOfCol)
       {

        if(playerturn)
        {
        GameObject coin = Instantiate(yellopiece, new Vector3(clickedpieceX * 10F, xcordinatesfilled[clickedpieceX]*10f, 0f), Quaternion.identity);
        coin.transform.parent = piece[clickedpieceX,xcordinatesfilled[clickedpieceX]].transform;
        piece[clickedpieceX,xcordinatesfilled[clickedpieceX]].GetComponent<boardPiece>().coin=1;
        lastcoinplacedX=clickedpieceX;
        lastcoinplacedY=xcordinatesfilled[clickedpieceX];
        xcordinatesfilled[clickedpieceX] += 1;
            if(xcordinatesfilled[clickedpieceX] >= noOfCol)
            {
                columnstofill.Remove(clickedpieceX);
            }
            
        }
        else
        {
         GameObject coin = Instantiate(redpiece, new Vector3(clickedpieceX * 10F, xcordinatesfilled[clickedpieceX]*10f, 0f), Quaternion.identity);
         coin.transform.parent = piece[clickedpieceX,xcordinatesfilled[clickedpieceX]].transform;
         piece[clickedpieceX,xcordinatesfilled[clickedpieceX]].GetComponent<boardPiece>().coin=2;
         lastcoinplacedX=clickedpieceX;
         lastcoinplacedY=xcordinatesfilled[clickedpieceX];
         xcordinatesfilled[clickedpieceX] += 1; 
            if(xcordinatesfilled[clickedpieceX] >= noOfCol)
            {
                columnstofill.Remove(clickedpieceX);
            }
        }
       
        if(!gameover)
           checkwinning(playerturn);

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
            Debug.Log("right side check");
        if(piece[lastcoinplacedX+1,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue)
        {
           if(piece[lastcoinplacedX+2,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue)
           {
               Debug.Log("check right hardmode");
               cpuCheckRightendX=lastcoinplacedX+3;
               cpuCheckRightendY=lastcoinplacedY;
               cpuCheckLeftendX=lastcoinplacedX-1;
               cpuCheckLeftendY=lastcoinplacedY;
               hardmode = true;
               if(piece[lastcoinplacedX+3,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue)
               {
                   hardmode = true;
                   Debug.Log("player"+coinvalue+" win");
                   Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX+1,lastcoinplacedY],piece[lastcoinplacedX+2,lastcoinplacedY],piece[lastcoinplacedX+3,lastcoinplacedY]);
               }
           }
        }
        }

        //checking leftside
        if(lastcoinplacedX >= 3)
        {
           Debug.Log("left side check");
        if(piece[lastcoinplacedX-1,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue)
        {
           if(piece[lastcoinplacedX-2,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue)
           {
               Debug.Log("check left hardmode");
               cpuCheckRightendX=lastcoinplacedX+1;
               cpuCheckRightendY=lastcoinplacedY;
               
               hardmode = true;

               if(piece[lastcoinplacedX-3,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue)
               {
                   Debug.Log("player"+coinvalue+" win");
                   Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX-1,lastcoinplacedY],piece[lastcoinplacedX-2,lastcoinplacedY],piece[lastcoinplacedX-3,lastcoinplacedY]);             
               }
           }
        }
        }

        //checking down
        if(lastcoinplacedY>=2)
        {
                Debug.Log("down check");
         if(piece[lastcoinplacedX,lastcoinplacedY-1].GetComponent<boardPiece>().coin == coinvalue)
        {
           if(piece[lastcoinplacedX,lastcoinplacedY-2].GetComponent<boardPiece>().coin == coinvalue)
           {
               Debug.Log("check down hardmode");
               if(lastcoinplacedY+1 < noOfCol)
               {
               cpuCheckRightendX=lastcoinplacedX;
               cpuCheckRightendY=lastcoinplacedY+1;
               cpuCheckLeftendX=lastcoinplacedX;
               cpuCheckLeftendY=lastcoinplacedY+1;
               hardmode = true;   
               }            

             if(lastcoinplacedY>=3)
                {
               if(piece[lastcoinplacedX,lastcoinplacedY-3].GetComponent<boardPiece>().coin == coinvalue)
               {
                   Debug.Log("player"+coinvalue+" win");
                   Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX,lastcoinplacedY-3],piece[lastcoinplacedX,lastcoinplacedY-2],piece[lastcoinplacedX,lastcoinplacedY-1]);
               }
                }
           }
        }
        }

        //checking right diagonal down
        if(lastcoinplacedY >= 3 && lastcoinplacedX < noOfRow-3)
        {
            //Debug.Log("right diagonal down");
        if(piece[lastcoinplacedX+1,lastcoinplacedY-1].GetComponent<boardPiece>().coin == coinvalue)
        {
           if(piece[lastcoinplacedX+2,lastcoinplacedY-2].GetComponent<boardPiece>().coin == coinvalue)
           {
               if(piece[lastcoinplacedX+3,lastcoinplacedY-3].GetComponent<boardPiece>().coin == coinvalue)
               {
                   Debug.Log("player"+coinvalue+" win");
                   Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX+3,lastcoinplacedY-3],piece[lastcoinplacedX+2,lastcoinplacedY-2],piece[lastcoinplacedX+1,lastcoinplacedY-1]);
               }
           }
        }
        }

        //checking left diagonal down
        if(lastcoinplacedY >= 3 && lastcoinplacedX >= 3)
        {
            // Debug.Log("left diagonal down");
         if(piece[lastcoinplacedX-1,lastcoinplacedY-1].GetComponent<boardPiece>().coin == coinvalue)
        {
           if(piece[lastcoinplacedX-2,lastcoinplacedY-2].GetComponent<boardPiece>().coin == coinvalue)
           {
               if(piece[lastcoinplacedX-3,lastcoinplacedY-3].GetComponent<boardPiece>().coin == coinvalue)
               {
                   Debug.Log("player"+coinvalue+" win");
                    Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX-3,lastcoinplacedY-3],piece[lastcoinplacedX-2,lastcoinplacedY-2],piece[lastcoinplacedX-1,lastcoinplacedY-1]);
               }
           }
        }
        }


        //chcek in betweens of the 4 in a row horizontal
        if(lastcoinplacedX>0 && lastcoinplacedX < noOfRow-3)
        {
            if(piece[lastcoinplacedX-1,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue && piece[lastcoinplacedX+1,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue && piece[lastcoinplacedX+2,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue )
            {
            Debug.Log("player"+coinvalue+" win");
            Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX-1,lastcoinplacedY],piece[lastcoinplacedX+1,lastcoinplacedY],piece[lastcoinplacedX+2,lastcoinplacedY]);
            }
        }

        //chcek in betweens of the 4 in a row horizontal
        if(lastcoinplacedX>2 && lastcoinplacedX < noOfRow-1)
        {
            if(piece[lastcoinplacedX-1,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue && piece[lastcoinplacedX-2,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue && piece[lastcoinplacedX+1,lastcoinplacedY].GetComponent<boardPiece>().coin == coinvalue )
            {
            Debug.Log("player"+coinvalue+" win");
            Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX-1,lastcoinplacedY],piece[lastcoinplacedX-2,lastcoinplacedY],piece[lastcoinplacedX+1,lastcoinplacedY]);
            }
        }


        changeturns();




    }

    public void Restartgame()
    {  
       SceneManager.LoadScene("Game");  
    }

    private void Resetgame()
    {  
        transform.position = transform.position + new Vector3(0,0,50f);
       Gamewinningpanel.SetActive(true);
       if(playerturn)
        {          
        playerwin.SetActive(true);
        }
        else
        {
        cpuwin.SetActive(true);
        }
      
       
    }



    private void Higlight(GameObject piece1,GameObject piece2,GameObject piece3,GameObject piece4)
    {   
        gameover = true;
        piece1.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;
        piece2.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;
        piece3.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;
        piece4.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;

        Invoke("Resetgame",2f);
    }


    private void Cpumoveeasy()
    {
        Debug.Log("cpu easy move");
        spawnplayercoin( columnstofill[Random.Range(0, columnstofill.Count)], Random.Range(0,noOfCol-1));
    }

    private void Cpumovehard()
    {
        Debug.Log("cpu hard move");
        
        try
        {
            if(piece[cpuCheckRightendX,cpuCheckRightendY].GetComponent<boardPiece>().coin == 0)
            {
            spawnplayercoin(cpuCheckRightendX, cpuCheckRightendY);
            hardmode = false;
            }
        }
        catch 
        { 
            Debug.Log("index error trying 2nd possibility");
            try
            {
                if(piece[cpuCheckLeftendX,cpuCheckLeftendY].GetComponent<boardPiece>().coin == 0)
                {
                spawnplayercoin(cpuCheckLeftendX, cpuCheckLeftendY);
                hardmode = false;
                }
            }
            catch
            {
                Debug.Log("index error trying 2nd  failed");
                Cpumoveeasy();

            }           

        }

        /*if(piece[cpuCheckRightendX,cpuCheckRightendY].GetComponent<boardPiece>().coin == 0)
        {
        spawnplayercoin(cpuCheckRightendX, cpuCheckRightendY);
        }
        else if(piece[cpuCheckLeftendX,cpuCheckLeftendY].GetComponent<boardPiece>().coin == 0)
        {
        spawnplayercoin(cpuCheckLeftendX, cpuCheckLeftendY);
        }*/        
    }

    private void changeturns()
    {
        Debug.Log("pleyar turn change"+ playerturn);

        if(playerturn)
        {          
        playerturn = false;
        }
        else
        {
        playerturn = true; 
        }
    }




   

    


    
}
