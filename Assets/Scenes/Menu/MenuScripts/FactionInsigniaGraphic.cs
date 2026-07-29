using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Menu.MenuScripts
{
    [AddComponentMenu("UI/WONML/Faction Insignia")]
    public sealed class FactionInsigniaGraphic : MaskableGraphic
    {
        [SerializeField] private NationTabs.Nations faction;
        private const int CircleSegments = 48;

        public void SetFaction(NationTabs.Nations newFaction, bool locked)
        {
            faction = newFaction;
            color = locked ? new Color(0.55f, 0.52f, 0.46f, 1f) : Color.white;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            var rect = GetPixelAdjustedRect();
            var center = rect.center;
            var radius = Mathf.Min(rect.width, rect.height) * 0.48f;
            var dark = new Color32(37, 30, 24, 255);
            var cream = new Color32(232, 221, 183, 255);
            var red = new Color32(132, 42, 34, 255);
            var navy = new Color32(38, 57, 78, 255);
            var blue = new Color32(55, 90, 132, 255);
            var green = new Color32(52, 91, 60, 255);

            AddDisc(vertexHelper, center, radius, dark);

            switch (faction)
            {
                case NationTabs.Nations.Germany:
                    AddDisc(vertexHelper, center, radius * 0.86f, cream);
                    AddIronCross(vertexHelper, center, radius * 0.72f, dark);
                    break;

                case NationTabs.Nations.Britain:
                    AddDisc(vertexHelper, center, radius * 0.84f, navy);
                    AddDisc(vertexHelper, center, radius * 0.58f, cream);
                    AddDisc(vertexHelper, center, radius * 0.31f, red);
                    break;

                case NationTabs.Nations.France:
                    AddDisc(vertexHelper, center, radius * 0.84f, red);
                    AddDisc(vertexHelper, center, radius * 0.58f, cream);
                    AddDisc(vertexHelper, center, radius * 0.31f, blue);
                    break;

                case NationTabs.Nations.Italy:
                    AddDisc(vertexHelper, center, radius * 0.84f, red);
                    AddDisc(vertexHelper, center, radius * 0.58f, cream);
                    AddDisc(vertexHelper, center, radius * 0.31f, green);
                    break;

                case NationTabs.Nations.Russia:
                    AddDisc(vertexHelper, center, radius * 0.84f, red);
                    AddDisc(vertexHelper, center, radius * 0.58f, blue);
                    AddDisc(vertexHelper, center, radius * 0.31f, cream);
                    break;

                case NationTabs.Nations.America:
                    AddDisc(vertexHelper, center, radius * 0.84f, navy);
                    AddStar(vertexHelper, center, radius * 0.62f, cream);
                    AddDisc(vertexHelper, center, radius * 0.15f, red);
                    break;
            }
        }

        private static void AddDisc(VertexHelper vertexHelper, Vector2 center, float radius, Color32 color)
        {
            var firstVertex = vertexHelper.currentVertCount;
            vertexHelper.AddVert(center, color, Vector2.zero);

            for (var i = 0; i <= CircleSegments; i++)
            {
                var angle = i * Mathf.PI * 2f / CircleSegments;
                var point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                vertexHelper.AddVert(point, color, Vector2.zero);
            }

            for (var i = 0; i < CircleSegments; i++)
            {
                vertexHelper.AddTriangle(firstVertex, firstVertex + i + 1, firstVertex + i + 2);
            }
        }

        private static void AddIronCross(VertexHelper vertexHelper, Vector2 center, float radius, Color32 color)
        {
            var hub = radius * 0.18f;
            var flare = radius * 0.48f;
            var reach = radius * 0.72f;

            AddConvexQuad(
                vertexHelper,
                center + new Vector2(-hub, hub),
                center + new Vector2(hub, hub),
                center + new Vector2(flare, reach),
                center + new Vector2(-flare, reach),
                color);
            AddConvexQuad(
                vertexHelper,
                center + new Vector2(-flare, -reach),
                center + new Vector2(flare, -reach),
                center + new Vector2(hub, -hub),
                center + new Vector2(-hub, -hub),
                color);
            AddConvexQuad(
                vertexHelper,
                center + new Vector2(-reach, -flare),
                center + new Vector2(-hub, -hub),
                center + new Vector2(-hub, hub),
                center + new Vector2(-reach, flare),
                color);
            AddConvexQuad(
                vertexHelper,
                center + new Vector2(hub, -hub),
                center + new Vector2(reach, -flare),
                center + new Vector2(reach, flare),
                center + new Vector2(hub, hub),
                color);
            AddQuad(vertexHelper, center, new Vector2(hub * 2.1f, hub * 2.1f), color);
        }

        private static void AddStar(VertexHelper vertexHelper, Vector2 center, float radius, Color32 color)
        {
            var firstVertex = vertexHelper.currentVertCount;
            vertexHelper.AddVert(center, color, Vector2.zero);
            for (var i = 0; i <= 10; i++)
            {
                var pointRadius = i % 2 == 0 ? radius : radius * 0.42f;
                var angle = Mathf.PI * 0.5f + i * Mathf.PI / 5f;
                var point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * pointRadius;
                vertexHelper.AddVert(point, color, Vector2.zero);
            }

            for (var i = 0; i < 10; i++)
            {
                vertexHelper.AddTriangle(firstVertex, firstVertex + i + 1, firstVertex + i + 2);
            }
        }

        private static void AddConvexQuad(
            VertexHelper vertexHelper,
            Vector2 bottomLeft,
            Vector2 bottomRight,
            Vector2 topRight,
            Vector2 topLeft,
            Color32 color)
        {
            var firstVertex = vertexHelper.currentVertCount;
            vertexHelper.AddVert(bottomLeft, color, Vector2.zero);
            vertexHelper.AddVert(topLeft, color, Vector2.zero);
            vertexHelper.AddVert(topRight, color, Vector2.zero);
            vertexHelper.AddVert(bottomRight, color, Vector2.zero);
            vertexHelper.AddTriangle(firstVertex, firstVertex + 1, firstVertex + 2);
            vertexHelper.AddTriangle(firstVertex, firstVertex + 2, firstVertex + 3);
        }

        private static void AddQuad(VertexHelper vertexHelper, Vector2 center, Vector2 size, Color32 color)
        {
            var half = size * 0.5f;
            var firstVertex = vertexHelper.currentVertCount;
            vertexHelper.AddVert(center + new Vector2(-half.x, -half.y), color, Vector2.zero);
            vertexHelper.AddVert(center + new Vector2(-half.x, half.y), color, Vector2.zero);
            vertexHelper.AddVert(center + new Vector2(half.x, half.y), color, Vector2.zero);
            vertexHelper.AddVert(center + new Vector2(half.x, -half.y), color, Vector2.zero);
            vertexHelper.AddTriangle(firstVertex, firstVertex + 1, firstVertex + 2);
            vertexHelper.AddTriangle(firstVertex, firstVertex + 2, firstVertex + 3);
        }
    }
}
