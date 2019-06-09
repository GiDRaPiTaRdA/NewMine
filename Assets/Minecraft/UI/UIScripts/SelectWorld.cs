using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SelectWorld : MonoBehaviour
{
    public ListScript list;

    public UnityEvent play;

    public void Play()
    {
        if (this.list.SelectedIndex != -1)
        {
            ListItemScript listItemScript = this.list.SelectedItem;

            string worldName = listItemScript.Text.text;

            SaveManager.Instance.SelectedSave = worldName;

            this.play?.Invoke();
        }


    }
}
