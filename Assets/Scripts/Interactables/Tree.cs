using Unity.Netcode;

public class NewTree : NetworkBehaviour
{
    private Interactable interactable;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    public void DestroyTreeVisuals()
    {
        // animation of tree falling or exploding or whatever
        // and then SetVisible(false)
        interactable.SetVisible(false);
    }
}
