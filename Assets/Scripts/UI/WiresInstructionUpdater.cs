using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WiresInstructionUpdater : MonoBehaviour
{

    public Text TextPrefab;

    ScrollRect _scrollRect;

    private void Start()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    public void UpdateInstructions(List<WiresGame.Instruction> instructions)
    {
        List<Text> textComponents = new List<Text>(_scrollRect.content.GetComponents<Text>());
        Debug.LogFormat("Instructions = {0}", instructions.Count);
        Debug.LogFormat("Text components = {0}", textComponents.Count);
        // instanciate missing components
        if (instructions.Count > textComponents.Count)
        {
            int numNeededComponents = instructions.Count - textComponents.Count;
            for (int i = 0; i < numNeededComponents; i++)
                textComponents.Add(Instantiate(TextPrefab, _scrollRect.content));
        }
        // destroy needless components
        else if (instructions.Count < textComponents.Count)
        {
            int numNeedlessComponents = textComponents.Count - instructions.Count;
            for (int i = 0; i < numNeedlessComponents; i++)
            {
                Destroy(textComponents[i].gameObject);
            }
            textComponents.RemoveRange(0, numNeedlessComponents);
        }
        Debug.LogFormat("Text components = {0}", textComponents.Count);
        // update text messages
        for (int i = 0; i < textComponents.Count; i++)
        {
            textComponents[i].text = instructions[i].generateOutput();
        }
    }
}
