using UnityEngine;

public class RagdollGenerator : MonoBehaviour
{
    [SerializeField] private GameObject rootBone;

    private void Start()
    {
        this.GenerateJoints(rootBone.transform);
    }

    void GenerateJoints(Transform joint)
    {
        CapsuleCollider collider = joint.gameObject.AddComponent<CapsuleCollider>();
        CharacterJoint characterJoint = joint.gameObject.AddComponent<CharacterJoint>();

        foreach (Transform childJoint in joint)
        {
            characterJoint.connectedBody = joint.parent.GetComponent<Rigidbody>();
            collider.center = childJoint.localPosition / 2;
            collider.height = childJoint.localPosition.magnitude;
            collider.radius = 0.0025f;

            if (childJoint.name.Contains("end")) return;

            this.GenerateJoints(childJoint);
        }

    }


}
