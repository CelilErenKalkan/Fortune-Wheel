using System.Collections.Generic;
using UnityEngine;

public static class Library
{
    /// <summary>
    /// Destroys wanted component inside children
    /// </summary>
    /// <typeparam name="T">Destroyed Component</typeparam>
    /// <param name="parentTrans"></param>
    public static void DestroyAllChildWithT<T>(this Transform parentTrans) where T : Component
    {
        T[] components = parentTrans.GetComponentsInChildren<T>();

        foreach (T component in components)
        {
            Object.Destroy(component.gameObject);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parentTrans"></param>
    /// <param name="activity"></param>
    public static void ChangeActivityAllListOfGameObjects(this List<GameObject> parentTrans, bool activity)
    {
        foreach (GameObject go in parentTrans)
        {
            go.SetActive(activity);
        }
    }

    ///For ragdoll character explosion

    #region Character Explosion

    /// <summary>
    /// Close/Open all child rigidbody's and adds force / mostly for ragdoll
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="activity">is kinematic activity</param>
    /// <param name="xforce"></param>
    /// <param name="yforce"></param>
    /// <param name="zforce"></param>
    /// <param name="isForced">is force applies?</param>
    public static void ChangeActiveRBAllChildren(this Transform parent, bool activity, float xforce, float yforce,
        float zforce, bool isForced = true)
    {
        Rigidbody[] rbs = parent.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = activity;
            if (isForced) rb.AddForce(xforce, yforce, zforce);
        }
    }

    /// <summary>
    /// Close/Open all child colliders / mostly for ragdoll
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="activity"></param>
    public static void ChangeActivityAllColliderInChildren(this Transform parent, bool activity)
    {
        Collider[] colliders = parent.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = activity;
        }
    }

    /// <summary>
    /// Ragdoll transform explodes
    /// </summary>
    /// <param name="explodedCharacter"></param>
    /// <param name="explosionMultiplier">Vector of explosion (direction and magnitude) </param>
    public static void CharacterExplosion(this Transform explodedCharacter, Vector3 explosionForce,
        bool isForced = true)
    {
        ChangeActiveRBAllChildren(explodedCharacter.transform, false, explosionForce.x, explosionForce.z,
            explosionForce.z, isForced);
        ChangeActivityAllColliderInChildren(explodedCharacter, true);
        //explodedCharacter.transform.GetComponent<Animator>().enabled = false;
        // explodedCharacter.transform.parent = null;
        foreach (Collider col in explodedCharacter.GetComponents<Collider>())
            col.enabled = false;
    }

    /// <summary>
    /// Direction of two transform
    /// </summary>
    /// <param name="startTrans"></param>
    /// <param name="endTrans"></param>
    /// <returns></returns>
    public static Vector3 DirectionVector(this Transform startTrans, Transform endTrans)
    {
        return endTrans.position - startTrans.position;
    }

    #endregion

    /// <summary>
    /// Direction of two vector
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    public static Vector3 DirectionVector(this Vector3 startPoint, Vector3 endPoint)
    {
        return endPoint - startPoint;
    }

    /// <summary>
    /// Creates objects and sets  transform position
    /// </summary>
    /// <param name="createdGO"></param>
    /// <param name="trans"></param>
    public static void CreateGameObjectandPlaceIt(this GameObject prefab, Transform trans)
    {
        GameObject go = Object.Instantiate(prefab);

        go.transform.position = trans.position;
    }

    /// <summary>
    /// Creates objects and sets  transform position
    /// </summary>
    /// <param name="createdGO"></param>
    /// <param name="trans"></param>
    public static void CreateGameObjectandPlaceIt(this GameObject prefab, Vector3 position)
    {
        GameObject go = Object.Instantiate(prefab);

        go.transform.position = position;
    }

    /// <summary>
    /// Creates object and sets rotation and position
    /// </summary>
    /// <param name="createdGO"></param>
    /// <param name="trans"></param>
    /// <param name="rotation"></param>
    public static void CreateGameObjectandPlaceIt(this GameObject prefab, Transform trans, Vector3 rotation)
    {
        GameObject go = Object.Instantiate(prefab);

        go.transform.position = trans.position;
        go.transform.eulerAngles = rotation;
    }

    /// <summary>
    /// Resets all TRIGGERS
    /// </summary>
    /// <param name="animator"></param>
    public static void ResetAllAnimatorTriggers(this Animator animator)
    {
        foreach (var trigger in animator.parameters)
        {
            if (trigger.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(trigger.name);
            }
        }
    }

    public static Dictionary<float, WaitForSeconds> waitList;

    /// <summary>
    /// Store wfs for next usage for better optimization
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static WaitForSeconds GetWait(this float time)
    {
        if (waitList == null) waitList = new Dictionary<float, WaitForSeconds>();
        if (!waitList.ContainsKey(time))
        {
            WaitForSeconds waitTime = new WaitForSeconds(time);
            waitList.Add(time, waitTime);
            return waitTime;
        }
        else
        {
            return waitList[time];
        }
    }

    /// <summary>
    /// Destroys object
    /// </summary>
    /// <param name="go"></param>
    public static void Destroy(this GameObject go)
    {
        Object.Destroy(go);
    }


    /// <summary>
    /// Object moves one point to another with a speed. This function gives
    /// how much time consumed while moving.
    /// </summary>
    /// <param name="startingTransform"></param>
    /// <param name="endTransform"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static float GetMovingTime(this Transform startingTransform, Transform endTransform, float speed)
    {
        return Vector3.Distance(startingTransform.position, endTransform.position) / speed;
    }

    /// <summary>
    /// Object moves one point to another with a speed. This function gives
    /// how much time consumed while moving.
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static float GetMovingTime(this Vector3 startPoint, Vector3 endPoint, float speed)
    {
        return Vector3.Distance(startPoint, endPoint) / speed;
    }

    public static Vector3 ChangeX(this Vector3 changingVector, float Xinput)
    {
        return new Vector3(Xinput, changingVector.y, changingVector.z);
    }

    public static Vector3 ChangeY(this Vector3 changingVector, float Yinput)
    {
        return new Vector3(changingVector.x, Yinput, changingVector.z);
    }

    public static Vector3 ChangeZ(this Vector3 changingVector, float Zinput)
    {
        return new Vector3(changingVector.x, changingVector.y, Zinput);
    }

    public static Vector3 ChangeXY(this Vector3 changingVector, float Xinput, float Yinput)
    {
        return new Vector3(Xinput, Yinput, changingVector.z);
    }

    public static Vector3 ChangeXZ(this Vector3 changingVector, float Xinput, float Zinput)
    {
        return new Vector3(Xinput, changingVector.y, Zinput);
    }

    public static Vector3 ChangeYZ(this Vector3 changingVector, float Yinput, float Zinput)
    {
        return new Vector3(changingVector.x, Yinput, Zinput);
    }
}