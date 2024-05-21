using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI : NetworkBehaviour
{
    // this is temp code and super hard coded for speed
    [Header("Canvas")]
    public Canvas authenticatingPanel;
    public Canvas menuPanel;
    public Canvas connectingPanel;
    public Canvas playerPanel;

    [Header("Main Menu")]
    public TMP_InputField joinCodeInput;
    public TextMeshProUGUI joinCodeText;
    public TextMeshProUGUI playerID;

    public static UI instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        connectingPanel.enabled = false;
        menuPanel.enabled = false;
        playerPanel.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            print(HostManager.instance.joinCode);
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnLocalClientIdChanged;
    }

    #region Canvases
    // this is temp code and super hard coded for speed
    public void SetAuthenticating()
    {
        authenticatingPanel.enabled = true;
        menuPanel.enabled = false;
        connectingPanel.enabled = false;
        playerPanel.enabled = false;
    }

    // this is temp code and super hard coded for speed
    public void SetLobbyMenu()
    {
        authenticatingPanel.enabled = false;
        menuPanel.enabled = true;
        connectingPanel.enabled = false;
        playerPanel.enabled = false;
    }

    // this is temp code and super hard coded for speed
    public void SetConnecting()
    {
        authenticatingPanel.enabled = false;
        menuPanel.enabled = false;
        connectingPanel.enabled = true;
        playerPanel.enabled = false;
    }

    // this is temp code and super hard coded for speed
    public void SetIngame()
    {
        authenticatingPanel.enabled = false;
        menuPanel.enabled = false;
        connectingPanel.enabled = false;
        playerPanel.enabled = true;
    }
    #endregion

    #region Join Code
    public void CopyJoinCodeToClipboard()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = joinCodeText.text;
        textEditor.SelectAll();
        textEditor.Copy();
    }

    private void OnLocalClientIdChanged(ulong newClientID)
    {
        playerID.text = $"client id: {NetworkManager.Singleton.LocalClientId}";
    }
    #endregion
}
