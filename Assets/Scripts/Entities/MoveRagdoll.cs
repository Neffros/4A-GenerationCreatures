using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRagdoll : MonoBehaviour
{

    private Rigidbody _rigidbody;

    private float _interval = 0.5f;

    private float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > _interval)
        {
            _rigidbody.AddForce(Vector3.forward*20, ForceMode.VelocityChange);
            _interval = Random.Range(0.25f, 0.100f);
            timer = 0;
        }
        timer += Time.deltaTime;
    }
}
