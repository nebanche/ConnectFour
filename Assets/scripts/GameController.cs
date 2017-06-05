using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    static public GameController S;
	enum Piece{
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
    private  GameObject pieceRed;
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
	Color btnPlayAgainHoverColor = new Color(255, 143,4);

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
    //Duplicated only for asethetics
	int[,] board;
    int[] height; 
 

	bool playerOneTurn = true;

    [SerializeField]
    public bool computerPlayer = false;


	bool isLoading = true;
	bool isDropping = false; 
	bool mouseButtonPressed = false;

    [SerializeField]
    GameObject simpleAI;

   static public bool commandLine = false;

	bool gameOver = false;
	bool isCheckingForWinner = false;

	// Start runs on program intialization, spawns board. 
	void Start () {

        S = this;

		int max = Mathf.Max (numRows, numColumns);

		if(numPiecesToWin > max)
			numPiecesToWin = max;

		CreateBoard ();

        playerOneTurn = true;

		btnPlayAgainOrigColor = btnPlayAgain.GetComponent<Renderer>().material.color;
	}

		
	/// Creates the game board
	void CreateBoard(){
		winningText.SetActive(false);
		btnPlayAgain.SetActive(false);

		isLoading = true;

		gameObjectField = GameObject.Find ("Field");
		if(gameObjectField != null){
			DestroyImmediate(gameObjectField);
		}
		gameObjectField = new GameObject("Field");

		// create an empty board and instantiate the cells
		board = new int[numColumns, numRows];
        height = new int[numColumns];
		for(int x = 0; x < numColumns; x++){
            height[x] = 6;
			for(int y = 0; y < numRows; y++){
				board[x, y] = (int)Piece.Empty;
				GameObject g = Instantiate(pieceField, new Vector3(
                    x, y * -1, -1), Quaternion.identity) as GameObject;
				g.transform.parent = gameObjectField.transform;
			}
		}

		isLoading = false;
		gameOver = false;

		// center camera
		Camera.main.transform.position = new Vector3(
			(numColumns-1) / 2.0f, -((numRows-1) / 2.0f), Camera.main.transform.position.z);

		winningText.transform.position = new Vector3(
			(numColumns-1) / 2.0f, -((numRows-1) / 2.0f) + 1, winningText.transform.position.z);

		btnPlayAgain.transform.position = new Vector3(
			(numColumns-1) / 2.0f, -((numRows-1) / 2.0f) - 1, btnPlayAgain.transform.position.z);
	}

		
	/// Spawns a piece at mouse position above the first row
	GameObject SpawnPiece(){
		Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					
		if(!playerOneTurn && computerPlayer){
            //computer thinking code goes here 
            if (simpleAI) {
                int move = simpleAI.GetComponent<SimpleAI>().CalcuateMove();
                spawnPos = new Vector3(move, 0, 0);
            }
		}

		GameObject g = Instantiate(
				playerOneTurn ? pieceBlue : pieceRed, // is players turn = spawn blue, else spawn red
				new Vector3(
				Mathf.Clamp(spawnPos.x, 0, numColumns-1), 
				gameObjectField.transform.position.y + 1, 0), // spawn it above the first row
				Quaternion.identity) as GameObject;

		return g;
	}

	void UpdatePlayAgainButton(){
		RaycastHit hit;
		//ray shooting out of the camera from where the mouse is
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
		if (Physics.Raycast(ray, out hit) && hit.collider.name == btnPlayAgain.name){
			btnPlayAgain.GetComponent<Renderer>().material.color = btnPlayAgainHoverColor;
			//check if the left mouse has been pressed down this frame
			if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && btnPlayAgainTouching == false){
				btnPlayAgainTouching = true;
					
				//CreateField();
				Application.LoadLevel(0);
			}
		}
		else{
			btnPlayAgain.GetComponent<Renderer>().material.color = btnPlayAgainOrigColor;
		}
			
		if(Input.touchCount == 0){
			btnPlayAgainTouching = false;
		}
	}

	// Update is called once per frame
	void Update () {

     
		if(isLoading)
			return;

		if(isCheckingForWinner)
			return;

		if(gameOver){
			winningText.SetActive(true);
			btnPlayAgain.SetActive(true);

			UpdatePlayAgainButton();

			return;
		}

		if(playerOneTurn || !computerPlayer){
            //commandLineUI.SetActive(false);
          
            if (gameObjectTurn == null){
				gameObjectTurn = SpawnPiece();
			}
			else{
				// update the hovering piece position
				Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				gameObjectTurn.transform.position = new Vector3(
					Mathf.Clamp(pos.x, 0, numColumns-1), 
					gameObjectField.transform.position.y + 1, 0);

				// click the left mouse button to drop the piece into the selected column
				if(Input.GetMouseButtonDown(0) && !mouseButtonPressed && !isDropping){
					mouseButtonPressed= true;

					StartCoroutine(dropPiece(gameObjectTurn));
				}
				else{
					mouseButtonPressed = false;
				}
			}
		}
        //computer player is on 
		else{
            //ensure peice is still above game board
			if(gameObjectTurn == null){
				gameObjectTurn = SpawnPiece();
			}
			else{
                if (!isDropping) {
                    StartCoroutine(dropPiece(gameObjectTurn));
                }

            }
        }
	}

    
    //checks if a peice is dropable in this row
   public bool canDrop(int col) {

        for (int i = numRows - 1; i >= 0; i--) {
            if (board[col, i] == 0) {
                return true;
            }
        }

        return false;

    }


    /// This method  is a cocourtine and searches for a empty cell and lets 
    /// the object fall down into this cell
    /// prevents the player from dropping into the middle colmn
    IEnumerator dropPiece(GameObject gObject){
		isDropping = true;

		Vector3 startPosition = gObject.transform.position;
		Vector3 endPosition = new Vector3();

		// round to a grid cell
		int x = Mathf.RoundToInt(startPosition.x);
		startPosition = new Vector3(x, startPosition.y, startPosition.z);

		// is there a free cell in the selected column?
		bool foundFreeCell = false;
        for (int i = numRows - 1; i >= 0; i--) {
            if (board[x, i] == 0) {
                foundFreeCell = true;
                board[x, i] = playerOneTurn ? (int)Piece.Blue : (int)Piece.Red;
                height[x] = i;
                //print(height[x]);
                endPosition = new Vector3(x, i * -1, startPosition.z);
                break;
            }
        }

		if(foundFreeCell){
			// Instantiate a new Piece, disable the temporary floating piece
			GameObject g = Instantiate (gObject) as GameObject;
			gameObjectTurn.GetComponent<Renderer>().enabled = false;

			float distance = Vector3.Distance(startPosition, endPosition);

			float t = 0;
			while(t < 1){
				t += Time.deltaTime * dropTime * ((numRows - distance) + 1);

				g.transform.position = Vector3.Lerp (startPosition, endPosition, t);
				yield return null;
			}

			g.transform.parent = gameObjectField.transform;

			// remove the temporary gameobject
			DestroyImmediate(gameObjectTurn);

			// run coroutine to check if someone has won
			StartCoroutine(Won());

			// wait until winning check is done
			while(isCheckingForWinner)
				yield return null;

			playerOneTurn = ! playerOneTurn;
		}

		isDropping = false;

		yield return 0;
	}

	/// Check for Winner
	IEnumerator Won(){
		isCheckingForWinner = true;

		for(int x = 0; x < numColumns; x++){
			for(int y = 0; y < numRows; y++){
				// Get the Layermask to Raycast against, if its Players turn only include
				// Layermask Blue otherwise Layermask Red
				int layermask = playerOneTurn ? (1 << 8) : (1 << 9);

				// If Players turn ignore red as Starting piece and wise versa
				if(board[x, y] != (playerOneTurn ? (int)Piece.Blue : (int)Piece.Red)){
					continue;
				}

				// Shoot a ray to the right to test horizontally
				RaycastHit[] hitsHorz = Physics.RaycastAll(
					new Vector3(x, y * -1, 0), 
					Vector3.right, 
					numPiecesToWin - 1, 
					layermask);

				// return true (won) if enough hits
				if(hitsHorz.Length == numPiecesToWin - 1){
					gameOver = true;
					break;
				}

				// Shoot a ray up to test vertically
				RaycastHit[] hitsVert = Physics.RaycastAll(
					new Vector3(x, y * -1, 0), 
					Vector3.up, 
					numPiecesToWin - 1, 
					layermask);
					
				if(hitsVert.Length == numPiecesToWin - 1){
					gameOver = true;
					break;
				}

					
                // Test Diagonally
					
				// Calculate the length of the ray to shoot diagonally
				float length = Vector2.Distance(new Vector2(0, 0), new Vector2(numPiecesToWin - 1, numPiecesToWin - 1));

				RaycastHit[] hitsDiaLeft = Physics.RaycastAll(
					new Vector3(x, y * -1, 0), 
					new Vector3(-1 , 1), 
					length, 
					layermask);
						
					if(hitsDiaLeft.Length == numPiecesToWin - 1){
						gameOver = true;
						break;
					}

					RaycastHit[] hitsDiaRight = Physics.RaycastAll(
						new Vector3(x, y * -1, 0), 
						new Vector3(1 , 1), 
						length, 
						layermask);
						
					if(hitsDiaRight.Length == numPiecesToWin - 1) {
						gameOver = true;
						break;
					}
					

					yield return null;
				}

				yield return null;
			}

			// if Game Over update the winning text to show who has won
			if(gameOver == true){
                print(playerOneTurn);
                if (playerOneTurn) {
                    winningText.GetComponent<TextMesh>().text = playerOneText;

                }else {
                    winningText.GetComponent<TextMesh>().text = playerTwoText;
                }
				
			}
			else {
				// check if there are any empty cells left, if not set game over and update text to show a draw
				if(!BoardContainsEmptyCell()){
					gameOver = true;
					winningText.GetComponent<TextMesh>().text = drawText;
				}
			}

			isCheckingForWinner = false;

			yield return 0;
		}

    //checks if the current player's move would win the game
    public bool isWinningMove(int col, int current_player) {

        print("height of col " + col + ":" + height[col]);
        bool winner = false;      
        ///if fucntion is used improperly on a row we cannot use. 
        if(height[col] - 1 < 0) {
            return false;
        }

        // "place" the predicted peice
        board[col, height[col] - 1] = current_player;

        int count = 0; 

        //checks vertical
        for (int i = 0; i < numRows; i++) {
            if (board[col, i] == current_player)
                count++;
            else
                count = 0;

            if (count >= 4) {
                print("vertical");
                winner = true;
            }
                
        }

       count = 0 ;
        ///checks for horizontal win
        for (int i = 0; i < numColumns; i++) {
            if (board[i, height[col] - 1 ] == current_player) {
               print(board[i, height[col] - 1]);
                count++;
            }       
            else
                count = 0;

            if (count >= 4) {
                print("horizonal");
                winner = true;
            }
                
        }

        // ascending diagonalCheck 
        for (int i = 3; i < numColumns; i++) {
            for (int j = 0; j < numRows - 3; j++) {
                if (board[i, j] == current_player &&
                    board[i - 1, j + 1] == current_player &&
                    board[i - 2, j + 2] == current_player &&
                    board[i - 3, j + 3] == current_player) {
                    winner = true;
                    break;
                }
            }
        }


        // descending diagonalCheck
        for (int i = 3; i < numColumns; i++) {
            for (int j = 3; j < numRows; j++) {
                if (board[i, j] == current_player &&
                    board[i - 1, j - 1] == current_player &&
                    board[i - 2, j - 2] == current_player &&
                    board[i - 3, j - 3] == current_player) {
                    winner = true;
                    break;
                }
            }
        } 

        //removes the  predicted piece
        board[col, height[col] - 1] = (int)Piece.Empty;

        if (winner) 
            return true;
        else
            return false;
    }

		
	///Check if the board field contains an empty cell 
	///True if it contains empty cell, false otherwise.
	bool BoardContainsEmptyCell(){
		for(int x = 0; x < numColumns; x++){
			for(int y = 0; y < numRows; y++){
				if(board[x, y] == (int)Piece.Empty)
					return true;
			}
		}
		return false;
	}


    public void DebugBoard() {
        string boardString = "";
        print("DEBUG");
        for (int x = 0; x < GameController.numColumns; x++) {
            boardString = boardString + "Col" + x + ":";
            for (int y = 0; y < GameController.numRows; y++) {
                boardString = boardString + board[x, y];
            }
        }

        print(boardString);
    }

}