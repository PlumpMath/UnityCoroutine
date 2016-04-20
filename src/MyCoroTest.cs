using System;
using System.Collections;

using UnityEngine;

public class MyCoroTest : MyMonoBehaviour
{
    void Start()
    {
        MyStartCoroutine(MyCoro());
    }

    IEnumerator MyCoro()
    {
        Debug.Log("MyCoro()");

        yield return new MyWaitForSeconds(3);
        Debug.Log("After 3secs");
        yield return new MyWaitForSeconds(1);
        Debug.Log("After 1secs");
        yield break;

        Debug.Log("After break");

        yield return new MyWaitForSeconds(1);
    }
}
