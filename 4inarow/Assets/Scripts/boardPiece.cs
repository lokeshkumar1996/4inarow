using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardPiece : MonoBehaviour
{
    public int xCord;
    public int yCord; 
    
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
