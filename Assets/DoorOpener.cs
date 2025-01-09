using System;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public bool openDoor = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                openDoor = true;
                Console.WriteLine("Door is open");
            }
        }
    }
}
