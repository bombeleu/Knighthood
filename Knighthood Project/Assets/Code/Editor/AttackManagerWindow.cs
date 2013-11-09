// Steve Yeager
// 11.5.2013

using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class AttackManagerWindow : EditorWindow
{
    #region References

    private AttackManager myManager;
    private SerializedObject mySO;
    private static Camera PreviewCamera;

    #endregion
    
    #region Private Fields

    private bool attacksToggle;
    private Vector2 attackScroll = new Vector2();
    private int opened = -1;
    private int locked = -1;
    private int moveIndex = -1;
    private int confirmDelete = -1;

    #endregion

    #region Static Fields

    private static Vector2 windowSize = new Vector2(1000, 600);
    private static Rect attacksRect = new Rect(0, 0, 400, 250);
    private static Rect previewRect = new Rect(25, 550, 450, 250);
    private static Rect lockedRect = new Rect(500, 0, 250, 600);
    private static Rect openedRect = new Rect(750, 0, 250, 600);

    private static Vector3 previewPosition = new Vector3(0, 0, -1000f);

    #endregion


    #region EditorWindow Overrides

    [MenuItem("Tools/Attack Manager %A")]
    private static void Init()
    {
        var window = (AttackManagerWindow)GetWindow(typeof(AttackManagerWindow), false, "Attacks");
        window.maxSize = windowSize;
        window.minSize = windowSize;
        var camera = new GameObject("Attack Preview");
        camera.transform.position = previewPosition;
        PreviewCamera = camera.AddComponent<Camera>();
        PreviewCamera.isOrthoGraphic = true;
    }


    [MenuItem("Tools/Attack Manager %A", true)]
    private static bool Validator()
    {
        return true;
    }


    private void OnDestroy()
    {
        DestroyImmediate(PreviewCamera.gameObject, true);
    }


    private void OnEnable()
    {
        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<UltimateAttackManager>() != null)
        {
            SelectCharacter(Selection.activeGameObject.GetComponent<AttackManager>());
        }
        else
        {
            myManager = null;
        }
        Repaint();
    }


    private void OnSelectionChange()
    {
        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<UltimateAttackManager>() != null)
        {
            SelectCharacter(Selection.activeGameObject.GetComponent<AttackManager>());
        }
        else
        {
            myManager = null;
        }
        Repaint();
    }


    private void OnGUI()
    {
        if (myManager == null)
        {
            GUILayout.Label("Need to select a Character.");
            return;
        }

        mySO.Update();

        // attacks
        var toggleRect = new Rect(attacksRect);
        toggleRect.height = 20;
        attacksToggle = EditorGUI.Foldout(toggleRect, attacksToggle, new GUIContent("Attacks"));
        var scrollPosRect = new Rect(0, 20, attacksRect.width, attacksRect.height - 100);
        var scrollViewRect = new Rect(0, 0, attacksRect.width - 16, 20 * myManager.attacks.Length);
        if (attacksToggle)
        {
            EditorGUI.indentLevel++;
            attackScroll = GUI.BeginScrollView(scrollPosRect, attackScroll, scrollViewRect);
            {
                for (int i = 0; i < myManager.attacks.Length; i++)
                {
                    myManager.inputs[i] = EditorGUI.TextField(new Rect(0, i*20, 150, 20), myManager.inputs[i]);
                    myManager.attacks[i].attackName = EditorGUI.TextField(new Rect(142, i*20, 150, 20), myManager.attacks[i].attackName);
                    if (confirmDelete == i)
                    {
                        if (GUI.Button(new Rect(294, i * 20, 45, 20), "D"))
                        {
                            DeleteAttack(i);
                            confirmDelete = -1;
                            return;
                        }
                        if (GUI.Button(new Rect(339, i * 20, 45, 20), "C"))
                        {
                            confirmDelete = -1;
                        }
                    }
                    else
                    {
                        // move
                        Rect moveRect = new Rect(294, i * 20, 30, 20);
                        if (moveIndex == i)
                        {
                            if (GUI.Button(moveRect, "X"))
                            {
                                moveIndex = -1;
                            }
                        }
                        else if (moveIndex != -1)
                        {
                            if (GUI.Button(moveRect, "T"))
                            {
                                if (opened == moveIndex) opened = i;
                                else if (opened == i) opened = moveIndex;
                                if (locked == moveIndex) locked = i;
                                else if (locked == i) locked = moveIndex;

                                mySO.FindProperty("attacks").MoveArrayElement(moveIndex, i);
                                mySO.FindProperty("inputs").MoveArrayElement(moveIndex, i);
                                mySO.ApplyModifiedProperties();
                                moveIndex = -1;
                                return;
                            }
                        }
                        else
                        {
                            if (GUI.Button(moveRect, "M"))
                            {
                                moveIndex = i;
                            }
                        }
                        // delete
                        if (GUI.Button(new Rect(324, i * 20, 30, 20), "-"))
                        {
                            confirmDelete = i;
                        }
                        // open
                        if (GUI.Button(new Rect(354, i * 20, 30, 20), opened == i ? "|" : (locked == i ? "L" : "O")))
                        {
                            ToggleAttack(i);
                        }
                    }
                }
            }
            GUI.EndScrollView();

            // create new attack
            Rect createRect = new Rect(142, scrollViewRect.height + 30, 100, 30);
            if (createRect.y > scrollPosRect.height + 30)
            {
                createRect.y = scrollPosRect.height + 30;
            }
            if (GUI.Button(createRect, "Create Attack"))
            {
                CreateAttack();
                return;
            }

            EditorGUI.indentLevel--;
        }

        // locked attack
        GUI.Box(lockedRect, "");
        if (locked != -1)
        {
            GUI.BeginGroup(lockedRect);
            {
                DrawAttack(lockedRect, myManager.attacks[locked], locked);
            }
            GUI.EndGroup();
        }

        // opened attack
        GUI.Box(openedRect, "");
        if (opened != -1)
        {
            GUI.BeginGroup(openedRect);
            {
                DrawAttack(openedRect, myManager.attacks[opened], opened);
            }
            GUI.EndGroup();
        }

        // preview
        Handles.DrawCamera(previewRect, PreviewCamera);

        mySO.ApplyModifiedProperties();
    }

    #endregion

    #region Public Methods

    public void SelectCharacter(AttackManager attackManager)
    {
        myManager = attackManager;
        mySO = new SerializedObject(attackManager);
        title = myManager.name + " Attacks";

        Repaint();
    }

    #endregion

    #region Private Methods

    private void DrawAttack(Rect rect, Attack attack, int index)
    {
        bool isLocked = locked == index;
        EditorGUI.LabelField(new Rect(10, 10, rect.width-60, 20), myManager.inputs[index] + " - " + attack.attackName, EditorStyles.boldLabel);
        // close
        if (!isLocked)
        if (GUI.Button(new Rect(rect.width - 51, 10, 20, 20), "|"))
        {
            opened = -1;
        }
        // locked
        if (GUI.Button(new Rect(rect.width - 30, 10, 20, 20), isLocked ? "U" : "L"))
        {
            if (isLocked)
            {
                locked = -1;
            }
            else
            {
                locked = index;
                opened = -1;
            }
        }

        // attack
        #region Serialized Fields

        SerializedObject attackSO = new SerializedObject(attack);
        attackSO.Update();
        SerializedProperty attackAnimation = attackSO.FindProperty("attackAnimation");
        SerializedProperty Attack_Prefab = attackSO.FindProperty("Attack_Prefab");
        SerializedProperty windUp = attackSO.FindProperty("windUp");
        SerializedProperty attackTime = attackSO.FindProperty("attackTime");
        SerializedProperty windDown = attackSO.FindProperty("windDown");
        SerializedProperty cooldown = attackSO.FindProperty("cooldown");
        SerializedProperty hitInfo = attackSO.FindProperty("hitInfo");
        SerializedProperty hitboxTime = attackSO.FindProperty("hitboxTime");
        SerializedProperty offset = attackSO.FindProperty("offset");
        SerializedProperty hitNumber = attackSO.FindProperty("hitNumber");
        SerializedProperty oneShot = attackSO.FindProperty("oneShot");
        SerializedProperty melee = attackSO.FindProperty("melee");
        SerializedProperty meleeSize = attackSO.FindProperty("meleeSize");
        SerializedProperty magic = attackSO.FindProperty("magic");
        SerializedProperty magicRequired = attackSO.FindProperty("magicRequired");
        SerializedProperty projectile = attackSO.FindProperty("projectile");
        SerializedProperty projectileAngle = attackSO.FindProperty("projectileAngle");
        SerializedProperty projectileSpeed = attackSO.FindProperty("projectileSpeed");

        #endregion

        #region GUI
        
        // animation
        EditorGUI.PropertyField(new Rect(10, 42, rect.width-20, 20), attackAnimation, new GUIContent("Animation", "Animation to play play during attack."));
        // attack prefab
        GUI.enabled = !melee.boolValue;
        EditorGUI.PropertyField(new Rect(10, 64, rect.width - 20, 20), Attack_Prefab, new GUIContent("Attack Prefab", "Prefab of the attack."));
        GUI.enabled = true;
        // windup
        EditorGUI.PropertyField(new Rect(10, 86, rect.width - 20, 20), windUp, new GUIContent("Windup Time", "Time between animation start and the hitting attack."));
        // attack time
        EditorGUI.PropertyField(new Rect(10, 108, rect.width - 20, 20), attackTime, new GUIContent("Attack Time", "Duration of the hitting attack."));
        // winddown
        EditorGUI.PropertyField(new Rect(10, 130, rect.width - 20, 20), windDown, new GUIContent("Winddown Time", "Time after the hitting attack has finished and the attack is done."));
        // cooldown
        EditorGUI.PropertyField(new Rect(10, 152, rect.width - 20, 20), cooldown, new GUIContent("Cooldown Time", "Time after the attack is over before it can be activated again. Usually zero."));
        // hitinfo
        EditorGUI.LabelField(new Rect(10, 174, rect.width - 20, 20), new GUIContent("HitInfo", "Attack info passed to hitbox and then reciever."));
        EditorGUI.indentLevel++;
        EditorGUI.PropertyField(new Rect(10, 196, rect.width - 20, 20), hitInfo.FindPropertyRelative("damage"), new GUIContent("Damage", "Base damage for attack."));
        EditorGUI.PropertyField(new Rect(10, 218, rect.width - 20, 20), hitInfo.FindPropertyRelative("effect"), new GUIContent("Effect", "Attack's status effect."));
        EditorGUI.LabelField(new Rect(10, 240, 80, 20), new GUIContent("Knockback", "Direction and magnitude of attack's knockback."));
        EditorGUI.LabelField(new Rect(125, 240, 20, 20), new GUIContent("X"));
        EditorGUI.PropertyField(new Rect(145, 240, 30, 20), hitInfo.FindPropertyRelative("knockBack").FindPropertyRelative("z"), GUIContent.none);
        EditorGUI.LabelField(new Rect(180, 240, 20, 20), new GUIContent("Y"));
        EditorGUI.PropertyField(new Rect(200, 240, 30, 20), hitInfo.FindPropertyRelative("knockBack").FindPropertyRelative("y"), GUIContent.none);
        EditorGUI.indentLevel--;
        // cooldown
        EditorGUI.PropertyField(new Rect(10, 262, rect.width - 20, 20), hitboxTime, new GUIContent("Hitbox Duration", "Duration of the hitbox."));
        // offset
        EditorGUI.LabelField(new Rect(10, 284, 80, 20), new GUIContent("Hitbox Offset", "Placement of the hitbox relative to the Character pivot."));
        EditorGUI.LabelField(new Rect(125, 284, 20, 20), new GUIContent("X"));
        EditorGUI.PropertyField(new Rect(145, 284, 30, 20), offset.FindPropertyRelative("x"), GUIContent.none);
        EditorGUI.LabelField(new Rect(180, 284, 20, 20), new GUIContent("Y"));
        EditorGUI.PropertyField(new Rect(200, 284, 30, 20), offset.FindPropertyRelative("y"), GUIContent.none);
        // hit number
        GUI.enabled = !oneShot.boolValue;
        EditorGUI.PropertyField(new Rect(10, 306, rect.width - 20, 20), hitNumber, new GUIContent("Hit Number", "Possible number of hits. Usually one."));
        GUI.enabled = true;
        // one shot
        EditorGUI.PropertyField(new Rect(10, 328, rect.width - 20, 20), oneShot, new GUIContent("One Shot", "Should the hitbox disappear after landing one hit?"));
        // melee
        EditorGUI.PropertyField(new Rect(10, 350, rect.width - 20, 20), melee, new GUIContent("Melee", "Does the attack use a melee hitbox?"));
        float meleeHeight = melee.boolValue ? 22 : 0f;
        if (melee.boolValue)
        {
            EditorGUI.indentLevel++;
            // melee size
            EditorGUI.LabelField(new Rect(10, 372, 80, 20), new GUIContent("Melee Size", "How big the melee hitbox is."));
            EditorGUI.LabelField(new Rect(125, 372, 20, 20), new GUIContent("X"));
            EditorGUI.PropertyField(new Rect(145, 372, 30, 20), meleeSize.FindPropertyRelative("x"), GUIContent.none);
            EditorGUI.LabelField(new Rect(180, 372, 20, 20), new GUIContent("Y"));
            EditorGUI.PropertyField(new Rect(200, 372, 30, 20), meleeSize.FindPropertyRelative("y"), GUIContent.none);
            EditorGUI.indentLevel--;
        }
        // magic
        EditorGUI.PropertyField(new Rect(10, 372 + meleeHeight, rect.width - 20, 20), magic, new GUIContent("Magic", "Does the attack require magic?"));
        float magicHeight = magic.boolValue ? 22 : 0f;
        if (magic.boolValue)
        {
            EditorGUI.indentLevel++;
            // magic required
            EditorGUI.PropertyField(new Rect(10, 394 + meleeHeight, rect.width - 20, 20), magicRequired, new GUIContent("Magic Required", "How much magic is needed to perform the attack."));
            EditorGUI.indentLevel--;
        }
        // projectile
        EditorGUI.PropertyField(new Rect(10, 394 + meleeHeight + magicHeight, rect.width - 20, 20), projectile, new GUIContent("Projectile", "Is this attack a projectile?"));
        if (projectile.boolValue)
        {
            EditorGUI.indentLevel++;
            // angle
            EditorGUI.PropertyField(new Rect(10, 416 + meleeHeight + magicHeight, rect.width - 20, 20), projectileAngle, new GUIContent("Angle", "Angle of projectile."));
            // speed
            EditorGUI.PropertyField(new Rect(10, 438 + meleeHeight + magicHeight, rect.width - 20, 20), projectileSpeed, new GUIContent("Speed", "Speed of projectile."));
            EditorGUI.indentLevel--;
        }

        attackSO.ApplyModifiedProperties();

        #endregion
    }


    private void DeleteAttack(int attack)
    {
        if (opened == attack)
        {
            opened = -1;
        }
        else if (opened > attack)
        {
            opened--;
        }

        if (locked == attack)
        {
            locked = -1;
        }
        else if (locked > attack)
        {
            locked--;
        }

        DestroyImmediate(myManager.attacks[attack], true);
        mySO.FindProperty("attacks").DeleteArrayElementAtIndex(attack);
        mySO.FindProperty("attacks").DeleteArrayElementAtIndex(attack);
        mySO.FindProperty("inputs").DeleteArrayElementAtIndex(attack);
        mySO.ApplyModifiedProperties();
        Repaint();
    }


    private void CreateAttack()
    {
        int index = myManager.attacks.Length;

        mySO.FindProperty("attacks").InsertArrayElementAtIndex(index);
        mySO.FindProperty("inputs").InsertArrayElementAtIndex(index);
        mySO.ApplyModifiedProperties();

        myManager.attacks[index] = myManager.gameObject.AddComponent("GenericAttack") as Attack;
        myManager.attacks[index].attackName = "";
        myManager.attacks[index].hideFlags = HideFlags.HideInInspector;
        myManager.inputs[index] = "";
        ToggleAttack(index);

        Repaint();
    }


    private void ToggleAttack(int index)
    {
        if (opened == index)
        {
            opened = -1;
        }
        else if (locked != index)
        {
            opened = index;
        }
        else if (locked == index)
        {
            locked = -1;
        }
    }

    #endregion
}