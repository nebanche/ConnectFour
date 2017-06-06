using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C4_AI;

public class AdvancedAI : SimpleAI {

    long nodeCount;

    override public int CalcuateMove(Position gameBoard) {

        //if first play in the middle 
        if (gameBoard.GetMoves() == 0) {
            print("AI plays first");
            //AI should always play the middle col if playing first
            return 3; 
        }
        ///create a copy of the boardstate to expand tree from
        Position boardState = new Position (gameBoard);

        ScoreCol calcuate = MinMaxStart(boardState);
        print("Our best answer here is col" + calcuate.GetCol());
        print("Best answer is of score" + calcuate.GetScore());
        return calcuate.GetCol();
    }


    //starter function for recursive MinMax
    ScoreCol MinMaxStart( Position boardState) {

        return MinMax(ref boardState);

    }


    //MinMax algorithmn without alpha-beta pruning. Modifies the boardState to find the 
    ScoreCol MinMax(ref Position boardState) {

        //make sure game is not a tie
        if(boardState.GetMoves() == GameController.numColumns * GameController.numRows) {
            ScoreCol tie = new ScoreCol(0, 0);
            return tie; 
        }


        ///check to prevent winning move from player
        int winningcol = CheckWinningMove(boardState);

        if (winningcol != -1) {

            ScoreCol winner = new ScoreCol(GameController.numColumns * GameController.numRows +1 - boardState.GetMoves()/ 2
                , winningcol);

            return winner;
        }


        int bestScore = - GameController.numColumns * GameController.numRows; //  best possible score with a lower bound of score.
        int bestCol = -1;
        // compute the score of all possible moves and keep the best one
        for (int x = 0; x < GameController.numColumns; x++)
            if (boardState.CanPlay(x)) {
                Position deeperState = new Position (boardState);
                // It's opponent turn in deeper position after current player plays x column. 
                deeperState.SimPlay(x);
                // If current player plays col x, his score will be the opposite of opponent's score after playing col 
                ScoreCol check= MinMax(ref deeperState);
                int score = check.GetScore();
                // keep track of best possible score so far.
                if (score > bestScore) {
                    bestScore = score;
                    bestCol = x;
                }
            }
        ScoreCol exit = new ScoreCol(bestScore, bestCol);
        return exit; 
       
    }


    
}