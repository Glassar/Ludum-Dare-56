using Unity.VisualScripting;
using UnityEngine;

public class GuageController : MonoBehaviour
{
    [SerializeField] Transform m_pointer;
    [SerializeField] PlayerController m_player;
    [SerializeField] float m_zeroRotation;
    [SerializeField] float m_fullRotation;

    void Update()
    {
        UpdatePointer();
    }

    public void UpdatePointer() {
        float ratio = m_player.oxygen / m_player.maxOxygen;

        Vector3 localRotation = m_pointer.localEulerAngles;

        m_pointer.localRotation = Quaternion.Euler(localRotation.x, localRotation.y, Mathf.Lerp(m_zeroRotation,m_fullRotation, ratio));
    }
}
