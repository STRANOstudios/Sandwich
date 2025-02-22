using System.Collections.Generic;
using UnityEngine;

namespace Sandwich
{
    [System.Serializable]
    public class Node
    {
        public Vector3 Position;
        public List<GameObject> Storaged;

        public Node(Vector3 position)
        {
            Position = position;
            Storaged = new List<GameObject>();
        }

        /// <summary>
        /// Clone
        /// </summary>
        public Node Clone()
        {
            Node clonedNode = new(this.Position)
            {
                Storaged = new List<GameObject>(this.Storaged)
            };

            return clonedNode;
        }
    }
}
