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
    public Vector3 lookAtDirection;
    public Vector2 offsetDistance;

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
            Vector3 dir = new Vector2(centeredMousePos.x, centeredMousePos.y);

            if (autoFollow)
            {
                followOffset = Vector3.zero;

                if (IsMouseInsideScreen())
                {
                    if (Input.mousePosition.x <= manualMoveBorderDistance || Input.mousePosition.y <= manualMoveBorderDistance ||
                        Input.mousePosition.x > Screen.width - manualMoveBorderDistance || Input.mousePosition.y > Screen.height - manualMoveBorderDistance)
                    {
                        color.a = 0.2f;
                        followOffset = new Vector3(
                            dir.x * offsetDistance.x * 0.001f,
                            0,
                            dir.y * offsetDistance.y * 0.001f);
                    }
                }

                transform.position = Vector3.Lerp(transform.position,
                                                    playerTarget.transform.position + (lookAtDirection * distance) + followOffset,
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
                        transform.position += new Vector3(dir.x * manualMoveSpeed, 0f, dir.y * manualMoveSpeed) * Time.deltaTime;
                    }
                }
            }

            transform.LookAt(transform.position - lookAtDirection);
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
