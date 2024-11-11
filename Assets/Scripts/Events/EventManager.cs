using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    
    private void Awake()
    {
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
    
    // convert all these UnityActions to delegates and events afterwards. ENCOURAGE BEING EXPLICIT!
    
  
}