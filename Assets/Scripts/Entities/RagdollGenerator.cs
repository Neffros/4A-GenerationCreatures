using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollGenerator : MonoBehaviour
{
    public GameObject rootbone;

    public GameObject sphere;
    /*
     *
     * Les character joint sont dans un script, générer en fonction des bones :wink:
        Si je prend l’exemple d’un bras simple, en termes d’architecture t’aura un bone qui part de l’épaule jusqu’au coude, 
        un qui part du coude jusqu’à la main et un « bone end » qui n’à que la main comme point
        Pour chaque bone, il aura un character joint relié à son parent
        Pour sa capsule collider, tu mets le centre au milieu de deux bones et tu le scale en fonction de la distance entre ces deux

        Juste à faire ça du coup dans une boucle parcourant chaque bone

        Dites moi si besoins de précision , c’est peut etre pas super clair
     */
    private void Start()
    {
        GenerateJoints(rootbone);
    }

    void GenerateJoints(GameObject rootBone)
    {
        foreach (Transform child in rootBone.transform)
        {
            //Debug.Log(child.name);
            GenerateJoint(child);
        }
    }
    
    void GenerateJoint(Transform joint)
    {
        joint.gameObject.AddComponent<Rigidbody>();
        if (joint.parent.gameObject == rootbone)
        {
            GenerateJoint(joint.GetChild(0));
            return;
        }
        
        Transform parentJoint = joint.parent;
        
        CapsuleCollider capsuleCollider = joint.gameObject.AddComponent<CapsuleCollider>();
        CharacterJoint characterJoint = joint.gameObject.AddComponent<CharacterJoint>();
        
        //remove line below to have static object to debug
        characterJoint.connectedBody = parentJoint.GetComponent<Rigidbody>(); //joint must be connected to parent rigid body 
        Vector3 centerPos = Vector3.Lerp(parentJoint.localPosition , joint.localPosition,0.5f);
        capsuleCollider.center =centerPos;
        
        Debug.Log(centerPos.x + " - " + centerPos.y + " - "+ centerPos.z); //<-- only way to see the actual values, printing vec3 shows (0,0,0)
        
   
        capsuleCollider.height =   Vector3.Distance(joint.position, parentJoint.position)/100;

        capsuleCollider.radius = 0.0025f;
        if (joint.name.Contains("end")) return;
        GenerateJoint(joint.GetChild(0));
        
        //other logic where there was a reference to next instead of previous joint 
        /*
        Transform nextJoint = joint.GetChild(0);

        nextJoint.gameObject.AddComponent<Rigidbody>();
        CapsuleCollider capsuleCollider = nextJoint.gameObject.AddComponent<CapsuleCollider>();
        CharacterJoint characterJoint = nextJoint.gameObject.AddComponent<CharacterJoint>();
        capsuleCollider.isTrigger = true;
        characterJoint.connectedBody = joint.GetComponent<Rigidbody>();
        capsuleCollider.center = Vector3.Lerp(nextJoint.localPosition , joint.localPosition,0.5f);

        
        /*sphere.transform.localPosition =  Vector3.Lerp(joint.position, nextJoint.position, 0.5f);
        GameObject initedSphere = Instantiate(sphere);
        initedSphere.transform.SetParent(joint.parent); 
        initedSphere.transform.localPosition = joint.localPosition;
        
        
        capsuleCollider.radius = 0.0025f;
        capsuleCollider.height =   Vector3.Distance(joint.position, nextJoint.position)/100;
        
        if (nextJoint.name.Contains("end")) return;

        GenerateJoint(nextJoint);*/

    }
    
    
}
