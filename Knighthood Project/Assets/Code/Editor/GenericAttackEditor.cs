// Steve Yeager
// 9.7.2013

using UnityEditor;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[CustomEditor(typeof(GenericAttack))]
public class GenericAttackEditor : AttackEditor
{
    #region Serialized Fields

    private SerializedObject myObject;

    private SerializedProperty
        showTooltips,
        attackAnimation,
        windUp,
        attackTime,
        windDown,
        cooldown,
        Attack_Prefab,
        hitInfo,
        hitboxTime,
        offset,
        parented,
        hitNumber,
        oneShot;

    private SerializedProperty
        melee,
        meleeSize;

    private SerializedProperty
        magic,
        magicRequired;

    private SerializedProperty
        projectile,
        projectileVector,
        projectileSpeed;
        

    #endregion

    #region Toggle Fields

    private bool titlebarToggle;

    #endregion


    #region Editor Overrides

    protected override void OnEnable()
    {
        base.OnEnable();
        
        myObject = new SerializedObject(target);

        showTooltips = myObject.FindProperty("showTooltips");
        attackAnimation = myObject.FindProperty("attackAnimation");
        windUp = myObject.FindProperty("windUp");
        attackTime = myObject.FindProperty("attackTime");
        windDown = myObject.FindProperty("windDown");
        cooldown = myObject.FindProperty("cooldown");
        Attack_Prefab = myObject.FindProperty("Attack_Prefab");
        hitInfo = myObject.FindProperty("hitInfo");
        hitboxTime = myObject.FindProperty("hitboxTime");
        offset = myObject.FindProperty("offset");
        parented = myObject.FindProperty("parented");
        hitNumber = myObject.FindProperty("hitNumber");
        oneShot = myObject.FindProperty("oneShot");

        melee = myObject.FindProperty("melee");
        meleeSize = myObject.FindProperty("meleeSize");

        magic = myObject.FindProperty("magic");
        magicRequired = myObject.FindProperty("magicRequired");

        projectile = myObject.FindProperty("projectile");
        projectileVector = myObject.FindProperty("projectileVector");
        projectileSpeed = myObject.FindProperty("projectileSpeed");

        titlebarToggle = true;
    }


    public override void OnInspectorGUI()
    {
        myObject.Update();

        #region Base

        showTooltips.boolValue = EditorGUILayout.Toggle("Show Tooltips", showTooltips.boolValue);
        
        base.OnInspectorGUI();

        PropertyField(attackAnimation, "Animation", "Sprite animation to play while attacking.");
        PropertyField(windUp, "Wind Up Time", "Time in seconds before hitbox is created.");
        PropertyField(attackTime, "Attack Time", "Time in seconds performing the attack takes.");
        PropertyField(windDown, "Wind Down Time", "Time in seconds after the attack stops but the animation is still playing.");
        PropertyField(cooldown, "Cool Down Time", "Time in seconds after the attack is completed before it can be activated again.");
        GUI.enabled = !melee.boolValue;
        PropertyField(Attack_Prefab, "Attack Prefab", "Time in seconds after the attack is completed before it can be activated again.");
        GUI.enabled = true;
        PropertyField(hitInfo, "Hit Info", "Time in seconds after the attack is completed before it can be activated again.", true);
        PropertyField(hitboxTime, "Hitbox Duration", "Time in seconds after the attack is completed before it can be activated again.");
        DrawVector(offset, "Hitbox Offset", "Hitbox local offset from character when created.");
        //GUI.enabled = !projectile.boolValue;
        //PropertyField(parented, "Parented", "Is the hitbox parented to the character's transform?");
        //GUI.enabled = true;
        PropertyField(hitNumber, "Hit Number", "How many hits the hitbox can land. Usaully 1.");
        PropertyField(oneShot, "One Shot", "Should the hitbox disappear after landing one hit?");

        EditorGUI.indentLevel--;

        #endregion

        #region Melee

        EditorGUILayout.LabelField("Melee", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        melee.boolValue = EditorGUILayout.Toggle(new GUIContent("Melee", "Should the attack be a melee?"), melee.boolValue);
        if (melee.boolValue)
        {
            DrawVector(meleeSize, "Size", "Hitbox size.");
        }
        EditorGUI.indentLevel--;

        #endregion

        #region Magic

        EditorGUILayout.LabelField("Magic", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        magic.boolValue = EditorGUILayout.Toggle(new GUIContent("Magic", "Does the attack require magic?"), magic.boolValue);
        if (magic.boolValue)
        {
            PropertyField(magicRequired, "Magic Required", "How much magic is needed to perform the attack.");
        }
        EditorGUI.indentLevel--;

        #endregion

        #region Projectile

        EditorGUILayout.LabelField("Projectile", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        projectile.boolValue = EditorGUILayout.Toggle(new GUIContent("Projectile", "Does this attack get shot?"), projectile.boolValue);
        if (projectile.boolValue)
        {
            DrawVector(projectileVector, "Direction", "Vector the hitbox is shot from.");
            PropertyField(projectileSpeed, "Speed", "How fast the projectile flies.");
        }
        EditorGUI.indentLevel--;

        #endregion

        //base.OnInspectorGUI();
        myObject.ApplyModifiedProperties();
    }

    #endregion

    #region Draw Methods

    private void DrawVector(SerializedProperty property, string label, string tooltip)
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel(new GUIContent(label, (showTooltips.boolValue ? tooltip : "")));
            PropertyField(property, "", "");
        }
        EditorGUILayout.EndHorizontal();
    }

    private void PropertyField(SerializedProperty property, string label, string tooltip, bool showChildren = false)
    {
        EditorGUILayout.PropertyField(property, new GUIContent(label, (showTooltips.boolValue ? tooltip : "")), showChildren, GUILayout.MinWidth(0));
    }

    #endregion
}