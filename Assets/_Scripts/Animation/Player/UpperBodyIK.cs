using UnityEngine;
using RootMotion.FinalIK;
public class UpperBodyIK : MonoBehaviour
{
    #region Variables

    [Header("Final IK Modules")]
    [SerializeField] private LookAtIK _headLookAtIK = default;
    [SerializeField] private LookAtIK _bodyLookAtIK = default;
    #endregion

    #region BuiltIn Methods
    private void Start()
    {
        _headLookAtIK.enabled = false;
        _bodyLookAtIK.enabled = false;
    }
    private void Update()
    {
      // avoid issues with character
        _bodyLookAtIK.solver.FixTransforms();
        _headLookAtIK.solver.FixTransforms();
    }

    // Final IK overrides animation result at last frame, need to call solvers from lateupdate
    private void LateUpdate()
    {
        LookAtIKUpdate();
    }
    #endregion

    #region Custom Methods
    private void LookAtIKUpdate()
    {
        _bodyLookAtIK.solver.Update();
        _headLookAtIK.solver.Update();
    }
    #endregion
}