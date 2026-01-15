using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AmbushManager : MonoBehaviour
{
    [SerializeField] List<Transform> rangedEnemySpawnPosList = new List<Transform>();
    [SerializeField] List<Transform> meleeEnemySpawnPosList = new List<Transform>();
    [SerializeField] List<Transform> meleeTweak1EnemySpawnPosList = new List<Transform>();
    [SerializeField] List<Transform> meleeTweak2EnemySpawnPosList = new List<Transform>();
    [SerializeField] List<GameObject> currentAmbushEnemies = new List<GameObject>();
    [SerializeField] CinemachineCamera cinCamera;
    [SerializeField] RectTransform ambushWarningImage;
    [SerializeField] RectTransform ambushClearedImage;
    [SerializeField] GameObject clearedSignPrefab;
    [SerializeField] Camera mainCamera;
    [SerializeField] float dropDistance = 200f;
    [SerializeField] float animationTime = 1f;
    [SerializeField] float meleeTweak1DelayTime = 5f;
    [SerializeField] float meleeTweak2DelayTime = 10f;
    Transform playerTransform;

    public static AmbushManager instance;

    bool isCameraFrozen = false;
    bool ambushInProgress = false;

    [SerializeField] RangedEnemy rangedEnemyPrefab;
    [SerializeField] PathfindingEnemyController meleeEnemyPrefab;
    GameObject[] rangedEnemySpawnPos;
    GameObject[] meleeEnemySpawnPos;
    GameObject[] meleeTweak1EnemySpawnPos;
    GameObject[] meleeTweak2EnemySpawnPos;

    private void Awake()
    {
        rangedEnemySpawnPos = GameObject.FindGameObjectsWithTag("AmbushRanged");
        meleeEnemySpawnPos = GameObject.FindGameObjectsWithTag("AmbushMelee");
        meleeTweak1EnemySpawnPos = GameObject.FindGameObjectsWithTag("AmbushMeleeTweak1");
        meleeTweak2EnemySpawnPos = GameObject.FindGameObjectsWithTag("AmbushMeleeTweak2");
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        instance = this;
    }
    private void Update()
    {
        if (isCameraFrozen)
        {
            RestrictPlayerMovement();
        }
        if (ambushInProgress && IsAmbushCleared())
        {
            AmbusCleared();
        }
        

    }

    public void Ambush()
    {
        StartCoroutine(AnimateDrop(ambushWarningImage));
        for (int i = 0; i < rangedEnemySpawnPos.Length; i++)
        {
            float dis = Vector3.Distance(rangedEnemySpawnPos[i].transform.position, playerTransform.position);
            if (dis < 30)
            {
                rangedEnemySpawnPosList.Add(rangedEnemySpawnPos[i].transform);
            }
        }
        for (int i = 0; i < meleeEnemySpawnPos.Length; i++)
        {
            float dis = Vector3.Distance(meleeEnemySpawnPos[i].transform.position, playerTransform.position);
            if (dis < 30)
            {
                meleeEnemySpawnPosList.Add(meleeEnemySpawnPos[i].transform);
            }
        }
        for (int i = 0; i < meleeTweak1EnemySpawnPos.Length; i++)
        {
            float dis = Vector3.Distance(meleeTweak1EnemySpawnPos[i].transform.position, playerTransform.position);
            if (dis < 30)
            {
                meleeTweak1EnemySpawnPosList.Add(meleeTweak1EnemySpawnPos[i].transform);
            }
        }
        for (int i = 0; i < meleeTweak2EnemySpawnPos.Length; i++)
        {
            float dis = Vector3.Distance(meleeTweak2EnemySpawnPos[i].transform.position, playerTransform.position);
            if (dis < 30)
            {
                meleeTweak2EnemySpawnPosList.Add(meleeTweak2EnemySpawnPos[i].transform);
            }
        }
        foreach (Transform t in rangedEnemySpawnPosList)
        {
            Vector3 offset = new Vector3(0, 10, 0);
            RangedEnemy rangedEnemy = Instantiate(rangedEnemyPrefab, t.position + offset, Quaternion.identity);
            currentAmbushEnemies.Add(rangedEnemy.gameObject);
        }
        foreach (Transform t in meleeEnemySpawnPosList)
        {
            var meleeEnemy = Instantiate(meleeEnemyPrefab, t.position, Quaternion.identity);
            currentAmbushEnemies.Add(meleeEnemy.gameObject);
        }
        StartCoroutine(SpawnMeleeTweak1AfterDelay());
        StartCoroutine(SpawnMeleeTweak2AfterDelay());

        isCameraFrozen = true;
        cinCamera.Follow = null;
        cinCamera.transform.position = new Vector3(cinCamera.transform.position.x, 2, cinCamera.transform.position.z);
        ambushInProgress = true;
        rangedEnemySpawnPosList.Clear();
        meleeEnemySpawnPosList.Clear();
    }
    private void RestrictPlayerMovement()
    {
        Vector3 playerPosition = playerTransform.position;
        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;

        float clampedX = Mathf.Clamp(playerPosition.x, mainCamera.transform.position.x - cameraHalfWidth + 11, mainCamera.transform.position.x + cameraHalfWidth - 7);
        playerTransform.position = new Vector3(clampedX, playerPosition.y, 0);

    }
    void AmbusCleared()
    {
        cinCamera.Follow = playerTransform;
        isCameraFrozen = false;
        ambushInProgress = false;
        StartCoroutine(AnimateDrop(ambushClearedImage));
        SpawnClearedSign();
    }
    private IEnumerator AnimateDrop(RectTransform aImage)
    {
        Vector2 originalPos = aImage.anchoredPosition;
        Vector2 targetPos = originalPos + new Vector2(0, -dropDistance);

        float elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            aImage.anchoredPosition = Vector2.Lerp(originalPos, targetPos, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        aImage.anchoredPosition = targetPos;

        yield return new WaitForSeconds(0.5f);

        elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            aImage.anchoredPosition = Vector2.Lerp(targetPos, originalPos, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        aImage.anchoredPosition = originalPos;
    }

    void SpawnClearedSign()
    {
        GameObject spawnedSign = Instantiate(clearedSignPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        spawnedSign.transform.position = new Vector3(playerTransform.position.x + 10, -2, 0);
        StartCoroutine(MoveUpwards(spawnedSign));
    }
    IEnumerator MoveUpwards(GameObject aSpawnedSign)
    {
        while (aSpawnedSign.transform.position.y < 0)
        {
            aSpawnedSign.transform.Translate(Vector3.up * 1f * Time.deltaTime);
            yield return null;
        }

        CheckDistanceAndDestroy(aSpawnedSign);
    }

    void CheckDistanceAndDestroy(GameObject aSpawnedSign)
    {
        StartCoroutine(CheckDistanceCoroutine(aSpawnedSign));
    }

    IEnumerator CheckDistanceCoroutine(GameObject aSpawnedSign)
    {
        while (Vector3.Distance(aSpawnedSign.transform.position, playerTransform.position) < 18)
        {
            yield return null;
        }

        Destroy(aSpawnedSign);
    }
    IEnumerator SpawnMeleeTweak1AfterDelay()
    {
        // Wait for the specified delay
        
        yield return new WaitForSeconds(meleeTweak1DelayTime);

        // Spawn the enemies for Tweak 1
        foreach (Transform t in meleeTweak1EnemySpawnPosList)
        {
            var meleeEnemy = Instantiate(meleeEnemyPrefab, t.position, Quaternion.identity);
            currentAmbushEnemies.Add(meleeEnemy.gameObject);
        }
        meleeTweak1EnemySpawnPosList.Clear();
    }
    IEnumerator SpawnMeleeTweak2AfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(meleeTweak2DelayTime);

        // Spawn the enemies for Tweak 2
        foreach (Transform t in meleeTweak2EnemySpawnPosList)
        {
            var meleeEnemy = Instantiate(meleeEnemyPrefab, t.position, Quaternion.identity);
            currentAmbushEnemies.Add(meleeEnemy.gameObject);
        }
        meleeTweak2EnemySpawnPosList.Clear();
    }
    bool IsAmbushCleared()
    {
        bool IsAllDead = true;

        foreach (var enemy in currentAmbushEnemies)
        {
            if (enemy != null)
            {
                IsAllDead = false;
                break;
            }
        }
        return IsAllDead;
    }
}
