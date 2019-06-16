using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class ListScript : MonoBehaviour
{
    public GameObject Content;

    public GameObject ItemPrefab;

    private Color defaultColor;

    public Color selectedColor;

    private List<ListItemScript> items;

    public ListItemScript SelectedItem=> this.SelectedIndex!=-1? this.items.ElementAt(this.SelectedIndex):null;

    public int SelectedIndex { get; set; }

    public EventInt onSelected;

    public EventBool onOperationEnabled;

    // Start is called before the first frame update
    void Start()
    {
        this.SelectedIndex = -1;

        this.defaultColor = this.ItemPrefab.GetComponent<ListItemScript>().panel.color;

        this.onSelected.AddListener(index=>this.onOperationEnabled?.Invoke(index != -1));

        this.items = new List<ListItemScript>();

        string[] saves = SaveManager.Instance.LoadSaves();

        foreach (string save in saves)
        {
            this.AddItem(save);
        }
    }

    public void AddItem(string saveName)
    {
        GameObject item = Instantiate(this.ItemPrefab, this.Content.transform);

        ListItemScript itemScript = item.GetComponent<ListItemScript>();

        itemScript.Text.text = saveName;

        this.items.Add(itemScript);

        itemScript.button.onClick.AddListener(() => { this.OnSelect(this.items.IndexOf(itemScript)); });
    }

    public void Remove() => this.Remove(this.SelectedIndex);

    public void Remove(int index)
    {
        if (index != -1)
        {
            ListItemScript item = this.items.ElementAt(this.SelectedIndex);

            this.items.Remove(item);
            item.button.onClick.RemoveAllListeners();

            Destroy(item.gameObject);

            this.SelectedIndex = -1;
            this.OnSelect(this.SelectedIndex);
        }
    }

    public void OnSelect(int index)
    {
        this.SelectedIndex = index;

        foreach (ListItemScript listItemScript in this.items)
        {
            if (this.items.IndexOf(listItemScript) != index)
            {
                listItemScript.panel.color = this.defaultColor;
            }
            else
            {
                listItemScript.panel.color = this.selectedColor;
            }
        }

        this.onSelected?.Invoke(this.SelectedIndex);

    }




}
