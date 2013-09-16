// Steve Yeager
// 8.7.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Text that appears above an object that just recieved damage/health indicating the amount.
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
    private const float StartHeight = 1.5f;
    //
    private float currentHeight;

    #endregion


    #region MonoBehaviour Overrides

    private void Awake()
    {
        // get references
        myTransform = transform;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set position, text, and color.
    /// </summary>
    /// <param name="parent">Set transform to parent.</param>
    /// <param name="damage">Damage for text.</param>
    public void Initiate(Transform parent, int damage)
    {
        myTransform.position = parent.position + Vector3.up * StartHeight;
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

        InvokeMethod("Kill", time);
        StartCoroutine("Float");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Disable the indicator.
    /// </summary>
    private void Kill()
    {
        StopCoroutine("Float");
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Move up.
    /// </summary>
    private IEnumerator Float()
    {
        while (true)
        {
            currentHeight += speed*Time.deltaTime;
            myTransform.position = parent.position + Vector3.up*(StartHeight + currentHeight);
            yield return null;
        }
    }

    #endregion
}