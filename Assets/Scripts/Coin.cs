using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float Speed = 2f; // Speed of a coin traversing space
    public Transform StartingTransform; // Position where coins fly to after being moved or spawned
    public Transform HiddenTransform; // this is some position far away from camera where the coins can be stacked
    public float Delay;
    public bool Check_To_Move_Uncheck_To_Destroy;
    protected bool _stopMoving;
    public bool InitiateTurningOff;
    public Vector3 coinInstantiatePosition;

    void Start()
    {
        coinInstantiatePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(InitiateTurningOff)
        {
            InitiateTurningOff = false;
            StartDelayedDestroy();
        }
        if(!_stopMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, StartingTransform.position, Speed * Time.deltaTime);
        }/*
        else
        {
            gameObject.SetActive(false);
        }*/
    }


    void StartDelayedDestroy()
    {
        StartCoroutine(DelayedDestroy());
    }
    void StartDelayedMoveAway()
    {
        StartCoroutine(DelayedMoveAway());
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(Delay);
        gameObject.SetActive(false);
    }
    /// <summary>
    /// Moves the object to a default HiddenTransform position that can be changed
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator DelayedMoveAway()
    {
        yield return new WaitForSeconds(Delay);

        _stopMoving = true;
        transform.position = HiddenTransform.position;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Moves the object to a specified transform position
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <returns></returns>
    protected virtual IEnumerator DelayedMoveAway(Transform targetTransform)
    {
        
        yield return new WaitForSeconds(Delay);
        _stopMoving = true;
        transform.position = targetTransform.position;
        gameObject.SetActive(false);
    }


    /// <summary>
    ///  Moves the object to a default StartingTransform position that can be changed
    /// </summary>
    /// /// <param name="delay"></param>
    /// <returns></returns>
    protected virtual IEnumerator DelayedMoveBack(float delay = -1)
    {

        yield return delay != -1 ? new WaitForSeconds(delay) : new WaitForSeconds(Delay);

        /*  Next 10 or so lines are just a little more descriptive showcase of a statement from above
         *
          if (delay != -1)
        {
            yield return new WaitForSeconds(delay);
        }
        else
        {
            yield return new WaitForSeconds(Delay);
        }
         *
         */
        _stopMoving = false;
        transform.position = StartingTransform.position;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Moves the object back to a specified transform position
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <returns></returns>
    protected virtual IEnumerator DelayedMoveBack(Transform targetTransform, float delay = -1)
    {
        yield return delay != -1 ? new WaitForSeconds(delay) : new WaitForSeconds(Delay);
        _stopMoving = false;
        transform.position = targetTransform.position;
    }
}
