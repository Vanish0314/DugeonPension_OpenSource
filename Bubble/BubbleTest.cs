using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class BubbleTest : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            transform.position += Vector3.right * Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                BubbleManager.Instance.ShowBubble(this.transform,"Bubble",BubbleID.DialogueBubble);
            }
        }
    }
}
