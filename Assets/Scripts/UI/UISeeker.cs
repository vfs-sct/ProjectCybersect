using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISeeker : MonoBehaviour
{
    public Sprite unlocked;
    public Sprite locked;
    public Sprite outOfRange;

    private Grapple grapple;
    private Image seekerImage;

    private void Start()
    {
        grapple = GameObject.Find("player").GetComponent<Grapple>();
        seekerImage = GetComponent<Image>();
    }

    GrappleCursorState lastCursorState;
    private void LateUpdate()
    {
        if (lastCursorState != grapple.cursorState)
        {
            switch (grapple.cursorState)
            {
                case GrappleCursorState.LOCKED:
                    seekerImage.sprite = locked;
                    break;
                case GrappleCursorState.UNLOCKED:
                    seekerImage.sprite = unlocked;
                    break;
                case GrappleCursorState.OUT_OF_RANGE:
                    seekerImage.sprite = outOfRange;
                    break;
            }
        }

        lastCursorState = grapple.cursorState;
    }
}
