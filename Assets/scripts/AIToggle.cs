using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIToggle : MonoBehaviour {

    
  
    private void Update() {
        if(GameController.S.computerPlayer == true) {
            GetComponent<Image>().color = Color.red;
        } else {
            GetComponent<Image>().color = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            GameController.S.computerPlayer = !GameController.S.computerPlayer;
        }    
    }
 
}
