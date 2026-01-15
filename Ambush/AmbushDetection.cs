using UnityEngine;
using UnityEngine.Events;

public class AmbushDetection : MonoBehaviour
{
    public UnityEvent triggerAmbush;

    private void Start()
    {
        triggerAmbush.AddListener(AmbushManager.instance.Ambush);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            triggerAmbush.Invoke();
            Destroy(gameObject);
        }
    }
}
