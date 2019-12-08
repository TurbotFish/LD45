using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class LaunchGameButton : MonoBehaviour
{
    public bool hasBeenClicked;
    public Animator buttonAnim;
    public bool isFirstButton;

    [DllImport("__Internal")]
    private static extern void StartGameEvent();

    [DllImport("__Internal")]
    private static extern void ReplayEvent();
    private void OnMouseDown()
    {
        if (!hasBeenClicked)
        {
            SoundManager.Instance.PlaySound(1, SoundManager.Instance.click);
            hasBeenClicked = true;
            buttonAnim.SetBool("clicked", hasBeenClicked);
            FlowManager.Instance.PlayGameFromMenu();

#if UNITY_WEBGL
            if (isFirstButton)
            {
                StartGameEvent();
            }
            else
            {
                ReplayEvent();
            }
#endif
        }
    }

    public void ResetMenu()
    {
        hasBeenClicked = false;
        buttonAnim.SetBool("clicked", hasBeenClicked);
    }

}


