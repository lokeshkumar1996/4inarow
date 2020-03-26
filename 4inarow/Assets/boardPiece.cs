using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardPiece : MonoBehaviour
{
    public int xCord;
    public int yCord;    
   
   // private SpriteRenderer myRenderer;  // 0 no coind, 1 player coind, 2 cpu coin
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {  

           
    }

   /* void OnMouseDown()
    {
        //Board.instance.clickedpieceX = xCord;
        //Board.instance.clickedpieceY = yCord;
        Board.instance.spawnplayercoin(xCord,yCord);
    }*/

    public void piececlicked()
    {
        Board.instance.spawnplayercoin(xCord,yCord);
    }

}
