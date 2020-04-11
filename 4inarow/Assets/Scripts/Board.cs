using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;


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
    bool playerwon = false;

    
    int noInRow ; // pieces in x cordinates , min row 4
    int noInCol ; // pieces in y cordinates, min colum 4    
    
    public GameObject[,] piece; 
    
    
     int lastcoinplacedX ;
     int lastcoinplacedY ;
    

    private bool playerturn = true;
    bool gameover = false;
    bool playerstartsgame = true;

     int[] xcordinatesfilled; //no of coins filled in easch posistion of xcordinate
    
     int[,] boardcoins; // array of coins placed in the board


    //botvariables
     int[] cpumove = new int[2];  // index 0 is for defence position of X and index 1 is for offence position of X 
     bool defencemode =false;
     bool offencemode =false;
     List<int> columnstofill = new List<int>();
     List<int> columnoutofbound = new List<int>();
     bool cpufirstmove = false;


   
    // Start is called before the first frame update
    void Start()
    {
        instance =this;
            
        
        noInCol = GameObject.Find("Rowcolinfo").GetComponent<SceneChange>().columns;
        noInRow = GameObject.Find("Rowcolinfo").GetComponent<SceneChange>().rows;
        
        //read textfiel and get the row and column value
        //readdatafromtext();
        //readdatafromjson();

        genarateBoard(noInRow,noInCol);

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
        //cpu turn
        if(!playerturn) 
        { 
            //clear the list for no of collums
            columnstofill.Clear();

            //reinitialize the list for no of colums
            for(int i=0; i<noInRow; i++)
            { columnstofill.Add(i); } 


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

    /* streaming asset folder isnot found in android so commenting it out and using text file
    private void readdatafromjson()
    {

        Dictionary<string, string> Data = new Dictionary<string, string> ();

            string fileName = "rowandcol.json";
            string filePath = Path.Combine (Application.streamingAssetsPath, fileName);

         
         
            string dataAsJson = File.ReadAllText (filePath);
            rowcoldata loadedData = JsonUtility.FromJson<rowcoldata> (dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++) 
            {
                Data.Add (loadedData.items [i].key, loadedData.items [i].value);    
            }

            Debug.Log ("Data loaded, dictionary contains: " + Data.Count + " entries");
            Debug.Log(Data ["Row"]);
            Debug.Log(Data ["Column"]);

            noInRow = int.Parse(Data ["Column"]);
            noInCol = int.Parse(Data ["Row"]);

    }*/

    private void readdatafromtext()
    {
        //read no of row and col from the text file in resource
        TextAsset txt = (TextAsset)Resources.Load("rowandcol", typeof(TextAsset));
        string content = txt.text;        
        string[] Data = content.Split(' ');
        string[] row = Data[0].Split('=');
        string[] column = Data[1].Split('=');

        

        //assign value to the row and col
        noInRow = int.Parse(column[1]);   // noInRow is the number of elements in the row
        noInCol = int.Parse(row[1]);   // noInCol is the number of elements in the collum
    }

    //create a gameboard and adjust the camera accrouding to the size
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

        //adjust camera to set it center of the board
        maincamera.transform.position = new Vector3(maincameraXcord/2 * 10F, maincameraYcord/2 * 10F +maincameraYcord, -10f);
        Camera.main.orthographicSize = noInRow*10f +noInRow;
    }

    //when restarting game delete all previous game data
    private void regenarateBoard(int x, int y)
    {    
        // reset xcordinatesfilled to 0
        for(int i =0; i<noInRow; i++)
            {
                xcordinatesfilled[i] = 0;
            }

        //reset all  boardcoins to 0 and and destory coins
        for(int j =0; j<noInCol; j++)
        {
            for(int i =0; i<noInRow; i++)
            {
                boardcoins[i,j] = 0;
                if(piece[i,j].transform.childCount > 0 )
                Destroy(piece[i,j].transform.GetChild(0).gameObject);
            }
        } 

        //regernearte the colunmsfill list
        columnstofill.Clear();
        for(int i=0; i<noInRow; i++)
        {
            columnstofill.Add(i);
        }

        //clear the outof bound column list
        columnoutofbound.Clear();

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

    //spawing the player and cpu coins
    public void spawnplayercoin(int clickedpieceX, int clickedpieceY )
    {
       if(xcordinatesfilled[clickedpieceX] < noInCol)
       {
           if(playerturn)
            {
                GameObject coin = Instantiate(yellopiece, new Vector3(clickedpieceX * 10F, (noInCol-1)*10f, 0f), Quaternion.identity);

                coin.transform.parent = piece[clickedpieceX,xcordinatesfilled[clickedpieceX]].transform;        
                boardcoins[clickedpieceX,xcordinatesfilled[clickedpieceX]] = 1;
                
                Vector3 target = new Vector3(clickedpieceX * 10F, xcordinatesfilled[clickedpieceX]*10f, 0f);                 
                StartCoroutine(droppiece(target,coin,0f));
                StopCoroutine("droppiece");                
                

                lastcoinplacedX=clickedpieceX;
                lastcoinplacedY=xcordinatesfilled[clickedpieceX];

                    xcordinatesfilled[clickedpieceX] += 1;
                   
                    if(xcordinatesfilled[clickedpieceX] >= noInCol)
                    {
                        columnoutofbound.Add(clickedpieceX);
                    }
            }
            else
            {
               
                GameObject coin = Instantiate(redpiece, new Vector3(clickedpieceX * 10F, (noInCol-1)*10f, 0f), Quaternion.identity);
                coin.transform.parent = piece[clickedpieceX,xcordinatesfilled[clickedpieceX]].transform;        
                boardcoins[clickedpieceX,xcordinatesfilled[clickedpieceX]] = 2;

                Vector3 target = new Vector3(clickedpieceX * 10F, xcordinatesfilled[clickedpieceX]*10f, 0f);                 
                StartCoroutine(droppiece(target,coin, 1f));
                StopCoroutine("droppiece");                

                lastcoinplacedX=clickedpieceX;
                lastcoinplacedY=xcordinatesfilled[clickedpieceX];
   
                    xcordinatesfilled[clickedpieceX] += 1;
                           
                    if(xcordinatesfilled[clickedpieceX] >= noInCol)
                    {
                        columnoutofbound.Add(clickedpieceX);
                    }
            }

            //ifnot gameover check who iswinning    
            if(!gameover)
            checkwinning(playerturn);
       }
      
       
    }

    //when a coin is dropped it check the all possstion to determind if the player wins or not
    private void checkwinning(bool player)
    {    

        int coinvalue;
         //CHECK RED OR YELLOW COIN
        if(player)
        coinvalue =1;
        else coinvalue =2;

        
        //horizontal check
        for(int j=0; j<noInCol; j++)
        {
            for(int i=0; i<noInRow-3;i++)
            {
              if(boardcoins[i,j] == coinvalue && boardcoins[i+1,j] == coinvalue && boardcoins[i+2,j] == coinvalue && boardcoins[i+3,j] == coinvalue)
              {    
                 Higlight(piece[i,j], piece[i+1,j], piece[i+2,j], piece[i+3,j]);
                  break;
              }
            }
        }

        //diagonal rightsideup check
        for(int j=0; j<noInCol-3; j++)
        {
            for(int i=0; i<noInRow-3;i++)
            {
              if(boardcoins[i,j] == coinvalue && boardcoins[i+1,j+1] == coinvalue && boardcoins[i+2,j+2] == coinvalue && boardcoins[i+3,j+3] == coinvalue)
              {
                    Higlight(piece[i,j], piece[i+1,j+1], piece[i+2,j+2], piece[i+3,j+3]);
                    break; 
                   
              }
            }
        }

        //diagonal leftsideup check
        for(int j=0; j<noInCol-3; j++)
        {
            for(int i=noInRow-1; i>3; i--)
            {
              if(boardcoins[i,j] == coinvalue && boardcoins[i-1,j+1] == coinvalue && boardcoins[i-2,j+2] == coinvalue && boardcoins[i-3,j+3] == coinvalue)
              {
                  Higlight(piece[i,j], piece[i-1,j+1], piece[i-2,j+2], piece[i-3,j+3]);
                break; 
              } 
            }
        }

        //to check the coins pile up in y axis        
        for(int i=0; i<noInRow; i++)
        {
            for(int j=0; j< noInCol-3; j++)
            {
                if(boardcoins[i,j] == coinvalue && boardcoins[i,j+1] == coinvalue && boardcoins[i,j+2] == coinvalue && boardcoins[i,j+3] == coinvalue)
                {
                    Higlight(piece[i,j], piece[i,j+1], piece[i,j+2], piece[i,j+3]);
                  break;
                }
            }
        }

        // change the player turn if no one is winning
        if(!gameover) 
        { 
        changeturns();   
        } 
       
    }

    //instance of new game
    public void Restartgame()
    {        
       gameover = false; 
       
       Canvas.GetComponent<GraphicRaycaster>().enabled = true;      
       cpuwin.SetActive(false);
       playerwin.SetActive(false);
       Gamewinningpanel.SetActive(false);

       regenarateBoard(noInRow,noInCol); 

      // changeturns();       
    }

    //after game over
    private void Resetgame()
    {  
       
       Canvas.GetComponent<GraphicRaycaster>().enabled = false;       
       Gamewinningpanel.SetActive(true);

       if(playerwon)
        {          
        playerwin.SetActive(true); 
        cpuwin.SetActive(false);      
        }
        else
        {
        cpuwin.SetActive(true);
        playerwin.SetActive(false);        
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
        playerwon = true;
        }
        else
        {
        cpuscore+= 1;
        cpuscoretext.text = cpuscore.ToString();
        playerwon =false;
        }

        Invoke("Resetgame",1.5f);
    }

    //easy mode (radndomly drops the coin)
    private void Cpueasymove()
    {
        
        if(cpufirstmove) //if cpu plays first
        {  
            cpufirstmove = false;
            spawnplayercoin((int)noInRow/2, 0);         
        }
        else
        {
            for(int i=0; i<columnoutofbound.Count; i++)
            {   
                try{columnstofill.Remove(columnoutofbound[i]);}
                catch{}
            }

            Debug.Log("cpu easy move");
            spawnplayercoin( columnstofill[Random.Range(0, columnstofill.Count)], 0);
        }
        
    }

    //cpu plays to defend player
    private void Cpudefencemove()
    {          
            spawnplayercoin(cpumove[0], 0);
            defencemode = false;
            Debug.Log("cpu defence move");
    }

    //cpu plays to win
    private void Cpuoffencemove()
    {
        spawnplayercoin(cpumove[1], 0);
        offencemode = false;
        Debug.Log("cpu offence move");       
    }


    //changes the player and CPU turn
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
        for(int j=0; j<noInCol; j++)
        {
            for(int i=0; i<noInRow-4;i++)
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
        for(int j=0; j<noInCol; j++)
        {
            for(int i=0; i<noInRow-4;i++)
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
        for(int i=0; i<noInRow; i++)
        {
            for(int j=0; j< noInCol-3; j++)
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
        for(int j=0; j<noInCol; j++)
        {
            for(int i=0; i<noInRow-3;i++)
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
                    else{ columnstofill.Remove(i+2);}
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
                    else{ columnstofill.Remove(i+1);}
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
                    else{ columnstofill.Remove(i);}
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
                    else{columnstofill.Remove(i+3);}
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
        for(int j=0; j<noInCol-3; j++)
        {
            for(int i=0; i<noInRow-3;i++)
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
        for(int j=0; j<noInCol-3; j++)
        {
            for(int i=0; i<noInRow-3;i++)
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
        for(int j=0; j<noInCol-3; j++)
        {
            for(int i=noInRow-1; i>3; i--)
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
        for(int j=0; j<noInCol-3; j++)
        {
            for(int i=noInRow-1; i>3; i--)
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

    public void quitgame()
    {
        if(GameObject.Find("Rowcolinfo") != null)
        {
            Destroy(GameObject.Find("Rowcolinfo"));
        }
        SceneManager.LoadScene("Mainmenu");

    }

    private IEnumerator droppiece(Vector3 target, GameObject  piece, float time)
    {
        //Debug.Log("coroutineentered");
        if(time > 0f)
        yield return new WaitForSeconds(.2f);  

        while(Vector3.Distance(piece.transform.position, target) > 100f)
        {
            piece.transform.position = Vector3.MoveTowards(piece.transform.position, target, Time.deltaTime*1000);
            //Debug.Log("discance=" + Vector3.Distance(piece.transform.position, target));

            yield return null;
           
        }
         yield return new WaitForSeconds(.5f);  
    }

    


    
}
