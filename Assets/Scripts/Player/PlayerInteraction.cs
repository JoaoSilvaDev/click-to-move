using UnityEngine;
using Unity.Netcode;

public class PlayerInteraction : NetworkBehaviour
{
    public float interactionRange = 2f;
    private Camera cam;
    private Interactable hoveredInteractable;
    private Player player;
    public bool IsHoveringInteractable { get { return hoveredInteractable != null; } }

    public void Init(Player player)
    {
        this.player = player;
        cam = Camera.main;
    }

    private void Update()
    {
        if (!IsOwner) return;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Interactable newInteractable;
            if (hit.collider.gameObject.TryGetComponent<Interactable>(out newInteractable))
            {
                if (newInteractable != hoveredInteractable)
                    SelectInteractable(newInteractable);
            }
            else
            {
                DeselectCurentInteractable();
            }
        }
        else
        {
            DeselectCurentInteractable();
        }

        if (Input.GetMouseButtonDown(0) && IsHoveringInteractable)
            TryInteract(hoveredInteractable);
    }

    public void DeselectCurentInteractable()
    {
        if (hoveredInteractable)
        {
            hoveredInteractable.Unhover();
            hoveredInteractable = null;
        }
    }

    private void SelectInteractable(Interactable interactable)
    {
        DeselectCurentInteractable();
        interactable.Hover();
        hoveredInteractable = interactable;
    }

    public bool TryInteract(Interactable interactable)
    {
        if (!interactable.canInteract.Value) return false;

        if (Vector3.Distance(transform.position, interactable.transform.position) > interactionRange)
        {
            player.movement.SetTargetInteractable(interactable);
            return false;
        }
        else
        {
            Interact(interactable);
            return true;
        }
    }

    private void Interact(Interactable interactable)
    {
        interactable.Interact(player);
    }
}
