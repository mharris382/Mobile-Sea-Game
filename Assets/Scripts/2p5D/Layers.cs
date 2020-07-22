using UnityEngine;

namespace _2p5D
{
    public static class Layers
    {
        private static int[] layers;

         static Layers()
        {
            layers = new[]
            {
                LayerMask.NameToLayer("Ground"),
                LayerMask.NameToLayer("Ground1"),
                LayerMask.NameToLayer("Ground2"),
                LayerMask.NameToLayer("Ground4"),
            };
            
            
        }
        
        public static int GetLayer(int layer)
        {
            layer = Mathf.Clamp(layer, 0, 3);
            return layers[layer];
        }
        
        
        private class GameDepthLayer
        {
            private int _layer;
            private int _sortingLayer;
            private int _orderInLayer;
        }
    }

    internal struct LayerDepth
    {
        private int _layer;
    }
}
