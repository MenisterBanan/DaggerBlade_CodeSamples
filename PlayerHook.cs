using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHook : MonoBehaviour
{
    [SerializeField] List<Transform> hookSpotsPosList = new List<Transform>();
    [SerializeField] Transform hookIndicatorTransform;
    [SerializeField] float hookRange = 30f;
    [SerializeField] float hookSpeed = 30f;
    [SerializeField] float hookCooldown = 3f;
    private GameObject[] hookPoints;
    float hookTimer = 0;
    public bool IsHooking = false;
    Rigidbody rb;
    LineRenderer hookLineRenderer;

    Transform savedHookSpotPos;

    private PlayerController playerController;

    CapsuleCollider capsuleCollider;
    public PlayerHook Initialize(PlayerController playerController)
    {
        this.playerController = playerController;

        hookPoints = GameObject.FindGameObjectsWithTag("HookPoint");
        hookIndicatorTransform = GameObject.FindGameObjectWithTag("HookIndicator")?.transform;
        rb = GetComponent<Rigidbody>();

        for (int i = 0; i < hookPoints.Length; i++)
        {
            hookSpotsPosList.Add(hookPoints[i].transform);
        }

        capsuleCollider = GetComponent<CapsuleCollider>();
        hookLineRenderer = GetComponent<LineRenderer>();

        hookLineRenderer.positionCount = 2;
        hookLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        hookLineRenderer.startWidth = 0.03f;
        hookLineRenderer.endWidth = 0.03f;
        hookLineRenderer.startColor = Color.red;
        hookLineRenderer.endColor = Color.red;
        hookLineRenderer.textureMode = LineTextureMode.Stretch;
        hookLineRenderer.numCapVertices = 10;

        return this;
    }

    void Update()
    {
        if (IsHooking)
        {
            MoveToHookedSpot();
            DrawHookLine();
        }
        if (!IsHooking)
        {
            HideHookLine();
        }

        EnableIsTriggerWhileHooking();
        ShowHookShotIndicator();

        hookTimer = Mathf.Clamp(hookTimer-Time.deltaTime,0,hookCooldown);

    }
    public void HandleHookShotInput(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled || hookTimer > 0)
            return;
        TryHookShot();

    }
    void TryHookShot()
    {
        savedHookSpotPos = GetCorrectHookSpot(transform, hookSpotsPosList);

        if (savedHookSpotPos != null)
        {
            if (Vector3.Distance(transform.position, savedHookSpotPos.position) < hookRange)
            {
                IsHooking = true;
                hookTimer = hookCooldown;
            }
        }
    }

    void MoveToHookedSpot()
    {

        if (Vector3.Distance(transform.position, savedHookSpotPos.position) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, savedHookSpotPos.position, hookSpeed * Time.deltaTime);
        }

        else
        {
            IsHooking = false;
        }
    }

    void EnableIsTriggerWhileHooking()
    {
        capsuleCollider.isTrigger = IsHooking;

    }

    Transform GetCorrectHookSpot(Transform player, List<Transform> hookSpots)
    {
        Transform correctHookSpot = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform hookpoint in hookSpots)
        {
            Vector3 directinToHookPoint = hookpoint.position - player.position;
            float dotProduct = Vector3.Dot(player.forward, directinToHookPoint.normalized); // check if hookpoint is in the forward direction
            if (dotProduct > -0.01)
            {
                if (hookpoint.position.y - 1.5f > player.position.y) // check if the hookpoint is above player
                {
                    float distance = Vector3.Distance(player.position, hookpoint.position);
                    if (distance <= hookRange && distance < closestDistance)
                    {
                        closestDistance = distance;
                        correctHookSpot = hookpoint;
                    }
                }
            }
        }

        return correctHookSpot;

    }

    void ShowHookShotIndicator()
    {
        if (!hookIndicatorTransform) return;

        Transform nextHookSpot = GetCorrectHookSpot(transform, hookSpotsPosList);
        Vector3 offset = new Vector3(-0.2f, -0.5f, 0);
        if (nextHookSpot != null)
        {
            if (!hookIndicatorTransform.gameObject.activeInHierarchy)
            {
                hookIndicatorTransform.gameObject.SetActive(true);
            }
            hookIndicatorTransform.position = nextHookSpot.position + offset;
            hookIndicatorTransform.transform.Rotate(0f, 0f, -200f * Time.deltaTime);
        }
        else
        {
            if (hookIndicatorTransform.gameObject.activeInHierarchy)
            {
                hookIndicatorTransform.gameObject.SetActive(false);
            }
        }
    }

    void DrawHookLine()
    {
        Vector3 offset = new Vector3(-0.2f, -0.5f, 0);
        if (savedHookSpotPos != null)
        {
            hookLineRenderer.enabled = true;
            hookLineRenderer.SetPosition(0, transform.position + new Vector3(0, 0.5f, 0));
            hookLineRenderer.SetPosition(1, savedHookSpotPos.position + offset);
        }
    }
    void HideHookLine()
    {
        if (hookLineRenderer != null)
        {
            hookLineRenderer.enabled = false;
        }
    }
}
