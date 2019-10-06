using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchGameButton : MonoBehaviour
{
    public bool hasBeenClicked;
    public Animator buttonAnim;

    // Start is called before the first frame update
    void Start()
    {

    }


    private void OnMouseDown()
    {
        if (!hasBeenClicked)
        {
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


