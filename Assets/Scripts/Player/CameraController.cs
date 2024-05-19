using UnityEngine;
using Drawing;
using Unity.Netcode;
using UnityEditor;

public class CameraController : MonoBehaviour
{
    // todo: if hovering one of the outside areas add an offset to the offset
    // so it still follows the player but adds an offset
    [Header("AutoFollow")]
    public bool autoFollow = false;
    public float followLerpSpeed = 1f;
    public float distance = 15f;
    public Vector3 cameraDirection;
    public float offsetDistance = 2f;
    public float followMoveBorderDistance = 1f;

    [Header("Manual")]
    public float manualMoveSpeed = 1f;
    public float manualMoveBorderDistance = 1f;

    private Player playerTarget;
    private Vector3 followOffset;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            autoFollow = !autoFollow;


        if (!playerTarget)
        {
            playerTarget = PlayerManager.Instance.GetPlayerByClientID(NetworkManager.Singleton.LocalClientId);
        }

        if (playerTarget)
        {
            Color color = new Color(1f, 0f, 0f, 0.1f);
            Vector2 centeredMousePos = new Vector2(Input.mousePosition.x * 2f - Screen.width, Input.mousePosition.y * 2f - Screen.height);
            Vector3 dir = new Vector3(centeredMousePos.x, 0f, centeredMousePos.y);

            if (autoFollow)
            {
                if (IsMouseInsideScreen() && centeredMousePos.magnitude > followMoveBorderDistance)
                {
                    color.a = 0.2f;
                    followOffset = dir * 0.001f * offsetDistance;
                }
                else
                    followOffset = Vector3.zero;

                transform.position = Vector3.Lerp(transform.position,
                                                    playerTarget.transform.position + (cameraDirection * distance) + followOffset,
                                                    followLerpSpeed * Time.deltaTime);
            }
            else
            {
                if (IsMouseInsideScreen())
                {
                    if (Input.mousePosition.x <= manualMoveBorderDistance || Input.mousePosition.y <= manualMoveBorderDistance ||
                        Input.mousePosition.x > Screen.width - manualMoveBorderDistance || Input.mousePosition.y > Screen.height - manualMoveBorderDistance)
                    {
                        color.a = 0.2f;
                        transform.position += dir * manualMoveSpeed * Time.deltaTime;
                    }
                }
            }

            using (Draw.InScreenSpace(Camera.main))
            {
                if (autoFollow)
                    Draw.xy.Circle(new Unity.Mathematics.float2(Screen.width * 0.5f, Screen.height * 0.5f), followMoveBorderDistance * 0.5f, color);
                else
                {
                    Draw.xy.SolidRectangle(new Rect(0, 0, Screen.width, manualMoveBorderDistance), color);
                    Draw.xy.SolidRectangle(new Rect(0, 0, manualMoveBorderDistance, Screen.height), color);
                    Draw.xy.SolidRectangle(new Rect(0, Screen.height - manualMoveBorderDistance, Screen.width, manualMoveBorderDistance), color);
                    Draw.xy.SolidRectangle(new Rect(Screen.width - manualMoveBorderDistance, 0, manualMoveBorderDistance, Screen.height), color);
                }
            }
        }
    }

    public bool IsMouseInsideScreen()
    {
#if UNITY_EDITOR
        return Input.mousePosition.x >= 0 && Input.mousePosition.y >= 0 && Input.mousePosition.x <= Handles.GetMainGameViewSize().x && Input.mousePosition.y <= Handles.GetMainGameViewSize().y;
#else
        return Input.mousePosition.x >= 0 && Input.mousePosition.y >= 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y < Screen.height;
#endif
    }
}
