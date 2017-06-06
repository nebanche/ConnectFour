using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace C4_AI {
    //Position is a represention of the board state for an advanced AI. Structure from Russel/Norvig Textbook
    public class Position : MonoBehaviour {

        int[,] board;
        int[] height;


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

    }

}

