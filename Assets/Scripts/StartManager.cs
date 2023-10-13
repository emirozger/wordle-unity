using UnityEngine;
using UnityEngine.EventSystems;

public class StartManager : MonoBehaviour,IPointerDownHandler
{
    public static StartManager Instance { get; private set; }
    public bool isGameStarted;
    [SerializeField] private GameObject tapToPlay;

    private void Awake()
    {
        Instance = this;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isGameStarted)
        {
            tapToPlay.gameObject.SetActive(false);
            isGameStarted = true;
            Debug.Log(" basladi");
        }
    }
}