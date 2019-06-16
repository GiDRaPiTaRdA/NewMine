using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Minecraft.Scripts.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.UIElements;

public class ChooseWorldBehaviour : MonoBehaviour
{
    public ListScript list;

    public UnityEvent play;
    // Start is called before the first frame update
    void Start()
    {
        GameData.SelectedSave = null;
        GameData.LoadMode = null;
    }

    public void CreateWorld()
    {
        PopUpManager.Instance.ShowEditField("Enter world name", "WorldName", (worldName) =>
        {
                this.Play(worldName, LoadMode.Create);
        });
    }

    public void EditWorld()
    {
        PopUpManager.Instance.ShowEditField("Enter new world name", this.list.SelectedItem.Text.text, (worldName) =>
        {
                SaveManager.Instance.Rename(this.list.SelectedItem.Text.text, worldName);
                this.list.SelectedItem.Text.text = worldName;
            
        });
    }

    public void Play()
    {
        if (this.list.SelectedIndex != -1)
        {
            ListItemScript listItemScript = this.list.SelectedItem;

            string worldName = listItemScript.Text.text;

            this.Play(worldName, LoadMode.Load);
        }
    }

    public void DeleteWorld()
    {
        PopUpManager.Instance.Show($"Remove world?",() =>
        {
            SaveManager.Instance.Delete(this.list.SelectedItem.Text.text);
            this.list.Remove(this.list.SelectedIndex);
        });
    }


    public void Play(string worldName, LoadMode loadMode)
    {
        GameData.SelectedSave = worldName;
        GameData.LoadMode = loadMode;

        this.play?.Invoke();
    }
}
