using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class ButtonCanvasGroupController : MonoBehaviour
    {

        public CanvasGroup normal;
        public CanvasGroup highlight;
        public CanvasGroup pressed;

        public void SetCanvasGroupAlpha()
        {
            pressed.alpha = 0;
        }


    }
}
