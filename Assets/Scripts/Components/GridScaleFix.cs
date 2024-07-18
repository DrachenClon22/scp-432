using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridScaleFix : MonoBehaviour
{
    public static Vector2 size { get; private set; }

    GridLayoutGroup gridLayoutGroup;
    RectTransform rect;

    public GameObject targetGrid;

    public float divider = 4;

    void Start()
    {
        gridLayoutGroup = targetGrid.GetComponent<GridLayoutGroup>();
        rect = targetGrid.GetComponent<RectTransform>();

        gridLayoutGroup.cellSize = new Vector2(rect.rect.height / divider, rect.rect.height / divider);
        size = gridLayoutGroup.cellSize;
    }
}
