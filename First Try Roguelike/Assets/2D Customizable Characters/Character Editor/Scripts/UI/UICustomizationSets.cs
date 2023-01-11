namespace CustomizableCharacters.CharacterEditor.UI
{
    public class UICustomizationSets : UIButtonGroupData<CustomizationSet>
    {
        protected override void OnConfirmWindowSelectedYes(int index)
        {
            var set = _datas[index];
            _currentCharacter.Customizer.ApplySet(set);
        }

        protected override string GetConfirmationText()
        {
            return "All current customizations matching the set will be removed, are you sure?";
        }

        protected override CustomizationSet GetObjectToSave()
        {
            // TODO implement creating sets in playmode
            return null;
        }

        protected override void ApplyLoadedData(CustomizationSet data)
        {
            _currentCharacter.Customizer.ApplySet(data);
        }
    }
}