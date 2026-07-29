using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SavedVariables;
using SavedVariables.PilotUpgrades;
using SavedVariables.Planes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Menu.MenuScripts
{
    [DisallowMultipleComponent]
    public sealed class ArtDecoHangarScreen : MonoBehaviour
    {
        private sealed class PilotNodeView
        {
            internal SkillTreeNode Source;
            internal ArtDecoPlaqueGraphic Plaque;
            internal TextMeshProUGUI Label;
        }

        private TMP_FontAsset font;
        private Transform legacyRoot;
        private Transform legacyThumbnail;
        private Transform legacyEnhanceUi;
        private Transform legacyDescriptionBox;
        private Button legacyBack;
        private Button legacyPlanesTab;
        private Button legacyPilotTab;
        private NationTabs legacyNationTabs;

        private RectTransform aircraftPage;
        private RectTransform pilotPage;
        private RectTransform workshopPage;
        private RectTransform planeListContent;
        private TextMeshProUGUI xpText;
        private TextMeshProUGUI factionStatus;
        private TextMeshProUGUI detailName;
        private TextMeshProUGUI detailBio;
        private TextMeshProUGUI detailDescription;
        private TextMeshProUGUI detailState;
        private Image detailPlaneImage;
        private Button equipButton;
        private Button enhanceButton;
        private Button unlockButton;
        private Button aircraftTabButton;
        private Button pilotTabButton;
        private TextMeshProUGUI pilotSelectionTitle;
        private TextMeshProUGUI pilotSelectionDescription;
        private TextMeshProUGUI pilotSelectionCost;
        private Button authorizeButton;
        private TextMeshProUGUI workshopTitle;
        private TextMeshProUGUI workshopPoints;
        private RectTransform workshopRows;

        private readonly Dictionary<NationTabs.Nations, Button> factionButtons = new();
        private readonly List<GameObject> dynamicPlaneCards = new();
        private readonly List<GameObject> dynamicWorkshopRows = new();
        private readonly List<PilotNodeView> pilotNodes = new();

        private PlaneShowcaseProperties[] planeSources = Array.Empty<PlaneShowcaseProperties>();
        private PlaneShowcaseProperties selectedPlane;
        private SkillTreeNode selectedSkill;
        private NationTabs.Nations selectedNation = NationTabs.Nations.Germany;
        private int lastXp = int.MinValue;

        public void Initialize(TMP_FontAsset menuFont)
        {
            font = menuFont != null ? menuFont : TMP_Settings.defaultFontAsset;
            legacyRoot = transform.Find("LegacyHangar");
            if (legacyRoot == null)
            {
                return;
            }

            legacyBack = legacyRoot.Find("Panel/Container/Back")?.GetComponent<Button>();
            legacyPlanesTab = legacyRoot.Find("Panel/Container/Sections/Tabs/PlanesButton")?.GetComponent<Button>();
            legacyPilotTab = legacyRoot.Find("Panel/Container/Sections/Tabs/PilotButton")?.GetComponent<Button>();
            legacyThumbnail = ArtDecoUi.FindDeep(legacyRoot, "Thumbnail");
            legacyEnhanceUi = ArtDecoUi.FindDeep(legacyThumbnail, "EnhanceUI");
            legacyDescriptionBox = ArtDecoUi.FindDeep(legacyRoot, "Description Box");
            legacyNationTabs = legacyRoot.GetComponentInChildren<NationTabs>(true);
            planeSources = legacyRoot.GetComponentsInChildren<PlaneShowcaseProperties>(true);

            Build();
        }

        private void Update()
        {
            if (xpText == null || lastXp == BankedXp.BankedXpValue)
            {
                return;
            }

            lastXp = BankedXp.BankedXpValue;
            xpText.text = $"XP  •  {lastXp}";
            RefreshWorkshopSummary();
        }

        private void Build()
        {
            var safeArea = ArtDecoUi.CreateRect("ArtDecoSafeArea", transform);
            ArtDecoUi.Stretch(safeArea);
            safeArea.gameObject.AddComponent<RuntimeSafeAreaFitter>();

            ArtDecoUi.CreateHeader(
                safeArea,
                "HANGAR",
                "AIRCRAFT COLLECTION  •  PILOT SERVICE FILE",
                font,
                legacyBack == null ? null : legacyBack.onClick.Invoke);

            var xpPlate = ArtDecoUi.CreatePanel(
                "XpPlate",
                safeArea,
                new Vector2(0.865f, 0.925f),
                new Vector2(225f, 84f),
                ArtDecoTheme.Enamel);
            xpText = ArtDecoUi.CreateText(
                "XpText",
                xpPlate,
                string.Empty,
                22f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.Stretch(xpText.rectTransform, 12f);
            xpText.fontStyle = FontStyles.Bold;
            xpText.characterSpacing = 3f;

            aircraftTabButton = ArtDecoUi.CreateButton(
                "AircraftTab",
                safeArea,
                "AIRCRAFT",
                new Vector2(0.36f, 0.82f),
                new Vector2(310f, 92f),
                ArtDecoTheme.EnamelSelected,
                font,
                ShowAircraft,
                25f);

            pilotTabButton = ArtDecoUi.CreateButton(
                "PilotTab",
                safeArea,
                "PILOT",
                new Vector2(0.64f, 0.82f),
                new Vector2(310f, 92f),
                ArtDecoTheme.Enamel,
                font,
                ShowPilot,
                25f);

            var content = ArtDecoUi.CreateRect("HangarContent", safeArea);
            ArtDecoUi.Anchor(content, new Vector2(0.5f, 0.425f), new Vector2(960f, 1320f));

            BuildAircraftPage(content);
            BuildPilotPage(content);
            BuildWorkshopPage(content);

            selectedNation = FactionUnlockProgress.SelectedFaction;
            if (!FactionUnlockProgress.IsUnlocked(selectedNation))
            {
                selectedNation = NationTabs.Nations.Germany;
            }

            ShowNation(selectedNation);
            BuildPilotNodes();

            if (CurrentHangarTab.CurrentHangarTabValue == 1)
            {
                ShowPilot();
            }
            else
            {
                ShowAircraft();
            }

            lastXp = int.MinValue;
        }

        private void BuildAircraftPage(RectTransform parent)
        {
            aircraftPage = ArtDecoUi.CreateRect("AircraftPage", parent);
            ArtDecoUi.Stretch(aircraftPage);

            var factionPanel = ArtDecoUi.CreatePanel(
                "FactionRegistry",
                aircraftPage,
                new Vector2(0.5f, 0.5f),
                new Vector2(920f, 260f),
                new Color32(45, 50, 45, 247));
            factionPanel.anchoredPosition = new Vector2(0f, 510f);

            var title = ArtDecoUi.CreateText(
                "RegistryTitle",
                factionPanel,
                "SQUADRON REGISTRY",
                22f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(title.rectTransform, new Vector2(0f, 92f), new Vector2(500f, 42f));
            title.fontStyle = FontStyles.Bold;
            title.characterSpacing = 4f;

            var factions = new[]
            {
                (NationTabs.Nations.Germany, "GERMANY"),
                (NationTabs.Nations.Britain, "BRITAIN"),
                (NationTabs.Nations.France, "FRANCE"),
                (NationTabs.Nations.Italy, "ITALY"),
                (NationTabs.Nations.Russia, "RUSSIA"),
                (NationTabs.Nations.America, "USA")
            };

            for (var i = 0; i < factions.Length; i++)
            {
                var faction = factions[i].Item1;
                var unlocked = FactionUnlockProgress.IsUnlocked(faction);
                var column = i % 3;
                var row = i / 3;
                var label = unlocked ? factions[i].Item2 : $"{factions[i].Item2}  •  LOCKED";
                var button = ArtDecoUi.CreateButton(
                    "Faction_" + factions[i].Item2,
                    factionPanel,
                    label,
                    new Vector2(0.18f + column * 0.32f, 0.54f - row * 0.35f),
                    new Vector2(260f, 72f),
                    unlocked ? ArtDecoTheme.Enamel : new Color32(56, 52, 46, 255),
                    font,
                    () => ShowNation(faction),
                    unlocked ? 18f : 15f);
                factionButtons[faction] = button;
            }

            factionStatus = ArtDecoUi.CreateText(
                "FactionStatus",
                aircraftPage,
                string.Empty,
                19f,
                ArtDecoTheme.Ink,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(factionStatus.rectTransform, new Vector2(0f, 350f), new Vector2(880f, 48f));
            factionStatus.fontStyle = FontStyles.Bold;
            factionStatus.characterSpacing = 3f;

            var listPanel = ArtDecoUi.CreatePanel(
                "AircraftList",
                aircraftPage,
                new Vector2(0.5f, 0.5f),
                new Vector2(290f, 795f),
                new Color32(45, 50, 45, 247));
            listPanel.anchoredPosition = new Vector2(-320f, -90f);

            var listTitle = ArtDecoUi.CreateText(
                "ListTitle",
                listPanel,
                "AIRCRAFT",
                23f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(listTitle.rectTransform, new Vector2(0f, 340f), new Vector2(230f, 44f));
            listTitle.fontStyle = FontStyles.Bold;
            listTitle.characterSpacing = 4f;

            planeListContent = ArtDecoUi.CreateRect("ListContent", listPanel);
            ArtDecoUi.SetAnchored(planeListContent, new Vector2(0f, -35f), new Vector2(250f, 660f));

            var detailPanel = ArtDecoUi.CreatePanel(
                "AircraftDetail",
                aircraftPage,
                new Vector2(0.5f, 0.5f),
                new Vector2(610f, 795f),
                new Color32(88, 59, 40, 247));
            detailPanel.anchoredPosition = new Vector2(170f, -90f);

            detailName = ArtDecoUi.CreateText(
                "AircraftName",
                detailPanel,
                "NO AIRCRAFT FILE",
                34f,
                ArtDecoTheme.Cream,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(detailName.rectTransform, new Vector2(0f, 333f), new Vector2(540f, 62f));
            detailName.fontStyle = FontStyles.Bold;
            detailName.characterSpacing = 3f;

            detailBio = ArtDecoUi.CreateText(
                "AircraftBio",
                detailPanel,
                string.Empty,
                18f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(detailBio.rectTransform, new Vector2(0f, 286f), new Vector2(530f, 42f));
            detailBio.characterSpacing = 2f;

            var imagePlate = ArtDecoUi.CreatePanel(
                "AircraftImagePlate",
                detailPanel,
                new Vector2(0.5f, 0.5f),
                new Vector2(520f, 300f),
                new Color32(236, 220, 166, 245));
            imagePlate.anchoredPosition = new Vector2(0f, 105f);

            detailPlaneImage = ArtDecoUi.CreateImage(
                "AircraftImage",
                imagePlate,
                null,
                Color.white,
                true);
            ArtDecoUi.Stretch(detailPlaneImage.rectTransform, 34f);

            detailDescription = ArtDecoUi.CreateText(
                "AircraftDescription",
                detailPanel,
                "Choose an available squadron to inspect its aircraft.",
                20f,
                ArtDecoTheme.Muted,
                TextAlignmentOptions.TopLeft,
                font);
            ArtDecoUi.SetAnchored(detailDescription.rectTransform, new Vector2(0f, -112f), new Vector2(515f, 110f));

            detailState = ArtDecoUi.CreateText(
                "AircraftState",
                detailPanel,
                string.Empty,
                18f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(detailState.rectTransform, new Vector2(0f, -205f), new Vector2(510f, 45f));
            detailState.fontStyle = FontStyles.Bold;
            detailState.characterSpacing = 3f;

            equipButton = ArtDecoUi.CreateButton(
                "Equip",
                detailPanel,
                "EQUIP",
                new Vector2(0.32f, 0.085f),
                new Vector2(245f, 92f),
                ArtDecoTheme.EnamelSelected,
                font,
                EquipSelected,
                22f);

            enhanceButton = ArtDecoUi.CreateButton(
                "Enhance",
                detailPanel,
                "ENHANCE",
                new Vector2(0.72f, 0.085f),
                new Vector2(245f, 92f),
                ArtDecoTheme.Enamel,
                font,
                ShowWorkshop,
                22f);

            unlockButton = ArtDecoUi.CreateButton(
                "Unlock",
                detailPanel,
                "UNLOCK  •  10 XP",
                new Vector2(0.5f, 0.085f),
                new Vector2(500f, 92f),
                ArtDecoTheme.Brown,
                font,
                UnlockSelected,
                22f);
        }

        private void BuildPilotPage(RectTransform parent)
        {
            pilotPage = ArtDecoUi.CreateRect("PilotPage", parent);
            ArtDecoUi.Stretch(pilotPage);

            var servicePlate = ArtDecoUi.CreatePanel(
                "ServiceRecord",
                pilotPage,
                new Vector2(0.5f, 0.5f),
                new Vector2(900f, 145f),
                ArtDecoTheme.Brown);
            servicePlate.anchoredPosition = new Vector2(0f, 530f);

            var profile = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID);
            var pilotName = string.IsNullOrWhiteSpace(profile.statData.pilotName)
                ? $"PILOT {CurrentPilot.PilotID + 1:00}"
                : profile.statData.pilotName.ToUpperInvariant();

            var name = ArtDecoUi.CreateText(
                "PilotName",
                servicePlate,
                pilotName,
                30f,
                ArtDecoTheme.Cream,
                TextAlignmentOptions.Left,
                font);
            ArtDecoUi.SetAnchored(name.rectTransform, new Vector2(-170f, 28f), new Vector2(470f, 52f));
            name.fontStyle = FontStyles.Bold;
            name.characterSpacing = 4f;

            var rank = ArtDecoUi.CreateText(
                "Rank",
                servicePlate,
                "FLIGHT CADET  •  SERVICE FILE",
                18f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Left,
                font);
            ArtDecoUi.SetAnchored(rank.rectTransform, new Vector2(-170f, -25f), new Vector2(470f, 40f));
            rank.characterSpacing = 2f;

            var xp = ArtDecoUi.CreateText(
                "PilotXp",
                servicePlate,
                $"AVAILABLE XP  •  {BankedXp.BankedXpValue}",
                20f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(xp.rectTransform, new Vector2(285f, 0f), new Vector2(250f, 62f));
            xp.fontStyle = FontStyles.Bold;

            var doctrinePanel = ArtDecoUi.CreatePanel(
                "DoctrinePanel",
                pilotPage,
                new Vector2(0.5f, 0.5f),
                new Vector2(900f, 900f),
                new Color32(45, 50, 45, 247));
            doctrinePanel.anchoredPosition = new Vector2(0f, -5f);

            var labels = new[] { "MECHANIC", "TACTICIAN", "ACROBAT" };
            var xs = new[] { -290f, 0f, 290f };
            for (var i = 0; i < labels.Length; i++)
            {
                var heading = ArtDecoUi.CreateText(
                    labels[i],
                    doctrinePanel,
                    labels[i],
                    21f,
                    ArtDecoTheme.Brass,
                    TextAlignmentOptions.Center,
                    font);
                ArtDecoUi.SetAnchored(heading.rectTransform, new Vector2(xs[i], 378f), new Vector2(245f, 44f));
                heading.fontStyle = FontStyles.Bold;
                heading.characterSpacing = 3f;

                var line = ArtDecoUi.CreateImage(
                    labels[i] + "Line",
                    doctrinePanel,
                    null,
                    ArtDecoTheme.Brass);
                ArtDecoUi.SetAnchored(line.rectTransform, new Vector2(xs[i], 35f), new Vector2(5f, 620f));
                line.transform.SetAsFirstSibling();
            }

            var selection = ArtDecoUi.CreatePanel(
                "SelectedSkill",
                pilotPage,
                new Vector2(0.5f, 0.5f),
                new Vector2(900f, 255f),
                new Color32(88, 59, 40, 247));
            selection.anchoredPosition = new Vector2(0f, -585f);

            pilotSelectionTitle = ArtDecoUi.CreateText(
                "SkillTitle",
                selection,
                "SELECT A TRAINING BADGE",
                23f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Left,
                font);
            ArtDecoUi.SetAnchored(pilotSelectionTitle.rectTransform, new Vector2(-120f, 72f), new Vector2(580f, 45f));
            pilotSelectionTitle.fontStyle = FontStyles.Bold;
            pilotSelectionTitle.characterSpacing = 3f;

            pilotSelectionDescription = ArtDecoUi.CreateText(
                "SkillDescription",
                selection,
                "Inspect a doctrine badge to review its service benefit.",
                19f,
                ArtDecoTheme.Cream,
                TextAlignmentOptions.TopLeft,
                font);
            ArtDecoUi.SetAnchored(pilotSelectionDescription.rectTransform, new Vector2(-120f, -15f), new Vector2(580f, 105f));

            pilotSelectionCost = ArtDecoUi.CreateText(
                "SkillCost",
                selection,
                string.Empty,
                18f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(pilotSelectionCost.rectTransform, new Vector2(305f, 66f), new Vector2(210f, 40f));
            pilotSelectionCost.fontStyle = FontStyles.Bold;

            authorizeButton = ArtDecoUi.CreateButton(
                "Authorize",
                selection,
                "AUTHORIZE",
                new Vector2(0.84f, 0.35f),
                new Vector2(235f, 88f),
                ArtDecoTheme.EnamelSelected,
                font,
                AuthorizeSelectedSkill,
                19f);
            authorizeButton.gameObject.SetActive(false);
        }

        private void BuildWorkshopPage(RectTransform parent)
        {
            workshopPage = ArtDecoUi.CreateRect("WorkshopPage", parent);
            ArtDecoUi.Stretch(workshopPage);
            workshopPage.gameObject.SetActive(false);

            var panel = ArtDecoUi.CreatePanel(
                "WorkshopPanel",
                workshopPage,
                new Vector2(0.5f, 0.5f),
                new Vector2(900f, 1190f),
                new Color32(45, 50, 45, 250));

            var section = ArtDecoUi.CreateText(
                "WorkshopLabel",
                panel,
                "FIELD WORKSHOP",
                20f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(section.rectTransform, new Vector2(0f, 515f), new Vector2(500f, 44f));
            section.fontStyle = FontStyles.Bold;
            section.characterSpacing = 5f;

            workshopTitle = ArtDecoUi.CreateText(
                "WorkshopTitle",
                panel,
                "AIRCRAFT ENHANCEMENT",
                36f,
                ArtDecoTheme.Cream,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(workshopTitle.rectTransform, new Vector2(0f, 455f), new Vector2(760f, 65f));
            workshopTitle.fontStyle = FontStyles.Bold;
            workshopTitle.characterSpacing = 4f;

            workshopPoints = ArtDecoUi.CreateText(
                "WorkshopPoints",
                panel,
                string.Empty,
                20f,
                ArtDecoTheme.Brass,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(workshopPoints.rectTransform, new Vector2(0f, 400f), new Vector2(650f, 40f));
            workshopPoints.fontStyle = FontStyles.Bold;
            workshopPoints.characterSpacing = 3f;

            workshopRows = ArtDecoUi.CreateRect("UpgradeRows", panel);
            ArtDecoUi.SetAnchored(workshopRows, new Vector2(0f, -20f), new Vector2(800f, 760f));

            ArtDecoUi.CreateButton(
                "WorkshopBack",
                panel,
                "‹  AIRCRAFT FILE",
                new Vector2(0.5f, 0.075f),
                new Vector2(410f, 94f),
                ArtDecoTheme.Brown,
                font,
                CloseWorkshop,
                22f);
        }

        private void ShowAircraft()
        {
            CurrentHangarTab.CurrentHangarTabValue = 0;
            aircraftPage.gameObject.SetActive(true);
            pilotPage.gameObject.SetActive(false);
            workshopPage.gameObject.SetActive(false);
            UpdateTabStyle(false);
            ShowNation(selectedNation);
        }

        private void ShowPilot()
        {
            CurrentHangarTab.CurrentHangarTabValue = 1;
            aircraftPage.gameObject.SetActive(false);
            pilotPage.gameObject.SetActive(true);
            workshopPage.gameObject.SetActive(false);
            UpdateTabStyle(true);
            RefreshPilotNodeStates();
        }

        private void UpdateTabStyle(bool pilotSelected)
        {
            SetButtonPalette(
                aircraftTabButton,
                pilotSelected ? ArtDecoTheme.Enamel : ArtDecoTheme.EnamelSelected,
                !pilotSelected);
            SetButtonPalette(
                pilotTabButton,
                pilotSelected ? ArtDecoTheme.EnamelSelected : ArtDecoTheme.Enamel,
                pilotSelected);
        }

        private static void SetButtonPalette(Button button, Color fill, bool selected)
        {
            if (button == null)
            {
                return;
            }

            button.GetComponent<ArtDecoPlaqueGraphic>()?.SetPalette(
                fill,
                ArtDecoTheme.Brass,
                ArtDecoTheme.Ink);
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.color = selected ? ArtDecoTheme.Brass : ArtDecoTheme.Cream;
            }
        }

        private void ShowNation(NationTabs.Nations nation)
        {
            selectedNation = nation;
            var unlocked = FactionUnlockProgress.IsUnlocked(nation);
            if (unlocked)
            {
                FactionUnlockProgress.TrySelect(nation);
                legacyNationTabs?.SwapNationTab(nation);
            }

            foreach (var pair in factionButtons)
            {
                var isSelected = pair.Key == nation;
                var isUnlocked = FactionUnlockProgress.IsUnlocked(pair.Key);
                var fill = !isUnlocked
                    ? new Color32(56, 52, 46, 255)
                    : isSelected
                        ? ArtDecoTheme.EnamelSelected
                        : ArtDecoTheme.Enamel;
                SetButtonPalette(pair.Value, fill, isSelected && isUnlocked);
            }

            ClearPlaneCards();
            selectedPlane = null;

            if (!unlocked)
            {
                factionStatus.text = "LOCKED  •  COMPLETE A CAMPAIGN TO OPEN THIS SQUADRON";
                factionStatus.color = ArtDecoTheme.Locked;
                SetEmptyDetail("LOCKED SQUADRON", "Complete the required campaign to open its aircraft registry.");
                return;
            }

            factionStatus.text = $"{nation.ToString().ToUpperInvariant()}  •  SQUADRON AVAILABLE";
            factionStatus.color = ArtDecoTheme.Ink;

            var matchingPlanes = planeSources
                .Where(source => GetNation(source.planeType) == nation)
                .OrderBy(source => (int)source.planeType)
                .ToArray();

            if (matchingPlanes.Length == 0)
            {
                SetEmptyDetail(
                    $"{nation.ToString().ToUpperInvariant()} REGISTRY",
                    "No aircraft dossier has been issued for this squadron yet.");
                return;
            }

            for (var i = 0; i < matchingPlanes.Length; i++)
            {
                CreatePlaneCard(matchingPlanes[i], i);
            }

            SelectPlane(matchingPlanes[0]);
        }

        private void CreatePlaneCard(PlaneShowcaseProperties source, int index)
        {
            var rect = ArtDecoUi.CreateRect("Plane_" + source.planeType, planeListContent);
            ArtDecoUi.SetAnchored(rect, new Vector2(0f, 245f - index * 165f), new Vector2(230f, 140f));

            var plaque = rect.gameObject.AddComponent<ArtDecoPlaqueGraphic>();
            plaque.SetPalette(ArtDecoTheme.Brown, ArtDecoTheme.Brass, ArtDecoTheme.Ink);

            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = plaque;
            button.colors = ArtDecoUi.ButtonColors();
            button.onClick.AddListener(() => SelectPlane(source));

            var sourceImage = source.transform.Find("PlaneImage")?.GetComponent<Image>();
            var icon = ArtDecoUi.CreateImage(
                "PlaneImage",
                rect,
                sourceImage == null ? null : sourceImage.sprite,
                Color.white,
                true);
            ArtDecoUi.SetAnchored(icon.rectTransform, new Vector2(0f, 23f), new Vector2(150f, 72f));

            var name = ArtDecoUi.CreateText(
                "Name",
                rect,
                ListOfPlanes.GetDisplayName(source.planeType).ToUpperInvariant(),
                17f,
                ArtDecoTheme.Cream,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.SetAnchored(name.rectTransform, new Vector2(0f, -46f), new Vector2(200f, 42f));
            name.fontStyle = FontStyles.Bold;
            name.characterSpacing = 1f;

            dynamicPlaneCards.Add(rect.gameObject);
        }

        private void SelectPlane(PlaneShowcaseProperties source)
        {
            selectedPlane = source;
            source.GetComponent<Button>()?.onClick.Invoke();
            source.CallSwapThumbnail();
            source.CallSwapEnhanceTarget();

            detailName.text = ListOfPlanes.GetDisplayName(source.planeType).ToUpperInvariant();

            var thumbnailName = legacyThumbnail?.Find("AirplaneName")?.GetComponent<TextMeshProUGUI>();
            if (thumbnailName != null && !string.IsNullOrWhiteSpace(thumbnailName.text))
            {
                detailName.text = thumbnailName.text.ToUpperInvariant();
            }

            detailBio.text = legacyThumbnail?.Find("BIO")?.GetComponent<TextMeshProUGUI>()?.text ?? string.Empty;
            detailDescription.text =
                legacyThumbnail?.Find("Description")?.GetComponent<TextMeshProUGUI>()?.text ??
                "Aircraft dossier unavailable.";

            var legacyImage = legacyThumbnail?.Find("PlaneBackground/PlaneImage")?.GetComponent<Image>();
            detailPlaneImage.enabled = true;
            detailPlaneImage.sprite = legacyImage != null
                ? legacyImage.sprite
                : source.transform.Find("PlaneImage")?.GetComponent<Image>()?.sprite;

            var planeData = PlaneSaveSystem.LoadPlaneProperties(source.planeType);
            var isEquipped = CurrentPlane.EquippedPlane == source.planeType;
            if (planeData.isUnlocked)
            {
                detailState.text = isEquipped ? "EQUIPPED  •  READY FOR SORTIE" : "OWNED  •  READY TO EQUIP";
                detailState.color = ArtDecoTheme.Brass;
                equipButton.gameObject.SetActive(true);
                enhanceButton.gameObject.SetActive(true);
                unlockButton.gameObject.SetActive(false);
                equipButton.GetComponentInChildren<TextMeshProUGUI>().text = isEquipped ? "EQUIPPED" : "EQUIP";
                equipButton.interactable = !isEquipped;
            }
            else
            {
                detailState.text = "LOCKED  •  REQUIRES 10 XP";
                detailState.color = ArtDecoTheme.Locked;
                equipButton.gameObject.SetActive(false);
                enhanceButton.gameObject.SetActive(false);
                unlockButton.gameObject.SetActive(true);
            }
        }

        private void EquipSelected()
        {
            if (selectedPlane == null)
            {
                return;
            }

            CurrentPlane.EquippedPlane = selectedPlane.planeType;
            SelectPlane(selectedPlane);
        }

        private void UnlockSelected()
        {
            if (selectedPlane == null || legacyThumbnail == null)
            {
                return;
            }

            legacyThumbnail.Find("Unlock")?.GetComponent<Button>()?.onClick.Invoke();
            SelectPlane(selectedPlane);
        }

        private void ShowWorkshop()
        {
            if (selectedPlane == null)
            {
                return;
            }

            selectedPlane.CallSwapEnhanceTarget();
            if (legacyEnhanceUi != null)
            {
                legacyEnhanceUi.gameObject.SetActive(true);
            }

            aircraftPage.gameObject.SetActive(false);
            pilotPage.gameObject.SetActive(false);
            workshopPage.gameObject.SetActive(true);
            workshopTitle.text = $"{ListOfPlanes.GetDisplayName(selectedPlane.planeType).ToUpperInvariant()}  •  ENHANCEMENT";
            BuildWorkshopRows();
            RefreshWorkshopSummary();
        }

        private void CloseWorkshop()
        {
            if (legacyEnhanceUi != null)
            {
                legacyEnhanceUi.gameObject.SetActive(false);
            }

            workshopPage.gameObject.SetActive(false);
            aircraftPage.gameObject.SetActive(true);
            UpdateTabStyle(false);
            SelectPlane(selectedPlane);
        }

        private void BuildWorkshopRows()
        {
            foreach (var row in dynamicWorkshopRows)
            {
                Destroy(row);
            }

            dynamicWorkshopRows.Clear();
            if (legacyEnhanceUi == null)
            {
                return;
            }

            var rowNames = new[] { "fallSpeed", "jumpStrength", "fireSpeed", "fireDistance" };
            for (var i = 0; i < rowNames.Length; i++)
            {
                var sourceRow = legacyEnhanceUi.Find(rowNames[i]);
                if (sourceRow == null)
                {
                    continue;
                }

                var row = ArtDecoUi.CreatePanel(
                    "Upgrade_" + rowNames[i],
                    workshopRows,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(750f, 150f),
                    ArtDecoTheme.EnamelLight);
                row.anchoredPosition = new Vector2(0f, 260f - i * 175f);

                var labelSource = sourceRow.GetComponentInChildren<TextMeshProUGUI>(true);
                var label = ArtDecoUi.CreateText(
                    "StatName",
                    row,
                    labelSource == null ? Humanize(rowNames[i]) : labelSource.text.ToUpperInvariant(),
                    23f,
                    ArtDecoTheme.Cream,
                    TextAlignmentOptions.Left,
                    font);
                ArtDecoUi.SetAnchored(label.rectTransform, new Vector2(-190f, 23f), new Vector2(300f, 48f));
                label.fontStyle = FontStyles.Bold;
                label.characterSpacing = 2f;

                var sourceLevel = ArtDecoUi.FindDeep(sourceRow, "LevelText")?.GetComponent<TextMeshProUGUI>();
                var level = ArtDecoUi.CreateText(
                    "Level",
                    row,
                    sourceLevel == null ? "100%" : sourceLevel.text,
                    19f,
                    ArtDecoTheme.Brass,
                    TextAlignmentOptions.Left,
                    font);
                ArtDecoUi.SetAnchored(level.rectTransform, new Vector2(-190f, -30f), new Vector2(300f, 40f));

                var sourceMinus = FindChildButton(sourceRow, "-");
                var sourcePlus = FindChildButton(sourceRow, "+");

                ArtDecoUi.CreateButton(
                    "Decrease",
                    row,
                    "−",
                    new Vector2(0.72f, 0.5f),
                    new Vector2(95f, 92f),
                    ArtDecoTheme.Brown,
                    font,
                    () =>
                    {
                        sourceMinus?.onClick.Invoke();
                        if (sourceLevel != null)
                        {
                            level.text = sourceLevel.text;
                        }
                        RefreshWorkshopSummary();
                    },
                    34f);

                ArtDecoUi.CreateButton(
                    "Increase",
                    row,
                    "+",
                    new Vector2(0.88f, 0.5f),
                    new Vector2(95f, 92f),
                    ArtDecoTheme.EnamelSelected,
                    font,
                    () =>
                    {
                        sourcePlus?.onClick.Invoke();
                        if (sourceLevel != null)
                        {
                            level.text = sourceLevel.text;
                        }
                        RefreshWorkshopSummary();
                    },
                    32f);

                dynamicWorkshopRows.Add(row.gameObject);
            }
        }

        private void RefreshWorkshopSummary()
        {
            if (workshopPoints == null || selectedPlane == null)
            {
                return;
            }

            var data = PlaneSaveSystem.LoadPlaneProperties(selectedPlane.planeType);
            var used = data.enhanceFallSpeed + data.enhanceJumpStrength + data.enhanceFireSpeed + data.enhanceFireRange;
            workshopPoints.text = $"ENHANCEMENTS  {used}/{data.enhancePointMaximum}   •   AVAILABLE XP  {BankedXp.BankedXpValue}";
        }

        private void BuildPilotNodes()
        {
            var doctrinePanel = pilotPage.Find("DoctrinePanel") as RectTransform;
            if (doctrinePanel == null)
            {
                return;
            }

            var allNodes = legacyRoot.GetComponentsInChildren<SkillTreeNode>(true);
            BuildDoctrineColumn(
                doctrinePanel,
                allNodes.Where(node => node.type == SkillTreeNode.Type.Mechanic).ToArray(),
                -290f);
            BuildDoctrineColumn(
                doctrinePanel,
                allNodes.Where(node => node.type == SkillTreeNode.Type.Tactician).ToArray(),
                0f);
            BuildDoctrineColumn(
                doctrinePanel,
                allNodes.Where(node => node.type == SkillTreeNode.Type.Acrobat).ToArray(),
                290f);

            var specials = allNodes
                .Where(node => node.type == SkillTreeNode.Type.Special)
                .OrderBy(node => node.name)
                .ToArray();
            for (var i = 0; i < specials.Length; i++)
            {
                CreatePilotNode(
                    doctrinePanel,
                    specials[i],
                    new Vector2(i == 0 ? -145f : 145f, -365f),
                    $"SPECIAL {i + 1}");
            }

            RefreshPilotNodeStates();
        }

        private void BuildDoctrineColumn(RectTransform parent, SkillTreeNode[] sourceNodes, float x)
        {
            var ordered = sourceNodes
                .OrderByDescending(node => ((RectTransform)node.transform).anchoredPosition.y)
                .ToArray();

            for (var i = 0; i < ordered.Length; i++)
            {
                CreatePilotNode(
                    parent,
                    ordered[i],
                    new Vector2(x, 270f - i * 170f),
                    $"TIER {i + 1}");
            }
        }

        private void CreatePilotNode(
            RectTransform parent,
            SkillTreeNode source,
            Vector2 position,
            string tier)
        {
            var rect = ArtDecoUi.CreateRect("PilotNode_" + source.name, parent);
            ArtDecoUi.SetAnchored(rect, position, new Vector2(230f, 120f));
            var plaque = rect.gameObject.AddComponent<ArtDecoPlaqueGraphic>();
            plaque.SetPalette(ArtDecoTheme.Brown, ArtDecoTheme.Brass, ArtDecoTheme.Ink);

            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = plaque;
            button.colors = ArtDecoUi.ButtonColors();
            button.onClick.AddListener(() => SelectPilotNode(source));

            var label = ArtDecoUi.CreateText(
                "Label",
                rect,
                $"{tier}\n{SkillDisplayName(source)}",
                16f,
                ArtDecoTheme.Cream,
                TextAlignmentOptions.Center,
                font);
            ArtDecoUi.Stretch(label.rectTransform, 10f);
            label.fontStyle = FontStyles.Bold;
            label.characterSpacing = 1f;
            label.raycastTarget = false;

            pilotNodes.Add(new PilotNodeView
            {
                Source = source,
                Plaque = plaque,
                Label = label
            });
        }

        private void SelectPilotNode(SkillTreeNode source)
        {
            selectedSkill = source;
            var details = source.GetComponent<SendToDescriptionBox>();
            var profile = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID);
            var unlocked = details != null &&
                           profile.skillData.unlockedSkillsIds.Contains(details.ownedSkill.ToString());

            pilotSelectionTitle.text = SkillDisplayName(source).ToUpperInvariant();
            if (unlocked)
            {
                pilotSelectionDescription.text = "Recorded in the pilot service file.";
                pilotSelectionCost.text = "AUTHORIZED";
                authorizeButton.gameObject.SetActive(false);
                return;
            }

            details?.SendToDesBox();
            pilotSelectionDescription.text =
                legacyDescriptionBox?.Find("Description")?.GetComponent<TextMeshProUGUI>()?.text ??
                "Training details unavailable.";
            pilotSelectionCost.text =
                legacyDescriptionBox?.Find("Cost")?.GetComponent<TextMeshProUGUI>()?.text?.ToUpperInvariant() ??
                string.Empty;
            authorizeButton.gameObject.SetActive(true);
        }

        private void AuthorizeSelectedSkill()
        {
            if (selectedSkill == null || legacyDescriptionBox == null)
            {
                return;
            }

            selectedSkill.GetComponent<SendToDescriptionBox>()?.SendToDesBox();
            legacyDescriptionBox.Find("Button")?.GetComponent<Button>()?.onClick.Invoke();
            RefreshPilotNodeStates();
            SelectPilotNode(selectedSkill);
        }

        private void RefreshPilotNodeStates()
        {
            var profile = PilotSaveSystem.LoadPilotData(CurrentPilot.PilotID);
            foreach (var node in pilotNodes)
            {
                var details = node.Source.GetComponent<SendToDescriptionBox>();
                var unlocked = details != null &&
                               profile.skillData.unlockedSkillsIds.Contains(details.ownedSkill.ToString());
                node.Plaque.SetPalette(
                    unlocked ? ArtDecoTheme.EnamelSelected : ArtDecoTheme.Brown,
                    ArtDecoTheme.Brass,
                    ArtDecoTheme.Ink);
                node.Label.color = unlocked ? ArtDecoTheme.Brass : ArtDecoTheme.Cream;
            }
        }

        private void SetEmptyDetail(string title, string description)
        {
            detailName.text = title;
            detailBio.text = string.Empty;
            detailDescription.text = description;
            detailPlaneImage.sprite = null;
            detailPlaneImage.enabled = false;
            detailState.text = string.Empty;
            equipButton.gameObject.SetActive(false);
            enhanceButton.gameObject.SetActive(false);
            unlockButton.gameObject.SetActive(false);
        }

        private void ClearPlaneCards()
        {
            foreach (var card in dynamicPlaneCards)
            {
                Destroy(card);
            }

            dynamicPlaneCards.Clear();
        }

        private static NationTabs.Nations GetNation(ListOfPlanes.Planes plane)
        {
            return ListOfPlanes.GetNation(plane) switch
            {
                ListOfPlanes.PlaneNation.Germany => NationTabs.Nations.Germany,
                ListOfPlanes.PlaneNation.Britain => NationTabs.Nations.Britain,
                ListOfPlanes.PlaneNation.France => NationTabs.Nations.France,
                ListOfPlanes.PlaneNation.Italy => NationTabs.Nations.Italy,
                ListOfPlanes.PlaneNation.Russia => NationTabs.Nations.Russia,
                ListOfPlanes.PlaneNation.America => NationTabs.Nations.America,
                _ => NationTabs.Nations.Germany
            };
        }

        private static Button FindChildButton(Transform root, string name)
        {
            var buttons = root.GetComponentsInChildren<Button>(true);
            return buttons.FirstOrDefault(button => button.name == name);
        }

        private static string SkillDisplayName(SkillTreeNode node)
        {
            if (node.name == "M3Repair")
            {
                return "FIELD REPAIR";
            }

            if (node.name == "SA0")
            {
                var specialText = node.GetComponentInChildren<TextMeshProUGUI>(true)?.text;
                return string.IsNullOrWhiteSpace(specialText) ? "SHIELD TRAINING" : specialText.ToUpperInvariant();
            }

            if (node.name == "SB0")
            {
                return "SPECIAL MANOEUVRE";
            }

            var raw = node.name;
            var underscore = raw.IndexOf('_');
            if (underscore >= 0 && underscore < raw.Length - 1)
            {
                raw = raw[(underscore + 1)..];
            }
            else if (Regex.IsMatch(raw, "^[AMT]\\d+$"))
            {
                return "ADVANCED TRAINING";
            }

            raw = Regex.Replace(raw, "(?<=[a-z])(?=[A-Z0-9])", " ");
            raw = Regex.Replace(raw, "(?<=[A-Z])(?=[0-9])", " ");
            return raw.Replace("1", " I").Replace("2", " II").Trim().ToUpperInvariant();
        }

        private static string Humanize(string value)
        {
            return Regex.Replace(value, "(?<=[a-z])(?=[A-Z])", " ").ToUpperInvariant();
        }
    }
}
