using UnityEngine;

public class LagBehindTarget : MonoBehaviour
{
    [SerializeField] Transform m_followTarget;
    [SerializeField] Transform m_animateTarget;
    [SerializeField] float m_followSpeed = 1f;
    [SerializeField] float m_rotateSpeed = 1f;

    private void Start()
    {
        if (m_animateTarget == null)
        {
            m_animateTarget = transform;
        }
    }

    private void Update()
    {
        m_animateTarget.transform.position = Vector3.Lerp(m_animateTarget.position, m_followTarget.position, Time.deltaTime * m_followSpeed);
        m_animateTarget.rotation = Quaternion.Lerp(m_animateTarget.rotation, m_followTarget.rotation, Time.deltaTime * m_rotateSpeed);
    }
}
