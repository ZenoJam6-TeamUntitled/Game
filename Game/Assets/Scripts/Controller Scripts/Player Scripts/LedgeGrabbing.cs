using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabbing : MonoBehaviour
{
    public PlayerController Mantle;
     // Start is called before the first frame update
    public LayerMask vaultLayer;
    private float playerHeight = 2f;
    private float playerRadius = 0.5f;
    public float mantling;

    void start()
    {
        vaultLayer = ~vaultLayer;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        mantling = Mantle.playerInput.Player.Mantle.ReadValue<float>();
        Vault();
    }
    private void Vault()
    {
        Vector3 rayOrigin = transform.position;

        // Set the direction of the ray to be forward
        Vector3 rayDirection = new Vector3(transform.forward.x, transform.forward.y, 0);

        float rayLength = transform.forward.x;
        if (Mathf.Abs(mantling) > 0.5f)
        {
            if (Physics.Raycast(rayOrigin, rayDirection, out var firstHit, rayLength, vaultLayer)) 
            {
                Debug.Log(vaultLayer);
                //Debug.Log(firstHit.point + (rayOrigin * playerRadius) + (Vector3.up * 0.2f * playerHeight));
                print("vaultable in front");
                if (Physics.Raycast(firstHit.point + (rayDirection * playerRadius) + (Vector3.up * 0.6f * playerHeight), Vector3.down, out var secondHit, playerHeight))
                {
                    print(secondHit);
                    StartCoroutine(LerpVault(secondHit.point, 0.7f));
                }
            }

        }



    }
    IEnumerator LerpVault(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
