using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C4_AI {
    //Position is a represention of the board state for an advanced AI. Structure from Russel/Norvig Textbook
    public class Position : MonoBehaviour {

        int[,] board;
        private Position boardState;
        int[] height;

        //count of move to determine what player is playing. 
        int moves; 

        public Position(Position boardState) {
            this.boardState = boardState;
        }

        public Position(int[,] gameBoard) {

            board = new int[GameController.numColumns, GameController.numRows];
            height = new int[GameController.numColumns];
            for (int x = 0; x < GameController.numColumns; x++) {
                height[x] = 6;
                for (int y = 0; y < GameController.numRows; y++) {
                    board[x, y] = 0;
                }
            }

            board = gameBoard;

            GameController.S.DebugBoard(board);
        }

        //Simuates a play of a peice on col on the structure for board representation
        void Simulate_Play(int col) {

            board[col, height[col] - 1] = 1 + moves % 2; //moves%2 adds 1 every other time 
            height[col] = height[col] - 1;
            moves++;

        }

        //checks if you can play in that row for the simulated board
        bool Can_Sim(int col) {

            for (int i = GameController.numRows - 1; i >= 0; i--) {
                if (board[col, i] == 0) {
                    return true;
                }
            }

            return false;
        }

    }

}

