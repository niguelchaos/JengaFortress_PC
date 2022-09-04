using UnityEngine;
using RootMotion.FinalIK;
public class UpperBodyIK : MonoBehaviour
{
    #region Variables

    [Header("Final IK Modules")]
    [SerializeField] private LookAtIK _headLookAtIK = default;
    [SerializeField] private LookAtIK _bodyLookAtIK = default;
    [SerializeField] private ArmIK _leftArmIK = default;
    [SerializeField] private ArmIK _rightArmIK = default;
    [SerializeField] private FullBodyBipedIK _fbbIK = default;

    [Header("LookAt Settings")]
    [SerializeField] private Transform _camera = default;
    [SerializeField] private Transform _headTarget = default;
    [SerializeField] private Transform _bodyTarget = default;
    
    [Header("Head Effector Settings")]
    [SerializeField] private Transform _headEffector = default;

    [Range(-89, 0)]
    [SerializeField] private float _maxAngleUp = -50f;
    [Range(0, 89)]
    [SerializeField] private float _maxAngleDown = 70f;

    [Range(-89f, 89f)]
    private float _bodyOffsetAngle = 45f;
    #endregion

    #region BuiltIn Methods
    private void Start()
    {
        _headLookAtIK.enabled = false;
        _bodyLookAtIK.enabled = false;
        _rightArmIK.enabled = false;
        _leftArmIK.enabled = false;
        _fbbIK.enabled = false;
    }
    private void Update()
    {
      // avoid issues with character
        // _bodyLookAtIK.solver.FixTransforms();
        // _headLookAtIK.solver.FixTransforms();
        // _fbbIK.solver.FixTransforms();
        // _rightArmIK.solver.FixTransforms();
        // _leftArmIK.solver.FixTransforms();
    }

    // Final IK overrides animation result at last frame, need to call solvers from lateupdate
    private void LateUpdate()
    {
        // LookAtIKUpdate();
        // FBBIKUpdate();
        // ArmsIKUpdate();
    }
    #endregion

    #region Custom Methods
    private void LookAtIKUpdate()
    {
        _bodyLookAtIK.solver.Update();
        _headLookAtIK.solver.Update();
    }

    private void FBBIKUpdate()
    {
        _fbbIK.solver.Update();
        // _camera.LookAt(_headTarget);
        // _headEffector.LookAt(_headTarget);
        UpdateLookTargetPos();
    }

    private void ArmsIKUpdate()
    {
        _rightArmIK.solver.Update();
        _leftArmIK.solver.Update();
    }
    
    private void UpdateLookTargetPos()
    {
        Vector3 targetForward = Quaternion.LookRotation(new 
            Vector3(_camera.transform.forward.x, 0f, _camera.transform.forward.z)) * Vector3.forward;

        // Vector3 targetForward = new Vector3(_camera.transform.forward.x, 0f, _camera.transform.forward.z);
        // targetForward = Quaternion.Euler(targetForward) * Vector3.forward;
        
        // signed angle gets angle between directions - here, between camera look and forward direction of char
        float angle = Vector3.SignedAngle(targetForward, _camera.forward, _camera.right);
        float percent;
        float maxY = 100f;
        float minY = -100f;

        if (angle < 0)
        {
            percent = Mathf.Clamp01(angle / _maxAngleUp);
            if (percent >= 1f)
            {
                maxY = 0f;
            }
        }
        else
        {
            percent = Mathf.Clamp01(angle / _maxAngleDown);
            if (percent >= 1f)
            {
                minY = 0f;
            }
        }
        
        Vector3 offset = _camera.right * InputManager.Instance.lookInput.x + 
            _camera.up * Mathf.Clamp(InputManager.Instance.lookInput.y, minY, maxY);
        
        offset += _headTarget.transform.position;
        Vector3 projectedPoint = (offset - _camera.position).normalized * 20f + _camera.position; 

        // update head and body look at targets
        _headTarget.transform.position = projectedPoint;
        _bodyTarget.transform.position = GetPosFromAngle(projectedPoint, _bodyOffsetAngle, transform.right);
    }

    private Vector3 GetPosFromAngle(Vector3 projectedPoint, float angle, Vector3 axis)
    {
        float dist = (projectedPoint - transform.position).magnitude * 
            Mathf.Tan(angle * Mathf.Deg2Rad);
        return projectedPoint + (dist * axis);
    }


    #endregion
}