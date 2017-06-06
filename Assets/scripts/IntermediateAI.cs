using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C4_AI;

// An AI with "Level 1" thinking, taking in considerating it's own move to make sure the player cannot win 
public class IntermediateAI : SimpleAI {

    override public int CalcuateMove(Position gameBoard) {
        ///choose to block a winning play from the opponent. 

        if (gameBoard.GetMoves() == 0) {
            print("AI plays first");
            //AI should always play the middle col if playing first
            return 3;
        }
        ///create a copy of the boardstate to expand tree from
        Position boardState = new Position(gameBoard);

        List<int> moves = GetPossibleMoves(gameBoard);
        int choice = -1;
        //take winning move if available

        foreach (int col in moves) {
            if (boardState.isWInningMove(col, 2)) {
                return col;
            }  
        }

        int winningcol = CheckWinningMove(gameBoard);
        if (winningcol != -1) {
            return winningcol;
        }

        if (moves.Count > 0) {
            //take winning move if given to you
            foreach (int col in moves){
                if (boardState.isWInningMove(col, 2)) {
                    return col;
                }
            }
            //randomly choose a move, if this move would result in the player having a winning move, pick a different move
            choice = moves[Random.Range(0, moves.Count)];
            boardState.SimPlay(choice);
            if (CheckWinningMove(boardState) == choice) {
                print("choice would let other player win: " + choice);
                
                int intermediateChoice = choice;
                while (choice != intermediateChoice) {
                    intermediateChoice = moves[Random.Range(0, moves.Count)];
                }     
                choice = intermediateChoice;
                print("new choice:" + choice);
            }

            return choice;
        }
        //no moves are available
        return -1;

    }



}
