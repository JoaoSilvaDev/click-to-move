using UnityEngine;

public class CatalogueManager : MonoBehaviour
{
    public static CatalogueManager Instance { get; private set; }

    [SerializeField] private ItemCatalogue itemCatalogue;
    public ItemCatalogue ItemCatalogue { get { return itemCatalogue; } }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}