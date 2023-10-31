public class HighlightedState : ActionStateBase
{
    public override void Enter(ActionBone action)
    {
        action.SetColor(action.Colors.Highlighted);
        action.Outline.enabled = true;
    }

    public override void Exit(ActionBone action)
    {
        action.SetColor(action.Colors.Default);
        action.Outline.enabled = false;
    }
}
