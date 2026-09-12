using UnityEngine;

public class HookVisual : MonoBehaviour
{
    [SerializeField] private LineRenderer ropeLineRenderer;

    [SerializeField] private Transform hookTransform;

    [SerializeField] private Transform boatTransform;

    private void LateUpdate()
    {
        if (!ropeLineRenderer.enabled) return;
        ropeLineRenderer.SetPosition(0, boatTransform.position);
        ropeLineRenderer.SetPosition(1, hookTransform.position);
    }

    public void ToggleRope(bool isActive) => ropeLineRenderer.enabled = isActive;
}
