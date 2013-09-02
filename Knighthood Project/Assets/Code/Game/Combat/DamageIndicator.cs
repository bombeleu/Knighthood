// Steve Yeager
// 8.7.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(TextMesh))]
public class DamageIndicator : BaseMono
{
    #region References

    private Transform myTransform;
    private Transform parent;

    #endregion

    #region Display Fields

    /// <summary>How long to stay visible.</summary>
    public float time;
    /// <summary>How fast to move upwards.</summary>
    public float speed;
    /// <summary>How far above the parent's origion to start.</summary>
    private const float startHeight = 1.5f;
    //
    private float currentHeight = 0f;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // get references
        myTransform = transform;
    } // end Awake


    private void Update()
    {
        currentHeight += speed * Time.deltaTime;
        myTransform.position = parent.position + Vector3.up * (startHeight + currentHeight);
    } // end Update

    #endregion

    #region Indicator Methods

    /// <summary>
    /// Set position, text, and color.
    /// </summary>
    /// <param name="parent">Set transform to parent.</param>
    /// <param name="damage">Damage for text.</param>
    public void Initiate(Transform parent, int damage)
    {
        myTransform.position = parent.position + Vector3.up * startHeight;
        this.parent = parent;
        currentHeight = 0f;

        GetComponent<TextMesh>().text = Mathf.Abs(damage).ToString();
        if (damage < 0)
        {
            GetComponent<TextMesh>().color = Color.green;
        }
        else
        {
            GetComponent<TextMesh>().color = Color.red;
        }

        StartCoroutine(KillTimer());
    } // end Activate


    private IEnumerator KillTimer()
    {
        yield return WaitForTime(time);
        gameObject.SetActive(false);
    } // end KillTimer

    #endregion

} // end DamageIndicator class