using UnityEngine;

public class Wire : MonoBehaviour
{
    private readonly float _minRadius = 0.01f;
    private readonly float _minheight = 0.01f;
    private readonly float _maxSwingAngle = 60f;

    [SerializeField] private float _mass = 0.1f;
    [SerializeField] private CharacterJoint _probe;

    void Start()
    {
        SkinnedMeshRenderer skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
        Transform[] bones = skinnedRenderer.bones;

        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i].TryGetComponent<Rigidbody>(out Rigidbody rigidbody) == false)
            {
                Rigidbody rb = bones[i].gameObject.AddComponent<Rigidbody>();
                rb.mass = _mass;
            }
            else
            {
                rigidbody.mass = _mass;
            }

            CharacterJoint joint;

            CapsuleCollider collider = bones[i].gameObject.AddComponent<CapsuleCollider>();
            collider.radius = _minRadius;
            collider.height = _minheight;

            if (i > 0)
            {
                joint = bones[i].gameObject.AddComponent<CharacterJoint>();
                joint.connectedBody = bones[i - 1].GetComponent<Rigidbody>();
                SoftJointLimit limit = new SoftJointLimit();
                limit.limit = _maxSwingAngle / 2f;
                joint.swing1Limit = limit;
                joint.swing2Limit = limit;
            }
        }

        _probe.connectedBody = bones[bones.Length - 1].GetComponent<Rigidbody>();
    }
}