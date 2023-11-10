using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using TMPro;
using UnityEngine;

namespace CharacterWhitelist
{
    public class CharacterWhitelistUI : MonoBehaviour
    {
        // Button 3 = Cancelled (B)
        // Button 2 = Accepted (A)
        // Button 64 = X
        // Button 48 = Y
        // Button 45 = Left Bumper
        // Button 47 = Right Bumper
        // 64 conflicted on keyboard...
        internal const int AddToRemoveFromListActionID = 1;
        internal const int SwitchListTypeActionID = 48;
        internal const int ClearListActionID = 47;
        internal const int ToggleBaseCharactersActionID = 45;
        internal static CharacterWhitelistUI Instance;
        private TextMeshProUGUI _infoLabel;
        private TextMeshProUGUI _addRemoveLabel;
        private TextMeshProUGUI _switchListTypeLabel;
        private TextMeshProUGUI _clearListLabel;
        private TextMeshProUGUI _toggleBaseCharactersLabel;

        static TextMeshProUGUI MakeLabel(TextMeshProUGUI reference, string name)
        {
            var font = reference.font;
            var fontSize = reference.fontSize;
            var fontMaterial = reference.fontMaterial;

            var labelObj = new GameObject(name);
            var newLabel = labelObj.AddComponent<TextMeshProUGUI>();
            newLabel.font = font;
            newLabel.fontSize = fontSize;
            newLabel.fontMaterial = fontMaterial;
            newLabel.alignment = TextAlignmentOptions.MidlineLeft;
            newLabel.fontStyle = FontStyles.Bold;
            newLabel.outlineWidth = 0.2f;

            return newLabel;
        }

        static TextMeshProUGUI MakeGlyph(TextMeshProUGUI reference, int actionID)
        {
            var label = MakeLabel(reference, "");
            var glyph = label.gameObject.AddComponent<UIButtonGlyphComponent>();
            glyph.inputActionID = actionID;
            glyph.localizedGlyphTextComponent = label;
            glyph.localizedTextComponent = label;
            glyph.enabled = true;
            return label;
        }

        internal static void InitializeUI(GameplayUI gameplayUI)
        {
            if (Instance != null)
                return;
            Instance = new GameObject("Character Whitelist UI Root").AddComponent<CharacterWhitelistUI>();
            Instance.transform.SetParent(gameplayUI.transform.parent.GetComponent<RectTransform>(), false);
            Instance.Init();
            Instance.Deactivate();
        }

        void Init()
        {
            var rectParent = Instance.gameObject.AddComponent<RectTransform>();
            rectParent.anchorMin = Vector2.zero;
            rectParent.anchorMax = Vector2.one;

            var uiManager = Core.Instance.UIManager;
            var labels = uiManager.danceAbilityUI.GetComponentsInChildren<TextMeshProUGUI>(true);
            TextMeshProUGUI referenceText = null;
            foreach (var label in labels)
            {
                if (label.transform.gameObject.name == "DanceSelectConfirmText")
                    referenceText = label;
            }

            if (referenceText == null)
                return;

            var labelLeft = 100f;
            var labelSeparation = -50f;
            var labelBegin = 50f;
            var glyphOffset = 75f;

            _infoLabel = MakeLabel(referenceText, "InfoLabel");
            _infoLabel.text = "Tryce is on the list";
            _infoLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _infoLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _infoLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _infoLabel.rectTransform.anchoredPosition = new Vector2(labelLeft, labelBegin);
            _infoLabel.rectTransform.SetParent(rectParent, false);

            _addRemoveLabel = MakeLabel(referenceText, "AddRemoveLabel");
            _addRemoveLabel.text = "Add to List";
            _addRemoveLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _addRemoveLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _addRemoveLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _addRemoveLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + labelSeparation);
            _addRemoveLabel.rectTransform.SetParent(rectParent, false);

            var glyph = MakeGlyph(referenceText, AddToRemoveFromListActionID);
            glyph.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            glyph.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            glyph.rectTransform.pivot = new Vector2(0f, 1f);
            glyph.rectTransform.anchoredPosition = new Vector2(labelLeft, labelBegin + labelSeparation);
            glyph.rectTransform.SetParent(rectParent, false);


            _switchListTypeLabel = MakeLabel(referenceText, "ListTypeLabel");
            _switchListTypeLabel.text = "List Type: Whitelist";
            _switchListTypeLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _switchListTypeLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _switchListTypeLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _switchListTypeLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + (labelSeparation * 2));
            _switchListTypeLabel.rectTransform.SetParent(rectParent, false);

            glyph = MakeGlyph(referenceText, SwitchListTypeActionID);
            glyph.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            glyph.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            glyph.rectTransform.pivot = new Vector2(0f, 1f);
            glyph.rectTransform.anchoredPosition = new Vector2(labelLeft, labelBegin + (labelSeparation * 2));
            glyph.rectTransform.SetParent(rectParent, false);

            _clearListLabel = MakeLabel(referenceText, "ClearListLabel");
            _clearListLabel.text = "Clear List";
            _clearListLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _clearListLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _clearListLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _clearListLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + (labelSeparation * 3));
            _clearListLabel.rectTransform.SetParent(rectParent, false);

            glyph = MakeGlyph(referenceText, ClearListActionID);
            glyph.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            glyph.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            glyph.rectTransform.pivot = new Vector2(0f, 1f);
            glyph.rectTransform.anchoredPosition = new Vector2(labelLeft, labelBegin + (labelSeparation * 3));
            glyph.rectTransform.SetParent(rectParent, false);

            _toggleBaseCharactersLabel = MakeLabel(referenceText, "BaseCharacterLabel");
            _toggleBaseCharactersLabel.text = "Base characters are exempt";
            _toggleBaseCharactersLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _toggleBaseCharactersLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _toggleBaseCharactersLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _toggleBaseCharactersLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + (labelSeparation * 4));
            _toggleBaseCharactersLabel.rectTransform.SetParent(rectParent, false);

            glyph = MakeGlyph(referenceText, ToggleBaseCharactersActionID);
            glyph.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            glyph.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            glyph.rectTransform.pivot = new Vector2(0f, 1f);
            glyph.rectTransform.anchoredPosition = new Vector2(labelLeft, labelBegin + (labelSeparation * 4));
            glyph.rectTransform.SetParent(rectParent, false);
        }

        public void Activate(Characters currentCharacter)
        {
            gameObject.SetActive(true);
            UpdateLabels(currentCharacter);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void UpdateLabels(Characters currentCharacter)
        {
            if (CharacterWhitelistPlugin.IsCharacterInList(currentCharacter))
            {
                _infoLabel.text = "Character is on the List";
                _addRemoveLabel.text = "Remove from List";
            }
            else
            {
                _infoLabel.text = "Character is NOT on the List";
                _addRemoveLabel.text = "Add to List";
            }

            if (CharacterWhitelistPlugin.ListType == ListType.Whitelist)
                _switchListTypeLabel.text = "List Type: Whitelist";
            else
                _switchListTypeLabel.text = "List Type: Blacklist";

            _clearListLabel.text = $"Clear List ({CharacterWhitelistPlugin.CharacterSet.Count})";

            if (CharacterWhitelistPlugin.AlwaysAllowBaseCharacters)
                _toggleBaseCharactersLabel.text = "Base characters are unaffected";
            else
                _toggleBaseCharactersLabel.text = "Base characters are affected";
        }

        internal static void DestroyUI()
        {
            if (Instance != null)
                GameObject.Destroy(Instance);
            Instance = null;
        }

        public void UpdateUI(CharacterSelect characterSelect)
        {
            var gameInput = characterSelect.gameInput;
            var character = characterSelect.CharactersInCircle[characterSelect.selectionInCircle].character;
            var characterInList = CharacterWhitelistPlugin.IsCharacterInList(character);

            if (gameInput.GetButtonNew(AddToRemoveFromListActionID))
            {
                if (characterInList)
                    CharacterWhitelistPlugin.RemoveCharacterFromList(character);
                else
                    CharacterWhitelistPlugin.AddCharacterToList(character);

                Core.Instance.AudioManager.PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.confirm, 0f);
                UpdateLabels(character);
            }

            if (gameInput.GetButtonNew(SwitchListTypeActionID))
            {
                if (CharacterWhitelistPlugin.ListType == ListType.Whitelist)
                    CharacterWhitelistPlugin.ListType = ListType.Blacklist;
                else
                    CharacterWhitelistPlugin.ListType = ListType.Whitelist;

                Core.Instance.AudioManager.PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.confirm, 0f);
                UpdateLabels(character);
            }

            if (gameInput.GetButtonNew(ClearListActionID))
            {
                CharacterWhitelistPlugin.ClearCharacterList();

                Core.Instance.AudioManager.PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.confirm, 0f);
                UpdateLabels(character);
            }

            if (gameInput.GetButtonNew(ToggleBaseCharactersActionID))
            {
                CharacterWhitelistPlugin.AlwaysAllowBaseCharacters = !CharacterWhitelistPlugin.AlwaysAllowBaseCharacters;
                Core.Instance.AudioManager.PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.confirm, 0f);
                UpdateLabels(character);
            }
        }
    }
}
