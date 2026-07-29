using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Menu.MenuScripts
{
    [AddComponentMenu("UI/WONML/Art Deco Plaque")]
    public sealed class ArtDecoPlaqueGraphic : MaskableGraphic
    {
        [SerializeField] private Color32 fillColor = new(43, 47, 43, 255);
        [SerializeField] private Color32 brassColor = new(191, 153, 79, 255);
        [SerializeField] private Color32 inkColor = new(35, 25, 20, 255);
        [SerializeField, Min(2f)] private float cornerCut = 18f;
        [SerializeField, Min(1f)] private float outerBorder = 6f;
        [SerializeField, Min(1f)] private float innerLine = 3f;

        public void SetPalette(Color fill, Color brass, Color ink)
        {
            fillColor = fill;
            brassColor = brass;
            inkColor = ink;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            var rect = GetPixelAdjustedRect();
            var shortestSide = Mathf.Min(rect.width, rect.height);
            var cut = Mathf.Clamp(cornerCut, 2f, shortestSide * 0.35f);

            AddFilledPolygon(vertexHelper, BuildOctagon(rect, 0f, cut), brassColor);
            AddFilledPolygon(vertexHelper, BuildOctagon(rect, outerBorder, cut - outerBorder * 0.25f), inkColor);
            AddFilledPolygon(vertexHelper, BuildOctagon(rect, outerBorder + 2f, cut - outerBorder * 0.35f), fillColor);

            var lineInset = outerBorder + 10f;
            AddFilledPolygon(vertexHelper, BuildOctagon(rect, lineInset, cut - lineInset * 0.25f), brassColor);
            AddFilledPolygon(
                vertexHelper,
                BuildOctagon(rect, lineInset + innerLine, cut - (lineInset + innerLine) * 0.25f),
                fillColor);
        }

        private static List<Vector2> BuildOctagon(Rect rect, float inset, float cut)
        {
            var xMin = rect.xMin + inset;
            var xMax = rect.xMax - inset;
            var yMin = rect.yMin + inset;
            var yMax = rect.yMax - inset;
            var safeCut = Mathf.Clamp(cut, 1f, Mathf.Min(xMax - xMin, yMax - yMin) * 0.45f);

            return new List<Vector2>(8)
            {
                new(xMin + safeCut, yMin),
                new(xMax - safeCut, yMin),
                new(xMax, yMin + safeCut),
                new(xMax, yMax - safeCut),
                new(xMax - safeCut, yMax),
                new(xMin + safeCut, yMax),
                new(xMin, yMax - safeCut),
                new(xMin, yMin + safeCut)
            };
        }

        private static void AddFilledPolygon(VertexHelper vertexHelper, IReadOnlyList<Vector2> points, Color32 color)
        {
            var firstVertex = vertexHelper.currentVertCount;
            for (var i = 0; i < points.Count; i++)
            {
                vertexHelper.AddVert(points[i], color, Vector2.zero);
            }

            for (var i = 1; i < points.Count - 1; i++)
            {
                vertexHelper.AddTriangle(firstVertex, firstVertex + i, firstVertex + i + 1);
            }
        }
    }
}
