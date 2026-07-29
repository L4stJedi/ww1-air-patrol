using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scenes.Menu.MenuScripts
{
    internal static class ArtDecoTheme
    {
        internal static readonly Color32 Enamel = new(43, 49, 45, 255);
        internal static readonly Color32 EnamelLight = new(58, 65, 59, 255);
        internal static readonly Color32 EnamelSelected = new(45, 57, 58, 255);
        internal static readonly Color32 Brown = new(87, 57, 39, 255);
        internal static readonly Color32 Brass = new(193, 155, 79, 255);
        internal static readonly Color32 Ink = new(38, 27, 21, 255);
        internal static readonly Color32 Cream = new(235, 224, 185, 255);
        internal static readonly Color32 Paper = new(242, 231, 191, 255);
        internal static readonly Color32 Burgundy = new(111, 34, 30, 255);
        internal static readonly Color32 Locked = new(174, 88, 70, 255);
        internal static readonly Color32 Muted = new(194, 188, 163, 255);
    }

    internal static class ArtDecoUi
    {
        internal static RectTransform CreateRect(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
            gameObject.layer = 5;
            var rect = (RectTransform)gameObject.transform;
            rect.SetParent(parent, false);
            rect.localScale = Vector3.one;
            return rect;
        }

        internal static RectTransform CreatePanel(
            string name,
            Transform parent,
            Vector2 anchor,
            Vector2 size,
            Color fill)
        {
            var rect = CreateRect(name, parent);
            Anchor(rect, anchor, size);
            var plaque = rect.gameObject.AddComponent<ArtDecoPlaqueGraphic>();
            plaque.SetPalette(fill, ArtDecoTheme.Brass, ArtDecoTheme.Ink);
            plaque.raycastTarget = false;
            return rect;
        }

        internal static Button CreateButton(
            string name,
            Transform parent,
            string label,
            Vector2 anchor,
            Vector2 size,
            Color fill,
            TMP_FontAsset font,
            UnityAction action = null,
            float fontSize = 25f)
        {
            var rect = CreateRect(name, parent);
            Anchor(rect, anchor, size);

            var plaque = rect.gameObject.AddComponent<ArtDecoPlaqueGraphic>();
            plaque.SetPalette(fill, ArtDecoTheme.Brass, ArtDecoTheme.Ink);

            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = plaque;
            button.transition = Selectable.Transition.ColorTint;
            button.colors = ButtonColors();
            if (action != null)
            {
                button.onClick.AddListener(action);
            }

            var text = CreateText(
                "Label",
                rect,
                label,
                fontSize,
                ArtDecoTheme.Cream,
                TextAlignmentOptions.Center,
                font);
            Stretch(text.rectTransform, 14f);
            text.fontStyle = FontStyles.Bold;
            text.characterSpacing = 3f;
            text.raycastTarget = false;
            return button;
        }

        internal static TextMeshProUGUI CreateText(
            string name,
            Transform parent,
            string value,
            float fontSize,
            Color color,
            TextAlignmentOptions alignment,
            TMP_FontAsset font)
        {
            var rect = CreateRect(name, parent);
            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.text = value;
            text.font = font;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Ellipsis;
            return text;
        }

        internal static Image CreateImage(
            string name,
            Transform parent,
            Sprite sprite,
            Color color,
            bool preserveAspect = false)
        {
            var rect = CreateRect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.preserveAspect = preserveAspect;
            image.raycastTarget = false;
            return image;
        }

        internal static void CreateRule(Transform parent, Vector2 anchor, float width)
        {
            var rule = CreateRect("DecoRule", parent);
            Anchor(rule, anchor, new Vector2(width, 5f));
            var image = rule.gameObject.AddComponent<Image>();
            image.color = ArtDecoTheme.Brass;
            image.raycastTarget = false;

            var diamond = CreateRect("Diamond", rule);
            Anchor(diamond, new Vector2(0.5f, 0.5f), new Vector2(16f, 16f));
            diamond.localRotation = Quaternion.Euler(0f, 0f, 45f);
            var diamondImage = diamond.gameObject.AddComponent<Image>();
            diamondImage.color = ArtDecoTheme.Brass;
            diamondImage.raycastTarget = false;
        }

        internal static void CreateHeader(
            RectTransform safeArea,
            string title,
            string subtitle,
            TMP_FontAsset font,
            UnityAction backAction)
        {
            CreateButton(
                "Back",
                safeArea,
                "‹  BACK",
                new Vector2(0.13f, 0.925f),
                new Vector2(230f, 84f),
                ArtDecoTheme.Brown,
                font,
                backAction,
                23f);

            var heading = CreateText(
                "PageTitle",
                safeArea,
                title,
                44f,
                ArtDecoTheme.Burgundy,
                TextAlignmentOptions.Center,
                font);
            Anchor(heading.rectTransform, new Vector2(0.5f, 0.93f), new Vector2(560f, 72f));
            heading.fontStyle = FontStyles.Bold;
            heading.characterSpacing = 6f;

            var subheading = CreateText(
                "PageSubtitle",
                safeArea,
                subtitle,
                20f,
                ArtDecoTheme.Ink,
                TextAlignmentOptions.Center,
                font);
            Anchor(subheading.rectTransform, new Vector2(0.5f, 0.885f), new Vector2(720f, 44f));
            subheading.characterSpacing = 4f;

            CreateRule(safeArea, new Vector2(0.19f, 0.885f), 160f);
            CreateRule(safeArea, new Vector2(0.81f, 0.885f), 160f);
        }

        internal static void Anchor(RectTransform rect, Vector2 anchor, Vector2 size)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
        }

        internal static void SetAnchored(RectTransform rect, Vector2 position, Vector2 size)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        internal static void Stretch(RectTransform rect, float inset = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }

        internal static Transform FindDeep(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == name)
            {
                return root;
            }

            foreach (Transform child in root)
            {
                var found = FindDeep(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        internal static ColorBlock ButtonColors()
        {
            var colors = ColorBlock.defaultColorBlock;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.08f, 1.05f, 0.92f, 1f);
            colors.pressedColor = new Color(0.7f, 0.7f, 0.66f, 1f);
            colors.selectedColor = new Color(1.05f, 1.02f, 0.9f, 1f);
            colors.disabledColor = new Color(0.48f, 0.48f, 0.46f, 0.55f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            return colors;
        }
    }
}
