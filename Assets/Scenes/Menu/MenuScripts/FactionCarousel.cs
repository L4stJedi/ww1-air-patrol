using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scenes.Menu.MenuScripts
{
    public sealed class FactionCarousel :
        MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IScrollHandler
    {
        public readonly struct FactionData
        {
            public FactionData(NationTabs.Nations faction, string name, string service)
            {
                Faction = faction;
                Name = name;
                Service = service;
            }

            public NationTabs.Nations Faction { get; }
            public string Name { get; }
            public string Service { get; }
        }

        public sealed class CardView
        {
            public RectTransform Rect;
            public CanvasGroup Group;
            public TextMeshProUGUI Status;
            public bool Locked;
        }

        private const float CardWidth = 310f;
        private const float CardSpacing = 34f;
        private const float SnapSpeed = 13f;

        private readonly List<CardView> cards = new();
        private FactionData[] factionData = Array.Empty<FactionData>();
        private RectTransform viewport;
        private RectTransform content;
        private TextMeshProUGUI selectionLabel;
        private Canvas rootCanvas;
        private int focusedIndex;
        private float targetX;
        private bool dragging;

        public void Initialize(
            RectTransform viewportRect,
            RectTransform contentRect,
            TextMeshProUGUI currentSelectionLabel,
            FactionData[] data,
            List<CardView> cardViews)
        {
            viewport = viewportRect;
            content = contentRect;
            selectionLabel = currentSelectionLabel;
            factionData = data;
            cards.AddRange(cardViews);
            rootCanvas = GetComponentInParent<Canvas>();

            focusedIndex = Array.FindIndex(
                factionData,
                item => item.Faction == FactionUnlockProgress.SelectedFaction);
            if (focusedIndex < 0)
            {
                focusedIndex = 0;
            }

            SetFocusedIndex(focusedIndex, true);
        }

        private void Update()
        {
            if (content == null || viewport == null)
            {
                return;
            }

            if (!dragging)
            {
                targetX = CenterPositionForIndex(focusedIndex);
                var position = content.anchoredPosition;
                position.x = Mathf.Lerp(position.x, targetX, 1f - Mathf.Exp(-SnapSpeed * Time.unscaledDeltaTime));
                content.anchoredPosition = position;
            }

            UpdateCardPresentation();
        }

        public void Previous()
        {
            SetFocusedIndex(focusedIndex - 1, false);
        }

        public void Next()
        {
            SetFocusedIndex(focusedIndex + 1, false);
        }

        public void Focus(int index)
        {
            SetFocusedIndex(index, false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            var scaleFactor = rootCanvas == null ? 1f : rootCanvas.scaleFactor;
            var position = content.anchoredPosition;
            position.x += eventData.delta.x / Mathf.Max(0.01f, scaleFactor);

            var maximumX = CenterPositionForIndex(0) + 70f;
            var minimumX = CenterPositionForIndex(factionData.Length - 1) - 70f;
            position.x = Mathf.Clamp(position.x, minimumX, maximumX);
            content.anchoredPosition = position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
            var nearest = Mathf.RoundToInt(
                (viewport.rect.width * 0.5f - CardWidth * 0.5f - content.anchoredPosition.x)
                / (CardWidth + CardSpacing));
            SetFocusedIndex(nearest, false);
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (Mathf.Abs(eventData.scrollDelta.y) < 0.01f)
            {
                return;
            }

            SetFocusedIndex(focusedIndex + (eventData.scrollDelta.y < 0f ? 1 : -1), false);
        }

        private void SetFocusedIndex(int index, bool immediate)
        {
            if (factionData.Length == 0)
            {
                return;
            }

            focusedIndex = Mathf.Clamp(index, 0, factionData.Length - 1);
            targetX = CenterPositionForIndex(focusedIndex);

            if (immediate)
            {
                content.anchoredPosition = new Vector2(targetX, content.anchoredPosition.y);
            }

            var focusedFaction = factionData[focusedIndex];
            var unlocked = FactionUnlockProgress.TrySelect(focusedFaction.Faction);

            selectionLabel.text = unlocked
                ? $"{focusedFaction.Name.ToUpperInvariant()} SELECTED  •  OPEN CAMPAIGNS"
                : "LOCKED  •  COMPLETE A CAMPAIGN TO UNLOCK";
            selectionLabel.color = unlocked
                ? new Color32(76, 47, 32, 255)
                : new Color32(174, 88, 70, 255);

            for (var i = 0; i < cards.Count; i++)
            {
                if (cards[i].Locked)
                {
                    cards[i].Status.text = "LOCKED";
                    cards[i].Status.color = new Color32(174, 88, 70, 255);
                }
                else if (i == focusedIndex)
                {
                    cards[i].Status.text = "SELECTED";
                    cards[i].Status.color = new Color32(205, 170, 93, 255);
                }
                else
                {
                    cards[i].Status.text = "AVAILABLE";
                    cards[i].Status.color = new Color32(196, 191, 164, 255);
                }
            }
        }

        private float CenterPositionForIndex(int index)
        {
            var cardCenter = CardWidth * 0.5f + index * (CardWidth + CardSpacing);
            return viewport.rect.width * 0.5f - cardCenter;
        }

        private void UpdateCardPresentation()
        {
            var visibleCenter = viewport.rect.width * 0.5f - content.anchoredPosition.x;
            for (var i = 0; i < cards.Count; i++)
            {
                var distance = Mathf.Abs(cards[i].Rect.anchoredPosition.x - visibleCenter);
                var focus = 1f - Mathf.Clamp01(distance / (CardWidth + CardSpacing));
                var scale = Mathf.Lerp(0.82f, 1f, focus);
                cards[i].Rect.localScale = new Vector3(scale, scale, 1f);
                cards[i].Group.alpha = Mathf.Lerp(cards[i].Locked ? 0.42f : 0.62f, 1f, focus);
            }
        }
    }
}
