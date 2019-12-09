using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proseduraltreecontroller : MonoBehaviour
{
    
    [SerializeField] ProceduralTree tree;

    void Update() {
        if (Input.anyKey) {
            tree.isGworing = true;
        }
        else {
            tree.isGworing = false;
        }

        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKeyDown(vKey) && vKey != KeyCode.Backspace) {
                tree.branchChance += Random.Range(0,6);
                tree.bigRootChance += Random.Range(0, 2);
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace)) {
            tree.branchChance -= 6;
            tree.bigRootChance -= 2;

            if (tree.branchChance < 0)
            {
                tree.branchChance = 0;
            }

            if (tree.bigRootChance < 0)
            {
                tree.bigRootChance = 0;
            }
        }
    }
}