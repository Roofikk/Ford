public class SelectedState : ActionStateBase
{
    public override void Enter(ActionBone action)
    {
        action.SetColor(action.Colors.Drag);
        action.Outline.enabled = true;
    }

    public override void Exit(ActionBone action)
    {
        action.Outline.enabled = false;
    }
}
