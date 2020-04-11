using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
     string rowsstring;
     string columnsstring;
     public int rows;
     public int columns;

    //public InputField rowfield;
    //public InputField columnfield;

    public Text rowfield;
    public Text columnfield;
    


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void Gotogame()
    {
        rowsstring = rowfield.text;
        rows =int.Parse(rowsstring);
        columnsstring = columnfield.text;
        columns =int.Parse(columnsstring);
    SceneManager.LoadScene("Game");
    }

    public void Quitgame()
    {
        Application.Quit();
    }

}
