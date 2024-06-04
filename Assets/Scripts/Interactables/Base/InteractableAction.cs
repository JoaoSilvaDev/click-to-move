using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class InteractableAction : NetworkBehaviour
{
    public UnityEvent OnHoverEvent;
    public UnityEvent OnUnhoverEvent;
    public UnityEvent<Player> OnClientInteractEvent;
    // public UnityEvent OnNetworkInteractEvent;

    [Header("Visuals")]
    public Color hoverColorAdd = new Color(0.2f, 0.2f, 0.2f);

    private Interactable interactable;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.OnHover += OnHover;
        interactable.OnUnhover += OnUnhover;
        interactable.OnClientInteract += OnClientInteract;
        // interactable.OnNetworkInteract += OnNetworkInteract;
    }

    private void OnHover()
    {
        interactable.renderers[0].material.color = interactable.defaultColor + new Color(0.2f, 0.2f, 0.2f);

        OnHoverEvent.Invoke();
    }

    private void OnUnhover()
    {
        interactable.renderers[0].material.color = interactable.defaultColor;

        OnUnhoverEvent.Invoke();
    }

    private void OnClientInteract(Player interactor)
    {
        OnClientInteractEvent.Invoke(interactor);
    }

    // private void OnNetworkInteract()
    // {
    //     OnNetworkInteractEvent.Invoke();
    // }
}
