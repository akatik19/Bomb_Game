using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SymbolGame))]
public class SymbolGameUI : MonoBehaviour
{

    public GameObject ImageElementPrefab;

    public GameObject ColumnPrefab;

    public GameObject TablePrefab;

    public int SpaceBetweenImages;

    void OnEnable()
    {
        var game = GetComponent<SymbolGame>();
        //if (game)
        //    game.ShownSymbolsChanged += UpdateImages;
        //UpdateImages(game.GetShownSymbols());
        UpdateColumns(game);
    }

    void OnDisable()
    {
        var game = GetComponent<SymbolGame>();
        //if (game)
        //    game.ShownSymbolsChanged -= UpdateImages;
    }

    void UpdateColumns(SymbolGame game)
    {
        GameObject symbolContent = GameObject.FindGameObjectWithTag("SymbolContent");
        if (!symbolContent)
        {
            Debug.LogWarning("Game object SymbolContent wasn't found.");
            return;
        }

        GridLayoutGroup gridLayoutGroup = symbolContent.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = game.ColumnCount;

        for (int j = 0; j < game.RowCount; j++)
        {
            for (int i = 0; i < game.ColumnCount; i++)
            {
                var imageGameObject = Instantiate(ImageElementPrefab, symbolContent.transform);
                Image image = imageGameObject.GetComponent<Image>();
                image.sprite = game.GetSymbol(j, i);
            }
        }
    }
}
