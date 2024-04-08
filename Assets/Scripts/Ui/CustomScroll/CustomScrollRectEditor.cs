#if UNITY_EDITOR

using UnityEditor.UI;

namespace UnityEditor
{
    [CustomEditor(typeof(CustomScrollRect))]
    public class CustomScrollRectEditor : ScrollRectEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            CustomScrollRect targetScroll = (CustomScrollRect)target;
            targetScroll.LoadIconPrefab = (LoadIcon)EditorGUILayout.ObjectField(
                "Load Icon Prefab", targetScroll.LoadIconPrefab, typeof(LoadIcon), true);
            targetScroll.DistanceForPagination = EditorGUILayout.FloatField(
                "Distance For Pagination", targetScroll.DistanceForPagination);
            targetScroll.scrollLocker = (LockCustomScroll)EditorGUILayout.ObjectField(
                "Scroll Locker", targetScroll.scrollLocker, typeof(LockCustomScroll), true);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(targetScroll, "Change reference of object");
            }

            base.OnInspectorGUI();
        }
    }
}

#endif