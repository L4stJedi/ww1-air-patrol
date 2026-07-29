using System.Collections;
using SavedVariables.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scenes.Menu.MenuScripts
{
    /// <summary>
    /// Builds the shared period menu shell after the legacy menu scripts have initialized.
    /// Existing UnityEvents and save data remain the source of truth.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ArtDecoMenuSuite : MonoBehaviour
    {
        private TMP_FontAsset font;
        private Sprite backgroundSprite;

        public void Initialize(TMP_FontAsset menuFont)
        {
            font = menuFont != null ? menuFont : TMP_Settings.defaultFontAsset;
        }

        private IEnumerator Start()
        {
            yield return null;

            var mainMenu = transform;
            backgroundSprite = mainMenu.Find("Background")?.GetComponent<Image>()?.sprite;

            BuildSettings(FindCanvas("Settings"));
            BuildCampaigns(FindCanvas("Campaigns"));
            BuildHangar(FindCanvas("Hangar"));
        }

        private static Canvas FindCanvas(string name)
        {
            var canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include);
            foreach (var canvas in canvases)
            {
                if (canvas.name == name)
                {
                    return canvas;
                }
            }

            return null;
        }

        private void BuildSettings(Canvas canvas)
        {
            if (canvas == null || canvas.transform.Find("ArtDecoSafeArea") != null)
            {
                return;
            }

            PrepareCanvas(canvas, "LegacySettings");

            var safeArea = CreateSafeArea(canvas.transform);
            var backSource = FindLegacyButton(canvas.transform, "Back");
            ArtDecoUi.CreateHeader(
                safeArea,
                "SETTINGS",
                "FIELD PREFERENCES  •  KEEP FLYING SIMPLE",
                font,
                backSource == null ? null : backSource.onClick.Invoke);

            var panel = ArtDecoUi.CreatePanel(
                "PreferencesPanel",
                safeArea,
                new Vector2(0.5f, 0.56f),
                new Vector2(870f, 820f),
                new Color32(45, 50, 45, 247));

            var section = ArtDecoUi.CreateText(
                "SectionTitle",
                panel,
                "FLIGHT ASSISTS",
                30f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(section.rectTransform, new Vector2(0f, 325f), new Vector2(700f, 60f));
            section.fontStyle = FontStyles.Bold;
            section.characterSpacing = 5f;

            CreateSettingRow(
                panel,
                "HAPTIC FEEDBACK",
                "A short vibration confirms important actions.",
                new Vector2(0f, 160f),
                () => SavedSettings.IsHapticEnabled,
                value =>
                {
                    SavedSettings.IsHapticEnabled = value;
                    if (value)
                    {
                        HapticWrapper.Feedback(HapticType.Heavy);
                    }
                });

            CreateSettingRow(
                panel,
                "LEFT-HANDED CONTROLS",
                "Moves the climb control to the left side in flight.",
                new Vector2(0f, -70f),
                () => SavedSettings.IsLeftHanded,
                value => SavedSettings.IsLeftHanded = value);

            var controlPlate = ArtDecoUi.CreatePanel(
                "ControlsNote",
                panel,
                new Vector2(0.5f, 0.5f),
                new Vector2(740f, 180f),
                new Color32(86, 58, 40, 255));
            controlPlate.anchoredPosition = new Vector2(0f, -285f);

            var controlTitle = ArtDecoUi.CreateText(
                "ControlTitle",
                controlPlate,
                "ONE-BUTTON FLIGHT",
                25f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(controlTitle.rectTransform, new Vector2(0f, 38f), new Vector2(630f, 44f));
            controlTitle.fontStyle = FontStyles.Bold;
            controlTitle.characterSpacing = 4f;

            var controls = ArtDecoUi.CreateText(
                "Controls",
                controlPlate,
                "TAP  •  CLICK  •  SPACE  =  CLIMB",
                23f,
                ArtDecoTheme.Cream,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(controls.rectTransform, new Vector2(0f, -32f), new Vector2(650f, 54f));
            controls.characterSpacing = 3f;

            var note = ArtDecoUi.CreateText(
                "FooterNote",
                safeArea,
                "SETTINGS SAVE AUTOMATICALLY",
                21f,
                ArtDecoTheme.Burgundy,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.Anchor(note.rectTransform, new Vector2(0.5f, 0.22f), new Vector2(720f, 48f));
            note.fontStyle = FontStyles.Bold;
            note.characterSpacing = 3f;
        }

        private void CreateSettingRow(
            RectTransform parent,
            string label,
            string description,
            Vector2 position,
            System.Func<bool> getter,
            System.Action<bool> setter)
        {
            var row = ArtDecoUi.CreatePanel(
                label.Replace(" ", string.Empty),
                parent,
                new Vector2(0.5f, 0.5f),
                new Vector2(760f, 185f),
                ArtDecoTheme.EnamelLight);
            row.anchoredPosition = position;

            var title = ArtDecoUi.CreateText(
                "Label",
                row,
                label,
                26f,
                ArtDecoTheme.Cream,
                TextAlignmentOptions.Left,
                font);
            ArtDecoUi.SetAnchored(title.rectTransform, new Vector2(-120f, 38f), new Vector2(430f, 50f));
            title.fontStyle = FontStyles.Bold;
            title.characterSpacing = 3f;

            var body = ArtDecoUi.CreateText(
                "Description",
                row,
                description,
                19f,
                ArtDecoTheme.Muted,
                TextAlignmentOptions.Left,
                font);
            ArtDecoUi.SetAnchored(body.rectTransform, new Vector2(-120f, -30f), new Vector2(430f, 68f));

            var switchButton = ArtDecoUi.CreateButton(
                "Switch",
                row,
                string.Empty,
                new Vector2(0.83f, 0.5f),
                new Vector2(190f, 86f),
                getter() ? ArtDecoTheme.EnamelSelected : ArtDecoTheme.Brown,
                font);
            var stateText = switchButton.GetComponentInChildren<TextMeshProUGUI>();

            void Refresh()
            {
                var value = getter();
                stateText.text = value ? "ON" : "OFF";
                stateText.color = value ? ArtDecoTheme.Brass : ArtDecoTheme.Cream;
                switchButton.GetComponent<ArtDecoPlaqueGraphic>().SetPalette(
                    value ? ArtDecoTheme.EnamelSelected : ArtDecoTheme.Brown,
                    ArtDecoTheme.Brass,
                    ArtDecoTheme.Ink);
            }

            switchButton.onClick.AddListener(() =>
            {
                setter(!getter());
                Refresh();
            });
            Refresh();
        }

        private void BuildCampaigns(Canvas canvas)
        {
            if (canvas == null || canvas.transform.Find("ArtDecoSafeArea") != null)
            {
                return;
            }

            PrepareCanvas(canvas, "LegacyCampaigns");

            var safeArea = CreateSafeArea(canvas.transform);
            var backSource = FindLegacyButton(canvas.transform, "Back");
            ArtDecoUi.CreateHeader(
                safeArea,
                "CAMPAIGNS",
                "SQUADRON DISPATCHES  •  WESTERN FRONT",
                font,
                backSource == null ? null : backSource.onClick.Invoke);

            var routePanel = ArtDecoUi.CreatePanel(
                "CampaignRoute",
                safeArea,
                new Vector2(0.23f, 0.485f),
                new Vector2(360f, 1250f),
                new Color32(45, 50, 45, 246));

            var routeTitle = ArtDecoUi.CreateText(
                "RouteTitle",
                routePanel,
                "SERVICE ROUTE",
                26f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(routeTitle.rectTransform, new Vector2(0f, 530f), new Vector2(290f, 55f));
            routeTitle.fontStyle = FontStyles.Bold;
            routeTitle.characterSpacing = 4f;

            CreateCampaignPathLine(routePanel, new Vector2(0f, 45f), 820f);
            CreateCampaignNode(routePanel, "01", "BELGIUM", "1914", new Vector2(0f, 330f), true, "AVAILABLE");
            CreateCampaignNode(routePanel, "02", "FLANDERS", "1915", new Vector2(0f, 20f), false, "COMPLETE BELGIUM");
            CreateCampaignNode(routePanel, "03", "VERDUN", "1916", new Vector2(0f, -290f), false, "COMPLETE FLANDERS");

            var detail = ArtDecoUi.CreatePanel(
                "CampaignBriefing",
                safeArea,
                new Vector2(0.655f, 0.485f),
                new Vector2(590f, 1250f),
                new Color32(89, 60, 41, 246));

            var stamp = ArtDecoUi.CreateText(
                "StatusStamp",
                detail,
                "AVAILABLE",
                18f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(stamp.rectTransform, new Vector2(0f, 535f), new Vector2(250f, 45f));
            stamp.fontStyle = FontStyles.Bold;
            stamp.characterSpacing = 5f;

            var campaignName = ArtDecoUi.CreateText(
                "CampaignName",
                detail,
                "BELGIUM",
                45f,
                ArtDecoTheme.Cream,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(campaignName.rectTransform, new Vector2(0f, 465f), new Vector2(500f, 75f));
            campaignName.fontStyle = FontStyles.Bold;
            campaignName.characterSpacing = 6f;

            var metadata = ArtDecoUi.CreateText(
                "Metadata",
                detail,
                "1914  •  WESTERN FRONT  •  5 MISSIONS",
                19f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(metadata.rectTransform, new Vector2(0f, 405f), new Vector2(510f, 44f));
            metadata.characterSpacing = 2f;

            var briefingTitle = ArtDecoUi.CreateText(
                "BriefingTitle",
                detail,
                "FIELD BRIEFING",
                22f,
                ArtDecoTheme.Cream,
                TextAlignmentOptions.Left,
                font);
            ArtDecoUi.SetAnchored(briefingTitle.rectTransform, new Vector2(-5f, 322f), new Vector2(485f, 44f));
            briefingTitle.fontStyle = FontStyles.Bold;
            briefingTitle.characterSpacing = 4f;

            var briefing = ArtDecoUi.CreateText(
                "Briefing",
                detail,
                "The front is moving quickly. Fly the opening patrols, cross the flak screen, and challenge the enemy ace over the contested line.",
                22f,
                ArtDecoTheme.Muted,
                TextAlignmentOptions.TopLeft,
                font);
            ArtDecoUi.SetAnchored(briefing.rectTransform, new Vector2(0f, 205f), new Vector2(490f, 180f));
            briefing.overflowMode = TextOverflowModes.Truncate;

            CreateFactionPair(detail);
            CreateMissionLedger(detail);

            var start = ArtDecoUi.CreateButton(
                "StartSortie",
                detail,
                "START SORTIE  ›",
                new Vector2(0.5f, 0.085f),
                new Vector2(470f, 105f),
                ArtDecoTheme.EnamelSelected,
                font,
                () => SceneManager.LoadScene("TestLevel"),
                27f);
            start.GetComponentInChildren<TextMeshProUGUI>().characterSpacing = 4f;
        }

        private void CreateCampaignNode(
            RectTransform parent,
            string number,
            string name,
            string year,
            Vector2 position,
            bool available,
            string status)
        {
            var node = ArtDecoUi.CreatePanel(
                "Campaign_" + name,
                parent,
                new Vector2(0.5f, 0.5f),
                new Vector2(290f, 230f),
                available ? ArtDecoTheme.EnamelSelected : new Color32(57, 54, 48, 255));
            node.anchoredPosition = position;

            var numberText = ArtDecoUi.CreateText(
                "Number",
                node,
                number,
                23f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(numberText.rectTransform, new Vector2(0f, 70f), new Vector2(90f, 42f));
            numberText.fontStyle = FontStyles.Bold;

            var nameText = ArtDecoUi.CreateText(
                "Name",
                node,
                name,
                26f,
                ArtDecoTheme.Cream,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(nameText.rectTransform, new Vector2(0f, 18f), new Vector2(250f, 48f));
            nameText.fontStyle = FontStyles.Bold;
            nameText.characterSpacing = 3f;

            var yearText = ArtDecoUi.CreateText(
                "Year",
                node,
                year,
                18f,
                ArtDecoTheme.Muted,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(yearText.rectTransform, new Vector2(0f, -26f), new Vector2(180f, 35f));
            yearText.characterSpacing = 3f;

            var state = ArtDecoUi.CreateText(
                "State",
                node,
                status,
                15f,
                available ? ArtDecoTheme.Brass : ArtDecoTheme.Locked,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(state.rectTransform, new Vector2(0f, -76f), new Vector2(255f, 40f));
            state.fontStyle = FontStyles.Bold;
            state.characterSpacing = available ? 3f : 1f;
        }

        private static void CreateCampaignPathLine(RectTransform parent, Vector2 position, float height)
        {
            var line = ArtDecoUi.CreateRect("RouteLine", parent);
            ArtDecoUi.SetAnchored(line, position, new Vector2(6f, height));
            var image = line.gameObject.AddComponent<Image>();
            image.color = ArtDecoTheme.Brass;
            image.raycastTarget = false;
            line.SetAsFirstSibling();
        }

        private void CreateFactionPair(RectTransform parent)
        {
            var title = ArtDecoUi.CreateText(
                "AvailableSides",
                parent,
                "AVAILABLE SIDES",
                19f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(title.rectTransform, new Vector2(0f, 78f), new Vector2(420f, 40f));
            title.fontStyle = FontStyles.Bold;
            title.characterSpacing = 4f;

            CreateFactionBadge(parent, NationTabs.Nations.Germany, "GERMANY", new Vector2(-135f, 0f));
            CreateFactionBadge(parent, NationTabs.Nations.France, "FRANCE", new Vector2(135f, 0f));
        }

        private void CreateFactionBadge(
            RectTransform parent,
            NationTabs.Nations faction,
            string label,
            Vector2 position)
        {
            var badge = ArtDecoUi.CreatePanel(
                label,
                parent,
                new Vector2(0.5f, 0.5f),
                new Vector2(225f, 118f),
                ArtDecoTheme.Enamel);
            badge.anchoredPosition = position;

            var roundelRect = ArtDecoUi.CreateRect("Roundel", badge);
            ArtDecoUi.SetAnchored(roundelRect, new Vector2(-63f, 0f), new Vector2(70f, 70f));
            var roundel = roundelRect.gameObject.AddComponent<FactionInsigniaGraphic>();
            roundel.raycastTarget = false;
            roundel.SetFaction(faction, false);

            var text = ArtDecoUi.CreateText(
                "Label",
                badge,
                label,
                18f,
                ArtDecoTheme.Cream,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(text.rectTransform, new Vector2(38f, 0f), new Vector2(125f, 55f));
            text.fontStyle = FontStyles.Bold;
            text.characterSpacing = 2f;
        }

        private void CreateMissionLedger(RectTransform parent)
        {
            var ledgerTitle = ArtDecoUi.CreateText(
                "MissionLedgerTitle",
                parent,
                "MISSION LEDGER",
                19f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(ledgerTitle.rectTransform, new Vector2(0f, -102f), new Vector2(420f, 40f));
            ledgerTitle.fontStyle = FontStyles.Bold;
            ledgerTitle.characterSpacing = 4f;

            var missions = new[]
            {
                "01  OPENING PATROL",
                "02  FLAK SCREEN",
                "03  OBSERVATION LINE",
                "04  CONTESTED LINE",
                "05  ACE PURSUIT"
            };

            for (var i = 0; i < missions.Length; i++)
            {
                var row = ArtDecoUi.CreateText(
                    "Mission" + (i + 1),
                    parent,
                    missions[i],
                    18f,
                    i == 0 ? ArtDecoTheme.Cream : ArtDecoTheme.Muted,
                    TextAlignmentOptions.Left,
                    font);
                ArtDecoUi.SetAnchored(
                    row.rectTransform,
                    new Vector2(0f, -160f - i * 48f),
                    new Vector2(440f, 38f));
                if (i == 0)
                {
                    row.fontStyle = FontStyles.Bold;
                }
            }
        }

        private void BuildHangar(Canvas canvas)
        {
            if (canvas == null || canvas.transform.Find("ArtDecoSafeArea") != null)
            {
                return;
            }

            PrepareCanvas(canvas, "LegacyHangar");
            var presenter = canvas.gameObject.AddComponent<ArtDecoHangarScreen>();
            presenter.Initialize(font);
        }

        private void PrepareCanvas(Canvas canvas, string legacyName)
        {
            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080f, 1920f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            var legacy = new GameObject(legacyName, typeof(RectTransform), typeof(CanvasGroup));
            legacy.layer = 5;
            var legacyRect = (RectTransform)legacy.transform;
            legacyRect.SetParent(canvas.transform, false);
            ArtDecoUi.Stretch(legacyRect);
            legacyRect.SetAsFirstSibling();

            var existingChildren = new Transform[canvas.transform.childCount - 1];
            var index = 0;
            foreach (Transform child in canvas.transform)
            {
                if (child != legacyRect)
                {
                    existingChildren[index++] = child;
                }
            }

            foreach (var child in existingChildren)
            {
                child.SetParent(legacyRect, true);
            }

            var group = legacy.GetComponent<CanvasGroup>();
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;

            var background = ArtDecoUi.CreateImage(
                "ArtDecoBackground",
                canvas.transform,
                backgroundSprite,
                new Color(0.86f, 0.86f, 0.82f, 1f));
            ArtDecoUi.Stretch(background.rectTransform);
            background.transform.SetAsFirstSibling();

            var wash = ArtDecoUi.CreateImage(
                "ReadabilityWash",
                canvas.transform,
                null,
                new Color(0.93f, 0.88f, 0.72f, 0.08f));
            ArtDecoUi.Stretch(wash.rectTransform);
            wash.transform.SetSiblingIndex(1);
        }

        private static RectTransform CreateSafeArea(Transform canvas)
        {
            var safeArea = ArtDecoUi.CreateRect("ArtDecoSafeArea", canvas);
            ArtDecoUi.Stretch(safeArea);
            safeArea.gameObject.AddComponent<RuntimeSafeAreaFitter>();
            return safeArea;
        }

        private static Button FindLegacyButton(Transform canvas, string name)
        {
            var legacy = canvas.Find("LegacySettings") ??
                         canvas.Find("LegacyCampaigns") ??
                         canvas.Find("LegacyHangar");
            return ArtDecoUi.FindDeep(legacy, name)?.GetComponent<Button>();
        }
    }
}
