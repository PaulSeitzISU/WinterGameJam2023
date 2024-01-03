using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject objectToSpawn; // Reference to the GameObject to spawn
    public float moveDuration = 1.0f; // Duration in seconds for the object to move
    public float destroyDelay = 1.0f; // Time delay before destroying the object

    public void FireProjectile(GameObject targetObj, int damage)
    {
        StartCoroutine(MoveObject(targetObj, damage));
    }

    IEnumerator MoveObject(GameObject targetObj, int damage)
    {
        GameObject spawnedObject = Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        float elapsedTime = 0f;
        Transform targetPosition = targetObj.transform;

        // Rotate the spawned object to face the target
        spawnedObject.transform.LookAt(targetPosition);
        spawnedObject.transform.Rotate(0, 90, 0);

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

        // Deal damage to the target object
        if (targetObj.TryGetComponent(out Health targetHealth))
        {
            targetHealth.TakeDamageAndCheckIfDead(damage);
        }

        // Wait for a delay before destroying the object
        yield return new WaitForSeconds(destroyDelay);

        // Destroy the spawned object
        Destroy(spawnedObject);
    }
}
