using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XianXia
{
    public class InitialCanvas : MonoBehaviour
    {
        // Start is called before the first frame update
        int screenWidth;
        [SerializeField]
        int originHeight;
        int screenHeight;
        [SerializeField]
        TMP_Text text;
        void Start()
        {
            screenHeight = Screen.height;
            float value = screenHeight / (float)originHeight;
            GetComponent<CanvasScaler>().scaleFactor = value;
        }

        // Update is called once per frame
        void Update()
        {
            text.text = Screen.width + "//" + Screen.height + "//" + GetComponent<CanvasScaler>().scaleFactor.ToString();

        }
    }
}
