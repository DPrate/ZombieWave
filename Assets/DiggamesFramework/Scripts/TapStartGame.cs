using UnityEngine;
using System.Collections;

public class TapStartGame : MonoBehaviour
{
    private static bool hasStartedGame = false;

    public GameObject TapStartGameObject;

    void Start()
    {
        if(hasStartedGame)
            TapStartGameObject.SetActive(false);
    }

    //Unity event call
    public void OnButtonTapToStartGame()
    {
        hasStartedGame = true;

        TapStartGameObject.SetActive(false);
    }
}
