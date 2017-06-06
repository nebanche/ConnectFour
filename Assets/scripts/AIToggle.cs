using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIToggle : MonoBehaviour {
    [SerializeField]
    Text ButtonText;
    private void Update() {
        if(GameController.S.computerPlayer == true) {
            GetComponent<Image>().color = Color.red;
            ButtonText.text = "AI On";
        } else {
            GetComponent<Image>().color = Color.white;
            ButtonText.text = "AI Off";
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            GameController.S.computerPlayer = !GameController.S.computerPlayer;
        }    
    }
 
}
