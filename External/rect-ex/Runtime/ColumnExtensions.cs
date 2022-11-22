using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TNRD.DeepInspector.RectEx.Internal;

namespace TNRD.DeepInspector.RectEx {
    internal static class ColumnExtensions {

        private const float SPACE = 2f;

        public static Rect[] Column(this Rect rect, int count, float space = SPACE){
            rect = rect.Invert();
            var result = rect.Row(count, space);
            return result.Select(x => x.Invert()).ToArray();
        }

        public static Rect[] Column(this Rect rect, float[] weights, float space = SPACE){
            return Column(rect, weights, null, space);
        }

        public static Rect[] Column(this Rect rect, float[] weights, float[] widthes, float space = SPACE) {
            rect = rect.Invert();
            var result = rect.Row(weights, widthes, space);
            return result.Select(x => x.Invert()).ToArray();
        }

    }
}
