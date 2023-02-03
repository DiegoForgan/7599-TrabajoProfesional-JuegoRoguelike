using UnityEditor;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class UICharacterPresets : UIButtonGroupData<CharacterPreset>
    {
        protected override void OnConfirmWindowSelectedYes(int index)
        {
            var preset = _datas[index];
            if (preset != null)
                _currentCharacter.ApplyPreset(preset);
        }

        protected override string GetConfirmationText()
        {
            return "This action will remove all current customizations, are you sure?";
        }

        protected override CharacterPreset GetObjectToSave()
        {
            return _currentCharacter.CreatePreset();
        }

        protected override void ApplyLoadedData(CharacterPreset data)
        {
            _currentCharacter.ApplyPreset(data);
        }
    }
}