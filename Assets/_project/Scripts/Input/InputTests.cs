using UnityEngine;

public class InputTests : MonoBehaviour
{
    private void Start()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.OnMoveMonsterPressed += OnMoveTest;
        InputManager.Instance.OnMoveTurboPressed += OnLookTest;
        InputManager.Instance.OnTotem1Pressed += OnButton0Test;
        InputManager.Instance.OnTotem2Pressed += OnButton1Test;
    }

    private void OnMoveTest(Vector2 moveDirection)
    {
        Debug.Log("move");
    }

    private void OnLookTest(Vector2 lookDirection)
    {
        Debug.Log("look");
    }

    private void OnButton0Test(bool pressed)
    {
        Debug.Log("button0");
    }

    private void OnButton1Test(bool pressed)
    {
        Debug.Log("button1");
    }
}
