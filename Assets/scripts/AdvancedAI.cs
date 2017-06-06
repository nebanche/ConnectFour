using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C4_AI;

public class AdvancedAI : SimpleAI {


    override public int CalcuateMove() {

        //if first play in the middle 

        long nodeCount;

        int[,] boardCopy = GameController.S.read_board();

        Position boardState = new Position (boardCopy);

        return MinMaxStart(boardState);
    }


    // returns the  
    int MinMaxStart( Position boardState) {

        return MinMax(ref boardState);

    }

    int MinMax(ref Position boardState) {

        ///check for tie? 
        /*
        int bestScore = - GameController.numColumns * GameController.numRows; // init the best possible score with a lower bound of score.

        for (int x = 0; x < GameController.numColumns; x++) // compute the score of all possible next move and keep the best one
            if (boardState.Can_sim(x)) {
                Position deeperState = new Position (boardState);
                // It's opponent turn in deeper position after current player plays x column. 
                deeperState.SimPLayx);               
                // If current player plays col x, his score will be the opposite of opponent's score after playing col 
                int score = -MinMax(deeperState);
                // keep track of best possible score so far.
                if (score > bestScore) {
                    bestScore = score;
                }
            }
        
        return bestScore; */
        return -1;
    }

}