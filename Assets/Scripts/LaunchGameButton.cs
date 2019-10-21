using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchGameButton : MonoBehaviour
{
    public bool hasBeenClicked;
    public Animator buttonAnim;




    private void OnMouseDown()
    {
        if (!hasBeenClicked)
        {
            SoundManager.Instance.PlaySound(1, SoundManager.Instance.click);
            hasBeenClicked = true;
            buttonAnim.SetBool("clicked", hasBeenClicked);
            FlowManager.Instance.PlayGameFromMenu();

        }
    }

    public void ResetMenu()
    {
        hasBeenClicked = false;
        buttonAnim.SetBool("clicked", hasBeenClicked);
    }

}


