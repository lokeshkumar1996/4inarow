using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


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
    public GameObject Canvas; 

    public Text playerscoretext;
    public Text cpuscoretext;

    int playerscore =0;
    int cpuscore=0;  

    
    public int noOfRow ; // pieces in x cordinates , min row 4
    public int noOfCol ; // pieces in y cordinates, min colum 4    
    
    public GameObject[,] piece; 
    
    
    public int lastcoinplacedX ;
    public int lastcoinplacedY ;
    

    [SerializeField] private bool playerturn = true;
    [SerializeField] bool gameover = false;
    bool playerstartsgame = true;

    private int[] xcordinatesfilled; //no of coins filled in easch posistion of xcordinate
    
    private int[,] boardcoins; // array of coins placed in the board


    //botvariables
    public int[] cpumove = new int[2];  // index 0 is for defence position of X and index 1 is for offence position of X 
    public bool defencemode =false;
    public bool offencemode =false;
    public List<int> columnstofill = new List<int>();
    public List<int> noeasymove = new List<int>();
    public bool cpufirstmove = false;


   
    // Start is called before the first frame update
    void Start()
    {
        instance =this;       
        genarateBoard(noOfRow,noOfCol);

        //populating the list
        for(int i=0; i<noOfRow; i++)
        {
            columnstofill.Add(i);
        }

        //switch player and cpu turns at start
        if(playerstartsgame)
        {
            playerturn = true;
            playerstartsgame =false; //for next game
            
        }
        else 
        {
            playerturn = false;
            cpufirstmove = true;
            playerstartsgame = true; //for next game
        } 
    }

    void Update () 
    {
        if(!playerturn) //cpu turn
        { 
            //chosing different mode
            checkpossiblemoves("defence");
            if(defencemode) //plays in defence
            {
             Cpudefencemove(); 
            }
            else  //plays in offence
            {
                checkpossiblemoves("offence");
                if(offencemode)    
                 {
                 Cpuoffencemove();  
                 }   
                 else
                 {
                 Cpueasymove();
                 }             
            }
            playerturn =true;
        }
    } 


    private void genarateBoard(int x, int y)
    {
        piece = new GameObject[x,y];
        boardcoins = new int[x,y];
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
       //Canvas.GetComponent<RectTransform>().localPosition = new Vector3(-maincameraXcord/2 * 10F, -maincameraYcord/2 * 10F +maincameraYcord, 100f);
       
       Camera.main.orthographicSize = noOfRow*10f ;
       
    }

    private void regenarateBoard(int x, int y)
    {    
        // reset xcordinatesfilled to 0
        for(int i =0; i<noOfRow; i++)
            {
                xcordinatesfilled[i] = 0;
            }

        //reset all  boardcoins to 0 and adn destory coins
        for(int j =0; j<noOfCol; j++)
        {
            for(int i =0; i<noOfRow; i++)
            {
                boardcoins[i,j] = 0;
                if(piece[i,j].transform.childCount > 0 )
                Destroy(piece[i,j].transform.GetChild(0).gameObject);
            }
        } 

        //regernearte the colunmsfill list
        columnstofill.Clear();
        for(int i=0; i<noOfRow; i++)
        {
            columnstofill.Add(i);
            
        }

        //switch player and cpu turns at start
        if(playerstartsgame)
        {
            playerturn = true;
            playerstartsgame = false; //for next game
        }
        else 
        {
            playerturn = false;
            cpufirstmove = true;
            playerstartsgame = true; //for next game

        }     
        
       
    }





    public void spawnplayercoin(int clickedpieceX, int clickedpieceY )
    {
       if(xcordinatesfilled[clickedpieceX] < noOfCol)
       {

        if(playerturn)
        {
            GameObject coin = Instantiate(yellopiece, new Vector3(clickedpieceX * 10F, xcordinatesfilled[clickedpieceX]*10f, 0f), Quaternion.identity);
            coin.transform.parent = piece[clickedpieceX,xcordinatesfilled[clickedpieceX]].transform;        
            boardcoins[clickedpieceX,xcordinatesfilled[clickedpieceX]] = 1;

            lastcoinplacedX=clickedpieceX;
            lastcoinplacedY=xcordinatesfilled[clickedpieceX];

                if(xcordinatesfilled[clickedpieceX] < noOfCol-1 )
                {
                    xcordinatesfilled[clickedpieceX] += 1;
                }        
                if(xcordinatesfilled[clickedpieceX] >= noOfCol)
                {
                    columnstofill.Remove(clickedpieceX);
                }
                
        }
        else
        {
            GameObject coin = Instantiate(redpiece, new Vector3(clickedpieceX * 10F, xcordinatesfilled[clickedpieceX]*10f, 0f), Quaternion.identity);
            coin.transform.parent = piece[clickedpieceX,xcordinatesfilled[clickedpieceX]].transform;         
            boardcoins[clickedpieceX,xcordinatesfilled[clickedpieceX]] = 2;

            lastcoinplacedX=clickedpieceX;
            lastcoinplacedY=xcordinatesfilled[clickedpieceX];

                    
                if(xcordinatesfilled[clickedpieceX] < noOfCol-1 )
                {
                    xcordinatesfilled[clickedpieceX] += 1;
                }        
                if(xcordinatesfilled[clickedpieceX] >= noOfCol)
                {
                    columnstofill.Remove(clickedpieceX);
                }
        }

       
        if(!gameover)
        checkwinning(playerturn);

       }
    }




    //when a coin is dropped it check the all possstion to determind if the player wins or not
    public void checkwinning(bool player)
    {
        int coinvalue; //CHECK RED OR YELLOW COIN
        if(player)
        coinvalue =1;
        else coinvalue =2;

        //checking rightside 
        if(lastcoinplacedX < noOfRow-3)
        {
            if(boardcoins[lastcoinplacedX+1,lastcoinplacedY] == coinvalue)
            {
                if(boardcoins[lastcoinplacedX+2,lastcoinplacedY] == coinvalue)
                {
                    if(boardcoins[lastcoinplacedX+3,lastcoinplacedY]== coinvalue)
                    {
                    defencemode = true;
                    Debug.Log("player"+coinvalue+" win");
                    Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX+1,lastcoinplacedY],piece[lastcoinplacedX+2,lastcoinplacedY],piece[lastcoinplacedX+3,lastcoinplacedY]);
                    }
                }
            }
        }

        //checking leftside
        if(lastcoinplacedX >= 3)
        {
            if(boardcoins[lastcoinplacedX-1,lastcoinplacedY]== coinvalue)
             {
                if(boardcoins[lastcoinplacedX-2,lastcoinplacedY]== coinvalue)
                {
                    if(boardcoins[lastcoinplacedX-3,lastcoinplacedY]== coinvalue)
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
            if(boardcoins[lastcoinplacedX,lastcoinplacedY-1] == coinvalue)
            {
                if(boardcoins[lastcoinplacedX,lastcoinplacedY-2] == coinvalue)
                {
                    if(lastcoinplacedY+1 < noOfCol)
                    {                    
                    cpumove[0]=lastcoinplacedX;                                      
                    defencemode = true;   
                    }            

                    if(lastcoinplacedY>=3)
                    {
                        if(boardcoins[lastcoinplacedX,lastcoinplacedY-3] == coinvalue)
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
            if(boardcoins[lastcoinplacedX+1,lastcoinplacedY-1]== coinvalue)
            {
                if(boardcoins[lastcoinplacedX+2,lastcoinplacedY-2]== coinvalue)
                {
                    if(boardcoins[lastcoinplacedX+3,lastcoinplacedY-3]== coinvalue)
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
            if(boardcoins[lastcoinplacedX-1,lastcoinplacedY-1]== coinvalue)
            {
                if(boardcoins[lastcoinplacedX-2,lastcoinplacedY-2]== coinvalue)
                {
                    if(boardcoins[lastcoinplacedX-3,lastcoinplacedY-3]== coinvalue)
                    {
                        Debug.Log("player"+coinvalue+" win");
                            Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX-3,lastcoinplacedY-3],piece[lastcoinplacedX-2,lastcoinplacedY-2],piece[lastcoinplacedX-1,lastcoinplacedY-1]);
                    }
                }
            }
        }


        //check in betweens of the 4COINS in a  horizontal (1,0,1,1) PATTERN
        if(lastcoinplacedX>0 && lastcoinplacedX < noOfRow-3)
        {
            if(boardcoins[lastcoinplacedX-1,lastcoinplacedY] == coinvalue && boardcoins[lastcoinplacedX+1,lastcoinplacedY] == coinvalue && boardcoins[lastcoinplacedX+2,lastcoinplacedY] == coinvalue )
            {
             Debug.Log("player"+coinvalue+" win");
             Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX-1,lastcoinplacedY],piece[lastcoinplacedX+1,lastcoinplacedY],piece[lastcoinplacedX+2,lastcoinplacedY]);
            }
        }

        //check in betweens of the 4COINS in a  horizontal (1,1,0,1) PATTERN
        if(lastcoinplacedX>2 && lastcoinplacedX < noOfRow-1)
        {
            if(boardcoins[lastcoinplacedX-1,lastcoinplacedY] == coinvalue && boardcoins[lastcoinplacedX-2,lastcoinplacedY] == coinvalue && boardcoins[lastcoinplacedX+1,lastcoinplacedY] == coinvalue )
            {
             Debug.Log("player"+coinvalue+" win");
             Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX-1,lastcoinplacedY],piece[lastcoinplacedX-2,lastcoinplacedY],piece[lastcoinplacedX+1,lastcoinplacedY]);
            }
        }


        //check diadgonals right up for (1,1,0,1) PATTERN
        if((lastcoinplacedX>0 && lastcoinplacedY>0) && lastcoinplacedX < noOfRow-2 && lastcoinplacedY < noOfCol-2)
        {
            if(boardcoins[lastcoinplacedX-1,lastcoinplacedY-1] == coinvalue && boardcoins[lastcoinplacedX+1,lastcoinplacedY+1] == coinvalue && boardcoins[lastcoinplacedX+2,lastcoinplacedY+2] == coinvalue)
            {
                 Debug.Log("player"+coinvalue+" win");
             Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX-1,lastcoinplacedY-1],piece[lastcoinplacedX+1,lastcoinplacedY+1],piece[lastcoinplacedX+2,lastcoinplacedY+2]);
            }
        }

        //check diadgonals right up for (1,0,1,1) PATTERN
        if((lastcoinplacedX>1 && lastcoinplacedY>1) && (lastcoinplacedX < noOfRow-1 && lastcoinplacedY < noOfCol-1))
        {
            if(boardcoins[lastcoinplacedX-1,lastcoinplacedY-1] == coinvalue && boardcoins[lastcoinplacedX-2,lastcoinplacedY-2] == coinvalue && boardcoins[lastcoinplacedX+1,lastcoinplacedY+1] == coinvalue )
            {
                 Debug.Log("player"+coinvalue+" win");
             Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX-2,lastcoinplacedY-2],piece[lastcoinplacedX-1,lastcoinplacedY-1],piece[lastcoinplacedX+1,lastcoinplacedY+1]);
            }
        }

         //check diadgonals left up for (1,1,0,1) PATTERN
        if((lastcoinplacedX < noOfRow-1 && lastcoinplacedY > 0) && (lastcoinplacedX < 1 && lastcoinplacedY < noOfCol-2))
        {
            if(boardcoins[lastcoinplacedX+1,lastcoinplacedY-1] == coinvalue && boardcoins[lastcoinplacedX-1,lastcoinplacedY+1] == coinvalue && boardcoins[lastcoinplacedX-2,lastcoinplacedY+2] == coinvalue )
            {
                 Debug.Log("player"+coinvalue+" win");
             Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX+1,lastcoinplacedY-1],piece[lastcoinplacedX-1,lastcoinplacedY+1],piece[lastcoinplacedX-2,lastcoinplacedY+2]);
            }
        }

         //check diadgonals left up for (1,0,1,1) PATTERN
        if((lastcoinplacedX < noOfRow-2 && lastcoinplacedY > 1) && (lastcoinplacedX > 0 && lastcoinplacedY < noOfCol-1))
        {
            if(boardcoins[lastcoinplacedX+1,lastcoinplacedY-1] == coinvalue && boardcoins[lastcoinplacedX-1,lastcoinplacedY+1] == coinvalue && boardcoins[lastcoinplacedX+2,lastcoinplacedY-2] == coinvalue )
            {
                 Debug.Log("player"+coinvalue+" win");
             Higlight(piece[lastcoinplacedX,lastcoinplacedY],piece[lastcoinplacedX+1,lastcoinplacedY-1],piece[lastcoinplacedX-1,lastcoinplacedY+1],piece[lastcoinplacedX+2,lastcoinplacedY-2]);
            }
        }

        
        
        
        
        
        
        if(!gameover) 
        {
        // check for other possible places for defence
        //checkpossiblemoves("defence"); 

        // change the player turn if no one is winning
        changeturns();   
        }       
        


    }

    //instance of new game
    public void Restartgame()
    {  
       //SceneManager.LoadScene("Game");
       gameover = false; 
       
       Canvas.GetComponent<GraphicRaycaster>().enabled = true;      
       cpuwin.SetActive(false);
       playerwin.SetActive(false);
       Gamewinningpanel.SetActive(false);

       regenarateBoard(noOfRow,noOfCol); 

       changeturns();
       
    }

    //after game over
    private void Resetgame()
    {  
       
       Canvas.GetComponent<GraphicRaycaster>().enabled = false;
       //transform.position = transform.position + new Vector3(0,0,50f);
       Gamewinningpanel.SetActive(true);
       if(playerturn)
        {          
        playerwin.SetActive(true);
        playerscore+= 1;
        }
        else
        {
        cpuwin.SetActive(true);
        cpuscore+= 1;
        }
       
    }


    //highlights the winning coin and disables input
    private void Higlight(GameObject piece1,GameObject piece2,GameObject piece3,GameObject piece4)
    {   
        gameover = true;
        piece1.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;
        piece2.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;
        piece3.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;
        piece4.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;
        
        if(playerturn)
        {  
        playerscore+= 1;
        playerscoretext.text = playerscore.ToString();
        }
        else
        {
        cpuscore+= 1;
        cpuscoretext.text = cpuscore.ToString();
        }

        Invoke("Resetgame",1.5f);
    }

    //easy mode (radndomly drops the coin)
    private void Cpueasymove()
    {
        
        if(cpufirstmove) //if cpu plays first
        {  
            Debug.Log("cpu easy move1"+cpufirstmove); 
            cpufirstmove = false;
            

            spawnplayercoin((int)noOfRow/2, 0);         
        }
        else
        {
            Debug.Log("cpu easy move");
            spawnplayercoin( columnstofill[Random.Range(0, columnstofill.Count)], 0);
        }
        
    }

    //defencing mode
    private void Cpudefencemove()
    {          
            spawnplayercoin(cpumove[0], 0);
            defencemode = false;
            Debug.Log("cpu defence move");
       
    }

    private void Cpuoffencemove()
    {
        spawnplayercoin(cpumove[1], 0);
        offencemode = false;
         Debug.Log("cpu offence move");       
    }


    //changes the player and cup turn
    private void changeturns()
    {
        if(playerturn)
        {          
        playerturn = false;
        }
        else
        {
        playerturn = true; 
        }
    }


    //defencive and offencice mode calcuclation

    private void checkpossiblemoves(string type)
    {

        int coinvalue=0;
        if(type == "defence")
        {
            coinvalue=1;
            
        }
        else if(type == "offence")
        {
            coinvalue=2;
        }

        //offence move to place coins adjacent to each other wheen in patetren 0,1,1,0,0 or 0,0,1,1,0,0
        for(int j=0; j<=xcordinatesfilled.Max(); j++)
        {
            for(int i=0; i<noOfRow-4;i++)
            {   
                //pattern 0,2,0,0,0
                if(boardcoins[i,j] == 0 &&  boardcoins[i+1,j] == coinvalue && boardcoins[i+2,j] == 0 && boardcoins[i+3,j] == 0 && boardcoins[i+4,j] == 0)
                {    
                    if(j!=0)
                    {
                        if( boardcoins[i,j-1] != 0 &&  boardcoins[i+2,j-1] != 0 &&  boardcoins[i+3,j-1] != 0 &&  boardcoins[i+4,j-1] != 0 )
                        {
                            cpumove[coinvalue-1] = i+2;
                            if(type == "defence")
                            defencemode =true;
                            else offencemode =true;
                            
                            Debug.Log("to place 2 coins adjacent");
                            break;
                        }
                    }  
                    else
                    {
                            cpumove[coinvalue-1] = i+2;
                            if(type == "defence")
                            defencemode =true;
                            else offencemode =true;
                            Debug.Log("to place 2 coins adjacent");
                            break;    
                    }   
                    
                }

                //pattern 0,0,2,0,0
                if(boardcoins[i,j] == 0 &&  boardcoins[i+1,j] == 0 && boardcoins[i+2,j] == coinvalue && boardcoins[i+3,j] == 0 && boardcoins[i+4,j] == 0)
                {    
                    if(j!=0)
                    {
                        if( boardcoins[i,j-1] != 0 &&  boardcoins[i+1,j-1] != 0 &&  boardcoins[i+3,j-1] != 0 &&  boardcoins[i+4,j-1] != 0 )
                        {
                            cpumove[coinvalue-1] = i+1;
                            if(type == "defence")
                            defencemode =true;
                            else offencemode =true;
                            Debug.Log("to place 2 coins adjacent");
                            break;
                        }
                    }  
                    else
                    {
                            cpumove[coinvalue-1] = i+1;
                            if(type == "defence")
                            defencemode =true;
                            else offencemode =true;
                            Debug.Log("to place 2 coins adjacent");
                            break;    
                    }   
                    
                }

                //pattern 0,0,0,2,0
                if(boardcoins[i,j] == 0 &&  boardcoins[i+1,j] == 0 && boardcoins[i+2,j] == 0 && boardcoins[i+3,j] == coinvalue && boardcoins[i+4,j] == 0)
                {    
                    if(j!=0)
                    {
                       if( boardcoins[i,j-1] != 0 &&  boardcoins[i+1,j-1] != 0 &&  boardcoins[i+2,j-1] != 0 &&  boardcoins[i+4,j-1] != 0 )
                        {
                            cpumove[coinvalue-1] = i+2;
                            if(type == "defence")
                            defencemode =true;
                            else offencemode =true;
                            Debug.Log("to place 2 coins adjacent");
                            break;
                        }
                    }  
                    else
                    {
                            cpumove[coinvalue-1] = i+2;
                            if(type == "defence")
                            defencemode =true;
                            else offencemode =true;
                            Debug.Log("to place 2 coins adjacent");
                            break;    
                    }   
                    
                }

            }
        }
       

        //to prevent 3rd coin to place adjacent to eachother 
        for(int j=0; j<=xcordinatesfilled.Max(); j++)
        {
            for(int i=0; i<noOfRow-4;i++)
            {
                //pattern 0,1,1,0,0
                if(boardcoins[i,j] == 0 &&  boardcoins[i+1,j] == coinvalue && boardcoins[i+2,j] == coinvalue && boardcoins[i+3,j] == 0 && boardcoins[i+4,j] == 0)
                {    
                    if(j!=0)
                    {
                        if( boardcoins[i,j-1] != 0 && boardcoins[i+3,j-1] != 0 && boardcoins[i+4,j-1] != 0 )
                        {
                            cpumove[coinvalue-1] = i+3;
                            if(type == "defence")
                            defencemode =true;
                            else offencemode =true;
                            Debug.Log("to prevent 3rd coin to place adjacent to eachother");
                            break;
                        }
                    }
                    else
                    {
                        cpumove[coinvalue-1] = i+3;
                        if(type == "defence")
                        defencemode =true;
                        else offencemode =true;
                        Debug.Log("to prevent 3rd coin to place adjacent to eachother");
                        break;
                    }    
                    
                }

                //pattern 0,0,1,1,0
                if(boardcoins[i,j] == 0 && boardcoins[i+1,j] == 0 && boardcoins[i+2,j] == coinvalue && boardcoins[i+3,j] == coinvalue && boardcoins[i+4,j] == 0)
                {               
                    if(j!=0)
                    {
                        if( boardcoins[i,j-1] != 0 && boardcoins[i+2,j-1] != 0 && boardcoins[i+4,j-1] != 0 )
                        {
                            cpumove[coinvalue-1] = i+1;
                            if(type == "defence")
                            defencemode =true;
                            else offencemode =true;
                            Debug.Log("to prevent 3rd coin to place adjacent to eachother");
                            break;
                         }
                    }
                    else
                    {
                        cpumove[coinvalue-1] = i+1;
                        if(type == "defence")
                        defencemode =true;
                        else offencemode =true;
                        Debug.Log("to prevent 3rd coin to place adjacent to eachother");
                        break;
                    } 
                          
                }
            }
        }
        

        //to check the coins pile up in y axis        
        for(int i=0; i<noOfRow; i++)
        {
            for(int j=0; j< noOfCol-3; j++)
            {
                if(boardcoins[i,j] == coinvalue && boardcoins[i,j+1] == coinvalue && boardcoins[i,j+2] == coinvalue && boardcoins[i,j+3] == 0)
                {
                  cpumove[coinvalue-1] = i;
                  if(type == "defence")
                  defencemode =true;
                  else offencemode =true;
                  Debug.Log("to check the coins pile up in y axis");
                  break;
                }
            }
        }
        

        //check horizontal fill position (1,0,1,1) or (1,1,0,1) or (0,1,1,1) or (1,1,1,0)       
        for(int j=0; j<=xcordinatesfilled.Max(); j++)
        {
            for(int i=0; i<noOfRow-3;i++)
            {
              if(boardcoins[i,j] == coinvalue && boardcoins[i+1,j] == coinvalue && boardcoins[i+2,j] == 0 && boardcoins[i+3,j] == coinvalue)
              {    
                  if(j!=0)
                  {
                    if(boardcoins[i+2,j-1] != 0)   
                    {        
                        cpumove[coinvalue-1] = i+2;
                        if(type == "defence")
                        defencemode =true;
                        else offencemode =true;
                        Debug.Log("check horizontal fill position (1,1,0,1)");
                        break; 
                    }
                  }
                  else
                  {
                    cpumove[coinvalue-1] = i+2;
                    if(type == "defence")
                    defencemode =true;
                    else offencemode =true;
                    Debug.Log("check horizontal fill position (1,1,0,1)");
                    break;
                  }
              }

              if(boardcoins[i,j] == coinvalue && boardcoins[i+1,j] == 0 && boardcoins[i+2,j] == coinvalue && boardcoins[i+3,j] == coinvalue)
              {
                  if(j!=0)
                  {
                    if(boardcoins[i+1,j-1] != 0)   
                    {        
                        cpumove[coinvalue-1] = i+1;
                        if(type == "defence")
                        defencemode =true;
                        else offencemode =true;
                        Debug.Log("check horizontal fill position (1,0,1,1)");
                        break; 
                    }
                  }
                  else
                  {
                    cpumove[coinvalue-1] = i+1;
                    if(type == "defence")
                    defencemode =true;
                    else offencemode =true;
                    Debug.Log("check horizontal fill position (1,0,1,1)");
                    break;
                  }
              }

              if(boardcoins[i,j] == 0 && boardcoins[i+1,j] == coinvalue && boardcoins[i+2,j] == coinvalue && boardcoins[i+3,j] == coinvalue)
              {
                  if(j!=0)
                  {
                    if(boardcoins[i,j-1] != 0)   
                    {        
                        cpumove[coinvalue-1] = i;
                        if(type == "defence")
                        defencemode =true;
                        else offencemode =true;
                        Debug.Log("check horizontal fill position (0,1,1,1)");
                        break; 
                    }
                  }
                  else
                  {
                    cpumove[coinvalue-1] = i;
                    if(type == "defence")
                    defencemode =true;
                    else offencemode =true;
                    Debug.Log("check horizontal fill position (0,1,1,1)");
                    break;
                  }
              }

              if(boardcoins[i,j] == coinvalue && boardcoins[i+1,j] == coinvalue && boardcoins[i+2,j] == coinvalue && boardcoins[i+3,j] == 0)
              {
                  if(j!=0)
                  {
                    if(boardcoins[i+3,j-1] != 0)   
                    {        
                        cpumove[coinvalue-1] = i+3;
                        if(type == "defence")
                        defencemode =true;
                        else offencemode =true;
                        Debug.Log("check horizontal fill position (1,1,1,0)");
                        break; 
                    }
                  }
                  else
                  {
                    cpumove[coinvalue-1] = i+3;
                    if(type == "defence")
                    defencemode =true;
                    else offencemode =true;
                    Debug.Log("check horizontal fill position (1,1,1,0)");
                    break;
                  }
              }

            }
        }


        //check diagonal rightsideup  for (1,0,1,1) or (1,1,0,1)         
        for(int j=0; j<=noOfCol-3; j++)
        {
            for(int i=0; i<noOfRow-3;i++)
            {
              if(boardcoins[i,j] == coinvalue && boardcoins[i+1,j+1] == coinvalue && boardcoins[i+2,j+2] == 0 && boardcoins[i+3,j+3] == coinvalue)
              {
                  if(boardcoins[i+2,j+1] != 0)
                  {
                    cpumove[coinvalue-1] = i+2;
                    if(type == "defence")
                    defencemode =true;
                    else offencemode =true;
                    Debug.Log("check diagonal rightsideup  for (1,1,0,1)   ");
                    break; 
                  }
              }

              if(boardcoins[i,j] == coinvalue && boardcoins[i+1,j+1] == 0 && boardcoins[i+2,j+2] == coinvalue && boardcoins[i+3,j+3] == coinvalue)
              {
                  if(boardcoins[i+1,j] != 0)
                  {
                    cpumove[coinvalue-1] = i+1;
                    if(type == "defence")
                    defencemode =true;
                    else offencemode =true;
                    Debug.Log("check diagonal rightsideup  for (1,0,1,1)");
                    break;
                  }
              }

            }
        }

        //check diagonal rightsideup  for (1,1,1,0)          
        for(int j=0; j<=noOfCol-3; j++)
        {
            for(int i=0; i<noOfRow-3;i++)
            {
              if(boardcoins[i,j] == coinvalue && boardcoins[i+1,j+1] == coinvalue && boardcoins[i+2,j+2] == coinvalue && boardcoins[i+3,j+3] == 0)
              {
                   if(boardcoins[i+3,j+2] != 0) 
                   {
                    cpumove[coinvalue-1] = i+3;
                    if(type == "defence")
                    defencemode =true;
                    else offencemode =true;
                    Debug.Log("check diagonal rightsideup  for (1,1,1,0)");
                    break; 
                   }
              }
            }
        }

        //check diagonal leftsideup  for (1,0,1,1) or (1,1,0,1)         
        for(int j=0; j<=noOfCol-3; j++)
        {
            for(int i=noOfRow-1; i>3; i--)
            {
              if(boardcoins[i,j] == coinvalue && boardcoins[i-1,j+1] == coinvalue && boardcoins[i-2,j+2] == 0 && boardcoins[i-3,j+3] == coinvalue)
              {
                  if(boardcoins[i-2,j+1] != 0)
                  {
                    cpumove[coinvalue-1] = i+2;
                    if(type == "defence")
                    defencemode =true;
                    else offencemode =true;
                    Debug.Log("check diagonal rightsideup  for (1,0,1,1) or (1,1,0,1)   ");
                    break; 
                  }
              }
              if(boardcoins[i,j] == coinvalue && boardcoins[i-1,j+1] == 0 && boardcoins[i-2,j+2] == coinvalue && boardcoins[i-3,j+3] == coinvalue)
              {
                  if(boardcoins[i-1,j] != 0)
                  {
                    cpumove[coinvalue-1] = i+1;
                    if(type == "defence")
                    defencemode =true;
                    else offencemode =true;
                    Debug.Log("check diagonal rightsideup  for (1,0,1,1) or (1,1,0,1)");
                    break;
                  }
              }

            }
        }

        ////check diagonal leftsideup  for (0,1,1,1)        
        for(int j=0; j<=noOfCol-3; j++)
        {
            for(int i=noOfRow-1; i>3; i--)
            {
              if(boardcoins[i,j] == coinvalue && boardcoins[i-1,j+1] == coinvalue && boardcoins[i-2,j+2] == coinvalue && boardcoins[i-3,j+3] == 0)
              {
                  if(boardcoins[i-3,j+2] != 0)
                  {
                    cpumove[coinvalue-1] = i-3;
                    if(type == "defence")
                    defencemode =true;
                    else offencemode =true;
                    Debug.Log("check diagonal leftsideup  for (0,1,1,1)  ");
                    break; 
                  }
              } 
            }
        }


        
        

    }






   

    


    
}
