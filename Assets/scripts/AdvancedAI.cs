using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C4_AI;

public class AdvancedAI : SimpleAI {


    override public int CalcuateMove() {

        //if first play in the middle 


        int[,] boardCopy = GameController.S.read_board();

        Position boardState = new Position (boardCopy);
        

        ///check if next move is the winning one 
        

        return MinMaxStart(boardState);
    }


    // returns the  
    int MinMaxStart( Position boardState) {

        return MinMax(ref boardState);

    }

    int MinMax(ref Position boardState) {

        ///minmax code will be here. 
        return -1;
    }

}