using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C4_AI {
    //Position is a represention  and interface of the board state for an advanced AI. Structure from Russel/Norvig Textbook
    public class Position {

        public int[,] board;
        public int[] height;

        //count of move to determine what player is playing. 
        int moves; 

        public Position(Position boardState) {

            Debug.Log("we are making a copy of a position");
            board = new int[GameController.numColumns, GameController.numRows];
            height = new int[GameController.numColumns];
            for (int x = 0; x < GameController.numColumns; x++) {
                height[x] = boardState.height[x];
                for (int y = 0; y < GameController.numRows; y++) {
                    board[x, y] = boardState.board[x,y];
                }
            }
        }

        public Position(int[,] gameBoard) {

            Debug.Log("WE ARE USING THIS WEIRD CTOR");
            InitalizeBoard();

            board = gameBoard;
            moves = 0;
            DebugBoard();
        }


        public Position() {
            InitalizeBoard();
        }

        void InitalizeBoard() {

            Debug.Log("we have hit initalization");
            board = new int[GameController.numColumns, GameController.numRows];
            height = new int[GameController.numColumns];
            for (int x = 0; x < GameController.numColumns; x++) {
                height[x] = 6;
                for (int y = 0; y < GameController.numRows; y++) {
                    board[x, y] = 0;
                }
            }
            DebugBoard();
        }

        //Simuates a play of a peice on col on the structure for board representation. Does not play the peice to the front end 
        public void SimPlay(int col) {

            ///need to check
            board[col, height[col] - 1] = 1 + moves % 2; //moves%2 adds 1 every other time 
            height[col] = height[col] - 1;
            moves++;

        }

        //checks if you can play in that row for the simulated board
        public bool CanPlay(int col) {

            for (int i = GameController.numRows - 1; i >= 0; i--) {
                if (board[col, i] == 0) {
                    return true;
                }
            }

            return false;
        }
        
        //returns immutable board
        //no  proper const methods in c#? Learning something everyday. 
        public int[,] ReadBoard() {
            return board;
        }

        //plays a peice for the asethetics of the game. Returns a Vector3 of the col position for the gameboard. 
        //Edits the board and height
        public Vector3 GamePlay(int x, Vector3 startPosition) {
            Vector3 endPosition = Vector3.zero;

            for (int i = GameController.numRows - 1; i >= 0; i--) {
                if (board[x, i] == 0) {
                    ///if player's turn, 1 , 2 if computer's turn
                    board[x, i] = 1 + moves % 2; 
                    height[x] = i;
                    moves++;
                    //print(height[x]);
                    endPosition = new Vector3(x, i * -1, startPosition.z);
                    break;
                }
            }
            //print(endPosition);
            return endPosition;
        }


        //checks if the opponent can win in the next move in the current boardState  
        public bool isWInningMove(int col, int current_player) {

           // print("height of col " + col + ":" + height[col]);
            bool winner = false;
            ///if fucntion is used improperly on a row we cannot use. 
            if (height[col] - 1 < 0) {
                return false;
            }

            // "place" the predicted peice
           
            board[col, height[col] - 1] = current_player;

            winner = WinCheck(current_player);

            //removes the  predicted piece
            
            board[col, height[col] - 1] = 0;
            

            if (winner)
                return true;
            else
                return false;
        }

        public bool WinCheck( int current_player){

            int count = 0;
            bool winner = false;
            //checks vertical
            for (int x = 0; x < GameController.numColumns; x++) {
                for (int y = 0; y < GameController.numRows; y++) {
                    if (board[x, y] == current_player)
                        count++;
                    else
                        count = 0;

                    if (count >= 4) {
                       // print("Winner in col: " + x);
                        //print("vertical");
                        winner = true;
                    }

                }
            }

            count = 0;
            ///checks for horizontal win
            for (int y = 0; y < GameController.numRows; y++) {
                for (int x = 0; x < GameController.numColumns; x++) {
                    if (board[x, y] == current_player) {
                        count++;
                    } else {
                        count = 0;
                    }
                    if (count >= 4) {
                        //print(count);
                       // print("horizonal");
                        winner = true;
                    }

                }
            }

            //print("horizontal count" + count);

            // ascending diagonalCheck 
            for (int i = 3; i < GameController.numColumns; i++) {
                for (int j = 0; j < GameController.numRows - 3; j++) {
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
            for (int i = 3; i < GameController.numColumns; i++) {
                for (int j = 3; j < GameController.numRows; j++) {
                    if (board[i, j] == current_player &&
                        board[i - 1, j - 1] == current_player &&
                        board[i - 2, j - 2] == current_player &&
                        board[i - 3, j - 3] == current_player) {
                        winner = true;
                        break;
                    }
                }
            }


            return winner; 
        }
        //Check if the board field contains an empty cell 
        ///True if it contains empty cell, false otherwise.
        public bool BoardContainsEmptyCell() {
            for (int x = 0; x < GameController.numColumns; x++) {
                for (int y = 0; y < GameController.numRows; y++) {
                    if (board[x, y] == 0)
                        return true;
                }
            }
            return false;
        }


        public void DebugBoard() {
            string boardString = "";
            //print("DEBUG");
            for (int x = 0; x < GameController.numColumns; x++) {
                boardString = boardString + "Col" + x + ":";
                for (int y = 0; y < GameController.numRows; y++) {
                    boardString = boardString + board[x, y];
                }
            }
            Debug.Log(boardString);
            //print(boardString);
        }


        public int GetMoves() {
            return moves;
        }

        public int [] ReadHeight() {
            return height;
        }

        
    } //postition

    //bummer, no tuple in C#
    public class ScoreCol : MonoBehaviour {

        //minmax score
        int score;
        //ingame column 
        int col; 


        public ScoreCol(int bestScore, int column) {
            int score = bestScore;
            int col = column;
        }

        public int GetScore() {
            return score;
        }

        public int GetCol() {
            return col;
        }

    }

} //namespace

