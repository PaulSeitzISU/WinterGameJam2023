using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject objectToSpawn; // Reference to the GameObject to spawn
    Transform targetPosition; // Target position where the object will fly
    public float moveDuration = 1.0f; // Duration in seconds for the object to move
    public float destroyDelay = 1.0f; // Time delay before destroying the object

    private GameObject spawnedObject; // Reference to the spawned object
    private float elapsedTime = 0f; // Elapsed time for lerping

    public void FireProjectile(GameObject obj, int damage)
    {
        targetPosition = obj.transform;
        // Spawn the object at the initial position
        spawnedObject = Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        elapsedTime = 0f;
        // Start moving the object towards the target position
        StartCoroutine(MoveObject(obj, damage));
    }

    IEnumerator MoveObject(GameObject obj,int damage )
    {
        //angle it at obj
        spawnedObject.transform.LookAt(obj.transform);

        Vector3 startPosition = spawnedObject.transform.position;
        float startTime = Time.time;

        while (elapsedTime < moveDuration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            spawnedObject.transform.position = Vector3.Lerp(startPosition, targetPosition.position, t);
            yield return null;
        }

        // Ensure the object reaches the target position precisely
        spawnedObject.transform.position = targetPosition.position;
        obj.GetComponent<Health>().TakeDamageAndCheckIfDead(damage);
        // Wait for a delay before destroying the object
        yield return new WaitForSeconds(destroyDelay);

        // Destroy the spawned object
        Destroy(spawnedObject);
    }
}
