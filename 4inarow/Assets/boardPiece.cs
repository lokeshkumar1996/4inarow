using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardPiece : MonoBehaviour
{
    public int xCord;
    public int yCord;
    public int coin =0;
   
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

    void OnMouseOver()
    {
        
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();//SpriteRenderer renderer = GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
        renderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
    }
     void OnMouseExit()
    {
         SpriteRenderer renderer = GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
        renderer.color = new Color(1f, 1f, 1f, 1f);
    }
    
    


}
