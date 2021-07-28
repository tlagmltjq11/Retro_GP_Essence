using UnityEngine;

public class LateUpdateFollow : MonoBehaviour
{
    public Transform m_targetToFollow;

    //Update 이후에 
    private void LateUpdate()
    {
        transform.position = m_targetToFollow.position;
        transform.rotation = m_targetToFollow.rotation;
    }
}