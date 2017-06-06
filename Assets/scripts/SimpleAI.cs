using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C4_AI;

public class SimpleAI : MonoBehaviour {

    virtual public int CalcuateMove(Position gameBoard) {
        ///choose to block a winning play from the opponent. 

        int winningcol = CheckWinningMove(gameBoard);

        if(winningcol != -1) {
            return winningcol;
        }
        

        List<int> moves = GetPossibleMoves(gameBoard);

        //randomly choose a move
        if (moves.Count > 0) {
            return moves[Random.Range(0, moves.Count)];

        }

        //no moves are available
        return -1;

    }

    public List<int> GetPossibleMoves(Position gameBoard) {
        List<int> possibleMoves = new List<int>();
        for (int x = 0; x < GameController.numColumns; x++) {
            if (gameBoard.CanPlay(x)) {
                possibleMoves.Add(x);
            }
        }
        return possibleMoves;
    }

    //generalized to check winning move 
    public int CheckWinningMove(Position gameBoard) {

        for (int x = 0; x < GameController.numColumns; x++) { // check if current player can win next move
            if (gameBoard.CanPlay(x) && gameBoard.isWInningMove(x, 1)) {
                print("Winning move would be at");
                print(x);
                return x;
            }
        }

        //no winning move
        return -1; 
    }




}