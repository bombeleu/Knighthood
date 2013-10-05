// Steve Yeager
// 10.4.2013

using UnityEditor;

[CustomEditor(typeof(CharacterHealth))]
public class CharacterHealthEditor : HealthEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}