using System;
using UnityEngine;

[ExecuteInEditMode]
public class RagdollGenerator : MonoBehaviour
{
    //[SerializeField] private GameObject rootBone;
    private Transform rootBone;
    #region Variables statiques

    private static RagdollGenerator _instance = null;

    #endregion

    #region Propriétés statiques

    public static RagdollGenerator Instance => RagdollGenerator._instance;

    #endregion

    private void Awake()
    {
        RagdollGenerator._instance = this;
    }
    
    private void OnDestroy()
    {
        if (RagdollGenerator._instance == this)
            RagdollGenerator._instance = null;
    }

    /*private void Start()
    {
        rootBone.AddComponent<Rigidbody>();
        rootBone.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        foreach (Transform child in rootBone.transform)
        {
            GenerateJoints(child);
        }
    }*/

    public GameObject GenerateRagdoll(GameObject asset, Vector3 scale)
    {
        asset.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
        asset.transform.GetChild(0).gameObject.AddComponent<Rigidbody>();
        asset.transform.GetChild(0).gameObject.GetComponent<BoxCollider>().size = scale;
        asset.transform.GetChild(0).gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        rootBone = asset.transform.GetChild(0).GetChild(0);
        rootBone.gameObject.AddComponent<Rigidbody>();
        rootBone.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        foreach (Transform child in rootBone)
        {
            GenerateJoints(child);
        }

        return asset;
    }

    void GenerateJoints(Transform joint)
    {
        CapsuleCollider collider = joint.gameObject.AddComponent<CapsuleCollider>();
        CharacterJoint characterJoint = joint.gameObject.AddComponent<CharacterJoint>();

        foreach (Transform childJoint in joint)
        {
            
            characterJoint.connectedBody = joint.parent.GetComponent<Rigidbody>();
            if (joint.parent == rootBone)
            {
                joint.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            }
            
            joint.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            collider.center = childJoint.localPosition / 2;
            collider.height = childJoint.localPosition.magnitude;
            collider.radius = 0.0025f;

            if (childJoint.name.Contains("end")) return;
            this.GenerateJoints(childJoint);
        }

    }


}
