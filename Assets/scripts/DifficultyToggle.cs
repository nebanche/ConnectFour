using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyToggle : MonoBehaviour {
    int presscount = 0;
    [SerializeField]
    Text ButtonText;
    private void Update() {
        if (GameController.S.simpleOn == true) {
            GetComponent<Image>().color = Color.green;
        } else {
            GetComponent<Image>().color = Color.yellow;
        }

        if (Input.GetKeyDown(KeyCode.D)) {

            presscount++;
            if(presscount%2 == 0) {
                ButtonText.text = "Simple";
                GameController.S.simpleOn = true;
            }else {
                ButtonText.text = "Intermediate";
                GameController.S.simpleOn = false;
            }

        }
    }
}
