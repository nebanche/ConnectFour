using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using C4_AI;

public class GameController : MonoBehaviour {

    static public GameController S;
    enum Piece {
        Empty = 0,
        Blue = 1,
        Red = 2
    }

    // Game Variables

    static public int numRows = 6;

    static public int numColumns = 7;

    public int numPiecesToWin = 4;

    public float dropTime = 4f;

    // Gameobjects that are public for unity editor reference
    [SerializeField]
    private GameObject pieceRed;
    [SerializeField]
    private GameObject pieceBlue;
    [SerializeField]
    private GameObject pieceField;

    [SerializeField]
    private GameObject commandLineUI;

    public GameObject winningText;
    public string playerOneText = "Red Wins!";
    public string playerTwoText = "Yellow Wins!";
    public string drawText = "Draw!";

    public GameObject btnPlayAgain;
    bool btnPlayAgainTouching = false;
    Color btnPlayAgainOrigColor;
    Color btnPlayAgainHoverColor = new Color(255, 143, 4);

    GameObject gameObjectField;

    // temporary gameobject, holds the piece at mouse position until the mouse has clicked
    GameObject gameObjectTurn;

    /// <summary>
    /// The Game field.
    /// 0 = Empty
    /// 1 = Blue
    /// 2 = Red
    /// </summary>
    /// 
    Position gameBoard; 

    bool playerOneTurn = true;

    [SerializeField]
    public bool computerPlayer = false;

    public bool simpleOn = true;

    [SerializeField]
    GameObject simpleAI;

    [SerializeField]
    GameObject intermediateAI;


    bool isLoading = true;
    bool isDropping = false;
    bool mouseButtonPressed = false;

    bool gameOver = false;
    bool isCheckingForWinner = false;

    // Start runs on program intialization, spawns board. 
    void Start() {

        S = this;

        int max = Mathf.Max(numRows, numColumns);

        if (numPiecesToWin > max)
            numPiecesToWin = max;


        gameBoard = new Position();
        CreateBoard();

        playerOneTurn = true;

        btnPlayAgainOrigColor = btnPlayAgain.GetComponent<Renderer>().material.color;
    }


    /// Creates the game board
    void CreateBoard() {
        winningText.SetActive(false);
        btnPlayAgain.SetActive(false);

        isLoading = true;

        gameObjectField = GameObject.Find("Field");
        if (gameObjectField != null) {
            DestroyImmediate(gameObjectField);
        }
        gameObjectField = new GameObject("Field");

        //instantiate the cells on the board graphically
        for (int x = 0; x < numColumns; x++){
            for (int y = 0; y < numRows; y++){  
                GameObject g = Instantiate(pieceField, new Vector3(
                    x, y * -1, -1), Quaternion.identity) as GameObject;
                g.transform.parent = gameObjectField.transform;
            }
        }

        isLoading = false;
        gameOver = false;

        // center camera
        Camera.main.transform.position = new Vector3(
            (numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f), Camera.main.transform.position.z);

        winningText.transform.position = new Vector3(
            (numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f) + 1, winningText.transform.position.z);

        btnPlayAgain.transform.position = new Vector3(
            (numColumns - 1) / 2.0f, -((numRows - 1) / 2.0f) - 1, btnPlayAgain.transform.position.z);
    }


    /// Spawns a piece at mouse position above the first row
    GameObject SpawnPiece() {
        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (!playerOneTurn && computerPlayer) {
            //computer thinking code goes here 
            if (simpleOn) {
                int choice = simpleAI.GetComponent<SimpleAI>().CalcuateMove(gameBoard);
                spawnPos = new Vector3(choice, 0, 0);
            } else {
                int choice = intermediateAI.GetComponent<SimpleAI>().CalcuateMove(gameBoard);
                spawnPos = new Vector3(choice, 0, 0);
            }
        }

        GameObject g = Instantiate(
                playerOneTurn ? pieceBlue : pieceRed, // is players turn = spawn yellow, else spawn red
                new Vector3(
                Mathf.Clamp(spawnPos.x, 0, numColumns - 1),
                gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
                Quaternion.identity) as GameObject;

        return g;
    }

    void UpdatePlayAgainButton() {
        RaycastHit hit;
        //ray shooting out of the camera from where the mouse is
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.collider.name == btnPlayAgain.name) {
            btnPlayAgain.GetComponent<Renderer>().material.color = btnPlayAgainHoverColor;
            //check if the left mouse has been pressed down this frame
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && btnPlayAgainTouching == false) {
                btnPlayAgainTouching = true;

                SceneManager.LoadScene(0);
             
            }
        } 
        
        else {
            btnPlayAgain.GetComponent<Renderer>().material.color = btnPlayAgainOrigColor;
        }

        if (Input.touchCount == 0) {
            btnPlayAgainTouching = false;
        }
    }

    // Update is called once per frame
    void Update() {

        if (isLoading)
            return;

        if (isCheckingForWinner)
            return;

        if (gameOver) {
            winningText.SetActive(true);
            btnPlayAgain.SetActive(true);

            UpdatePlayAgainButton();

            return;
        }

        if (playerOneTurn || !computerPlayer) {

            if (gameObjectTurn == null) {
                gameObjectTurn = SpawnPiece();
            } else {
                // update the hovering piece position
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                gameObjectTurn.transform.position = new Vector3(
                    Mathf.Clamp(pos.x, 0, numColumns - 1),
                    gameObjectField.transform.position.y + 1, 0);

                // click the left mouse button to drop the piece into the selected column
                if (Input.GetMouseButtonDown(0) && !mouseButtonPressed && !isDropping) {
                    mouseButtonPressed = true;

                    StartCoroutine(dropPiece(gameObjectTurn));
                } else {
                    mouseButtonPressed = false;
                }
            }
        }
        //computer player is on 
        else {
            //ensure peice is still above game board
            if (gameObjectTurn == null) {
                gameObjectTurn = SpawnPiece();
            } else {
                if (!isDropping) {
                    StartCoroutine(dropPiece(gameObjectTurn));
                }

            }
        }
    }

    /// This method  is a cocourtine and searches for a empty cell and lets 
    /// the object fall down into this cell
    /// prevents the player from dropping into the middle colmn
    IEnumerator dropPiece(GameObject gObject) {
        isDropping = true;

        Vector3 startPosition = gObject.transform.position;
        Vector3 endPosition = new Vector3();

        // round to a grid cell
        int x = Mathf.RoundToInt(startPosition.x);
        startPosition = new Vector3(x, startPosition.y, startPosition.z);

        // is there a free cell in the selected column?
        bool foundFreeCell = false;

        //plays the peice 
        endPosition = gameBoard.GamePlay(x, startPosition);

        if (endPosition != new Vector3(-1,-1,-1) ){
            // Instantiate a new Piece, disable the temporary floating piece
            GameObject g = Instantiate(gObject) as GameObject;
            gameObjectTurn.GetComponent<Renderer>().enabled = false;

            float distance = Vector3.Distance(startPosition, endPosition);

            float t = 0;
            while (t < 1) {
                t += Time.deltaTime * dropTime * ((numRows - distance) + 1);

                g.transform.position = Vector3.Lerp(startPosition, endPosition, t);
                yield return null;
            }

            g.transform.parent = gameObjectField.transform;

            // remove the temporary gameobject
            DestroyImmediate(gameObjectTurn);

            // run coroutine to check if someone has won
            StartCoroutine(Won());

            // wait until winning check is done
            while (isCheckingForWinner)
                yield return null;

            playerOneTurn = !playerOneTurn;
        }

        isDropping = false;
        print("turn end");
        gameBoard.DebugBoard();
        yield return 0;
    }

    /// Check for Winner
    IEnumerator Won() {
        isCheckingForWinner = true;

        if (gameBoard.WinCheck(1) || gameBoard.WinCheck(2)) {
            gameOver = true;
        }
        // if Game Over update the winning text to show who has won
        if (gameOver == true) {
            print(playerOneTurn);
            if (playerOneTurn) {
                winningText.GetComponent<TextMesh>().text = playerOneText;

            } else {
                winningText.GetComponent<TextMesh>().text = playerTwoText;
            }

        } else {
            // check if there are any empty cells left, if not set game over and update text to show a draw
            if (!gameBoard.BoardContainsEmptyCell()) {
                gameOver = true;
                winningText.GetComponent<TextMesh>().text = drawText;
            }
        }

        isCheckingForWinner = false;

        yield return 0;
    }
    //prints board given to the debug board
    

}