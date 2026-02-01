using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PuzzleZone : MonoBehaviour
{
    [SerializeField] private GameObject handle;
    [SerializeField] private DynamoCore core;
    [SerializeField] private Door door;
    private bool isActive = false;
    private bool isOpen = false;

    private void Start()
    {

        handle.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isActive) return;

        if (other.CompareTag("Player"))
        {
            if (PlayerPuzzles.Instance.hasHandle)
            {
                isActive = true;
                handle.SetActive(true);
            }
        }
    }
    private void Update()
    {
        if (core.NormalizedCharge >= 0.95f)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;

        door.TriggerOpen();
        StartCoroutine(CloseDoor());
    }

    private IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(30f);

        door.TriggerClose();
    }
}