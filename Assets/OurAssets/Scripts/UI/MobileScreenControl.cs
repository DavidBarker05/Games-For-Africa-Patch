using UnityEngine;

public class MobileScreenControl : MonoBehaviour
{
    void Awake()
    {
        #if UNITY_STANDALONE && !UNITY_EDITOR
            gameObject.SetActive(false);
        #endif
    }
}
