using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class CubeChange : MonoBehaviour
{
    private BlockType? blockType;

    public GameObject blockInteractionGameObject;

    private BlockInteraction blockInteraction;

    // Start is called before the first frame update
    void Start()
    {
        this.blockInteraction = this.blockInteractionGameObject.GetComponent<BlockInteraction>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.blockType != this.blockInteraction.blockType)
        {
            this.blockType = this.blockInteraction.blockType;

            CubeDescription description = StaticWorld.Instance.CubeDescriptions[this.blockType.Value];

            Material m = description.previewMaterial;
            Texture2D tex = (Texture2D) m.mainTexture;

            Image image = this.GetComponent<Image>();

            image.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

            if (m.HasProperty("_Color"))
                image.color = m.GetColor("_Color");

        }
    }
}
