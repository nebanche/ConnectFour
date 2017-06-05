using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour {

    public int CalcuateMove() {
        GameController.S.DebugBoard();
        ///choose to block a winning play from the opponent. 
        for (int x = 0; x < GameController.numColumns; x++) { // check if current player can win next move
           // print(" Can drop:" + GameController.S.canDrop(x));
            if (GameController.S.canDrop(x) && GameController.S.isWinningMove(x, 1)) {
                print("Winning move would be at");
                print(x);
                return x;
            }
        }

        List<int> moves = GetPossibleMoves();

        //randomly choose a move
        if (moves.Count > 0) {
            return moves[Random.Range(0, moves.Count)];

        }

        //no moves are available
        return -1;

    }



    public List<int> GetPossibleMoves() {
        List<int> possibleMoves = new List<int>();
        for (int x = 0; x < GameController.numColumns; x++) {
            if (GameController.S.canDrop(x)) {
                possibleMoves.Add(x);
            }
        }
        return possibleMoves;
    }




}