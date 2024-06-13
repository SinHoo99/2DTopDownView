using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickUpItem : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnPickedUp(other.gameObject);

        // pickupSound가 null이 아니면 소리를 냄
        if (pickupSound)
            SoundManager.PlayClip(pickupSound);

        Destroy(gameObject);
    }

    protected abstract void OnPickedUp(GameObject receiver);
}
