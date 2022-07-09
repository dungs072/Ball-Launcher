using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float delayDuration = 0.5f;
    [SerializeField] private float timeSpawned = 1f;
    private Camera mainCamera;
    private bool isDragging;
    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentSpringJoint2D;
    private void Start()
    {
        mainCamera = Camera.main;
        SpawnBall();
    }
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();   
    }
    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
    private void Update()
    {
        if (currentBallRigidbody == null) { return; }
        if (Touch.activeTouches.Count==0) 
        {
            
            if(isDragging)
            {
                LaunchBall();      
            }
            isDragging = false;
            return;
        }
        isDragging = true;
        currentBallRigidbody.isKinematic = true;
        Vector2 touchPosition = new Vector2();
        foreach(Touch touch in Touch.activeTouches)
        {
            touchPosition += touch.screenPosition;
        }
        touchPosition /= Touch.activeTouches.Count;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        currentBallRigidbody.position = worldPosition;
    }
    private void LaunchBall()
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;
        Invoke(nameof(DetachBall), delayDuration);
    }
    private void DetachBall()
    {
        currentSpringJoint2D.enabled = false;
        currentSpringJoint2D = null;
        Invoke(nameof(SpawnBall), timeSpawned);
    }
    private void SpawnBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);
        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentSpringJoint2D = pivot.GetComponent<SpringJoint2D>();
        currentSpringJoint2D.connectedBody = currentBallRigidbody;
        currentSpringJoint2D.enabled = true;
    }
}
