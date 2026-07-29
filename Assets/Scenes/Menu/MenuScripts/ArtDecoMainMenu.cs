using System;
using System.Collections.Generic;
using SavedVariables;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scenes.Menu.MenuScripts
{
    [DisallowMultipleComponent]
    public sealed class ArtDecoMainMenu : MonoBehaviour
    {
        private static readonly Color32 Enamel = new(43, 49, 45, 255);
        private static readonly Color32 EnamelSelected = new(45, 57, 58, 255);
        private static readonly Color32 Brass = new(193, 155, 79, 255);
        private static readonly Color32 Ink = new(38, 27, 21, 255);
        private static readonly Color32 Cream = new(235, 224, 185, 255);
        private static readonly Color32 Burgundy = new(111, 34, 30, 255);

        private TMP_FontAsset font;

        private void Awake()
        {
            if (transform.Find("ArtDecoSafeArea") != null)
            {
                return;
            }

            EnsureMenuCamera();

            var sourceTitle = transform.Find("Text (TMP)")?.GetComponent<TextMeshProUGUI>();
            font = sourceTitle != null ? sourceTitle.font : TMP_Settings.defaultFontAsset;

            var campaignSource = FindButton("ButtonCampaigns");
            var settingsSource = FindButton("ButtonSettings");
            var hangarSource = FindButton("ButtonHangar");

            PrepareLegacyMenu(campaignSource, settingsSource, hangarSource);

            var safeArea = CreateRect("ArtDecoSafeArea", transform);
            Stretch(safeArea);
            safeArea.gameObject.AddComponent<RuntimeSafeAreaFitter>();

            CreateTitle(safeArea);
            CreateSectionHeading(safeArea);
            CreateFactionCarousel(safeArea);
            CreateNavigation(safeArea, campaignSource, settingsSource, hangarSource);
            CreateScorePlate(safeArea);

            var suite = gameObject.AddComponent<ArtDecoMenuSuite>();
            suite.Initialize(font);
        }

        private Button FindButton(string objectName)
        {
            return transform.Find(objectName)?.GetComponent<Button>();
        }

        private void PrepareLegacyMenu(Button campaign, Button settings, Button hangar)
        {
            var background = transform.Find("Background")?.GetComponent<Image>();
            if (background != null)
            {
                background.raycastTarget = false;
                background.color = new Color(0.88f, 0.88f, 0.84f, 1f);
            }

            SetInactive("ButtonRFC");
            SetInactive("ButtonGAS");
            SetInactive("Text (TMP)");

            foreach (Transform child in transform)
            {
                if (child.name == "BestScoreBackground")
                {
                    child.gameObject.SetActive(false);
                }
            }

            HideSourceButton(campaign);
            HideSourceButton(settings);
            HideSourceButton(hangar);
        }

        private void SetInactive(string objectName)
        {
            var target = transform.Find(objectName);
            if (target != null)
            {
                target.gameObject.SetActive(false);
            }
        }

        private static void HideSourceButton(Button source)
        {
            if (source == null)
            {
                return;
            }

            source.interactable = false;
            var sourceImage = source.GetComponent<Image>();
            if (sourceImage != null)
            {
                sourceImage.enabled = false;
                sourceImage.raycastTarget = false;
            }

            foreach (var label in source.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                label.enabled = false;
            }
        }

        private void CreateTitle(RectTransform parent)
        {
            var title = CreateText(
                "GameTitle",
                parent,
                "WINGS OVER\nNO MAN'S LAND",
                64f,
                Burgundy,
                TextAlignmentOptions.Center);
            Anchor(title.rectTransform, new Vector2(0.5f, 0.79f), new Vector2(850f, 180f));
            title.fontStyle = FontStyles.Bold;
            title.characterSpacing = 4f;
            title.outlineColor = new Color32(55, 30, 22, 230);
            title.outlineWidth = 0.08f;

            var era = CreateText(
                "EraLine",
                parent,
                "AERIAL ADVENTURES  •  1914–1918",
                23f,
                Ink,
                TextAlignmentOptions.Center);
            Anchor(era.rectTransform, new Vector2(0.5f, 0.705f), new Vector2(760f, 50f));
            era.characterSpacing = 3f;
        }

        private void CreateSectionHeading(RectTransform parent)
        {
            var heading = CreateText(
                "ChooseYourSide",
                parent,
                "CHOOSE YOUR SIDE",
                34f,
                Ink,
                TextAlignmentOptions.Center);
            Anchor(heading.rectTransform, new Vector2(0.5f, 0.625f), new Vector2(520f, 70f));
            heading.fontStyle = FontStyles.Bold;
            heading.characterSpacing = 6f;

            CreateDecoRule(parent, new Vector2(0.16f, 0.625f), 180f);
            CreateDecoRule(parent, new Vector2(0.84f, 0.625f), 180f);
        }

        private void CreateFactionCarousel(RectTransform parent)
        {
            var viewport = CreateRect("FactionCarousel", parent);
            Anchor(viewport, new Vector2(0.5f, 0.445f), new Vector2(930f, 380f));

            var inputSurface = viewport.gameObject.AddComponent<Image>();
            inputSurface.color = new Color(0f, 0f, 0f, 0.001f);
            inputSurface.raycastTarget = true;
            viewport.gameObject.AddComponent<RectMask2D>();

            var content = CreateRect("Content", viewport);
            content.anchorMin = new Vector2(0f, 0.5f);
            content.anchorMax = new Vector2(0f, 0.5f);
            content.pivot = new Vector2(0f, 0.5f);
            content.anchoredPosition = Vector2.zero;

            var data = new[]
            {
                new FactionCarousel.FactionData(NationTabs.Nations.Germany, "Germany", "LUFTSTREITKRÄFTE"),
                new FactionCarousel.FactionData(NationTabs.Nations.Britain, "Britain", "ROYAL FLYING CORPS"),
                new FactionCarousel.FactionData(NationTabs.Nations.America, "USA", "U.S. AIR SERVICE"),
                new FactionCarousel.FactionData(NationTabs.Nations.France, "France", "AÉRONAUTIQUE MILITAIRE"),
                new FactionCarousel.FactionData(NationTabs.Nations.Italy, "Italy", "CORPO AERONAUTICO"),
                new FactionCarousel.FactionData(NationTabs.Nations.Russia, "Russia", "IMPERIAL AIR SERVICE")
            };

            const float cardWidth = 310f;
            const float cardHeight = 310f;
            const float spacing = 34f;
            content.sizeDelta = new Vector2(data.Length * cardWidth + (data.Length - 1) * spacing, cardHeight);

            var carousel = viewport.gameObject.AddComponent<FactionCarousel>();
            var cards = new List<FactionCarousel.CardView>();

            for (var i = 0; i < data.Length; i++)
            {
                var index = i;
                var unlocked = FactionUnlockProgress.IsUnlocked(data[i].Faction);
                var card = CreateRect($"Faction_{data[i].Name}", content);
                card.anchorMin = new Vector2(0f, 0.5f);
                card.anchorMax = new Vector2(0f, 0.5f);
                card.pivot = new Vector2(0.5f, 0.5f);
                card.sizeDelta = new Vector2(cardWidth, cardHeight);
                card.anchoredPosition = new Vector2(cardWidth * 0.5f + i * (cardWidth + spacing), 0f);

                var plaque = card.gameObject.AddComponent<ArtDecoPlaqueGraphic>();
                plaque.SetPalette(unlocked ? EnamelSelected : new Color32(54, 51, 46, 255), Brass, Ink);

                var cardButton = card.gameObject.AddComponent<Button>();
                cardButton.targetGraphic = plaque;
                cardButton.transition = Selectable.Transition.ColorTint;
                cardButton.colors = CreateButtonColors();
                cardButton.onClick.AddListener(() => carousel.Focus(index));

                var group = card.gameObject.AddComponent<CanvasGroup>();

                var insigniaRect = CreateRect("Roundel", card);
                insigniaRect.anchorMin = insigniaRect.anchorMax = new Vector2(0.5f, 0.5f);
                insigniaRect.anchoredPosition = new Vector2(0f, 62f);
                insigniaRect.sizeDelta = new Vector2(112f, 112f);
                var insignia = insigniaRect.gameObject.AddComponent<FactionInsigniaGraphic>();
                insignia.raycastTarget = false;
                insignia.SetFaction(data[i].Faction, !unlocked);

                var factionName = CreateText(
                    "FactionName",
                    card,
                    data[i].Name.ToUpperInvariant(),
                    31f,
                    Cream,
                    TextAlignmentOptions.Center);
                factionName.fontStyle = FontStyles.Bold;
                factionName.characterSpacing = 5f;
                SetAnchored(factionName.rectTransform, new Vector2(0f, -20f), new Vector2(280f, 55f));

                var service = CreateText(
                    "ServiceName",
                    card,
                    data[i].Service,
                    17f,
                    new Color32(205, 198, 170, 255),
                    TextAlignmentOptions.Center);
                service.characterSpacing = 2f;
                service.enableAutoSizing = true;
                service.fontSizeMin = 13f;
                service.fontSizeMax = 17f;
                SetAnchored(service.rectTransform, new Vector2(0f, -68f), new Vector2(270f, 45f));

                var status = CreateText(
                    "Status",
                    card,
                    unlocked ? "AVAILABLE" : "LOCKED",
                    19f,
                    unlocked ? Brass : new Color32(174, 88, 70, 255),
                    TextAlignmentOptions.Center);
                status.fontStyle = FontStyles.Bold;
                status.characterSpacing = 5f;
                SetAnchored(status.rectTransform, new Vector2(0f, -118f), new Vector2(260f, 38f));

                cards.Add(new FactionCarousel.CardView
                {
                    Rect = card,
                    Group = group,
                    Status = status,
                    Locked = !unlocked
                });
            }

            var selectionLabel = CreateText(
                "FactionState",
                parent,
                string.Empty,
                22f,
                Cream,
                TextAlignmentOptions.Center);
            Anchor(selectionLabel.rectTransform, new Vector2(0.5f, 0.27f), new Vector2(820f, 62f));
            selectionLabel.fontStyle = FontStyles.Bold;
            selectionLabel.characterSpacing = 3f;

            carousel.Initialize(viewport, content, selectionLabel, data, cards);

            var previous = CreateDecoButton(
                "PreviousFaction",
                parent,
                "‹",
                new Vector2(84f, 96f),
                new Vector2(0.055f, 0.445f),
                Enamel,
                carousel.Previous);
            previous.GetComponentInChildren<TextMeshProUGUI>().fontSize = 52f;

            var next = CreateDecoButton(
                "NextFaction",
                parent,
                "›",
                new Vector2(84f, 96f),
                new Vector2(0.945f, 0.445f),
                Enamel,
                carousel.Next);
            next.GetComponentInChildren<TextMeshProUGUI>().fontSize = 52f;
        }

        private void CreateNavigation(
            RectTransform parent,
            Button campaignSource,
            Button settingsSource,
            Button hangarSource)
        {
            CreateDecoButton(
                "Campaigns",
                parent,
                "CAMPAIGNS",
                new Vector2(285f, 92f),
                new Vector2(0.22f, 0.19f),
                new Color32(87, 57, 39, 255),
                campaignSource == null ? null : campaignSource.onClick.Invoke);

            CreateDecoButton(
                "Hangar",
                parent,
                "HANGAR",
                new Vector2(250f, 92f),
                new Vector2(0.5f, 0.19f),
                Enamel,
                hangarSource == null ? null : hangarSource.onClick.Invoke);

            CreateDecoButton(
                "Settings",
                parent,
                "SETTINGS",
                new Vector2(250f, 92f),
                new Vector2(0.78f, 0.19f),
                Enamel,
                settingsSource == null ? null : settingsSource.onClick.Invoke);
        }

        private void CreateScorePlate(RectTransform parent)
        {
            var plate = CreateRect("BestSortiePlate", parent);
            Anchor(plate, new Vector2(0.5f, 0.075f), new Vector2(520f, 78f));
            var plaque = plate.gameObject.AddComponent<ArtDecoPlaqueGraphic>();
            plaque.SetPalette(new Color32(53, 58, 52, 238), Brass, Ink);
            plaque.raycastTarget = false;

            var score = CreateText(
                "BestSortie",
                plate,
                $"BEST SORTIE  •  {BestScore.CurrentBestScore}",
                25f,
                Cream,
                TextAlignmentOptions.Center);
            Stretch(score.rectTransform, 16f);
            score.fontStyle = FontStyles.Bold;
            score.characterSpacing = 3f;
            score.raycastTarget = false;
        }

        private Button CreateDecoButton(
            string name,
            RectTransform parent,
            string label,
            Vector2 size,
            Vector2 anchor,
            Color fill,
            UnityAction action)
        {
            var rect = CreateRect(name, parent);
            Anchor(rect, anchor, size);

            var plaque = rect.gameObject.AddComponent<ArtDecoPlaqueGraphic>();
            plaque.SetPalette(fill, Brass, Ink);

            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = plaque;
            button.transition = Selectable.Transition.ColorTint;
            button.colors = CreateButtonColors();
            if (action != null)
            {
                button.onClick.AddListener(action);
            }

            var text = CreateText(
                "Label",
                rect,
                label,
                26f,
                Cream,
                TextAlignmentOptions.Center);
            Stretch(text.rectTransform, 14f);
            text.fontStyle = FontStyles.Bold;
            text.characterSpacing = 4f;
            text.raycastTarget = false;

            return button;
        }

        private static ColorBlock CreateButtonColors()
        {
            var colors = ColorBlock.defaultColorBlock;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.08f, 1.05f, 0.92f, 1f);
            colors.pressedColor = new Color(0.72f, 0.72f, 0.68f, 1f);
            colors.selectedColor = new Color(1.05f, 1.02f, 0.9f, 1f);
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.55f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            return colors;
        }

        private static void EnsureMenuCamera()
        {
            if (Camera.allCamerasCount > 0)
            {
                return;
            }

            var cameraObject = new GameObject("Menu Camera", typeof(Camera));
            var menuCamera = cameraObject.GetComponent<Camera>();
            menuCamera.clearFlags = CameraClearFlags.SolidColor;
            menuCamera.backgroundColor = new Color32(224, 216, 181, 255);
            menuCamera.cullingMask = 0;
            menuCamera.depth = -100f;
        }

        private void CreateDecoRule(RectTransform parent, Vector2 anchor, float width)
        {
            var rule = CreateRect("DecoRule", parent);
            Anchor(rule, anchor, new Vector2(width, 5f));
            var image = rule.gameObject.AddComponent<Image>();
            image.color = Brass;
            image.raycastTarget = false;

            var diamond = CreateRect("Diamond", rule);
            diamond.anchorMin = diamond.anchorMax = new Vector2(0.5f, 0.5f);
            diamond.sizeDelta = new Vector2(16f, 16f);
            diamond.localRotation = Quaternion.Euler(0f, 0f, 45f);
            var diamondImage = diamond.gameObject.AddComponent<Image>();
            diamondImage.color = Brass;
            diamondImage.raycastTarget = false;
        }

        private TextMeshProUGUI CreateText(
            string name,
            RectTransform parent,
            string value,
            float fontSize,
            Color color,
            TextAlignmentOptions alignment)
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

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
            gameObject.layer = 5;
            var rect = (RectTransform)gameObject.transform;
            rect.SetParent(parent, false);
            rect.localScale = Vector3.one;
            return rect;
        }

        private static void Anchor(RectTransform rect, Vector2 anchor, Vector2 size)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
        }

        private static void SetAnchored(RectTransform rect, Vector2 position, Vector2 size)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static void Stretch(RectTransform rect, float inset = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }
    }

    internal sealed class RuntimeSafeAreaFitter : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Rect lastSafeArea;
        private Vector2Int lastScreenSize;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
            Apply();
        }

        private void Update()
        {
            var screenSize = new Vector2Int(Screen.width, Screen.height);
            if (Screen.safeArea != lastSafeArea || screenSize != lastScreenSize)
            {
                Apply();
            }
        }

        private void Apply()
        {
            if (rectTransform == null || Screen.width <= 0 || Screen.height <= 0)
            {
                return;
            }

            lastSafeArea = Screen.safeArea;
            lastScreenSize = new Vector2Int(Screen.width, Screen.height);

            var anchorMin = new Vector2(
                Mathf.Clamp01(lastSafeArea.xMin / Screen.width),
                Mathf.Clamp01(lastSafeArea.yMin / Screen.height));
            var anchorMax = new Vector2(
                Mathf.Clamp01(lastSafeArea.xMax / Screen.width),
                Mathf.Clamp01(lastSafeArea.yMax / Screen.height));

            if (anchorMax.x <= anchorMin.x || anchorMax.y <= anchorMin.y)
            {
                anchorMin = Vector2.zero;
                anchorMax = Vector2.one;
            }

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}
