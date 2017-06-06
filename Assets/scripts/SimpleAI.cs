using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour {

    virtual public int CalcuateMove() {
        ///choose to block a winning play from the opponent. 

        int winningcol = CheckWinningMove();

        if(winningcol != -1) {
            return winningcol;
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

    //generalized to check winning move to use in advanced AI. 
    public int CheckWinningMove() {

        for (int x = 0; x < GameController.numColumns; x++) { // check if current player can win next move
            if (GameController.S.canDrop(x) && GameController.S.isWinningMove(x, 1)) {
                print("Winning move would be at");
                print(x);
                return x;
            }
        }

        //no winning move
        return -1; 
    }




}