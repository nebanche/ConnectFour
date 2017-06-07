using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C4_AI;



//// not implemented in the final game, but left to show what I was working on for this assesment. 
public class AdvancedAI : SimpleAI {

    long nodeCount;
    int winningSlot = -1;
    bool winningmovefound = false; 

    override public int CalcuateMove(Position gameBoard) {

        winningmovefound = false;
        //if first play in the middle 
        if (gameBoard.GetMoves() == 0) {
            print("AI plays first");
            //AI should always play the middle col if playing first
            return 3; 
        }
        ///create a copy of the boardstate to expand tree from
        Position boardState = new Position (gameBoard);

        print("before minmax");
        boardState.DebugBoard();

        ScoreCol calcuate = MinMaxStart(boardState);

        print("after minmax");
        boardState.DebugBoard();
        print("original board");
        gameBoard.DebugBoard();

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
        nodeCount++;

        if (winningmovefound == true) {
            ScoreCol winner = new ScoreCol(0, winningSlot);
            return winner;
        }
        //
        
        //make sure game is not a tie
        if(boardState.GetMoves() == GameController.numColumns * GameController.numRows) {
            print("we hit a tie");
            ScoreCol tie = new ScoreCol(0, 0);
            return tie; 
        }


        ///check to prevent winning move from player
        int winningcol = CheckWinningMove(boardState);

        if (winningcol != -1) {

            ScoreCol winner = new ScoreCol(GameController.numColumns * GameController.numRows + 1 - boardState.GetMoves()/ 2
                , winningcol);
            print("the other player will win");
            print(winningSlot);
            winningmovefound = true;
            winningSlot = winningcol;
            return winner;
        }

        if (nodeCount > 50) {
            ScoreCol earlyexit = new ScoreCol(-(GameController.numColumns * GameController.numRows), -1);
            return earlyexit;
        }

        int bestScore = -(GameController.numColumns * GameController.numRows); //  best possible score with a lower bound of score.
        int bestCol = -1;
        // compute the score of all possible moves and keep the best one
        for (int x = 0; x < GameController.numColumns; x++) {
            if (boardState.CanPlay(x)) {
                Position deeperState = new Position(boardState);
                // It's opponent turn in deeper position after current player plays x column. 
                deeperState.SimPlay(x);
                // If current player plays col x, his score will be the opposite of opponent's score after playing col 
                ScoreCol check = MinMax(ref deeperState);
                int score = check.GetScore();
                // keep track of best possible score so far.
                if (score > bestScore) {
                    bestScore = score;
                    bestCol = x;
                    if(bestScore >= (GameController.numColumns * GameController.numRows)/10) {
                        ScoreCol earlyexit = new ScoreCol(bestScore, bestCol);
                        return earlyexit;
                    }
                }
            }
        }
        ScoreCol exit = new ScoreCol(bestScore, bestCol);
        return exit; 
       
    }


    
}