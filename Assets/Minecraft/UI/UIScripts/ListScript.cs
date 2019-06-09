using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ListScript : MonoBehaviour
{
    public GameObject Content;

    public GameObject ItemPrefab;

    private Color defaultColor;

    public Color selectedColor;

    private List<ListItemScript> items;

    public ListItemScript SelectedItem=> this.SelectedIndex!=-1? this.items.ElementAt(this.SelectedIndex):null;

    public int SelectedIndex { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        this.SelectedIndex = -1;

        this.defaultColor = this.ItemPrefab.GetComponent<ListItemScript>().panel.color;

        this.items = new List<ListItemScript>();

        string[] saves = SaveManager.Instance.LoadSaves();

        foreach (string save in saves)
        {
            this.AddItem(save);
        }
    }

    public void AddItem(string saveName)
    {
        GameObject item = Instantiate(this.ItemPrefab);

        ListItemScript itemScript = item.GetComponent<ListItemScript>();

        itemScript.Text.text = saveName;

        item.transform.parent = this.Content.transform;

        this.items.Add(itemScript);

        itemScript.button.onClick.AddListener(() => { this.OnSelect(this.items.IndexOf(itemScript)); });
    }

    public void Remove()
    {
        if (this.SelectedIndex != -1)
        {
            ListItemScript item = this.items.ElementAt(this.SelectedIndex);

            this.items.Remove(item);
            item.button.onClick.RemoveAllListeners();

            Destroy(item.gameObject);

            this.SelectedIndex = -1;
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

    }


}
