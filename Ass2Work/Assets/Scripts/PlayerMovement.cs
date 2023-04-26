using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    private Camera mainCamera;
    [SyncVar] public float movementSpeed = 5.0f;
    private void Start()
    {
        // Enable the NavMeshAgent for all instances
        agent.enabled = true;

        if (isLocalPlayer)
        {
            mainCamera = Camera.main; // Camera reference

        }
    }

    private void Update()
    {
        // Make sure only the local player's input is processed
        if (!isLocalPlayer) return;

        // Get the input from the keyboard
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Calculate the movement direction
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (movement.magnitude >= 0.1f)
        {
            // Rotate the player towards the movement direction
            transform.rotation = Quaternion.LookRotation(movement);

            // Calculate the new position based on the movement direction
            Vector3 newPosition = transform.position + movement * movementSpeed * Time.deltaTime;

            // Move the player to the new position using the NavMeshAgent
            agent.SetDestination(newPosition);
        }
    }
}

