using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactCounter : MonoBehaviour
{
    public int count = 0;

    public void AddContact() { count++; }
    public void RemoveContact() { count--; }
    public bool HasLiveContacts() { return count > 0; }
}
