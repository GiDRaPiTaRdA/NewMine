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

    public BlockInteraction blockInteraction;

    public Image image;

    // Update is called once per frame
    void Update()
    {
        if (this.blockType != this.blockInteraction.blockType)
        {
            this.blockType = this.blockInteraction.blockType;

            CubeDescription description = StaticWorld.Instance.CubeDescriptions[this.blockType.Value];

            Material m = description.previewMaterial;
            Texture2D tex = (Texture2D) m.mainTexture;

            this.image.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

            if (m.HasProperty("_Color")) this.image.color = m.GetColor("_Color");

        }
    }
}
